using System.Collections.Generic;
using FortDefense.Data;
using FortDefense.Gameplay;
using FortDefense.Utilities;
using UnityEngine;

namespace FortDefense.Buildings
{
    public abstract class BuildingBase : MonoBehaviour
    {
        private readonly Dictionary<ResourceType, int> _investedResources = new Dictionary<ResourceType, int>();

        public BuildingDefinition Definition { get; private set; }
        public BuildTile Tile { get; private set; }
        protected ResourceBank ResourceBank { get; private set; }
        public int Level { get; private set; }
        public string RuntimeStatus { get; protected set; }

        public bool CanUpgrade
        {
            get { return Level < Mathf.Max(1, Definition.MaxLevel); }
        }

        public virtual void Initialize(BuildingDefinition definition, BuildTile tile, ResourceBank resourceBank)
        {
            Definition = definition;
            Tile = tile;
            ResourceBank = resourceBank;
            Level = 1;
            RuntimeStatus = "Operational";
            _investedResources.Clear();
            RecordInvestment(definition.BuildCost);
            RefreshLevelVisual();
        }

        public List<ResourceAmount> GetUpgradeCost()
        {
            if (!CanUpgrade)
            {
                return new List<ResourceAmount>();
            }

            float multiplier = Mathf.Pow(Definition.UpgradeCostMultiplierPerLevel, Level - 1);
            return ResourceListUtility.ScaleCosts(Definition.UpgradeCost, multiplier);
        }

        public bool TryUpgrade()
        {
            if (!CanUpgrade)
            {
                return false;
            }

            List<ResourceAmount> cost = GetUpgradeCost();
            if (!ResourceBank.TrySpend(cost))
            {
                return false;
            }

            RecordInvestment(cost);
            Level++;
            RefreshLevelVisual();
            OnUpgraded();
            return true;
        }

        public List<ResourceAmount> GetSellRefund()
        {
            List<ResourceAmount> invested = ResourceListUtility.ToList(_investedResources);
            return ResourceListUtility.ScaleRefunds(invested, Definition.SellRefundPercent);
        }

        public List<ResourceAmount> Sell()
        {
            List<ResourceAmount> refund = GetSellRefund();
            ResourceBank.AddRange(refund);
            Tile.AssignBuilding(null);
            Destroy(gameObject);
            return refund;
        }

        public virtual string GetDetailsText()
        {
            return string.Empty;
        }

        public virtual string GetStatusText()
        {
            return RuntimeStatus;
        }

        protected virtual void OnUpgraded()
        {
        }

        protected void SetRuntimeStatus(string runtimeStatus)
        {
            RuntimeStatus = runtimeStatus;
        }

        private void RecordInvestment(IList<ResourceAmount> amounts)
        {
            ResourceListUtility.MergeInto(_investedResources, amounts);
        }

        private void RefreshLevelVisual()
        {
            float scale = 1f + ((Level - 1) * 0.08f);
            transform.localScale = new Vector3(scale, scale, scale);
        }
    }
}
