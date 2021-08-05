using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour {

    private GridMap<bool> grid;

    // Start is called before the first frame update
    private void Start() {
        grid = new GridMap<bool>(10, 10, 5.0f);
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            Vector3 mousePos = Utils.GetMouseWorldPosition();
            grid.SetValue(mousePos, !grid.GetValue(mousePos));
        }

        if (Input.GetMouseButtonDown(1)) {
            Debug.Log(grid.GetValue(Utils.GetMouseWorldPosition()));
        }
    }
}
