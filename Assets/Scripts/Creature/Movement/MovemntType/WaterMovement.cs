using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterMovement : DirectionMovement {
    
    public WaterMovement(Creature creature)
    : base(creature) {
        this.movementBlock = 1;
    }

    public override void AnimateMoving () {
        moveTime = Mathf.Min (1, moveTime + Time.deltaTime * this.creature.speed);
        // float height = (1 - 4 * (moveTime - .5f) * (moveTime - .5f)) * moveArcHeight;
        this.creature.transform.position = Vector3.Lerp (moveStartPosition, moveTargetPosition, moveTime);
        // this.creature.transform.position = Vector3.Lerp (moveStartPosition, moveTargetPosition - new Vector3(0f, 1f, 0f), moveTime);

        if (moveTime >= 1) {
            moveTime = 0;
            this.isMoving = false;
        }
    }

    protected override float GetCubeHeight(float x, float z) {
        // return Creature.objectMap[(int)x, (int)z].transform.position.y + this.creature.meshHeight - ((1 - this.creature.scale) / 2)
        return Creature.objectMap[(int)x, (int)z].transform.position.y;
    }
    
}

