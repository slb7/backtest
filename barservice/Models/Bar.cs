using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace barservice.Models
{
    public class Bar
    {
        public float Open;
        public float High;
        public float Low;
        public float Close;
        public float DayHigh;
        public float DayLow;
        public int Volume;
        public DateTime Time;
    }
}