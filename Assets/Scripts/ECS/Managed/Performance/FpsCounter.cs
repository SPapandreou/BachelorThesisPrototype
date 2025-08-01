using UnityEngine;

namespace ECS.Managed.Performance
{
    public class FpsCounter : MonoBehaviour
    {
        public float updateInterval = 0.5f;

        private float _lastUpdateTime;
        private float _fps;
        private int _totalFrames;
        private int _intervalFrames;

        private void Start()
        {
            _lastUpdateTime = Time.unscaledTime;
            _totalFrames = 0;
            _fps = 0;
        }

        private void Update()
        {
            _totalFrames++;
            _intervalFrames++;
            float elapsed = Time.unscaledTime - _lastUpdateTime;

            if (elapsed >= updateInterval)
            {
                _fps = _intervalFrames / elapsed;
                _intervalFrames = 0;
                _lastUpdateTime = Time.unscaledTime;
            }
        }

        public float GetCurrentFps()
        {
            return _fps;
        }

        public float GetAverageFps()
        {
            return _totalFrames / Time.unscaledTime;
        }

    }
}