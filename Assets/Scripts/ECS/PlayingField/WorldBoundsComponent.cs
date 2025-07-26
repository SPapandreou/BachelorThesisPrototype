using Unity.Entities;
using UnityEngine;

namespace ECS.Components
{
    public struct WorldBoundsComponent : IComponentData
    {
        public Rect Value;
    }
}