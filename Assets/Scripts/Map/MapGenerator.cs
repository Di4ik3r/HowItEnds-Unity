using Assets.Scripts.Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public GameObject cube;
    public Material[] materials;
    public GameObject[] decorations;

    MapHolder Map = MapHolder.getInstance();

    void Awake()
    {
        Map.Cube = cube;
        Map.Decorations = decorations;
        Map.Materials = materials;
    }

    void Start()
    {        
        GenerateMap();
    }

    public void GenerateMap()
    {
        Map.Cube = cube;
        Map.Decorations = decorations;
        Map.Materials = materials;

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

        Map.BulidMap(Instantiate, fieldPlatform, decorationsPlatform);
    }
}
