using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacedObject : MonoBehaviour {
    public static PlacedObject Create(Vector3 worldPosition, Vector2Int origin, PlacedObjectTypeSO.Dir dir, PlacedObjectTypeSO placedObjectType) {
        Transform placedObjectTransform =
            Instantiate(
                placedObjectType.prefab,
                worldPosition,
                Quaternion.Euler(0, placedObjectType.GetRotationAngle(dir), 0)
            );

        PlacedObject placedObject = placedObjectTransform.GetComponent<PlacedObject>();
        placedObject.objectType = placedObjectType;
        placedObject.origin = origin;
        placedObject.dir = dir;

        return placedObject;
    }
    private PlacedObjectTypeSO objectType;
    private Vector2Int origin;
    private PlacedObjectTypeSO.Dir dir;

    public override string ToString() {
        return this.objectType.nameString;
    }

    public List<Vector2Int> GetGridPositionList() {
        return objectType.GetGridPositionList(origin, dir);
    }
    public void DestroySelf() {
        Destroy(gameObject);
    }
}
