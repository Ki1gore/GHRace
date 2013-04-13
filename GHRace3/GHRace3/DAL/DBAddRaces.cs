using GHRace3.Models;
using Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GHRace3.DAL
{
    public class DBAddRaces
    {
        private GHRaceContext ctx = new GHRaceContext();

        public void Add(ICollection<Track> tracklist)
        {
            foreach (var track in tracklist)
            {
                if (DataNotPresent(track))
                {
                    Track t = null;
                    if (ctx.Tracks.Count() > 0)
                    {
                        try
                        {
                            t = (from Track tr in ctx.Tracks
                                 where tr.Name == track.Name
                                 select tr).Single();
                        }
                        catch (Exception)
                        {
                            
                        }
                        
                    }

                    if (t == null) //no existing track
                    {
                        ctx.Tracks.Add(track);
                    }
                    else //already have this track
                    {
                        foreach (var greyhound in track.Greyhounds)
                        {
                            //Greyhound gh = ctx.Greyhounds.Find(greyhound);
                            var gh = (from g in ctx.Greyhounds
                                      where g.Name == greyhound.Name
                                      select g);
                            if (gh.Count() < 1) //new dog so add all races
                            {
                                t.Greyhounds.Add(new Greyhound
                                {
                                    Name = greyhound.Name,
                                    Races = greyhound.Races
                                });
                            }
                            else //found existing dog so add the races to its races collection
                            {
                                foreach (Greyhound item in gh)
                                {
                                    foreach (var r in greyhound.Races)
                                    {
                                        item.Races.Add(r);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            ctx.SaveChanges();
        }

        private bool DataNotPresent(Track track)
        {
            Meeting m = new Meeting();
            m.SetTrackDateHash(track);
            Meeting meet = ctx.Meetings.Find(m.TrackDateHash);
            if (meet == null) //race meeting not in DB
            {
                ctx.Meetings.Add(m);
                return true;
            }
            return false;
        }
    }
}