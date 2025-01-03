using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceSleeping : Piece {
    public override bool IsMoveValid(int currentX, int currentY, int targetX, int targetY) {
        return false;
    }
}