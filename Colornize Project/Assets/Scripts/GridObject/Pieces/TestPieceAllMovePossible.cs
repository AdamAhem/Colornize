using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPieceAllMovePossible : Piece {

    public override bool IsMoveValid(int currentX, int currentY, int targetX, int targetY) {
        return true;
    }
}

