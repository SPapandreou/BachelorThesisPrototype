using Unity.Entities;

namespace ECS.ECSExtensions
{
    public static class WorldExtensions
    {
        public static T GetBlackboardComponent<T>(this World world) where T : unmanaged, IComponentData
        {
            var manager = world.EntityManager;
            return manager.GetComponentData<T>(BlackboardCache.GetBlackboardEntity(manager));
        }

        public static void SetBlackboardComponent<T>(this World world, T componentData)
            where T : unmanaged, IComponentData
        {
            var manager = world.EntityManager;
            manager.SetComponentData<T>(BlackboardCache.GetBlackboardEntity(manager), componentData);
        }

        public static bool HasBlackboardComponent<T>(this World world) where T : unmanaged, IComponentData
        {
            var manager = world.EntityManager;
            return manager.HasComponent<T>(BlackboardCache.GetBlackboardEntity(manager));
        }
    }
}