using FortDefense.Buildings;
using FortDefense.Data;
using UnityEngine;

namespace FortDefense.Gameplay
{
    public class BuildTile : MonoBehaviour
    {
        private static readonly Color SelectedColor = new Color(0.92f, 0.8f, 0.29f);
        private static readonly Color OccupiedColor = new Color(0.23f, 0.57f, 0.36f);

        [SerializeField] private Renderer tileRenderer;
        [SerializeField] private Renderer highlightRenderer;

        private bool _isSelected;

        public Vector2Int GridPosition { get; private set; }
        public BuildTileAffinity Affinity { get; private set; }
        public BuildingBase PlacedBuilding { get; private set; }

        public bool IsOccupied
        {
            get { return PlacedBuilding != null; }
        }

        public void Initialize(
            Vector2Int gridPosition,
            Renderer baseRenderer,
            Renderer selectionRenderer,
            BuildTileAffinity affinity)
        {
            GridPosition = gridPosition;
            tileRenderer = baseRenderer;
            highlightRenderer = selectionRenderer;
            Affinity = affinity;

            if (highlightRenderer != null)
            {
                highlightRenderer.enabled = false;
                highlightRenderer.material.color = SelectedColor;
            }

            RefreshVisual();
        }

        public void SetSelected(bool isSelected)
        {
            _isSelected = isSelected;
            RefreshVisual();
        }

        public void AssignBuilding(BuildingBase building)
        {
            PlacedBuilding = building;
            RefreshVisual();
        }

        public float GetProductionBonusMultiplier(BuildingDefinition definition)
        {
            if (definition == null || definition.Category != BuildingCategory.Economy)
            {
                return 1f;
            }

            switch (Affinity)
            {
                case BuildTileAffinity.OreNode:
                    return definition.VisualStyle == BuildingVisualStyle.Mine ? 1.6f : 1f;
                case BuildTileAffinity.PowerNode:
                    return definition.VisualStyle == BuildingVisualStyle.Generator ? 1.6f : 1f;
                case BuildTileAffinity.IndustryPad:
                    return definition.VisualStyle == BuildingVisualStyle.Smelter || definition.VisualStyle == BuildingVisualStyle.AmmoFactory ? 1.35f : 1f;
                default:
                    return 1f;
            }
        }

        public string GetPlacementHint()
        {
            switch (Affinity)
            {
                case BuildTileAffinity.OreNode:
                    return "Ore Vein: Mine output +60%.";
                case BuildTileAffinity.PowerNode:
                    return "Power Conduit: Generator output +60%.";
                case BuildTileAffinity.IndustryPad:
                    return "Industry Pad: Smelter and Ammo Factory output +35%.";
                default:
                    return "Flexible slot: any structure can be placed here.";
            }
        }

        public string GetActiveBonusHint(BuildingDefinition definition)
        {
            float multiplier = GetProductionBonusMultiplier(definition);
            if (Affinity == BuildTileAffinity.Neutral)
            {
                return GetPlacementHint();
            }

            if (multiplier > 1.01f)
            {
                return "Active node bonus: " + GetPlacementHint();
            }

            return GetPlacementHint() + " Current building receives no node bonus.";
        }

        private void RefreshVisual()
        {
            if (tileRenderer != null)
            {
                Color baseColor = GetAffinityColor();
                tileRenderer.material.color = IsOccupied
                    ? Color.Lerp(baseColor, OccupiedColor, 0.62f)
                    : (_isSelected ? SelectedColor : baseColor);
            }

            if (highlightRenderer != null)
            {
                highlightRenderer.enabled = _isSelected;
            }
        }

        private Color GetAffinityColor()
        {
            switch (Affinity)
            {
                case BuildTileAffinity.OreNode:
                    return new Color(0.29f, 0.5f, 0.74f);
                case BuildTileAffinity.PowerNode:
                    return new Color(0.44f, 0.54f, 0.24f);
                case BuildTileAffinity.IndustryPad:
                    return new Color(0.53f, 0.4f, 0.3f);
                default:
                    return new Color(0.27f, 0.47f, 0.67f);
            }
        }
    }
}
