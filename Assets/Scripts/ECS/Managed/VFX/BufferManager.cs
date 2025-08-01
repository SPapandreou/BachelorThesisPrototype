using System;
using System.Collections.Generic;
using UnityEngine.VFX;

namespace ECS.Managed.VFX
{
    public class BufferManager<T> : IDisposable where T : struct
    {
        private readonly Queue<GraphicsBufferHandle<T>> _bufferQueue = new();

        private readonly List<GraphicsBufferHandle<T>> _ownedBuffers = new();

        public BufferManager(int graphCount, int bufferingModeCount, int bufferCapacity, string bufferProperty,
            string countProperty)
        {
            for (int i = 0; i < graphCount * bufferingModeCount; i++)
            {
                var buffer = new GraphicsBufferHandle<T>(bufferCapacity, bufferProperty, countProperty, this);
                _bufferQueue.Enqueue(buffer);
                _ownedBuffers.Add(buffer);
            }
        }

        public GraphicsBufferHandle<T> GetBuffer(VisualEffect graph)
        {
            if (_bufferQueue.Count == 0)
            {
                throw new InvalidOperationException("Buffer queue is empty");
            }

            var buffer = _bufferQueue.Dequeue();
            buffer.SetGraph(graph);
            return buffer;
        }

        public void ReturnBuffer(GraphicsBufferHandle<T> buffer)
        {
            _bufferQueue.Enqueue(buffer);
        }

        public void Dispose()
        {
            foreach (var buffer in _ownedBuffers)
            {
                buffer.Dispose();
            }
        }
    }
}