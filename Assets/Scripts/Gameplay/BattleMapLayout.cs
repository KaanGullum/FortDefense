using System.Collections.Generic;
using FortDefense.Data;
using UnityEngine;

namespace FortDefense.Gameplay
{
    public class BattleMapLayout
    {
        public const float CellSize = 2f;

        public readonly List<Vector2Int> PathWaypoints = new List<Vector2Int>();
        public readonly List<Vector2Int> RoadCells = new List<Vector2Int>();
        public readonly List<Vector2Int> BuildTileCells = new List<Vector2Int>();
        public readonly Dictionary<Vector2Int, BuildTileAffinity> TileAffinities = new Dictionary<Vector2Int, BuildTileAffinity>();

        public Vector2Int CoreCell;

        public static BattleMapLayout CreateDefault()
        {
            BattleMapLayout layout = new BattleMapLayout();

            layout.PathWaypoints.Add(new Vector2Int(-9, -5));
            layout.PathWaypoints.Add(new Vector2Int(-6, -5));
            layout.PathWaypoints.Add(new Vector2Int(-6, 3));
            layout.PathWaypoints.Add(new Vector2Int(-1, 3));
            layout.PathWaypoints.Add(new Vector2Int(-1, -1));
            layout.PathWaypoints.Add(new Vector2Int(4, -1));
            layout.PathWaypoints.Add(new Vector2Int(4, 5));
            layout.PathWaypoints.Add(new Vector2Int(8, 5));
            layout.PathWaypoints.Add(new Vector2Int(8, 0));
            layout.PathWaypoints.Add(new Vector2Int(10, 0));

            for (int index = 0; index < layout.PathWaypoints.Count - 1; index++)
            {
                AddRoadSegment(layout.RoadCells, layout.PathWaypoints[index], layout.PathWaypoints[index + 1]);
            }

            layout.CoreCell = new Vector2Int(11, 0);

            AddBuildTile(layout, -8, -3);
            AddBuildTile(layout, -7, -3);
            AddBuildTile(layout, -8, -7);
            AddBuildTile(layout, -7, -7);
            AddBuildTile(layout, -8, -2);
            AddBuildTile(layout, -4, -2);
            AddBuildTile(layout, -8, 0);
            AddBuildTile(layout, -4, 0);
            AddBuildTile(layout, -8, 2);
            AddBuildTile(layout, -4, 2);
            AddBuildTile(layout, -5, 5);
            AddBuildTile(layout, -3, 5);
            AddBuildTile(layout, -1, 5);
            AddBuildTile(layout, -5, 1);
            AddBuildTile(layout, -3, 1);
            AddBuildTile(layout, 0, 1);
            AddBuildTile(layout, 2, 1);
            AddBuildTile(layout, 4, 1);
            AddBuildTile(layout, 0, -3);
            AddBuildTile(layout, 2, -3);
            AddBuildTile(layout, 4, -3);
            AddBuildTile(layout, 2, 2);
            AddBuildTile(layout, 6, 2);
            AddBuildTile(layout, 2, 4);
            AddBuildTile(layout, 6, 4);
            AddBuildTile(layout, 5, 7);
            AddBuildTile(layout, 7, 7);
            AddBuildTile(layout, 5, 3);
            AddBuildTile(layout, 7, 3);
            AddBuildTile(layout, 6, 0);
            AddBuildTile(layout, 10, 2);
            AddBuildTile(layout, 10, -2);

            SetAffinity(layout, -8, -7, BuildTileAffinity.OreNode);
            SetAffinity(layout, -7, -7, BuildTileAffinity.OreNode);
            SetAffinity(layout, -5, 5, BuildTileAffinity.OreNode);
            SetAffinity(layout, -3, 5, BuildTileAffinity.OreNode);

            SetAffinity(layout, 10, 2, BuildTileAffinity.PowerNode);
            SetAffinity(layout, 10, -2, BuildTileAffinity.PowerNode);
            SetAffinity(layout, 7, 7, BuildTileAffinity.PowerNode);

            SetAffinity(layout, 6, 2, BuildTileAffinity.IndustryPad);
            SetAffinity(layout, 6, 4, BuildTileAffinity.IndustryPad);
            SetAffinity(layout, 5, 3, BuildTileAffinity.IndustryPad);
            SetAffinity(layout, 7, 3, BuildTileAffinity.IndustryPad);

            return layout;
        }

        public List<Vector3> GetWorldPathPoints()
        {
            List<Vector3> worldPoints = new List<Vector3>();
            for (int index = 0; index < PathWaypoints.Count; index++)
            {
                worldPoints.Add(ToWorld(PathWaypoints[index], 0.25f));
            }

            return worldPoints;
        }

        public static Vector3 ToWorld(Vector2Int cell, float y = 0f)
        {
            return new Vector3(cell.x * CellSize, y, cell.y * CellSize);
        }

        private static void AddRoadSegment(List<Vector2Int> roadCells, Vector2Int from, Vector2Int to)
        {
            Vector2Int direction = new Vector2Int(
                from.x == to.x ? 0 : (from.x < to.x ? 1 : -1),
                from.y == to.y ? 0 : (from.y < to.y ? 1 : -1));

            Vector2Int current = from;
            roadCells.Add(current);

            while (current != to)
            {
                current += direction;
                if (!roadCells.Contains(current))
                {
                    roadCells.Add(current);
                }
            }
        }

        private static void AddBuildTile(BattleMapLayout layout, int x, int y)
        {
            Vector2Int cell = new Vector2Int(x, y);
            if (layout.RoadCells.Contains(cell) || layout.BuildTileCells.Contains(cell))
            {
                return;
            }

            layout.BuildTileCells.Add(cell);
        }

        private static void SetAffinity(BattleMapLayout layout, int x, int y, BuildTileAffinity affinity)
        {
            Vector2Int cell = new Vector2Int(x, y);
            if (!layout.BuildTileCells.Contains(cell))
            {
                return;
            }

            layout.TileAffinities[cell] = affinity;
        }
    }
}
