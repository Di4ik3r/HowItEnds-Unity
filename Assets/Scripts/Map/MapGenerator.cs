using Assets.Scripts.Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public Vector2 mapSize;   
    public float noiseScale;
    public int octaves;
    [Range(0, 1)]
    public float persistence;
    [Range(0, 1)]
    public float lacunarity;
    public Vector2 offset;
    public int seed;
    [Range(-1.5f, 15)]
    public float heightDifference;
    [Range(0, 100)]
    public float foodPercent;
    [Range(0, 100)]
    public float decorationPercent;
    public GameObject cube;
    public Material[] materials;
    public GameObject[] decorations;

    [HideInInspector]
    public List<Vector3> groundCoordinates;
    [HideInInspector]
    public List<Vector3> waterCoordinates;
    [HideInInspector]
    public List<Vector3> foodCoordinates;
    [HideInInspector]
    public List<Vector3> decorationCoordinates;
    [HideInInspector]
    public Map map;

    void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        groundCoordinates = new List<Vector3>();
        waterCoordinates = new List<Vector3>();
        foodCoordinates = new List<Vector3>();
        decorationCoordinates = new List<Vector3>();

        string holderName = "Field";
        if (transform.Find(holderName))
        {
            DestroyImmediate(transform.Find(holderName).gameObject);
        }

        Transform platform = new GameObject(holderName).transform;
        platform.parent = transform;

        holderName = "Decorations";
        if (transform.Find(holderName))
        {
            DestroyImmediate(transform.Find(holderName).gameObject);
        }

        Transform decorationsPlatform = new GameObject(holderName).transform;
        decorationsPlatform.parent = transform;

        int width = System.Convert.ToInt32(mapSize.x);
        int lenght = System.Convert.ToInt32(mapSize.y);

        float[,] noiseArray = Noise.GenerateNoiseMap(width, lenght, seed, noiseScale, octaves, persistence, lacunarity, offset);

        map = new Map(width, lenght);
        map.CreateGameObjectMap(Instantiate, cube, platform, noiseArray, heightDifference);
        map.PlaceFood((int)foodPercent);
        map.PlaceDecoration((int)decorationPercent);
        map.PaintMap(materials, noiseArray, heightDifference, groundCoordinates, waterCoordinates, decorationCoordinates, foodCoordinates);
        map.LocateDecorations(Instantiate, decorations, decorationCoordinates, decorationsPlatform, cube);
    }
}
