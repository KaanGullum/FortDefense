using UnityEngine;
using UnityEngine.EventSystems;

namespace FortDefense.Gameplay
{
    public class BattleInputController : MonoBehaviour
    {
        private Camera _worldCamera;
        private TileSelectionController _tileSelectionController;

        public void Initialize(Camera worldCamera, TileSelectionController tileSelectionController)
        {
            _worldCamera = worldCamera;
            _tileSelectionController = tileSelectionController;
        }

        private void Update()
        {
            Vector2 screenPosition;
            int pointerId;
            if (!TryGetTap(out screenPosition, out pointerId))
            {
                return;
            }

            EventSystem eventSystem = EventSystem.current;
            if (eventSystem != null && eventSystem.IsPointerOverGameObject(pointerId))
            {
                return;
            }

            Ray ray = _worldCamera.ScreenPointToRay(screenPosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 200f))
            {
                BuildTile tile = hit.collider.GetComponentInParent<BuildTile>();
                if (tile != null)
                {
                    _tileSelectionController.SelectTile(tile);
                    return;
                }
            }

            _tileSelectionController.ClearSelection();
        }

        private static bool TryGetTap(out Vector2 screenPosition, out int pointerId)
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Ended)
                {
                    screenPosition = touch.position;
                    pointerId = touch.fingerId;
                    return true;
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                screenPosition = Input.mousePosition;
                pointerId = -1;
                return true;
            }

            screenPosition = Vector2.zero;
            pointerId = -1;
            return false;
        }
    }
}

