using System;
using System.Collections.Generic;
using FortDefense.Enemies;
using UnityEngine;

namespace FortDefense.Gameplay
{
    public class EnemyTracker : MonoBehaviour
    {
        public event Action<int> EnemyCountChanged;

        private readonly List<EnemyUnit> _activeEnemies = new List<EnemyUnit>();

        public IReadOnlyList<EnemyUnit> ActiveEnemies
        {
            get { return _activeEnemies; }
        }

        public void Register(EnemyUnit enemy)
        {
            if (enemy == null || _activeEnemies.Contains(enemy))
            {
                return;
            }

            _activeEnemies.Add(enemy);
            EnemyCountChanged?.Invoke(_activeEnemies.Count);
        }

        public void Unregister(EnemyUnit enemy)
        {
            if (enemy == null)
            {
                return;
            }

            if (_activeEnemies.Remove(enemy))
            {
                EnemyCountChanged?.Invoke(_activeEnemies.Count);
            }
        }
    }
}

