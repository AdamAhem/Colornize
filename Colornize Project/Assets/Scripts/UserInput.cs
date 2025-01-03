using System;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using TMPro;

public class UserInput : MonoBehaviour {
    // TODO: split this script into proper managers and handlers !

    public event EventHandler<ScoreChangeEventArgs> OnScoreChange;

    public class ScoreChangeEventArgs : EventArgs {
        public Dictionary<CellOccupiedStateSO, int> scoreDict;
        public ScoreChangeEventArgs(Dictionary<CellOccupiedStateSO, int> scoreDict) {
            this.scoreDict = scoreDict;
        }
    }

    [SerializeField] private GridCell cellPrefab;
    [SerializeField] private CellOccupiedStateSO initialCellStateSO;
    [SerializeField] private Camera cam;

    [SerializeField] private CellOccupiedStateSO emptySO;

    [SerializeField] private List<CellOccupiedStateSO> cellStatesSOForScoreCountingList;

    [SerializeField] private TMP_Text gridInUseText;

    [SerializeField] private bool canOverride = true;

    #region variables for multiple grid implementation
    private int numberOfGrid = 1;
    private int gridInUseIndex = 0;
    private List<Grid> gridList;
    private List<GameObject> container;
    #endregion

    #region variables for grid construction
    private int gridWidth = 5;
    private int gridHeight = 5;
    private int gridCellSize = 10;
    private Vector3 gridOrigin = new Vector3(0, 0);
    #endregion

    #region variables for command pattern
    private BoardCommandInvoker boardCommandInvoker;
    #endregion

    #region variables for user actions
    private CellOccupiedStateSO selectedCellStateSO = null;
    private GridObject selectedGridObject = null;
    //private Piece selectedGridObjectPiece = null;
    //private TeleportGate selectedGridObjectGate = null;
    private int selectedPieceGridIndex = -1;
    #endregion

    private void Start() {
        numberOfGrid = 3;
        gridList = new List<Grid>();
        container = new List<GameObject>();
        InitialiseGrid();
    }

    private void Update() {
        HandleMouseInputLeft();
        HandleMouseInputRight();
        HandleKeyInputZ();
        HandleKeyInputX();
        HandleKeyInputLeftArrow();
        HandleKeyInputRightArrow();
    }

