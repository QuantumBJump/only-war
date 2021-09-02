using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlaceable
{
    void DestroySelf();

    List<Vector3Int> GetGridPositionList();
}
