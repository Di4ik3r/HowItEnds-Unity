using Assets.Scripts.DayNightCycle;
using Assets.Scripts.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Services
{
    [Serializable]
    public class GameModel
    {
        public MapHolder Map { get; set; }
        public TimeHolder Time { get; set; }

        public GameModel(MapHolder map, TimeHolder time)
        {
            Map = map;
            Time = time;
        }
    }
}
