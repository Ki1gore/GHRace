using Autofac;
using GHRace3.Models;
using Interfaces;
using Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace GHRace3.DBService
{
    /// <summary>
    /// A class used to seed the DB with data - probably not needed since the DB can be seeded from the Load page.
    /// Left here as a reference.
    /// </summary>
    public class GHRaceInitializer : CreateDatabaseIfNotExists<GHRaceContext>
    {
        IContainer _container;
        IRetrieveRaceData _seedData;
        GHRaceContext _ctx;

        protected override void Seed(GHRaceContext context)
        {
            GHRaceDBAccessService dba = new GHRaceDBAccessService();
            //AddData();
            string seedURL = @"http://www.thedogs.co.uk/resultsMeeting.aspx?racedate=23/03/2013%2000:00:00&track=Wimbledon";
            //string seedURL2 = @"http://www.thedogs.co.uk/resultsMeeting.aspx?racedate=16/03/2013%2000:00:00&track=Wimbledon";

            _container = MvcApplication.Container;
            _seedData = _container.Resolve<IRetrieveRaceData>();
            _ctx = context;
            string htmlString = "";
            //string htmlString2;
            using (WebClient wb = new WebClient())
            {
                if (dba.RaceMeetingNotInDB(seedURL))
                {
                    htmlString = wb.DownloadString(new Uri(seedURL));
                }
            }

            Dictionary<string, string> p = new Dictionary<string, string>();
            p.Add(seedURL, htmlString);
            //p.Add(htmlString2);
            ICollection<ITrack> trackData = _seedData.GetData(p);
            List<Track> tracks = new List<Track>();
            
            foreach (var item in trackData)
            {
                tracks.Add(ModelConverter.ConvertToTrack(item));
                //_ctx.Tracks.Add(ModelConverter.ConvertToTrack(item));
            }
            dba.Add(tracks);
        }

    }

}