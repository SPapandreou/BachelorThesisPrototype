using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.VFX;


namespace Test
{
    public class TestGraphController : MonoBehaviour
    {
        public VisualEffect vfx;
        public int maxParticles;
        private GraphicsBuffer _initBuffer;

        private void Awake()
        {
            _initBuffer =
                new GraphicsBuffer(GraphicsBuffer.Target.Structured, maxParticles, Marshal.SizeOf<ParticleInitData>());
            vfx.SetGraphicsBuffer("InitBuffer", _initBuffer);
        }

        private void Update()
        {
            int count = Random.Range(1, maxParticles);
            var data = new ParticleInitData[count];
            for (int i = 0; i < count; i++)
            {
                Vector3 position = Random.insideUnitCircle * 30f;
                var size = Random.Range(0.1f, 0.3f);
                data[i] = new ParticleInitData
                {
                    Position = position,
                    Size = size
                };

            }
            
            _initBuffer.SetData(data);

            vfx.SetInt("InitBufferCount", count);
        }

        private void OnDestroy()
        {
            _initBuffer?.Dispose();
        }
    }
}