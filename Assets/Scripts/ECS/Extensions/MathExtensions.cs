using Unity.Mathematics;

namespace ECS.Extensions
{
    public static class MathExtensions
    {
        public static bool Approximately(float a, float b, float epsilon = 1e-6f)
        {
            return math.abs(a - b) <= epsilon * math.max(1.0f, math.max(math.abs(a), math.abs(b)));
        }
    }
}