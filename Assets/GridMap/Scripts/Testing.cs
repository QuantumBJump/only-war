using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour {

    [SerializeField] private HeatMapGenericVisual heatMapGenericVisual;

    private GridMap<HeatMapGridObject> grid;

    // Start is called before the first frame update
    private void Start() {
        grid = new GridMap<HeatMapGridObject>(20, 10, 5.0f, (GridMap<HeatMapGridObject> g, int x, int y) => new HeatMapGridObject(g, x, y));
        heatMapGenericVisual.SetGrid(grid);
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            Vector3 mousePos = Utils.GetMouseWorldPosition();
            HeatMapGridObject go = grid.GetGridObject(mousePos);
            if (go != null) {
                go.AddValue(5);
            }
        }

        if (Input.GetMouseButtonDown(1)) {
            Debug.Log(grid.GetGridObject(Utils.GetMouseWorldPosition()));
        }
    }
}

public class HeatMapGridObject {
    private const int MIN = 0;
    private const int MAX = 100;

    private GridMap<HeatMapGridObject> grid;
    private int x;
    private int y;
    private int value;

    public HeatMapGridObject(GridMap<HeatMapGridObject> grid, int x, int y) {
        this.x = x;
        this.y = y;
        this.grid = grid;
    }

    public void AddValue(int addValue) {
        Debug.Log("Adding " + addValue);
        value += addValue;
        Mathf.Clamp(value, MIN, MAX);
        grid.TriggerGridObjectChanged(x, y);
    }

    public float GetValueNormalised() {
        return (float)value / MAX;
    }

    public override string ToString() {
        return value.ToString();
    }
}
