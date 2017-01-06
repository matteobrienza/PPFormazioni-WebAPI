using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PPFormazioniAPI.Models
{
    public class Team
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public string MarketValue { get; set; }
        public string Logo_URL { get; set; }
        public string Players_URL { get; set; }
        public int ChampionshipId { get; set; }
        public int CoachId { get; set; }
        public virtual Coach Coach { get; set; }
        public virtual ICollection<Player> Players { get; set; }
        
    }
}
