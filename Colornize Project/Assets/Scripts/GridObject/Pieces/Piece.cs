using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Piece : GridObject {

    public abstract bool IsMoveValid(int currentX, int currentY, int targetX, int targetY);

}
