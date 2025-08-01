using Unity.Entities;
using Unity.Rendering;

namespace ECS.Data.Visuals
{
    [MaterialProperty("_HitResponse")]
    public struct HitResponseData : IComponentData
    {
        public float Value;
    }
}