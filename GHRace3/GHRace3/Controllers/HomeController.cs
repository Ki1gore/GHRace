using Autofac;
using GHRace3.DAL;
using GHRace3.dummyScoreCalc;
using GHRace3.Models;
using GHRace3.ViewModels;
using Interfaces;
using System;
using System.Collections.Generic;
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
        

        public ActionResult Index()
        { 
            ViewBag.Message = "Show home page";

            return View();
        }

        public ActionResult About()
        {
            //string seedURL2 = @"http://www.thedogs.co.uk/resultsMeeting.aspx?racedate=16/03/2013%2000:00:00&track=Wimbledon";
            ////string seedURL = @"http://www.thedogs.co.uk/resultsMeeting.aspx?racedate=23/03/2013%2000:00:00&track=Wimbledon";
            //IContainer _container = MvcApplication.Container;
            //IRetrieveRaceData _seedData = _container.Resolve<IRetrieveRaceData>();

            ////string htmlString;
            //string htmlString2;
            //using (WebClient wb = new WebClient())
            //{
            //    htmlString2 = wb.DownloadString(new Uri(seedURL2));
            //}

            //List<string> p = new List<string>();
            //p.Add(htmlString2);
            ////p.Add(htmlString2);
            //ICollection<ITrack> trackData = _seedData.GetData(p);
            //List<Track> tracks = new List<Track>();
            //DBAddRaces dba = new DBAddRaces();
            //foreach (var item in trackData)
            //{
            //    tracks.Add(ModelConverter.ConvertToTrack(item));
            //    //_ctx.Tracks.Add(ModelConverter.ConvertToTrack(item));
            //}
            //dba.Add(tracks);

            var temp = ctx.Tracks.ToList();

            foreach (Track item in temp)
            {

            }

            return View(temp);
        }

        public ActionResult Load(UrlString urlList)
        {
            if (urlList.Url == null)
            {
                return View();
            }
           
            List<RaceViewModel> rvmList = new List<RaceViewModel>(); //list of each individual race (ie 19:30 (race1), 19:45 (race2), etc)
            string[] urls = urlList.Url.Replace('\r', ' ').Trim().Split('\n');

            IContainer _container = MvcApplication.Container;
            IRetrieveRaceData _seedData = _container.Resolve<IRetrieveRaceData>();
            Dictionary<string, string> p = new Dictionary<string, string>();

            using (WebClient wb = new WebClient())
            {
                foreach (var url in urls.Where(u => u.Contains("http")))
                {
                    p.Add(url, wb.DownloadString(new Uri(url)));
                }
            }

            //p.Add(htmlString2);
            ICollection<ITrack> trackData = _seedData.GetData(p);
            List<Track> tracks = new List<Track>();
            DBAddRaces dba = new DBAddRaces();
            foreach (var item in trackData)
            {
                tracks.Add(ModelConverter.ConvertToTrack(item));
            }
            //dba.Add(tracks);
            foreach (var item in tracks)
            {
                foreach (var g in item.Greyhounds)
                {
                    rvmList.Add(new RaceViewModel
                    {
                        RaceDate = g.Races.First().Date.ToString(),
                        Trackname = item.Name,
                        RaceDistance = g.Races.First().RaceDistance.ToString(),
                        Comments = g.Races.First().Comments,
                        Grade = g.Races.First().Grade,
                        Finish = g.Races.First().Finish.ToString(),
                        GreyhoundName = g.Name,
                        SP1 = g.Races.First().SP1.ToString(),
                        SP2 = g.Races.First().SP2.ToString(),
                        SP3 = g.Races.First().SP3,
                        TimeDistance1 = g.Races.First().TimeDistance1.ToString(),
                        TimeDistance2 = g.Races.First().TimeDistance2.ToString(),
                        TimeDistance3 = g.Races.First().TimeDistance3,
                        Details = g.Races.First().Details,
                        TimeSec = g.Races.First().TimeSec.ToString(),
                        Trainer = g.Races.First().Trainer,
                        Trap = g.Races.First().Trap.ToString()
                    });
                }
            }
            return View(rvmList);
            //return null;
        }

        public ActionResult Predict()
        {
            return View();
        }

        public ActionResult GetPredictions(List<Runner> runners)
        {
            //TODO: check runners for more than one race so that multiple races can be predicted
            List<List<Greyhound>> allRunners = new List<List<Greyhound>>();
            List<Greyhound> singleRace = new List<Greyhound>();
            int count = 1;
            //foreach (var item in runners)
            while (true)
            {
                {
                    try
                    {
                        //Greyhound g = ctx.Greyhounds.Single(gh => gh.Name == item.Name);
                        Greyhound g = ctx.Greyhounds.Single(gh => gh.GreyhoundID == count);
                        singleRace.Add(g);
                    }
                    catch (Exception)
                    {
                        singleRace.Add(new Greyhound());
                    }
                    count++;
                    if (count == 7)
                    {
                        break;
                    }
                }
            }
           
            allRunners.Add(singleRace);
            scoreCalc sc = new scoreCalc();
            return Json(sc.GetScores(allRunners));
        }

        public ActionResult GetAutocompleteNames(Message searchTerm)
        {
            var query = from n in ctx.Greyhounds
                        where n.Name.StartsWith(searchTerm.Content)
                        select n.Name;

            List<string> returnList = new List<string>(query);

            return Json(returnList);
        }
    }
}
