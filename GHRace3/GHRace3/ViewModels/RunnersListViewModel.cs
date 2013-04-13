using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GHRace3.ViewModels
{
    //represents a single race, with a list of 5/6 dogs
    public class RaceListViewModel
    {
        public List<Runner> Runners { get; set; } //a list of the dogs running in this race => {1, "Lances Gallahad"} = Lances Gallahad running in trap 1
    }
}