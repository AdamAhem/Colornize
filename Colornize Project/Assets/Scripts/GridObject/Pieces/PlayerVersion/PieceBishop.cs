using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceBishop : Piece {
    public override bool IsMoveValid(int currentX, int currentY, int targetX, int targetY) {

        int dx = Mathf.Abs(targetX - currentX);
        int dy = Mathf.Abs(targetY - currentY);

        return (dx == dy) && (dx + dy <= 4);
    }
}