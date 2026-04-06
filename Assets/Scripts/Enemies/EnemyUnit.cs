using System.Collections;
using System.Collections.Generic;
using FortDefense.Data;
using FortDefense.Gameplay;
using UnityEngine;

namespace FortDefense.Enemies
{
    public class EnemyUnit : MonoBehaviour, IDamageable
    {
        private readonly List<Vector3> _pathPoints = new List<Vector3>();

        private EnemyDefinition _definition;
        private CoreBase _coreBase;
        private EnemyTracker _enemyTracker;
        private Renderer[] _renderers;
        private Color[] _baseColors;
        private Transform _healthFill;

        private int _pathIndex = 1;
        private float _totalPathLength;
        private float _distanceTravelled;
        private float _impactFlashTimer;
        private float _currentHealth;
        private bool _isRegistered;

        public bool IsDead { get; private set; }

        public float Progress01
        {
            get
            {
                if (_totalPathLength <= 0.001f)
                {
                    return 0f;
                }

                return Mathf.Clamp01(_distanceTravelled / _totalPathLength);
            }
        }

        public Vector3 AimPoint
        {
            get { return transform.position + Vector3.up * (1.2f * _definition.Scale); }
        }

        public void Initialize(
            EnemyDefinition definition,
            IList<Vector3> pathPoints,
            CoreBase coreBase,
            EnemyTracker enemyTracker,
            Transform healthFill)
        {
            _definition = definition;
            _coreBase = coreBase;
            _enemyTracker = enemyTracker;
            _healthFill = healthFill;

            _pathPoints.Clear();
            for (int index = 0; index < pathPoints.Count; index++)
            {
                _pathPoints.Add(pathPoints[index]);
            }

            _currentHealth = definition.MaxHealth;
            transform.position = _pathPoints[0];

            for (int index = 1; index < _pathPoints.Count; index++)
            {
                _totalPathLength += Vector3.Distance(_pathPoints[index - 1], _pathPoints[index]);
            }

            _renderers = GetComponentsInChildren<Renderer>();
            _baseColors = new Color[_renderers.Length];
            for (int index = 0; index < _renderers.Length; index++)
            {
                _baseColors[index] = _renderers[index].material.color;
            }

            UpdateHealthBar();

            _enemyTracker.Register(this);
            _isRegistered = true;
        }

        private void Update()
        {
            if (IsDead)
            {
                return;
            }

            UpdateMovement();
            UpdateFlash();
        }

        public void ApplyDamage(float damage, DamageKind damageKind, Vector3 hitPoint)
        {
            if (IsDead)
            {
                return;
            }

            float finalDamage = damage * _definition.GetDamageMultiplier(damageKind);
            _currentHealth -= finalDamage;
            _impactFlashTimer = 0.08f;
            UpdateHealthBar();

            if (_currentHealth <= 0f)
            {
                StartCoroutine(DieRoutine());
            }
        }

        private void UpdateMovement()
        {
            if (_pathIndex >= _pathPoints.Count)
            {
                ReachCore();
                return;
            }

            float remainingStep = _definition.MoveSpeed * Time.deltaTime;

            while (remainingStep > 0f && _pathIndex < _pathPoints.Count)
            {
                Vector3 targetPoint = _pathPoints[_pathIndex];
                Vector3 toTarget = targetPoint - transform.position;
                float distanceToNext = toTarget.magnitude;

                if (distanceToNext <= remainingStep)
                {
                    transform.position = targetPoint;
                    _distanceTravelled += distanceToNext;
                    _pathIndex++;
                    remainingStep -= distanceToNext;
                }
                else
                {
                    Vector3 move = toTarget.normalized * remainingStep;
                    transform.position += move;
                    _distanceTravelled += remainingStep;
                    remainingStep = 0f;
                }
            }

            if (_pathIndex >= _pathPoints.Count)
            {
                ReachCore();
                return;
            }

            Vector3 lookTarget = _pathPoints[_pathIndex] - transform.position;
            lookTarget.y = 0f;
            if (lookTarget.sqrMagnitude > 0.001f)
            {
                transform.forward = Vector3.Lerp(transform.forward, lookTarget.normalized, Time.deltaTime * 10f);
            }
        }

        private void ReachCore()
        {
            if (IsDead)
            {
                return;
            }

            IsDead = true;
            SafeUnregister();
            _coreBase.ApplyCoreDamage(_definition.CoreDamage);
            Destroy(gameObject);
        }

        private IEnumerator DieRoutine()
        {
            IsDead = true;
            SafeUnregister();

            Vector3 initialScale = transform.localScale;
            float elapsed = 0f;
            while (elapsed < 0.18f)
            {
                elapsed += Time.deltaTime;
                transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, elapsed / 0.18f);
                yield return null;
            }

            Destroy(gameObject);
        }

        private void SafeUnregister()
        {
            if (!_isRegistered)
            {
                return;
            }

            _enemyTracker.Unregister(this);
            _isRegistered = false;
        }

        private void UpdateFlash()
        {
            if (_renderers == null || _renderers.Length == 0)
            {
                return;
            }

            _impactFlashTimer -= Time.deltaTime;
            float flashT = Mathf.Clamp01(_impactFlashTimer / 0.08f);
            for (int index = 0; index < _renderers.Length; index++)
            {
                _renderers[index].material.color = Color.Lerp(_baseColors[index], Color.white, flashT);
            }
        }

        private void UpdateHealthBar()
        {
            if (_healthFill == null)
            {
                return;
            }

            float ratio = Mathf.Clamp01(_currentHealth / _definition.MaxHealth);
            _healthFill.localScale = new Vector3(ratio, 1f, 1f);
            _healthFill.localPosition = new Vector3((ratio - 1f) * 0.4f, 0f, 0f);
        }
    }
}

