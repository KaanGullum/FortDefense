using System.Collections.Generic;
using UnityEngine;

namespace FortDefense.Data
{
    [CreateAssetMenu(menuName = "Fort Defense/Building Definition", fileName = "BuildingDefinition")]
    public class BuildingDefinition : ScriptableObject
    {
        [Header("Identity")]
        public string Id = "building";
        public string DisplayName = "Building";
        public BuildingCategory Category = BuildingCategory.Defense;
        public BuildingVisualStyle VisualStyle = BuildingVisualStyle.GunTower;

        [Header("Costs")]
        public List<ResourceAmount> BuildCost = new List<ResourceAmount>();

        [Header("Progression")]
        public int MaxLevel = 3;
        public List<ResourceAmount> UpgradeCost = new List<ResourceAmount>();
        public float UpgradeCostMultiplierPerLevel = 1.3f;
        [Range(0.1f, 1f)] public float SellRefundPercent = 0.6f;

        [Header("Presentation")]
        [Range(0.35f, 1.5f)] public float VisualFootprintScale = 0.72f;
        [Range(0.35f, 1.5f)] public float VisualHeightScale = 0.78f;
        [Range(-0.2f, 0.6f)] public float VisualYOffset = 0f;

        [Header("Combat")]
        public DamageKind DamageKind = DamageKind.Light;
        public float Damage = 5f;
        public float AttackInterval = 1f;
        public float Range = 4f;
        public float ProjectileSpeed = 10f;
        public float SplashRadius = 0f;
        public bool UsesArcProjectile;
        public List<ResourceAmount> AttackCostPerShot = new List<ResourceAmount>();
        public float DamageMultiplierPerLevel = 1.25f;
        public float AttackIntervalMultiplierPerLevel = 0.92f;
        public float RangeBonusPerLevel = 0.45f;

        [Header("Economy")]
        public List<ResourceAmount> ConsumptionPerCycle = new List<ResourceAmount>();
        public List<ResourceAmount> ProductionPerCycle = new List<ResourceAmount>();
        public float ProductionInterval = 2.5f;
        public float ProductionMultiplierPerLevel = 1.35f;

        public bool IsDefense
        {
            get { return Category == BuildingCategory.Defense; }
        }
    }
}
