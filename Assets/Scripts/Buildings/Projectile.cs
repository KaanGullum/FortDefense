using FortDefense.Data;
using FortDefense.Enemies;
using FortDefense.Gameplay;
using FortDefense.Utilities;
using UnityEngine;

namespace FortDefense.Buildings
{
    public class Projectile : MonoBehaviour
    {
        private EnemyUnit _target;
        private EnemyTracker _enemyTracker;
        private DamageKind _damageKind;
        private float _damage;
        private float _speed;
        private float _splashRadius;
        private Vector3 _fallbackTargetPosition;

        public void Initialize(
            EnemyUnit target,
            EnemyTracker enemyTracker,
            float speed,
            float damage,
            DamageKind damageKind,
            float splashRadius)
        {
            _target = target;
            _enemyTracker = enemyTracker;
            _speed = speed;
            _damage = damage;
            _damageKind = damageKind;
            _splashRadius = splashRadius;
            _fallbackTargetPosition = target != null ? target.AimPoint : transform.position;
        }

        private void Update()
        {
            Vector3 targetPosition = _target != null && !_target.IsDead ? _target.AimPoint : _fallbackTargetPosition;
            _fallbackTargetPosition = targetPosition;

            Vector3 direction = targetPosition - transform.position;
            float step = _speed * Time.deltaTime;

            if (direction.magnitude <= step)
            {
                Impact(targetPosition);
                return;
            }

            transform.position += direction.normalized * step;
            transform.forward = direction.normalized;
        }

        private void Impact(Vector3 impactPoint)
        {
            if (_splashRadius > 0.05f)
            {
                for (int index = _enemyTracker.ActiveEnemies.Count - 1; index >= 0; index--)
                {
                    EnemyUnit enemy = _enemyTracker.ActiveEnemies[index];
                    if (enemy == null || enemy.IsDead)
                    {
                        continue;
                    }

                    if ((enemy.transform.position - impactPoint).sqrMagnitude <= _splashRadius * _splashRadius)
                    {
                        enemy.ApplyDamage(_damage, _damageKind, impactPoint);
                    }
                }
            }
            else if (_target != null && !_target.IsDead)
            {
                _target.ApplyDamage(_damage, _damageKind, impactPoint);
            }

            GameObject blast = PrimitiveFactory.CreatePrimitive(
                "Impact",
                PrimitiveType.Sphere,
                null,
                impactPoint,
                new Vector3(0.35f, 0.35f, 0.35f),
                new Color(1f, 0.86f, 0.34f));

            Destroy(blast, 0.18f);
            Destroy(gameObject);
        }
    }
}
