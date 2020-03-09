using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class PredatoryCreature : Creature {

    public Vector2 consumedCreature;

    public static PredatoryCreature Create(Vector2 position, int birthDay) {
        var creature = Creature.Create<PredatoryCreature>(position, birthDay);

        creature.InitProperties();

        return creature;
    }

    protected void InitProperties() {
        this.type = CreatureType.Predatory;

        this.gameObject.GetComponent<Renderer>().materials[0].color = new Color(
            this.speed.Map(this.speedLimit.x, this.speedLimit.y, 0.15f, 0.6f),
            this.speed.Map(this.speedLimit.x, this.speedLimit.y, 0.0f, 0.15f),
            0f);
    }


    public override void MakeMove() {
        if(!this.isAlive) {
            this.movement.DeadMove();
            return;
        }

        
        switch (this.action) {
            case CreatureAction.Eating:
                Eat();
                break;
            
            case CreatureAction.Drinking:
                Drink();
                break;

            case CreatureAction.Walking:
                if(isHunger) {
                    var weakCreature = GetWeakCreature();
                    if(weakCreature.x != -1)
                        Debug.Log(weakCreature);
                    if(IsNeededBlockInTouch(weakCreature)) {
                        this.consumedCreature = weakCreature;
                        CreatureManager.Instance.KillCreature(weakCreature);
                        StartEat();
                    } else {
                        // Debug.Log("starting search");
                        FindFood();
                    }
                // andrii;
                } else if(isThirst) {
                    if(IsNeededBlockInTouch(1)) {
                        StartDrink();
                    } else {
                        FindWater();
                    }
                }
                break;

            default:
                break;
        }
                this.movement.Jump();
    }

    protected virtual void Eat() {
        consumeTime += consumeStep;

        if(consumeTime >= consumeLimit * eatingMultiplier) {
            this.consumeTime = 0;
            this.action = CreatureAction.Walking;
            this.hunger = 0;
            CreatureManager.Instance.Borrow(this.consumedCreature);
        }
    }


    protected override void FindFood() {
        // var foodBlock = GetBlock(2);
      
        var foodBlock = GetWeakCreature(this.searchRadius);
        if(foodBlock.x != -1) {
            this.foodBlock = foodBlock;
            this.movement.MoveTo(foodBlock);
        }
    }
}
