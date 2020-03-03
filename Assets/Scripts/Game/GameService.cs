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

        public static void SaveGame(GameModel game, string fileName = "undefined", bool autoSave = false)
        {
            string saveName;
            
            if (autoSave)
            {
                saveName = Application.persistentDataPath + "/" + DateTime.Now.ToString().Replace("/", "").Replace(" ", "").Replace(":", "") + "_autosave.save";
            }
            else
            {
                saveName = Application.persistentDataPath + "/" + fileName + ".save";
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
        }

        private static void FillTimeHolder()
        {
            TimeHolder timeHolder = TimeHolder.getInstance();

            PropertyCopier<TimeHolder, TimeHolder>.Copy(Game.Time, timeHolder);         
        }        
    }
}
