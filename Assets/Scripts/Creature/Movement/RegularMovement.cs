using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public enum Direction {
    UP,
    RIGHT,
    DOWN,
    LEFT,
    Z_UNAVAILABLE
}


public class RegularMovement : IMovement {

    // Напрямок в якому буде рухатись Юніт
    private Direction moveDirection;
    // Список кліток даного напрямку
    private List<Vector2> directionChoices;
    // Статичний масив можливих напрямків
    private static Direction[] DIRECTIONS = {Direction.UP, Direction.RIGHT, Direction.DOWN, Direction.LEFT};

    protected int pathIndex;
    private Stack pathToCell;
    private Vector2 lookingForCell;
    public bool isMoving;

    // Випадквий ліміт ходів в даному напрямку та кількість вже зроблених кроків в даному напрямку
    private int limitMoveCount, currentMoveCount;


    // Змінні які потрібні для анімованого руху
    public float moveTime;
    public float moveArcHeight = 1f;
    public Vector3 moveStartPosition;
    public Vector3 moveTargetPosition;


    protected Creature creature;
    public RegularMovement(Creature creature) {
        this.creature = creature;

        this.isMoving = false;
        
        this.lookingForCell = new Vector2(
            creature.transform.position.x,
            creature.transform.position.y
        );
        
        this.pathToCell = new Stack();

        // this.RandomizeMove();
    }

    public void Jump() {
        if(this.isMoving)
            AnimateMoving();
        else {
            StartMoving();
        }
    }

    void AnimateMoving () {
        moveTime = Mathf.Min (1, moveTime + Time.deltaTime * this.creature.speed);
        float height = (1 - 4 * (moveTime - .5f) * (moveTime - .5f)) * moveArcHeight;
        this.creature.transform.position = Vector3.Lerp (moveStartPosition, moveTargetPosition, moveTime) + Vector3.up * height;

        if (moveTime >= 1) {
            moveTime = 0;
            this.isMoving = false;
        }
    }

    public void StartMoving() {
        this.moveStartPosition = this.creature.transform.position;
        // this.moveTargetPosition = MoveLogic();
        // if(this.lookingForCell != null) {
        // if(this.isStanding) {
        if(this.pathIndex < this.pathToCell.Count) {
            Debug.Log("looking for cell");
            // this.lookingForCell = this.pathToCell[pathIndex++];
            this.lookingForCell = (Vector2)this.pathToCell.Pop();
            this.moveTargetPosition = MoveLogic((Vector2)this.lookingForCell);
        } else {
            this.moveTargetPosition = MoveLogic();
        }
        this.isMoving = true;

        // При кожному ході - збільшення показників відчуття голоду та спраги
        this.creature.hunger += this.creature.HUNGER_STEP;
        this.creature.thirst += this.creature.THIRST_STEP;

        // Debug.Log($"Hunger: {this.creature.hunger}; Thirst: {this.creature.thirst}");
    }


    // Логіка вибору клітки, на яку ступить Юніт
    private Vector3 MoveLogic() {
        // Якщо кількість кроків на даний напрямок не вичерпана або на даниий напрямок немає
        // доступних кліток
        if(this.currentMoveCount >= this.limitMoveCount || NoAvailableChioces()) {
            // перевибираємо напрямок
            RandomizeMove();
        } 
        // Інакше, перевибираємо клітки даного напрямку
        else RandomizeChioces();

        // Оголошуємо змінні, яка повернемо і запамятовуємо поточну позицію
        float   x = this.creature.transform.position.x,
                y = this.creature.transform.position.y,
                z = this.creature.transform.position.z;
        // Занульовуємо поточну клітку, щоб залишити її вільною для ходу, т.я. ми вибиремо нову
        Creature.digitalMap[(int)x, (int)z] = 0;

        // Випадково вибираємо клікту з можливих
        int chioce = Random.Range(0, directionChoices.Count);
        // Якщо немає доступних кліток
        if(directionChoices.Count > 0) {
            // Обираємо поточну і запамятовуємо її
            x = directionChoices[chioce].x;
            z = directionChoices[chioce].y;
            // y = Creature.objectMap[(int)x, (int)z].transform.position.y + this.creature.meshHeight;
            y = Creature.objectMap[(int)x, (int)z].transform.position.y + this.creature.meshHeight - ((1 - this.creature.scale) / 2);
        }

        // Позначаємо нашу позицію в глобальному масиві, щоб ніхто не зміг стати на цю позицію
        // т.я. вона вже занята
        Creature.digitalMap[(int)x, (int)z] = 9;
        // Збільшуємо індексер кроків в даному напрямку
        this.currentMoveCount++;

        // Формуємо результат
        Vector3 result = new Vector3(x, y, z);

        // Повертаємо результат
        return result;
    }

