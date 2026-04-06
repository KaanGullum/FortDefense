using FortDefense.Data;
using FortDefense.Utilities;
using UnityEngine;

namespace FortDefense.Buildings
{
    public struct BuildingVisualResult
    {
        public Transform TurretPivot;
        public Transform Muzzle;
    }

    public static class BuildingVisualFactory
    {
        public static BuildingVisualResult BuildVisual(BuildingDefinition definition, Transform root)
        {
            BuildingVisualResult result = new BuildingVisualResult();

            switch (definition.VisualStyle)
            {
                case BuildingVisualStyle.GunTower:
                    CreateGunTower(root, ref result);
                    break;
                case BuildingVisualStyle.CannonTower:
                    CreateCannonTower(root, ref result);
                    break;
                case BuildingVisualStyle.MortarTower:
                    CreateMortarTower(root, ref result);
                    break;
                case BuildingVisualStyle.Mine:
                    CreateMine(root);
                    break;
                case BuildingVisualStyle.Generator:
                    CreateGenerator(root);
                    break;
                case BuildingVisualStyle.Smelter:
                    CreateSmelter(root);
                    break;
                case BuildingVisualStyle.AmmoFactory:
                    CreateAmmoFactory(root);
                    break;
            }

            return result;
        }

        private static void CreateGunTower(Transform root, ref BuildingVisualResult result)
        {
            PrimitiveFactory.CreatePrimitive("Base", PrimitiveType.Cylinder, root, new Vector3(0f, 0.45f, 0f), new Vector3(1.1f, 0.45f, 1.1f), new Color(0.62f, 0.62f, 0.67f));
            PrimitiveFactory.CreatePrimitive("Column", PrimitiveType.Cylinder, root, new Vector3(0f, 1.05f, 0f), new Vector3(0.52f, 0.6f, 0.52f), new Color(0.45f, 0.45f, 0.5f));

            GameObject pivot = new GameObject("TurretPivot");
            pivot.transform.SetParent(root, false);
            pivot.transform.localPosition = new Vector3(0f, 1.4f, 0f);
            result.TurretPivot = pivot.transform;

            PrimitiveFactory.CreatePrimitive("Turret", PrimitiveType.Cube, pivot.transform, new Vector3(0f, 0.1f, 0f), new Vector3(0.95f, 0.45f, 0.95f), new Color(0.23f, 0.33f, 0.45f));
            PrimitiveFactory.CreatePrimitive("Barrel", PrimitiveType.Cylinder, pivot.transform, new Vector3(0f, 0.12f, 0.72f), Quaternion.Euler(90f, 0f, 0f), new Vector3(0.18f, 0.55f, 0.18f), new Color(0.16f, 0.18f, 0.2f));

            GameObject muzzle = new GameObject("Muzzle");
            muzzle.transform.SetParent(pivot.transform, false);
            muzzle.transform.localPosition = new Vector3(0f, 0.12f, 1.24f);
            result.Muzzle = muzzle.transform;
        }

        private static void CreateCannonTower(Transform root, ref BuildingVisualResult result)
        {
            PrimitiveFactory.CreatePrimitive("Base", PrimitiveType.Cylinder, root, new Vector3(0f, 0.48f, 0f), new Vector3(1.2f, 0.48f, 1.2f), new Color(0.72f, 0.64f, 0.48f));
            PrimitiveFactory.CreatePrimitive("Platform", PrimitiveType.Cube, root, new Vector3(0f, 1.05f, 0f), new Vector3(1.35f, 0.18f, 1.35f), new Color(0.55f, 0.44f, 0.28f));

            GameObject pivot = new GameObject("TurretPivot");
            pivot.transform.SetParent(root, false);
            pivot.transform.localPosition = new Vector3(0f, 1.15f, 0f);
            result.TurretPivot = pivot.transform;

            PrimitiveFactory.CreatePrimitive("Body", PrimitiveType.Cube, pivot.transform, new Vector3(0f, 0.3f, 0f), new Vector3(1.2f, 0.6f, 1f), new Color(0.28f, 0.24f, 0.2f));
            PrimitiveFactory.CreatePrimitive("Barrel", PrimitiveType.Cylinder, pivot.transform, new Vector3(0f, 0.32f, 0.95f), Quaternion.Euler(90f, 0f, 0f), new Vector3(0.28f, 0.82f, 0.28f), new Color(0.14f, 0.14f, 0.16f));

            GameObject muzzle = new GameObject("Muzzle");
            muzzle.transform.SetParent(pivot.transform, false);
            muzzle.transform.localPosition = new Vector3(0f, 0.32f, 1.7f);
            result.Muzzle = muzzle.transform;
        }

