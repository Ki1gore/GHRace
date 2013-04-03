using System;
namespace Interfaces
{
    public interface ITrack
    {
        System.Collections.Generic.ICollection<IGreyhound> Greyhounds { get; set; }
        string Name { get; set; }
        int TrackID { get; set; }
    }
}
