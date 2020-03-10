using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.DayNightCycle;

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
    public float arcHeight = 1f;
    public Vector3 moveStartPosition;
    public Vector3 moveTargetPosition;
    public float timeLimit = 1f;
    public Quaternion lookRotation;
    public float rotationSpeed = 10f;



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
            // this.PaintPath();
            this.lookingForCell = (Vector2)this.pathToCell.Pop();
            this.moveTargetPosition = MoveLogic(this.lookingForCell);
        } else {
            switch(this.creature.action) {
                case CreatureAction.Drinking:
                case CreatureAction.Eating:
                    this.moveArcHeight = 0;
                    this.moveTargetPosition = this.moveStartPosition;
                    break;
                default:
                    this.moveArcHeight = this.arcHeight;
                    this.moveTargetPosition = MoveLogic();
                    break;
            }
            // this.moveTargetPosition = MoveLogic();
        }
        this.isMoving = true;

        // При кожному ході - збільшення показників відчуття голоду та спраги
        this.creature.hunger += this.creature.HUNGER_STEP * this.creature.hungerMultiplier / TimeHolder.Instance.DayLength;
        this.creature.thirst += this.creature.THIRST_STEP * this.creature.thirstMultiplier / TimeHolder.Instance.DayLength;
        this.creature.RefreshStatus();
        this.creature.Check();
    }

    public void DeadMove() {
        this.AnimateMoving();
    }

    public void Borrow() {
        this.moveStartPosition = this.moveTargetPosition;
        // this.moveTargetPosition += Vector3.up * (-5);
        this.moveTargetPosition = new Vector3(moveTargetPosition.x, -2, moveTargetPosition.z);
        moveTime = 0;
        this.isMoving = false;
        this.creature.speed = 0.5f;

        Creature.digitalMap[(int)Mathf.Floor(this.creature.transform.position.x), (int)Mathf.Floor(this.creature.transform.position.z)] = 0;
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

    public void RefreshAnimVariables() {
        this.moveArcHeight = this.arcHeight;
    }
}
