using UnityEngine;

public class SetCellStateCommand : ICommand {

    Grid grid;
    Vector3 cellWorldPosition;
    GridObject objectOverrode;
    CellOccupiedStateSO newCellStateSO;
    CellOccupiedStateSO lastCellStateSO;

    public SetCellStateCommand(Grid gird, Vector3 worldPosition, CellOccupiedStateSO cellStateSO) {
        this.grid = gird;
        this.cellWorldPosition = worldPosition;
        this.newCellStateSO = cellStateSO;
    }

    public void Execute() {
        lastCellStateSO = grid.GetCell(cellWorldPosition).GetOccupiedState();
        grid.GetCell(cellWorldPosition).SetOccupiedState(newCellStateSO);

        objectOverrode = grid.GetGridObject(cellWorldPosition);
        if (objectOverrode != null) {
            grid.RemoveGridObjectAt(cellWorldPosition);
            objectOverrode.gameObject.SetActive(false);
        }
    }

    public void Undo() {
        if (objectOverrode != null) {
            objectOverrode.gameObject.SetActive(true);
            grid.SetGridObject(cellWorldPosition, objectOverrode);
        }
        grid.GetCell(cellWorldPosition).SetOccupiedState(lastCellStateSO);
    }
}
