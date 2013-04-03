using Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GHRace3.Models
{
    public class Race
    {
        public int RaceID { get; set; }
        public DateTime Date { get; set; }
        public string Grade { get; set; }
        public int Finish { get; set; }
        public int Trap { get; set; }
        public int? SP1 { get; set; }
        public int? SP2 { get; set; }
        public string SP3 { get; set; }
        public double? TimeSec { get; set; }
        public double? TimeDistance1 { get; set; }
        public double? TimeDistance2 { get; set; }
        public string TimeDistance3 { get; set; }
        public string Details { get; set; }
        public string Trainer { get; set; }
        public string Comments { get; set; }
        public int RaceDistance { get; set; }
        public virtual Greyhound Greyhound { get; set; }
    }
}