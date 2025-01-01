using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.SearchService;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class Grid {

    #region variables for grid initialisation
    private int width;
    private int height;
    private int cellSize;
    private Vector3 originPosition;
    private GameObject parent;
    private GridCell cellPrefab;
    private CellOccupiedStateSO initialCellStateSO;
    #endregion

    private GridCell[,] cellArray;
    private GridObject[,] gridObjectArray;

    public Grid(int width, int height, int cellSize, Vector3 originPosition, GameObject parent, GridCell cellPrefab, CellOccupiedStateSO initialCellStateSO) {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;
        this.parent = parent;
        this.cellPrefab = cellPrefab;
        this.initialCellStateSO = initialCellStateSO;

        cellArray = new GridCell[width, height];
        gridObjectArray = new GridObject[width, height];

        for (int x = 0; x < cellArray.GetLength(0); x++) {
            for (int y = 0; y < cellArray.GetLength(1); y++) {
                cellArray[x, y] = GameObject.Instantiate(cellPrefab, parent.transform);
                cellArray[x, y].SetOccupiedState(initialCellStateSO);
                cellArray[x, y].name = $"Cell {x} {y}";
                cellArray[x, y].transform.position = new Vector3((x + .5f) * cellSize, (y + .5f) * cellSize) + originPosition;
                gridObjectArray[x, y] = null;
                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);
            }
        }
        Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
        Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);
    }

    public Vector3 GetWorldPosition(int x, int y) {
        return new Vector3(x + 0.5f, y + 0.5f) * cellSize + originPosition;
    }

    public void GetXY(Vector3 worldPosition, out int x, out int y) {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        y = Mathf.FloorToInt((worldPosition - originPosition).y / cellSize);
    }

    public bool IsInGrid(int x, int y) {
        return (x >= 0 && y >= 0 && x < width && y < height);
    }

    public bool IsInGrid(Vector3 worldPosition) {
        int x, y;
        GetXY(worldPosition, out x, out y);
        return IsInGrid(x, y);
    }

    #region GridCell related methods
    public GridCell GetCell(int x, int y) {
        return IsInGrid(x, y) ? cellArray[x, y] : null;
    }

    public GridCell GetCell(Vector3 worldPosition) {
        int x, y;
        GetXY(worldPosition, out x, out y);
        return GetCell(x, y);
    }

    public string GetCellDebugText(Vector3 worldPosition) {
        int x, y;
        GetXY(worldPosition, out x, out y);
        return GetCellDebugString(x, y);
    }
    #endregion

    #region GridObject related methods
    public GridObject GetGridObject(int x, int y) {
        return IsInGrid(x, y) ? gridObjectArray[x, y] : null;
    }

    public GridObject GetGridObject(Vector3 worldPosition) {
        int x, y;
        GetXY(worldPosition, out x, out y);
        return GetGridObject(x, y);
    }

    public void SetGridObject(int x, int y, GridObject gridObject) {
        if (IsInGrid(x, y)) {
            gridObjectArray[x, y] = gridObject;
            if (gridObject != null) {
                gridObjectArray[x, y].transform.SetParent(parent.transform);
                gridObjectArray[x, y].transform.position = new Vector3((x + .5f) * cellSize, (y + .5f) * cellSize) + originPosition;
            }
        }
    }

    public void SetGridObject(Vector3 worldPosition, GridObject gridObject) {
        int x, y;
        GetXY(worldPosition, out x, out y);
        SetGridObject(x, y, gridObject);
    }

    public void RemoveGridObjectAt(int x, int y) {
        if (IsInGrid(x, y)) {
            if (gridObjectArray[x, y] != null) {
                gridObjectArray[x, y] = null;
            }
        }
    }

    public void RemoveGridObjectAt(Vector3 worldPosition) {
        int x, y;
        GetXY(worldPosition, out x, out y);
        RemoveGridObjectAt(x, y);
    }
    public bool FindGridObject(GridObject gridObject, out int xFound, out int yFound) {
        for (int x = 0; x < gridObjectArray.GetLength(0); x++) {
            for (int y = 0; y < gridObjectArray.GetLength(1); y++) {
                if (gridObjectArray[x, y] == gridObject) {
                    xFound = x;
                    yFound = y;
                    return true;
                }
            }
        }
        xFound = -1;
        yFound = -1;
        return false;
    }
    #endregion

    public GameObject GetParentObject() {
        return parent;
    }

    public void DistroyAll() {
        for (int x = 0; x < gridObjectArray.GetLength(0); x++) {
            for (int y = 0; y < gridObjectArray.GetLength(1); y++) {
                cellArray[x, y]?.DestroySelf();
                gridObjectArray[x, y]?.DestroySelf();
            }
        }
    }

    public string GetCellDebugString(int x, int y) {
        if (IsInGrid(x, y)) {
            if (gridObjectArray[x, y] != null) {
                return cellArray[x, y].GetDebugString() + "\n" + gridObjectArray[x, y].GetDebugString();
            }
            return cellArray[x, y].GetDebugString();
        } else {
            return "Not in grid";
        }
    }

    public int CountCellWithStateSO(CellOccupiedStateSO cellStateSO) {
        int count = 0;
        for (int x = 0; x < cellArray.GetLength(0); x++) {
            for (int y = 0; y < cellArray.GetLength(1); y++) {
                if (cellArray[x, y].GetOccupiedState() == cellStateSO) {
                    count++;
                }
            }
        }
        return count;
    }

    public List<(int, int)> GetCellPositionListInRadius(int centerX, int centerY, int radius) {
        // Calculate positions of cells in the given radius
        List<(int, int)> cellPositionList = new List<(int, int)>();
        for (int xOffset = -radius; xOffset <= radius; xOffset++) {
            for (int yOffset = -radius; yOffset <= radius; yOffset++) {
                if (IsInGrid(centerX + xOffset, centerY + yOffset)) {
                    cellPositionList.Add((centerX + xOffset, centerY + yOffset));
                }
            }
        }
        Debug.Log(cellPositionList.Count);

        return cellPositionList;
    }
    public List<(int, int)> GetCellPositionListInRadius(Vector3 worldPosition, int radius) {
        int centerX, centerY;
        GetXY(worldPosition, out centerX, out centerY);
        return GetCellPositionListInRadius(centerX, centerY, radius);
    }

    public List<GridObject> GetGridObjectListNearyBy(int centerX, int centerY, int radius) {
        List<GridObject> objectList = new List<GridObject>();
        List<(int, int)> cellPositionList = GetCellPositionListInRadius(centerX, centerY, radius);
        for (int i = 0; i < cellPositionList.Count; i++) {
            GridObject gridObject = GetGridObject(cellPositionList[i].Item1, cellPositionList[i].Item2);
            if (gridObject != null) {
                objectList.Add(gridObject);
            }
        }
        return objectList;
    }

    public List<GridObject> GetGridObjectListNearyBy(Vector3 worldPosition, int radius) {
        int centerX, centerY;
        GetXY(worldPosition, out centerX, out centerY);
        return GetGridObjectListNearyBy(centerX, centerY, radius);
    }


    public void HightlightPossibleMove(Piece piece, CellOccupiedStateSO emptySO) {
        List<(int, int)> moveList = GetAllPossibleMove(piece, emptySO);
        foreach ((int, int) point in moveList) {
            cellArray[point.Item1, point.Item2].ChangeHighlight(true);
        }
    }

    public void AllHighlightOff() {
        for (int x = 0; x < cellArray.GetLength(0); x++) {
            for (int y = 0; y < cellArray.GetLength(1); y++) {
                cellArray[x, y].ChangeHighlight(false);
            }
        }
    }

    private List<(int, int)> GetAllPossibleMove(Piece piece, CellOccupiedStateSO emptySO) {
        List<(int, int)> moveList = new List<(int, int)>();

        FindGridObject(piece, out int xFound, out int yFound);

        // add basic moves
        for (int x = 0; x < cellArray.GetLength(0); x++) {
            for (int y = 0; y < cellArray.GetLength(1); y++) {
                if (piece.IsMoveValid(xFound, yFound, x, y)) {
                    if (cellArray[x, y].GetOccupiedState().stateName == piece.GetCellStateSO().stateName
                        || cellArray[x, y].GetOccupiedState().stateName == emptySO.stateName) {
                        moveList.Add((x, y));
                    }
                }
            }
        }

        // add climbing move
        for (int y = yFound - 1; y >= 0; y--) {
            if (gridObjectArray[xFound, y] != null) {
                if ((gridObjectArray[xFound, y].GetComponent<Piece>() != null)
                && (gridObjectArray[xFound, y].GetComponent<Piece>().GetCellStateSO().stateName == piece.GetCellStateSO().stateName)) {
                    moveList.Add((xFound, y + 1));
                }
                break;
            }
        }

        for (int y = yFound + 1; y < height; y++) {
            if ((gridObjectArray[xFound, y] != null)) {
                if ((gridObjectArray[xFound, y].GetComponent<Piece>() != null)
                && (gridObjectArray[xFound, y].GetComponent<Piece>().GetCellStateSO().stateName == piece.GetCellStateSO().stateName)) {
                    moveList.Add((xFound, y - 1));
                }
                break;
            }
        }

        for (int x = xFound - 1; x >= 0; x--) {
            if ((gridObjectArray[x, yFound] != null)) {
                if ((gridObjectArray[x, yFound].GetComponent<Piece>() != null)
                && (gridObjectArray[x, yFound].GetComponent<Piece>().GetCellStateSO().stateName == piece.GetCellStateSO().stateName)) {
                    moveList.Add((x + 1, yFound));
                }
                break;
            }
        }

        for (int x = xFound + 1; x < width; x++) {
            if ((gridObjectArray[x, yFound] != null)) {
                if ((gridObjectArray[x, yFound].GetComponent<Piece>() != null)
                && (gridObjectArray[x, yFound].GetComponent<Piece>().GetCellStateSO().stateName == piece.GetCellStateSO().stateName)) {
                    moveList.Add((x - 1, yFound));
                }
                break;
            }
        }

        return moveList;
    }
}



