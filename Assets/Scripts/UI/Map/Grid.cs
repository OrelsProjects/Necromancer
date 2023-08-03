using UnityEngine;

public class Grid
{
    private int width;
    private int height;
    private float cellSize;
    private int[,] gridArray;

    private int[,] _populatedAreas = new int[14, 8];

    private int[] _previousColoredCell;

    public Grid(int width, int height, float cellSize)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;

        this._populatedAreas[2, 3] = 1;
        this._populatedAreas[2, 4] = 1;
        this._populatedAreas[2, 5] = 1;

        gridArray = new int[width, height];

        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                gridArray[x, y] = 0;
                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);
                if (_populatedAreas[x, y] == 1)
                {
                    ResetColorToCell(x, y);

                }
            }
        }
        Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
        Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);
    }

    private Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x, y) * cellSize;
    }

    private int[] GetCellNumberByPosition(Vector3 position)
    {
        int x = Mathf.FloorToInt(position.x / cellSize);
        int y = Mathf.FloorToInt(position.y / cellSize);
        return new int[] { x, y };
    }

    private void ResetColorToCell(int x, int y)
    {
        if (_populatedAreas[x, y] == 1)
        {
            SetColorToCell(x, y, Color.blue);
        }
        else
        {
            SetColorToCell(x, y, Color.white);
        }
    }

    private void SetColorToCell(int x, int y, Color color)
    {
        Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), color, 100f);
        Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), color, 100f);
        Debug.DrawLine(GetWorldPosition(x + 1, y), GetWorldPosition(x + 1, y + 1), color, 100f);
        Debug.DrawLine(GetWorldPosition(x, y + 1), GetWorldPosition(x + 1, y + 1), color, 100f);
    }

    public void ColorCell(Vector3 position, Color color)
    {
        if (_previousColoredCell != null)
        {
            ResetColorToCell(_previousColoredCell[0], _previousColoredCell[1]);
        }
        int[] cellNumbers = GetCellNumberByPosition(position);
        int x = cellNumbers[0];
        int y = cellNumbers[1];
        // if x, y are not in the grid, return
        if (x < 0 || x >= width || y < 0 || y >= height)
        {
            return;
        }
        SetColorToCell(x, y, color);
        _previousColoredCell = cellNumbers;
    }

}