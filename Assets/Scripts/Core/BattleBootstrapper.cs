using System;
using System.Collections.Generic;
using FortDefense.Data;
using FortDefense.Gameplay;
using FortDefense.UI;
using FortDefense.Waves;
using UnityEngine;
using UnityEngine.UI;

namespace FortDefense.Core
{
    public class BattleBootstrapper : MonoBehaviour
    {
        private BattleInputController _inputController;
        private BattleUIController _battleUiController;
        private WaveManager _waveManager;
        private CoreBase _coreBase;
        private TileSelectionController _selectionController;
        private PlacementController _placementController;
        private ResourceBank _resourceBank;
        private bool _battleEnded;

        private void Start()
        {
            Time.timeScale = 1f;
            UiFactory.EnsureEventSystem();

            GameBalanceConfig balanceConfig = Resources.Load<GameBalanceConfig>(PrototypeNames.GameBalanceResourcePath);
            List<BuildingDefinition> buildingDefinitions = LoadAndSortBuildings();
            List<WaveDefinition> waveDefinitions = LoadAndSortWaves();

            if (balanceConfig == null || buildingDefinitions.Count == 0 || waveDefinitions.Count == 0)
            {
                Debug.LogError("Prototype data assets are missing. Run Tools/Fort Defense/Rebuild Prototype Assets & Scenes.");
                CreateFailureOverlay();
                return;
            }

            Transform worldRoot = new GameObject("BattleWorld").transform;
            worldRoot.SetParent(transform, false);

            MapBuildResult map = new MapBuilder().Build(worldRoot);
            Camera worldCamera = CreateCamera(map.MapCenter);
            CreateLighting();

            _coreBase = map.CoreObject.AddComponent<CoreBase>();
            _coreBase.Initialize(balanceConfig.StartingCoreHealth);

            _resourceBank = new GameObject("ResourceBank").AddComponent<ResourceBank>();
            _resourceBank.Initialize(balanceConfig.StartingResources);

            EnemyTracker enemyTracker = new GameObject("EnemyTracker").AddComponent<EnemyTracker>();
            _selectionController = new GameObject("TileSelectionController").AddComponent<TileSelectionController>();

            _placementController = new GameObject("PlacementController").AddComponent<PlacementController>();
            _placementController.Initialize(_resourceBank, enemyTracker, _selectionController);

            _battleUiController = new GameObject("BattleUiController").AddComponent<BattleUIController>();
            _battleUiController.Initialize(buildingDefinitions, _resourceBank, _placementController);
            _battleUiController.RefreshCoreHealth(_coreBase.CurrentHealth, _coreBase.MaxHealth);

            _selectionController.TileSelected += _battleUiController.ShowTileSelection;
            _selectionController.SelectionCleared += _battleUiController.HideSelection;

            _placementController.PlacementRejected += message => _battleUiController.ShowStatus(message, new Color(0.96f, 0.43f, 0.31f));
            _placementController.BuildingPlaced += building =>
            {
                _battleUiController.ShowTileSelection(building.Tile);
                _battleUiController.ShowStatus(building.Definition.DisplayName + " deployed.", new Color(0.38f, 0.95f, 0.57f));
            };
            _placementController.BuildingUpgraded += building =>
            {
                _battleUiController.ShowTileSelection(building.Tile);
                _battleUiController.ShowStatus(building.Definition.DisplayName + " upgraded to Lv." + building.Level + ".", new Color(0.38f, 0.95f, 0.57f));
            };
            _placementController.BuildingSold += tile =>
            {
                _battleUiController.ShowTileSelection(tile);
                _battleUiController.ShowStatus("Structure sold. Slot reopened.", new Color(0.97f, 0.87f, 0.34f));
            };

            _resourceBank.ResourcesChanged += _battleUiController.RefreshResources;
            _coreBase.HealthChanged += _battleUiController.RefreshCoreHealth;
            _coreBase.Destroyed += HandleCoreDestroyed;

            _inputController = new GameObject("BattleInputController").AddComponent<BattleInputController>();
            _inputController.Initialize(worldCamera, _selectionController);

            Transform enemyRoot = new GameObject("Enemies").transform;
            enemyRoot.SetParent(worldRoot, false);

            _waveManager = new GameObject("WaveManager").AddComponent<WaveManager>();
            _waveManager.Initialize(waveDefinitions, map.PathPoints, enemyTracker, _coreBase, enemyRoot);
            _waveManager.WaveStarted += _battleUiController.RefreshWaveStarted;
            _waveManager.WaveCountdownUpdated += _battleUiController.RefreshWaveCountdown;
            _waveManager.AllWavesCompleted += HandleAllWavesCompleted;
            _waveManager.Begin();

            _battleUiController.RefreshResources();
            _battleUiController.RefreshWaveCountdown(1, waveDefinitions[0].CountdownBeforeWave);
            _battleUiController.ShowStatus("Tap a slot, build economy, then feed towers with power and ammo.", new Color(0.97f, 0.87f, 0.34f), 4f);
        }

