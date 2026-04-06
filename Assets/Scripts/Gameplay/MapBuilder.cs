using System.Collections.Generic;
using FortDefense.Data;
using FortDefense.Utilities;
using UnityEngine;

namespace FortDefense.Gameplay
{
    public class MapBuilder
    {
        private static readonly Color TerrainColor = new Color(0.44f, 0.61f, 0.37f);
        private static readonly Color RoadColor = new Color(0.34f, 0.31f, 0.29f);
        private static readonly Color WallColor = new Color(0.69f, 0.67f, 0.72f);
        private static readonly Color AccentColor = new Color(0.74f, 0.39f, 0.2f);

        public MapBuildResult Build(Transform root)
        {
            BattleMapLayout layout = BattleMapLayout.CreateDefault();
            MapBuildResult result = new MapBuildResult();

            GameObject terrain = PrimitiveFactory.CreatePrimitive(
                "Terrain",
                PrimitiveType.Cube,
                root,
                new Vector3(2f, -0.8f, -1f),
                new Vector3(50f, 1.2f, 34f),
                TerrainColor);

            terrain.GetComponent<Renderer>().receiveShadows = true;

            Transform roadRoot = new GameObject("Road").transform;
            roadRoot.SetParent(root, false);

            for (int index = 0; index < layout.RoadCells.Count; index++)
            {
                PrimitiveFactory.CreatePrimitive(
                    "RoadTile_" + index,
                    PrimitiveType.Cube,
                    roadRoot,
                    BattleMapLayout.ToWorld(layout.RoadCells[index], 0.05f),
                    new Vector3(1.9f, 0.15f, 1.9f),
                    RoadColor);
            }

            Transform slotRoot = new GameObject("BuildTiles").transform;
            slotRoot.SetParent(root, false);

            for (int index = 0; index < layout.BuildTileCells.Count; index++)
            {
                Vector2Int cell = layout.BuildTileCells[index];
                GameObject tileObject = new GameObject("BuildTile_" + cell.x + "_" + cell.y);
                tileObject.transform.SetParent(slotRoot, false);
                tileObject.transform.position = BattleMapLayout.ToWorld(cell);

                GameObject tileBase = PrimitiveFactory.CreatePrimitive(
                    "TileBase",
                    PrimitiveType.Cube,
                    tileObject.transform,
                    new Vector3(0f, 0.08f, 0f),
                    new Vector3(1.8f, 0.18f, 1.8f),
                    new Color(0.27f, 0.47f, 0.67f),
                    true);

                Renderer tileRenderer = tileBase.GetComponent<Renderer>();
                GameObject highlight = PrimitiveFactory.CreatePrimitive(
                    "SelectionHighlight",
                    PrimitiveType.Cube,
                    tileObject.transform,
                    new Vector3(0f, 0.02f, 0f),
                    new Vector3(2.02f, 0.04f, 2.02f),
                    new Color(0.95f, 0.83f, 0.32f));

                BuildTileAffinity affinity = layout.TileAffinities.ContainsKey(cell)
                    ? layout.TileAffinities[cell]
                    : BuildTileAffinity.Neutral;

                CreateAffinityMarker(tileObject.transform, affinity);

                BuildTile buildTile = tileObject.AddComponent<BuildTile>();
                buildTile.Initialize(cell, tileRenderer, highlight.GetComponent<Renderer>(), affinity);
                result.BuildTiles.Add(buildTile);
            }

            CreateFortWalls(root);

            GameObject coreRoot = new GameObject("Core");
            coreRoot.transform.SetParent(root, false);
            coreRoot.transform.position = BattleMapLayout.ToWorld(layout.CoreCell);

            PrimitiveFactory.CreatePrimitive(
                "CoreBase",
                PrimitiveType.Cylinder,
                coreRoot.transform,
                new Vector3(0f, 0.75f, 0f),
                new Vector3(3f, 1.2f, 3f),
                WallColor);

            PrimitiveFactory.CreatePrimitive(
                "CoreCrystal",
                PrimitiveType.Capsule,
                coreRoot.transform,
                new Vector3(0f, 2.3f, 0f),
                new Vector3(1.1f, 1.5f, 1.1f),
                AccentColor);

            PrimitiveFactory.CreatePrimitive(
                "CoreGate",
                PrimitiveType.Cube,
                coreRoot.transform,
                new Vector3(-2.1f, 0.95f, 0f),
                new Vector3(0.8f, 1.8f, 3f),
                WallColor);

            result.CoreObject = coreRoot;
            result.CorePosition = coreRoot.transform.position;
            result.PathPoints.AddRange(layout.GetWorldPathPoints());
            result.SpawnPosition = result.PathPoints[0];
            result.MapCenter = new Vector3(2f, 0f, -1f);

            return result;
        }

