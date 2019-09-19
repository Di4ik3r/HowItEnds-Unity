using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Creature : MonoBehaviour
{

    public static int ID_COUNTER = 0;
    public static int[] map;
    public static GameObject[] cubes;

    private static Vector2 DEATH_RANGE = new Vector2(6, 15);
    private static float HUNGER_LIMIT = .6f, HUNGER_STEP = .08f;
    private static float THIRST_LIMIT = .4f, THIRST_STEP = .06f;

    private int     id;
    private bool    isAlive;
    private float   hunger;
    private float   thirst;
    private int     birthDay;
    private int     deathDay;
    private float   speed;
    private float   weight;
    private bool    isMoving;
    private Vector2 lookingForCell;
    private bool    isLookingFor;

    private bool isHunger { get { return this.hunger >= Creature.HUNGER_LIMIT ? true : false; } }
    private bool isThirst { get { return this.thirst >= Creature.THIRST_LIMIT ? true : false; } }

    public Creature(Vector2 position, int birthDay) {
        this.id = Creature.ID_COUNTER++;
        this.hunger = 0;
        this.thirst = 0;
        this.birthDay = birthDay;
        this.deathDay = (int)Random.Range(DEATH_RANGE.x, DEATH_RANGE.y);
        this.speed = 0;
        this.weight = 0;
        this.isMoving = false;
        this.isLookingFor = false;
        this.lookingForCell = new Vector2(position.x, position.y);
        this.isAlive = true;

        this.transform.position = new Vector3(position.x, 0, position.y);
    }

    public void MakeMove() {
        if(this.isMoving || !this.isAlive)
            return;
        
        if(isHunger)
            FindFood();
        if(isThirst)
            FindWater();

        Jump();
    }

    private void Jump() {

        this.hunger += HUNGER_STEP;
        this.thirst += THIRST_STEP;
    }

    private void FindWater() {

    }

    private void FindFood() {

    }
}
