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
        this.walls = new Wall[6];
        PopulateWalls();
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

    // Gets the world position of the wall on a given facing
    public Vector3 WallPosition(Facing facing) {
        Vector3 basePosition = grid.GetCenter(this.x, this.y, this.z);
        Vector3 offset;
        switch (facing) {
        case Facing.North:
            offset = new Vector3(0, 0, 0.5f) * grid.GetCellSize();
            break;
        case Facing.East:
            offset = new Vector3(0.5f, 0, 0) * grid.GetCellSize();
            break;
        case Facing.South:
            offset = new Vector3(0, 0, -0.5f) * grid.GetCellSize();
            break;
        case Facing.West:
            offset = new Vector3(-0.5f, 0, 0) * grid.GetCellSize();
            break;
        case Facing.Up:
            offset = new Vector3(0, 0.5f, 0) * grid.GetCellSize();
            break;
        case Facing.Down:
            offset = new Vector3(0, -0.5f, 0) * grid.GetCellSize();
            break;
        default:
            Debug.Log("Aah! facing test failed!");
            offset = Vector3.zero;
            break;
        }

        return basePosition + offset;

    }

    private Facing oppositeFacing(Facing initial) {
        switch (initial) {
        case Facing.North:
            return Facing.South;
        case Facing.East:
            return Facing.West;
        case Facing.South:
            return Facing.North;
        case Facing.West:
            return Facing.East;
        case Facing.Up:
            return Facing.Down;
        case Facing.Down:
            return Facing.Up;
        default:
            return Facing.Down;
        }
    }


    public void PopulateWalls() {
        // Check each facing
        // For each facing which ISN'T already populated,
        for (int i = 0; i < 6; i++) {
            WallTypeSO toPlace;
            toPlace = GameState.Instance.wallTypes[2];
            // switch ((Facing)i) {
            //     default:
            //     case Facing.North:
            //     case Facing.East:
            //     case Facing.South:
            //     case Facing.West:
            //         toPlace = GameState.Instance.wallTypes[0];
            //         break;
            //     case Facing.Up:
            //     case Facing.Down:
            //         toPlace = GameState.Instance.wallTypes[1];
            //         break;
            // }
            // Go through each of this tile's facings
            if (this.walls[i] != null) {
                // If the facing is already populated, we can skip it
                continue;
            }
            GridNode3D neighbour = GetTileInDirection((Facing)i);
            if (neighbour != null) {
                // If we've got a neighbour in that direction, see if they've already got a wall
                Wall otherWall = neighbour.walls[(int)oppositeFacing((Facing)i)];
                if (otherWall != null) {
                    // If they have the wall we want, just copy it
                    this.walls[i] = otherWall;
                    continue;
                } else {
                    // If they don't already have a wall, set our wall and their wall to a new object
                    Wall newWall = Wall.Create(WallPosition((Facing)i), (Facing)i, toPlace);
                    this.walls[i] = newWall;
                    neighbour.walls[(int)oppositeFacing((Facing)i)] = newWall;
                    continue;
                }
            } else {
                // if there's no neighbour, we'll need to do it ourselves.
                Wall newWall = Wall.Create(WallPosition((Facing)i), (Facing)i, toPlace);
                this.walls[i] = newWall;
            }
        }
    }

}
