﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPFormazioniAPI.Models
{
    public class TeamNewspaperReliability
    {
        public int Id { get; set; }
        public int TeamId { get; set; }
        public int NewspaperId { get; set; }
        public int Reliability { get; set; }
    }
}
