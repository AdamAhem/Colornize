using UnityEngine;

public class MovePieceCommand : ICommand {

    Grid originGrid;
    Grid destinationGrid;
    Vector3 originWorldPosition;
    Vector3 destinationWorldPosition;
    Piece pieceMoving;
    GridObject objectOverrode;
    CellOccupiedStateSO lastCellStateSO;

    /// <summary>
    /// Command to move a piece in origin cell to a destination cell
    /// </summary>
    /// <param name="originGrid">The starting grid piece is in</param>
    /// <param name="destinationGrid">The destination grid will be in</param>
    /// <param name="originWorldPosition">The starting world position of piece</param>
    /// <param name="destinationWorldPosition">The destination world position of piece</param>
    public MovePieceCommand(Grid originGrid, Grid destinationGrid, Vector3 originWorldPosition, Vector3 destinationWorldPosition) {
        this.originGrid = originGrid;
        this.destinationGrid = destinationGrid;
        this.originWorldPosition = originWorldPosition;
        this.destinationWorldPosition = destinationWorldPosition;
    }

    public void Execute() {
        pieceMoving = originGrid.GetGridObject(originWorldPosition).GetComponent<Piece>();

        // 1. store pieceOverrode and cell state at destination grid before move is done
        objectOverrode = destinationGrid.GetGridObject(destinationWorldPosition);
        lastCellStateSO = destinationGrid.GetCell(destinationWorldPosition).GetOccupiedState();

        // 2. if have pieceOverrode at destination cell, disable it
        if (objectOverrode != null) {
            // before move piece, cell has piece
            // remove and record piece removed (use disable instead of destroy)
            destinationGrid.RemoveGridObjectAt(destinationWorldPosition);
            objectOverrode.gameObject.SetActive(false);
        }

        // 3. remove the piece moving from its grid
        originGrid.RemoveGridObjectAt(originWorldPosition);

        // 4. set piece moving to destination position, update cell state
        destinationGrid.SetGridObject(destinationWorldPosition, pieceMoving.GetComponent<GridObject>());
        destinationGrid.GetCell(destinationWorldPosition).SetOccupiedState(pieceMoving.GetComponent<GridObject>().GetCellStateSO());
    }

    public void Undo() {
        // 1. remove piece moving from destination grid and cell, change back destination cell state
        destinationGrid.RemoveGridObjectAt(destinationWorldPosition);
        destinationGrid.GetCell(destinationWorldPosition).SetOccupiedState(lastCellStateSO);

        // 2. set piece moving back to origin grid and cell
        originGrid.SetGridObject(originWorldPosition, pieceMoving.GetComponent<GridObject>());

        // 3. enable piece overrode at destination is any and set it back to cell
        if (objectOverrode != null) {
            // before move piece, cell has piece
            objectOverrode.gameObject.SetActive(true);
            destinationGrid.SetGridObject(destinationWorldPosition, objectOverrode);
        }
    }
}