using System.Collections.Generic;
using UnityEngine;

namespace ECS.Managed.Spawning
{
    [CreateAssetMenu(menuName = "SpaceSurvivors/WaveData")]
    public class WaveData : ScriptableObject
    {
        public List<WaveListEntry> enemies;
        public float time;
        public float spawns;
        public float totalEnemies;
        
    }
}