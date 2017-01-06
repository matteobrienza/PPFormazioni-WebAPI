using Newtonsoft.Json.Linq;
using PPFormazioniAPI.Helpers;
using PPFormazioniAPI.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace PPFormazioniAPI.DAL
{
    public class PPFormazioniDatabaseInitializer : DropCreateDatabaseAlways<PPFormazioniContext>
    {
        protected override void Seed(PPFormazioniContext context)
        {
            PopulateChampionships(context);
            PopulateCoaches(context);
            PopulateTeams(context);
            PopulatePlayers(context);
            PopulateNewspaper(context);
            PopulateTeamNewspaperReliability(context);
            PopulateStanding(context);
        }
        

        public void PopulateNewspaper(PPFormazioniContext context)
        {
            var newspapers = new List<Newspaper>
            {
                new Newspaper { Name = "Gazzetta dello Sport", Website_URL = "http://www.gazzetta.it/Calcio/prob_form/", ChampionshipId=1},
                new Newspaper { Name = "Corriere dello Sport", Website_URL = "http://www.corrieredellosport.it/calcio/serie-a/probabili-formazioni/", ChampionshipId=1 },
                new Newspaper { Name = "Sky Sport", Website_URL = "http://sport.sky.it/calcio/serie-a/probabili-formazioni/", ChampionshipId=1 }
            };

            newspapers.ForEach(n => context.Newspapers.Add(n));
            context.SaveChanges();
        }

        public void PopulateTeamNewspaperReliability(PPFormazioniContext context)
        {
            var newspaperRels = new List<TeamNewspaperReliability>
            {
                #region GAZZETTA DELLO SPORT
                new TeamNewspaperReliability { NewspaperId = 1, TeamId=1,  Reliability=2  },
                new TeamNewspaperReliability { NewspaperId = 1, TeamId=2,  Reliability=2  },
                new TeamNewspaperReliability { NewspaperId = 1, TeamId=3,  Reliability=3  },
                new TeamNewspaperReliability { NewspaperId = 1, TeamId=4,  Reliability=1  },
                new TeamNewspaperReliability { NewspaperId = 1, TeamId=5,  Reliability=3  },
                new TeamNewspaperReliability { NewspaperId = 1, TeamId=6,  Reliability=2  },
                new TeamNewspaperReliability { NewspaperId = 1, TeamId=7,  Reliability=1  },
                new TeamNewspaperReliability { NewspaperId = 1, TeamId=8,  Reliability=3  },
                new TeamNewspaperReliability { NewspaperId = 1, TeamId=9,  Reliability=1  },
                new TeamNewspaperReliability { NewspaperId = 1, TeamId=10, Reliability=2  },
                new TeamNewspaperReliability { NewspaperId = 1, TeamId=11, Reliability=2  },
                new TeamNewspaperReliability { NewspaperId = 1, TeamId=12, Reliability=1  },
                new TeamNewspaperReliability { NewspaperId = 1, TeamId=13, Reliability=1  },
                new TeamNewspaperReliability { NewspaperId = 1, TeamId=14, Reliability=1  },
                new TeamNewspaperReliability { NewspaperId = 1, TeamId=15, Reliability=1  },
                new TeamNewspaperReliability { NewspaperId = 1, TeamId=16, Reliability=1  },
                new TeamNewspaperReliability { NewspaperId = 1, TeamId=17, Reliability=1  },
                new TeamNewspaperReliability { NewspaperId = 1, TeamId=18, Reliability=1  },
                new TeamNewspaperReliability { NewspaperId = 1, TeamId=19, Reliability=2  },
                new TeamNewspaperReliability { NewspaperId = 1, TeamId=20, Reliability=2  },
                #endregion  

                #region CORRIERE DELLO SPORT
                new TeamNewspaperReliability { NewspaperId = 2, TeamId=1,  Reliability=3  },
                new TeamNewspaperReliability { NewspaperId = 2, TeamId=2,  Reliability=1  },
                new TeamNewspaperReliability { NewspaperId = 2, TeamId=3,  Reliability=2  },
                new TeamNewspaperReliability { NewspaperId = 2, TeamId=4,  Reliability=2  },
                new TeamNewspaperReliability { NewspaperId = 2, TeamId=5,  Reliability=2  },
                new TeamNewspaperReliability { NewspaperId = 2, TeamId=6,  Reliability=1  },
                new TeamNewspaperReliability { NewspaperId = 2, TeamId=7,  Reliability=1  },
                new TeamNewspaperReliability { NewspaperId = 2, TeamId=8,  Reliability=2  },
                new TeamNewspaperReliability { NewspaperId = 2, TeamId=9,  Reliability=1  },
                new TeamNewspaperReliability { NewspaperId = 2, TeamId=10, Reliability=1  },
                new TeamNewspaperReliability { NewspaperId = 2, TeamId=11, Reliability=1  },
                new TeamNewspaperReliability { NewspaperId = 2, TeamId=12, Reliability=1  },
                new TeamNewspaperReliability { NewspaperId = 2, TeamId=13, Reliability=1  },
                new TeamNewspaperReliability { NewspaperId = 2, TeamId=14, Reliability=1  },
                new TeamNewspaperReliability { NewspaperId = 2, TeamId=15, Reliability=1  },
                new TeamNewspaperReliability { NewspaperId = 2, TeamId=16, Reliability=1  },
                new TeamNewspaperReliability { NewspaperId = 2, TeamId=17, Reliability=1  },
                new TeamNewspaperReliability { NewspaperId = 2, TeamId=18, Reliability=2  },
                new TeamNewspaperReliability { NewspaperId = 2, TeamId=19, Reliability=1  },
                new TeamNewspaperReliability { NewspaperId = 2, TeamId=20, Reliability=3  },
                #endregion  

                #region SKY SPORT
                new TeamNewspaperReliability { NewspaperId = 3, TeamId=1,  Reliability=3  },
                new TeamNewspaperReliability { NewspaperId = 3, TeamId=2,  Reliability=1  },
                new TeamNewspaperReliability { NewspaperId = 3, TeamId=3,  Reliability=3  },
                new TeamNewspaperReliability { NewspaperId = 3, TeamId=4,  Reliability=3  },
                new TeamNewspaperReliability { NewspaperId = 3, TeamId=5,  Reliability=3  },
                new TeamNewspaperReliability { NewspaperId = 3, TeamId=6,  Reliability=2  },
                new TeamNewspaperReliability { NewspaperId = 3, TeamId=7,  Reliability=1  },
                new TeamNewspaperReliability { NewspaperId = 3, TeamId=8,  Reliability=3  },
                new TeamNewspaperReliability { NewspaperId = 3, TeamId=9,  Reliability=1  },
                new TeamNewspaperReliability { NewspaperId = 3, TeamId=10, Reliability=2  },
                new TeamNewspaperReliability { NewspaperId = 3, TeamId=11, Reliability=2  },
                new TeamNewspaperReliability { NewspaperId = 3, TeamId=12, Reliability=1  },
                new TeamNewspaperReliability { NewspaperId = 3, TeamId=13, Reliability=1  },
                new TeamNewspaperReliability { NewspaperId = 3, TeamId=14, Reliability=1  },
                new TeamNewspaperReliability { NewspaperId = 3, TeamId=15, Reliability=1  },
                new TeamNewspaperReliability { NewspaperId = 3, TeamId=16, Reliability=2  },
                new TeamNewspaperReliability { NewspaperId = 3, TeamId=17, Reliability=1  },
                new TeamNewspaperReliability { NewspaperId = 3, TeamId=18, Reliability=3  },
                new TeamNewspaperReliability { NewspaperId = 3, TeamId=19, Reliability=2  },
                new TeamNewspaperReliability { NewspaperId = 3, TeamId=20, Reliability=3  },
                #endregion  
            };

            newspaperRels.ForEach(n => context.TeamNewspaperReliabilities.Add(n));
            context.SaveChanges();
        }

        public void PopulateChampionships(PPFormazioniContext context)
        {
            using (var client = new WebClient())
            {
                try
                {
                    client.Headers.Add("X-Auth-Token", Constants.API_TOKEN);
                    client.Headers.Add("Content-Type", "application/json;charset=UTF-8");

                    JObject response = JObject.Parse(client.DownloadString(Constants.COMPETITION_URL));

                    Championship c = new Championship
                    {
                        Name = (string)response["caption"],
                        Year = (int)response["year"],
                        CurrentMatchDayNumber = (int)response["currentMatchday"],
                        TotalMatchDaysNumber = (int)response["numberOfMatchdays"],
                        Teams = new List<Team>(),
                        Days = new List<Day>()
                    };

                    context.Championships.Add(c);

                    context.SaveChanges();
                }
                catch (Exception e)
                {

                }
            }


        }

        public void PopulateTeams(PPFormazioniContext context)
        {
            Championship c = context.Championships.Where(x => x.Id == 1).FirstOrDefault();

            using (var client = new WebClient())
            {
                try
                {
                    client.Headers.Add("X-Auth-Token", Constants.API_TOKEN);
                    client.Headers.Add("Content-Type", "application/json;charset=UTF-8");

                    JObject response = JObject.Parse(client.DownloadString(Constants.TEAMS_URL));

                    JArray json_teams = JArray.Parse(response["teams"].ToString());

                    int i = 1;

                    List<Team> teams = json_teams.Select(p => new Team
                    {
                        FullName = (string)p["name"],
                        Name = (string)p["shortName"],
                        MarketValue = (string)p["squadMarketValue"],
                        Logo_URL = (string)p["crestUrl"],
                        Players_URL = (string)p["_links"]["players"]["href"],
                        Players = new List<Player>(),
                        ChampionshipId = c.Id,
                        CoachId = i++
                    }).ToList();

                    teams.ForEach(x => context.Teams.Add(x));

                    teams.ForEach(x => c.Teams.Add(x));

                    context.SaveChanges();
                }
                catch (Exception e)
                {

                }
            }
        }

        public void PopulatePlayers(PPFormazioniContext context)
        {
            List<Team> teams = context.Teams.ToList();

            foreach (Team t in teams)
            {
                using (var client = new WebClient())
                {
                    try
                    {
                        client.Headers.Add("X-Auth-Token", Constants.API_TOKEN);
                        client.Headers.Add("Content-Type", "application/json;charset=UTF-8");

                        JObject response = JObject.Parse(client.DownloadString(t.Players_URL));

                        JArray json_players = JArray.Parse(response["players"].ToString());

                        List<Player> players = json_players.Select(tm => new Player
                        {
                            Name = (string)tm["name"],
                            Position = (string)tm["position"],
                            Number = Convert.ToString(tm["jerseyNumber"]),
                            DateOfBirth = Convert.ToString(tm["dateOfBirth"]),
                            Nationality = (string)tm["nationality"],
                            ContractUntil = Convert.ToString(tm["contractUntil"]),
                            MarketValue = (string)tm["marketValue"]
                        }).ToList();

                        players.ForEach(p => context.Players.Add(p));

                        players.ForEach(p => t.Players.Add(p));


                    }
                    catch (Exception e)
                    {

                    }



                }
            }

            try
            {
                context.SaveChanges();
            }
            catch (Exception e)
            {

            }


        }

        public void PopulateStanding(PPFormazioniContext context)
        {
            Championship c = context.Championships.Where(x => x.Id == 1).FirstOrDefault();

            using (var client = new WebClient())
            {
                try
                {
                    client.Headers.Add("X-Auth-Token", Constants.API_TOKEN);
                    client.Headers.Add("Content-Type", "application/json;charset=UTF-8");

                    JObject response = JObject.Parse(client.DownloadString(Constants.STANDING_URL));

                    JArray standing = JArray.Parse(response["standing"].ToString());

                    List<TeamChampionshipStats> tcstats = standing.Select(p => new TeamChampionshipStats
                    {
                        ChampionshipId = c.Id,
                        TeamId = GetTeamId((string)p["teamName"], context),
                        PlayedGames = (int)p["playedGames"],
                        Points = (int)p["points"],
                        Goals = (int)p["goals"],
                        GoalAgainst = (int)p["goalsAgainst"],
                        GoalDifference = (int)p["goalDifference"],
                        Wins = (int)p["wins"],
                        Draws = (int)p["draws"],
                        Losses = (int)p["losses"],
                        HomeGoals = (int)p["home"]["goals"],
                        HomeGoalsAgainst = (int)p["home"]["goalsAgainst"],
                        HomeWins = (int)p["home"]["wins"],
                        HomeDraws = (int)p["home"]["draws"],
                        HomeLosses = (int)p["home"]["losses"],
                        AwayGoals = (int)p["away"]["goals"],
                        AwayGoalsAgainst = (int)p["away"]["goalsAgainst"],
                        AwayWins = (int)p["away"]["wins"],
                        AwayDraws = (int)p["away"]["draws"],
                        AwayLosses = (int)p["away"]["losses"]
                    }).ToList();

                    tcstats.ForEach(x => context.TeamsChampionshipStats.Add(x));

                    tcstats.ForEach(x => c.TeamsChampionshipStats.Add(x));

                    context.SaveChanges();
                }
                catch (Exception e)
                {

                }
            }
        }

        public int GetTeamId(string TeamName, PPFormazioniContext context)
        {
            return context.Teams.Where(t => t.FullName.Contains(TeamName)).FirstOrDefault().Id;
        }

        public void PopulateCoaches(PPFormazioniContext context)
        {
            try
            {
                var coaches = new List<Coach>
            {
                new Coach { Name="Luciano", Surname="Spalletti", TeamId=1},
                new Coach { Name="Luigi", Surname="Delneri", TeamId=2},
                new Coach { Name="Massimiliano", Surname="Allegri", TeamId=3},
                new Coach { Name="Paulo", Surname="Sousa", TeamId=4},
                new Coach { Name="Vincenzo", Surname="Montella", TeamId=5},
                new Coach { Name="Sinisa", Surname="Mihajlovic", TeamId=6},
                new Coach { Name="Rolando", Surname="Maran", TeamId=7},
                new Coach { Name="Stefano", Surname="Pioli", TeamId=8},
                new Coach { Name="Giovanni", Surname="Martusciello", TeamId=9},
                new Coach { Name="Marco", Surname="Giampaolo", TeamId=10},
                new Coach { Name="Ivan", Surname="Jurić", TeamId=11},
                new Coach { Name="Massimo", Surname="Rastelli", TeamId=12},
                new Coach { Name="Roberto", Surname="Donadoni", TeamId=13},
                new Coach { Name="Davide", Surname="Nicola", TeamId=14},
                new Coach { Name="Eugenio", Surname="Corini", TeamId=15},
                new Coach { Name="Eusebio", Surname="Di Francesco", TeamId=16},
                new Coach { Name="Massimo", Surname="Oddo", TeamId=17},
                new Coach { Name="Maurizio", Surname="Sarri", TeamId=18},
                new Coach { Name="Gian Piero", Surname="Gasperini", TeamId=19},
                new Coach { Name="Simone", Surname="Inzaghi", TeamId=20}
            };

                coaches.ForEach(c => context.Coaches.Add(c));
                context.SaveChanges();
            }
            catch(Exception e)
            {

            }
            
        }

    }
}
