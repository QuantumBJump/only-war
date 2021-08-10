using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitPathfinder : MonoBehaviour
{
    private Pathfinding pathfinder;
    private List<Vector3> pathVectorList;
    private int pathIndex = -1;


    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(pathfinder);
    }

    // Update is called once per frame
    void Update()
    {
        if (pathIndex != -1) {
            Vector3 nextPathPosition = pathVectorList[pathIndex];
            Vector3 moveVelocity = (nextPathPosition - transform.position).normalized;
            GetComponent<MoveUnit>().SetVelocity(moveVelocity);

            float reachedPathPositionDistance = 1f;
            if (Vector3.Distance(transform.position, nextPathPosition) < reachedPathPositionDistance) {
                pathIndex++;
                if (pathIndex >= pathVectorList.Count) {
                    // End of path
                    pathIndex = -1;
                }
            }
        } else {
            // Idle
            GetComponent<MoveUnit>().SetVelocity(Vector3.zero);
        }
    }

    public void SetMovePosition(Vector3 movePosition) {
        if (this.pathfinder == null) {
            this.pathfinder = Pathfinding.Instance;
        }
        Debug.Log(movePosition);
        Debug.Log(this.transform.position);
        pathfinder.GetGrid().GetXZ(this.transform.position, out int startX, out int startZ);
        pathfinder.GetGrid().GetXZ(movePosition, out int endX, out int endZ);
        pathVectorList = pathfinder.FindPath_Vectors(startX, startZ, endX, endZ);
        if (pathVectorList.Count > 0) {
            pathIndex = 0;
        }
    }
}
