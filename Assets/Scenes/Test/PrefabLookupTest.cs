using System;
using System.Collections.Generic;
using Data;
using ECS.ECSBridge;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Scenes.Test
{
    public class PrefabLookupTest : MonoBehaviour
    {
        public PrefabId prefab;
        
        private readonly Dictionary<Guid, Entity> _prefabMap = new();
        private EntityQuery _query;
        
        public void Start()
        {
            var manager = World.DefaultGameObjectInjectionWorld.EntityManager;
            _query = manager.CreateEntityQuery(typeof(PrefabRegistryComponent));
            
        }

        private void Update()
        {
            Debug.Log("Updating....");
            if (_query.CalculateEntityCount() == 0) return;
            var components = _query.ToComponentDataArray<PrefabRegistryComponent>(Allocator.Temp);

            Debug.Log("Prefab: " + prefab.Value);
            
            foreach (var component in components)
            {
                _prefabMap[component.PrefabId] = component.Entity;
                Debug.Log(component.PrefabId);
                Debug.Log(component.Entity);
            }
            
            Debug.Log("Resolved ID: " + _prefabMap[prefab.Value]);
            foreach (var pair in _prefabMap)
            {
                Debug.Log(pair.Key);
                Debug.Log(pair.Value);
            }
            
            var manager = World.DefaultGameObjectInjectionWorld.EntityManager;
            manager.Instantiate(_prefabMap[prefab.Value]);
            manager.Instantiate(_prefabMap[prefab.Value]);
            
            enabled = false;
        }
    }
}