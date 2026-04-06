using System.Collections.Generic;
using FortDefense.Data;
using FortDefense.Gameplay;
using FortDefense.Utilities;
using UnityEngine;

namespace FortDefense.Enemies
{
    public static class EnemyFactory
    {
        public static EnemyUnit CreateEnemy(
            EnemyDefinition definition,
            IList<Vector3> pathPoints,
            CoreBase coreBase,
            EnemyTracker enemyTracker,
            Transform parent)
        {
            GameObject root = new GameObject(definition.DisplayName);
            root.transform.SetParent(parent, false);
            root.transform.localScale = Vector3.one * definition.Scale;

            Transform visualRoot = new GameObject("VisualRoot").transform;
            visualRoot.SetParent(root.transform, false);

            BuildVisual(definition, visualRoot);
            Transform healthFill = CreateHealthBar(root.transform);

            EnemyUnit enemy = root.AddComponent<EnemyUnit>();
            enemy.Initialize(definition, pathPoints, coreBase, enemyTracker, visualRoot, healthFill);

            return enemy;
        }

        private static void BuildVisual(EnemyDefinition definition, Transform root)
        {
            Color secondary = definition.SecondaryColor;
            Color dark = Color.Lerp(secondary, Color.black, 0.28f);

            switch (definition.VisualStyle)
            {
                case EnemyVisualStyle.Runner:
                    CreatePrimaryPart(definition, "Body", PrimitiveType.Capsule, root, new Vector3(0f, 0.75f, 0f), new Vector3(0.8f, 1.1f, 0.8f));
                    CreateDarkPart(definition, "Head", PrimitiveType.Sphere, root, new Vector3(0f, 1.65f, 0f), new Vector3(0.5f, 0.5f, 0.5f), dark);
                    break;
                case EnemyVisualStyle.Brute:
                    CreatePrimaryPart(definition, "Body", PrimitiveType.Cube, root, new Vector3(0f, 0.95f, 0f), new Vector3(1.4f, 1.5f, 1.2f));
                    CreateDarkPart(definition, "ShoulderA", PrimitiveType.Sphere, root, new Vector3(-0.62f, 1.2f, 0f), new Vector3(0.52f, 0.52f, 0.52f), dark);
                    CreateDarkPart(definition, "ShoulderB", PrimitiveType.Sphere, root, new Vector3(0.62f, 1.2f, 0f), new Vector3(0.52f, 0.52f, 0.52f), dark);
                    break;
                case EnemyVisualStyle.Armored:
                    CreatePrimaryPart(definition, "Body", PrimitiveType.Capsule, root, new Vector3(0f, 0.85f, 0f), new Vector3(1f, 1.2f, 1f));
                    CreateSecondaryPart(definition, "Shield", PrimitiveType.Cube, root, new Vector3(0f, 0.95f, 0.55f), new Vector3(1.1f, 1.15f, 0.18f));
                    CreateDarkPart(definition, "Helmet", PrimitiveType.Cylinder, root, new Vector3(0f, 1.72f, 0f), new Vector3(0.42f, 0.22f, 0.42f), dark);
                    break;
                case EnemyVisualStyle.NeedleScout:
                    CreatePrimaryPart(definition, "Body", PrimitiveType.Cylinder, root, new Vector3(0f, 0.72f, -0.02f), new Vector3(0.44f, 0.72f, 0.44f));
                    CreateSecondaryPart(definition, "Head", PrimitiveType.Cube, root, new Vector3(0f, 1.34f, 0.08f), new Vector3(0.52f, 0.36f, 0.54f));
                    CreateAccentPart(definition, "EyeBar", PrimitiveType.Cube, root, new Vector3(0f, 1.35f, 0.37f), new Vector3(0.34f, 0.1f, 0.08f));
                    CreateAccentPart(definition, "Needle", PrimitiveType.Cube, root, new Vector3(0f, 1.18f, 0.56f), new Vector3(0.14f, 0.08f, 0.48f));
                    CreateDarkPart(definition, "Backpack", PrimitiveType.Cube, root, new Vector3(0f, 0.84f, -0.34f), new Vector3(0.28f, 0.52f, 0.2f), dark);
                    CreateSecondaryPart(definition, "LegA", PrimitiveType.Cylinder, root, new Vector3(-0.18f, 0.3f, -0.04f), new Vector3(0.07f, 0.34f, 0.07f));
                    CreateSecondaryPart(definition, "LegB", PrimitiveType.Cylinder, root, new Vector3(0.18f, 0.3f, -0.04f), new Vector3(0.07f, 0.34f, 0.07f));
                    CreateDarkPart(definition, "StabilizerA", PrimitiveType.Cube, root, new Vector3(-0.28f, 0.98f, -0.18f), Quaternion.Euler(0f, 0f, 28f), new Vector3(0.26f, 0.05f, 0.16f), dark);
                    CreateDarkPart(definition, "StabilizerB", PrimitiveType.Cube, root, new Vector3(0.28f, 0.98f, -0.18f), Quaternion.Euler(0f, 0f, -28f), new Vector3(0.26f, 0.05f, 0.16f), dark);
                    break;
                case EnemyVisualStyle.SiegeCrawler:
                    CreatePrimaryPart(definition, "Chassis", PrimitiveType.Cube, root, new Vector3(0f, 0.68f, 0f), new Vector3(1.6f, 0.62f, 1.82f));
                    CreateSecondaryPart(definition, "Turret", PrimitiveType.Cylinder, root, new Vector3(0f, 1.16f, -0.1f), new Vector3(0.66f, 0.2f, 0.66f));
                    CreateAccentPart(definition, "Ram", PrimitiveType.Cube, root, new Vector3(0f, 0.68f, 1.14f), new Vector3(0.48f, 0.24f, 0.72f));
                    CreateDarkPart(definition, "TrackLeft", PrimitiveType.Cube, root, new Vector3(-0.88f, 0.34f, 0f), new Vector3(0.28f, 0.28f, 1.76f), dark);
                    CreateDarkPart(definition, "TrackRight", PrimitiveType.Cube, root, new Vector3(0.88f, 0.34f, 0f), new Vector3(0.28f, 0.28f, 1.76f), dark);
                    CreateAccentPart(definition, "ExhaustLeft", PrimitiveType.Cylinder, root, new Vector3(-0.42f, 0.84f, -0.98f), Quaternion.Euler(90f, 0f, 0f), new Vector3(0.1f, 0.18f, 0.1f));
                    CreateAccentPart(definition, "ExhaustRight", PrimitiveType.Cylinder, root, new Vector3(0.42f, 0.84f, -0.98f), Quaternion.Euler(90f, 0f, 0f), new Vector3(0.1f, 0.18f, 0.1f));
                    break;
                case EnemyVisualStyle.ShieldWarden:
                    CreatePrimaryPart(definition, "Core", PrimitiveType.Sphere, root, new Vector3(0f, 1.08f, 0f), new Vector3(0.82f, 0.82f, 0.82f));
                    CreateSecondaryPart(definition, "LowerDisc", PrimitiveType.Cylinder, root, new Vector3(0f, 0.84f, 0f), new Vector3(0.98f, 0.08f, 0.98f));
                    CreateDarkPart(definition, "UpperDisc", PrimitiveType.Cylinder, root, new Vector3(0f, 1.28f, 0f), new Vector3(0.76f, 0.06f, 0.76f), dark);
                    CreateAccentPart(definition, "FrontLens", PrimitiveType.Sphere, root, new Vector3(0f, 1.08f, 0.42f), new Vector3(0.24f, 0.24f, 0.24f));
                    CreateAccentPart(definition, "ShieldFront", PrimitiveType.Cube, root, new Vector3(0f, 1.02f, 0.78f), new Vector3(1.18f, 0.84f, 0.08f));
                    CreateAccentPart(definition, "ShieldWingA", PrimitiveType.Cube, root, new Vector3(-0.54f, 1.02f, 0.64f), Quaternion.Euler(0f, -34f, 0f), new Vector3(0.46f, 0.74f, 0.08f));
                    CreateAccentPart(definition, "ShieldWingB", PrimitiveType.Cube, root, new Vector3(0.54f, 1.02f, 0.64f), Quaternion.Euler(0f, 34f, 0f), new Vector3(0.46f, 0.74f, 0.08f));
                    CreateAccentPart(definition, "Antenna", PrimitiveType.Cylinder, root, new Vector3(0f, 1.72f, 0f), new Vector3(0.08f, 0.24f, 0.08f));
                    break;
            }
        }

