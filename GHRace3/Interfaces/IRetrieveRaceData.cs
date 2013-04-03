using System;
namespace Interfaces
{
    public interface IRetrieveRaceData
    {
        System.Collections.Generic.ICollection<Interfaces.ITrack> GetData(System.Collections.Generic.Dictionary<string, string> htmlPageList);
    }
}
