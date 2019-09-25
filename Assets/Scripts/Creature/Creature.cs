using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature {

    // Основне поле класу, яке тримає в собі основні компоненти Юніті (такі як Трансформ) і куда можна
    // добавити нові компоненти (такі як МешРендерер, МешФільтр і тд)
    private GameObject gameObject;
    // Властивість, яка дозволяє звертатись до Трансформ геймОбджекта даного класу
    public Transform transform { get{ return this.gameObject.transform; } }
    // Властивість, яка дозволяє звертатись до Позиції геймОбджекта даного класу
    public Vector3 position { get{ return this.gameObject.transform.position; }  set { this.gameObject.transform.position = value; } }

    // Статичне поле даного класу, яке дозволяє відслідковувати і обробляти ідентифікатори для кожного
    // юніта
    public static int ID_COUNTER = 0;
    // Статичне поле даного класу, яке дозволяє тримати ІНТовський масив карти
    public static int[,] digitalMap;
    // Статичне поле даного класу, яке дозволяє тримати масив кубів карти    
    public static GameObject[,] objectMap;

    // Статичне поле даного класу, яке дозволяє визначити мінімальну і максимальну кількість днів в межах
    // якої буде обиратись скільки Юніт проживе (в днях)
    private static Vector2 DEATH_RANGE = new Vector2(6, 15);
    // Ліміт, при досягненні якого - Юніт буде шукати їжу. Крок, який буде додаватись до загального показника
    // голоду
    private static float HUNGER_LIMIT = .6f, HUNGER_STEP = .08f;
    // Ліміт, при досягненні якого - Юніт буде шукати воду. Крок, який буде додаватись до загального показника
    // спраги
    private static float THIRST_LIMIT = .4f, THIRST_STEP = .06f;

    // Ідентифікатор Юніта
    private int     id;
    // Булівське значення, що відображає чи живий Юніт
    private bool    isAlive;
    // Показник голоду
    private float   hunger;
    // Показник спраги
    private float   thirst;
    // День, коли Юніт був створений
    private int     birthDay;
    // День, коли юніт помре природньою смертю (День народження + Рандом.Рендж(ДЕАД_РЕНДЖ.х, ДЕАД_РЕНДЖ.у))
    private int     deathDay;
    // Швидкість
    private float   speed;
    // Маса
    private float   weight;
    // Відображає чи Юніт рухається
    private bool    isMoving;
    // Клітка мапи, до якої прямує Юніт, якщо він відчуває голод або спрагу ((-1, -1), якщо ні до чого не прямує)
    private Vector2 lookingForCell;

    // Меш для Юніта (його завнішній вигляд)
    private Mesh mesh;
    // Висота Меша. Потрібно для того щоб юніт правильно ставав на блоки по висоті
    private float meshHeight;

    // Властивість, що відображає чи відчуває голод Юніт
    private bool isHunger { get { return this.hunger >= Creature.HUNGER_LIMIT ? true : false; } }
    // Властивість, що відображає чи відчуває спрагу Юніт
    private bool isThirst { get { return this.thirst >= Creature.THIRST_LIMIT ? true : false; } }

    public Creature(Vector2 position, int birthDay) {
        // За допомогою стягнутого класу ПрімітівХелпер - стягує меш з куба
        this.mesh = PrimitiveHelper.GetPrimitiveMesh(PrimitiveType.Cube);
        // Фукнція, що додає до геймОбджекта потрібні нам компоненти і правильно інціалізує їх
        ImplementComponents();

        // Ініціалізація полів
        this.id = Creature.ID_COUNTER++;
        this.hunger = 0;
        this.thirst = 0;
        this.birthDay = birthDay;
        this.deathDay = (int)Random.Range(DEATH_RANGE.x, DEATH_RANGE.y);
        this.speed = 0;
        this.weight = 0;
        this.isMoving = false;
        // this.isLookingFor = false;
        this.lookingForCell = new Vector2(position.x, position.y);
        this.isAlive = true;

        this.meshHeight = 1;

        this.position = new Vector3(position.x, 
                                    Creature.objectMap[(int)position.x, (int)position.y].transform.position.y + this.meshHeight,
                                    position.y);
    }

    // Додає до геймОбджекта потрібні нам компоненти і правильно інціалізує їх
    private void ImplementComponents() {
        // Створює екземпляр ГеймОбджекта
        this.gameObject = new GameObject("Creature");

        // Додає компонент МешФільтр, який тримає в собі меш
        this.gameObject.AddComponent<MeshFilter>();
        // Додає компонент МешРендерер, який тримає в собі параметри, потрібні для відображення
        // такі, як матеріал, колір, параметри освітлення і тд.
        this.gameObject.AddComponent<MeshRenderer>();
        
        // Присвоюю мешу компонента МешФільтра меш Юніта
        this.gameObject.GetComponent<MeshFilter>().mesh = this.mesh;
        // Присвою матеріал компоненту Рендерер
        this.gameObject.GetComponent<Renderer>().materials[0] = Resources.Load("BasicMaterial", typeof(Material)) as Material;
    }


    // Логіка руху Юніта
    public void MakeMove() {
        // Якщо мертвий - нічого не робимо
        if(!this.isAlive)
            return;

        // Якщо Юніт рухається
        if(this.isMoving) {
            
        }
        
        // Якщо голодний - шукаємо їжу
        if(isHunger)
            FindFood();
        // Якщо Юніт відчуває спрагу - шукаємо воду
        if(isThirst)
            FindWater();

        // Виконуємо рух
        Jump();
    }

    // Рух, при виконані якого - збільшується голод та спрага
    private void Jump() {
        this.position = MoveLogic();

        this.hunger += HUNGER_STEP;
        this.thirst += THIRST_STEP;
    }

    // Логіка вибору клітка, на яку ступить Юніт
    private Vector3 MoveLogic() {
        float   x = this.position.x,
                y = this.position.y,
                z = this.position.z;


        Creature.digitalMap[(int)x, (int)z] = 0;
        
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

        bool available = Creature.digitalMap[(int)provedChoices[choice].x, (int)provedChoices[choice].y] == 0;

        while(!available) {
            choice = Random.Range(0, provedChoices.Count - 1);
            available = Creature.digitalMap[(int)provedChoices[choice].x, (int)provedChoices[choice].y] == 0;
        }

        x = provedChoices[choice].x;
        z = provedChoices[choice].y;

        y = Creature.objectMap[(int)x, (int)z].transform.position.y + this.meshHeight;

        Creature.digitalMap[(int)x, (int)z] = 9;

        result = new Vector3(x, y, z);

        return result;
    }

    private void FindWater() {

    }

    private void FindFood() {

    }
}
