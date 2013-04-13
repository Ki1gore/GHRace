using GHRace3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GHRace3.DAL
{
    public class DBGetRaces
    {
        private GHRaceContext ctx = new GHRaceContext();

        public List<Greyhound> GetRunnersData(List<string> names)
        {
            List<Greyhound> dogs = new List<Greyhound>();
            foreach (var item in names)
            {
                try
                {
                    dogs.Add(ctx.Greyhounds.Single(gh => gh.Name == item));
                }
                catch (Exception) { }
            }
            return dogs;
        }
    }
}