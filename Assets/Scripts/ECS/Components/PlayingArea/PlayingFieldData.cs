using Unity.Entities;
using UnityEngine;

namespace ECS.Components.PlayingArea
{
    public struct PlayingFieldData : IComponentData
    {
        public Rect Bounds;
    }
}