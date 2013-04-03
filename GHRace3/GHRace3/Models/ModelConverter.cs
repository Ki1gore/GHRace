using Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GHRace3.Models
{
    public class ModelConverter
    {
        public static Track ConvertToTrack(Interfaces.ITrack item)
        {
            return new Track
            {
                Name = item.Name,
                Greyhounds = GetGreyHounds(item.Greyhounds)
            };
        }

        private static ICollection<Greyhound> GetGreyHounds(ICollection<Interfaces.IGreyhound> collection)
        {
            List<Greyhound> lg = new List<Greyhound>();
            foreach (var item in collection)
            {
                lg.Add(new Greyhound 
                {
                    Name = item.Name,
                    Races = GetRaces(item.Races)
                });
            }
            return lg;
        }

        private static ICollection<Race> GetRaces(ICollection<Interfaces.IRace> collection)
        {
            List<Race> lr = new List<Race>();
            foreach (var item in collection)
            {
                lr.Add(new Race
                {
                    Comments = item.Comments,
                    Date = item.Date,
                    Details = item.Details,
                    Finish = item.Finish,
                    Grade = item.Grade,
                    RaceDistance = item.RaceDistance,
                    SP1 = item.SP1,
                    SP2 = item.SP2,
                    SP3 = item.SP3,
                    TimeDistance1 = item.TimeDistance1,
                    TimeDistance2 = item.TimeDistance2,
                    TimeDistance3 = item.TimeDistance3,
                    TimeSec = item.TimeSec,
                    Trainer = item.Trainer,
                    Trap = item.Trap
                });
            }
            return lr;
        }
    }
}