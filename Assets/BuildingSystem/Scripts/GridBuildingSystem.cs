using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBuildingSystem : MonoBehaviour
{
    public enum PlacementMode {
        Building,
        Wall,
    }

    public void NextPlacementMode() {
        switch (placementMode) {
        case PlacementMode.Building:
            placementMode = PlacementMode.Wall;
            RefreshSelectedObjectType();
            break;
        case PlacementMode.Wall:
            placementMode = PlacementMode.Building;
            RefreshSelectedObjectType();
            break;
        default:
            break;
        }
    }
    [SerializeField] private List<PlacedObjectTypeSO> placedObjectTypesList;
    [SerializeField] private List<WallTypeSO> wallTypesList;
    private PlacedObjectTypeSO placedObjectType;
    private WallTypeSO wallType;
    private GridMap3D<GridObject> grid;
    private PlacedObjectTypeSO.Dir dir = PlacedObjectTypeSO.Dir.Down;
    private Facing facing = Facing.Down;

    public PlacementMode placementMode;

    public static GridBuildingSystem Instance { get; private set; }

    public event EventHandler OnSelectedChanged;

    private void Awake() {
        {
            Instance = this;
            this.placementMode = PlacementMode.Building;

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
        wallType = wallTypesList[0];

    }

    public class GridObject {

        private GridMap3D<GridObject> grid;
        private int x;
        private int y;
        private int z;
        private IPlaceable placedObject;
        private Wall[] walls;

        public GridObject(GridMap3D<GridObject> grid, int x, int y, int z) {
            this.grid = grid;
            this.x = x;
            this.z = z;
            this.walls = new Wall[6];
        }

        public void SetPlacedObject(PlacedObject placed) {
            this.placedObject = placed;
            grid.TriggerGridObjectChanged(x, z);
        }

        public IPlaceable GetPlacedObject() {
            return placedObject;
        }

        public void SetWall(Facing facing, Wall wall) {
            if (this.walls[(int) facing] != null) {
                Debug.Log("Destroying wall already there");
                this.walls[(int) facing].DestroySelf();
            }
            if (this.walls[(int) facing] == null) {
                Debug.Log("Setting " + facing + " wall of (" + this.x + "," + this.y + "," + this.z + ") to " + wall);
                this.walls[(int) facing] = wall;
            }
            this.walls[(int)facing] = wall;
            grid.TriggerGridObjectChanged(x, z);
        }

        public void ClearPlacedObject() {
            placedObject = null;
            grid.TriggerGridObjectChanged(x, z);
        }

        public bool CanBuild(PlacedObject building) {
            return this.placedObject == null;
        }

        public bool CanBuild(Wall wallToPlace, Facing facing) {
            Debug.Log("Checking facing: " + facing);
            if (this.walls[(int) facing] == null) {
                Debug.Log("No wall here, ok to build");
                return true;
            }
            if (this.walls[(int) facing].IsOccupied()) {
                Debug.Log("Wall " + this.walls[(int) facing] + " is occupied, not ok to build");
                return false;
            }
            Debug.Log("Wall " + this.walls[(int) facing] + " is not occupied!");
            return true;
        }
        public bool CanBuild(IPlaceable toPlace) {
            if (toPlace.GetType() == typeof(PlacedObject)) {
                return this.placedObject == null;
            } else if (toPlace.GetType() == typeof(Wall)) {
                Wall wallToPlace = (Wall) toPlace;
                Facing facing = wallToPlace.GetWallType().facing;
                Debug.Log("Checking facing: " + facing);
                if (this.walls[(int) facing] == null) {
                    Debug.Log("No wall here, ok to build");
                    return true;
                }
                if (this.walls[(int) facing].IsOccupied()) {
                    Debug.Log("Wall " + this.walls[(int) facing] + " is occupied, not ok to build");
                    return false;
                }
                Debug.Log("Wall " + this.walls[(int) facing] + " is not occupied!");
                return true;
            } else {
                return false;
            }
        }

        public override string ToString() {
            return "(" + x + ", " + z + ")\n" + placedObject;
        }
    }

    public void RotateObject() {
        if (placementMode == PlacementMode.Building) {
            dir = PlacedObjectTypeSO.GetNextDir(dir);
        } else if (placementMode == PlacementMode.Wall) {
            facing = WallTypeSO.GetNextFacing(facing);
            wallType.facing = facing;
        }
    }

    public void SelectObject(int index) {
        index--;
        if (placementMode == PlacementMode.Building) {
            if (placedObjectTypesList.Count <= index || index < 0) {
                return; // Out of range
            }
            placedObjectType = placedObjectTypesList[index];
            RefreshSelectedObjectType();
        } else if (placementMode == PlacementMode.Wall) {
            if (wallTypesList.Count <= index || index < 0) {
                return; // Out of range
            }
            wallType = wallTypesList[index];
            facing = wallType.facing;
            RefreshSelectedObjectType();
        }
    }

    public void PlaceObject() {
        if (this.placementMode == PlacementMode.Building) {
            PlaceBuilding();
        } else if (this.placementMode == PlacementMode.Wall) {
            PlaceWall();
        }
    }

    public void PlaceBuilding() {
        grid.GetXYZ(Utils.GetMouseWorldPositionAtCameraY(), out int x, out int y, out int z);

        PlaceBuilding(x, y, z);
    }

    private void PlaceBuilding(int x, int y, int z) {
        PlacedObjectTypeSO toPlace = placedObjectType;
        List<Vector3Int> gridPositionList = toPlace.GetGridPositionList(new Vector3Int(x, y, z), dir);

        bool buildable = true;
        foreach (Vector3Int pos in gridPositionList) {
            if (grid.InBounds(pos.x, pos.y, pos.z)) {
                if (!grid.GetGridObject(pos.x, pos.y, pos.z).CanBuild(toPlace.GetPlaceable())) {
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
                Pathfinding.Instance.GetNode(gridPosition.x, gridPosition.y, gridPosition.z).SetWalkable(false);
            }
        }
    }

    public void PlaceWall() {
        grid.GetXYZ(Utils.GetMouseWorldPositionAtCameraY(), out int x, out int y, out int z);

        PlaceWall(x, y, z);
    }

    private void PlaceWall(int x, int y, int z) {
        WallTypeSO toPlace = wallType;
        Wall wallToPlace = (Wall) toPlace.GetPlaceable();
        bool buildable = true;
        if (grid.InBounds(x, y, z)) {
            if (!grid.GetGridObject(x, y, z).CanBuild(wallToPlace, facing)) {
                buildable = false;
            }
        } else {
            buildable = false;
        }

        if (buildable) {
            Debug.Log("Can build!");
            Vector2Int rotationOffset = toPlace.GetRotationOffset(facing);
            Vector3 builtWorldPosition = grid.GetWorldPosition(x, y, z) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * grid.GetCellSize();
            Wall built = Wall.Create(builtWorldPosition, facing, toPlace);

            grid.GetGridObject(x, y, z).SetWall(facing, built);
        } else {
            Debug.Log("Cannot place wall here!");
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
                Pathfinding.Instance.GetNode(pos.x, pos.y, pos.z).SetWalkable(true);
            }
        }
    }

    public Vector3 GetMouseWorldSnappedPosition() {
        Vector3 mousePosition = Utils.GetMouseWorldPosition();
        return SnapWorldPositionToGrid(mousePosition);
    }

    public Vector3 SnapWorldPositionToGrid(Vector3 worldPosition) {
        grid.GetXYZ(worldPosition, out int x, out int y, out int z);
        if (placementMode == PlacementMode.Building) {
            if (placedObjectType != null) {
                Vector2Int rotationOffset = placedObjectType.GetRotationOffset(dir);
                Vector3 placedObjectWorldPosition = grid.GetWorldPosition(x, y, z) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * grid.GetCellSize();
                return placedObjectWorldPosition;
            } else {
                return worldPosition;
            }
        } else if (placementMode == PlacementMode.Wall) {
            if (wallType != null) {
                Vector2 rotationOffset = wallType.GetRotationOffset(facing);
                Vector3 wallWorldPosition = grid.GetWorldPosition(x, y, z) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * grid.GetCellSize();
                return wallWorldPosition;
            } else {
                return worldPosition;
            }
        } else {
            return worldPosition;
        }

    }

    private void RefreshSelectedObjectType() {
        OnSelectedChanged?.Invoke(this, EventArgs.Empty);
    }

    public Quaternion GetPlacedObjectRotation() {
        if (placementMode == PlacementMode.Building) {
            if (placedObjectType != null) {
                return Quaternion.Euler(0, placedObjectType.GetRotationAngle(dir), 0);
            } else {
                return Quaternion.identity;
            }
        } else if (placementMode == PlacementMode.Wall) {
            if (wallType != null) {
                return Quaternion.Euler(0, wallType.GetRotationAngle(facing), 0);
            } else {
                return Quaternion.identity;
            }
        } else {
            return Quaternion.identity;
        }
    }

    public PlacedObjectTypeSO GetPlacedObjectTypeSO() {
        return placedObjectType;
    }

    public WallTypeSO GetWallTypeSO() {
        return wallType;
    }

    public IPlaceable GetPlacedObject() {
        Vector3 mousePos = Utils.GetMouseWorldPositionWithCollider(Utils.unitLayerMask);
        grid.GetXYZ(mousePos, out int x, out int y, out int z);
        GridObject gridObject = grid.GetGridObject(x, y, z);
        if (gridObject == null) {
            return null;
        }
        IPlaceable placedObject = gridObject.GetPlacedObject();
        return placedObject;
    }
}