    private Vector3 MoveLogic(Vector2 to) {
        // Оголошуємо змінні, яка повернемо і запамятовуємо поточну позицію
        float   x = this.creature.transform.position.x,
                y = this.creature.transform.position.y,
                z = this.creature.transform.position.z;
        // Занульовуємо поточну клітку, щоб залишити її вільною для ходу, т.я. ми вибиремо нову
        Creature.digitalMap[(int)x, (int)z] = 0;

        // Випадково вибираємо клікту з можливих
        int chioce = Random.Range(0, directionChoices.Count);
        // Якщо немає доступних кліток
        x = to.x;
        z = to.y;
        y = Creature.objectMap[(int)x, (int)z].transform.position.y + this.creature.meshHeight - ((1 - this.creature.scale) / 2);

        // Позначаємо нашу позицію в глобальному масиві, щоб ніхто не зміг стати на цю позицію
        // т.я. вона вже занята
        Creature.digitalMap[(int)x, (int)z] = 9;
        // Збільшуємо індексер кроків в даному напрямку
        this.currentMoveCount++;

        // Формуємо результат
        Vector3 result = new Vector3(x, y, z);

        // Повертаємо результат
        return result;
    }

    // Перевірка на відсутність доступих кліток даного напрямку
    private bool NoAvailableChioces() {
        if(GetAvailableCellsIndexesByDirection().Count < 1)
            return true;
        return false;
    }

    // Перевірка клітки, на існування: чи індекси відповідають умовам та типу блока
    private bool CheckCell(Vector2 cell, int type) {
        // Якщо індекс Х знаходиться поза межами масиву
        if(cell.x >= 0 && cell.x < Creature.digitalMap.GetLength(0)) {
            // Якщо індекс У знаходиться поза межами масиву
            if(cell.y >= 0 && cell.y < Creature.digitalMap.GetLength(1))
                // Якщо клітка, на якому ми хочемо обрати - відповідає заданому типу
                if(Creature.digitalMap[(int)cell.x, (int)cell.y] == type)
                    return true;
        }
        return false;
    }

    // Випадково задає клітки даного напрямку
    private void RandomizeChioces() {
        this.directionChoices = GetAvailableCellsIndexesByDirection();
    }

    // Випадково обирає напрямок і задаємо змінним небохідні нам значення
    public void RandomizeMove() {
        // Вибір можливого випадкового напрямку
        this.moveDirection = GetRandomAvailableDirection();
        
        // Якщо немає доступних напрямків - виводимо про це інформацію
        if(this.moveDirection == Direction.Z_UNAVAILABLE) {
            Debug.Log("UNAVAILABLE");
        }

        // Випадкове задання ліміту кроків в даному напрямку
        this.limitMoveCount = Random.Range(this.creature.MOVES_LIMIT_MIN, this.creature.MOVES_LIMIT_MAX);
        // Занулення кількості вже зроблених кроків
        this.currentMoveCount = 0;

        // Вибірка списку доступних кроків по даному напрямку
        this.directionChoices = GetAvailableCellsIndexesByDirection();
    }
    
    // Формування списка доступних кліток за даним напрямком
    private List<Vector2> GetAvailableCellsIndexesByDirection() {
        // Формуємо список рузультатів
        List<Vector2> result = new List<Vector2>();
        // Стягуємо можливі клітки по даному напрямку
        List<Vector2> choices = GetCellsIndexesByDirection();

        // Пробігаємось по всім можливим кліткам
        foreach(Vector2 chioce in choices) {
            // Якщо клітка відповідає виомгам
            if(CheckCell(chioce, 0)) {
                // Додаємо її в список
                result.Add(chioce);
            }
        }

        return result;
    }

    // Формуємання списка доступних кліток за заданим намрямком
    private List<Vector2> GetAvailableCellsIndexesByDirection(Direction direction) {
        // Формуємо список рузультатів
        List<Vector2> result = new List<Vector2>();
        // Стягуємо можливі клітки по заданому напрямку
        List<Vector2> choices = GetCellsIndexesByDirection(direction);

        // Пробігаємось по всім можливим кліткам
        foreach(Vector2 chioce in choices) {
            // Якщо клітка відповідає виомгам
            if(CheckCell(chioce, 0)) {
                // Додаємо її в список
                result.Add(chioce);
            }
        }

        return result;
    }

    // Формування списка індексів за даним напрямком
    // Правила обирання кліток, за даним напрямком
    private List<Vector2> GetCellsIndexesByDirection() {
        List<Vector2> result = new List<Vector2>();

        switch(this.moveDirection) {
            case Direction.UP: {
            // Лівий верхній кут
                result.Add(new Vector2(this.creature.transform.position.x - 1,   this.creature.transform.position.z + 1));
            // Середньо-верхня клітка
                result.Add(new Vector2(this.creature.transform.position.x,       this.creature.transform.position.z + 1));
            // Правий верхній кут
                result.Add(new Vector2(this.creature.transform.position.x + 1,   this.creature.transform.position.z + 1));
                break;
            }
            case Direction.RIGHT: {
            // Правий верхній кут
                result.Add(new Vector2(this.creature.transform.position.x + 1,   this.creature.transform.position.z + 1));
            // Право-середня клітка
                result.Add(new Vector2(this.creature.transform.position.x + 1,   this.creature.transform.position.z));
            // Правий нижній кут
                result.Add(new Vector2(this.creature.transform.position.x + 1,   this.creature.transform.position.z - 1));
                break;
            }
            case Direction.DOWN: {
            // Правий нижній кут
                result.Add(new Vector2(this.creature.transform.position.x + 1,   this.creature.transform.position.z - 1));
            // Середньо-нижня клітка
                result.Add(new Vector2(this.creature.transform.position.x,       this.creature.transform.position.z - 1));
            // Лівий нижній кут
                result.Add(new Vector2(this.creature.transform.position.x - 1,   this.creature.transform.position.z - 1));
                break;
            }    
            case Direction.LEFT: {
            // Лівий нижній кут
                result.Add(new Vector2(this.creature.transform.position.x - 1,   this.creature.transform.position.z - 1));
            // Ліво-центральна клітка
                result.Add(new Vector2(this.creature.transform.position.x - 1,   this.creature.transform.position.z));
            // Лівий верхній кут
                result.Add(new Vector2(this.creature.transform.position.x - 1,   this.creature.transform.position.z + 1));
                break;
            }

        }
        return result;
    }

