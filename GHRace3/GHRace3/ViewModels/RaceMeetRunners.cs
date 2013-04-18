using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GHRace3.ViewModels
{
    /// <summary>
    /// container for multiple runners sent from the predict page
    /// </summary>
    [Serializable]
    public class RaceMeetRunners
    {
        public string multipleRace_Runners { get; set; }
    }
}