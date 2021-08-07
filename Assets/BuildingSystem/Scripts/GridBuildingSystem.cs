using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBuildingSystem : MonoBehaviour
{
    [SerializeField] private List<PlacedObjectTypeSO> placedObjectTypesList;
    private PlacedObjectTypeSO placedObjectType;
    private GridMap<GridObject> grid;
    private PlacedObjectTypeSO.Dir dir = PlacedObjectTypeSO.Dir.Down;

    public static GridBuildingSystem Instance { get; private set; }

    public event EventHandler OnSelectedChanged;

    private void Awake() {
        {
            Instance = this;

            int gridWidth = 10;
            int gridHeight = 10;
            float cellSize = 5f;
            grid = new GridMap<GridObject>(gridWidth, gridHeight, cellSize, (GridMap<GridObject> g, int x, int z) => new GridObject(g, x, z));
        }
    }

    private void Start() {
        placedObjectType = placedObjectTypesList[2];
        PlaceBuilding(0, 0);
        placedObjectType = placedObjectTypesList[0];

    }

    public class GridObject {

        private GridMap<GridObject> grid;
        private int x;
        private int z;
        private PlacedObject placedObject;

        public GridObject(GridMap<GridObject> grid, int x, int z) {
            this.grid = grid;
            this.x = x;
            this.z = z;
        }

        public void SetPlacedObject(PlacedObject placed) {
            this.placedObject = placed;
            grid.TriggerGridObjectChanged(x, z);
        }

        public PlacedObject GetPlacedObject() {
            return placedObject;
        }

        public void ClearPlacedObject() {
            placedObject = null;
            grid.TriggerGridObjectChanged(x, z);
        }

        public bool CanBuild() {
            return placedObject == null;
        }

        public override string ToString() {
            return "(" + x + ", " + z + ")\n" + placedObject;
        }
    }

    public void RotateBuilding() {
        dir = PlacedObjectTypeSO.GetNextDir(dir);
    }

    public void SelectBuilding(int building) {
        building--;
        if (placedObjectTypesList.Count <= building || building < 0) {
            return;
        }
        placedObjectType = placedObjectTypesList[building];
    }

    public void PlaceBuilding() {
        grid.GetXZ(Utils.GetMouseWorldPosition(), out int x, out int z);

        PlaceBuilding(x, z);
    }

    private void PlaceBuilding(int x, int z) {
        PlacedObjectTypeSO toPlace = placedObjectType;
        List<Vector2Int> gridPositionList = toPlace.GetGridPositionList(new Vector2Int(x, z), dir);

        bool buildable = true;
        foreach (Vector2Int pos in gridPositionList) {
            if (pos.x < 0 || pos.x >= grid.GetWidth() || pos.y < 0 || pos.y >= grid.GetHeight()) {
                // Out of bounds
                return;
            }
            if (!grid.GetGridObject(pos.x, pos.y).CanBuild()) {
                // Cannot build here
                buildable = false;
                break;
            }
        }

        if (buildable) {
            Vector2Int rotationOffset = placedObjectType.GetRotationOffset(dir);
            Vector3 builtWorldPosition = grid.GetWorldPosition(x, z) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * grid.GetCellSize();
            PlacedObject built = PlacedObject.Create(builtWorldPosition, new Vector2Int(x, z), dir, placedObjectType);

            foreach (Vector2Int gridPosition in gridPositionList) {
                grid.GetGridObject(gridPosition.x, gridPosition.y).SetPlacedObject(built);
                Pathfinding.Instance.GetNode(gridPosition.x, gridPosition.y).SetWalkable(false);
            }
        }
    }

    public void DemolishBuilding() {
        GridObject gridObject = grid.GetGridObject(Utils.GetMouseWorldPosition());
        PlacedObject placedObject = gridObject.GetPlacedObject();
        if (placedObject != null) {
            placedObject.DestroySelf();

            List<Vector2Int> gridPositionList = placedObject.GetGridPositionList();

            foreach (Vector2Int pos in gridPositionList) {
                grid.GetGridObject(pos.x, pos.y).ClearPlacedObject();
                Pathfinding.Instance.GetNode(pos.x, pos.y).SetWalkable(true);
            }
        }
    }

    public Vector3 GetMouseWorldSnappedPosition() {
        Vector3 mousePosition = Utils.GetMouseWorldPosition();
        grid.GetXZ(mousePosition, out int x, out int z);

        if (placedObjectType != null) {
            Vector2Int rotationOffset = placedObjectType.GetRotationOffset(dir);
            Vector3 placedObjectWorldPosition = grid.GetWorldPosition(x, z) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * grid.GetCellSize();
            return placedObjectWorldPosition;
        } else {
            return mousePosition;
        }
    }

    private void RefreshSelectedObjectType() {
        OnSelectedChanged?.Invoke(this, EventArgs.Empty);
    }

    public Quaternion GetPlacedObjectRotation() {
        if (placedObjectType != null) {
            return Quaternion.Euler(0, placedObjectType.GetRotationAngle(dir), 0);
        } else {
            return Quaternion.identity;
        }
    }

    public PlacedObjectTypeSO GetPlacedObjectTypeSO() {
        return placedObjectType;
    }

    public PlacedObject GetPlacedObject() {
        Vector3 mousePos = Utils.GetMouseWorldPosition();
        grid.GetXZ(mousePos, out int x, out int z);
        GridObject gridObject = grid.GetGridObject(x, z);
        PlacedObject placedObject = gridObject.GetPlacedObject();
        return placedObject;
    }
}
