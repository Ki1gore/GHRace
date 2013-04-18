using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GHRace3.ViewModels
{
    /// <summary>
    /// Returned after new data is added.  New races that have been added appear in the Added list, those not added appear in Skipped
    /// </summary>
    public class LoadStatusViewModel
    {
        public List<TrackAndUrl> Added { get; set; }
        public List<TrackAndUrl> Skipped { get; set; }
    }

    public class TrackAndUrl
    {
        public string TrackName { get; set; }
        public string Date { get; set; }
        public string url { get; set; }
    }
}