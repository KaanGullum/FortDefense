using System;
using FortDefense.Buildings;
using FortDefense.Data;
using UnityEngine;

namespace FortDefense.Gameplay
{
    public class PlacementController : MonoBehaviour
    {
        public event Action<BuildingBase> BuildingPlaced;
        public event Action<BuildingBase> BuildingUpgraded;
        public event Action<BuildTile> BuildingSold;
        public event Action<string> PlacementRejected;

        private ResourceBank _resourceBank;
        private EnemyTracker _enemyTracker;
        private TileSelectionController _tileSelectionController;

        public void Initialize(
            ResourceBank resourceBank,
            EnemyTracker enemyTracker,
            TileSelectionController tileSelectionController)
        {
            _resourceBank = resourceBank;
            _enemyTracker = enemyTracker;
            _tileSelectionController = tileSelectionController;
        }

        public bool TryPlaceSelected(BuildingDefinition definition)
        {
            BuildTile tile = _tileSelectionController.SelectedTile;
            if (tile == null)
            {
                PlacementRejected?.Invoke("Select a build tile first.");
                return false;
            }

            if (tile.IsOccupied)
            {
                PlacementRejected?.Invoke(tile.PlacedBuilding.Definition.DisplayName + " already occupies this slot.");
                return false;
            }

            if (!_resourceBank.TrySpend(definition.BuildCost))
            {
                PlacementRejected?.Invoke("Not enough resources for " + definition.DisplayName + ".");
                return false;
            }

            BuildingBase building = BuildingFactory.CreateBuilding(definition, tile, _resourceBank, _enemyTracker);
            tile.AssignBuilding(building);
            BuildingPlaced?.Invoke(building);
            _tileSelectionController.SelectTile(tile);
            return true;
        }

        public bool TryUpgradeSelected()
        {
            BuildTile tile = _tileSelectionController.SelectedTile;
            if (tile == null || !tile.IsOccupied)
            {
                PlacementRejected?.Invoke("Select a placed structure to upgrade.");
                return false;
            }

            BuildingBase building = tile.PlacedBuilding;
            if (!building.CanUpgrade)
            {
                PlacementRejected?.Invoke(building.Definition.DisplayName + " is already max level.");
                return false;
            }

            if (!_resourceBank.CanAfford(building.GetUpgradeCost()))
            {
                PlacementRejected?.Invoke("Not enough resources to upgrade " + building.Definition.DisplayName + ".");
                return false;
            }

            if (!building.TryUpgrade())
            {
                PlacementRejected?.Invoke("Upgrade failed.");
                return false;
            }

            BuildingUpgraded?.Invoke(building);
            _tileSelectionController.SelectTile(tile);
            return true;
        }

        public bool TrySellSelected()
        {
            BuildTile tile = _tileSelectionController.SelectedTile;
            if (tile == null || !tile.IsOccupied)
            {
                PlacementRejected?.Invoke("Select a placed structure to sell.");
                return false;
            }

            tile.PlacedBuilding.Sell();
            BuildingSold?.Invoke(tile);
            _tileSelectionController.SelectTile(tile);
            return true;
        }
    }
}
