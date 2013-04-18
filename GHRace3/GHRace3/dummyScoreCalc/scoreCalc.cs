using GHRace3.DBService;
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
        Random random = new Random();
        GHRaceDBAccessService _dba;
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
            _dba = new GHRaceDBAccessService();
            scores sc = new scores();
            sc.Scores = new List<score>();
            foreach (var item in race)
            {
                Greyhound g = _dba.GetGreyhound(item.Name);
                if (g != null)
                {
                    sc.Scores.Add(new score
                    {
                        Comments = g.Results.First().Comments,
                        scoreField1 = true,
                        NoData = false,
                        GradesUsed = "A1, A2, A3, A4",
                        scoreField2 = FILLERTEXT,
                        scoreField3 = FILLERTEXT,
                        scoreField4 = FILLERTEXT,
                        scoreField5 = FILLERTEXT,
                        scoreField6 = FILLERTEXT,
                        Name = g.Name,
                        Score = random.Next(1, 1000),
                        scoreField7 = FILLERTEXT,
                        Trap = g.Results.First().Trap
                    });
                }
            }
            return sc;
        }
    }
}