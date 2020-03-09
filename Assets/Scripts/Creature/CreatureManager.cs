using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureManager {

    private static CreatureManager instance;
    public static CreatureManager Instance {
        get { 
            if(instance == null) {
                instance = new CreatureManager();
            }
            return instance;
        }
        // set { }
    }

    private List<Creature> creatures;
    public List<Creature> Creatures {
        get { return this.creatures; }
        set { this.creatures = value; }
    }

    private CreatureManager() {
        creatures = new List<Creature>();
    }

    public void Borrow(Vector2 coord) {
        var creature = GetCreature(coord);
        if(creature != null) {
            creature.movement.Borrow();
        }
    }

    public void KillCreature(Vector2 coord) {
        var creature = GetCreature(coord);
        Debug.Log($"gonna kill {coord} - {Creature.digitalMap[(int)coord.x, (int)coord.y]}");
        if(creature != null) {
            Debug.Log("killed");
            creature.isAlive = false;
        }
    }

    public void KillCreature(int id) {
        foreach (var creature in creatures) {
            if(creature.id == id) {
                creature.isAlive = false;
            }
        }
    }

    public bool IsVegeterian(int id) {
        foreach (var creature in creatures) {
            if(creature.id == id) {
                if(creature.type == CreatureType.Vegetarian) {
                    return true;
                }
            }
        }
        return false;
    }

    public void AddCreatures(List<Creature> creatures) {
        foreach(var creature in creatures) {
            this.creatures.Add(creature);
        }
    }

    public Creature GetCreature(int id) {
        foreach (var creature in creatures) {
            if(creature.id == id)
                return creature;
        }

        return null;
    }

    public Creature GetCreature(Vector2 coord) {
        foreach (var creature in creatures) {
            // if(creature.id == Creature.digitalMap[(int)coord.x, (int)coord.y])
            if(creature.id == Creature.digitalMap[(int)Mathf.Floor(coord.x), (int)Mathf.Floor(coord.y)])
                return creature;
        }

        return null;
    }

    public Creature GetCreature(int x, int y) {
        foreach (var creature in creatures) {
            if(creature.id == Creature.digitalMap[x, y])
                return creature;
        }

        return null;
    }

}