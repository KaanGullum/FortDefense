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

            BuildVisual(definition, root.transform);
            Transform healthFill = CreateHealthBar(root.transform);

            EnemyUnit enemy = root.AddComponent<EnemyUnit>();
            enemy.Initialize(definition, pathPoints, coreBase, enemyTracker, healthFill);

            return enemy;
        }

        private static void BuildVisual(EnemyDefinition definition, Transform root)
        {
            Color primary = definition.PrimaryColor;
            Color secondary = definition.SecondaryColor;
            Color accent = definition.AccentColor;
            Color dark = Color.Lerp(secondary, Color.black, 0.28f);

            switch (definition.VisualStyle)
            {
                case EnemyVisualStyle.Runner:
                    PrimitiveFactory.CreatePrimitive("Body", PrimitiveType.Capsule, root, new Vector3(0f, 0.75f, 0f), new Vector3(0.8f, 1.1f, 0.8f), primary);
                    PrimitiveFactory.CreatePrimitive("Head", PrimitiveType.Sphere, root, new Vector3(0f, 1.65f, 0f), new Vector3(0.5f, 0.5f, 0.5f), dark);
                    break;
                case EnemyVisualStyle.Brute:
                    PrimitiveFactory.CreatePrimitive("Body", PrimitiveType.Cube, root, new Vector3(0f, 0.95f, 0f), new Vector3(1.4f, 1.5f, 1.2f), primary);
                    PrimitiveFactory.CreatePrimitive("ShoulderA", PrimitiveType.Sphere, root, new Vector3(-0.62f, 1.2f, 0f), new Vector3(0.52f, 0.52f, 0.52f), dark);
                    PrimitiveFactory.CreatePrimitive("ShoulderB", PrimitiveType.Sphere, root, new Vector3(0.62f, 1.2f, 0f), new Vector3(0.52f, 0.52f, 0.52f), dark);
                    break;
                case EnemyVisualStyle.Armored:
                    PrimitiveFactory.CreatePrimitive("Body", PrimitiveType.Capsule, root, new Vector3(0f, 0.85f, 0f), new Vector3(1f, 1.2f, 1f), primary);
                    PrimitiveFactory.CreatePrimitive("Shield", PrimitiveType.Cube, root, new Vector3(0f, 0.95f, 0.55f), new Vector3(1.1f, 1.15f, 0.18f), secondary);
                    PrimitiveFactory.CreatePrimitive("Helmet", PrimitiveType.Cylinder, root, new Vector3(0f, 1.72f, 0f), new Vector3(0.42f, 0.22f, 0.42f), dark);
                    break;
                case EnemyVisualStyle.NeedleScout:
                    PrimitiveFactory.CreatePrimitive("Body", PrimitiveType.Cylinder, root, new Vector3(0f, 0.74f, 0f), new Vector3(0.6f, 0.52f, 0.6f), primary);
                    PrimitiveFactory.CreatePrimitive("Head", PrimitiveType.Cube, root, new Vector3(0f, 1.32f, 0.08f), new Vector3(0.66f, 0.4f, 0.48f), secondary);
                    PrimitiveFactory.CreatePrimitive("EyeBar", PrimitiveType.Cube, root, new Vector3(0f, 1.35f, 0.34f), new Vector3(0.42f, 0.11f, 0.08f), accent);
                    PrimitiveFactory.CreatePrimitive("Backpack", PrimitiveType.Cube, root, new Vector3(0f, 0.82f, -0.32f), new Vector3(0.4f, 0.55f, 0.18f), dark);
                    PrimitiveFactory.CreatePrimitive("LegA", PrimitiveType.Cylinder, root, new Vector3(-0.2f, 0.28f, 0f), new Vector3(0.08f, 0.32f, 0.08f), secondary);
                    PrimitiveFactory.CreatePrimitive("LegB", PrimitiveType.Cylinder, root, new Vector3(0.2f, 0.28f, 0f), new Vector3(0.08f, 0.32f, 0.08f), secondary);
                    break;
                case EnemyVisualStyle.SiegeCrawler:
                    PrimitiveFactory.CreatePrimitive("Chassis", PrimitiveType.Cube, root, new Vector3(0f, 0.68f, 0f), new Vector3(1.55f, 0.7f, 1.72f), primary);
                    PrimitiveFactory.CreatePrimitive("Turret", PrimitiveType.Cylinder, root, new Vector3(0f, 1.18f, 0f), new Vector3(0.72f, 0.18f, 0.72f), secondary);
                    PrimitiveFactory.CreatePrimitive("Ram", PrimitiveType.Cube, root, new Vector3(0f, 0.68f, 1.02f), new Vector3(0.5f, 0.26f, 0.52f), accent);
                    PrimitiveFactory.CreatePrimitive("TrackLeft", PrimitiveType.Cube, root, new Vector3(-0.86f, 0.36f, 0f), new Vector3(0.24f, 0.28f, 1.68f), dark);
                    PrimitiveFactory.CreatePrimitive("TrackRight", PrimitiveType.Cube, root, new Vector3(0.86f, 0.36f, 0f), new Vector3(0.24f, 0.28f, 1.68f), dark);
                    break;
                case EnemyVisualStyle.ShieldWarden:
                    PrimitiveFactory.CreatePrimitive("Core", PrimitiveType.Sphere, root, new Vector3(0f, 1.08f, 0f), new Vector3(0.82f, 0.82f, 0.82f), primary);
                    PrimitiveFactory.CreatePrimitive("LowerDisc", PrimitiveType.Cylinder, root, new Vector3(0f, 0.84f, 0f), new Vector3(0.96f, 0.08f, 0.96f), secondary);
                    PrimitiveFactory.CreatePrimitive("UpperDisc", PrimitiveType.Cylinder, root, new Vector3(0f, 1.26f, 0f), new Vector3(0.72f, 0.06f, 0.72f), dark);
                    PrimitiveFactory.CreatePrimitive("FrontLens", PrimitiveType.Sphere, root, new Vector3(0f, 1.08f, 0.42f), new Vector3(0.24f, 0.24f, 0.24f), accent);
                    PrimitiveFactory.CreatePrimitive("Antenna", PrimitiveType.Cylinder, root, new Vector3(0f, 1.7f, 0f), new Vector3(0.08f, 0.24f, 0.08f), accent);
                    break;
            }
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
