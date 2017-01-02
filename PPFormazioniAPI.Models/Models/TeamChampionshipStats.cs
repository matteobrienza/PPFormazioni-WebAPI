using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPFormazioniAPI.Models
{
    public class TeamChampionshipStats
    {
        public int Id { get; set; }
        public int TeamId { get; set; }
        public int ChampionshipId { get; set; }
        public int PlayedGames { get; set; }
        public int Points { get; set; }
        public int Goals { get; set; }
        public int GoalAgainst { get; set; }
        public int GoalDifference { get; set; }
        public int Wins { get; set; }
        public int Draws { get; set; }
        public int Losses { get; set; }
        public int HomeGoals { get; set; }
        public int HomeGoalsAgainst { get; set; }
        public int HomeWins { get; set; }
        public int HomeDraws { get; set; }
        public int HomeLosses { get; set; }
        public int AwayGoals { get; set; }
        public int AwayGoalsAgainst { get; set; }
        public int AwayWins { get; set; }
        public int AwayDraws { get; set; }
        public int AwayLosses { get; set; }

    }
}
