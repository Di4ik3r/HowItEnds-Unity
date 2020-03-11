﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.DayNightCycle
{
    [Serializable]
    public class TimeHolder
    {
        public float DayLength { get; set; } = 1f;
        public int YearLength { get; set; } = 365;

        public int DayNumber { get; set; } = 1;
        public int YearNumber { get; set; } = 1;

        public float TimeOfDay { get; set; } = 0f;
        public float ElapsedTime { get; set; } = 0f;

        public bool Use24Hours { get; set; }

        //private static TimeHolder instance;
        //public static TimeHolder Instance
        //{
        //    get
        //    {
        //        if (instance == null)
        //        {
        //            instance = new TimeHolder();
        //        }
        //        return instance;
        //    }
        //    set
        //    {
        //        TimeHolder.instance = value;
        //    }
        //}

        private static TimeHolder instance;

        private TimeHolder() { }

        public static TimeHolder getInstance()
        {
            if (instance == null)
                instance = new TimeHolder();
            return instance;
        }
    }
}
