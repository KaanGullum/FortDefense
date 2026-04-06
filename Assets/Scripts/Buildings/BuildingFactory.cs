using FortDefense.Data;
using FortDefense.Gameplay;
using UnityEngine;

namespace FortDefense.Buildings
{
    public static class BuildingFactory
    {
        public static BuildingBase CreateBuilding(
            BuildingDefinition definition,
            BuildTile tile,
            ResourceBank resourceBank,
            EnemyTracker enemyTracker)
        {
            GameObject root = new GameObject(definition.DisplayName);
            root.transform.SetParent(tile.transform, false);
            root.transform.localPosition = new Vector3(0f, 0.15f + definition.VisualYOffset, 0f);

            BuildingVisualResult visuals = BuildingVisualFactory.BuildVisual(definition, root.transform);

            if (definition.IsDefense)
            {
                TowerBuilding tower = root.AddComponent<TowerBuilding>();
                tower.Initialize(definition, tile, resourceBank, enemyTracker, visuals.TurretPivot, visuals.Muzzle);
                return tower;
            }

            EconomyBuilding economyBuilding = root.AddComponent<EconomyBuilding>();
            economyBuilding.Initialize(definition, tile, resourceBank);
            return economyBuilding;
        }
    }
}
