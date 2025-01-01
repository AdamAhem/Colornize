using System;
using UnityEngine;

public class GridCell : MonoBehaviour {

    // A GridCell store information about its state

    public event EventHandler OnCellStateChange;
    public event Action<bool> OnChangeHighlight;

    private CellOccupiedStateSO cellOccupiedState;

    public GridCell(CellOccupiedStateSO stateSO) {
        this.cellOccupiedState = stateSO;
    }

    public void SetOccupiedState(CellOccupiedStateSO newState) {
        cellOccupiedState = newState;
        OnCellStateChange?.Invoke(this, EventArgs.Empty);
    }

    public CellOccupiedStateSO GetOccupiedState() {
        return cellOccupiedState;
    }

    public string GetDebugString() {
        return $"GridCell {{ Name: {this.name}, State: {this.cellOccupiedState.stateName}}}";
    }

    public void DestroySelf() {
        Destroy(this.gameObject);
    }

    public void ChangeHighlight(bool trigger) {
        OnChangeHighlight?.Invoke(trigger);
    }

}
