using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundMovement : DirectionMovement {
    
    public GroundMovement(Creature creature)
    : base(creature) {
        this.movementBlock = 0;
    }

    public override void AnimateMoving () {
        moveTime = Mathf.Min (1, moveTime + Time.deltaTime * this.creature.speed);
        float height = (1 - 4 * (moveTime - .5f) * (moveTime - .5f)) * moveArcHeight;
        this.creature.transform.position = Vector3.Lerp (moveStartPosition, moveTargetPosition, moveTime) + Vector3.up * height;

        if (moveTime >= 1) {
            moveTime = 0;
            this.isMoving = false;
        }
    }
    
}

