using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class GridMap<TGridObject> {

    public event EventHandler<OnGridObjectChangedEventArgs> OnGridObjectChanged;
    public class OnGridObjectChangedEventArgs: EventArgs {
        public int x;
        public int y;
    }

    private int width;
    private int depth;
    private Vector3 originPosition;
    private float cellSize;
    private TGridObject[,] gridArray;

    // Constructor
    public GridMap(int width, int depth, float cellSize, Func<GridMap<TGridObject>, int, int, TGridObject> createGridObject, Vector3? originPosition = null) {
        if (originPosition == null) {
            this.originPosition = Vector3.zero;
        } else {
            this.originPosition = (Vector3)originPosition;
        }
        this.width = width;
        this.depth = depth;
        this.cellSize = cellSize;

        gridArray = new TGridObject[width, depth];

        for (int x = 0; x < gridArray.GetLength(0); x++) {
            for (int y = 0; y < gridArray.GetLength(1); y++) {
                gridArray[x, y] = createGridObject(this, x, y);
            }
        }

        bool showDebug = true;
        if (showDebug) {
            TextMeshPro[,] debugTextArray = new TextMeshPro[width, depth];
            for (int x=0; x < gridArray.GetLength(0); x++)
            {
                for (int y = 0; y < gridArray.GetLength(1); y++)
                {
                    debugTextArray[x, y] = Utils.CreateWorldText(gridArray[x, y]?.ToString(), null, GetWorldPosition(x, y)+ new Vector3(cellSize/2f, 0.05f, cellSize/2f), 12, Color.black, TMPro.TextContainerAnchors.Middle);
                    DrawSquare(x, y);
                }
            }

            OnGridObjectChanged += (object sender, OnGridObjectChangedEventArgs eventArgs) => {
                debugTextArray[eventArgs.x, eventArgs.y].text = gridArray[eventArgs.x, eventArgs.y]?.ToString();
            };
        }
    }

    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x, 0, y) * cellSize + originPosition;
    }

    public void GetXZ(Vector3 worldPosition, out int x, out int z)
    {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        z = Mathf.FloorToInt((worldPosition - originPosition).z / cellSize);
    }

    private void DrawSquare(int x, int y)
    {
        Vector3 tenthVertical = new Vector3(0, 0, 1) * cellSize / 10f;
        Vector3 tenthHorizontal = new Vector3(1, 0, 0) * cellSize / 10f;
        // Draw bottom left corner
        Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y) + tenthVertical, Color.black, 100f);
        Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y) + tenthHorizontal, Color.black, 100f);

        // Bottom right
        Debug.DrawLine(GetWorldPosition(x + 1, y), GetWorldPosition(x + 1, y) - tenthHorizontal, Color.black, 100f);
        Debug.DrawLine(GetWorldPosition(x + 1, y), GetWorldPosition(x + 1, y) + tenthVertical, Color.black, 100f);

        Debug.DrawLine(GetWorldPosition(x, y+1), GetWorldPosition(x, y+1) + tenthHorizontal, Color.black, 100f);
        Debug.DrawLine(GetWorldPosition(x, y+1), GetWorldPosition(x, y+1) - tenthVertical, Color.black, 100f);

        Debug.DrawLine(GetWorldPosition(x + 1, y+1), GetWorldPosition(x + 1, y+1) - tenthHorizontal, Color.black, 100f);
        Debug.DrawLine(GetWorldPosition(x + 1, y+1), GetWorldPosition(x + 1, y+1) - tenthVertical, Color.black, 100f);

    }

    public int GetWidth() {
        return width;
    }

    public int GetHeight() {
        return depth;
    }

    public float GetCellSize() {
        return cellSize;
    }

    public void SetGridObject(int x, int y, TGridObject value)
    {
        if (x >= 0 && y >= 0 && x < width && y < depth)
        {
            gridArray[x, y] = value;
            if (OnGridObjectChanged != null) OnGridObjectChanged(this, new OnGridObjectChangedEventArgs { x = x, y = y });
        }
    }

    public void SetGridObject(Vector3 worldPosition, TGridObject value) {
        int x, z;
        GetXZ(worldPosition, out x, out z);
        SetGridObject(x, z, value);
    }

    public void TriggerGridObjectChanged(int x, int y ) {
        if (OnGridObjectChanged != null) OnGridObjectChanged(this, new OnGridObjectChangedEventArgs { x = x, y = y });
    }

    public TGridObject GetGridObject(int x, int y) {
        if (x >= 0 && x < width && y >= 0 && y < depth) {
            return gridArray[x, y];
        } else {
            return default;
        }
    }

    public TGridObject GetGridObject(Vector3 worldPosition) {
        int x, z;
        GetXZ(worldPosition, out x, out z);
        return GetGridObject(x, z);
    }
}
