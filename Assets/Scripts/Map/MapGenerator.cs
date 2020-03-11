using Assets.Scripts.Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public GameObject cube;
    public Material[] materials;
    public GameObject[] decorations;
    public GameObject[] food;

    MapHolder Map = MapHolder.Instance;

    void Awake()
    {
        Map.Cube = cube;
        Map.Decorations = decorations;
        Map.Materials = materials;
        Map.Food = food;
        GenerateMap();
    }

    // private void OnDrawGizmos() {
    //     Gizmos.color = new Color(1, 0, 0, 0.5f);
    //     for(var i = 0; i < Creature.digitalMap.GetLength(0); i++) {
    //         for(var j = 0; j < Creature.digitalMap.GetLength(1); j++) {
    //             if(Creature.digitalMap[i, j] == 0)
    //                 Gizmos.DrawCube(new Vector3(i, 10, j), Vector3.one);
    //         }
    //     }
    // }

    public void GenerateMap()
    {
        Map.Cube = cube;
        Map.Decorations = decorations;
        Map.Materials = materials;
        Map.Food = food;

        string holderName = "Field";
        if (transform.Find(holderName))
        {
            DestroyImmediate(transform.Find(holderName).gameObject);
        }

        Transform fieldPlatform = new GameObject(holderName).transform;
        fieldPlatform.parent = transform;

        holderName = "Decorations";
        if (transform.Find(holderName))
        {
            DestroyImmediate(transform.Find(holderName).gameObject);
        }

        Transform decorationsPlatform = new GameObject(holderName).transform;
        decorationsPlatform.parent = transform;

        holderName = "Food";
        if (transform.Find(holderName))
        {
            DestroyImmediate(transform.Find(holderName).gameObject);
        }

        Transform foodPlatfrom = new GameObject(holderName).transform;
        foodPlatfrom.parent = transform;

        Map.BuildMap(Instantiate, fieldPlatform, decorationsPlatform, foodPlatfrom);
        
        Creature.digitalMap = Map.DigitalMap;
        Creature.objectMap = Map.ObjectMap;
    }
}
