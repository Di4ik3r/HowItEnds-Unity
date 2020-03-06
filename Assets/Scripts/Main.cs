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
        // UnityEditor.SceneView.FocusWindowIfItsOpen(typeof(UnityEditor.SceneView));

        Creature.digitalMap = Map.DigitalMap;
        Creature.objectMap = Map.ObjectMap;

        groundCreatures = CreateCreatures(10);//CreateCreatures(OptionsMenu.GroundCreaturesCount);
        //waterCreatures = CreateCreaturesInWater(OptionsMenu.WaterCreaturesCount);
        new PathFinding();
        new Grid();
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
            creature.transform.parent = transform;
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
}
