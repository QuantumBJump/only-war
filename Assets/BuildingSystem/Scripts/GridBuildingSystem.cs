using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBuildingSystem : MonoBehaviour
{
    [SerializeField] private List<PlacedObjectTypeSO> placedObjectTypesList;
    private PlacedObjectTypeSO placedObjectType;
    private GridMap3D<GridObject> grid;
    private PlacedObjectTypeSO.Dir dir = PlacedObjectTypeSO.Dir.Down;

    public static GridBuildingSystem Instance { get; private set; }

    public event EventHandler OnSelectedChanged;

    private void Awake() {
        {
            Instance = this;

            int gridWidth = 10;
            int gridHeight = 3;
            int gridDepth = 10;
            float cellSize = 5f;
            grid = new GridMap3D<GridObject>(gridWidth, gridHeight, gridDepth, cellSize, (GridMap3D<GridObject> g, int x, int y, int z) => new GridObject(g, x, y, z));
        }
    }

    private void Start() {
        placedObjectType = placedObjectTypesList[2];
        PlaceBuilding(0, 0, 0);
        placedObjectType = placedObjectTypesList[0];

    }

    public class GridObject {

        private GridMap3D<GridObject> grid;
        private int x;
        private int y;
        private int z;
        private IPlaceable placedObject;

        public GridObject(GridMap3D<GridObject> grid, int x, int y, int z) {
            this.grid = grid;
            this.x = x;
            this.z = z;
        }

        public void SetPlacedObject(PlacedObject placed) {
            this.placedObject = placed;
            grid.TriggerGridObjectChanged(x, z);
        }

        public IPlaceable GetPlacedObject() {
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
        grid.GetXYZ(Utils.GetMouseWorldPosition(), out int x, out int y, out int z);

        PlaceBuilding(x, y, z);
    }

    private void PlaceBuilding(int x, int y, int z) {
        Debug.Log("Placing building at " + x + ", " + y + ", " + z);
        PlacedObjectTypeSO toPlace = placedObjectType;
        List<Vector3Int> gridPositionList = toPlace.GetGridPositionList(new Vector3Int(x, y, z), dir);

        bool buildable = true;
        foreach (Vector3Int pos in gridPositionList) {
            if (grid.InBounds(x, y, z)) {
                if (!grid.GetGridObject(pos.x, pos.y, pos.z).CanBuild()) {
                    // Cannot build here
                    buildable = false;
                    break;
                }
            } else {
                return;
            }
        }

        if (buildable) {
            Vector2Int rotationOffset = placedObjectType.GetRotationOffset(dir);
            Vector3 builtWorldPosition = grid.GetWorldPosition(x, y, z) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * grid.GetCellSize();
            PlacedObject built = PlacedObject.Create(builtWorldPosition, new Vector3Int(x, y, z), dir, placedObjectType);

            foreach (Vector3Int gridPosition in gridPositionList) {
                grid.GetGridObject(gridPosition.x, gridPosition.y, gridPosition.z).SetPlacedObject(built);
                Pathfinding.Instance.GetNode(gridPosition.x, gridPosition.z).SetWalkable(false);
            }
        }
    }

    public void DemolishBuilding() {
        GridObject gridObject = grid.GetGridObject(Utils.GetMouseWorldPosition());
        IPlaceable placedObject = gridObject.GetPlacedObject();
        if (placedObject != null) {
            placedObject.DestroySelf();

            List<Vector3Int> gridPositionList = placedObject.GetGridPositionList();

            foreach (Vector3Int pos in gridPositionList) {
                grid.GetGridObject(pos.x, pos.y, pos.z).ClearPlacedObject();
                Pathfinding.Instance.GetNode(pos.x, pos.y).SetWalkable(true);
            }
        }
    }

    public Vector3 GetMouseWorldSnappedPosition() {
        Vector3 mousePosition = Utils.GetMouseWorldPosition();
        grid.GetXYZ(mousePosition, out int x, out int y, out int z);

        if (placedObjectType != null) {
            Vector2Int rotationOffset = placedObjectType.GetRotationOffset(dir);
            Vector3 placedObjectWorldPosition = grid.GetWorldPosition(x, y, z) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * grid.GetCellSize();
            return placedObjectWorldPosition;
        } else {
            return mousePosition;
        }
    }

    public Vector3 SnapWorldPositionToGrid(Vector3 worldPosition) {
        grid.GetXYZ(worldPosition, out int x, out int y, out int z);

        if (placedObjectType != null) {
            Vector2Int rotationOffset = placedObjectType.GetRotationOffset(dir);
            Vector3 placedObjectWorldPosition = grid.GetWorldPosition(x, y, z) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * grid.GetCellSize();
            return placedObjectWorldPosition;
        } else {
            return worldPosition;
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

    public IPlaceable GetPlacedObject() {
        Vector3 mousePos = Utils.GetMouseWorldPosition();
        grid.GetXYZ(mousePos, out int x, out int y, out int z);
        GridObject gridObject = grid.GetGridObject(x, y, z);
        IPlaceable placedObject = gridObject.GetPlacedObject();
        return placedObject;
    }
}
