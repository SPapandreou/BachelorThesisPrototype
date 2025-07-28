using ECS.Components.PlayingArea;
using ECS.ECSExtensions;
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
            
            World.DefaultGameObjectInjectionWorld.SetBlackboardComponent(new PlayingFieldData
            {
                Bounds = _bounds,
            });
        }
    }
}