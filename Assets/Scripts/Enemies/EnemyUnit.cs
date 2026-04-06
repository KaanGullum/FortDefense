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
        private readonly Dictionary<Transform, Vector3> _baseLocalPositions = new Dictionary<Transform, Vector3>();
        private readonly Dictionary<Transform, Quaternion> _baseLocalRotations = new Dictionary<Transform, Quaternion>();
        private readonly Dictionary<Transform, Vector3> _baseLocalScales = new Dictionary<Transform, Vector3>();

        private EnemyDefinition _definition;
        private CoreBase _coreBase;
        private EnemyTracker _enemyTracker;
        private Renderer[] _renderers;
        private Color[] _baseColors;
        private Transform _visualRoot;
        private Transform _healthFill;
        private Transform _needleHead;
        private Transform _needleLegA;
        private Transform _needleLegB;
        private Transform _needleEyeBar;
        private Transform _needle;
        private Transform _crawlerTurret;
        private Transform _crawlerRam;
        private Transform _crawlerTrackLeft;
        private Transform _crawlerTrackRight;
        private Transform _crawlerExhaustLeft;
        private Transform _crawlerExhaustRight;
        private Transform _wardenUpperDisc;
        private Transform _wardenLowerDisc;
        private Transform _wardenFrontLens;
        private Transform _wardenAntenna;
        private Transform _wardenShieldFront;

        private int _pathIndex = 1;
        private float _totalPathLength;
        private float _distanceTravelled;
        private float _impactFlashTimer;
        private float _currentHealth;
        private float _presentationTime;
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
            Transform visualRoot,
            Transform healthFill)
        {
            _definition = definition;
            _coreBase = coreBase;
            _enemyTracker = enemyTracker;
            _visualRoot = visualRoot;
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

            _renderers = _visualRoot != null
                ? _visualRoot.GetComponentsInChildren<Renderer>()
                : GetComponentsInChildren<Renderer>();
            _baseColors = new Color[_renderers.Length];
            for (int index = 0; index < _renderers.Length; index++)
            {
                _baseColors[index] = _renderers[index].material.color;
            }

            CacheAnimationRig();
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
            UpdatePresentation();
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
                IsDead = true;
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

        private void UpdatePresentation()
        {
            if (_visualRoot == null)
            {
                return;
            }

            _presentationTime += Time.deltaTime;

            float hoverWave = Mathf.Sin(_presentationTime * _definition.HoverFrequency + Progress01 * 7f);
            float bobWave = Mathf.Sin(_presentationTime * _definition.BobFrequency + Progress01 * 18f);
            float lift = (hoverWave * _definition.HoverAmplitude) + (Mathf.Abs(bobWave) * _definition.BobAmplitude);
            float pitch = -bobWave * _definition.LeanAmount;
            float roll = Mathf.Cos(_presentationTime * (_definition.HoverFrequency * 0.75f)) * (_definition.LeanAmount * 0.35f);

            _visualRoot.localPosition = GetBasePosition(_visualRoot) + new Vector3(0f, lift, 0f);
            _visualRoot.localRotation = GetBaseRotation(_visualRoot) * Quaternion.Euler(pitch, 0f, roll);

            switch (_definition.VisualStyle)
            {
                case EnemyVisualStyle.NeedleScout:
                    AnimateNeedleScout(bobWave);
                    break;
                case EnemyVisualStyle.SiegeCrawler:
                    AnimateSiegeCrawler(bobWave);
                    break;
                case EnemyVisualStyle.ShieldWarden:
                    AnimateShieldWarden(bobWave);
                    break;
            }
        }

        private void AnimateNeedleScout(float bobWave)
        {
            float stride = Mathf.Sin(_presentationTime * (_definition.BobFrequency + 1.6f));
            SetPartRotation(_needleHead, Quaternion.Euler(0f, Mathf.Sin(_presentationTime * 2.8f) * 16f, 0f));
            SetPartRotation(_needleLegA, Quaternion.Euler(stride * 28f, 0f, 0f));
            SetPartRotation(_needleLegB, Quaternion.Euler(-stride * 28f, 0f, 0f));
            SetPartPosition(_needle, new Vector3(0f, Mathf.Abs(stride) * 0.03f, 0.02f));
            SetPartScale(_needleEyeBar, new Vector3(1f + Mathf.Abs(stride) * 0.14f, 1f, 1f));
        }

        private void AnimateSiegeCrawler(float bobWave)
        {
            float tread = Mathf.Sin(_presentationTime * (_definition.BobFrequency * 0.8f));
            SetPartRotation(_crawlerTurret, Quaternion.Euler(0f, Mathf.Sin(_presentationTime * 1.2f) * 10f, 0f));
            SetPartPosition(_crawlerRam, new Vector3(0f, 0f, Mathf.Abs(tread) * 0.06f));
            SetPartPosition(_crawlerTrackLeft, new Vector3(0f, 0f, tread * 0.08f));
            SetPartPosition(_crawlerTrackRight, new Vector3(0f, 0f, -tread * 0.08f));
            SetPartScale(_crawlerExhaustLeft, new Vector3(1f, 1f + Mathf.Abs(bobWave) * 0.45f, 1f));
            SetPartScale(_crawlerExhaustRight, new Vector3(1f, 1f + Mathf.Abs(bobWave) * 0.45f, 1f));
        }

        private void AnimateShieldWarden(float bobWave)
        {
            float spin = _presentationTime * 120f;
            SetPartRotation(_wardenUpperDisc, Quaternion.Euler(0f, spin, 0f));
            SetPartRotation(_wardenLowerDisc, Quaternion.Euler(0f, -spin * 0.75f, 0f));
            SetPartScale(_wardenFrontLens, Vector3.one * (1f + Mathf.Abs(bobWave) * 0.22f));
            SetPartRotation(_wardenAntenna, Quaternion.Euler(0f, Mathf.Sin(_presentationTime * 2.2f) * 24f, 0f));
            SetPartPosition(_wardenShieldFront, new Vector3(0f, 0f, Mathf.Abs(bobWave) * 0.05f));
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
            SafeUnregister();
            SpawnDebris();

            if (_healthFill != null && _healthFill.parent != null)
            {
                Destroy(_healthFill.parent.gameObject);
            }

            yield return null;
            Destroy(gameObject);
        }

        private void SpawnDebris()
        {
            if (_visualRoot == null || _visualRoot.childCount == 0)
            {
                return;
            }

            Transform debrisRoot = new GameObject(name + "_Debris").transform;
            debrisRoot.position = transform.position;

            List<Transform> fragments = new List<Transform>();
            for (int index = _visualRoot.childCount - 1; index >= 0; index--)
            {
                Transform child = _visualRoot.GetChild(index);
                child.SetParent(debrisRoot, true);
                fragments.Add(child);
            }

            EnemyDebris debris = debrisRoot.gameObject.AddComponent<EnemyDebris>();
            debris.Initialize(fragments, transform.position, transform.forward, _definition);
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

        private void CacheAnimationRig()
        {
            _baseLocalPositions.Clear();
            _baseLocalRotations.Clear();
            _baseLocalScales.Clear();

            RememberPart(_visualRoot);

            _needleHead = FindPart("Head");
            _needleLegA = FindPart("LegA");
            _needleLegB = FindPart("LegB");
            _needleEyeBar = FindPart("EyeBar");
            _needle = FindPart("Needle");
            _crawlerTurret = FindPart("Turret");
            _crawlerRam = FindPart("Ram");
            _crawlerTrackLeft = FindPart("TrackLeft");
            _crawlerTrackRight = FindPart("TrackRight");
            _crawlerExhaustLeft = FindPart("ExhaustLeft");
            _crawlerExhaustRight = FindPart("ExhaustRight");
            _wardenUpperDisc = FindPart("UpperDisc");
            _wardenLowerDisc = FindPart("LowerDisc");
            _wardenFrontLens = FindPart("FrontLens");
            _wardenAntenna = FindPart("Antenna");
            _wardenShieldFront = FindPart("ShieldFront");
        }

        private Transform FindPart(string partName)
        {
            if (_visualRoot == null)
            {
                return null;
            }

            Transform part = _visualRoot.Find(partName);
            RememberPart(part);
            return part;
        }

        private void RememberPart(Transform part)
        {
            if (part == null || _baseLocalPositions.ContainsKey(part))
            {
                return;
            }

            _baseLocalPositions[part] = part.localPosition;
            _baseLocalRotations[part] = part.localRotation;
            _baseLocalScales[part] = part.localScale;
        }

        private Vector3 GetBasePosition(Transform part)
        {
            if (part == null)
            {
                return Vector3.zero;
            }

            return _baseLocalPositions[part];
        }

        private Quaternion GetBaseRotation(Transform part)
        {
            if (part == null)
            {
                return Quaternion.identity;
            }

            return _baseLocalRotations[part];
        }

        private Vector3 GetBaseScale(Transform part)
        {
            if (part == null)
            {
                return Vector3.one;
            }

            return _baseLocalScales[part];
        }

        private void SetPartPosition(Transform part, Vector3 offset)
        {
            if (part == null)
            {
                return;
            }

            part.localPosition = GetBasePosition(part) + offset;
        }

        private void SetPartRotation(Transform part, Quaternion delta)
        {
            if (part == null)
            {
                return;
            }

            part.localRotation = GetBaseRotation(part) * delta;
        }

        private void SetPartScale(Transform part, Vector3 multiplier)
        {
            if (part == null)
            {
                return;
            }

            Vector3 baseScale = GetBaseScale(part);
            part.localScale = new Vector3(baseScale.x * multiplier.x, baseScale.y * multiplier.y, baseScale.z * multiplier.z);
        }
    }

    internal class EnemyDebris : MonoBehaviour
    {
        private readonly List<Transform> _fragments = new List<Transform>();
        private readonly List<Vector3> _velocities = new List<Vector3>();
        private readonly List<Vector3> _spinRates = new List<Vector3>();

        private float _lifetime;
        private float _elapsed;

        public void Initialize(IList<Transform> fragments, Vector3 origin, Vector3 forward, EnemyDefinition definition)
        {
            _lifetime = definition.DebrisLifetime;

            for (int index = 0; index < fragments.Count; index++)
            {
                Transform fragment = fragments[index];
                _fragments.Add(fragment);

                Vector3 radial = fragment.position - origin;
                radial.y = Mathf.Abs(radial.y) + 0.18f;
                if (radial.sqrMagnitude < 0.0001f)
                {
                    radial = Random.onUnitSphere;
                    radial.y = Mathf.Abs(radial.y);
                }

                radial.Normalize();
                Vector3 velocity = (forward * definition.DebrisBurstForce * 0.4f)
                    + (radial * definition.DebrisBurstForce * Random.Range(0.6f, 1.1f));
                _velocities.Add(velocity);

                _spinRates.Add(new Vector3(
                    Random.Range(-definition.DebrisSpinSpeed, definition.DebrisSpinSpeed),
                    Random.Range(-definition.DebrisSpinSpeed, definition.DebrisSpinSpeed),
                    Random.Range(-definition.DebrisSpinSpeed, definition.DebrisSpinSpeed)));
            }
        }

        private void Update()
        {
            _elapsed += Time.deltaTime;
            float fade = Mathf.Clamp01(_elapsed / _lifetime);

            for (int index = 0; index < _fragments.Count; index++)
            {
                Transform fragment = _fragments[index];
                Vector3 velocity = _velocities[index];
                velocity += Vector3.down * 9.5f * Time.deltaTime;
                _velocities[index] = velocity;

                fragment.position += velocity * Time.deltaTime;
                fragment.Rotate(_spinRates[index] * Time.deltaTime, Space.Self);
                fragment.localScale = Vector3.Lerp(fragment.localScale, Vector3.zero, fade * 0.18f);
            }

            if (_elapsed >= _lifetime)
            {
                Destroy(gameObject);
            }
        }
    }
}
