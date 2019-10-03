using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public Vector2 mapSize;
    public Vector2 lakeSize;
    public float noiseScale;
    public int octaves;
    [Range(0, 1)]
    public float persistence;
    [Range(0, 1)]
    public float lacunarity;
    public Vector2 offset;
    public int seed;
    [Range(0.1f, 15)]
    public float heightDifference;
    [Range(0, 100)]
    public float foodPercent;
    [Range(0, 100)]
    public float decorationPercent;
    public GameObject cube;
    public Material[] materials;

    public GameObject Sun;
    public GameObject Moon;

    private Map map;

    private static float TIME = 0;
    private static float TIME_STEP = .1f;

    private List<Creature> creatures;

    private List<Vector3> groundCoordinates;
    private List<Vector3> waterCoordinates;
    private List<Vector3> foodCoordinates;
    private List<Vector3> decorationCoordinates;

    //Running when the app starts
    void Start()
    {
        GenerateMap();

        Creature.digitalMap = this.map.digitalMap;
        Creature.objectMap = this.map.objectMap;
        creatures = CreateCreatures(10);
    
    }
    void Update() {
        creatureCycle();
        MapGenerator.TIME += MapGenerator.TIME_STEP;
    }
    private List<Creature> CreateCreatures(int amount) {
        List<Creature> result = new List<Creature>();
        
        for(int i = 0; i < amount; i++) {
            Vector3 pickedGroundCoordinates = groundCoordinates[Random.Range(0, groundCoordinates.Count)];
            Vector2 position = new Vector2(pickedGroundCoordinates.x, pickedGroundCoordinates.z);
            Creature creature = new Creature(position, 0);
            result.Add(creature);
        }

        return result;
    }

    private void creatureCycle() {
        foreach(Creature creature in creatures) {
            creature.MakeMove();
        }
    }

    public int[,] getDigitalMap(){ 
        return this.map.digitalMap;
    }

    public GameObject[,] getObjectMap() {
        return this.map.objectMap;
    }

    //The main method where everything is created, method take needed values
    public void GenerateMap()
    {
        groundCoordinates = new List<Vector3>();
        waterCoordinates = new List<Vector3>();
        foodCoordinates = new List<Vector3>();
        decorationCoordinates = new List<Vector3>();

        string holderName = "Platform";
        if (transform.Find(holderName))
        {
            DestroyImmediate(transform.Find(holderName).gameObject);
        }

        Transform platform = new GameObject(holderName).transform;
        platform.parent = transform;

        int width = System.Convert.ToInt32(mapSize.x);
        int lenght = System.Convert.ToInt32(mapSize.y);

        float[,] noiseArray = Noise.GenerateNoiseMap(width, lenght, seed, noiseScale, octaves, persistence, lacunarity, offset);

        map = new Map(width, lenght);
        map.CreateGameObjectMap(cube, platform, noiseArray, noiseScale, heightDifference);        
        map.PlaceFood((int)foodPercent);
        map.PlaceDecoration((int)decorationPercent);
        map.PaintMap(materials, noiseArray, heightDifference, groundCoordinates, waterCoordinates, decorationCoordinates, foodCoordinates);
    }
    //Class for making map
    class Map
    {
        public int Width { get; set; }
        public int Lenght { get; set; }
        public int[,] digitalMap { get; set; }
        public GameObject[,] objectMap { get; set; }
        private Renderer renderer;

        public Map(int width, int lenght)
        {
            Width = width;
            Lenght = lenght;
        }

        //Depending on the digital map value paint the cube in particular color
        //0 - earth
        //1 - water
        //2 - food
        //3 - decoration
        public void PaintMap(Material[] materials, float[,] noiseArray, float heightDifference, List<Vector3> gc, List<Vector3> wc, List<Vector3> dc, List<Vector3> fc)
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Lenght; j++)
                {
                    renderer = objectMap[i, j].GetComponent<Renderer>();
                    switch (digitalMap[i, j])
                    {
                        case 0:
                            {
                                renderer.material = materials[0];
                                gc.Add(objectMap[i, j].transform.position);
                                break;
                            }
                        case 1:
                            {
                                // objectMap[i, j].transform.position = new Vector3(-Width / 2 + 0.5f + i, (GetMinNoise(noiseArray) + heightDifference * 0.3f), -Lenght / 2 + 0.5f + j);
                                objectMap[i, j].transform.position = new Vector3(i, (GetMinNoise(noiseArray) + heightDifference * 0.3f), j);
                                renderer.material = materials[1];
                                wc.Add(objectMap[i, j].transform.position);
                                break;
                            }
                        case 2:
                            {
                                renderer.material = materials[2];
                                fc.Add(objectMap[i, j].transform.position);
                                break;
                            }
                        case 3:
                            {
                                renderer.material = materials[3];
                                dc.Add(objectMap[i, j].transform.position);
                                break;
                            }
                    }
                }
            }
        }

        //Randomly placing food on the map
        public void PlaceFood(int foodPercent)
        {
            System.Random rnd = new System.Random();
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Lenght; j++)
                {
                    if (digitalMap[i, j] != 1)
                    {
                        digitalMap[i, j] = (rnd.Next(0, 100) < foodPercent / 15) ? 2 : 0;                        
                    }
                }
            }
        }

        //Randomly placing decoration on the map
        public void PlaceDecoration(int decorationPercent)
        {
            System.Random rnd = new System.Random();
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Lenght; j++)
                {
                    if (digitalMap[i, j] != 1 && digitalMap[i, j] != 2)
                    {
                        digitalMap[i, j] = (rnd.Next(0, 100) < decorationPercent / 15) ? 3 : 0;
                    }
                }
            }
        }

        //Initializing digital and object arrays with values
        //Instantiating cubes, making map visible
        public void CreateGameObjectMap(GameObject gameObject, Transform platform, float[,] noiseArray, float noiseScale, float heightDifference)
        {
            System.Random rnd = new System.Random();
            digitalMap = new int[Width, Lenght];
            objectMap = new GameObject[Width, Lenght];

            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Lenght; j++)
                {
                    objectMap[i, j] = Instantiate(gameObject, new Vector3(i, noiseArray[i, j] * heightDifference, j), Quaternion.identity);
                    objectMap[i, j].transform.parent = platform;

                    if (System.Math.Round(noiseArray[i, j], 1) < 0.4)
                    {
                        digitalMap[i, j] = 1;
                    }
                    else
                    {
                        digitalMap[i, j] = 0;
                    }
                }
            }
        }   
        
        public void GetPostionVectors()
        {

        }
    }

    //Finding min noise value from array
    public static float GetMinNoise(float[,] noiseArray)
    {
        float min = noiseArray[0, 0];
        for (int i = 0; i < noiseArray.GetLength(0); i++)
        {
            for (int j = 0; j < noiseArray.GetLength(1); j++)
            {
                if (noiseArray[i, j] < min)
                {
                    min = noiseArray[i, j];
                }
            }
        }
        return min;
    }
}
