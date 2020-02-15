using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterMovement : DirectionMovement {
    
    public WaterMovement(Creature creature)
    : base(creature) {
        this.movementBlock = 1;
    }

    protected override void AnimateMoving () {
        moveTime = Mathf.Min (1, moveTime + Time.deltaTime * this.creature.speed);
        this.creature.transform.position = Vector3.Lerp (moveStartPosition, moveTargetPosition - new Vector3(0f, 1f, 0f), moveTime);

        if (moveTime >= 1) {
            moveTime = 0;
            this.isMoving = false;
        }
    }
    
}

