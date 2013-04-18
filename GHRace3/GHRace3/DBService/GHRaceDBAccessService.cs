using GHRace3.Models;
using GHRace3.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GHRace3.DBService
{
    public class GHRaceDBAccessService
    {
        private GHRaceContext ctx = new GHRaceContext();

        public List<Greyhound> GetAllGreyhounds()
        {
            return ctx.Greyhounds.ToList();
        }

        /// <summary>
        /// Returns a list of greyhound from a list of greyhound names
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        public List<Greyhound> GetRunnersData(List<string> names)
        {
            List<Greyhound> dogs = new List<Greyhound>();
            foreach (var item in names)
            {
                try
                {
                    dogs.Add(ctx.Greyhounds.Single(gh => gh.Name == item));
                }
                catch (Exception) { }
            }
            return dogs;
        }

        /// <summary>
        /// Add new data to the DB, since duplicate urls are screened out and races already added aren't even downloaded, we only check for existing tracks and existing greyhounds.
        /// If the track is new, just add it.
        /// If the track exists, try and add each greyhound to the track, if no greyhound found for this track, add it.
        /// if the greyhound does exist, just add the results to its results collection.
        /// </summary>
        /// <param name="tracklist"></param>
        public void Add(ICollection<Track> tracklist)
        {
            var tracks = GetGroupedList(tracklist);
            foreach (var track in tracks)
            {
                Track t = GetTrack(track.Name);
                if (t == null) //no such track in DB so add it and all its data (greyhounds and races)
                {
                    ctx.Tracks.Add(track);
                }
                else //track exists
                {
                    foreach (Greyhound greyhound in track.Greyhounds)
                    {
                        Greyhound g = GetGreyhound(greyhound.Name);
                        if (g == null) //greyhound not in DB yet so add it and its races
                        {
                            t.Greyhounds.Add(new Greyhound { Name = greyhound.Name, Results = greyhound.Results, Track = t });
                        }
                        else //greyhound already exists
                        {
                            //add each individual result
                            foreach (Result result in greyhound.Results)
                            {
                                g.Results.Add(result);
                            }
                        }
                    }
                }
            }

            ctx.SaveChanges();
        }

        /// <summary>
        /// Return a single greyhound or null if not found.
        /// TODO: this will need to be a track specific search as some dogs race at > 1 track and we're only interested in a dog's results from ONE track
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public Greyhound GetGreyhound(string p)
        {
            var query = from g in ctx.Greyhounds
                        where g.Name == p
                        select g;
            foreach (var item in query)
            {
                return item as Greyhound;
            }
            return null;
        }

        public Track GetTrack(string p)
        {
            var query = from g in ctx.Tracks
                        where g.Name == p
                        select g;
            foreach (var item in query)
            {
                return item as Track;
            }
            return null;
        }

        /// <summary>
        /// Flattens the tracklist so that each track is unique.  Since the data can be scraped in large batches (e.g. 12 race meets from wimbledon, 4 from Hall Green etc), a dog or dogs may have run in 
        /// 1 wimbledon race meeting or all of them.  To prevent a greyhound being added twice at this stage a greyhound appearing more than once in the track list needs to be removed an its results added
        /// to a single greyhound object:
        /// So the original list:
        /// {"Vegas Gold", Results.Count() = 1} //raced on 12/01/2013
        /// {"Vegas Gold", Results.Count() = 1} //raced on 15/01/2013
        /// {"Vegas Gold", Results.Count() = 1} //raced on 19/01/2013
        /// {"Vegas Gold", Results.Count() = 1} //raced on 29/01/2013
        /// is compressed to:
        /// {"Vegas Gold", Results.Count() = 4} //all results for this dog now in one place.
        /// </summary>
        /// <param name="tracklist"></param>
        /// <returns></returns>
        private List<Track> GetGroupedList(ICollection<Track> tracklist)
        {
            var query = (from n in tracklist
                        select n.Name).Distinct();
            List<string> trackNames = new List<string>(query); //a list of all the new tracks being added
            List<List<Track>> tracksByLocation = new List<List<Track>>();

            foreach (var item in trackNames)
            {
                var tracks = from t in tracklist
                            where t.Name == item
                            select t;
                tracksByLocation.Add(new List<Track>(tracks));
            }
            return GroupAllGreyhounds(tracksByLocation);
        }

        private List<Track> GroupAllGreyhounds(List<List<Track>> tracksByLocation)
        {
            List<Track> flattenedTrackList = new List<Track>();
            foreach (var item in tracksByLocation)
            {
                Dictionary<string, Greyhound> gList = new Dictionary<string, Greyhound>();
                string currentTrackName = "";
                foreach (var track in item)
                {
                    currentTrackName = track.Name;
                    foreach (var g in track.Greyhounds)
                    {
                        if (gList.ContainsKey(g.Name)) // greyhound already added in this session so just add the new races
                        {
                            gList[g.Name].Results.Add(g.Results.First());// can use first() here because a dog doesn't run twice in the same meeting 
                        }                                                // so in the un-formatted collection Results.Count() will always be 1
                        else
                        {
                            gList.Add(g.Name, g);
                        }
                    }
                }
                flattenedTrackList.Add(new Track { Name = currentTrackName, Greyhounds = gList.Values });

            }
            return flattenedTrackList;
        }

        /// <summary>
        /// Checks for the existence of a hash of track name and date in the DB to prevent duplicate resutls being added.
        /// If the hash is not found, add it to the context and return true, else return false.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public bool RaceMeetingNotInDB(string url)
        {
            Meeting m = new Meeting();
            m.SetTrackDateHash(url);
            Meeting meet = ctx.Meetings.Find(m.TrackDateHash);
            if (meet == null)
            {
                ctx.Meetings.Add(m);
                return true;
            }
            return false;
        }

        public int GetTrackCount()
        {
            return ctx.Tracks.Count();
        }

        public int GetGreyhoundCount()
        {
            return ctx.Greyhounds.Count();
        }

        public int GetResultsCount()
        {
            return ctx.Results.Count();
        }

        public int GetRaceMeetingCount()
        {
            return ctx.Meetings.Count();
        }

        /// <summary>
        /// Gets the date of the most recenlty added race meeting for each track.
        /// </summary>
        /// <returns></returns>
        public List<LatestRaceForTrack> GetLatestResultForAllTracks()
        {
            List<LatestRaceForTrack> list = new List<LatestRaceForTrack>();
            foreach (var item in ctx.Tracks)
            {
                var query = (from r in ctx.Results
                             where r.Greyhound.Track.Name == item.Name
                             select r).OrderByDescending(r => r.Date);
                foreach (var result in query)
                {
                    list.Add(new LatestRaceForTrack { DateofLatestRace = result.Date, TrackName = item.Name });
                    break;
                }
            }
            return list;
        }

    }
}