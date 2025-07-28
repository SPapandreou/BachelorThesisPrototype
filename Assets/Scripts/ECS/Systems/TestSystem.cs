using ECS.Components.Input;
using Latios;
using Latios.Psyshock;
using Latios.Transforms;
using Unity.Entities;
using UnityEngine;
using Collider = Latios.Psyshock.Collider;

namespace ECS.Systems
{
    public partial struct TestSystem : ISystem
    {
        private TransformAspect.Lookup _transformLookup;
        
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PlayerData>();
            _transformLookup = new TransformAspect.Lookup(ref state);
        }

        public void OnUpdate(ref SystemState state)
        {
            state.CompleteDependency();
            
            var world = state.GetLatiosWorldUnmanaged();
            var entity = world.worldBlackboardEntity.GetComponentData<PlayerData>().PlayerEntity;
            var collider = state.EntityManager.GetComponentData<Collider>(entity);
            _transformLookup.Update(ref state);
            var transform = _transformLookup[entity];
            
            PhysicsDebug.DrawCollider(collider, transform.worldTransform, Color.green);
            
        }
    }
}