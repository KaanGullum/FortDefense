using System;
using System.Collections;
using System.Collections.Generic;
using FortDefense.Buildings;
using FortDefense.Data;
using FortDefense.Gameplay;
using FortDefense.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace FortDefense.UI
{
    public class BattleUIController : MonoBehaviour
    {
        private readonly Dictionary<ResourceType, Text> _resourceTexts = new Dictionary<ResourceType, Text>();
        private readonly Dictionary<BuildingDefinition, Button> _buildButtons = new Dictionary<BuildingDefinition, Button>();
        private readonly Dictionary<BuildingDefinition, Text> _buildButtonTexts = new Dictionary<BuildingDefinition, Text>();

        private ResourceBank _resourceBank;
        private PlacementController _placementController;
        private BuildTile _currentSelectedTile;
        private GameObject _buildMenuPanel;
        private GameObject _selectedBuildingPanel;
        private GameObject _endPanel;
        private Text _tileInfoText;
        private Text _tileBonusText;
        private Text _selectedBuildingTitleText;
        private Text _selectedBuildingDetailsText;
        private Text _selectedBuildingStatusText;
        private Text _selectedBuildingBonusText;
        private Text _waveText;
        private Text _countdownText;
        private Text _coreHealthText;
        private Text _statusText;
        private Text _endTitleText;
        private Text _endBodyText;
        private Button _upgradeButton;
        private Button _sellButton;
        private Button _retryButton;
        private Button _menuButton;
        private Text _upgradeButtonText;
        private Text _sellButtonText;
        private Coroutine _statusRoutine;

        public void Initialize(
            IList<BuildingDefinition> buildingDefinitions,
            ResourceBank resourceBank,
            PlacementController placementController)
        {
            _resourceBank = resourceBank;
            _placementController = placementController;

            UiFactory.EnsureEventSystem();
            BuildLayout(buildingDefinitions);
            RefreshResources();
        }

        private void Update()
        {
            if (_currentSelectedTile != null && _currentSelectedTile.IsOccupied && _selectedBuildingPanel != null && _selectedBuildingPanel.activeSelf)
            {
                UpdateSelectedBuildingPanel();
            }
        }

        public void RefreshResources()
        {
            Array values = Enum.GetValues(typeof(ResourceType));
            for (int index = 0; index < values.Length; index++)
            {
                ResourceType type = (ResourceType)values.GetValue(index);
                Text label;
                if (_resourceTexts.TryGetValue(type, out label))
                {
                    label.text = ResourceFormatting.GetShortLabel(type) + "\n" + _resourceBank.GetAmount(type);
                }
            }

            UpdateBuildButtons();
            UpdateSelectedBuildingPanel();
        }

        public void RefreshCoreHealth(int current, int max)
        {
            _coreHealthText.text = "Core " + current + "/" + max;
        }

        public void RefreshWaveStarted(int waveNumber, int totalWaves)
        {
            _waveText.text = "Wave " + waveNumber + " / " + totalWaves;
            _countdownText.text = "Incoming wave active";
        }

        public void RefreshWaveCountdown(int nextWaveNumber, float secondsRemaining)
        {
            if (secondsRemaining <= 0.05f)
            {
                _countdownText.text = "Incoming wave active";
            }
            else if (secondsRemaining <= 3f)
            {
                _countdownText.text = "Incoming wave " + nextWaveNumber + " in " + Mathf.CeilToInt(secondsRemaining) + "s";
            }
            else
            {
                _countdownText.text = "Next wave " + nextWaveNumber + " in " + Mathf.CeilToInt(secondsRemaining) + "s";
            }
        }

        public void ShowTileSelection(BuildTile tile)
        {
            _currentSelectedTile = tile;

            if (tile == null)
            {
                HideSelection();
                return;
            }

            if (tile.IsOccupied)
            {
                _buildMenuPanel.SetActive(false);
                _selectedBuildingPanel.SetActive(true);
                UpdateSelectedBuildingPanel();
            }
            else
            {
                _selectedBuildingPanel.SetActive(false);
                _buildMenuPanel.SetActive(true);
                _tileInfoText.text = "Selected slot: choose a structure.";
                _tileBonusText.text = tile.GetPlacementHint();
            }

            UpdateBuildButtons();
        }

        public void HideSelection()
        {
            _currentSelectedTile = null;

            if (_buildMenuPanel != null)
            {
                _buildMenuPanel.SetActive(false);
            }

            if (_selectedBuildingPanel != null)
            {
                _selectedBuildingPanel.SetActive(false);
            }

            UpdateBuildButtons();
        }

        public void ShowStatus(string message, Color color, float duration = 2f)
        {
            if (_statusRoutine != null)
            {
                StopCoroutine(_statusRoutine);
            }

            _statusText.color = color;
            _statusText.text = message;
            _statusRoutine = StartCoroutine(ClearStatusAfter(duration));
        }

        public void ShowEndScreen(bool victory, Action onRetry, Action onMenu)
        {
            _endPanel.SetActive(true);
            _buildMenuPanel.SetActive(false);
            _selectedBuildingPanel.SetActive(false);

            _endTitleText.text = victory ? "Victory" : "Core Breached";
            _endBodyText.text = victory
                ? "The supply chain held. Your road-and-slots defense survived all four waves."
                : "The core fell. Try stabilizing energy and ammo before scaling heavier defenses.";

            _retryButton.onClick.RemoveAllListeners();
            _retryButton.onClick.AddListener(() => onRetry());

            _menuButton.onClick.RemoveAllListeners();
            _menuButton.onClick.AddListener(() => onMenu());
        }

        private IEnumerator ClearStatusAfter(float duration)
        {
            yield return new WaitForSecondsRealtime(duration);
            _statusText.text = string.Empty;
            _statusRoutine = null;
        }

        private void BuildLayout(IList<BuildingDefinition> buildingDefinitions)
        {
            Canvas canvas = UiFactory.CreateCanvas("BattleCanvas");
            canvas.transform.SetParent(transform, false);

            GameObject topBar = UiFactory.CreatePanel(
                "TopBar",
                canvas.transform,
                new Vector2(0f, 1f),
                new Vector2(1f, 1f),
                new Vector2(16f, -112f),
                new Vector2(-16f, -16f),
                new Color(0.05f, 0.08f, 0.13f, 0.82f));

            CreateResourceRow(topBar.transform);

            _waveText = UiFactory.CreateText("WaveText", topBar.transform, "Wave 1 / 4", 30, TextAnchor.UpperCenter, Color.white);
            RectTransform waveRect = _waveText.GetComponent<RectTransform>();
            waveRect.anchorMin = new Vector2(0.5f, 0f);
            waveRect.anchorMax = new Vector2(0.5f, 1f);
            waveRect.sizeDelta = new Vector2(320f, 36f);
            waveRect.anchoredPosition = new Vector2(0f, -22f);

            _countdownText = UiFactory.CreateText("CountdownText", topBar.transform, "Prepare defenses", 22, TextAnchor.UpperCenter, new Color(0.88f, 0.88f, 0.92f));
            RectTransform countdownRect = _countdownText.GetComponent<RectTransform>();
            countdownRect.anchorMin = new Vector2(0.5f, 0f);
            countdownRect.anchorMax = new Vector2(0.5f, 1f);
            countdownRect.sizeDelta = new Vector2(420f, 28f);
            countdownRect.anchoredPosition = new Vector2(0f, -58f);

            _coreHealthText = UiFactory.CreateText("CoreText", topBar.transform, "Core 25/25", 28, TextAnchor.UpperRight, Color.white);
            RectTransform coreRect = _coreHealthText.GetComponent<RectTransform>();
            coreRect.anchorMin = new Vector2(1f, 0f);
            coreRect.anchorMax = new Vector2(1f, 1f);
            coreRect.sizeDelta = new Vector2(220f, 36f);
            coreRect.anchoredPosition = new Vector2(-132f, -22f);

            _statusText = UiFactory.CreateText("StatusText", canvas.transform, string.Empty, 24, TextAnchor.MiddleCenter, new Color(0.97f, 0.87f, 0.34f));
            RectTransform statusRect = _statusText.GetComponent<RectTransform>();
            statusRect.anchorMin = new Vector2(0.5f, 1f);
            statusRect.anchorMax = new Vector2(0.5f, 1f);
            statusRect.sizeDelta = new Vector2(920f, 36f);
            statusRect.anchoredPosition = new Vector2(0f, -132f);

            BuildConstructionPanel(canvas.transform, buildingDefinitions);
            BuildSelectedBuildingPanel(canvas.transform);
            BuildEndPanel(canvas.transform);
        }

        private void BuildConstructionPanel(Transform parent, IList<BuildingDefinition> buildingDefinitions)
        {
            _buildMenuPanel = UiFactory.CreatePanel(
                "BuildMenu",
                parent,
                new Vector2(0.5f, 0f),
                new Vector2(0.5f, 0f),
                new Vector2(-470f, 24f),
                new Vector2(470f, 262f),
                new Color(0.06f, 0.09f, 0.12f, 0.92f));

            _tileInfoText = UiFactory.CreateText("TileInfo", _buildMenuPanel.transform, "Tap a build slot to deploy a structure.", 24, TextAnchor.UpperLeft, Color.white);
            RectTransform tileInfoRect = _tileInfoText.GetComponent<RectTransform>();
            tileInfoRect.anchorMin = new Vector2(0f, 1f);
            tileInfoRect.anchorMax = new Vector2(1f, 1f);
            tileInfoRect.offsetMin = new Vector2(20f, -54f);
            tileInfoRect.offsetMax = new Vector2(-20f, -14f);

            _tileBonusText = UiFactory.CreateText("TileBonus", _buildMenuPanel.transform, string.Empty, 18, TextAnchor.UpperLeft, new Color(0.84f, 0.88f, 0.92f));
            RectTransform bonusRect = _tileBonusText.GetComponent<RectTransform>();
            bonusRect.anchorMin = new Vector2(0f, 1f);
            bonusRect.anchorMax = new Vector2(1f, 1f);
            bonusRect.offsetMin = new Vector2(20f, -86f);
            bonusRect.offsetMax = new Vector2(-20f, -48f);

            GameObject buttonGridObject = new GameObject("ButtonGrid", typeof(RectTransform), typeof(GridLayoutGroup));
            buttonGridObject.transform.SetParent(_buildMenuPanel.transform, false);
            RectTransform buttonGridRect = buttonGridObject.GetComponent<RectTransform>();
            buttonGridRect.anchorMin = new Vector2(0.5f, 0f);
            buttonGridRect.anchorMax = new Vector2(0.5f, 0f);
            buttonGridRect.sizeDelta = new Vector2(880f, 138f);
            buttonGridRect.anchoredPosition = new Vector2(0f, 88f);

            GridLayoutGroup gridLayout = buttonGridObject.GetComponent<GridLayoutGroup>();
            gridLayout.cellSize = new Vector2(210f, 62f);
            gridLayout.spacing = new Vector2(10f, 10f);
            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.constraintCount = 4;

            for (int index = 0; index < buildingDefinitions.Count; index++)
            {
                BuildingDefinition definition = buildingDefinitions[index];
                Text label;
                Button button = UiFactory.CreateButton(
                    definition.DisplayName + "Button",
                    buttonGridObject.transform,
                    BuildButtonLabel(definition),
                    () => OnBuildPressed(definition),
                    out label);

                label.fontSize = 18;
                _buildButtons[definition] = button;
                _buildButtonTexts[definition] = label;
            }

            _buildMenuPanel.SetActive(false);
        }

        private void BuildSelectedBuildingPanel(Transform parent)
        {
            _selectedBuildingPanel = UiFactory.CreatePanel(
                "SelectedBuildingPanel",
                parent,
                new Vector2(1f, 0f),
                new Vector2(1f, 0f),
                new Vector2(-412f, 24f),
                new Vector2(-24f, 314f),
                new Color(0.06f, 0.09f, 0.12f, 0.92f));

            _selectedBuildingTitleText = UiFactory.CreateText("SelectedTitle", _selectedBuildingPanel.transform, "Tower Lvl 1", 28, TextAnchor.UpperLeft, Color.white);
            RectTransform titleRect = _selectedBuildingTitleText.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0f, 1f);
            titleRect.anchorMax = new Vector2(1f, 1f);
            titleRect.offsetMin = new Vector2(18f, -54f);
            titleRect.offsetMax = new Vector2(-18f, -14f);

            _selectedBuildingDetailsText = UiFactory.CreateText("SelectedDetails", _selectedBuildingPanel.transform, string.Empty, 19, TextAnchor.UpperLeft, new Color(0.94f, 0.95f, 0.98f));
            RectTransform detailsRect = _selectedBuildingDetailsText.GetComponent<RectTransform>();
            detailsRect.anchorMin = new Vector2(0f, 1f);
            detailsRect.anchorMax = new Vector2(1f, 1f);
            detailsRect.offsetMin = new Vector2(18f, -136f);
            detailsRect.offsetMax = new Vector2(-18f, -56f);

            _selectedBuildingStatusText = UiFactory.CreateText("SelectedStatus", _selectedBuildingPanel.transform, string.Empty, 19, TextAnchor.UpperLeft, new Color(0.95f, 0.82f, 0.28f));
            RectTransform statusRect = _selectedBuildingStatusText.GetComponent<RectTransform>();
            statusRect.anchorMin = new Vector2(0f, 1f);
            statusRect.anchorMax = new Vector2(1f, 1f);
            statusRect.offsetMin = new Vector2(18f, -188f);
            statusRect.offsetMax = new Vector2(-18f, -138f);

            _selectedBuildingBonusText = UiFactory.CreateText("SelectedBonus", _selectedBuildingPanel.transform, string.Empty, 17, TextAnchor.UpperLeft, new Color(0.8f, 0.87f, 0.92f));
            RectTransform selectedBonusRect = _selectedBuildingBonusText.GetComponent<RectTransform>();
            selectedBonusRect.anchorMin = new Vector2(0f, 1f);
            selectedBonusRect.anchorMax = new Vector2(1f, 1f);
            selectedBonusRect.offsetMin = new Vector2(18f, -236f);
            selectedBonusRect.offsetMax = new Vector2(-18f, -186f);

            _upgradeButton = UiFactory.CreateButton("UpgradeButton", _selectedBuildingPanel.transform, "Upgrade", null, out _upgradeButtonText);
            RectTransform upgradeRect = _upgradeButton.GetComponent<RectTransform>();
            upgradeRect.anchorMin = new Vector2(0f, 0f);
            upgradeRect.anchorMax = new Vector2(0f, 0f);
            upgradeRect.sizeDelta = new Vector2(176f, 76f);
            upgradeRect.anchoredPosition = new Vector2(106f, 50f);
            _upgradeButton.onClick.AddListener(OnUpgradePressed);
            _upgradeButtonText.fontSize = 18;

            _sellButton = UiFactory.CreateButton("SellButton", _selectedBuildingPanel.transform, "Sell", null, out _sellButtonText);
            RectTransform sellRect = _sellButton.GetComponent<RectTransform>();
            sellRect.anchorMin = new Vector2(1f, 0f);
            sellRect.anchorMax = new Vector2(1f, 0f);
            sellRect.sizeDelta = new Vector2(176f, 76f);
            sellRect.anchoredPosition = new Vector2(-106f, 50f);
            _sellButton.onClick.AddListener(OnSellPressed);
            _sellButtonText.fontSize = 18;

            _selectedBuildingPanel.SetActive(false);
        }

        private void BuildEndPanel(Transform parent)
        {
            _endPanel = UiFactory.CreatePanel(
                "EndPanel",
                parent,
                Vector2.zero,
                Vector2.one,
                Vector2.zero,
                Vector2.zero,
                new Color(0.02f, 0.04f, 0.08f, 0.88f));

            _endTitleText = UiFactory.CreateText("EndTitle", _endPanel.transform, "Victory", 58, TextAnchor.MiddleCenter, Color.white);
            RectTransform endTitleRect = _endTitleText.GetComponent<RectTransform>();
            endTitleRect.anchorMin = new Vector2(0.5f, 0.5f);
            endTitleRect.anchorMax = new Vector2(0.5f, 0.5f);
            endTitleRect.sizeDelta = new Vector2(640f, 72f);
            endTitleRect.anchoredPosition = new Vector2(0f, 138f);

            _endBodyText = UiFactory.CreateText("EndBody", _endPanel.transform, string.Empty, 28, TextAnchor.MiddleCenter, new Color(0.92f, 0.92f, 0.95f));
            RectTransform endBodyRect = _endBodyText.GetComponent<RectTransform>();
            endBodyRect.anchorMin = new Vector2(0.5f, 0.5f);
            endBodyRect.anchorMax = new Vector2(0.5f, 0.5f);
            endBodyRect.sizeDelta = new Vector2(920f, 120f);
            endBodyRect.anchoredPosition = new Vector2(0f, 32f);

            Text retryLabel;
            _retryButton = UiFactory.CreateButton("RetryButton", _endPanel.transform, "Retry Battle", null, out retryLabel);
            RectTransform retryRect = _retryButton.GetComponent<RectTransform>();
            retryRect.anchorMin = new Vector2(0.5f, 0.5f);
            retryRect.anchorMax = new Vector2(0.5f, 0.5f);
            retryRect.sizeDelta = new Vector2(280f, 70f);
            retryRect.anchoredPosition = new Vector2(-154f, -120f);

            Text menuLabel;
            _menuButton = UiFactory.CreateButton("MenuButton", _endPanel.transform, "Main Menu", null, out menuLabel);
            RectTransform menuRect = _menuButton.GetComponent<RectTransform>();
            menuRect.anchorMin = new Vector2(0.5f, 0.5f);
            menuRect.anchorMax = new Vector2(0.5f, 0.5f);
            menuRect.sizeDelta = new Vector2(280f, 70f);
            menuRect.anchoredPosition = new Vector2(154f, -120f);

            _endPanel.SetActive(false);
        }

        private void CreateResourceRow(Transform parent)
        {
            ResourceType[] types = { ResourceType.Ore, ResourceType.Energy, ResourceType.Alloy, ResourceType.Ammo };

            for (int index = 0; index < types.Length; index++)
            {
                GameObject pill = UiFactory.CreatePanel(
                    types[index] + "Pill",
                    parent,
                    new Vector2(0f, 0f),
                    new Vector2(0f, 1f),
                    new Vector2(18f + (index * 112f), 14f),
                    new Vector2(118f + (index * 112f), -14f),
                    new Color(0.13f, 0.2f, 0.3f, 0.95f));

                Text label = UiFactory.CreateText(
                    types[index] + "Text",
                    pill.transform,
                    ResourceFormatting.GetShortLabel(types[index]) + "\n0",
                    20,
                    TextAnchor.MiddleCenter,
                    Color.white);

                RectTransform labelRect = label.GetComponent<RectTransform>();
                labelRect.anchorMin = Vector2.zero;
                labelRect.anchorMax = Vector2.one;
                labelRect.offsetMin = Vector2.zero;
                labelRect.offsetMax = Vector2.zero;

                _resourceTexts[types[index]] = label;
            }
        }

        private string BuildButtonLabel(BuildingDefinition definition)
        {
            return definition.DisplayName + "\n<size=14>" + ResourceFormatting.FormatCost(definition.BuildCost) + "</size>";
        }

        private void UpdateBuildButtons()
        {
            foreach (KeyValuePair<BuildingDefinition, Button> pair in _buildButtons)
            {
                bool canBuild = _currentSelectedTile != null
                    && !_currentSelectedTile.IsOccupied
                    && _resourceBank.CanAfford(pair.Key.BuildCost);

                pair.Value.interactable = canBuild;
                pair.Value.GetComponent<Image>().color = canBuild
                    ? new Color(0.17f, 0.28f, 0.41f, 0.94f)
                    : new Color(0.12f, 0.12f, 0.14f, 0.7f);

                Text label;
                if (_buildButtonTexts.TryGetValue(pair.Key, out label))
                {
                    label.text = BuildButtonLabel(pair.Key);
                }
            }
        }

        private void UpdateSelectedBuildingPanel()
        {
            if (_selectedBuildingPanel == null || _currentSelectedTile == null || !_currentSelectedTile.IsOccupied)
            {
                return;
            }

            BuildingBase building = _currentSelectedTile.PlacedBuilding;
            _selectedBuildingTitleText.text = building.Definition.DisplayName + "  Lv." + building.Level;
            _selectedBuildingDetailsText.text = building.GetDetailsText();
            _selectedBuildingStatusText.text = "Status: " + building.GetStatusText();
            _selectedBuildingBonusText.text = _currentSelectedTile.GetActiveBonusHint(building.Definition);

            List<ResourceAmount> upgradeCost = building.GetUpgradeCost();
            bool canUpgrade = building.CanUpgrade && _resourceBank.CanAfford(upgradeCost);
            _upgradeButton.interactable = canUpgrade;
            _upgradeButtonText.text = building.CanUpgrade
                ? "Upgrade\n<size=14>" + ResourceFormatting.FormatCost(upgradeCost) + "</size>"
                : "Max Level";

            _sellButton.interactable = true;
            _sellButtonText.text = "Sell\n<size=14>" + ResourceFormatting.FormatCost(building.GetSellRefund()) + "</size>";
        }

        private void OnBuildPressed(BuildingDefinition definition)
        {
            _placementController.TryPlaceSelected(definition);
        }

        private void OnUpgradePressed()
        {
            _placementController.TryUpgradeSelected();
        }

        private void OnSellPressed()
        {
            _placementController.TrySellSelected();
        }
    }
}
