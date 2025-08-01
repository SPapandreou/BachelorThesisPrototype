using System;
using ECS.Managed.ECSBridge;

namespace ECS.Managed.Spawning
{
    [Serializable]
    public class WaveListEntry
    {
        public GameObjectId enemyPrefab;
        public int weight;
        public float strength;
    }
}