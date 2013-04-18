using GHRace3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GHRace3.ViewModels
{
    /// <summary>
    /// not used at the moment since results scraped from URL's are no longer sent back the view
    /// </summary>
    public class RaceViewModel
    {
        public string Trackname { get; set; }
        public string RaceDate { get; set; }
        public string Grade { get; set; }
        public string Finish { get; set; }
        public string Trap { get; set; }
        public string SP1 { get; set; }
        public string SP2 { get; set; }
        public string SP3 { get; set; }
        public string TimeSec { get; set; }
        public string TimeDistance1 { get; set; }
        public string TimeDistance2 { get; set; }
        public string TimeDistance3 { get; set; }
        public string Details { get; set; }
        public string Trainer { get; set; }
        public string Comments { get; set; }
        public string RaceDistance { get; set; }
        public string GreyhoundName { get; set; }
    }
}