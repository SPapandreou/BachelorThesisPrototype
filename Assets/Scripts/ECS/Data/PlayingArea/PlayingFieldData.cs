using Unity.Entities;
using UnityEngine;

namespace ECS.Data.PlayingArea
{
    public struct PlayingFieldData : IComponentData
    {
        public Rect Bounds;
    }
}