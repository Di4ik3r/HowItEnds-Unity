using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.DayNightCycle;

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

    public void CheckDeath(int day) {
        for(var i = 0; i < creatures.Count; i++) {
            creatures[i].Check(day);
        }
        // foreach (var creature in creatures) {
        //     creature.Check(day);
        // }
    }

    public void BirthCreature(Creature parent, int day) {
        Creature creature;
        var position = new Vector2(parent.transform.position.x, parent.transform.position.z);

        for(var i = 0; i < parent.amountOfChilds; i++) {
            switch(parent.type) {
                case CreatureType.Vegetarian:
                    creature = VegetarianCreature.Create(parent, day);
                    break;

                case CreatureType.Predatory:
                    creature = PredatoryCreature.Create(parent, day);
                    break;
            }
        }

        KillCreature(parent);
    }

    public bool IsPredator(Vector2 coord) {
        var creature = GetCreature(coord);
        if(creature != null) {
            if(creature.type == CreatureType.Predatory)            
                return true;
        }

        return false;
    }

    public void Borrow(Creature creature) {
        creature.movement.Borrow();
        creatures.Remove(creature);
    }

    public void KillCreature(Creature creature) {
        creature.isAlive = false;
        Borrow(creature);
    }

    public void KillCreature(Vector2 coord) {
        var creature = GetCreature(coord);
        if(creature != null) {
            Debug.Log($"Killed {coord}");
            creature.isAlive = false;
        } else {
            Debug.Log($"Not killed {coord}");
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