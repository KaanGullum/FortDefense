using System;
using UnityEngine;

namespace FortDefense.Data
{
    [Serializable]
    public class WaveSpawnEntry
    {
        public EnemyDefinition Enemy;
        [Min(1)] public int Count = 5;
        [Min(0.1f)] public float SpawnInterval = 0.7f;
        [Min(0f)] public float StartDelay = 0f;
    }
}