        private static void CreateMortarTower(Transform root, ref BuildingVisualResult result)
        {
            PrimitiveFactory.CreatePrimitive("Base", PrimitiveType.Cylinder, root, new Vector3(0f, 0.45f, 0f), new Vector3(1.18f, 0.45f, 1.18f), new Color(0.47f, 0.36f, 0.3f));
            PrimitiveFactory.CreatePrimitive("Plate", PrimitiveType.Cube, root, new Vector3(0f, 0.9f, 0f), new Vector3(1.35f, 0.16f, 1.35f), new Color(0.36f, 0.29f, 0.24f));

            GameObject pivot = new GameObject("TurretPivot");
            pivot.transform.SetParent(root, false);
            pivot.transform.localPosition = new Vector3(0f, 1.02f, 0f);
            result.TurretPivot = pivot.transform;

            PrimitiveFactory.CreatePrimitive("MortarBody", PrimitiveType.Sphere, pivot.transform, new Vector3(0f, 0.26f, 0f), new Vector3(0.96f, 0.56f, 0.96f), new Color(0.17f, 0.22f, 0.27f));
            PrimitiveFactory.CreatePrimitive("Bowl", PrimitiveType.Cylinder, pivot.transform, new Vector3(0f, 0.22f, 0f), Quaternion.Euler(90f, 0f, 0f), new Vector3(0.24f, 0.4f, 0.24f), new Color(0.12f, 0.12f, 0.14f));

            GameObject muzzle = new GameObject("Muzzle");
            muzzle.transform.SetParent(pivot.transform, false);
            muzzle.transform.localPosition = new Vector3(0f, 0.75f, 0f);
            result.Muzzle = muzzle.transform;
        }

        private static void CreateMine(Transform root)
        {
            PrimitiveFactory.CreatePrimitive("Base", PrimitiveType.Cube, root, new Vector3(0f, 0.35f, 0f), new Vector3(1.4f, 0.7f, 1.4f), new Color(0.4f, 0.3f, 0.24f));
            PrimitiveFactory.CreatePrimitive("Drill", PrimitiveType.Cylinder, root, new Vector3(0f, 1.05f, 0.1f), Quaternion.Euler(0f, 0f, 35f), new Vector3(0.22f, 0.7f, 0.22f), new Color(0.7f, 0.62f, 0.32f));
            PrimitiveFactory.CreatePrimitive("Ore", PrimitiveType.Capsule, root, new Vector3(0.48f, 0.82f, -0.32f), new Vector3(0.4f, 0.45f, 0.4f), new Color(0.22f, 0.68f, 0.85f));
        }

        private static void CreateGenerator(Transform root)
        {
            PrimitiveFactory.CreatePrimitive("Base", PrimitiveType.Cube, root, new Vector3(0f, 0.35f, 0f), new Vector3(1.4f, 0.7f, 1.4f), new Color(0.22f, 0.32f, 0.45f));
            PrimitiveFactory.CreatePrimitive("Core", PrimitiveType.Sphere, root, new Vector3(0f, 1.1f, 0f), new Vector3(0.6f, 0.6f, 0.6f), new Color(0.94f, 0.83f, 0.35f));
            PrimitiveFactory.CreatePrimitive("PylonA", PrimitiveType.Cylinder, root, new Vector3(-0.42f, 1f, 0f), new Vector3(0.12f, 0.7f, 0.12f), new Color(0.65f, 0.65f, 0.72f));
            PrimitiveFactory.CreatePrimitive("PylonB", PrimitiveType.Cylinder, root, new Vector3(0.42f, 1f, 0f), new Vector3(0.12f, 0.7f, 0.12f), new Color(0.65f, 0.65f, 0.72f));
        }

        private static void CreateSmelter(Transform root)
        {
            PrimitiveFactory.CreatePrimitive("Furnace", PrimitiveType.Cube, root, new Vector3(0f, 0.55f, 0f), new Vector3(1.55f, 1.1f, 1.2f), new Color(0.38f, 0.23f, 0.19f));
            PrimitiveFactory.CreatePrimitive("Chimney", PrimitiveType.Cylinder, root, new Vector3(0.4f, 1.55f, -0.25f), new Vector3(0.22f, 0.7f, 0.22f), new Color(0.24f, 0.24f, 0.26f));
            PrimitiveFactory.CreatePrimitive("MoltenCore", PrimitiveType.Cube, root, new Vector3(-0.25f, 0.55f, 0.62f), new Vector3(0.4f, 0.4f, 0.08f), new Color(0.95f, 0.47f, 0.16f));
        }

        private static void CreateAmmoFactory(Transform root)
        {
            PrimitiveFactory.CreatePrimitive("Base", PrimitiveType.Cube, root, new Vector3(0f, 0.45f, 0f), new Vector3(1.6f, 0.9f, 1.3f), new Color(0.31f, 0.35f, 0.24f));
            PrimitiveFactory.CreatePrimitive("Loader", PrimitiveType.Cylinder, root, new Vector3(0.45f, 1.05f, 0f), Quaternion.Euler(90f, 0f, 0f), new Vector3(0.18f, 0.45f, 0.18f), new Color(0.66f, 0.63f, 0.36f));
            PrimitiveFactory.CreatePrimitive("CrateA", PrimitiveType.Cube, root, new Vector3(-0.4f, 1.05f, 0.22f), new Vector3(0.38f, 0.38f, 0.38f), new Color(0.5f, 0.41f, 0.21f));
            PrimitiveFactory.CreatePrimitive("CrateB", PrimitiveType.Cube, root, new Vector3(-0.05f, 1.22f, -0.2f), new Vector3(0.38f, 0.38f, 0.38f), new Color(0.5f, 0.41f, 0.21f));
        }
    }
}

