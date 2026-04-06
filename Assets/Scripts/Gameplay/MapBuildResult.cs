using System.Collections.Generic;
using UnityEngine;

namespace FortDefense.Gameplay
{
    public class MapBuildResult
    {
        public readonly List<BuildTile> BuildTiles = new List<BuildTile>();
        public readonly List<Vector3> PathPoints = new List<Vector3>();

        public GameObject CoreObject;
        public Vector3 SpawnPosition;
        public Vector3 CorePosition;
        public Vector3 MapCenter;
    }
}

