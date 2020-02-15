using System.Collections;
using System.Collections.Generic;
using UnityEngine;




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
    public float HUNGER_LIMIT = .6f, HUNGER_STEP = .0008f;
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
    // Відображає чи Юніт рухається
    // Клітка мапи, до якої прямує Юніт, якщо він відчуває голод або спрагу ((-1, -1), якщо ні до чого не прямує)
    // private bool isStanding {
    //     get { 
    //         return lookingForCell.x == position.x ?
    //             lookingForCell.y == position.y ? 
    //                 true : false :
    //             false;
    //     }
    // }
    
    protected int searchRadius;

    // Висота Меша. Потрібно для того щоб юніт правильно ставав на блоки по висоті
    public float meshHeight;
    public float scale;
    private Vector2 speedLimit = new Vector2(0.6f, 2.4f);

    // Властивість, що відображає чи відчуває голод Юніт
    private bool isHunger { get { return this.hunger >= this.HUNGER_LIMIT ? true : false; } }
    // Властивість, що відображає чи відчуває спрагу Юніт
    private bool isThirst { get { return this.thirst >= this.THIRST_LIMIT ? true : false; } }
    

    protected IMovement movement;
    // Ліміт максимально можливих ходів в даному напрямку
    public int MOVES_LIMIT_MIN = 3;
    public int MOVES_LIMIT_MAX = 12;

    public static GameObject prefab;

    public static Creature Create(Vector2 position, int birthDay) {
        if(Creature.prefab == null) {
            Creature.prefab = Resources.Load<GameObject>("Creature/CreaturePrefab");
        }
        GameObject newObject = Instantiate(Creature.prefab) as GameObject;
        Creature obj = newObject.GetComponent<Creature>();

        // parameters init here
        obj.InitProperties(position, birthDay);
        
        // Позначення в глобальному масиві, що дана клітка зайнята
        Creature.digitalMap[(int)position.x, (int)position.y] = 9;
        // Вибір випадкового напрямку, кліток
        // ((RegularMovement)obj.movement).RandomizeMove();

        return obj;
    }

    public void Update() {
        this.MakeMove();
    }

    /* public Creature(Vector2 position, int birthDay) {
        // За допомогою стягнутого класу ПрімітівХелпер - стягує меш з куба
        this.mesh = PrimitiveHelper.GetPrimitiveMesh(PrimitiveType.Cube);
        // Фукнція, що додає до геймОбджекта потрібні нам компоненти і правильно інціалізує їх
        ImplementComponents();

        InitProperties(position, birthDay);
        
        // Позначення в глобальному масиві, що дана клітка зайнята
        Creature.digitalMap[(int)position.x, (int)position.y] = 9;
        // Вибір випадкового напрямку, кліток
        RandomizeMove();
    } */
    // Додає до геймОбджекта потрібні нам компоненти і правильно інціалізує їх
    // private void ImplementComponents() {
    //     // Створює екземпляр ГеймОбджекта
    //     this.gameObject = new GameObject("Creature");

    //     // Додає компонент МешФільтр, який тримає в собі меш
    //     this.gameObject.AddComponent<MeshFilter>();
    //     // Додає компонент МешРендерер, який тримає в собі параметри, потрібні для відображення
    //     // такі, як матеріал, колір, параметри освітлення і тд.
    //     this.gameObject.AddComponent<MeshRenderer>();
        
    //     // Присвоюю мешу компонента МешФільтра меш Юніта
    //     this.gameObject.GetComponent<MeshFilter>().mesh = this.mesh;
    //     // Присвою матеріал компоненту Рендерер
    //     this.gameObject.GetComponent<Renderer>().materials[0] = Resources.Load("BasicMaterial", typeof(Material)) as Material;
    //     // this.gameObject.GetComponent<Renderer>().materials[0].color = Color.black;
    // }

    private void InitProperties(Vector2 position, int birthDay) {
        // Ініціалізація полів
        this.id = Creature.ID_COUNTER++;
        this.hunger = 0;
        this.thirst = 0;
        this.birthDay = birthDay;
        this.deathDay = (int)Random.Range(DEATH_RANGE.x, DEATH_RANGE.y);
        // this.speed = Random.Range(0.7f, 3); 
        this.speed = Random.Range(this.speedLimit.x, this.speedLimit.y); 
        this.weight = 0;

        this.movement = new RegularMovement(this);
        
        this.isAlive = true;

        
        this.searchRadius = 3;

        this.meshHeight = 1;

        this.transform.position = new Vector3(position.x, 
                                    Creature.objectMap[(int)position.x, (int)position.y].transform.position.y + this.meshHeight,
                                    position.y);
                                    
        this.scale = 1 - this.speed.Map(this.speedLimit.x, this.speedLimit.y, 0.3f, 0.7f);
        // Debug.Log($"{this.speed} = {this.scale}");
        this.transform.localScale = new Vector3(this.scale, this.scale, this.scale);


        this.gameObject.GetComponent<Renderer>().materials[0].color = new Color(
            this.speed.Map(this.speedLimit.x, this.speedLimit.y, 0.1f, 0.6f),
            0f,
            0f);
    }


    // Логіка руху Юніта
    public void MakeMove() {
        // Якщо мертвий - нічого не робимо
        if(!this.isAlive)
            return;

        // Якщо Юніт рухається
        // if(this.pathIndex < this.pathToCell.Count) {
        //     if(this.pathIndex >= this.pathToCell.Count) {
        //         this.pathToCell.Clear();
        //         this.pathIndex = 0;
        //     } else {
        //         this.lookingForCell = this.pathToCell[pathIndex++];
        //         // Debug.Log("my tyt kyezyayem");
        //     }
        // } else {

        // Якщо голодний - шукаємо їжу
        if(isHunger) {
            FindFood();
            // andrii;
        }
        // Якщо Юніт відчуває спрагу - шукаємо воду
        if(isThirst 
            // && 
            // Creature.digitalMap[
            //     (int)lookingForCell.x,
            //     (int)lookingForCell.y
            // ] != 1
        ) {
            FindWater();

        }

        // }
        // Виконуємо рух
        this.movement.Jump();
    }

    // Рух, при виконані якого - збільшується голод та спрага
    

    

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