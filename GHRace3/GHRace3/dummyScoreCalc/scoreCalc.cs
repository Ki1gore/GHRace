using GHRace3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GHRace3.dummyScoreCalc
{
    //dummy score calculator that just provides data for the view
    public class scoreCalc
    {
        private const string FILLERTEXT = "field text";
        public List<scores> GetScores(List<List<Greyhound>> runners)
        {
            List<scores> raceScores = new List<scores>();

            foreach (var race in runners) //for every race in the meeting 
            {
                raceScores.Add(GetIndividualRaceScores(race));
            }

            return raceScores;
        }

        private scores GetIndividualRaceScores(List<Greyhound> race)
        {
            scores sc = new scores();
            sc.Scores = new List<score>();
            foreach (var item in race)
            {
                sc.Scores.Add(new score
                {
                    Comments = item.Races.First().Comments,
                    scoreField1 = true,
                    NoData = false,
                    GradesUsed = "A1, A2, A3, A4",
                    scoreField2 = FILLERTEXT,
                    scoreField3 = FILLERTEXT,
                    scoreField4 = FILLERTEXT,
                    scoreField5 = FILLERTEXT,
                    scoreField6 = FILLERTEXT,
                    Name = item.Name,
                    Score = RandomNumber(1, 1000),
                    scoreField7 = FILLERTEXT,
                    Trap = item.Races.First().Trap
                });
            }
            return sc;
        }

        private int RandomNumber(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }
    }
}