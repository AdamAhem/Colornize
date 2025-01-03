using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPieceRook : Piece {

    public override bool IsMoveValid(int currentX, int currentY, int targetX, int targetY) {
        int dx = Mathf.Abs(targetX - currentX);
        int dy = Mathf.Abs(targetY - currentY);
        Debug.Log("checkingMove");
        return ((dx <= 2 && dy == 0) || (dy <= 2 && dx == 0));
    }
}
