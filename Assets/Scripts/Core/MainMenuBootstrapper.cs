using System.Collections.Generic;
using FortDefense.Buildings;
using FortDefense.Data;
using FortDefense.Gameplay;
using FortDefense.UI;
using UnityEngine;
using UnityEngine.UI;

namespace FortDefense.Core
{
    public class MainMenuBootstrapper : MonoBehaviour
    {
        private void Start()
        {
            Time.timeScale = 1f;
            UiFactory.EnsureEventSystem();

            Transform worldRoot = new GameObject("MenuWorld").transform;
            worldRoot.SetParent(transform, false);

            MapBuildResult map = new MapBuilder().Build(worldRoot);
            DecorateMap(map);
            CreateCamera(map.MapCenter);
            CreateLighting();
            CreateMenuUi();
        }

        private static void DecorateMap(MapBuildResult map)
        {
            BuildingDefinition[] definitions = Resources.LoadAll<BuildingDefinition>(PrototypeNames.BuildingDefinitionsResourcePath);
            Dictionary<string, BuildingDefinition> lookup = new Dictionary<string, BuildingDefinition>();
            for (int index = 0; index < definitions.Length; index++)
            {
                lookup[definitions[index].Id] = definitions[index];
            }

            CreateDecorativeBuilding(map, 5, lookup, "tower_gun");
            CreateDecorativeBuilding(map, 14, lookup, "generator");
            CreateDecorativeBuilding(map, 22, lookup, "tower_mortar");
        }

        private static void CreateDecorativeBuilding(MapBuildResult map, int tileIndex, IDictionary<string, BuildingDefinition> lookup, string buildingId)
        {
            if (tileIndex < 0 || tileIndex >= map.BuildTiles.Count)
            {
                return;
            }

            BuildingDefinition definition;
            if (!lookup.TryGetValue(buildingId, out definition))
            {
                return;
            }

            GameObject preview = new GameObject(definition.DisplayName + "_Preview");
            preview.transform.position = map.BuildTiles[tileIndex].transform.position + Vector3.up * 0.15f;
            BuildingVisualFactory.BuildVisual(definition, preview.transform);
        }

        private static void CreateCamera(Vector3 mapCenter)
        {
            GameObject cameraObject = new GameObject("MainCamera");
            Camera camera = cameraObject.AddComponent<Camera>();
            camera.orthographic = true;
            camera.orthographicSize = 35.5f;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.62f, 0.78f, 0.94f);
            camera.transform.position = mapCenter + new Vector3(0f, 44f, -20f);
            camera.transform.rotation = Quaternion.Euler(63f, 0f, 0f);
        }

        private static void CreateLighting()
        {
            RenderSettings.ambientLight = new Color(0.72f, 0.74f, 0.77f);

            GameObject lightObject = new GameObject("Directional Light");
            Light light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.2f;
            light.color = new Color(1f, 0.96f, 0.89f);
            light.transform.rotation = Quaternion.Euler(48f, -32f, 0f);
        }

        private static void CreateMenuUi()
        {
            Canvas canvas = UiFactory.CreateCanvas("MenuCanvas");

            GameObject panel = UiFactory.CreatePanel(
                "MenuPanel",
                canvas.transform,
                new Vector2(0f, 0f),
                new Vector2(0f, 1f),
                new Vector2(28f, 28f),
                new Vector2(612f, -28f),
                new Color(0.04f, 0.06f, 0.09f, 0.82f));

            Text title = UiFactory.CreateText("Title", panel.transform, "Fort Defense", 58, TextAnchor.UpperLeft, Color.white);
            RectTransform titleRect = title.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0f, 1f);
            titleRect.anchorMax = new Vector2(1f, 1f);
            titleRect.offsetMin = new Vector2(28f, -90f);
            titleRect.offsetMax = new Vector2(-28f, -18f);

            Text subtitle = UiFactory.CreateText(
                "Subtitle",
                panel.transform,
                "Stylized 3D road-and-slots defense prototype for iPhone portrait.\nPower your guns, refine alloy, manufacture ammo, and hold four escalating waves.",
                28,
                TextAnchor.UpperLeft,
                new Color(0.9f, 0.92f, 0.95f));
            RectTransform subtitleRect = subtitle.GetComponent<RectTransform>();
            subtitleRect.anchorMin = new Vector2(0f, 1f);
            subtitleRect.anchorMax = new Vector2(1f, 1f);
            subtitleRect.offsetMin = new Vector2(28f, -206f);
            subtitleRect.offsetMax = new Vector2(-28f, -110f);

            Text notes = UiFactory.CreateText(
                "Notes",
                panel.transform,
                "Tap build slots near the road.\nSpecial ore, power, and industry pads boost the right economy buildings.\nGun, Cannon, and Mortar towers now depend on live energy and ammo supply.",
                24,
                TextAnchor.UpperLeft,
                new Color(0.83f, 0.86f, 0.9f));
            RectTransform notesRect = notes.GetComponent<RectTransform>();
            notesRect.anchorMin = new Vector2(0f, 1f);
            notesRect.anchorMax = new Vector2(1f, 1f);
            notesRect.offsetMin = new Vector2(28f, -380f);
            notesRect.offsetMax = new Vector2(-28f, -230f);

            Text playLabel;
            Button playButton = UiFactory.CreateButton("PlayButton", panel.transform, "Start Defense", SceneLoader.LoadBattle, out playLabel);
            RectTransform playRect = playButton.GetComponent<RectTransform>();
            playRect.anchorMin = new Vector2(0f, 0f);
            playRect.anchorMax = new Vector2(0f, 0f);
            playRect.sizeDelta = new Vector2(260f, 78f);
            playRect.anchoredPosition = new Vector2(158f, 98f);

            Text hint = UiFactory.CreateText("Hint", panel.transform, "Open `Assets/Scenes/MainMenu.unity` or press Play from here.", 18, TextAnchor.LowerLeft, new Color(0.73f, 0.77f, 0.82f));
            RectTransform hintRect = hint.GetComponent<RectTransform>();
            hintRect.anchorMin = new Vector2(0f, 0f);
            hintRect.anchorMax = new Vector2(1f, 0f);
            hintRect.offsetMin = new Vector2(28f, 20f);
            hintRect.offsetMax = new Vector2(-28f, 52f);
        }
    }
}
