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
    public GameObject[] decorations;

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
    Cycle cycle = new Cycle();
    //Running when the app starts
    void Start()
    {
        UnityEditor.SceneView.FocusWindowIfItsOpen(typeof(UnityEditor.SceneView));
        GenerateMap();

        Creature.digitalMap = this.map.digitalMap;
        Creature.objectMap = this.map.objectMap;

        Sun = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Sun.transform.position = new Vector3(mapSize.x / 2,mapSize.y / 2, 10f);
        Sun.GetComponent<Renderer>().material.color = Color.yellow;
        Sun.AddComponent<Light>().type = LightType.Directional;
        Sun.GetComponent<Light>().shadows = LightShadows.Soft;
        Sun.GetComponent<Light>().intensity = 1f;

        Moon = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Moon.transform.position = new Vector3(mapSize.x / 2, mapSize.y / 2, 10f);
        Moon.GetComponent<Renderer>().material.color = Color.white;
        Moon.AddComponent<Light>().type = LightType.Directional;
        Moon.GetComponent<Light>().shadows = LightShadows.Soft;
        Moon.GetComponent<Light>().intensity = 1f;
        //cycle.CreateSphere(Sun,"Sun",Color.yellow,mapSize.x,mapSize.y);
        //cycle.CreateSphere(Moon,"Moon",Color.white, mapSize.x, mapSize.y);
        creatures = CreateCreatures(13);

        // creatures = new List<Creature>();
        // creatures.Add(new Creature(new Vector2(0, 0), 0));
        // creatures.Add(new Creature(new Vector2(0, 5), 0));
        // creatures.Add(new Creature(new Vector2(5, 0), 0));
    }
  
    public class Cycle
    {
        // Радіус між двома сферами
        private float radius;
        // Час ротації 
       

        public Cycle() { }
        public GameObject CreateSphere(GameObject gameObject,string name,Color color,float sizeX,float sizeY)
        {
            gameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            gameObject.transform.position = new Vector3(sizeX / 2,sizeY / 2, 10f);
            gameObject.GetComponent<Renderer>().material.color = color;
            gameObject.AddComponent<Light>().type = LightType.Directional;
            gameObject.GetComponent<Light>().shadows = LightShadows.Soft;
            gameObject.GetComponent<Light>().intensity = 2f;
            return gameObject;
        }
        //public void TranformSpheres(GameObject gameObject1,GameObject gameObject2,float sizeX, float sizeY)
        //{
        //    timeRT = (timeRT + Time.deltaTime) % gameDayRLSeconds;
        //    float sunangle = TimeOfDay * 360;
        //    float moonangle = TimeOfDay * 360 + 180;
        //    Vector3 midpoint = new Vector3(sizeX / 2, sizeY / 2, 0);
        //    gameObject1.transform.position = midpoint + Quaternion.Euler(0, 0, sunangle) * (radius * Vector3.right);
        //    gameObject1.transform.LookAt(midpoint);
        //    gameObject2.transform.position = midpoint + Quaternion.Euler(0, 0, moonangle) * (radius * Vector3.right);
        //    gameObject2.transform.LookAt(midpoint);
        //}
        
    }
    public float timeRT = 0;
    public const float daytimeRLSeconds = 10.0f * 60;
    public const float duskRLSeconds = 1.5f * 60;
    public const float nighttimeRLSeconds = 7.0f * 60;
    public const float sunsetRLSeconds = 1.5f * 60;
    public const float gameDayRLSeconds = daytimeRLSeconds + duskRLSeconds + nighttimeRLSeconds + sunsetRLSeconds;

    public const float startOfDaytime = 0;
    public const float startOfDusk = daytimeRLSeconds / gameDayRLSeconds;
    public const float startOfNighttime = startOfDusk + duskRLSeconds / gameDayRLSeconds;
    public const float startOfSunset = startOfNighttime + nighttimeRLSeconds / gameDayRLSeconds;
    public float TimeOfDay
    {
        get { return timeRT / gameDayRLSeconds; }
        set { timeRT = value * gameDayRLSeconds; }
    }
    void OnGUI()
    {
        Rect rect = new Rect(10, 10, 120, 20);
        GUI.Label(rect, "time: " + TimeOfDay); rect.y += 20;
        GUI.Label(rect, "timeRT: " + timeRT);
        rect = new Rect(120, 10, 200, 10);
        TimeOfDay = GUI.HorizontalSlider(rect, TimeOfDay, 0, 1);
    }
    void Update() {
        creatureCycle();
        MapGenerator.TIME += MapGenerator.TIME_STEP;
        
        
        timeRT = (timeRT + Time.deltaTime) % gameDayRLSeconds;
        float sunangle = TimeOfDay * 360;
        float moonangle = TimeOfDay * 360 + 180;
        Vector3 midpoint = new Vector3(mapSize.x / 2, mapSize.y / 2, 0);
        Sun.transform.position = midpoint + Quaternion.Euler(0, 0, sunangle) * (20 * Vector3.right);
        Sun.transform.LookAt(midpoint);
        Moon.transform.position = midpoint + Quaternion.Euler(0, 0, moonangle) * (20 * Vector3.right);
        Moon.transform.LookAt(midpoint);

    }
    private List<Creature> CreateCreatures(int amount) {
        List<Creature> result = new List<Creature>();
        
        for(int i = 0; i < amount; i++) {
            var random = Random.Range(0, groundCoordinates.Count-1);
            Vector3 pickedGroundCoordinates = groundCoordinates[random];
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

        string holderName = "Map";
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
        map.CreateGameObjectMap(cube, platform, noiseArray, noiseScale, heightDifference);        
        map.PlaceFood((int)foodPercent);
        map.PlaceDecoration((int)decorationPercent);
        map.PaintMap(materials, noiseArray, heightDifference, groundCoordinates, waterCoordinates, decorationCoordinates, foodCoordinates);
        map.LocateDecorations(decorations, decorationCoordinates, decorationsPlatform, cube);
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

        public void LocateDecorations(GameObject[] decorations, List<Vector3> dc, Transform decoratinonsPlatform, GameObject cube)
        {
            float additionalHeight = 0;           

            //renderer = decorations[rnd].GetComponent<Renderer>();
            //additionalHeight = renderer.bounds.size.y / 2;            
            renderer = cube.GetComponent<Renderer>();            
            additionalHeight += renderer.bounds.size.y / 2;

            for (int i = 0; i < dc.Count; i++)
            {
                int rnd = Random.Range(0, decorations.Length);
                Instantiate(decorations[rnd], new Vector3(dc[i].x, dc[i].y + additionalHeight, dc[i].z), Quaternion.identity).transform.parent = decoratinonsPlatform;
            }
            //Vector3 vector3 = new Vector3();
            //float additionalHeight;

            //for (int i = 0; i < dc.Count; i++)
            //{
            //    int rnd = Random.Range(0, decorations.Length);
            //    renderer = decorations[rnd].GetComponent<Renderer>();               
            //    additionalHeight = renderer.bounds.size.y / 2;
            //    Debug.Log("Decor height / 2 " + additionalHeight);
            //    renderer = cube.GetComponent<Renderer>();
            //    Debug.Log("Cube height / 2 " + renderer.bounds.size.y / 2);
            //    additionalHeight += renderer.bounds.size.y / 2;
            //    Debug.Log("Additional height " + additionalHeight);
            //    switch (rnd)
            //    {
            //        case 0:
            //            {                                                      
            //                Debug.Log("dc[i].y " + dc[i].y);
            //                vector3 = new Vector3(dc[i].x, dc[i].y + additionalHeight, dc[i].z);
            //                break;
            //            }
            //        case 1:
            //            {                            
            //                Debug.Log("dc[i].y " + dc[i].y);                            
            //                vector3 = new Vector3(dc[i].x + 0.2f, dc[i].y + additionalHeight, dc[i].z - 0.2f);
            //                break;
            //            }
            //        case 2:
            //            {                            
            //                Debug.Log("dc[i].y " + dc[i].y);                                                       
            //                vector3 = new Vector3(dc[i].x, dc[i].y + additionalHeight, dc[i].z);
            //                break;
            //            }
            //    }
            //    Instantiate(decorations[rnd], vector3, Quaternion.identity).transform.parent = decoratinonsPlatform;                
            //}            
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
                                renderer.material = materials[0];//2
                                fc.Add(objectMap[i, j].transform.position);
                                break;
                            }
                        case 3:
                            {
                                renderer.material = materials[0];//3
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
