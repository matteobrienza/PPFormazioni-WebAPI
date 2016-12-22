using PPFormazioniAPI.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Threading.Tasks;

namespace PPFormazioniAPI.DAL
{

    //[DbConfigurationType(typeof(CodeConfig))]
    public class PPFormazioniContext: DbContext
    {

        public PPFormazioniContext(string connectionString) : base(connectionString)
        {
            try
            {
                Database.SetInitializer<PPFormazioniContext>(new PPFormazioniDatabaseInitializer());
                Database.Initialize(true);
            }catch(Exception e)
            {

            }
            
        }


        public DbSet<Match> Matches { get; set; }
        public DbSet<Championship> Championships { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Day> Days { get; set; }



        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            modelBuilder.Entity<Player>()
             .HasMany<Match>(p => p.MatchesPlayed)
             .WithMany(m => m.Players)
             .Map(cs =>
             {
                 cs.MapLeftKey("PlayerRefId");
                 cs.MapRightKey("MatchRefId");
                 cs.ToTable("PlayerMatches");
             });

        }
        
    }
}
