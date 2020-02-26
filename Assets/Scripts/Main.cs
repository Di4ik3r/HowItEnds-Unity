using Assets.Scripts.Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    private List<Creature> groundCreatures;
    private List<Creature> waterCreatures;

    private MapHolder Map = MapHolder.getInstance();

    void Start()
    {
        //UnityEditor.SceneView.FocusWindowIfItsOpen(typeof(UnityEditor.SceneView));

        Creature.digitalMap = Map.DigitalMap;
        Creature.objectMap = Map.ObjectMap;

        groundCreatures = CreateCreatures(Menu.GroundCreaturesCount);
        waterCreatures = CreateCreaturesInWater(Menu.WaterCreaturesCount);
        new PathFinding();

        // creatures = new List<Creature>();
        // creatures.Add(new Creature(new Vector2(0, 0), 0));
        // creatures.Add(new Creature(new Vector2(0, 5), 0));
        // creatures.Add(new Creature(new Vector2(5, 0), 0));

    }
  
    void Update() {

    }

    private List<Creature> CreateCreatures(int amount)
    {
        List<Creature> result = new List<Creature>();

        for (int i = 0; i < amount; i++)
        {
            var random = Random.Range(0, Map.GroundCoordinates.Count - 1);
            Vector3 pickedGroundCoordinates = Map.GroundCoordinates[random];
            Vector2 position = new Vector2(pickedGroundCoordinates.x, pickedGroundCoordinates.z);
            // Creature creature = new Creature(position, 0);
            var creature = GroundCreature.Create(position, 0);
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
