using ECS.Components;
using Unity.Entities;
using UnityEngine;

namespace ECS.PlayingFieldLogic
{
    public class WorldBoundsInjector : MonoBehaviour
    {
        public Rect GetBounds()
        {
            return new Rect(transform.position - transform.localScale/2, transform.localScale);
        }

        private void Awake()
        {
            var manager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var entity = manager.CreateEntity();
            manager.AddComponentData(entity, new WorldBoundsComponent
            {
                Value = GetBounds()
            });
        }
    }
}