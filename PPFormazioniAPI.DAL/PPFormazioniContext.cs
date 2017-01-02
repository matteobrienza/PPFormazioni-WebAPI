using PPFormazioniAPI.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Threading.Tasks;

namespace PPFormazioniAPI.DAL
{
    
    public class PPFormazioniContext: DbContext
    {

        public PPFormazioniContext(string connectionString) : base(connectionString)
        {
            try
            {
                Database.SetInitializer<PPFormazioniContext>(new PPFormazioniDatabaseInitializer());
                //Database.Initialize(true);
            }catch(Exception e)
            {

            }
            
        }


        public DbSet<Match> Matches { get; set; }
        public DbSet<Championship> Championships { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Day> Days { get; set; }
        public DbSet<Newspaper> Newspapers { get; set; }
        public DbSet<TeamNewspaperReliability> TeamNewspaperReliabilities { get; set; }
        public DbSet<PlayerMatch> PlayerMatches { get; set; }
        public DbSet<Coach> Coaches { get; set; }
        public DbSet<TeamChampionshipStats> TeamsChampionshipStats { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }
        
    }
}
