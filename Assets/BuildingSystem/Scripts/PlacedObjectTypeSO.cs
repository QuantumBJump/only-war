using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class PlacedObjectTypeSO : ScriptableObject
{
    public static Dir GetNextDir(Dir dir) {
        switch (dir) {
            default:
            case Dir.Down: return Dir.Left;
            case Dir.Left: return Dir.Up;
            case Dir.Up: return Dir.Right;
            case Dir.Right: return Dir.Down;
        }
    }

    public enum Dir {
        Down,
        Left,
        Up,
        Right,
    }

    public string nameString;
    public Transform prefab;
    public Transform visual;
    public int width;
    public int height;
    public int depth;

    public Vector2Int GetRotationOffset(Dir dir) {
        switch (dir) {
            default:
            case Dir.Down: return new Vector2Int(0, 0);
            case Dir.Left: return new Vector2Int(0, width);
            case Dir.Up: return new Vector2Int(width, depth);
            case Dir.Right: return new Vector2Int(depth, 0);
        }
    }

    public int GetRotationAngle(Dir dir) {
        switch (dir) {
            default:
            case Dir.Down: return 0;
            case Dir.Left: return 90;
            case Dir.Up: return 180;
            case Dir.Right: return 270;
        }
    }

    public List<Vector3Int> GetGridPositionList(Vector3Int offset, Dir dir) {
        List<Vector3Int> gridPositionList = new List<Vector3Int>();
        switch (dir) {
            default:
            case Dir.Down:
            case Dir.Up:
                for (int x = 0; x < width; x++) {
                    for (int y = 0; y < height; y++) {
                        for (int z = 0; z < depth; z++) {
                            gridPositionList.Add(offset + new Vector3Int(x, y, z));
                            
                        }
                    }
                }
                break;
            case Dir.Left:
            case Dir.Right:
                for (int x = 0; x < depth; x++) {
                    for (int y = 0; y < height; y++) {
                        for (int z = 0; z < width; z++) {
                            gridPositionList.Add(offset + new Vector3Int(x, y, z));
                        }
                    }
                }
                break;
        }
        return gridPositionList;
    }
}
