using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class WallTypeSO : ScriptableObject
{
    public string nameString;
    public Transform prefab;
    public Transform visual;
    public Facing facing;

    public int GetRotationAngle(Facing facing) {
        switch (facing) {
            default: return 0;
            case Facing.North: return 0;
            case Facing.East: return 90;
            case Facing.South: return 180;
            case Facing.West: return 270;
        }
    }
}
