using Assets.Scripts.DayNightCycle;
using Assets.Scripts.Map;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Services
{
    public static class GameService
    {
        public static GameModel Game { get; set; }       

        public static void SaveGame(GameModel game, bool autoSave = false)
        {
            //Convert.ToBase64String
            string saveName;
            
            if (autoSave)
            {
                saveName = Application.persistentDataPath + "/" + DateTime.Now.ToString().Replace("/", "").Replace(" ", "").Replace(":", "") + "_autosave.save";
            }
            else
            {
                saveName = Application.persistentDataPath + "/" + DateTime.Now.ToString().Replace("/", "").Replace(" ", "").Replace(":", "") + ".save";
            }

            using (FileStream fs = new FileStream(saveName, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, game);                
            }

            Debug.Log("Game saved, path: " + saveName);
        }

        public static void LoadGame(string fileName)
        {
            using (FileStream fs = new FileStream(Application.persistentDataPath + "/" + fileName, FileMode.Open))
            {
                BinaryFormatter bf = new BinaryFormatter();
                Game = (GameModel)bf.Deserialize(fs);
            }

            FillMapHolder();
            FillTimeHolder();
        }

        private static void FillMapHolder()
        {
            MapHolder mapHolder = MapHolder.getInstance();

            PropertyCopier<MapHolder, MapHolder>.Copy(Game.Map, mapHolder);

            //mapHolder.Width = Game.Map.Width;
            //mapHolder.Lenght = Game.Map.Lenght;
            //mapHolder.DecorationPercent = Game.Map.Lenght;
            //mapHolder.DigitalMap = Game.Map.DigitalMap;
            //mapHolder.FoodPercent = Game.Map.FoodPercent;
            //mapHolder.HeightDifference = Game.Map.HeightDifference;
            //mapHolder.Lacunarity = Game.Map.Lacunarity;
            //mapHolder.NoiseScale = Game.Map.NoiseScale;
            //mapHolder.Octaves = Game.Map.Octaves;
            //mapHolder.OffsetX = Game.Map.OffsetX;
            //mapHolder.OffsetY = Game.Map.OffsetY;
            //mapHolder.Persistence = Game.Map.Persistence;
            //mapHolder.Seed = Game.Map.Seed;
        }

        private static void FillTimeHolder()
        {
            TimeHolder timeHolder = TimeHolder.getInstance();

            PropertyCopier<TimeHolder, TimeHolder>.Copy(Game.Time, timeHolder);         

            //timeHolder.DayLength = Game.Time.DayLength;
            //timeHolder.DayNumber = Game.Time.DayNumber;
            //timeHolder.ElapsedTime = Game.Time.ElapsedTime;
            //timeHolder.TimeOfDay = Game.Time.TimeOfDay;
            //timeHolder.Use24Hours = Game.Time.Use24Hours;
            //timeHolder.YearLength = Game.Time.YearLength;
            //timeHolder.YearNumber = Game.Time.YearNumber;
        }        
    }
}
