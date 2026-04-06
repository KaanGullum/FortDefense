using System.Collections.Generic;
using System.IO;
using FortDefense.Core;
using FortDefense.Data;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace FortDefense.Editor
{
    [InitializeOnLoad]
    public static class PrototypeProjectSetup
    {
        private const int CurrentPrototypeVersion = 3;
        private const string SessionKey = "FortDefense.PrototypeSetupCompleted";
        private const string ResourcesRoot = "Assets/Resources";
        private const string ConfigRoot = "Assets/Resources/GameConfigs";
        private const string BuildingsRoot = "Assets/Resources/GameConfigs/Buildings";
        private const string EnemiesRoot = "Assets/Resources/GameConfigs/Enemies";
        private const string WavesRoot = "Assets/Resources/GameConfigs/Waves";

        static PrototypeProjectSetup()
        {
            EditorApplication.delayCall += RunOnceOnEditorLoad;
        }

        [MenuItem("Tools/Fort Defense/Rebuild Prototype Assets & Scenes")]
        public static void RebuildPrototype()
        {
            EnsurePrototypeAssets(true);
        }

        private static void RunOnceOnEditorLoad()
        {
            if (SessionState.GetBool(SessionKey, false))
            {
                return;
            }

            SessionState.SetBool(SessionKey, true);

            GameBalanceConfig balanceConfig = AssetDatabase.LoadAssetAtPath<GameBalanceConfig>(ConfigRoot + "/GameBalanceConfig.asset");
            if (!File.Exists("Assets/Scenes/MainMenu.unity")
                || !File.Exists("Assets/Scenes/Battle.unity")
                || balanceConfig == null
                || balanceConfig.PrototypeVersion < CurrentPrototypeVersion)
            {
                EnsurePrototypeAssets(true);
            }
        }

        private static void EnsurePrototypeAssets(bool forceRebuild)
        {
            EnsureFolder(ResourcesRoot);
            EnsureFolder(ConfigRoot);
            EnsureFolder(BuildingsRoot);
            EnsureFolder(EnemiesRoot);
            EnsureFolder(WavesRoot);

            GameBalanceConfig balanceConfig = CreateOrUpdateAsset<GameBalanceConfig>(
                ConfigRoot + "/GameBalanceConfig.asset",
                forceRebuild,
                asset =>
                {
                    asset.PrototypeVersion = CurrentPrototypeVersion;
                    asset.StartingCoreHealth = 28;
                    asset.StartingResources = new List<ResourceAmount>
                    {
                        new ResourceAmount(ResourceType.Ore, 320),
                        new ResourceAmount(ResourceType.Energy, 80),
                        new ResourceAmount(ResourceType.Alloy, 22),
                        new ResourceAmount(ResourceType.Ammo, 10)
                    };
                });

            BuildingDefinition gunTower = CreateOrUpdateAsset<BuildingDefinition>(BuildingsRoot + "/01_GunTower.asset", forceRebuild, asset =>
            {
                asset.Id = "tower_gun";
                asset.DisplayName = "Gun Tower";
                asset.Category = BuildingCategory.Defense;
                asset.VisualStyle = BuildingVisualStyle.GunTower;
                asset.BuildCost = Cost(new ResourceAmount(ResourceType.Ore, 55), new ResourceAmount(ResourceType.Energy, 5));
                asset.MaxLevel = 3;
                asset.UpgradeCost = Cost(new ResourceAmount(ResourceType.Ore, 40), new ResourceAmount(ResourceType.Energy, 10));
                asset.UpgradeCostMultiplierPerLevel = 1.35f;
                asset.SellRefundPercent = 0.65f;
                asset.DamageKind = DamageKind.Light;
                asset.Damage = 8.5f;
                asset.AttackInterval = 0.45f;
                asset.Range = 8.6f;
                asset.ProjectileSpeed = 24f;
                asset.SplashRadius = 0f;
                asset.UsesArcProjectile = false;
                asset.AttackCostPerShot = Cost(new ResourceAmount(ResourceType.Energy, 1));
                asset.DamageMultiplierPerLevel = 1.24f;
                asset.AttackIntervalMultiplierPerLevel = 0.93f;
                asset.RangeBonusPerLevel = 0.45f;
                asset.ConsumptionPerCycle = new List<ResourceAmount>();
                asset.ProductionPerCycle = new List<ResourceAmount>();
                asset.ProductionMultiplierPerLevel = 1.3f;
            });

            BuildingDefinition cannonTower = CreateOrUpdateAsset<BuildingDefinition>(BuildingsRoot + "/02_CannonTower.asset", forceRebuild, asset =>
            {
                asset.Id = "tower_cannon";
                asset.DisplayName = "Cannon Tower";
                asset.Category = BuildingCategory.Defense;
                asset.VisualStyle = BuildingVisualStyle.CannonTower;
                asset.BuildCost = Cost(
                    new ResourceAmount(ResourceType.Ore, 85),
                    new ResourceAmount(ResourceType.Energy, 10),
                    new ResourceAmount(ResourceType.Alloy, 14));
                asset.MaxLevel = 3;
                asset.UpgradeCost = Cost(
                    new ResourceAmount(ResourceType.Ore, 52),
                    new ResourceAmount(ResourceType.Energy, 10),
                    new ResourceAmount(ResourceType.Alloy, 8));
                asset.UpgradeCostMultiplierPerLevel = 1.35f;
                asset.SellRefundPercent = 0.65f;
                asset.DamageKind = DamageKind.Heavy;
                asset.Damage = 30f;
                asset.AttackInterval = 1.3f;
                asset.Range = 8.2f;
                asset.ProjectileSpeed = 14f;
                asset.SplashRadius = 0f;
                asset.UsesArcProjectile = false;
                asset.AttackCostPerShot = Cost(
                    new ResourceAmount(ResourceType.Ammo, 1),
                    new ResourceAmount(ResourceType.Energy, 1));
                asset.DamageMultiplierPerLevel = 1.28f;
                asset.AttackIntervalMultiplierPerLevel = 0.94f;
                asset.RangeBonusPerLevel = 0.45f;
                asset.ConsumptionPerCycle = new List<ResourceAmount>();
                asset.ProductionPerCycle = new List<ResourceAmount>();
                asset.ProductionMultiplierPerLevel = 1.3f;
            });

            BuildingDefinition mortarTower = CreateOrUpdateAsset<BuildingDefinition>(BuildingsRoot + "/03_MortarTower.asset", forceRebuild, asset =>
            {
                asset.Id = "tower_mortar";
                asset.DisplayName = "Mortar Tower";
                asset.Category = BuildingCategory.Defense;
                asset.VisualStyle = BuildingVisualStyle.MortarTower;
                asset.BuildCost = Cost(
                    new ResourceAmount(ResourceType.Ore, 95),
                    new ResourceAmount(ResourceType.Energy, 18),
                    new ResourceAmount(ResourceType.Alloy, 18),
                    new ResourceAmount(ResourceType.Ammo, 6));
                asset.MaxLevel = 3;
                asset.UpgradeCost = Cost(
                    new ResourceAmount(ResourceType.Ore, 60),
                    new ResourceAmount(ResourceType.Energy, 12),
                    new ResourceAmount(ResourceType.Alloy, 10));
                asset.UpgradeCostMultiplierPerLevel = 1.38f;
                asset.SellRefundPercent = 0.65f;
                asset.DamageKind = DamageKind.Explosive;
                asset.Damage = 20f;
                asset.AttackInterval = 2.2f;
                asset.Range = 11f;
                asset.ProjectileSpeed = 10f;
                asset.SplashRadius = 3.4f;
                asset.UsesArcProjectile = true;
                asset.AttackCostPerShot = Cost(
                    new ResourceAmount(ResourceType.Ammo, 2),
                    new ResourceAmount(ResourceType.Energy, 2));
                asset.DamageMultiplierPerLevel = 1.3f;
                asset.AttackIntervalMultiplierPerLevel = 0.95f;
                asset.RangeBonusPerLevel = 0.55f;
                asset.ConsumptionPerCycle = new List<ResourceAmount>();
                asset.ProductionPerCycle = new List<ResourceAmount>();
                asset.ProductionMultiplierPerLevel = 1.3f;
            });

            CreateOrUpdateAsset<BuildingDefinition>(BuildingsRoot + "/04_Mine.asset", forceRebuild, asset =>
            {
                asset.Id = "mine";
                asset.DisplayName = "Mine";
                asset.Category = BuildingCategory.Economy;
                asset.VisualStyle = BuildingVisualStyle.Mine;
                asset.BuildCost = Cost(new ResourceAmount(ResourceType.Ore, 45), new ResourceAmount(ResourceType.Energy, 5));
                asset.MaxLevel = 3;
                asset.UpgradeCost = Cost(new ResourceAmount(ResourceType.Ore, 30), new ResourceAmount(ResourceType.Energy, 8));
                asset.UpgradeCostMultiplierPerLevel = 1.35f;
                asset.SellRefundPercent = 0.65f;
                asset.ProductionInterval = 2.8f;
                asset.ConsumptionPerCycle = new List<ResourceAmount>();
                asset.ProductionPerCycle = Cost(new ResourceAmount(ResourceType.Ore, 12));
                asset.ProductionMultiplierPerLevel = 1.35f;
            });

            CreateOrUpdateAsset<BuildingDefinition>(BuildingsRoot + "/05_Generator.asset", forceRebuild, asset =>
            {
                asset.Id = "generator";
                asset.DisplayName = "Generator";
                asset.Category = BuildingCategory.Economy;
                asset.VisualStyle = BuildingVisualStyle.Generator;
                asset.BuildCost = Cost(new ResourceAmount(ResourceType.Ore, 40));
                asset.MaxLevel = 3;
                asset.UpgradeCost = Cost(new ResourceAmount(ResourceType.Ore, 32), new ResourceAmount(ResourceType.Energy, 6));
                asset.UpgradeCostMultiplierPerLevel = 1.35f;
                asset.SellRefundPercent = 0.65f;
                asset.ProductionInterval = 2.7f;
                asset.ConsumptionPerCycle = new List<ResourceAmount>();
                asset.ProductionPerCycle = Cost(new ResourceAmount(ResourceType.Energy, 8));
                asset.ProductionMultiplierPerLevel = 1.35f;
            });

            CreateOrUpdateAsset<BuildingDefinition>(BuildingsRoot + "/06_Smelter.asset", forceRebuild, asset =>
            {
                asset.Id = "smelter";
                asset.DisplayName = "Smelter";
                asset.Category = BuildingCategory.Economy;
                asset.VisualStyle = BuildingVisualStyle.Smelter;
                asset.BuildCost = Cost(new ResourceAmount(ResourceType.Ore, 60), new ResourceAmount(ResourceType.Energy, 15));
                asset.MaxLevel = 3;
                asset.UpgradeCost = Cost(new ResourceAmount(ResourceType.Ore, 36), new ResourceAmount(ResourceType.Energy, 10));
                asset.UpgradeCostMultiplierPerLevel = 1.35f;
                asset.SellRefundPercent = 0.65f;
                asset.ProductionInterval = 4.2f;
                asset.ConsumptionPerCycle = Cost(new ResourceAmount(ResourceType.Ore, 16));
                asset.ProductionPerCycle = Cost(new ResourceAmount(ResourceType.Alloy, 7));
                asset.ProductionMultiplierPerLevel = 1.32f;
            });

            CreateOrUpdateAsset<BuildingDefinition>(BuildingsRoot + "/07_AmmoFactory.asset", forceRebuild, asset =>
            {
                asset.Id = "ammo_factory";
                asset.DisplayName = "Ammo Factory";
                asset.Category = BuildingCategory.Economy;
                asset.VisualStyle = BuildingVisualStyle.AmmoFactory;
                asset.BuildCost = Cost(
                    new ResourceAmount(ResourceType.Ore, 55),
                    new ResourceAmount(ResourceType.Energy, 20),
                    new ResourceAmount(ResourceType.Alloy, 8));
                asset.MaxLevel = 3;
                asset.UpgradeCost = Cost(new ResourceAmount(ResourceType.Ore, 34), new ResourceAmount(ResourceType.Energy, 10), new ResourceAmount(ResourceType.Alloy, 5));
                asset.UpgradeCostMultiplierPerLevel = 1.35f;
                asset.SellRefundPercent = 0.65f;
                asset.ProductionInterval = 4f;
                asset.ConsumptionPerCycle = Cost(
                    new ResourceAmount(ResourceType.Alloy, 4),
                    new ResourceAmount(ResourceType.Energy, 3));
                asset.ProductionPerCycle = Cost(new ResourceAmount(ResourceType.Ammo, 9));
                asset.ProductionMultiplierPerLevel = 1.32f;
            });

            EnemyDefinition runner = CreateOrUpdateAsset<EnemyDefinition>(EnemiesRoot + "/01_Runner.asset", forceRebuild, asset =>
            {
                asset.Id = "runner";
                asset.DisplayName = "Needle Scout";
                asset.VisualStyle = EnemyVisualStyle.NeedleScout;
                asset.MaxHealth = 28f;
                asset.MoveSpeed = 5.2f;
                asset.CoreDamage = 1;
                asset.Scale = 0.9f;
                asset.LightDamageMultiplier = 1f;
                asset.HeavyDamageMultiplier = 1f;
                asset.ExplosiveDamageMultiplier = 1.1f;
                asset.PrimaryColor = new Color(0.24f, 0.82f, 0.74f);
                asset.SecondaryColor = new Color(0.15f, 0.2f, 0.28f);
                asset.AccentColor = new Color(0.96f, 0.84f, 0.28f);
            });

            EnemyDefinition brute = CreateOrUpdateAsset<EnemyDefinition>(EnemiesRoot + "/02_Brute.asset", forceRebuild, asset =>
            {
                asset.Id = "brute";
                asset.DisplayName = "Siege Crawler";
                asset.VisualStyle = EnemyVisualStyle.SiegeCrawler;
                asset.MaxHealth = 120f;
                asset.MoveSpeed = 2.25f;
                asset.CoreDamage = 3;
                asset.Scale = 1.2f;
                asset.LightDamageMultiplier = 0.82f;
                asset.HeavyDamageMultiplier = 1.25f;
                asset.ExplosiveDamageMultiplier = 1f;
                asset.PrimaryColor = new Color(0.74f, 0.37f, 0.22f);
                asset.SecondaryColor = new Color(0.27f, 0.24f, 0.22f);
                asset.AccentColor = new Color(0.98f, 0.66f, 0.22f);
            });

            EnemyDefinition armored = CreateOrUpdateAsset<EnemyDefinition>(EnemiesRoot + "/03_Armored.asset", forceRebuild, asset =>
            {
                asset.Id = "armored";
                asset.DisplayName = "Shield Warden";
                asset.VisualStyle = EnemyVisualStyle.ShieldWarden;
                asset.MaxHealth = 92f;
                asset.MoveSpeed = 3.2f;
                asset.CoreDamage = 2;
                asset.Scale = 1.05f;
                asset.LightDamageMultiplier = 0.55f;
                asset.HeavyDamageMultiplier = 1.1f;
                asset.ExplosiveDamageMultiplier = 0.85f;
                asset.PrimaryColor = new Color(0.49f, 0.67f, 0.89f);
                asset.SecondaryColor = new Color(0.26f, 0.31f, 0.38f);
                asset.AccentColor = new Color(0.88f, 0.95f, 1f);
            });

            CreateOrUpdateAsset<WaveDefinition>(WavesRoot + "/01_Wave01.asset", forceRebuild, asset =>
            {
                asset.WaveNumber = 1;
                asset.CountdownBeforeWave = 10f;
                asset.SpawnEntries = new List<WaveSpawnEntry>
                {
                    Spawn(runner, 8, 0.65f, 0f)
                };
            });

            CreateOrUpdateAsset<WaveDefinition>(WavesRoot + "/02_Wave02.asset", forceRebuild, asset =>
            {
                asset.WaveNumber = 2;
                asset.CountdownBeforeWave = 12f;
                asset.SpawnEntries = new List<WaveSpawnEntry>
                {
                    Spawn(runner, 10, 0.56f, 0f),
                    Spawn(brute, 4, 1.2f, 1.4f)
                };
            });

            CreateOrUpdateAsset<WaveDefinition>(WavesRoot + "/03_Wave03.asset", forceRebuild, asset =>
            {
                asset.WaveNumber = 3;
                asset.CountdownBeforeWave = 15f;
                asset.SpawnEntries = new List<WaveSpawnEntry>
                {
                    Spawn(runner, 8, 0.48f, 0f),
                    Spawn(armored, 6, 0.8f, 1.2f),
                    Spawn(brute, 5, 1.05f, 1.5f)
                };
            });

            CreateOrUpdateAsset<WaveDefinition>(WavesRoot + "/04_Wave04.asset", forceRebuild, asset =>
            {
                asset.WaveNumber = 4;
                asset.CountdownBeforeWave = 16f;
                asset.SpawnEntries = new List<WaveSpawnEntry>
                {
                    Spawn(runner, 10, 0.46f, 0f),
                    Spawn(armored, 8, 0.72f, 1f),
                    Spawn(brute, 7, 0.94f, 1.4f)
                };
            });

            CreateScene("Assets/Scenes/MainMenu.unity", typeof(MainMenuBootstrapper), forceRebuild);
            CreateScene("Assets/Scenes/Battle.unity", typeof(BattleBootstrapper), forceRebuild);

            ConfigureBuildSettings();
            ConfigurePlayerSettings();

            EditorUtility.SetDirty(balanceConfig);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void CreateScene(string path, System.Type bootstrapType, bool forceRebuild)
        {
            if (File.Exists(path) && !forceRebuild)
            {
                return;
            }

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            GameObject bootstrap = new GameObject(bootstrapType.Name);
            bootstrap.AddComponent(bootstrapType);
            EditorSceneManager.SaveScene(scene, path);
        }

        private static void ConfigureBuildSettings()
        {
            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene("Assets/Scenes/MainMenu.unity", true),
                new EditorBuildSettingsScene("Assets/Scenes/Battle.unity", true)
            };
        }

        private static void ConfigurePlayerSettings()
        {
            PlayerSettings.companyName = "Codex";
            PlayerSettings.productName = "Fort Defense";
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
            PlayerSettings.allowedAutorotateToPortrait = true;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
            PlayerSettings.allowedAutorotateToLandscapeLeft = false;
            PlayerSettings.allowedAutorotateToLandscapeRight = false;
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, "com.codex.fortdefense");
        }

        private static List<ResourceAmount> Cost(params ResourceAmount[] amounts)
        {
            return new List<ResourceAmount>(amounts);
        }

        private static WaveSpawnEntry Spawn(EnemyDefinition enemy, int count, float interval, float delay)
        {
            return new WaveSpawnEntry
            {
                Enemy = enemy,
                Count = count,
                SpawnInterval = interval,
                StartDelay = delay
            };
        }

        private static T CreateOrUpdateAsset<T>(string assetPath, bool forceRebuild, System.Action<T> configure) where T : ScriptableObject
        {
            T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);

            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<T>();
                AssetDatabase.CreateAsset(asset, assetPath);
            }
            else if (!forceRebuild)
            {
                return asset;
            }

            configure(asset);
            EditorUtility.SetDirty(asset);
            return asset;
        }

        private static void EnsureFolder(string path)
        {
            string[] segments = path.Split('/');
            string current = segments[0];

            for (int index = 1; index < segments.Length; index++)
            {
                string next = current + "/" + segments[index];
                if (!AssetDatabase.IsValidFolder(next))
                {
                    AssetDatabase.CreateFolder(current, segments[index]);
                }

                current = next;
            }
        }
    }
}
