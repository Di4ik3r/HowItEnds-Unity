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
    [Range(0, 100)]
    public float foodPercent;
    [Range(0, 100)]
    public float decorationPercent;
    public GameObject cube;
    public Material[] materials;

    private Map map;

    private static float TIME = 0;
    private static float TIME_STEP = .1f;

    private List<Creature> creatures;

    void Start()
    {
        GenerateMap();

        Creature.digitalMap = getDigitalMap();
        Creature.objectMap = getObjectMap();

        creatures = new List<Creature>();

        Creature c1 = new Creature(new Vector2(2, 20), 1);
        Creature c2 = new Creature(new Vector2(2, 21), 1);

        creatures.Add(c1);
        creatures.Add(c2);
    }

    void Update() {
        // if(MapGenerator.TIME % 2 == 0) {
        if(System.Math.Round(MapGenerator.TIME, 1) % 2 == 0) {
            creatureCycle();
        }
            

        MapGenerator.TIME += MapGenerator.TIME_STEP;
    }

    private void creatureCycle() {
        foreach(Creature creature in creatures) {
            creature.MakeMove();
        }
    }

    public void GenerateMap()
    {
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
        map.CreateGameObjectMap(cube, platform, noiseArray);
        map.MakeLake(System.Convert.ToInt32(lakeSize.x), System.Convert.ToInt32(lakeSize.y), noiseArray);
        map.PlaceFood((int)foodPercent);
        map.PlaceDecoration((int)decorationPercent);
        map.PaintMap(materials, noiseArray);
    }

    public int[,] getDigitalMap(){ 
        return this.map.digitalMap;
    }

    public GameObject[,] getObjectMap() {
        return this.map.objectMap;
    }

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

        public void PaintMap(Material[] materials, float[,] noiseArray)
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
                                break;
                            }
                        case 1:
                            {
                                objectMap[i, j].transform.position = new Vector3(i, GetMinNoise(noiseArray), j);
                                renderer.material = materials[1];
                                break;
                            }
                        case 2:
                            {
                                renderer.material = materials[2];
                                break;
                            }
                        case 3:
                            {
                                renderer.material = materials[3];
                                break;
                            }
                    }
                }
            }
        }

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

        public void CreateGameObjectMap(GameObject gameObject, Transform platform, float[,] noiseArray)
        {
            System.Random rnd = new System.Random();
            digitalMap = new int[Width, Lenght];
            objectMap = new GameObject[Width, Lenght];

            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Lenght; j++)
                {
                    objectMap[i, j] = Instantiate(gameObject, new Vector3(i, noiseArray[i, j], j), Quaternion.identity);
                    objectMap[i, j].transform.parent = platform;

                    if (System.Math.Round(noiseArray[i, j], 1) > 0.1 && System.Math.Round(noiseArray[i, j], 1) < 0.3)
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

        public void MakeLake(int lakeLenght, int lakeWidth, float[,] noiseArray)
        {
            System.Random rnd = new System.Random();
            int posX = rnd.Next(0, Width);
            int posY = rnd.Next(0, Lenght);

            int differenceX = System.Math.Abs(Width - (lakeWidth + posX));
            int differenceY = System.Math.Abs(Lenght - (lakeLenght + posY));

            if (lakeWidth + posX >= Width && lakeLenght + posY <= Lenght)
            {
                Loop(posX - differenceX, Width, posY, posY + lakeLenght, noiseArray);
            }
            else if (lakeLenght + posY >= Lenght && lakeWidth + posX < Width)
            {
                Loop(posX, posX + lakeWidth, posY - differenceY, Lenght, noiseArray);
            }
            else if (lakeLenght + posY >= Lenght && lakeWidth + posX >= Width)
            {
                Loop(posX - differenceX, Width, posY - differenceY, Lenght, noiseArray);
            }
            else
            {
                Loop(posX, posX + lakeWidth, posY, posY + lakeLenght, noiseArray);
            }
        }
       
        private void Loop(int iStart, int iEnd, int jStart, int jEnd, float[,] noiseArray)
        {            
            for (int i = iStart; i < iEnd; i++)
            {
                for (int j = jStart; j < jEnd; j++)
                {
                    digitalMap[i, j] = 1;
                }
            }
        }
    }

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
