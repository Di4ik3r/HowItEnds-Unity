using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Movement : IMovement {
    
    protected Creature creature;

    protected int movementBlock;

    protected int pathIndex;
    public Stack pathToCell;
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

    public void Update() {
        this.Jump();
    }

    // Jump logic (almost all the time: if is moving then animate)
    public void Jump() {
        if(this.isMoving)
            AnimateMoving();
        else {
            StartMoving();
        }
    }

    public abstract void AnimateMoving();

    public void StartMoving() {
        this.moveStartPosition = this.creature.transform.position;
        if(this.pathToCell.Count > 0) {
            this.PaintPath();
            // Debug.Log("looking for cell");
            this.lookingForCell = (Vector2)this.pathToCell.Pop();
            this.moveTargetPosition = MoveLogic(this.lookingForCell);
            if(this.pathToCell.Count <= 0) {
                Debug.Log(this.pathToCell.Count);
                // this.creature.isConsuming = false;
                // this.creature.hunger = 0;
                // this.creature.PaintToDefault();
            }
        } else {
            switch(this.creature.action) {
                case CreatureAction.Drinking:
                case CreatureAction.Eating:
                    this.moveTargetPosition = this.moveStartPosition;
                    break;
                default:
                    this.moveTargetPosition = MoveLogic();
                    break;
            }
            // this.moveTargetPosition = MoveLogic();
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


    public void MoveTo(Vector2 to) {
        this.pathToCell = new PathFinding().FindPath(
            new Vector2(this.creature.transform.position.x, this.creature.transform.position.z),
            new Vector2(to.x, to.y)
        );

        // Debug.Log($"{to} {Creature.digitalMap[(int)to.x, (int)to.y]}");
        // foreach (var block in pathToCell) {
        //     Debug.Log(block);
        // }

        // Debug.Log(this.pathToCell.Count);

        // this.creature.isConsuming = true;
    }
    

    public void PaintPath() {
        foreach (Vector2 block in this.pathToCell) {
            Creature.objectMap[(int)block.x, (int)block.y]
                .GetComponent<Renderer>()
                .materials[0]
                .color = new Color(0.2f, 0.3f, 0.4f);
        }
    }

    public bool PathIsExist() {
        return this.pathToCell.Count == 0 ? false: true;
    }
}
