using System;
using UnityEngine;

namespace FortDefense.Gameplay
{
    public class TileSelectionController : MonoBehaviour
    {
        public event Action<BuildTile> TileSelected;
        public event Action SelectionCleared;

        public BuildTile SelectedTile { get; private set; }

        public void SelectTile(BuildTile tile)
        {
            if (SelectedTile == tile)
            {
                TileSelected?.Invoke(tile);
                return;
            }

            if (SelectedTile != null)
            {
                SelectedTile.SetSelected(false);
            }

            SelectedTile = tile;

            if (SelectedTile != null)
            {
                SelectedTile.SetSelected(true);
                TileSelected?.Invoke(SelectedTile);
            }
            else
            {
                SelectionCleared?.Invoke();
            }
        }

        public void ClearSelection()
        {
            SelectTile(null);
        }
    }
}

