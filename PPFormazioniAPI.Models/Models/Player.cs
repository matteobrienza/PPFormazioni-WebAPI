﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PPFormazioniAPI.Models
{
    public class Player
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Number { get; set; }
        public string Position { get; set; }
        public string Nationality { get; set; }
        public string MarketValue { get; set; }
        public int TeamId { get; set; }
        public virtual Team Team { get; set; }
        public virtual ICollection<Match> MatchesPlayed { get; set; }
    }
}