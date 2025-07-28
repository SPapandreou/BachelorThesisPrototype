using Unity.Entities;

namespace ECS.ECSExtensions
{
    public static class BlackboardCache
    {
        private static bool _initialized = false;
        private static Entity _blackboardEntity;

        public static Entity GetBlackboardEntity(ref SystemState state)
        {
            return GetBlackboardEntity(state.EntityManager);
        }

        public static Entity GetBlackboardEntity(EntityManager manager)
        {
            if (_initialized) return _blackboardEntity;
            
            var query =
                manager.CreateEntityQuery(ComponentType.ReadOnly<BlackboardTag>());

            if (query.CalculateEntityCount() == 0)
            {
                _blackboardEntity = manager.CreateEntity();
                manager.AddComponent<BlackboardTag>(_blackboardEntity);
            }
            else
            {
                _blackboardEntity = query.GetSingletonEntity();
            }
            
            _initialized = true;
            return _blackboardEntity;
        }
    }
}