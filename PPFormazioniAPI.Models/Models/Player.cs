using System;
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
        public string DateOfBirth { get; set; }
        public string ContractUntil { get; set; }
        public int TeamId { get; set; }

        public string getMacroRole()
        {
            if (Position == null) return "";
            if (Position.Contains("Keeper")) return "keeper";
            if (Position.Contains("Back")) return "defender";
            if (Position.Contains("Midfield") || Position.Contains("Wing")) return "midfilder";
            return "striker";
        }
    }
}
