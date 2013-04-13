using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GHRace3.dummyScoreCalc
{
    [Serializable]
    public class scores
    {
        public List<score> Scores { get; set; }
    }

    [Serializable]
    public class score
    {
        public string Name { get; set; }
        public bool NoData { get; set; }
        public bool scoreField1 { get; set; }
        public int Score { get; set; }
        public string Comments { get; set; }
        public string scoreField2 { get; set; }
        public int Trap { get; set; }
        public string scoreField3 { get; set; }
        public string scoreField4 { get; set; }
        public string scoreField5 { get; set; }
        public string scoreField6 { get; set; }
        public string scoreField7 { get; set; }
        public string GradesUsed { get; set; }
    }
}