using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PPFormazioniAPI.Models
{
    public class Championship
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Year { get; set; }
        public int CurrentMatchDayNumber { get; set; }
        public int TotalMatchDaysNumber { get; set; }
        public virtual ICollection<Day> Days { get; set; }
        public virtual ICollection<Team> Teams { get; set; }
        public virtual ICollection<TeamChampionshipStats> TeamsChampionshipStats { get; set; }
    }
}
