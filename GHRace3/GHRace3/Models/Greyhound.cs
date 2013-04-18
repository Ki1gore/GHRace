using Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GHRace3.Models
{
    public class Greyhound
    {
        public int GreyhoundID { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Result> Results { get; set; }
        public virtual Track Track { get; set; }
    }
}