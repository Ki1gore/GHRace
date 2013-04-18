using Autofac;
using GHRace3.DBService;
using GHRace3.dummyScoreCalc;
using GHRace3.Models;
using GHRace3.ViewModels;
using Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace GHRace3.Controllers
{
    public class HomeController : Controller
    {
        private GHRaceContext ctx = new GHRaceContext();
        private const string TRACK = "Wimbledon"; //only returning greyhound names from wimbledon
        IRetrieveRaceData _retrieveRaceData;
        ICommonUtilities _utils;
        GHRaceDBAccessService _dba;

        public HomeController(IRetrieveRaceData retrieveRaceData, ICommonUtilities utils)
        {
            _retrieveRaceData = retrieveRaceData;
            _utils = utils;
            _dba = new GHRaceDBAccessService();
        }

        /// <summary>
        /// Returns a summary of whats currently in the database - track count, results count, greyhounds count, recently added.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            DBInfoViewModel vm = new DBInfoViewModel();
            vm.Tracks = _dba.GetTrackCount();
            vm.Results = _dba.GetResultsCount();
            vm.Greyhounds = _dba.GetGreyhoundCount();
            vm.RecentlyAdded = _dba.GetLatestResultForAllTracks();
            return View(vm);
        }

        /// <summary>
        /// Processes an ajax request containing a string of \n seperated urls, splits them into a distinct list of urls and downloads the html for each.
        /// Then passes a collection of urls to an html scraper.  Returns a list of urls added and any that were already in the DB and were skipped.
        /// </summary>
        /// <param name="urlList"></param>
        /// <returns></returns>
        public ActionResult Load(UrlString urlList)
        {
            if (urlList.Url == null) //normal request so return view
            {
                return View();
            }
           
            LoadStatusViewModel lsvm = new LoadStatusViewModel();
            lsvm.Skipped = new List<TrackAndUrl>();
            lsvm.Added = new List<TrackAndUrl>();
            string[] urlsRaw = urlList.Url.Replace('\r', ' ').Trim().Split('\n');

            var query = (from u in urlsRaw
                        select u).Distinct(); //remove any duplicate url's
            
            List<string> urls = new List<string>(query);
            Dictionary<string, string> p = new Dictionary<string, string>();

            using (WebClient wb = new WebClient())
            {
                foreach (var url in urls.Where(u => u.Contains("http://www.thedogs.co.uk/resultsMeeting.aspx?racedate")))
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(url);
                    //ParseQueryString on "http://www.thedogs.co.uk/resultsMeeting.aspx?racedate=23/03/2013%2000:00:00&track=Wimbledon" returns two keys: "track" and "http://www.thedogs.co.uk/resultsMeeting.aspx?racedate"
                    //hence the unusually long key string
                    string date = DateTime.Parse(nvc[@"http://www.thedogs.co.uk/resultsMeeting.aspx?racedate"].Trim()).ToShortDateString();
                    string trackName = nvc["track"].Trim();
                    if (_dba.RaceMeetingNotInDB(url))
                    {
                        lsvm.Added.Add(new TrackAndUrl { TrackName = trackName, Date = date, url = url });
                        p.Add(url, wb.DownloadString(new Uri(url)));    
                    }
                    else
                    {
                        lsvm.Skipped.Add(new TrackAndUrl { TrackName = trackName, Date = date, url = url });
                    }
                }
            }

            ICollection<ITrack> trackData = _retrieveRaceData.GetData(p);

            List<Track> tracks = new List<Track>();
            foreach (var item in trackData)
            {
                tracks.Add(ModelConverter.ConvertToTrack(item));
            }
            _dba.Add(tracks);
            
            return Json(lsvm);
        }

        public ActionResult Predict()
        {
            return View();
        }

        /// <summary>
        /// Responds to an ajax request containing either a list of greyhound names and starting traps (1 race) or a string with many \n seperated greyhound names in the format "1 Vegas Gold".
        /// Since these lists are going to a dummy score calc for the moment, **traps are ignored**.
        /// TODO: Runner can be changed to include a greyhound object thus preserving the trap value.
        /// </summary>
        /// <param name="runners"></param>
        /// <param name="m_runners"></param>
        /// <returns></returns>
        public ActionResult GetPredictions(List<Runner> runners, RaceMeetRunners m_runners)
        {
            List<List<Greyhound>> allRunners = GetMultipleRaceRunners(m_runners);
            List<Greyhound> singleRace = GetSingleRaceRunners(runners);

            scoreCalc sc = new scoreCalc();
            if (allRunners == null)
            {
                allRunners = new List<List<Greyhound>>();
                allRunners.Add(singleRace);
            }
            return Json(sc.GetScores(allRunners));
        }

        /// <summary>
        /// Returns a list of greyhound objects (so each object's Races collection can go to the score calc)
        /// </summary>
        /// <param name="runners"></param>
        /// <returns></returns>
        private List<Greyhound> GetSingleRaceRunners(List<Runner> runners)
        {
            if (runners == null)
            {
                return new List<Greyhound>();
            }
            var query = from r in runners
                        select r.Name;

            return _dba.GetRunnersData(new List<string>(query));
        }

        private List<List<Greyhound>> GetMultipleRaceRunners(RaceMeetRunners m_runners)
        {
            if (string.IsNullOrEmpty(m_runners.multipleRace_Runners))
            {
                return null;    
            }
            
            string[] p = m_runners.multipleRace_Runners.Split('\n'); //split the string into a list
            var query = from r in p
                        where !string.IsNullOrEmpty(r) //remove any empty list elements
                        select r;
            List<Runner> q = new List<Runner>();

            foreach (var item in query.Reverse()) //need to reverse the ordering on the linq query so they end up in the stack in the correct order (otherwise the last runner of the evening in
            {                                     //trap 6 will be at the top
                string[] r = item.Split(' ');
                q.Add(new Runner { Trap = int.Parse(_utils.RemoveLetters(item)), Name = _utils.RemoveNumbers(item).Trim() });
            }
            Stack<Runner> s = new Stack<Runner>(q);
            List<List<Greyhound>> g = new List<List<Greyhound>>();
            return DivideRaces(s, g);
        }

        /// <summary>
        /// Recursively loops through a Stack<Runner> s, starts by adding the first element to a new List<Greyhound>() (this might represent "wimbledon 19:30, 6 runners").  While loops around the elements,
        /// adding each one to the new List<Greyhound>() and then popping it from the stack, until trap 1 is found (signaling the end of this individual race), then we add the new List<Greyhound>() to
        /// the param List<List<Greyhound>> g and return DivideRaces(s, g).  When the stack is empty return the List<List<Greyhound>> g.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="g"></param>
        /// <returns></returns>
        private List<List<Greyhound>> DivideRaces(Stack<Runner> s, List<List<Greyhound>> g)
        {
            if (s.Count() > 0)
            {
                List<Greyhound> l = new List<Greyhound>();
                l.Add(new Greyhound { Name = s.First().Name });
                s.Pop();
                try
                {
                    while (s.First().Trap > 1)
                    {
                        l.Add(new Greyhound { Name = s.First().Name });
                        s.Pop();
                    }
                }
                catch (Exception)
                { }
                g.Add(l);
                return DivideRaces(s, g);
            }
            else
            {
                return g;
            }
            
        }

        /// <summary>
        /// Does the autocomplete, restricting this to only find names of dogs who race at Wimbledon for the moment
        /// </summary>
        /// <param name="searchTerm"></param>
        /// <returns></returns>
        public ActionResult GetAutocompleteNames(Message searchTerm)
        {
            var query = from n in ctx.Greyhounds
                        where n.Name.StartsWith(searchTerm.Content) && n.Track.Name == TRACK
                        select n.Name;

            List<string> returnList = new List<string>(query);

            return Json(returnList);
        }
    }
}
