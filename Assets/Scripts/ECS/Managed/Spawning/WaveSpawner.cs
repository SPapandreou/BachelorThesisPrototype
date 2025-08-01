using System.Collections.Generic;
using System.Linq;
using ECS.Data.Spawning;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using Hash128 = Unity.Entities.Hash128;

namespace ECS.Managed.Spawning
{
    public class WaveSpawner : MonoBehaviour
    {
        public List<WaveData> waves = new();

        private readonly Queue<WaveData> _waveQueue = new();
        
        private EntityQuery _queueQuery;
        private EntityQuery _bufferQuery;

        private WaveData _currentWave;
        private float _time;
        private float _interval;
        private float _remainingSpawns;

        private void Awake()
        {
            _queueQuery = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntityQuery(
                ComponentType.ReadWrite<SpawnQueue>());
            _bufferQuery = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntityQuery(
                ComponentType.ReadWrite<SpawnRequest>());
        }

        private void Update()
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            
            if (_queueQuery.CalculateEntityCount() == 0)
            {
                var entity = entityManager.CreateEntity();
                entityManager.AddComponent<SpawnQueue>(entity);
                entityManager.AddBuffer<SpawnRequest>(entity);
            }
            
            var spawnQueue = _queueQuery.GetSingleton<SpawnQueue>();
            var spawnBuffer = _bufferQuery.GetSingletonBuffer<SpawnRequest>();
            
            spawnQueue.WriteDependency.Complete();
            spawnQueue.WriteDependency = new JobHandle();
            _queueQuery.SetSingleton(spawnQueue);

            spawnBuffer.Clear();
            
            if (_waveQueue.Count == 0)
            {
                ScheduleWaves();
            }

            if (_remainingSpawns == 0)
            {
                ScheduleNextWave();
            }
            
            _time += Time.deltaTime;

            if (_time < _interval) return;

            ScheduleSpawns(ref spawnBuffer);
            
            _remainingSpawns--;
            _time = 0;
        }

        private void ScheduleWaves()
        {
            foreach (var wave in waves)
            {
                _waveQueue.Enqueue(wave);
            }
        }

        private void ScheduleNextWave()
        {
            _currentWave =  _waveQueue.Dequeue();
            _interval = _currentWave.time / _currentWave.spawns;
            _time = 0;
            _remainingSpawns = _currentWave.spawns;
        }

        private void ScheduleSpawns(ref DynamicBuffer<SpawnRequest> spawnBuffer)
        {
            var counts = new Dictionary<Hash128, int>();

            foreach (var enemy in _currentWave.enemies)
            {
                counts[enemy.enemyPrefab.hash] = 0;
            }
            
            var totalWeight = _currentWave.enemies.Sum(x => x.weight);
            for (int i = 0; i < _currentWave.totalEnemies / _currentWave.spawns; i++)
            {
                int random = Random.Range(0, totalWeight);
                foreach (var enemy in _currentWave.enemies)
                {
                    if (random < enemy.weight)
                    {
                        counts[enemy.enemyPrefab.hash]++;
                        break;
                    }

                    random -= enemy.weight;
                }
            }

            foreach (var (hash, count) in counts)
            {
                if (count == 0) continue;
                spawnBuffer.Add(new SpawnRequest
                {
                    Amount = count,
                    PrefabId = hash
                });
            }
        }
    }
}