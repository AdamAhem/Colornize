using UnityEngine;

public class SetGridObjectCommand : ICommand {

    Grid grid;
    Vector3 cellWorldPosition;
    GridObject objectPrefabStored;
    GridObject objectOverrode;
    CellOccupiedStateSO newCellStateSO;
    CellOccupiedStateSO lastCellStateSO;
    GridObject objectSet;

    public SetGridObjectCommand(Grid gird, Vector3 worldPosition, GridObject objectPrefab) {
        this.grid = gird;
        this.cellWorldPosition = worldPosition;
        this.objectPrefabStored = GameObject.Instantiate(objectPrefab);

        // update data manually to stored prefab
        this.objectPrefabStored.SetCellStateSO(objectPrefab.GetCellStateSO());
        if (objectPrefabStored.GetComponent<TeleportGate>() != null) {
            // manually set script data 
            this.objectPrefabStored.GetComponent<TeleportGate>().SetGateGroup(objectPrefab.GetComponent<TeleportGate>().GetGateGroup());
        }
        // hide stored prefab
        this.objectPrefabStored.gameObject.SetActive(false);
    }

    public void Execute() {
        objectSet = GameObject.Instantiate(objectPrefabStored);

        // update data manually to spawned prefab
        this.objectSet.SetCellStateSO(objectPrefabStored.GetCellStateSO());
        if (objectSet.GetComponent<TeleportGate>() != null) {
            // manually set script data 
            this.objectSet.GetComponent<TeleportGate>().SetGateGroup(objectPrefabStored.GetComponent<TeleportGate>().GetGateGroup());
        }

        objectSet.gameObject.SetActive(true);

        objectOverrode = grid.GetGridObject(cellWorldPosition);
        lastCellStateSO = grid.GetCell(cellWorldPosition).GetOccupiedState();
        if (objectOverrode != null) {
            // before spawn piece, cell has piece
            // remove and record piece removed (use disable instead of destroy)
            objectOverrode.gameObject.SetActive(false);
        }
        grid.SetGridObject(cellWorldPosition, objectSet);
        grid.GetCell(cellWorldPosition).SetOccupiedState(objectPrefabStored.GetCellStateSO());

    }

    public void Undo() {
        grid.RemoveGridObjectAt(cellWorldPosition);
        objectSet.DestroySelf();
        if (objectOverrode != null) {
            // before spawn piece, cell has piece
            // remove spawned piece, enable last piece, reset last cellState
            objectOverrode.gameObject.SetActive(true);
            grid.SetGridObject(cellWorldPosition, objectOverrode);
        } else {
            // before spawn piece, cell has no piece
            // remove spawned piece, reset last cellState
        }
        grid.GetCell(cellWorldPosition).SetOccupiedState(lastCellStateSO);
    }

}
