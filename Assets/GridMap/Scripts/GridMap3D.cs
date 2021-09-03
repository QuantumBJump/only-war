using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class GridMap3D<TGridObject> {

    public event EventHandler<OnGridObjectChangedEventArgs> OnGridObjectChanged;
    public class OnGridObjectChangedEventArgs: EventArgs {
        public int x;
        public int y;
        public int z;
    }

    private int width;
    private int depth;
    private int height;
    private Vector3 originPosition;
    private float cellSize;
    private TGridObject[,,] gridArray;

    // Constructor
    public GridMap3D(int width, int height, int depth, float cellSize, Func<GridMap3D<TGridObject>, int, int, int, TGridObject> createGridObject, Vector3? originPosition = null, bool showDebug = false) {
        if (originPosition == null) {
            this.originPosition = Vector3.zero;
        } else {
            this.originPosition = (Vector3)originPosition;
        }
        this.width = width;
        this.height = height;
        this.depth = depth;
        this.cellSize = cellSize;

        gridArray = new TGridObject[width, height, depth];

        for (int x = 0; x < gridArray.GetLength(0); x++) {
            for (int y = 0; y < gridArray.GetLength(1); y++) {
                for (int z = 0; z < gridArray.GetLength(2); z++) {
                    TGridObject newObject = createGridObject(this, x, y, z);
                    gridArray[x, y, z] = newObject;
                }
            }
        }

        if (showDebug) {
            TextMeshPro[,,] debugTextArray = new TextMeshPro[width, height, depth];
            for (int x=0; x < gridArray.GetLength(0); x++)
            {
                for (int y = 0; y < gridArray.GetLength(1); y++)
                {
                    for (int z = 0; z < gridArray.GetLength(2); z++) {
                        debugTextArray[x, y, z] = Utils.CreateWorldText(gridArray[x, y, z]?.ToString(), null, GetWorldPosition(x, y, z)+ new Vector3(cellSize/2f, cellSize/2f, cellSize/2f), 12, Color.black, TMPro.TextContainerAnchors.Middle);
                        DrawCube(x, y, z);
                    }
                }
            }

            OnGridObjectChanged += (object sender, OnGridObjectChangedEventArgs eventArgs) => {
                debugTextArray[eventArgs.x, eventArgs.y, eventArgs.z].text = gridArray[eventArgs.x, eventArgs.y, eventArgs.z]?.ToString();
            };
        }
    }

    public Vector3 GetWorldPosition(int x, int y, int z) {
        return new Vector3(x, y, z) * cellSize + originPosition;
    }

    public Vector3 GetCenter(int x, int y, int z) {
        return GetWorldPosition(x, y, z) + (new Vector3(0.5f, 0.5f, 0.5f) * cellSize);
    }

    public void GetXYZ(Vector3 worldPosition, out int x, out int y, out int z)
    {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        y = Mathf.FloorToInt((worldPosition - originPosition).y / cellSize);
        z = Mathf.FloorToInt((worldPosition - originPosition).z / cellSize);
    }

    private void DrawCube(int x, int y, int z)
    {
        Vector3 tenthZ = new Vector3(0, 0, 1) * cellSize / 10f;
        Vector3 tenthX = new Vector3(1, 0, 0) * cellSize / 10f;
        Vector3 tenthY = new Vector3(0, 1, 0) * cellSize / 10f;
        // Bottom front left
        Debug.DrawLine(GetWorldPosition(x, y, z), GetWorldPosition(x, y, z) + tenthZ, Color.black, 100f);
        Debug.DrawLine(GetWorldPosition(x, y, z), GetWorldPosition(x, y, z) + tenthX, Color.black, 100f);
        Debug.DrawLine(GetWorldPosition(x, y, z), GetWorldPosition(x, y, z) + tenthY, Color.black, 100f);

        // Bottom front right
        Debug.DrawLine(GetWorldPosition(x + 1, y, z), GetWorldPosition(x + 1, y, z) - tenthX, Color.black, 100f);
        Debug.DrawLine(GetWorldPosition(x + 1, y, z), GetWorldPosition(x + 1, y, z) + tenthZ, Color.black, 100f);
        Debug.DrawLine(GetWorldPosition(x + 1, y, z), GetWorldPosition(x + 1, y, z) + tenthY, Color.black, 100f);

        // Bottom back left
        Debug.DrawLine(GetWorldPosition(x, y, z+1), GetWorldPosition(x, y, z+1) + tenthX, Color.black, 100f);
        Debug.DrawLine(GetWorldPosition(x, y, z+1), GetWorldPosition(x, y, z+1) - tenthZ, Color.black, 100f);
        Debug.DrawLine(GetWorldPosition(x, y, z+1), GetWorldPosition(x, y, z+1) + tenthY, Color.black, 100f);

        // Bottom back right
        Debug.DrawLine(GetWorldPosition(x + 1, y, z+1), GetWorldPosition(x + 1, y, z+1) - tenthX, Color.black, 100f);
        Debug.DrawLine(GetWorldPosition(x + 1, y, z+1), GetWorldPosition(x + 1, y, z+1) - tenthZ, Color.black, 100f);
        Debug.DrawLine(GetWorldPosition(x + 1, y, z+1), GetWorldPosition(x + 1, y, z+1) + tenthY, Color.black, 100f);

        // Top front left
        Debug.DrawLine(GetWorldPosition(x, y+1, z), GetWorldPosition(x, y+1, z) + tenthZ, Color.black, 100f);
        Debug.DrawLine(GetWorldPosition(x, y+1, z), GetWorldPosition(x, y+1, z) + tenthX, Color.black, 100f);
        Debug.DrawLine(GetWorldPosition(x, y+1, z), GetWorldPosition(x, y+1, z) - tenthY, Color.black, 100f);

        // Top front right
        Debug.DrawLine(GetWorldPosition(x + 1, y+1, z), GetWorldPosition(x + 1, y+1, z) - tenthX, Color.black, 100f);
        Debug.DrawLine(GetWorldPosition(x + 1, y+1, z), GetWorldPosition(x + 1, y+1, z) + tenthZ, Color.black, 100f);
        Debug.DrawLine(GetWorldPosition(x + 1, y+1, z), GetWorldPosition(x + 1, y+1, z) - tenthY, Color.black, 100f);

        // Top back left
        Debug.DrawLine(GetWorldPosition(x, y+1, z+1), GetWorldPosition(x, y+1, z+1) + tenthX, Color.black, 100f);
        Debug.DrawLine(GetWorldPosition(x, y+1, z+1), GetWorldPosition(x, y+1, z+1) - tenthZ, Color.black, 100f);
        Debug.DrawLine(GetWorldPosition(x, y+1, z+1), GetWorldPosition(x, y+1, z+1) - tenthY, Color.black, 100f);

        // Top back right
        Debug.DrawLine(GetWorldPosition(x + 1, y+1, z+1), GetWorldPosition(x + 1, y+1, z+1) - tenthX, Color.black, 100f);
        Debug.DrawLine(GetWorldPosition(x + 1, y+1, z+1), GetWorldPosition(x + 1, y+1, z+1) - tenthZ, Color.black, 100f);
        Debug.DrawLine(GetWorldPosition(x + 1, y+1, z+1), GetWorldPosition(x + 1, y+1, z+1) - tenthY, Color.black, 100f);

    }


    public int GetWidth() {
        return width;
    }

    public int GetHeight() {
        return height;
    }

    public int GetDepth() {
        return depth;
    }

    public float GetCellSize() {
        return cellSize;
    }

    public void SetGridObject(int x, int y, int z, TGridObject value)
    {
        if (InBounds(x, y, z)) {
            gridArray[x, y, z] = value;
            if (OnGridObjectChanged != null) OnGridObjectChanged(this, new OnGridObjectChangedEventArgs { x = x, y = y, z = z });
        }
    }

    public void SetGridObject(Vector3 worldPosition, TGridObject value) {
        int x, y, z;
        GetXYZ(worldPosition, out x, out y, out z);
        SetGridObject(x, y, z, value);
    }

    public void TriggerGridObjectChanged(int x, int y ) {
        if (OnGridObjectChanged != null) OnGridObjectChanged(this, new OnGridObjectChangedEventArgs { x = x, y = y });
    }

    public TGridObject GetGridObject(int x, int y, int z) {
        if (InBounds(x, y, z)) {
            return gridArray[x, y, z];
        } else {
            return default;
        }
    }

    public TGridObject GetGridObject(Vector3 worldPosition) {
        GetXYZ(worldPosition, out int x, out int y, out int z);
        return GetGridObject(x, y, z);
    }

    public bool InBounds(int x, int y, int z) {
        return (x >= 0 && x < width && y >= 0 && y < height && z >= 0 && z < depth);
    }
}
