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
            PopulateTeams(context);
            PopulatePlayers(context);
        }

        public void PopulateChampionships(PPFormazioniContext context)
        {
            Championship c = new Championship
            {
                Name = "Serie A 2016 / 17",
                Year = 2016,
                CurrentMatchDayNumber = 13,
                Teams = new List<Team>(),
                Days = new List<Day>()
            };

            context.Championships.Add(c);

            context.SaveChanges();
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

                    List<Team> teams = json_teams.Select(p => new Team
                    {
                        FullName = (string)p["name"],
                        Name = (string)p["shortName"],
                        MarketValue = (string)p["squadMarketValue"],
                        Logo_URL = (string)p["crestUrl"],
                        Players_URL = (string)p["_links"]["players"]["href"],
                        Players = new List<Player>()
                    }).ToList();

                    teams.ForEach(x => context.Teams.Add(x));

                    teams.ForEach(x => c.Teams.Add(x));

                    context.SaveChanges();
                }
                catch(Exception e)
                {

                }
            }
        }

        public void PopulatePlayers(PPFormazioniContext context)
        {
            List<Team> teams = context.Teams.ToList();

            foreach(Team t in teams)
            {
                using(var client = new WebClient())
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
                            //DateOfBirth = tm["dateOfBirth"] != null ? Convert.ToDateTime(tm["dateOfBirth"]) : new DateTime(),
                            Nationality = (string)tm["nationality"],
                            //ContractUntil = tm["contractUntil"] != null ? Convert.ToDateTime(tm["contractUntil"]) : new DateTime(),
                            MarketValue = (string)tm["marketValue"],
                            MatchesPlayed = new List<Match>()
                        }).ToList();

                        players.ForEach(p => context.Players.Add(p));

                        players.ForEach(p => t.Players.Add(p));

                        
                    }
                    catch(Exception e)
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

        


          
    }
}
