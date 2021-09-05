using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class WallTypeSO : ScriptableObject
{
    public static Facing GetNextFacing(Facing facing) {
        switch (facing) {
        case Facing.North:
            return Facing.East;
        case Facing.East:
            return Facing.South;
        case Facing.South:
            return Facing.West;
        case Facing.West:
            return Facing.North;
        default:
        case Facing.Down:
        case Facing.Up:
            return facing;
        }
    }
    public string nameString;
    public Transform prefab;
    public Transform visual;
    public Facing facing;
    public bool occupied;

    public bool traversible;

    public Vector2Int GetRotationOffset(Facing facing) {
        switch (facing) {
        default:
        case Facing.Down:
        case Facing.Up:
        case Facing.North:
            return new Vector2Int(0, 0);
        case Facing.East:
            return new Vector2Int(0, 1);
        case Facing.South:
            return new Vector2Int(1, 1);
        case Facing.West:
            return new Vector2Int(1, 0);
        }
    }
    public int GetRotationAngle(Facing facing) {
        switch (facing) {
            default: return 0;
            case Facing.North: return 0;
            case Facing.East: return 90;
            case Facing.South: return 180;
            case Facing.West: return 270;
        }
    }

    public IPlaceable GetPlaceable() {
        return this.prefab.GetComponent<IPlaceable>();
    }
}
