using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GHRace3.ViewModels
{
    /// <summary>
    /// Used to provide current DB info to the front page
    /// </summary>
    public class DBInfoViewModel
    {
        public int Tracks { get; set; }
        public int Greyhounds { get; set; }
        public int Results { get; set; }
        public List<LatestRaceForTrack> RecentlyAdded { get; set; }
    }

    public class LatestRaceForTrack
    {
        public string TrackName { get; set; }
        public DateTime DateofLatestRace { get; set; }
    }
}