    // Формування списка індексів за заданим напрямком
    // Правила обирання кліток, за заданим напрямком
    private List<Vector2> GetCellsIndexesByDirection(Direction direction) {
        List<Vector2> result = new List<Vector2>();

        switch(direction) {
            case Direction.UP: {
            // Лівий верхній кут
                result.Add(new Vector2(this.creature.transform.position.x - 1,   this.creature.transform.position.z + 1));
            // Середньо-верхня клітка
                result.Add(new Vector2(this.creature.transform.position.x,       this.creature.transform.position.z + 1));
            // Правий верхній кут
                result.Add(new Vector2(this.creature.transform.position.x + 1,   this.creature.transform.position.z + 1));
                break;
            }
            case Direction.RIGHT: {
            // Правий верхній кут
                result.Add(new Vector2(this.creature.transform.position.x + 1,   this.creature.transform.position.z + 1));
            // Право-середня клітка
                result.Add(new Vector2(this.creature.transform.position.x + 1,   this.creature.transform.position.z));
            // Правий нижній кут
                result.Add(new Vector2(this.creature.transform.position.x + 1,   this.creature.transform.position.z - 1));
                break;
            }
            case Direction.DOWN: {
            // Правий нижній кут
                result.Add(new Vector2(this.creature.transform.position.x + 1,   this.creature.transform.position.z - 1));
            // Середньо-нижня клітка
                result.Add(new Vector2(this.creature.transform.position.x,       this.creature.transform.position.z - 1));
            // Лівий нижній кут
                result.Add(new Vector2(this.creature.transform.position.x - 1,   this.creature.transform.position.z - 1));
                break;
            }    
            case Direction.LEFT: {
            // Лівий нижній кут
                result.Add(new Vector2(this.creature.transform.position.x - 1,   this.creature.transform.position.z - 1));
            // Ліво-центральна клітка
                result.Add(new Vector2(this.creature.transform.position.x - 1,   this.creature.transform.position.z));
            // Лівий верхній кут
                result.Add(new Vector2(this.creature.transform.position.x - 1,   this.creature.transform.position.z + 1));
                break;
            }

        }
        return result;
    }

    // Вибір випадкового доступного напрямку
    private Direction GetRandomAvailableDirection() {
        List<Direction> availableDirections = new List<Direction>();
        for(int i = 0; i < 4; i++) {
            Direction direction = DIRECTIONS[i];
            if(GetAvailableCellsIndexesByDirection(direction).Count > 0)
                availableDirections.Add(direction);
        }
        if(availableDirections.Count < 1)
            return Direction.Z_UNAVAILABLE;
        return availableDirections[Random.Range(0, availableDirections.Count)];
    }

    // Вибір випадкового доступного напрямку (застаріла версія)
    private Direction GetRandomAvailableDirection_deprecated() {
        for(int i = 0; i < 15; i++) {
            Direction direction = GetRandomDirection();
            List<Vector2> chioces = GetCellsIndexesByDirection(direction);
            foreach(Vector2 chioce in chioces) {
                if(CheckCell(chioce, 0))
                    return direction;
            }
        }
        return Direction.Z_UNAVAILABLE;
    }

    // Вибір випадкового напрямку
    private Direction GetRandomDirection() {
        return DIRECTIONS[Random.Range(0, DIRECTIONS.Length)];
    }

    // Повертає список індексів доступних кліток, навколо Юніта
    private List<Vector2> GetAvailableCellsIndexes(int r, int type) {
        List<Vector2> result = new List<Vector2>();

        for(int x = (int)this.creature.transform.position.x - r; x <= this.creature.transform.position.x + r; x++) {
            if(x >= 0 && x < Creature.digitalMap.GetLength(0))
                for(int z = (int)this.creature.transform.position.z - r; z <= this.creature.transform.position.z + r; z++) {
                    if(x == (int)this.creature.transform.position.x && z == (int)this.creature.transform.position.z)
                        continue;
                    if(z >= 0 && z < Creature.digitalMap.GetLength(1))
                        if(Creature.digitalMap[x, z] == type)
                            result.Add(new Vector2(x, z));
                }
        }

        return result;
    }

}
