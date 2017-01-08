using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PPFormazioniAPI.Helpers
{
    public static class Constants
    {
        public const string CHAMPIONSHIP_URL = "";

        public const string TEAMS_URL = "http://api.football-data.org/v1/competitions/438/teams";

        public const string COMPETITION_URL = "http://api.football-data.org/v1/competitions/438";

        public const string STANDING_URL = "http://api.football-data.org/v1/competitions/438/leagueTable";

        public const string API_TOKEN = "924265ff273346e78ae336f3c6780a86";

        public static Dictionary<int, string> LogoUrls = new Dictionary<int, string>
        {
            { 1, "https://ppformazioni.000webhostapp.com/Roma.png"},
            { 2, "https://ppformazioni.000webhostapp.com/Udinese.png"},
            { 3, "https://ppformazioni.000webhostapp.com/Juventus.png"},
            { 4, "https://ppformazioni.000webhostapp.com/Fiorentina.png"},
            { 5, "https://ppformazioni.000webhostapp.com/Milan.png"},
            { 6, "https://ppformazioni.000webhostapp.com/Torino.png"},
            { 7, "https://ppformazioni.000webhostapp.com/Chievo.png"},
            { 8, "https://ppformazioni.000webhostapp.com/Inter.png"},
            { 9, "https://ppformazioni.000webhostapp.com/Empoli.png"},
            { 10, "https://ppformazioni.000webhostapp.com/Sampdoria.png"},
            { 11, "https://ppformazioni.000webhostapp.com/Genoa.png"},
            { 12, "https://ppformazioni.000webhostapp.com/Cagliari.png"},
            { 13, "https://ppformazioni.000webhostapp.com/Bologna.png"},
            { 14, "https://ppformazioni.000webhostapp.com/Crotone.png"},
            { 15, "https://ppformazioni.000webhostapp.com/Palermo.png"},
            { 16, "https://ppformazioni.000webhostapp.com/Sassuolo.png"},
            { 17, "https://ppformazioni.000webhostapp.com/Pescara.png"},
            { 18, "https://ppformazioni.000webhostapp.com/Napoli.png"},
            { 19, "https://ppformazioni.000webhostapp.com/Atalanta.png"},
            { 20, "https://ppformazioni.000webhostapp.com/Lazio.png"}
        };

    }
}
