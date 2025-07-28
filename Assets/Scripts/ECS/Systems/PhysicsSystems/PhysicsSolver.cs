using ECS.Components.PhysicsComponents;
using Latios;
using Latios.Psyshock;
using Latios.Transforms;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Collider = Latios.Psyshock.Collider;
using Physics = Latios.Psyshock.Physics;

namespace ECS.Systems.PhysicsSystems
{
    [BurstCompile]
    public partial struct PhysicsSolver : ISystem
    {
        private LatiosWorldUnmanaged _latiosWorld;

        private EntityQuery _rigidBodyQuery;

        private BuildCollisionLayerTypeHandles _typeHandles;
        
        private ComponentLookup<RigidBodyData> _rigidBodyLookupRW;
        private ComponentLookup<RigidBodyData> _rigidBodyLookupRO;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _latiosWorld = state.GetLatiosWorldUnmanaged();

            _rigidBodyQuery = state.Fluent().With<RigidBodyData>().PatchQueryForBuildingCollisionLayer().Build();

            _typeHandles = new BuildCollisionLayerTypeHandles(ref state);
            _rigidBodyLookupRO = state.GetComponentLookup<RigidBodyData>(true);
            _rigidBodyLookupRW = state.GetComponentLookup<RigidBodyData>();
            state.RequireForUpdate<RigidBodyData>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var deltaTime = state.WorldUnmanaged.Time.DeltaTime;
            var initialJobHandle = state.Dependency;
            
            _rigidBodyLookupRO.Update(ref state);
            _rigidBodyLookupRW.Update(ref state);

            _typeHandles.Update(ref state);

            var rigidBodyCount = _rigidBodyQuery.CalculateEntityCountWithoutFiltering();
            var rigidBodyColliderArray = CollectionHelper.CreateNativeArray<ColliderBody>(rigidBodyCount,
                state.WorldUpdateAllocator, NativeArrayOptions.UninitializedMemory);
            var rigidBodyAabbArray = CollectionHelper.CreateNativeArray<Aabb>(rigidBodyCount,
                state.WorldUpdateAllocator, NativeArrayOptions.UninitializedMemory);

            var buildRigidBodyJobHandle = new BuildRigidBodiesJob
            {
                DeltaTime = deltaTime,
                ColliderArray = rigidBodyColliderArray,
                AabbArray = rigidBodyAabbArray
            }.ScheduleParallel(_rigidBodyQuery, initialJobHandle);

            buildRigidBodyJobHandle = Physics.BuildCollisionLayer(rigidBodyColliderArray, rigidBodyAabbArray)
                .ScheduleParallel(out var rigidBodyLayer, state.WorldUpdateAllocator, buildRigidBodyJobHandle);

            var pairStream = new PairStream(rigidBodyLayer, state.WorldUpdateAllocator);
            var findBodyBodyProcessor = new FindBodyVsBodyProcessor
            {
                BodyLookup = _rigidBodyLookupRO,
                PairStream = pairStream.AsParallelWriter(),
                DeltaTime = deltaTime,
                InverseDeltaTime = math.rcp(deltaTime)
            };
            var bodyVsBodyJobHandle = Physics.FindPairs(in rigidBodyLayer, in findBodyBodyProcessor)
                .ScheduleParallelUnsafe(buildRigidBodyJobHandle);
            state.Dependency = bodyVsBodyJobHandle;

            var numIterations = 16;
            var solveProcessor = new SolveBodiesProcessor
            {
                RigidBodyLookup = _rigidBodyLookupRW,
                InvNumSolverIterations = math.rcp(numIterations)
            };
            for (int i = 0; i < numIterations; i++)
            {
                state.Dependency = Physics.ForEachPair(in pairStream, in solveProcessor)
                    .ScheduleParallel(state.Dependency);
            }
        }

        [BurstCompile]
        public partial struct BuildRigidBodiesJob : IJobEntity
        {
            [ReadOnly] public float DeltaTime;

            [NativeDisableParallelForRestriction] public NativeArray<ColliderBody> ColliderArray;
            [NativeDisableParallelForRestriction] public NativeArray<Aabb> AabbArray;

            public void Execute(Entity entity, [EntityIndexInQuery] int index, ref RigidBodyData rigidBody,
                in Collider collider, in WorldTransform transform)
            {
                var aabb = Physics.AabbFrom(in collider, in transform.worldTransform);
                var angularExpansion = UnitySim.AngularExpansionFactorFrom(in collider);
                var motionExpansion = new UnitySim.MotionExpansion(in rigidBody.Velocity, DeltaTime, angularExpansion);
                aabb = motionExpansion.ExpandAabb(aabb);
                rigidBody.MotionExpansion = motionExpansion;

                ColliderArray[index] = new ColliderBody
                {
                    collider = collider,
                    transform = transform.worldTransform,
                    entity = entity
                };

                AabbArray[index] = aabb;

                var localCenterOfMass = UnitySim.LocalCenterOfMassFrom(in collider);
                var localInertia = UnitySim.LocalInertiaTensorFrom(in collider, transform.stretch);
                UnitySim.ConvertToWorldMassInertia(in transform.worldTransform, in localInertia, localCenterOfMass,
                    rigidBody.Mass.inverseMass, out rigidBody.Mass, out rigidBody.InertialPoseWorldTransform);
            }
        }

