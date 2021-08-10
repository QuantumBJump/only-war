using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum TileContent {
    FullTile, // Occupies the full tile
    Floor,  // is a floor
    EastWest, // Wall exists east-west, like: --
    NorthSouth, // Wall exists north-south, like: |
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

}


public class Wall {
    public Wall(int x, int y, int z) {
    }

}