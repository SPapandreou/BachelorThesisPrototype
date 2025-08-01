using ECS.Data.PlayingArea;
using Unity.Entities;
using UnityEngine;

namespace ECS.Managed.PlayingArea
{
    public class PlayingFieldInjector : MonoBehaviour
    {
        private Rect _bounds;

        private void Awake()
        {
            _bounds = new Rect(transform.position - transform.localScale / 2, transform.localScale);
            var manager = World.DefaultGameObjectInjectionWorld.EntityManager;

            var entity = manager.CreateEntity();
            manager.AddComponent<PlayingFieldData>(entity);
            manager.SetComponentData(entity, new PlayingFieldData
            {
                Bounds = _bounds
            });
        }
    }
}