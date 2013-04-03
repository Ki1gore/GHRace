using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Interfaces;

namespace GHRace3.Models
{
    public class Track
    {
        public int TrackID { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Greyhound> Greyhounds { get; set; }
    }
}