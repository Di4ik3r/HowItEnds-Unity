using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Map
{
    [Serializable]
    public class MapHolder
    {
        public int Width { get; set; } = 25;
        public int Lenght { get; set; } = 25;
        public float NoiseScale { get; set; } = 10f;
        public int Octaves { get; set; } = 10;
        public float Persistence { get; set; } = 1f;
        public float Lacunarity { get; set; } = 0f;
        public int OffsetX { get; set; } = 0;
        public int OffsetY { get; set; } = 0;
        public int Seed { get; set; } = 0;
        public float HeightDifference { get; set; } = 4f;
        public float FoodPercent { get; set; } = 50f;
        public float DecorationPercent { get; set; } = 50f;

        [NonSerialized]
        public List<Vector3> GroundCoordinates;
        [NonSerialized]
        public List<Vector3> WaterCoordinates;
        [NonSerialized]
        public List<Vector3> FoodCoordinates;
        [NonSerialized]
        public List<Vector3> DecorationCoordinates;

        public int[,] DigitalMap { get; set; }
        private float[,] NoiseArray { get; set; }
       
        [NonSerialized]
        public GameObject Cube;
        [NonSerialized]
        public Material[] Materials;
        [NonSerialized]
        public GameObject[] Decorations;
        [NonSerialized]
        public GameObject[] Food;
        [NonSerialized]
        public GameObject[,] ObjectMap;
        [NonSerialized]
        private Renderer renderer;

        private static MapHolder instance;
        public static MapHolder Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MapHolder();
                }
                return instance;
            }
        }

        private Func<GameObject, Vector3, Quaternion, GameObject> Instantiate;

        public void BuildMap(Func<GameObject, Vector3, Quaternion, GameObject> instantiate, 
            Transform fieldPlatform, Transform decorPlatform, Transform foodPlatform)
        {
            Instantiate = instantiate;
            GroundCoordinates = new List<Vector3>();
            WaterCoordinates = new List<Vector3>();
            FoodCoordinates = new List<Vector3>();
            DecorationCoordinates = new List<Vector3>();

            if (Cube && Materials.Length >= 4  && Decorations.Length >= 3 && Food.Length >= 1)
            {
                NoiseArray = Noise.GenerateNoiseMap(Width, Lenght, Seed, NoiseScale, Octaves, Persistence, Lacunarity, new Vector2(OffsetX, OffsetY));
                CreateGameObjectMap(instantiate, fieldPlatform);
                PlaceFood();
                PlaceDecoration();
                PaintMap();
                LocateDecorations(instantiate, decorPlatform);
                LocateFood(instantiate, foodPlatform);
            }
        }

        public void PaintMap()
        {            
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Lenght; j++)
                {
                    renderer = ObjectMap[i, j].GetComponent<Renderer>();
                    switch (DigitalMap[i, j])
                    {
                        case 0:
                            {
                                renderer.material = Materials[0];
                                GroundCoordinates.Add(ObjectMap[i, j].transform.position);
                                break;
                            }
                        case 1:
                            {
                                ObjectMap[i, j].transform.position = new Vector3(i, (GetMinNoise(NoiseArray) + HeightDifference * 0.3f), j);
                                renderer.material = Materials[1];
                                WaterCoordinates.Add(ObjectMap[i, j].transform.position);
                                break;
                            }
                        case 2:
                            {
                                renderer.material = Materials[2];//2
                                FoodCoordinates.Add(ObjectMap[i, j].transform.position);
                                break;
                            }
                        case 3:
                            {
                                renderer.material = Materials[3];//3
                                DecorationCoordinates.Add(ObjectMap[i, j].transform.position);
                                break;
                            }
                    }
                }
            }
        }

        public void PlaceFood()
        {
            System.Random rnd = new System.Random();
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Lenght; j++)
                {
                    if (DigitalMap[i, j] != 1)
                    {
                        DigitalMap[i, j] = (rnd.Next(0, 100) < FoodPercent / 15) ? 2 : 0;
                    }
                }
            }
        }

        //Randomly placing decoration on the map
        public void PlaceDecoration()
        {
            System.Random rnd = new System.Random(Seed);
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Lenght; j++)
                {
                    if (DigitalMap[i, j] != 1 && DigitalMap[i, j] != 2)
                    {
                        DigitalMap[i, j] = (rnd.Next(0, 100) < DecorationPercent / 15) ? 3 : 0;
                    }
                }
            }
        }

        //Initializing digital and object arrays with values
        //Instantiating cubes, making map visible
        public void CreateGameObjectMap(Func<GameObject, Vector3, Quaternion, GameObject> instantiate, Transform fieldPlatform)
        {
            System.Random rnd = new System.Random();
            DigitalMap = new int[Width, Lenght];
            ObjectMap = new GameObject[Width, Lenght];            
            
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Lenght; j++)
                {
                    ObjectMap[i, j] = instantiate(Cube, new Vector3(i, NoiseArray[i, j] * HeightDifference, j), Quaternion.identity);
                    ObjectMap[i, j].transform.parent = fieldPlatform;

                    if (Math.Round(NoiseArray[i, j], 1) < 0.4)
                    {
                        DigitalMap[i, j] = 1;
                    }
                    else
                    {
                        DigitalMap[i, j] = 0;
                    }
                }
            }
        }

        public void LocateDecorations(Func<GameObject, Vector3, Quaternion, GameObject> instantiate, Transform decoratinonsPlatform)
        {
            float additionalHeight = 0;

            Renderer renderer = Cube.GetComponent<Renderer>();
            additionalHeight += renderer.bounds.size.y / 2;

            for (int i = 0; i < DecorationCoordinates.Count; i++)
            {
                int rnd = UnityEngine.Random.Range(0, Decorations.Length);
                instantiate(Decorations[rnd], new Vector3(DecorationCoordinates[i].x, DecorationCoordinates[i].y + additionalHeight, DecorationCoordinates[i].z),
                    /*Quaternion.identity*/Decorations[rnd].transform.rotation).transform.parent = decoratinonsPlatform;
            }
        }

        public void LocateFood(Func<GameObject, Vector3, Quaternion, GameObject> instantiate, Transform foodPlatform)
        {
            float additionalHeight = 0;

            Renderer renderer = Cube.GetComponent<Renderer>();
            additionalHeight += renderer.bounds.size.y / 2;

            for (int i = 0; i < FoodCoordinates.Count; i++)
            {
                int rnd = UnityEngine.Random.Range(0, Food.Length);
                instantiate(Food[rnd], new Vector3(FoodCoordinates[i].x, FoodCoordinates[i].y + additionalHeight, FoodCoordinates[i].z),
                    /*Quaternion.identity*/Food[rnd].transform.rotation).transform.parent = foodPlatform;
            }
        }

        public void SwapFoodWithDecor(Vector2 foodBlock)
        {
            float additionalHeight = 0;

            Renderer renderer = Cube.GetComponent<Renderer>();
            additionalHeight += renderer.bounds.size.y / 2;

            Debug.Log("foodBlock.x + foodBlock.y: " + foodBlock.x + foodBlock.y);
            Instantiate(Decorations[1], new Vector3(foodBlock.x, foodBlock.y + additionalHeight + 10f, foodBlock.y),
                  Decorations[1].transform.rotation);
        }

        private float GetMinNoise(float[,] noiseArray)
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
}
