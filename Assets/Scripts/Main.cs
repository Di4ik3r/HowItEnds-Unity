using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    private List<Creature> creatures;

    void Start()
    {        
        //UnityEditor.SceneView.FocusWindowIfItsOpen(typeof(UnityEditor.SceneView));
        //Creature.digitalMap = this.map.digitalMap;
        //Creature.objectMap = this.map.objectMap;

        // creatures = CreateCreatures(10);
        // creatures = CreateCreatures(5);
        // creatures = CreateCreaturesInWater(5);

        // creatures = new List<Creature>();
        // creatures.Add(new Creature(new Vector2(0, 0), 0));
        // creatures.Add(new Creature(new Vector2(0, 5), 0));
        // creatures.Add(new Creature(new Vector2(5, 0), 0));

        //new PathFinding();
    }
  
    void Update() {

    }

    //private List<Creature> CreateCreatures(int amount)
    //{
    //    List<Creature> result = new List<Creature>();

    //    for (int i = 0; i < amount; i++)
    //    {
    //        var random = Random.Range(0, groundCoordinates.Count - 1);
    //        Vector3 pickedGroundCoordinates = groundCoordinates[random];
    //        Vector2 position = new Vector2(pickedGroundCoordinates.x, pickedGroundCoordinates.z);
    //        // Creature creature = new Creature(position, 0);
    //        var creature = GroundCreature.Create(position, 0);
    //        result.Add(creature);
    //    }

    //    return result;
    //}

    //private List<Creature> CreateCreaturesInWater(int amount) {
    //    List<Creature> result = new List<Creature>();

    //    for(int i = 0; i < amount; i++) {
    //        var random = Random.Range(0, waterCoordinates.Count-1);
    //        Vector3 pickedCoordinates = waterCoordinates[random];
    //        Vector2 position = new Vector2(pickedCoordinates.x, pickedCoordinates.z);
    //        // Creature creature = new Creature(position, 0);
    //        var creature = WaterCreature.Create(position, 0);
    //        result.Add(creature);
    //    }

    //    return result;
    //}

    //private void creatureCycle() {
    //    foreach(Creature creature in creatures) {
    //        creature.MakeMove();
    //    }
    //}

    //public int[,] getDigitalMap(){ 
    //    return this.map.digitalMap;
    //}

    //public GameObject[,] getObjectMap() {
    //    return this.map.objectMap;
    //}
}
