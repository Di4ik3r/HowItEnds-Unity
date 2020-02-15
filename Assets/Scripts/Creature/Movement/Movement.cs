using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Movement : IMovement {
    
    protected Creature creature;

    protected int movementBlock;

    protected int pathIndex;
    protected Stack pathToCell;
    protected Vector2 lookingForCell;
    public bool isMoving;


    // Змінні які потрібні для анімованого руху
    public float moveTime;
    public float moveArcHeight = 1f;
    public Vector3 moveStartPosition;
    public Vector3 moveTargetPosition;



    public Movement(Creature creature) {
        this.creature = creature;

        this.isMoving = false;
        
        this.lookingForCell = new Vector2(
            creature.transform.position.x,
            creature.transform.position.y
        );
        
        this.pathToCell = new Stack();

        this.movementBlock = -1;
    }

    // Jump logic (almost all the time: if is moving then animate)
    public void Jump() {
        if(this.isMoving)
            AnimateMoving();
        else {
            StartMoving();
        }
    }
    // protected void AnimateMoving () {
    //     moveTime = Mathf.Min (1, moveTime + Time.deltaTime * this.creature.speed);
    //     this.creature.transform.position = Vector3.Lerp (moveStartPosition, moveTargetPosition, moveTime);

    //     if (moveTime >= 1) {
    //         moveTime = 0;
    //         this.isMoving = false;
    //     }
    // }

    // protected void AnimateMoving () {
    //     moveTime = Mathf.Min (1, moveTime + Time.deltaTime * this.creature.speed);
    //     switch(this.movementType) {
    //         case MovementType.Air:
    //             this.creature.transform.position = Vector3.Lerp (moveStartPosition, moveTargetPosition, moveTime);
    //             break;
    //         case MovementType.Ground:
    //             float height = (1 - 4 * (moveTime - .5f) * (moveTime - .5f)) * moveArcHeight;
    //             break;
    //         case MovementType.Water:
    //             this.creature.transform.position = Vector3.Lerp (moveStartPosition, moveTargetPosition - new Vector3(0f, 1f, 0f), moveTime);
    //             break;
    //         default:
    //             this.creature.transform.position = Vector3.Lerp (moveStartPosition, moveTargetPosition, moveTime);
    //             Debug.Log("unpicked movement type");
    //             break;
    //     }

    //     if (moveTime >= 1) {
    //         moveTime = 0;
    //         this.isMoving = false;
    //     }
    // }

    protected abstract void AnimateMoving();

    public void StartMoving() {
        this.moveStartPosition = this.creature.transform.position;
        if(this.pathToCell.Count > 0) {
            Debug.Log("looking for cell");
            this.lookingForCell = (Vector2)this.pathToCell.Pop();
            this.moveTargetPosition = MoveLogic(this.lookingForCell);
        } else {
            this.moveTargetPosition = MoveLogic();
        }
        this.isMoving = true;

        // При кожному ході - збільшення показників відчуття голоду та спраги
        this.creature.hunger += this.creature.HUNGER_STEP;
        this.creature.thirst += this.creature.THIRST_STEP;

        // Debug.Log($"Hunger: {this.creature.hunger}; Thirst: {this.creature.thirst}");
    }


    // This 2 functions (MoveLogic) will differ in each movement
    // cuz it how movement system make dicision where to go
    protected abstract Vector3 MoveLogic();
    protected abstract Vector3 MoveLogic(Vector2 to);
    
}

