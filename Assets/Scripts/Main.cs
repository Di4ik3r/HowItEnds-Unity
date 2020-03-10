using Assets.Scripts.Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.DayNightCycle;

public class Main : MonoBehaviour
{
    private List<Creature> groundCreatures;
    private List<Creature> waterCreatures;

    private MapHolder Map = MapHolder.Instance;

    void Start()
    {
        // UnityEditor.SceneView.FocusWindowIfItsOpen(typeof(UnityEditor.SceneView));

        Creature.digitalMap = Map.DigitalMap;
        Creature.objectMap = Map.ObjectMap;

        // groundCreatures = CreateCreatures(10);//CreateCreatures(OptionsMenu.GroundCreaturesCount);
        groundCreatures = CreateCreatures(13, CreatureType.Vegetarian);
        groundCreatures.AddRange(CreateCreatures(3, CreatureType.Predatory));
        CreatureManager.Instance.AddCreatures(groundCreatures);
        // groundCreatures.AddRange(CreateCreatures(5, CreatureType.Predatory));
        //waterCreatures = CreateCreaturesInWater(OptionsMenu.WaterCreaturesCount);
        new PathFinding();
        new Grid();

        // creatures = new List<Creature>();
        // creatures.Add(new Creature(new Vector2(0, 0), 0));
        // creatures.Add(new Creature(new Vector2(0, 5), 0));
        // creatures.Add(new Creature(new Vector2(5, 0), 0));

    }
  
    void Update() {

    }

    private List<Creature> CreateCreatures(int amount, CreatureType type)
    {
        List<Creature> result = new List<Creature>();

        for (int i = 0; i < amount; i++)
        {
            var random = Random.Range(0, Map.GroundCoordinates.Count - 1);
            Vector3 pickedGroundCoordinates = Map.GroundCoordinates[random];
            Vector2 position = new Vector2(pickedGroundCoordinates.x, pickedGroundCoordinates.z);
            // Creature creature = new Creature(position, 0);
            Creature creature;
            switch(type) {
                case CreatureType.Vegetarian:
                    creature = VegetarianCreature.Create(position, 2);
                    break;
                case CreatureType.Predatory:
                    creature = PredatoryCreature.Create(position, 2);
                    break;
                default:
                    creature = VegetarianCreature.Create(position, 2);
                    break;
            }
            result.Add(creature);
        }

        return result;
    }

    private List<Creature> CreateCreaturesInWater(int amount)
    {
        List<Creature> result = new List<Creature>();

        for (int i = 0; i < amount; i++)
        {
            var random = Random.Range(0, Map.WaterCoordinates.Count - 1);
            Vector3 pickedCoordinates = Map.WaterCoordinates[random];
            Vector2 position = new Vector2(pickedCoordinates.x, pickedCoordinates.z);
            // Creature creature = new Creature(position, 0);
            var creature = WaterCreature.Create(position, 0);
            result.Add(creature);
        }

        return result;
    }

    //private void creatureCycle()
    //{
    //    foreach (Creature creature in creatures)
    //    {
    //        creature.MakeMove();
    //    }
    //}

    //public int[,] getDigitalMap()
    //{
    //    return Map.DigitalMap;
    //}

    //public GameObject[,] getObjectMap()
    //{
    //    return Map.ObjectMap;
    //}
}