        private static void CreateFortWalls(Transform root)
        {
            Transform wallRoot = new GameObject("Walls").transform;
            wallRoot.SetParent(root, false);

            CreateWallSegment(wallRoot, new Vector3(2f, 0.8f, 15f), new Vector3(46f, 1.6f, 1.2f));
            CreateWallSegment(wallRoot, new Vector3(2f, 0.8f, -17f), new Vector3(46f, 1.6f, 1.2f));
            CreateWallSegment(wallRoot, new Vector3(-21f, 0.8f, -1f), new Vector3(1.2f, 1.6f, 31f));
            CreateWallSegment(wallRoot, new Vector3(25f, 0.8f, -1f), new Vector3(1.2f, 1.6f, 31f));

            CreateWallSegment(wallRoot, new Vector3(16f, 0.8f, 4f), new Vector3(8f, 1.6f, 1.2f));
            CreateWallSegment(wallRoot, new Vector3(16f, 0.8f, -4f), new Vector3(8f, 1.6f, 1.2f));
            CreateWallSegment(wallRoot, new Vector3(20f, 0.8f, 0f), new Vector3(1.2f, 1.6f, 9f));
        }

        private static void CreateWallSegment(Transform parent, Vector3 position, Vector3 scale)
        {
            PrimitiveFactory.CreatePrimitive(
                "Wall",
                PrimitiveType.Cube,
                parent,
                position,
                scale,
                WallColor);
        }

        private static void CreateAffinityMarker(Transform parent, BuildTileAffinity affinity)
        {
            switch (affinity)
            {
                case BuildTileAffinity.OreNode:
                    PrimitiveFactory.CreatePrimitive(
                        "OreMarker",
                        PrimitiveType.Capsule,
                        parent,
                        new Vector3(0.48f, 0.48f, -0.48f),
                        new Vector3(0.22f, 0.38f, 0.22f),
                        new Color(0.22f, 0.8f, 0.95f));
                    break;
                case BuildTileAffinity.PowerNode:
                    PrimitiveFactory.CreatePrimitive(
                        "PowerMarker",
                        PrimitiveType.Cylinder,
                        parent,
                        new Vector3(0.5f, 0.42f, -0.5f),
                        Quaternion.Euler(90f, 0f, 0f),
                        new Vector3(0.1f, 0.26f, 0.1f),
                        new Color(0.95f, 0.82f, 0.25f));
                    PrimitiveFactory.CreatePrimitive(
                        "PowerOrb",
                        PrimitiveType.Sphere,
                        parent,
                        new Vector3(0.5f, 0.62f, -0.5f),
                        new Vector3(0.22f, 0.22f, 0.22f),
                        new Color(0.97f, 0.89f, 0.38f));
                    break;
                case BuildTileAffinity.IndustryPad:
                    PrimitiveFactory.CreatePrimitive(
                        "IndustryMarker",
                        PrimitiveType.Cube,
                        parent,
                        new Vector3(0.5f, 0.36f, -0.5f),
                        new Vector3(0.32f, 0.22f, 0.32f),
                        new Color(0.88f, 0.5f, 0.22f));
                    break;
            }
        }
    }
}
