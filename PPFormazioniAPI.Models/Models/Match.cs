using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PPFormazioniAPI.Models
{
    public class Match
    {
        public int Id { get; set; }
        [ForeignKey("HomeTeam"), Column(Order = 0)]
        public int HomeTeamId { get; set; }
        [ForeignKey("AwayTeam"), Column(Order = 1)]
        public int AwayTeamId { get; set; }
        public int DayId { get; set; }
        public string MatchDate { get; set; }
        public virtual Team HomeTeam { get; set; }
        public virtual Team AwayTeam { get; set; }
        public virtual ICollection<PlayerMatch> Players { get; set; }

    }
}
