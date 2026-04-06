using System.Collections.Generic;
using FortDefense.Data;
using FortDefense.Gameplay;
using FortDefense.Utilities;
using UnityEngine;

namespace FortDefense.Buildings
{
    public class EconomyBuilding : BuildingBase
    {
        private ResourceBank _resourceBank;
        private float _timer;

        public void Initialize(BuildingDefinition definition, BuildTile tile, ResourceBank resourceBank)
        {
            base.Initialize(definition, tile, resourceBank);
            _resourceBank = resourceBank;
            _timer = definition.ProductionInterval;
        }

        private void Update()
        {
            _timer -= Time.deltaTime;
            if (_timer > 0f)
            {
                return;
            }

            _timer = Definition.ProductionInterval;

            if (!_resourceBank.CanAfford(Definition.ConsumptionPerCycle))
            {
                SetRuntimeStatus("Blocked: Need " + DescribeInputShortage());
                return;
            }

            if (Definition.ConsumptionPerCycle.Count > 0)
            {
                _resourceBank.TrySpend(Definition.ConsumptionPerCycle);
            }

            _resourceBank.AddRange(GetProducedResources());
            SetRuntimeStatus("Online");
        }

        public override string GetDetailsText()
        {
            string output = ResourceFormatting.FormatCost(GetProducedResources());
            string input = Definition.ConsumptionPerCycle.Count > 0
                ? ResourceFormatting.FormatCost(Definition.ConsumptionPerCycle)
                : "No input";

            float bonus = GetCurrentProductionMultiplier();
            string bonusText = bonus > 1.01f
                ? "  Node x" + bonus.ToString("0.00")
                : string.Empty;

            return "Cycle " + Definition.ProductionInterval.ToString("0.0") + "s\nOutput " + output + bonusText + "\nInput " + input;
        }

        protected override void OnUpgraded()
        {
            SetRuntimeStatus("Upgraded");
        }

        private List<ResourceAmount> GetProducedResources()
        {
            return ResourceListUtility.ScaleProduction(Definition.ProductionPerCycle, GetCurrentProductionMultiplier());
        }

        private float GetCurrentProductionMultiplier()
        {
            return Tile.GetProductionBonusMultiplier(Definition) * Mathf.Pow(Definition.ProductionMultiplierPerLevel, Level - 1);
        }

        private string DescribeInputShortage()
        {
            for (int index = 0; index < Definition.ConsumptionPerCycle.Count; index++)
            {
                ResourceAmount amount = Definition.ConsumptionPerCycle[index];
                if (_resourceBank.GetAmount(amount.Type) < amount.Amount)
                {
                    return ResourceFormatting.GetShortLabel(amount.Type);
                }
            }

            return "supply";
        }
    }
}