        public struct ContactStreamData
        {
            public UnitySim.ContactJacobianBodyParameters BodyParameters;
            public StreamSpan<UnitySim.ContactJacobianContactParameters> ContactParameters;
            public StreamSpan<float> ContactImpulses;
        }

        public struct FindBodyVsBodyProcessor : IFindPairsProcessor
        {
            [ReadOnly] public ComponentLookup<RigidBodyData> BodyLookup;
            public PairStream.ParallelWriter PairStream;
            public float DeltaTime;
            public float InverseDeltaTime;

            private DistanceBetweenAllCache _distanceBetweenAllCache;

            public void Execute(in FindPairsResult result)
            {
                ref readonly var rigidBodyA = ref BodyLookup.GetRefRO(result.entityA).ValueRO;
                ref readonly var rigidBodyB = ref BodyLookup.GetRefRO(result.entityB).ValueRO;

                var maxDistance =
                    UnitySim.MotionExpansion.GetMaxDistance(in rigidBodyA.MotionExpansion,
                        in rigidBodyB.MotionExpansion);
                Physics.DistanceBetweenAll(result.colliderA, result.transformA, result.colliderB, result.transformB,
                    maxDistance, ref _distanceBetweenAllCache);
                foreach (var distanceResult in _distanceBetweenAllCache)
                {
                    var contacts = UnitySim.ContactsBetween(result.colliderA, result.transformA, result.colliderB,
                        result.transformB, in distanceResult);

                    var coefficientOfFriction =
                        math.sqrt(rigidBodyA.Friction * rigidBodyB.Friction);
                    var coefficientOfRestitution =
                        math.sqrt(rigidBodyA.Restitution * rigidBodyB.Restitution);

                    ref var streamData =
                        ref PairStream.AddPairAndGetRef<ContactStreamData>(result.pairStreamKey, true, true,
                            out var pair);
                    streamData.ContactParameters =
                        pair.Allocate<UnitySim.ContactJacobianContactParameters>(contacts.contactCount,
                            NativeArrayOptions.UninitializedMemory);
                    streamData.ContactImpulses =
                        pair.Allocate<float>(contacts.contactCount, NativeArrayOptions.ClearMemory);

                    UnitySim.BuildJacobian(streamData.ContactParameters.AsSpan(),
                        out streamData.BodyParameters,
                        rigidBodyA.InertialPoseWorldTransform,
                        in rigidBodyA.Velocity,
                        in rigidBodyA.Mass,
                        rigidBodyB.InertialPoseWorldTransform,
                        in rigidBodyB.Velocity,
                        in rigidBodyB.Mass,
                        contacts.contactNormal,
                        contacts.AsSpan(),
                        coefficientOfRestitution,
                        coefficientOfFriction,
                        UnitySim.kMaxDepenetrationVelocityDynamicDynamic,
                        9.81f,
                        DeltaTime,
                        InverseDeltaTime,
                        1);
                }
            }
        }

        struct SolveBodiesProcessor : IForEachPairProcessor
        {
            public PhysicsComponentLookup<RigidBodyData> RigidBodyLookup;
            public float InvNumSolverIterations;

            public void Execute(ref PairStream.Pair pair)
            {
                ref var streamData = ref pair.GetRef<ContactStreamData>();

                ref var rigidBodyA = ref RigidBodyLookup.GetRW(pair.entityA).ValueRW;

                UnitySim.Velocity defaultVelocity = default;
                ref var velocityB = ref defaultVelocity;
                UnitySim.Mass massB = default;

                if (pair.bIsRW)
                {
                    ref var rigidBodyB = ref RigidBodyLookup.GetRW(pair.entityB).ValueRW;
                    velocityB = ref rigidBodyB.Velocity;
                    massB = rigidBodyB.Mass;
                }

                UnitySim.SolveJacobian(ref rigidBodyA.Velocity,
                    in rigidBodyA.Mass,
                    UnitySim.MotionStabilizer.kDefault,
                    ref velocityB,
                    in massB,
                    UnitySim.MotionStabilizer.kDefault,
                    streamData.ContactParameters.AsSpan(),
                    streamData.ContactImpulses.AsSpan(),
                    in streamData.BodyParameters,
                    false,
                    InvNumSolverIterations,
                    out _);
            }
        }
    }
}