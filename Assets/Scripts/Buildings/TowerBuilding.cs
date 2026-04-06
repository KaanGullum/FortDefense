using FortDefense.Data;
using FortDefense.Enemies;
using FortDefense.Gameplay;
using FortDefense.Utilities;
using UnityEngine;

namespace FortDefense.Buildings
{
    public class TowerBuilding : BuildingBase
    {
        private EnemyTracker _enemyTracker;
        private Transform _turretPivot;
        private Transform _muzzle;
        private ResourceBank _resourceBank;
        private float _cooldown;

        public void Initialize(
            BuildingDefinition definition,
            BuildTile tile,
            ResourceBank resourceBank,
            EnemyTracker enemyTracker,
            Transform turretPivot,
            Transform muzzle)
        {
            base.Initialize(definition, tile, resourceBank);
            _enemyTracker = enemyTracker;
            _resourceBank = resourceBank;
            _turretPivot = turretPivot;
            _muzzle = muzzle != null ? muzzle : transform;
        }

        private void Update()
        {
            _cooldown -= Time.deltaTime;

            EnemyUnit target = AcquireTarget();
            if (target == null)
            {
                SetRuntimeStatus("Watching lane");
                return;
            }

            RotateTurret(target.AimPoint);

            if (_cooldown > 0f)
            {
                return;
            }

            if (!TryConsumeAttackSupply())
            {
                _cooldown = 0.25f;
                return;
            }

            Fire(target);
            SetRuntimeStatus("Operational");
            _cooldown = GetAttackInterval();
        }

        private EnemyUnit AcquireTarget()
        {
            EnemyUnit bestTarget = null;
            float bestProgress = -1f;
            float rangeSqr = GetRange() * GetRange();

            for (int index = 0; index < _enemyTracker.ActiveEnemies.Count; index++)
            {
                EnemyUnit enemy = _enemyTracker.ActiveEnemies[index];
                if (enemy == null || enemy.IsDead)
                {
                    continue;
                }

                if ((enemy.transform.position - transform.position).sqrMagnitude > rangeSqr)
                {
                    continue;
                }

                if (enemy.Progress01 > bestProgress)
                {
                    bestProgress = enemy.Progress01;
                    bestTarget = enemy;
                }
            }

            return bestTarget;
        }

        private void RotateTurret(Vector3 targetPosition)
        {
            if (_turretPivot == null)
            {
                return;
            }

            Vector3 flattenedDirection = targetPosition - _turretPivot.position;
            flattenedDirection.y = 0f;

            if (flattenedDirection.sqrMagnitude <= 0.001f)
            {
                return;
            }

            Quaternion targetRotation = Quaternion.LookRotation(flattenedDirection.normalized);
            _turretPivot.rotation = Quaternion.Slerp(_turretPivot.rotation, targetRotation, Time.deltaTime * 12f);
        }

        private void Fire(EnemyUnit target)
        {
            if (Definition.UsesArcProjectile)
            {
                GameObject shell = PrimitiveFactory.CreatePrimitive(
                    "MortarShell",
                    PrimitiveType.Sphere,
                    null,
                    _muzzle.position,
                    new Vector3(0.34f, 0.34f, 0.34f),
                    new Color(0.18f, 0.18f, 0.2f));

                MortarProjectile mortarProjectile = shell.AddComponent<MortarProjectile>();
                mortarProjectile.Initialize(
                    _enemyTracker,
                    _muzzle.position,
                    target.transform.position,
                    Definition.ProjectileSpeed,
                    GetDamage(),
                    Definition.DamageKind,
                    Definition.SplashRadius);
                return;
            }

            GameObject projectileObject = PrimitiveFactory.CreatePrimitive(
                "Projectile",
                PrimitiveType.Sphere,
                null,
                _muzzle.position,
                Definition.VisualStyle == BuildingVisualStyle.CannonTower ? new Vector3(0.42f, 0.42f, 0.42f) : new Vector3(0.24f, 0.24f, 0.24f),
                Definition.VisualStyle == BuildingVisualStyle.CannonTower ? new Color(0.95f, 0.62f, 0.2f) : new Color(0.92f, 0.92f, 0.95f));

            Projectile projectile = projectileObject.AddComponent<Projectile>();
            projectile.Initialize(
                target,
                _enemyTracker,
                Definition.ProjectileSpeed,
                GetDamage(),
                Definition.DamageKind,
                Definition.SplashRadius);
        }

        public override string GetDetailsText()
        {
            float attackInterval = GetAttackInterval();
            float damage = GetDamage();
            float dps = damage / Mathf.Max(0.1f, attackInterval);

            return "Damage " + Mathf.RoundToInt(damage)
                + "  DPS " + dps.ToString("0.0")
                + "\nRange " + GetRange().ToString("0.0")
                + "  Shot " + ResourceFormatting.FormatCost(Definition.AttackCostPerShot);
        }

        protected override void OnUpgraded()
        {
            _cooldown = Mathf.Min(_cooldown, GetAttackInterval());
            SetRuntimeStatus("Upgraded");
        }

        private float GetDamage()
        {
            return Definition.Damage * Mathf.Pow(Definition.DamageMultiplierPerLevel, Level - 1);
        }

        private float GetAttackInterval()
        {
            return Mathf.Max(0.12f, Definition.AttackInterval * Mathf.Pow(Definition.AttackIntervalMultiplierPerLevel, Level - 1));
        }

        private float GetRange()
        {
            return Definition.Range + (Definition.RangeBonusPerLevel * (Level - 1));
        }

        private bool TryConsumeAttackSupply()
        {
            if (Definition.AttackCostPerShot == null || Definition.AttackCostPerShot.Count == 0)
            {
                return true;
            }

            if (_resourceBank.TrySpend(Definition.AttackCostPerShot))
            {
                return true;
            }

            bool needsAmmo = false;
            bool needsPower = false;

            for (int index = 0; index < Definition.AttackCostPerShot.Count; index++)
            {
                ResourceAmount amount = Definition.AttackCostPerShot[index];
                if (_resourceBank.GetAmount(amount.Type) >= amount.Amount)
                {
                    continue;
                }

                if (amount.Type == ResourceType.Ammo)
                {
                    needsAmmo = true;
                }
                else if (amount.Type == ResourceType.Energy)
                {
                    needsPower = true;
                }
            }

            if (needsAmmo && needsPower)
            {
                SetRuntimeStatus("Out of Ammo / No Power");
            }
            else if (needsAmmo)
            {
                SetRuntimeStatus("Out of Ammo");
            }
            else if (needsPower)
            {
                SetRuntimeStatus("No Power");
            }
            else
            {
                SetRuntimeStatus("Supply blocked");
            }

            return false;
        }
    }
}
