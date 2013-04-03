using Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceDataService
{
    [Serializable]
    public class SeedGreyhound : IGreyhound
    {
        public int GreyhoundID { get; set; }
        public string Name { get; set; }
        public ICollection<IRace> Races { get; set; }
    }
}
