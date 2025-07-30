using System;
using UnityEngine;
using Hash128 = Unity.Entities.Hash128;

namespace ECS.Managed.ECSBridge
{
    public class GameObjectId : MonoBehaviour
    {
        public bool initialized;
        public Hash128 hash;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (initialized) return;

            Guid guid = Guid.NewGuid();
            hash = new Hash128(guid.ToString("N"));

            initialized = true;
        }
#endif
    }
}