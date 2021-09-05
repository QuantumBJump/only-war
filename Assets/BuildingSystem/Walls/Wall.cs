using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour, IPlaceable {
    private Vector3 position;
    private GridMap3D<GridNode3D> grid;
    private WallTypeSO wallType;
    private Facing facing;

    public void DestroySelf() {
        Destroy(gameObject);
    }
    public List<Vector3Int> GetGridPositionList() {
        List<Vector3Int> res = new List<Vector3Int>();
        res.Add(new Vector3Int((int)position.x, (int)position.y, (int)position.z));
        return res;
    }
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
        placedWall.facing = facing;

        return placedWall;
    }

    public Facing GetFacing() {
        return this.facing;
    }

    public WallTypeSO GetWallType() {
        return this.wallType;
    }
}
