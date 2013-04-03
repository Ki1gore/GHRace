using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;


namespace GHRace3.Models
{
    public class GHRaceContext : DbContext
    {
        public DbSet<Track> Tracks { get; set; }
        public DbSet<Greyhound> Greyhounds { get; set; }
        public DbSet<Race> Races { get; set; }
        public DbSet<Meeting> Meetings { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}