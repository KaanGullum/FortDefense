using System.Collections.Generic;
using UnityEngine;

namespace FortDefense.Data
{
    [CreateAssetMenu(menuName = "Fort Defense/Wave Definition", fileName = "WaveDefinition")]
    public class WaveDefinition : ScriptableObject
    {
        public int WaveNumber = 1;
        public float CountdownBeforeWave = 6f;
        public List<WaveSpawnEntry> SpawnEntries = new List<WaveSpawnEntry>();
    }
}

