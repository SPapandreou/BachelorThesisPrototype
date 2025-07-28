using ECS.Components.PlayingArea;
using Latios;
using Unity.Entities;
using UnityEngine;

namespace ECS.Managed.PlayingArea
{
    public class PlayingFieldInjector : MonoBehaviour
    {
        private Rect _bounds;
        
        private void Awake()
        {
            _bounds = new Rect(transform.position - transform.localScale/2, transform.localScale);
            var world = World.DefaultGameObjectInjectionWorld as LatiosWorld;
            world!.worldBlackboardEntity.AddComponentData(new PlayingFieldData
            {
                Bounds = _bounds,
            });
        }
    }
}