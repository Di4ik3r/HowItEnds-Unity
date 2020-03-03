using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Creature : MonoBehaviour {

    // Основне поле класу, яке тримає в собі основні компоненти Юніті (такі як Трансформ) і куда можна
    // добавити нові компоненти (такі як МешРендерер, МешФільтр і тд)
    
    // Статичне поле даного класу, яке дозволяє відслідковувати і обробляти ідентифікатори для кожного
    // юніта
    public static int ID_COUNTER = 0;
    // Статичне поле даного класу, яке дозволяє тримати ІНТовський масив карти
    public static int[,] digitalMap;
    // Статичне поле даного класу, яке дозволяє тримати масив кубів карти    
    public static GameObject[,] objectMap;

    // Статичне поле даного класу, яке дозволяє визначити мінімальну і максимальну кількість днів в межах
    // якої буде обиратись скільки Юніт проживе (в днях)
    public static Vector2 DEATH_RANGE = new Vector2(6, 15);
    // Ліміт, при досягненні якого - Юніт буде шукати їжу. Крок, який буде додаватись до загального показника
    // голоду
    public float HUNGER_LIMIT = .6f, HUNGER_STEP = .0004f;
    // Ліміт, при досягненні якого - Юніт буде шукати воду. Крок, який буде додаватись до загального показника
    // спраги
    // private float THIRST_LIMIT = .4f, THIRST_STEP = .06f;
    public float THIRST_LIMIT = .5f, THIRST_STEP = .0006f;

    // Ідентифікатор Юніта
    private int     id;
    // Булівське значення, що відображає чи живий Юніт
    private bool    isAlive;
    // Показник голоду
    public float   hunger;
    // Показник спраги
    public float   thirst;
    // День, коли Юніт був створений
    private int     birthDay;
    // День, коли юніт помре природньою смертю (День народження + Рандом.Рендж(ДЕАД_РЕНДЖ.х, ДЕАД_РЕНДЖ.у))
    private int     deathDay;
    // Швидкість
    public float   speed;
    // Маса
    private float   weight;
    
    protected int searchRadius;

    // Висота Меша. Потрібно для того щоб юніт правильно ставав на блоки по висоті
    public float meshHeight;
    public float scale;
    protected Vector2 speedLimit = new Vector2(0.6f, 2.4f);

    // Властивість, що відображає чи відчуває голод Юніт
    private bool isHunger { get { return this.hunger >= this.HUNGER_LIMIT ? true : false; } }
    // Властивість, що відображає чи відчуває спрагу Юніт
    private bool isThirst { get { return this.thirst >= this.THIRST_LIMIT ? true : false; } }
    

    public IMovement movement;
    // Ліміт максимально можливих ходів в даному напрямку
    public int MOVES_LIMIT_MIN = 3;
    public int MOVES_LIMIT_MAX = 12;

    // public static GameObject prefab;

    ///CHANGES MADE BY ILLUHA
    ///CHANGES MADE BY ILLUHA
    [Header("Creatures stats UI")]
    public GameObject statsCanvas;
    public Image foodBar;
    public Image waterBar;

    void OnMouseDown()
    {
        foodBar.fillAmount += 0.1f;
        waterBar.fillAmount += 0.1f;
        statsCanvas.SetActive(true);
    }
    ///CHANGES MADE BY ILLUHA
    ///CHANGES MADE BY ILLUHA

    public static T Create<T>(Vector2 position, int birthDay) {
        GameObject prefab = Resources.Load<GameObject>($"Creature/{typeof(T).FullName}Prefab");
        GameObject newObject = Instantiate(prefab) as GameObject;
        T obj = newObject.GetComponent<T>();

        // parameters init here
        (obj as Creature).InitProperties(position, birthDay);
        
        // Позначення в глобальному масиві, що дана клітка зайнята
        Creature.digitalMap[(int)position.x, (int)position.y] = 9;

        return obj;
    }

    public void Update() {
        this.MakeMove();
    }

    private void InitProperties(Vector2 position, int birthDay) {
        // Ініціалізація полів
        this.id = Creature.ID_COUNTER++;
        this.hunger = 0;
        this.thirst = 0;
        this.birthDay = birthDay;
        this.deathDay = (int)Random.Range(DEATH_RANGE.x, DEATH_RANGE.y);
        this.speed = Random.Range(this.speedLimit.x, this.speedLimit.y); 
        this.weight = 0;

        this.movement = new WaterMovement(this);
        
        this.isAlive = true;

        
        this.searchRadius = 3;

        this.scale = 1 - this.speed.Map(this.speedLimit.x, this.speedLimit.y, 0.3f, 0.7f);
        this.meshHeight = this.scale;
        // this.meshHeight = 1;

        this.transform.position = new Vector3(position.x, 
                                    Creature.objectMap[(int)position.x, (int)position.y].transform.position.y + this.meshHeight,
                                    position.y);
                                    
        this.transform.localScale = new Vector3(this.scale, this.scale, this.scale);


        this.gameObject.GetComponent<Renderer>().materials[0].color = new Color(
            1f,
            1f,
            1f);
    }


    // Логіка руху Юніта
    public void MakeMove() {
        if(!this.isAlive)
            return;

        // Якщо голодний - шукаємо їжу
        if(isHunger) {
            FindFood();
            // andrii;
        }
        if(isThirst) {
            FindWater();

        }

        this.movement.Jump();
    }

    private void FindWater() {
        
        // PaintToDefault();

        // var blocks = GetBlocksByRadius(this.searchRadius);

        // foreach (var block in blocks) {
        //     // Debug.Log($"block: {block}");
        //     Creature.objectMap[(int)block.x, (int)block.y]
        //         .GetComponent<Renderer>()
        //         .materials[0]
        //         .color = new Color(0f, 0f, 0.3f);

        //     if(Creature.digitalMap[(int)block.x, (int)block.y] == 1) {
        //         this.pathToCell = new PathFinding().FindPath(
        //             new Vector2(this.transform.position.x, this.transform.position.z),
        //             new Vector2(block.x, block.y));

        //         Debug.Log(this.pathToCell.Count);
        //         this.pathIndex = 0;
        //         return;
        //     }
        // }
    }

    private void FindFood() {
        
    }



    protected List<Vector2> GetBlocksByRadius(int radius) {
        var result = new List<Vector2>();

        // int yStart = (int)this.transform.position.y - radius;
        // yStart = yStart < 0 ? 0 : yStart;

        // int yEnd = (int)this.transform.position.y + radius;
        // yEnd = yEnd >= Creature.digitalMap.GetLength(1) ? Creature.digitalMap.GetLength(1) : yEnd;

        // int xStart = (int)this.transform.position.x - radius;
        // xStart = xStart < 0 ? 0 : xStart;

        // int xEnd = (int)this.transform.position.x + radius;
        // xEnd = xEnd >= Creature.digitalMap.GetLength(0) ? Creature.digitalMap.GetLength(0) : xEnd;

        int currentX = (int)(this.transform.position.x);
        int currentY = (int)(this.transform.position.z);
        // Debug.Log($"{currentX} : {this.transform.position.x}");
        // Debug.Log($"{currentY} : {this.transform.position.z}");
        for(int y = currentY - radius; y <= currentY + radius; y++) {
            for(int x = currentX - radius; x <= currentX + radius; x++) {
                if((x == currentX && y == currentY) ||
                    (x < 0 || x >= Creature.digitalMap.GetLength(0)) ||
                    (y < 0 || y >= Creature.digitalMap.GetLength(1)))
                    continue;

                if(Mathf.Pow(x - currentX, 2) + Mathf.Pow(y - currentY, 2) <= Mathf.Pow(this.searchRadius, 2))
                    result.Add(new Vector2(x, y));
            }
        }

        return result;
    }

    private void PaintToDefault(){ 
        for(int y = 0; y < Creature.objectMap.GetLength(1); y++) {
            for(int x = 0; x < Creature.objectMap.GetLength(0); x++) {
                Color color = new Color(0f, 0f, 0f);
                switch(Creature.digitalMap[x, y]) {
                    case 0: {
                        color = new Color(0.2f, 0.5f, 0f);
                        break;            
                    }
                    case 1: {
                        color = new Color(0f, 0f, 0.7f);
                        break;            
                    }
                }
                Creature.objectMap[x, y]
                    .GetComponent<Renderer>()
                    .materials[0]
                    .color = color;
            }
        }
    }
}