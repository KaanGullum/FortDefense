using System;
using System.Collections;
using System.Collections.Generic;
using FortDefense.Data;
using FortDefense.Enemies;
using FortDefense.Gameplay;
using UnityEngine;

namespace FortDefense.Waves
{
    public class WaveManager : MonoBehaviour
    {
        public event Action<int, int> WaveStarted;
        public event Action<int, float> WaveCountdownUpdated;
        public event Action AllWavesCompleted;

        private readonly List<WaveDefinition> _waves = new List<WaveDefinition>();
        private readonly List<Vector3> _pathPoints = new List<Vector3>();

        private EnemyTracker _enemyTracker;
        private CoreBase _coreBase;
        private Transform _enemyRoot;

        public int CurrentWaveNumber { get; private set; }
        public bool HasFinishedAllWaves { get; private set; }

        public void Initialize(
            IList<WaveDefinition> waves,
            IList<Vector3> pathPoints,
            EnemyTracker enemyTracker,
            CoreBase coreBase,
            Transform enemyRoot)
        {
            _waves.Clear();
            _pathPoints.Clear();

            for (int index = 0; index < waves.Count; index++)
            {
                _waves.Add(waves[index]);
            }

            for (int index = 0; index < pathPoints.Count; index++)
            {
                _pathPoints.Add(pathPoints[index]);
            }

            _enemyTracker = enemyTracker;
            _coreBase = coreBase;
            _enemyRoot = enemyRoot;
        }

        public void Begin()
        {
            StopAllCoroutines();
            StartCoroutine(RunWaves());
        }

        private IEnumerator RunWaves()
        {
            if (_waves.Count == 0)
            {
                HasFinishedAllWaves = true;
                AllWavesCompleted?.Invoke();
                yield break;
            }

            for (int waveIndex = 0; waveIndex < _waves.Count; waveIndex++)
            {
                WaveDefinition wave = _waves[waveIndex];
                CurrentWaveNumber = wave.WaveNumber;

                float countdown = wave.CountdownBeforeWave;
                while (countdown > 0f)
                {
                    WaveCountdownUpdated?.Invoke(CurrentWaveNumber, countdown);
                    countdown -= Time.deltaTime;
                    yield return null;
                }

                WaveCountdownUpdated?.Invoke(CurrentWaveNumber, 0f);
                WaveStarted?.Invoke(CurrentWaveNumber, _waves.Count);

                for (int entryIndex = 0; entryIndex < wave.SpawnEntries.Count; entryIndex++)
                {
                    WaveSpawnEntry entry = wave.SpawnEntries[entryIndex];

                    if (entry.StartDelay > 0f)
                    {
                        yield return new WaitForSeconds(entry.StartDelay);
                    }

                    for (int count = 0; count < entry.Count; count++)
                    {
                        EnemyFactory.CreateEnemy(entry.Enemy, _pathPoints, _coreBase, _enemyTracker, _enemyRoot);
                        yield return new WaitForSeconds(entry.SpawnInterval);
                    }
                }

                while (_enemyTracker.ActiveEnemies.Count > 0)
                {
                    yield return null;
                }
            }

            HasFinishedAllWaves = true;
            AllWavesCompleted?.Invoke();
        }
    }
}
