using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GridMap<TGridObject> {
    private int width;
    private int depth;
    private Vector3 originPosition;
    private float cellSize;
    private TGridObject[,] gridArray;
    private TextMeshPro[,] debugTextArray;
    public GridMap(int width, int depth, float cellSize, Vector3? originPosition = null)
    {
        if (originPosition == null) {
            this.originPosition = Vector3.zero;
        } else {
            this.originPosition = (Vector3)originPosition;
        }
        this.width = width;
        this.depth = depth;
        this.cellSize = cellSize;

        gridArray = new TGridObject[width, depth];
        debugTextArray = new TextMeshPro[width, depth];


        for (int x=0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                debugTextArray[x, y] = Utils.CreateWorldText(gridArray[x, y].ToString(), null, GetWorldPosition(x, y)+ new Vector3(cellSize/2f, 0.05f, cellSize/2f), 12, Color.black, TMPro.TextContainerAnchors.Middle);
                DrawSquare(x, y);
            }
        }
    }

    private Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x, 0, y) * cellSize + originPosition;
    }

    private void GetXY(Vector3 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        y = Mathf.FloorToInt((worldPosition - originPosition).z / cellSize);
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

    public void SetValue(int x, int y, TGridObject value)
    {
        if (x >= 0 && y >= 0 && x < width && y < depth)
        {
            gridArray[x, y] = value;
            debugTextArray[x, y].text = value.ToString();
        }
    }

    public void SetValue(Vector3 worldPosition, TGridObject value) {
        int x, y;
        GetXY(worldPosition, out x, out y);
        if (x >= 0 && x < width && y >= 0 && y < depth) {
            SetValue(x, y, value);
        }
    }

    public TGridObject GetValue(int x, int y) {
        if (x >= 0 && x < width && y >= 0 && y < depth) {
            return gridArray[x, y];
        } else {
            return default;
        }
    }

    public TGridObject GetValue(Vector3 worldPosition) {
        int x, y;
        GetXY(worldPosition, out x, out y);
        return GetValue(x, y);
    }
}
