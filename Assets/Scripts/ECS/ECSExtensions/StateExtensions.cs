using Unity.Entities;

namespace ECS.ECSExtensions
{
    public static class StateExtensions
    {
        public static T GetBlackboardComponent<T>(this ref SystemState state) where T : unmanaged, IComponentData
        {
            return state.EntityManager.GetComponentData<T>(BlackboardCache.GetBlackboardEntity(ref state));
        }

        public static void SetBlackboardComponent<T>(this ref SystemState state, T componentData)
            where T : unmanaged, IComponentData
        {
            state.EntityManager.SetComponentData<T>(BlackboardCache.GetBlackboardEntity(ref state), componentData);
        }

        public static bool HasBlackboardComponent<T>(this ref SystemState state) where T : unmanaged, IComponentData
        {
            return state.EntityManager.HasComponent<T>(BlackboardCache.GetBlackboardEntity(ref state));
        }
    }
}