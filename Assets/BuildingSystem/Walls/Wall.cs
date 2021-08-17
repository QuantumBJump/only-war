using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    private Vector3 position;
    private GridMap3D<GridNode3D> grid;

    private WallTypeSO wallType;

    public Wall(GridMap3D<GridNode3D> grid, Vector3 worldPosition) {
        this.grid = grid;
        this.position = worldPosition;
        Debug.Log("Creating wall at position [" + this.position.ToString() + "]");
    }

    public static Wall Create(Vector3 worldPosition, Facing facing, WallTypeSO wallType) {
        Debug.Log("Wall type: " + wallType.ToString());
        Transform wallTransform =
            Instantiate(
                wallType.prefab,
                worldPosition,
                Quaternion.Euler(0, wallType.GetRotationAngle(facing), 0)
            );

        Wall placedWall = wallTransform.GetComponent<Wall>();
        placedWall.wallType = wallType;

        return placedWall;
    }
}
