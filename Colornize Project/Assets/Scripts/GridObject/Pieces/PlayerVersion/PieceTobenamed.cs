using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceTobenamed : Piece {
    public override bool IsMoveValid(int currentX, int currentY, int targetX, int targetY) {

        int dx = Mathf.Abs(targetX - currentX);
        int dy = Mathf.Abs(targetY - currentY);

        return (dx + dy == 1) || (dx + dy == 4 && (dx == 0 || dy == 0));
    }
}