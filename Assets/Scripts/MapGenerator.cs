﻿using System.Collections;
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

    void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        string holderName = "Platform";
        if (transform.FindChild(holderName))
        {
            DestroyImmediate(transform.FindChild(holderName).gameObject);
        }

        Transform platform = new GameObject(holderName).transform;
        platform.parent = transform;

        int width = System.Convert.ToInt32(mapSize.x);
        int lenght = System.Convert.ToInt32(mapSize.y);

        float[,] noiseArray = Noise.GenerateNoiseMap(width, lenght, seed, noiseScale, octaves, persistence, lacunarity, offset);

        Map map = new Map(width, lenght);
        map.CreateGameObjectMap(cube, materials, platform, noiseArray);
        map.MakeLake(System.Convert.ToInt32(lakeSize.x), System.Convert.ToInt32(lakeSize.y), materials, noiseArray);        
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

        public void CreateDigitalMap()
        {
            digitalMap = new int[Width, Lenght];
            System.Random rnd = new System.Random();
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Lenght; j++)
                {
                    digitalMap[i, j] = rnd.Next(0, 4);
                }
            }
        }

        public void CreateGameObjectMap(GameObject gameObject, Material[] materials, Transform platform, float[,] noiseArray)
        {
            System.Random rnd = new System.Random();
            CreateDigitalMap();
            objectMap = new GameObject[Width, Lenght];
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Lenght; j++)
                {
                    objectMap[i, j] = Instantiate(gameObject, new Vector3(i, noiseArray[i, j], j), Quaternion.identity);
                    objectMap[i, j].transform.parent = platform;

                    if (System.Math.Round(noiseArray[i, j], 1) > 0.1 && System.Math.Round(noiseArray[i, j], 1) < 0.3)
                    {
                        objectMap[i, j].transform.position = new Vector3(i, GetMinNoise(noiseArray), j);
                        renderer = objectMap[i, j].GetComponent<Renderer>();
                        renderer.material = materials[1];
                        digitalMap[i, j] = 1;
                    }
                    else
                    {
                        renderer = objectMap[i, j].GetComponent<Renderer>();
                        renderer.material = materials[0];
                        digitalMap[i, j] = 0;
                    }
                }
            }
        }

        public void MakeLake(int lakeLenght, int lakeWidth, Material[] materials, float[,] noiseArray)
        {
            System.Random rnd = new System.Random();
            int posX = rnd.Next(0, Width);
            int posY = rnd.Next(0, Lenght);

            int differenceX = System.Math.Abs(Width - (lakeWidth + posX));
            int differenceY = System.Math.Abs(Lenght - (lakeLenght + posY));

            if (lakeWidth + posX >= Width && lakeLenght + posY <= Lenght)
            {
                Loop(posX - differenceX, Width, posY, posY + lakeLenght, materials, noiseArray);
            }
            else if (lakeLenght + posY >= Lenght && lakeWidth + posX < Width)
            {
                Loop(posX, posX + lakeWidth, posY - differenceY, Lenght, materials, noiseArray);
            }
            else if (lakeLenght + posY >= Lenght && lakeWidth + posX >= Width)
            {
                Loop(posX - differenceX, Width, posY - differenceY, Lenght, materials, noiseArray);
            }
            else
            {
                Loop(posX, posX + lakeWidth, posY, posY + lakeLenght, materials, noiseArray);
            }
        }
       
        private void Loop(int iStart, int iEnd, int jStart, int jEnd, Material[] materials, float[,] noiseArray)
        {
            Renderer renderer;
            for (int i = iStart; i < iEnd; i++)
            {
                for (int j = jStart; j < jEnd; j++)
                {
                    digitalMap[i, j] = 1;
                    objectMap[i, j].transform.position = new Vector3(i, GetMinNoise(noiseArray), j);
                    renderer = objectMap[i, j].GetComponent<Renderer>();
                    renderer.material = materials[1];
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