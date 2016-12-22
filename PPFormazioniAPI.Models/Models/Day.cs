using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PPFormazioniAPI.Models
{
    public class Day
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public int ChampionshipId { get; set; }
        public virtual Championship Championship{ get; set; }
        public virtual ICollection<Match> Matches { get; set; }
    }
}
