using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Net.Http;
using System.Net;
using PPFormazioniAPI.Models;
using PPFormazioniAPI;
using PPFormazioniAPI.DAL;

namespace PPFormazioniAPI.DataImporter
{
    class Program
    {
        static void Main(string[] args)
        {

            PPFormazioniContext dbcontext = new PPFormazioniContext("Data Source=MATTEOBRIENZA;Initial Catalog=PPFormazioni;Integrated Security=True;MultipleActiveResultSets=True;");

            GetTeamsFromGazzetta(dbcontext);

            Console.ReadLine();
        }

        public static void GetTeamsFromGazzetta(PPFormazioniContext dbcontext)
        {
            try
            {
                HttpClient http = new HttpClient();

                var response = http.GetByteArrayAsync(Constants.PF_GAZZETTA_URL);

                String source = Encoding.GetEncoding("utf-8").GetString(response.Result, 0, response.Result.Length - 1);

                source = WebUtility.HtmlDecode(source);

                HtmlDocument res = new HtmlDocument();

                res.LoadHtml(source);

                var matches = res.DocumentNode.Descendants("div").Where(d => d.Attributes.Contains("class") && d.Attributes["class"].Value.Contains("matchFieldContainer"));

                int i = 1;

                Day day = new Day
                {
                    ChampionshipId = 1,
                    Number = 17,
                    Matches = new List<Match>()
                };

                dbcontext.Days.Add(day);
                dbcontext.SaveChanges();

                foreach (var m in matches)
                {

                    var matchInfo = m.SelectSingleNode("descendant::div[@class='match']");

                    String homeTeamName = matchInfo.SelectSingleNode("descendant::div[@class='team homeTeam']").SelectSingleNode("descendant::span[@class='teamName']").FirstChild.InnerText;

                    String awayTeamName = matchInfo.SelectSingleNode("descendant::div[@class='team awayTeam']").SelectSingleNode("descendant::span[@class='teamName']").FirstChild.InnerText;

                    Console.Write("Match: " + i++ + " " + homeTeamName.ToUpper() + " - " + awayTeamName.ToUpper());

                    var teamPlayersContainers = m.SelectSingleNode("descendant::div[@class='matchFieldInner']").Descendants("ul").Where(d => d.Attributes.Contains("class") && d.Attributes["class"].Value.Contains("team-players")).Skip(1);

                    Team homeTeam = dbcontext.Teams.Where(t => t.FullName.Contains(homeTeamName)).FirstOrDefault();


                    Team awayTeam = dbcontext.Teams.Where(t => t.FullName.Contains(awayTeamName)).FirstOrDefault();

                    Match match = new Match
                    {
                        HomeTeamId = homeTeam.Id,
                        AwayTeamId = awayTeam.Id,
                        DayId = day.Id,
                        Players = new List<Player>()
                    };

                    dbcontext.Matches.Add(match);

                    day.Matches.Add(match);

                    dbcontext.SaveChanges();

                    int teamIndex = 0;
                    foreach (var tps in teamPlayersContainers)
                    {
                        Console.WriteLine("\n");
                        var playersList = tps.Descendants("li");
                        foreach (var p in playersList)
                        {
                            string playerName = p.SelectSingleNode("descendant::span[@class='team-player']").InnerText;
                            string playerNumber = p.SelectSingleNode("descendant::span[@class='numero']").InnerText;

                            Player player = null;

                            if (teamIndex == 0)
                            {
                                //HOME TEAM
                                player = dbcontext.Players.Where(pl => pl.Number.Equals(playerNumber) && pl.Team.Id == homeTeam.Id).FirstOrDefault();
                            }else
                            {
                                //AWAY TEAM
                                player = dbcontext.Players.Where(pl => pl.Number.Equals(playerNumber) && pl.Team.Id == awayTeam.Id).FirstOrDefault();
                            }
                            

                            if (player != null)
                            {
                                match.Players.Add(player);
                                Console.WriteLine(playerName + " " + playerNumber);
                            }
                            else Console.WriteLine(playerName + " " + playerNumber + "not finded!");


                        }
                        teamIndex++;

                    }

                    Console.WriteLine("\n");
                }


                //dbcontext.Days.Add(day);
                dbcontext.SaveChanges();
            }
            catch(Exception e)
            {

            }
            
        }

        //public static void GetTeamsFromCorriereDelloSport(PPFormazioniContext dbcontext)
        //{
        //    HttpClient http = new HttpClient();

        //    var response = http.GetByteArrayAsync("http://www.gazzetta.it/Calcio/prob_form/");

        //    String source = Encoding.GetEncoding("utf-8").GetString(response.Result, 0, response.Result.Length - 1);

        //    source = WebUtility.HtmlDecode(source);

        //    HtmlDocument res = new HtmlDocument();

        //    res.LoadHtml(source);
        //}
    }


}