        private void OnDestroy()
        {
            Time.timeScale = 1f;
        }

        private void HandleCoreDestroyed()
        {
            EndBattle(false);
        }

        private void HandleAllWavesCompleted()
        {
            EndBattle(true);
        }

        private void EndBattle(bool victory)
        {
            if (_battleEnded)
            {
                return;
            }

            _battleEnded = true;
            Time.timeScale = 0f;

            if (_waveManager != null)
            {
                _waveManager.StopAllCoroutines();
            }

            if (_inputController != null)
            {
                _inputController.enabled = false;
            }

            if (_selectionController != null)
            {
                _selectionController.ClearSelection();
            }

            _battleUiController.ShowEndScreen(victory, SceneLoader.LoadBattle, SceneLoader.LoadMainMenu);
        }

        private static Camera CreateCamera(Vector3 mapCenter)
        {
            GameObject cameraObject = new GameObject("MainCamera");
            cameraObject.tag = "MainCamera";

            Camera camera = cameraObject.AddComponent<Camera>();
            camera.orthographic = true;
            camera.orthographicSize = 18.5f;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.62f, 0.78f, 0.94f);
            camera.transform.position = mapCenter + new Vector3(-11f, 30f, -16f);
            camera.transform.rotation = Quaternion.Euler(54f, 34f, 0f);

            return camera;
        }

        private static void CreateLighting()
        {
            RenderSettings.ambientLight = new Color(0.72f, 0.74f, 0.77f);

            GameObject lightObject = new GameObject("Directional Light");
            Light light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.25f;
            light.color = new Color(1f, 0.95f, 0.88f);
            light.transform.rotation = Quaternion.Euler(48f, -28f, 0f);
        }

        private static List<BuildingDefinition> LoadAndSortBuildings()
        {
            BuildingDefinition[] definitions = Resources.LoadAll<BuildingDefinition>(PrototypeNames.BuildingDefinitionsResourcePath);
            List<BuildingDefinition> sorted = new List<BuildingDefinition>(definitions);
            sorted.Sort((left, right) => GetBuildingSortOrder(left.Id).CompareTo(GetBuildingSortOrder(right.Id)));
            return sorted;
        }

        private static int GetBuildingSortOrder(string id)
        {
            switch (id)
            {
                case "tower_gun":
                    return 0;
                case "tower_cannon":
                    return 1;
                case "tower_mortar":
                    return 2;
                case "mine":
                    return 3;
                case "generator":
                    return 4;
                case "smelter":
                    return 5;
                case "ammo_factory":
                    return 6;
                default:
                    return 99;
            }
        }

        private static List<WaveDefinition> LoadAndSortWaves()
        {
            WaveDefinition[] definitions = Resources.LoadAll<WaveDefinition>(PrototypeNames.WaveDefinitionsResourcePath);
            List<WaveDefinition> sorted = new List<WaveDefinition>(definitions);
            sorted.Sort((left, right) => left.WaveNumber.CompareTo(right.WaveNumber));
            return sorted;
        }

        private static void CreateFailureOverlay()
        {
            Canvas canvas = UiFactory.CreateCanvas("FailureCanvas");

            GameObject panel = UiFactory.CreatePanel(
                "FailurePanel",
                canvas.transform,
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(-420f, -180f),
                new Vector2(420f, 180f),
                new Color(0.08f, 0.1f, 0.14f, 0.92f));

            Text message = UiFactory.CreateText(
                "Message",
                panel.transform,
                "Prototype data is missing.\nRun Tools/Fort Defense/Rebuild Prototype Assets & Scenes in the Unity editor.",
                30,
                TextAnchor.MiddleCenter,
                Color.white);
            RectTransform messageRect = message.GetComponent<RectTransform>();
            messageRect.anchorMin = Vector2.zero;
            messageRect.anchorMax = Vector2.one;
            messageRect.offsetMin = new Vector2(24f, 24f);
            messageRect.offsetMax = new Vector2(-24f, -24f);
        }
    }
}