        private static GameObject CreatePrimaryPart(EnemyDefinition definition, string name, PrimitiveType primitiveType, Transform parent, Vector3 localPosition, Vector3 localScale)
        {
            return PrimitiveFactory.CreatePrimitive(name, primitiveType, parent, localPosition, localScale, definition.PrimaryColor, definition.SurfaceGlossiness * 1.08f, 0f);
        }

        private static GameObject CreateSecondaryPart(EnemyDefinition definition, string name, PrimitiveType primitiveType, Transform parent, Vector3 localPosition, Vector3 localScale)
        {
            return PrimitiveFactory.CreatePrimitive(name, primitiveType, parent, localPosition, localScale, definition.SecondaryColor, Mathf.Clamp01(definition.SurfaceGlossiness * 1.18f), 0f);
        }

        private static GameObject CreateAccentPart(EnemyDefinition definition, string name, PrimitiveType primitiveType, Transform parent, Vector3 localPosition, Vector3 localScale)
        {
            return PrimitiveFactory.CreatePrimitive(name, primitiveType, parent, localPosition, localScale, definition.AccentColor, Mathf.Clamp01(definition.SurfaceGlossiness * 0.95f), definition.AccentEmission);
        }

        private static GameObject CreateAccentPart(EnemyDefinition definition, string name, PrimitiveType primitiveType, Transform parent, Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
        {
            return PrimitiveFactory.CreatePrimitive(name, primitiveType, parent, localPosition, localRotation, localScale, definition.AccentColor, Mathf.Clamp01(definition.SurfaceGlossiness * 0.95f), definition.AccentEmission);
        }

        private static GameObject CreateDarkPart(EnemyDefinition definition, string name, PrimitiveType primitiveType, Transform parent, Vector3 localPosition, Vector3 localScale, Color darkColor)
        {
            return PrimitiveFactory.CreatePrimitive(name, primitiveType, parent, localPosition, localScale, darkColor, definition.SurfaceGlossiness * 0.7f, 0f);
        }

        private static GameObject CreateDarkPart(EnemyDefinition definition, string name, PrimitiveType primitiveType, Transform parent, Vector3 localPosition, Quaternion localRotation, Vector3 localScale, Color darkColor)
        {
            return PrimitiveFactory.CreatePrimitive(name, primitiveType, parent, localPosition, localRotation, localScale, darkColor, definition.SurfaceGlossiness * 0.7f, 0f);
        }

        private static Transform CreateHealthBar(Transform root)
        {
            GameObject container = new GameObject("HealthBar");
            container.transform.SetParent(root, false);
            container.transform.localPosition = new Vector3(0f, 2.3f, 0f);

            PrimitiveFactory.CreatePrimitive("Back", PrimitiveType.Cube, container.transform, Vector3.zero, new Vector3(0.84f, 0.08f, 0.12f), new Color(0.14f, 0.12f, 0.12f));
            GameObject fill = PrimitiveFactory.CreatePrimitive("Fill", PrimitiveType.Cube, container.transform, Vector3.zero, new Vector3(0.8f, 0.12f, 0.08f), new Color(0.31f, 0.91f, 0.42f));
            return fill.transform;
        }
    }
}
