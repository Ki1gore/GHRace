using System;
namespace Interfaces
{
    public interface IRace
    {
        string Comments { get; set; }
        DateTime Date { get; set; }
        string Details { get; set; }
        int Finish { get; set; }
        string Grade { get; set; }
        int RaceDistance { get; set; }
        int RaceID { get; set; }
        int? SP1 { get; set; }
        int? SP2 { get; set; }
        string SP3 { get; set; }
        double? TimeDistance1 { get; set; }
        double? TimeDistance2 { get; set; }
        string TimeDistance3 { get; set; }
        double? TimeSec { get; set; }
        string Trainer { get; set; }
        int Trap { get; set; }
    }
}
