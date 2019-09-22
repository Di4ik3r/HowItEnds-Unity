using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature {

    [SerializeField]
    private GameObject gameObject;
    public Transform transform { get{ return this.gameObject.transform; } }
    public Vector3 position { get{ return this.gameObject.transform.position; }  set { this.gameObject.transform.position = value; } }

    public static int ID_COUNTER = 0;
    public static int[,] digitalMap;
    public static GameObject[,] objectMap;

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

    private Mesh mesh;
    private float meshHeight;

    private bool isHunger { get { return this.hunger >= Creature.HUNGER_LIMIT ? true : false; } }
    private bool isThirst { get { return this.thirst >= Creature.THIRST_LIMIT ? true : false; } }

    public Creature(Vector2 position, int birthDay) {
        this.mesh = PrimitiveHelper.GetPrimitiveMesh(PrimitiveType.Cube);
        implementComponents();
        // this.mesh = Resources.Load<GameObject>("Prefab/CreaturePrefab");
        // this.mesh = PrimitiveType.Sphere;
        // this.mesh = GameObject.CreatePrimitive(PrimitiveType.Cube);
        // this.mesh = (GameObject)Resources.Load("CreaturePrefab");

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

        this.meshHeight = 1;

        this.position = new Vector3(position.x, 
                                    Creature.objectMap[(int)position.x, (int)position.y].transform.position.y + this.meshHeight,
                                    position.y);
    }

    private void implementComponents() {
        this.gameObject = new GameObject("Creature");

        // this.gameObject.AddComponent<Transform>();
        this.gameObject.AddComponent<MeshFilter>();
        this.gameObject.AddComponent<MeshRenderer>();

        this.gameObject.GetComponent<MeshFilter>().mesh = this.mesh;
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
        this.position = moveLogic();

        this.hunger += HUNGER_STEP;
        this.thirst += THIRST_STEP;
    }

    private Vector3 moveLogic() {
        float   x = this.position.x,
                y = this.position.y,
                z = this.position.z;
        Vector3 result;

        Vector2[] possibleChoices = new Vector2[8];

        int choice = Random.Range(0, 7), indexer = 0;

        possibleChoices[indexer++] = new Vector2(this.position.x - 1,   this.position.z + 1);     // 1    zdelanno
        possibleChoices[indexer++] = new Vector2(this.position.x,       this.position.z + 1);     // 2    zdelanno
        possibleChoices[indexer++] = new Vector2(this.position.x + 1,   this.position.z + 1);     // 3    zdelanno
        possibleChoices[indexer++] = new Vector2(this.position.x + 1,   this.position.z);         // 4    zdelanno
        possibleChoices[indexer++] = new Vector2(this.position.x + 1,   this.position.z - 1);     // 5    zdelanno
        possibleChoices[indexer++] = new Vector2(this.position.x,       this.position.z - 1);     // 6    zdelanno
        possibleChoices[indexer++] = new Vector2(this.position.x - 1,   this.position.z - 1);     // 7    zdelanno
        possibleChoices[indexer++] = new Vector2(this.position.x - 1,   this.position.z);         // 8    zdelanno

        List<Vector2> provedChoices = new List<Vector2>();
        foreach(Vector2 possibleChoice in possibleChoices) {
            if( possibleChoice.x > 0 && possibleChoice.x < Creature.digitalMap.GetLength(0)
                && possibleChoice.y > 0 && possibleChoice.y < Creature.digitalMap.GetLength(1))
            provedChoices.Add(possibleChoice);
        }

        choice = Random.Range(0, provedChoices.Count - 1);

        // bool available = Creature.digitalMap[(int)possibleChoices[choice].x, (int)possibleChoices[choice].y] == 0;
        bool available = Creature.digitalMap[(int)provedChoices[choice].x, (int)provedChoices[choice].y] == 0;

        while(!available) {
            choice = Random.Range(0, 7);
            available = Creature.digitalMap[(int)provedChoices[choice].x, (int)provedChoices[choice].y] == 0;
        }

        // while(!available) {
        //     choice = Random.Range(0, 7);
        //     Debug.Log(choice + ":_ " + (int)possibleChoices[choice].x + " : " + (int)possibleChoices[choice].y + " = " + Creature.digitalMap[(int)possibleChoices[choice].x, (int)possibleChoices[choice].y]);
        //     Debug.Log(Creature.digitalMap.GetLength(0) + " : " + Creature.digitalMap.GetLength(1));            
        //     available = Creature.digitalMap[(int)possibleChoices[choice].x, (int)possibleChoices[choice].y] == 0;
        // }

        // x = possibleChoices[choice].x;
        // z = possibleChoices[choice].y;

        x = provedChoices[choice].x;
        z = provedChoices[choice].y;

        // switch(choice) {
        //     case 1: { x = this.position.x - 1;  z = this.position.z + 1;    break; }
        //     case 2: { x = this.position.x;      z = this.position.z + 1;    break; }
        //     case 3: { x = this.position.x + 1;  z = this.position.z + 1;    break; }
        //     case 4: { x = this.position.x + 1;  z = this.position.z;        break; }
        //     case 5: { x = this.position.x + 1;  z = this.position.z - 1;    break; }
        //     case 6: { x = this.position.x;      z = this.position.z - 1;    break; }
        //     case 7: { x = this.position.x - 1;  z = this.position.z - 1;    break; }
        //     case 8: { x = this.position.x - 1;  z = this.position.z;        break; }
        // }

        // x = normalizeX(x);
        // z = normalizeZ(z);

        y = Creature.objectMap[(int)x, (int)z].transform.position.y + this.meshHeight;

        // if(x < 0)
        //     if(this.position.x == 0)
        //         x = 1;
        //     else x = 0;
        // if(z < 0) 
        //     if(this.position.z == 0)
        //         z = 1;
        //     else z = 0;

        result = new Vector3(x, y, z);

        return result;
    }

    private float normalizeX(int x) {
        if(x < 0)
            return 0;
        if(x >= Creature.digitalMap.GetLength(0))
            return Creature.digitalMap.GetLength(0) - 1;

        return 0;
    }

    private float normalizeZ(int z) {
        if(z < 0)
            return 0;
        if(z >= Creature.digitalMap.GetLength(1))
            return Creature.digitalMap.GetLength(1) - 1;

        return 0;
    }

    // private float normalizeX(float x) {
    //     if(x < 0)
    //         if(this.position.x == 0)
    //             return 1;
    //         else 
    //             return x;
    //     if(x + 1 >= Creature.digitalMap.GetLength(1))
    //         if(this.position.x == Creature.digitalMap.GetLength(1))
    //             return Creature.digitalMap.GetLength(1) - 1;
    //     return x;
    //     // return x < 0 ? this.position.x == 0 ? 1 : 0 : x;
    // }

    // private float normalizeZ(float z) {
    //     if(z < 0)
    //         if(this.position.z == 0)
    //             return 1;
    //         else 
    //             return z;
    //     if(z + 1 >= Creature.digitalMap.GetLength(0))
    //         if(this.position.z == Creature.digitalMap.GetLength(0))
    //             return Creature.digitalMap.GetLength(0) - 1;
    //     return z;
    //     // return z < 0 ? this.position.z == 0 ? 1 : 0 : z;
    // }

    private void FindWater() {

    }

    private void FindFood() {

    }
}
