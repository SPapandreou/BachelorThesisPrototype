using System;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.VFX;

namespace ECS.Managed.VFX
{
    public class GraphicsBufferHandle<T> : IGraphicsBufferHandle, IDisposable where T : struct
    {
        private VisualEffect _graph;

        private readonly string _propertyName;
        private readonly string _countPropertyName;

        private readonly unsafe int* _counter;

        private readonly GraphicsBuffer _buffer;
        private readonly int _capacity;

        private readonly BufferManager<T> _manager;

        private bool _isLocked;

        public GraphicsBufferHandle(int capacity, string propertyName, string countPropertyName,
            BufferManager<T> manager)
        {
            _propertyName = propertyName;
            _countPropertyName = countPropertyName;
            _manager = manager;
            _capacity = capacity;

            _buffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, GraphicsBuffer.UsageFlags.LockBufferForWrite,
                capacity, Marshal.SizeOf<T>());
            unsafe
            {
                _counter = (int*)UnsafeUtility.Malloc(sizeof(int), UnsafeUtility.AlignOf<int>(), Allocator.Persistent);
            }
        }

        public NativeArray<T> Lock()
        {
            unsafe
            {
                *_counter = 0;
            }
            _isLocked = true;
            return _buffer.LockBufferForWrite<T>(0, _capacity);
        }

        public unsafe int* GetCounterPtr()
        {
            return _counter;
        }

        public void Unlock()
        {
            if (!_isLocked) return;
            unsafe
            {
                _buffer.UnlockBufferAfterWrite<T>(*_counter);    
            }
            _isLocked = false;
        }

        public void SetGraph(VisualEffect graph)
        {
            _graph = graph;
        }

        public void Upload()
        {
            _graph.SetGraphicsBuffer(_propertyName, _buffer);
            unsafe
            {
                _graph.SetInt(_countPropertyName, *_counter);
            }
            
            _manager.ReturnBuffer(this);
        }


        public void Dispose()
        {
            _buffer.Release();
            _buffer.Dispose();
            unsafe
            {
                UnsafeUtility.Free(_counter, Allocator.Persistent);    
            }
        }
    }
}