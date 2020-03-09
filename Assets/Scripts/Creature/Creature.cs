using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.DayNightCycle;

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
    // public static Vector2 DEATH_RANGE = new Vector2(6, 15);
    public static Vector2 DEATH_RANGE = new Vector2(2, 2);
    public int amountOfChilds;
    // Ліміт, при досягненні якого - Юніт буде шукати їжу. Крок, який буде додаватись до загального показника
    // голоду
    public float HUNGER_LIMIT = 1f, HUNGER_STEP = .112f;
    // Ліміт, при досягненні якого - Юніт буде шукати воду. Крок, який буде додаватись до загального показника
    // спраги
    // private float THIRST_LIMIT = .4f, THIRST_STEP = .06f;
    public float THIRST_LIMIT = 1f, THIRST_STEP = .126f;
    public float hungerBorder = 3.2f;
    public float thirstBorder = 3.4f;

    public float hungerMultiplier = 1f;
    public float thirstMultiplier = 1f;

    // Ідентифікатор Юніта
    public int     id;
    // Булівське значення, що відображає чи живий Юніт
    public bool    isAlive;
    // Показник голоду
    public float   hunger;
    // Показник спраги
    public float   thirst;
    // День, коли Юніт був створений
    public int     birthDay;
    // День, коли юніт помре природньою смертю (День народження + Рандом.Рендж(ДЕАД_РЕНДЖ.х, ДЕАД_РЕНДЖ.у))
    public int     deathDay;
    // Швидкість
    public float   speed;
    public float adrenalin;
    // Маса
    protected float   weight;
    
    protected int searchRadius;

    // Висота Меша. Потрібно для того щоб юніт правильно ставав на блоки по висоті
    public float meshHeight;
    public float scale;
    protected Vector2 speedLimit = new Vector2(0.6f, 2.4f);

    // Властивість, що відображає чи відчуває голод Юніт
    protected bool isHunger { get { return this.hunger >= this.HUNGER_LIMIT ? true : false; } }
    // Властивість, що відображає чи відчуває спрагу Юніт
    protected bool isThirst { get { return this.thirst >= this.THIRST_LIMIT ? true : false; } }
    

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

    public CreatureType type;
    public Vector2 foodBlock;


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
        Creature.digitalMap[(int)position.x, (int)position.y] = (obj as Creature).id;

        (obj as Creature).statsCanvas.SetActive(true);

        return obj;
    }

    public static T Create<T>(Creature parent, int birthDay) {
        GameObject prefab = Resources.Load<GameObject>($"Creature/{typeof(T).FullName}Prefab");
        GameObject newObject = Instantiate(prefab) as GameObject;
        T obj = newObject.GetComponent<T>();

        // parameters init here
        (obj as Creature).InitProperties(parent, birthDay);

        var position = new Vector2(parent.transform.position.x, parent.transform.position.z);
        // Позначення в глобальному масиві, що дана клітка зайнята
        Creature.digitalMap[(int)position.x, (int)position.y] = (obj as Creature).id;

        (obj as Creature).statsCanvas.SetActive(true);

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
        this.deathDay = birthDay + (int)Random.Range(DEATH_RANGE.x, DEATH_RANGE.y);
        this.speed = Random.Range(this.speedLimit.x, this.speedLimit.y); 
        this.adrenalin = this.speed / 100 * 20;
        this.weight = 0;

        this.movement = new GroundMovement(this);
        
        this.isAlive = true;

        
        this.searchRadius = 9;

        this.scale = 1 - this.speed.Map(this.speedLimit.x, this.speedLimit.y, 0.3f, 0.7f);
        this.amountOfChilds = (int)this.scale.Map(0.3f, 0.7f, 1, 3);


        this.transform.position = new Vector3(position.x, 
                                    Creature.objectMap[(int)position.x, (int)position.y].transform.position.y + this.scale,
                                    position.y);
                                    
        this.transform.localScale = new Vector3(this.scale, this.scale, this.scale);


        this.gameObject.GetComponent<Renderer>().materials[0].color = new Color(
            1f,
            1f,
            1f);
    }

    private void InitProperties(Creature parent, int birthDay) {
        // Ініціалізація полів
        this.id = Creature.ID_COUNTER++;
        this.hunger = 0;
        this.thirst = 0;
        this.birthDay = birthDay;
        this.deathDay = birthDay + (int)Random.Range(DEATH_RANGE.x, DEATH_RANGE.y);
        // this.deathDay = birthDay;
        // this.speed = Random.Range(this.speedLimit.x, this.speedLimit.y); 
        this.speed = parent.speed + (Random.Range(-0.2f, 0.2f));
        this.adrenalin = this.speed / 100 * 20;
        this.weight = 0;

        this.movement = new GroundMovement(this);
        
        this.isAlive = true;

        
        this.searchRadius = parent.searchRadius + ((int)Random.Range(-1, 1));

        this.scale = 1 - this.speed.Map(this.speedLimit.x, this.speedLimit.y, 0.3f, 0.7f);
        this.amountOfChilds = (int)this.scale.Map(0.3f, 0.7f, 1, 3);


        this.transform.position = parent.transform.position;
        // this.transform.position = new Vector3(parent.transform.position.x, 
        //                             Creature.objectMap[(int)parent.transform.position.x, (int)parent.transform.position.z].transform.position.y + this.scale,
        //                             parent.transform.position.z);
                                    
        this.transform.localScale = new Vector3(this.scale, this.scale, this.scale);


        this.gameObject.GetComponent<Renderer>().materials[0].color = new Color(
            1f,
            1f,
            1f);
    }


    // Логіка руху Юніта
    public virtual void MakeMove() {
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
            
            case CreatureAction.Escaping:
                if(!PredatorIsClose()) {
                    EndEscape();
                } else {
                    Escape();
                }
                break;

            case CreatureAction.Walking:
                if(PredatorIsClose()) {
                    StartEscape();
                } else if(isHunger) {
                    // if(IsNeededBlockInTouch()) {
                    // if(IsNeededBlockInTouch(Creature.digitalMap[(int)foodBlock.x, (int)foodBlock.y])) {
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

    public void Check() {
        if(this.hunger >= this.hungerBorder || this.thirst >= this.thirstBorder) {
            CreatureManager.Instance.KillCreature(this);
        }
    }

    public void Check(int day) {
        if(this.hunger >= this.hungerBorder || this.thirst >= this.thirstBorder) {
            CreatureManager.Instance.KillCreature(this);
        }
        if(this.deathDay == day) {
            NaturalDeath(day);
        }
    }

    public void NaturalDeath(int day) {
        CreatureManager.Instance.BirthCreature(this, day);
    }

    protected void Escape() {
        // Debug.Log("escaping");
        int currentX = (int)(this.transform.position.x);
        int currentY = (int)(this.transform.position.z);
        var currentPos = new Vector2(currentX, currentY);

        var blocks = GetBlocksByRadius(this.searchRadius);
        var predators = new List<Vector2>();
        foreach (var block in blocks) {
            if(CreatureManager.Instance.IsPredator(block)) {
                predators.Add(block);
            }
        }

        if(predators.Count < 1)
            return;
        predators.Sort( (a, b) => 
            PathFinding.GetDistance(currentPos, a) -
            PathFinding.GetDistance(currentPos, b) 
        );
        
        var predator = predators[0];
        var avaiableBlocks = new List<Vector2>();
        foreach (var block in blocks) {
            if(Creature.digitalMap[(int)Mathf.Floor(block.x), (int)Mathf.Floor(block.y)] == 0)
                avaiableBlocks.Add(block);
        }

        if(avaiableBlocks.Count < 1)
            return;
        avaiableBlocks.Sort( (a, b) => 
            PathFinding.GetDistance(predator, a) -
            PathFinding.GetDistance(predator, b) 
        );

        this.movement.MoveTo(avaiableBlocks[avaiableBlocks.Count - 1]);
    }

    protected void EndEscape() {
        this.action = CreatureAction.Walking;
        // Debug.Log("escaping ended");
        this.speed -= this.adrenalin;
    }

    protected void StartEscape() {
        // Debug.Log("Started Escape");
        this.action = CreatureAction.Escaping;
        this.speed += this.adrenalin;
    }

    protected bool PredatorIsClose() {
        var blocks = GetBlocksByRadius(this.searchRadius);
        var predators = new List<Vector2>();
        foreach (var block in blocks) {
            if(CreatureManager.Instance.IsPredator(block)) {
                return true;
            }
        }

        return false;
    }

    protected void StartEat() {
        this.action = CreatureAction.Eating;
        Eat();
    }
    protected virtual void Eat() {
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

    protected void FindWater() {
        if(!this.movement.PathIsExist()) {
            var waterBlock = GetBlock(1);
            if(waterBlock.x != -1)
                this.movement.MoveTo(waterBlock);
        }
    }

    protected virtual void FindFood() {
        // var foodBlock = GetBlock(2);
        // switch(this.type) {
        //     case CreatureType.Vegetarian:
                if(!this.movement.PathIsExist()) {
                    foodBlock = GetBlock(2);
                    if(foodBlock.x != -1) {
                        this.foodBlock = foodBlock;
                        this.movement.MoveTo(foodBlock);
                    }
                }
                // break;
            
            // case CreatureType.Predatory:
            //     foodBlock = GetWeakCreature();
            //     if(foodBlock.x != -1) {
            //         this.foodBlock = foodBlock;
            //         this.movement.MoveTo(foodBlock);
            //     }
            //     break;
        // }
        
    }

    protected Vector2 GetWeakCreature(int radius) {
        var weakCreatures = new List<Vector2>();
        
        int currentX = (int)(this.transform.position.x);
        int currentY = (int)(this.transform.position.z);
        var currentPos = new Vector2(currentX, currentY);
        
        for(int y = currentY - radius; y <= currentY + radius; y++) {
            for(int x = currentX - radius; x <= currentX + radius; x++) {
                if((x == currentX && y == currentY) ||
                    (x < 0 || x >= Creature.digitalMap.GetLength(0)) ||
                    (y < 0 || y >= Creature.digitalMap.GetLength(1)))
                    continue;

                if(Mathf.Pow(x - currentX, 2) + Mathf.Pow(y - currentY, 2) <= Mathf.Pow(radius, 2)) {
                    var isVegeterian = CreatureManager.Instance.IsVegeterian(Creature.digitalMap[x, y]);
                    
                    if(Creature.digitalMap[currentX, currentY] >= 50 && this.id != Creature.digitalMap[x, y]) {
                    // if(isVegeterian) {
                        var creature = CreatureManager.Instance.GetCreature(x, y);
                        if(creature != null) {
                            // if(this.IsWeaker(creature))
                                weakCreatures.Add(new Vector2(x, y));
                        }
                    }
                }
            }
        }

        weakCreatures.Sort( (a, b) => 
            PathFinding.GetDistance(currentPos, a) -
            PathFinding.GetDistance(currentPos, b) 
        );

        var searchedCreature = weakCreatures.Count == 0 ? new Vector2(-1, -1) : weakCreatures[0];
        if(searchedCreature.x == -1)
            return new Vector2(-1, -1);
        var searchedNeighbours = GetNeighboursBlocks(searchedCreature);
        searchedNeighbours.Sort( (a, b) => 
            PathFinding.GetDistance(currentPos, a) -
            PathFinding.GetDistance(currentPos, b) 
        );


        return searchedNeighbours[0];
    }

    protected Vector2 GetWeakCreature() {
        var weakCreatures = new List<Vector2>();
        
        int currentX = (int)(this.transform.position.x);
        int currentY = (int)(this.transform.position.z);
        var currentPos = new Vector2(currentX, currentY);
        
        for(int y = currentY - 1; y <= currentY + 1; y++) {
            for(int x = currentX - 1; x <= currentX + 1; x++) {
                if((x == currentX && y == currentY) ||
                    (x < 0 || x >= Creature.digitalMap.GetLength(0)) ||
                    (y < 0 || y >= Creature.digitalMap.GetLength(1)))
                    continue;

                var isVegeterian = CreatureManager.Instance.IsVegeterian(Creature.digitalMap[x, y]);
                if(Creature.digitalMap[currentX, currentY] >= 50 && this.id != Creature.digitalMap[x, y]) {
                // if(isVegeterian) {
                    var creature = CreatureManager.Instance.GetCreature(x, y);
                    if(creature != null) {
                        // if(this.IsWeaker(creature))
                        weakCreatures.Add(new Vector2(x, y));
                    }
                }
            }
        }

        if(weakCreatures.Count > 0)
            return weakCreatures[0];
        else return new Vector2(-1, -1);

        // weakCreatures.Sort( (a, b) => 
        //     PathFinding.GetDistance(currentPos, a) -
        //     PathFinding.GetDistance(currentPos, b) 
        // );

        // var searchedCreature = weakCreatures.Count == 0 ? new Vector2(-1, -1) : weakCreatures[0];
        // if(searchedCreature.x == -1)
        //     return new Vector2(-1, -1);
        // var searchedNeighbours = GetNeighboursBlocks(searchedCreature);
        // searchedNeighbours.Sort( (a, b) => 
        //     PathFinding.GetDistance(currentPos, a) -
        //     PathFinding.GetDistance(currentPos, b) 
        // );

        // Debug.Log($"lmfo: {searchedNeighbours[1]}");
        // return new Vector2[] { searchedNeighbours[0], searchedNeighbours[1] };
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

    protected bool IsNeededBlockInTouch(Vector2 block) {
        if(block.x == -1)
            return false;
        int currentX = (int)(this.transform.position.x);
        int currentY = (int)(this.transform.position.z);
        
        for(int y = currentY - 1; y <= currentY + 1; y++) {
            for(int x = currentX - 1; x <= currentX + 1; x++) {
                if((x == currentX && y == currentY) ||
                    (x < 0 || x >= Creature.digitalMap.GetLength(0)) ||
                    (y < 0 || y >= Creature.digitalMap.GetLength(1)))
                    continue;
            
                // var temp = new Vector2(x, y);

                // if(temp.x == block.x && temp.y == block.y) {
                    // Debug.Log($"{x} {y}");
                    // Debug.Log($"{Mathf.Floor(block.x)} {Mathf.Floor(block.y)}");
                if(x == Mathf.Floor(block.x) && y == Mathf.Floor(block.y)) {
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
        
        int currentX = (int)(this.transform.position.x);
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


    public bool IsWeaker(Creature creature) {
        return this.speed < creature.speed;
    }
}