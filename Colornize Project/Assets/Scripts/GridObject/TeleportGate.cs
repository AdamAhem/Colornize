using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportGate : GridObject {

    // handle teleport gate logic

    private int teleportGroup; // piece can only teleport (move across grid) from and to cell near gate of same group

    public int GetGateGroup() {
        return teleportGroup;
    }

    public void SetGateGroup(int gateGroup) {
        this.teleportGroup = gateGroup;
    }

    public bool IsSameGroup(TeleportGate anotherGate) {
        return anotherGate.GetGateGroup() == teleportGroup;
    }
}
