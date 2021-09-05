using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode {

    private GridMap3D<GridNode3D> grid;
    public int x;
    public int y;
    public int z;
    const int fullTraversal = 67108863; // 26 1s

    private int canExit;
    private int canEnter;

    public int gCost;
    public int hCost;
    public int fCost;

    private bool walkable;

    public PathNode prevNode;

    public PathNode(GridMap3D<GridNode3D> grid, int x, int y, int z) {
        this.grid = grid;
        this.x = x;
        this.y = y;
        this.z = z;
        this.walkable = true;
    }

    public void CalculateFCost() {
        fCost = gCost + hCost;
    }

    public Vector3 GetCenter() {
        Vector3 basePos = new Vector3(x * grid.GetCellSize(), 0, z * grid.GetCellSize());
        Vector3 correction = new Vector3(grid.GetCellSize() / 2, 0, grid.GetCellSize() / 2);
        return basePos + correction;
    }

    public override string ToString() {
        if (!walkable) {
            return "WALL";
        }
        return x + "," + y;
    }

    public bool IsWalkable() {
        return walkable;
    }
    public void SetWalkable(bool value) {
        walkable = value;
        grid.TriggerGridObjectChanged(x, y);
    }
}
