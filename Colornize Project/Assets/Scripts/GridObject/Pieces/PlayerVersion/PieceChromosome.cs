using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceChromosome : Piece {
    public override bool IsMoveValid(int currentX, int currentY, int targetX, int targetY) {

        int dx = Mathf.Abs(targetX - currentX);
        int dy = Mathf.Abs(targetY - currentY);

        return (dy == 1) && (dx == 1 || dx == 2);
    }
}