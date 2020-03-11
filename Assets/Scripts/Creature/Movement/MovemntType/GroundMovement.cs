using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundMovement : DirectionMovement {
    
    public GroundMovement(Creature creature)
    : base(creature) {
        this.movementBlock = 0;
    }

    public override void AnimateMoving () {
        moveTime = Mathf.Min (this.timeLimit, moveTime + Time.deltaTime * this.creature.speed);
        float height = (1 - 4 * (moveTime - .5f) * (moveTime - .5f)) * moveArcHeight;
        this.creature.transform.position = Vector3.Lerp (moveStartPosition, moveTargetPosition, moveTime) + Vector3.up * height;

        var rizn = this.moveTargetPosition - this.moveStartPosition;
        rizn.y = 0;
        if(rizn != Vector3.zero) {
            this.lookRotation = lookRotation = Quaternion.LookRotation((rizn).normalized);
            this.creature.transform.rotation = Quaternion.Slerp(this.creature.transform.rotation, this.lookRotation, Time.deltaTime * rotationSpeed);
        }
        

        if (moveTime >= this.timeLimit) {
            if(!this.creature.isAlive) {
                this.creature.DestroyGameObject();
                return;
            }
            moveTime = 0;
            this.isMoving = false;
        }
    }
    
}