    #region HandlePlayerInput
    private void HandleMouseInputLeft() {

        if (!Input.GetMouseButtonDown(0)) return;
        Vector3 clickPosition = UtilsClass.GetMouseWorldPosition();
        if (!gridList[gridInUseIndex].IsInGrid(clickPosition)) return;
        if (selectedCellStateSO != null || selectedGridObject != null) {
            HandlePlacementUserAction(clickPosition);
        } else {
            HandleSelectionUserAction(clickPosition);
        }
    }
    private void HandleMouseInputRight() {
        if (Input.GetMouseButtonDown(1)) {
            // right click for debug
            Vector3 clickPosition = UtilsClass.GetMouseWorldPosition();
            if (gridList[gridInUseIndex].IsInGrid(clickPosition)) {
                Debug.Log(gridList[gridInUseIndex].GetCellDebugText(UtilsClass.GetMouseWorldPosition()));
            }
        }
    }
    private void HandleKeyInputRightArrow() {
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) {
            ChangeGridInUse(+1);
        }
    }
    private void HandleKeyInputLeftArrow() {
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) {
            ChangeGridInUse(-1);
        }
    }
    private void HandleKeyInputX() {
        if (Input.GetKeyDown(KeyCode.X)) {
            RedoPreviousMove();
        }
    }
    private void HandleKeyInputZ() {
        if (Input.GetKeyDown(KeyCode.Z)) {
            UndoPreviousMove();
        }
    }
    #endregion
    private void HandleSelectionUserAction(Vector3 clickPosition) {
        if (gridList[gridInUseIndex].GetGridObject(clickPosition) != null) {
            Piece piece = gridList[gridInUseIndex].GetGridObject(clickPosition).GetComponent<Piece>();
            if (piece != null) {
                selectedGridObject = piece;
                HighlightPossibleMove(true, piece);
                selectedPieceGridIndex = gridInUseIndex;
                selectedCellStateSO = null;
            }
        }
    }

    private void HandlePlacementUserAction(Vector3 clickPosition) {
        if (selectedCellStateSO != null) {
            SetCellState(gridList[gridInUseIndex], clickPosition);
        } else if (selectedGridObject != null) {

            if (selectedGridObject.GetComponent<Piece>() != null) {
                HandlePieceAction(clickPosition);
            } else if (selectedGridObject.GetComponent<TeleportGate>() != null) {
                SetGridObject(gridList[gridInUseIndex], clickPosition);
            } else if (selectedGridObject.GetComponent<Stone>() != null) {
                SetGridObject(gridList[gridInUseIndex], clickPosition);
            }
            //} else if (selectedGridObjectGate != null) {
            //SetTeleportGate(gridList[gridInUseIndex], clickPosition);
        }
    }

    private void HandlePieceAction(Vector3 clickPosition) {

        if (FindGridObjectFromAllGrid(selectedGridObject.GetComponent<GridObject>(), out int gridIndexFound, out int xFound, out int yFound)) {
            // selected piece is from grid, move piece

            if (gridList[gridInUseIndex].GetGridObject(clickPosition) != null) {


                if (selectedGridObject == gridList[gridInUseIndex].GetGridObject(clickPosition).GetComponent<Piece>()) {
                    // click same piece, remove selected piece
                    HighlightPossibleMove(false, null);
                    selectedGridObject = null;
                    return;
                }
                if (!canOverride) {
                    return;
                }
            }

            if (!canOverride) {
                // can only move to cell with self or empty stateSO 
                if (gridList[gridInUseIndex].GetCell(clickPosition).GetOccupiedState().stateName != emptySO.stateName &&
                    gridList[gridInUseIndex].GetCell(clickPosition).GetOccupiedState().stateName != selectedGridObject.GetCellStateSO().stateName) {
                    return;
                }
            }

            if (selectedPieceGridIndex != gridInUseIndex) {
                MovePieceAcrossGrid(gridList[selectedPieceGridIndex], gridList[gridInUseIndex], clickPosition, selectedGridObject.GetComponent<Piece>());
            } else {
                MovePieceInSameGrid(gridList[selectedPieceGridIndex], clickPosition, selectedGridObject.GetComponent<Piece>());
            }
        } else {
            // selected piece is not from grid, set piece
            SetGridObject(gridList[gridInUseIndex], clickPosition);
        }
    }

    private void HandlePostPlayerAction() {
        selectedGridObject = null;
        HighlightPossibleMove(false, null);
        selectedCellStateSO = null;
        ClearRedoStack();
        UpdateScore();
    }

    #region Player Action Execution
    private void SetGridObject(Grid grid, Vector3 clickPosition) {
        boardCommandInvoker.AddCommand(new SetGridObjectCommand(grid, clickPosition, selectedGridObject));
        HandlePostPlayerAction();
    }
    private void SetCellState(Grid grid, Vector3 clickPosition) {
        boardCommandInvoker.AddCommand(new SetCellStateCommand(grid, clickPosition, selectedCellStateSO));
        HandlePostPlayerAction();
    }
    private void MovePieceInSameGrid(Grid grid, Vector3 clickPosition, Piece pieceMoving) {

        grid.FindGridObject(pieceMoving, out int xFound, out int yFound);
        gridList[gridInUseIndex].GetXY(clickPosition, out int xTarget, out int yTarget);

        List<(int, int)> moveList = gridList[gridInUseIndex].GetAllPossibleMove(pieceMoving, emptySO);
        foreach (var move in moveList) {
            if ((xTarget, yTarget) == move) {
                Vector3 pieceOriginalPosition = grid.GetWorldPosition(xFound, yFound);
                boardCommandInvoker.AddCommand(new MovePieceCommand(grid, grid, pieceOriginalPosition, clickPosition));
                HandlePostPlayerAction();
                return;
            }
        }

    }
    private void MovePieceAcrossGrid(Grid originGrid, Grid destinationGrid, Vector3 clickPosition, Piece pieceMoving) {
        // check if can move (teleport)
        int teleportRadius = 1;
        bool canTeleport = false;

        List<TeleportGate> originGateList = new List<TeleportGate>();
        List<TeleportGate> destinationGateList = new List<TeleportGate>();

        originGrid.FindGridObject(pieceMoving, out int xFound, out int yFound);

        foreach (GridObject gridObject in originGrid.GetGridObjectListNearyBy(xFound, yFound, teleportRadius)) {
            if (gridObject.GetComponent<TeleportGate>() != null) {
                originGateList.Add(gridObject.GetComponent<TeleportGate>());
            }
        }

        foreach (GridObject gridObject in destinationGrid.GetGridObjectListNearyBy(clickPosition, teleportRadius)) {
            if (gridObject.GetComponent<TeleportGate>() != null) {
                destinationGateList.Add(gridObject.GetComponent<TeleportGate>());
            }
        }

        Debug.Log($"originGateListtCount: {originGateList.Count}");
        Debug.Log($"destinationGateListCount: {destinationGateList.Count}");

        foreach (TeleportGate originGate in originGateList) {
            foreach (TeleportGate destinationGate in destinationGateList) {
                Debug.Log($"originGateGroup: {originGate.GetGateGroup()}");
                Debug.Log($"destinationGateGroup: {destinationGate.GetGateGroup()}");

                if (originGate.IsSameGroup(destinationGate)) {
                    canTeleport = true;
                    break;
                }
            }
        }

        if (canTeleport) {
            // can move
            Vector3 pieceMovingOrigin = originGrid.GetWorldPosition(xFound, yFound);
            boardCommandInvoker.AddCommand(new MovePieceCommand(originGrid, destinationGrid, pieceMovingOrigin, clickPosition));
            HandlePostPlayerAction();
        } else {
            Debug.Log("Not able to teleport");
        }
    }
    public void UndoPreviousMove() {
        boardCommandInvoker.Undo();
        selectedCellStateSO = null;
        HighlightPossibleMove(false, null);
        selectedGridObject = null;
        UpdateScore();
    }
    public void RedoPreviousMove() {
        boardCommandInvoker.Redo();
        selectedCellStateSO = null;
        HighlightPossibleMove(false, null);
        selectedGridObject = null;
        UpdateScore();
    }
    public void ChangeGridInUse(int changeIndex) {

        int nextGridIndex = gridInUseIndex + changeIndex;
        if (0 <= nextGridIndex && nextGridIndex < gridList.Count) {
            // valid change
            container[gridInUseIndex].SetActive(false);
            container[nextGridIndex].SetActive(true);
            gridInUseIndex = nextGridIndex;
        } else {
            Debug.Log("Invalid Grid Change, grid is already left/right most");
        }
        UpdateGridInUseText();
    }
    #endregion

    public void InitialiseGrid() {


        if (gridList.Count != 0) {
            for (int i = 0; i < gridList.Count; i++) {
                gridList[i].DistroyAll();
                Destroy(container[i]);
            }
        }

        gridList.Clear();
        container.Clear();

        for (int i = 0; i < numberOfGrid; i++) {
            container.Add(new GameObject($"Board {i}"));
            gridList.Add(new Grid(gridWidth, gridHeight, gridCellSize, gridOrigin, container[i], cellPrefab, initialCellStateSO));
            if (i != gridInUseIndex) {
                container[i].SetActive(false);
            }
        }

        UpdateScore();
        UpdateGridInUseText();
        boardCommandInvoker = new BoardCommandInvoker();

        cam.orthographicSize = (gridHeight + 2) * gridCellSize / 2f;
        Vector3 gridCenter = new Vector3((gridHeight) * gridCellSize / 2f, (gridHeight) * gridCellSize / 2f, -10) + gridOrigin;
        var aspectRatio = (float)Screen.width / Screen.height;
        float camWidth = cam.orthographicSize * 2f * aspectRatio;
        float xOffset = -(camWidth - (gridHeight + 2) * gridCellSize) / 2; // offset used to align grid to right most of camera view
        cam.transform.position = gridCenter + new Vector3(xOffset, 0, 0);
    }

    public bool FindGridObjectFromAllGrid(GridObject gridObject, out int gridFoundIndex, out int xFound, out int yFound) {
        for (int i = 0; i < numberOfGrid; i++) {
            gridFoundIndex = i;
            if (gridList[i].FindGridObject(gridObject, out xFound, out yFound)) {
                return true;
            }
        }
        gridFoundIndex = xFound = yFound = -1;
        return false;
    }

    #region UI related methods
    public void ButtonSetSelectCellStateSO(ButtonSelectCellStateSO button) {
        selectedCellStateSO = button.GetCellStateSO();
        selectedGridObject = null;
    }
    public void ButtonSetSelectPiece(ButtonSelectPiece button) {
        selectedGridObject = button.GetPiecePrefab();
        selectedCellStateSO = null;
    }
    public void ButtonSetSelectGate(ButtonSelectTeleportGate button) {
        selectedGridObject = button.GetGatePrefab();
        selectedCellStateSO = null;
    }
    public void ButtonSetSelectStone(ButtonSelectStone button) {
        selectedGridObject = button.GetGatePrefab();
        selectedCellStateSO = null;
    }
    public void SetGridDimension(int dimension) {
        this.gridWidth = dimension;
        this.gridHeight = dimension;
    }
    public void SetNumberOfGrid(int numberOfGrid) {
        this.numberOfGrid = numberOfGrid;
    }
    public void UpdateGridInUseText() {
        gridInUseText.text = $"Current\nBoard: {gridInUseIndex + 1}/{gridList.Count}";
    }
    public void NewGame() {
        InitialiseGrid();
    }
    #endregion

    private void ClearRedoStack() {
        boardCommandInvoker.ClaerRedoStack();
    }

    public Dictionary<CellOccupiedStateSO, int> UpdateScore() {
        Dictionary<CellOccupiedStateSO, int> CellStateSOCellCountDict = new Dictionary<CellOccupiedStateSO, int>();
        for (int cellStateSOIndex = 0; cellStateSOIndex < cellStatesSOForScoreCountingList.Count; cellStateSOIndex++) {
            CellOccupiedStateSO targetCellStateSO = cellStatesSOForScoreCountingList[cellStateSOIndex];
            int count = 0;
            for (int gridIndex = 0; gridIndex < gridList.Count; gridIndex++) {
                count += gridList[gridIndex].CountCellWithStateSO(targetCellStateSO);
                CellStateSOCellCountDict[targetCellStateSO] = count;
            }
        }
        OnScoreChange?.Invoke(this, new ScoreChangeEventArgs(CellStateSOCellCountDict));
        return CellStateSOCellCountDict;
    }

    public void HighlightPossibleMove(bool trigger, Piece piece) {

        if (trigger) {
            gridList[gridInUseIndex].HightlightPossibleMove(piece, emptySO);
        } else {
            foreach (Grid grid in gridList) {
                grid.AllHighlightOff();
            }
        }

    }

}
