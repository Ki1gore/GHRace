using Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceDataService
{
    [Serializable]
    public class SeedTrack : ITrack
    {
        public ICollection<IGreyhound> Greyhounds { get; set; }
        public string Name { get; set; }
        public int TrackID { get; set; }
    }
}
