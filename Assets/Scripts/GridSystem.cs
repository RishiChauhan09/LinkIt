using System;
using UnityEngine;

public class GridSystem<TGridObject> {

    private int width;
    private int height;
    private float cellSize;
    private TGridObject[,] gridArray;
    private Vector3 originPosition;

    public GridSystem(int width, int height, float cellSize, Vector3 originPosition) {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        gridArray = new TGridObject[width, height];
        this.originPosition = originPosition;
    }


    public Vector3 GetWorldPosition(int x, int y) {
        return new Vector3(x, y) * cellSize + originPosition + cellSize * .5f * Vector3.one;
    }

    public bool TryGetXY(Vector3 worldPos, out int x, out int y) {
        x = Mathf.FloorToInt((worldPos - originPosition).x / cellSize);
        y = Mathf.FloorToInt((worldPos - originPosition).y / cellSize);

        if(x < width && y < height && x >= 0 && y >= 0) return true;
        else return false;
    }

    public bool TryGetGridObject(int x, int y, out TGridObject gridObject) {
        if(x >= width || y >= height || x < 0 || y < 0) {
            gridObject = default(TGridObject);
            return false;
        } else {
            gridObject = gridArray[x, y];
            return true;
        }
    }

    public bool TryGetGridObject(Vector3 worldPos, out TGridObject gridObject) {
        int x, y;
        TryGetXY(worldPos, out x, out y);
        return TryGetGridObject(x, y, out gridObject);
    }

    public bool SetGridObject(int x, int y, TGridObject value) {
        if(x >= 0 && y >= 0 && x < width && y < height) {
            gridArray[x, y] = value;
            return true;
        }
        return false;
    }

    public void ChangePosition(Vector2Int from, Vector2Int to) {
        gridArray[to.x , to.y] = gridArray[from.x , from.y];
        gridArray[from.x , from.y] = default(TGridObject);
    }


    public int GetWidth() {
        return width;
    }

    public int GetHeight() {
        return height;
    }

    public float GetCellSize() {
        return cellSize;
    }

    public Vector3 GetOriginPosition() {
        return originPosition;
    }

}