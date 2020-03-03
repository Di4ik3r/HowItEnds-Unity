using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Creature : MonoBehaviour {

    // Основне поле класу, яке тримає в собі основні компоненти Юніті (такі як Трансформ) і куда можна
    // добавити нові компоненти (такі як МешРендерер, МешФільтр і тд)
    
    // Статичне поле даного класу, яке дозволяє відслідковувати і обробляти ідентифікатори для кожного
    // юніта
    public static int ID_COUNTER = 50;
    // Статичне поле даного класу, яке дозволяє тримати ІНТовський масив карти
    public static int[,] digitalMap;
    // Статичне поле даного класу, яке дозволяє тримати масив кубів карти    
    public static GameObject[,] objectMap;

    // Статичне поле даного класу, яке дозволяє визначити мінімальну і максимальну кількість днів в межах
    // якої буде обиратись скільки Юніт проживе (в днях)
    public static Vector2 DEATH_RANGE = new Vector2(6, 15);
    // Ліміт, при досягненні якого - Юніт буде шукати їжу. Крок, який буде додаватись до загального показника
    // голоду
    public float HUNGER_LIMIT = 1f, HUNGER_STEP = .112f;
    // Ліміт, при досягненні якого - Юніт буде шукати воду. Крок, який буде додаватись до загального показника
    // спраги
    // private float THIRST_LIMIT = .4f, THIRST_STEP = .06f;
    public float THIRST_LIMIT = 1f, THIRST_STEP = .126f;

    public float hungerMultiplier = 1f;
    public float thirstMultiplier = 1f;

    // Ідентифікатор Юніта
    public int     id;
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
    

    public Movement movement;
    // Ліміт максимально можливих ходів в даному напрямку
    public int MOVES_LIMIT_MIN = 3;
    public int MOVES_LIMIT_MAX = 12;

    // public static GameObject prefab;
    public CreatureAction action = CreatureAction.Walking;
    public float consumeLimit = 1f;
    public float consumeStep = 0.01f;
    public float consumeTime = 0;
    public float eatingMultiplier = 1f;
    public float drinkingMultiplier = 1f;


    ///CHANGES MADE BY ILLUHA
    ///CHANGES MADE BY ILLUHA
    [Header("Creatures stats UI")]
    public GameObject statsCanvas;
    public Image foodBar;
    public Image waterBar;

    public void RefreshStatus() {
        this.foodBar.fillAmount = 1 - this.hunger;
        this.waterBar.fillAmount = 1 - this.thirst;
    }

    void OnMouseDown()
    {
        // foodBar.fillAmount += 0.1f;
        // waterBar.fillAmount += 0.1f;
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

        
        this.searchRadius = 9;

        this.scale = 1 - this.speed.Map(this.speedLimit.x, this.speedLimit.y, 0.3f, 0.7f);

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

        
        switch (this.action) {
            case CreatureAction.Eating:
                Eat();
                break;
            
            case CreatureAction.Drinking:
                Drink();
                break;

            case CreatureAction.Walking:
                if(isHunger) {
                    if(IsNeededBlockInTouch(2)) {
                        StartEat();
                    } else {
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

    protected void StartEat() {
        this.action = CreatureAction.Eating;
        Eat();
    }
    protected void Eat() {
        consumeTime += consumeStep;

        if(consumeTime >= consumeLimit * eatingMultiplier) {
            this.consumeTime = 0;
            this.action = CreatureAction.Walking;
            this.hunger = 0;
        }
    }

    protected void StartDrink() {
        this.action = CreatureAction.Drinking;
        Drink();
    }
    protected void Drink() {
        consumeTime += consumeStep;
        if(consumeTime >= consumeLimit * drinkingMultiplier) {
            this.consumeTime = 0;
            this.action = CreatureAction.Walking;
            this.thirst = 0;
        }
    }

    private void FindWater() {
        if(!this.movement.PathIsExist()) {
            var waterBlock = GetBlock(1);
            if(waterBlock.x != -1)
                this.movement.MoveTo(waterBlock);
        }
    }

    private void FindFood() {
        if(!this.movement.PathIsExist()) {
            var foodBlock = GetBlock(2);
            if(foodBlock.x != -1)
                this.movement.MoveTo(foodBlock);
        }
    }


    protected bool IsNeededBlockInTouch(int block) {
        int currentX = (int)(this.transform.position.x);
        int currentY = (int)(this.transform.position.z);
        
        for(int y = currentY - 1; y <= currentY + 1; y++) {
            for(int x = currentX - 1; x <= currentX + 1; x++) {
                if((x == currentX && y == currentY) ||
                    (x < 0 || x >= Creature.digitalMap.GetLength(0)) ||
                    (y < 0 || y >= Creature.digitalMap.GetLength(1)))
                    continue;

                if(Creature.digitalMap[x, y] == block) {
                    return true;
                }
            }
        }

        return false;
    }
    // 0 ground
    // 1 water
    // 2 food
    // 3 decoration
    protected List<Vector2> GetBlocksByNeed(int block) {
        var result = new List<Vector2>();
        
        int currentX = (int
        )(this.transform.position.x);
        int currentY = (int)(this.transform.position.z);
        
        for(int y = currentY - 1; y <= currentY + 1; y++) {
            for(int x = currentX - 1; x <= currentX + 1; x++) {
                if((x == currentX && y == currentY) ||
                    (x < 0 || x >= Creature.digitalMap.GetLength(0)) ||
                    (y < 0 || y >= Creature.digitalMap.GetLength(1)))
                    continue;

                if(Creature.digitalMap[currentX, currentY] == block)
                    result.Add(new Vector2(x, y));
            }
        }

        return result;
    }

    protected Vector2 GetBlock(int type) {
        var blocks = GetBlocksByRadius(this.searchRadius);
        var searchedBlocks = new List<Vector2>();
        int currentX = (int)(this.transform.position.x);
        int currentY = (int)(this.transform.position.z);
        var currentPos = new Vector2(currentX, currentY);

        foreach (var block in blocks) {
            if(Creature.digitalMap[(int)block.x, (int)block.y] == type)
                searchedBlocks.Add(block);
        }

        searchedBlocks.Sort( (a, b) => 
            PathFinding.GetDistance(currentPos, a) -
            PathFinding.GetDistance(currentPos, b) 
        );

        var searchedBlock = searchedBlocks.Count == 0 ? new Vector2(-1, -1) : searchedBlocks[0];
        if(searchedBlock.x == -1)
            return new Vector2(-1, -1);
        var searchedNeighbours = GetNeighboursBlocks(searchedBlock);
        searchedNeighbours.Sort( (a, b) => 
            PathFinding.GetDistance(currentPos, a) -
            PathFinding.GetDistance(currentPos, b) 
        );


        return searchedNeighbours[0];
    }

    protected List<Vector2> GetNeighboursBlocks(Vector2 block) {
        var result = new List<Vector2>();

        for(int y = (int)block.y - 1; y <= (int)block.y + 1; y++) {
            for(int x = (int)block.x - 1; x <= (int)block.x + 1; x++) {
                if((x == block.x && y == block.y) ||
                    (x < 0 || x >= Creature.digitalMap.GetLength(0)) ||
                    (y < 0 || y >= Creature.digitalMap.GetLength(1)))
                    continue;

                result.Add(new Vector2(x, y));
            }
        }

        return result;
    }

    protected List<Vector2> GetBlocksByRadius(int radius) {
        var result = new List<Vector2>();


        int currentX = (int)(this.transform.position.x);
        int currentY = (int)(this.transform.position.z);
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
}