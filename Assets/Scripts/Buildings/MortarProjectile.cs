using FortDefense.Data;
using FortDefense.Enemies;
using FortDefense.Gameplay;
using FortDefense.Utilities;
using UnityEngine;

namespace FortDefense.Buildings
{
    public class MortarProjectile : MonoBehaviour
    {
        private EnemyTracker _enemyTracker;
        private DamageKind _damageKind;
        private float _damage;
        private float _splashRadius;
        private Vector3 _startPoint;
        private Vector3 _destination;
        private float _travelDuration;
        private float _elapsed;

        public void Initialize(
            EnemyTracker enemyTracker,
            Vector3 startPoint,
            Vector3 destination,
            float speed,
            float damage,
            DamageKind damageKind,
            float splashRadius)
        {
            _enemyTracker = enemyTracker;
            _startPoint = startPoint;
            _destination = destination;
            _damage = damage;
            _damageKind = damageKind;
            _splashRadius = splashRadius;
            _travelDuration = Mathf.Max(0.25f, Vector3.Distance(startPoint, destination) / Mathf.Max(0.1f, speed));
            transform.position = startPoint;
        }

        private void Update()
        {
            _elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(_elapsed / _travelDuration);
            Vector3 flatPosition = Vector3.Lerp(_startPoint, _destination, progress);
            float arcHeight = Mathf.Sin(progress * Mathf.PI) * 3f;
            transform.position = flatPosition + Vector3.up * arcHeight;

            if (progress >= 1f)
            {
                Explode();
            }
        }

        private void Explode()
        {
            for (int index = _enemyTracker.ActiveEnemies.Count - 1; index >= 0; index--)
            {
                EnemyUnit enemy = _enemyTracker.ActiveEnemies[index];
                if (enemy == null || enemy.IsDead)
                {
                    continue;
                }

                if ((enemy.transform.position - _destination).sqrMagnitude <= _splashRadius * _splashRadius)
                {
                    enemy.ApplyDamage(_damage, _damageKind, _destination);
                }
            }

            GameObject blast = PrimitiveFactory.CreatePrimitive(
                "MortarBlast",
                PrimitiveType.Sphere,
                null,
                _destination,
                new Vector3(_splashRadius, 0.25f, _splashRadius),
                new Color(1f, 0.48f, 0.22f));

            Destroy(blast, 0.22f);
            Destroy(gameObject);
        }
    }
}
