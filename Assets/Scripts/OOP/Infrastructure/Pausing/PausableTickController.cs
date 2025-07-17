using System.Collections.Generic;
using System.Linq;
using VContainer.Unity;

namespace OOP.Infrastructure.Pausing
{
    public class PausableTickController : ITickable
    {

        private readonly List<IPausableTickable> _tickables;
    
        private bool _paused;
        public void SetPaused(bool paused) => _paused = paused;
    
        public PausableTickController(IEnumerable<IPausableTickable> tickables)
        {
            _tickables = tickables as List<IPausableTickable> ?? tickables.ToList();
        }

        public void Register(IPausableTickable tickable)
        {
            _tickables.Add(tickable);
        }

        public void Unregister(IPausableTickable tickable)
        {
            _tickables.Remove(tickable);
        }

        public void Tick()
        {
            if (_paused) return;

            foreach (var tickable in _tickables)
            {
                tickable.Tick();
            }
        }
    }
}