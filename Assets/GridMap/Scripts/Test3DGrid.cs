using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Facing {
    North,
    East,
    South,
    West,
    Up,
    Down,
}

public class Test3DGrid {
    public static Test3DGrid Instance {get; private set;}
    private GridMap3D<GridNode3D> grid;

    private Wall[] walls;
    public Test3DGrid(int width, int height, int depth) {
        Instance = this;
        grid = new GridMap3D<GridNode3D>(width, height, depth, 5f, (GridMap3D<GridNode3D> g, int x, int y, int z) => new GridNode3D(g, x, y, z), Vector3.zero, true);
    }

}
public class GridNode3D {


    private GridMap3D<GridNode3D> grid;
    private int x;
    private int y;
    private int z;

    private PathNode pathNode;
    private PlacedObject placedObject;

    private Wall[] walls;

    public GridNode3D(GridMap3D<GridNode3D> grid, int x, int y, int z) {
        this.grid = grid;
        this.x = x;
        this.y = y;
        this.z = z;
        this.walls = new Wall[3];
    }

    public override string ToString()
    {
        return "(" + x + "," + y + "," + z + ")";
    }

    public bool CanBuild() {
        return (placedObject == null);
    }

    public GridNode3D GetTileInDirection(Facing direction) {
        switch (direction) {
        case Facing.North:
            return grid.GetGridObject(this.x, this.y, this.z+1);
        case Facing.East:
            return grid.GetGridObject(this.x+1, this.y, this.z);
        case Facing.South:
            return grid.GetGridObject(this.x, this.y, this.z-1);
        case Facing.West:
            return grid.GetGridObject(this.x-1, this.y, this.z);
        case Facing.Up:
            return grid.GetGridObject(this.x, this.y+1, this.z);
        case Facing.Down:
            return grid.GetGridObject(this.x, this.y-1, this.z);
        default:
            return null;
        }
    }

}


public class Wall {
    public Wall() {
    }

}