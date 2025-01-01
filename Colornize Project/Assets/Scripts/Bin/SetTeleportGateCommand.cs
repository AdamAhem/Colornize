using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetTeleportGateCommand : ICommand {

    Grid grid;
    Vector3 cellWorldPosition;
    TeleportGate gatePrefab;
    CellOccupiedStateSO newCellStateSO;
    CellOccupiedStateSO lastCellStateSO;
    TeleportGate gateSet;

    public SetTeleportGateCommand(Grid gird, Vector3 worldPosition, TeleportGate gatePrefab) {
        this.grid = gird;
        this.cellWorldPosition = worldPosition;
        this.gatePrefab = gatePrefab;
    }

    public void Execute() {
        lastCellStateSO = grid.GetCell(cellWorldPosition).GetOccupiedState();
        gateSet = GameObject.Instantiate(gatePrefab);
        grid.SetGridObject(cellWorldPosition, gateSet);
        grid.GetCell(cellWorldPosition).SetOccupiedState(gateSet.GetComponent<GridObject>().GetCellStateSO());
    }

    public void Undo() {
        Debug.Log("Undo SetGate");
    }

}