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
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace PPFormazioniAPI.DataImporter
{
    class Program
    {

        static void Main(string[] args)
        {

            PPFormazioniContext dbcontext = new PPFormazioniContext("Data Source=MATTEOBRIENZA;Initial Catalog=PPFormazioni;Integrated Security=True;MultipleActiveResultSets=True;");

            CreateCurrentDay(dbcontext);

            //CLEAR PLAYERMATCHES
            foreach (var pm in dbcontext.PlayerMatches)
                dbcontext.PlayerMatches.Remove(pm);
            dbcontext.SaveChanges();

            GetTeamsFromGazzetta(dbcontext);

            //GetTeamsFromCorriereDelloSport(dbcontext);

            GetTeamsFromSkySport(dbcontext);

            //NotifyClient(dbcontext);

            Console.ReadLine();
        }
        public static void CreateCurrentDay(PPFormazioniContext context)
        {
            Championship c = context.Championships.Where(ch => ch.Id == 1).FirstOrDefault();
            Day day = context.Days.Where(d => d.Number == c.CurrentMatchDayNumber).FirstOrDefault();
            if (day == null)
            {
                day = new Day
                {
                    ChampionshipId = 1,
                    Number = c.CurrentMatchDayNumber,
                    Matches = new List<Models.Match>()
                };

                context.Days.Add(day);
                context.SaveChanges();
            }

        }

        public static void GetTeamsFromGazzetta(PPFormazioniContext dbcontext)
        {
            Console.WriteLine("GetTeamsFrom__GazzettaDelloSport....");
            try
            {
                Championship c = dbcontext.Championships.Where(ch => ch.Id == 1).FirstOrDefault();

                HttpClient http = new HttpClient();

                var response = http.GetByteArrayAsync(Constants.PF_GAZZETTA_URL);

                String source = Encoding.GetEncoding("utf-8").GetString(response.Result, 0, response.Result.Length - 1);

                source = WebUtility.HtmlDecode(source);

                HtmlDocument res = new HtmlDocument();

                res.LoadHtml(source);

                var matches = res.DocumentNode.Descendants("div").Where(d => d.Attributes.Contains("class") && d.Attributes["class"].Value.Contains("matchFieldContainer"));

                int i = 1;

                Day day = dbcontext.Days.Where(d => d.Number == c.CurrentMatchDayNumber).FirstOrDefault();

                foreach (var m in matches)
                {

                    var matchInfo = m.SelectSingleNode("descendant::div[@class='match']");

                    String homeTeamName = matchInfo.SelectSingleNode("descendant::div[@class='team homeTeam']").SelectSingleNode("descendant::span[@class='teamName']").FirstChild.InnerText;

                    String awayTeamName = matchInfo.SelectSingleNode("descendant::div[@class='team awayTeam']").SelectSingleNode("descendant::span[@class='teamName']").FirstChild.InnerText;

                    Debug.Write("Match: " + i++ + " " + homeTeamName.ToUpper() + " - " + awayTeamName.ToUpper());

                    var teamPlayersContainers = m.SelectSingleNode("descendant::div[@class='matchFieldInner']").Descendants("ul").Where(d => d.Attributes.Contains("class") && d.Attributes["class"].Value.Contains("team-players")).Skip(1);

                    var teamSubPlayersContainer = m.SelectSingleNode("descendant::div[@class='matchFieldInner']").SelectSingleNode("descendant::div[@class='matchDetails']");


                    Team homeTeam = dbcontext.Teams.Where(t => t.FullName.Contains(homeTeamName)).FirstOrDefault();

                    Team awayTeam = dbcontext.Teams.Where(t => t.FullName.Contains(awayTeamName)).FirstOrDefault();

                    Models.Match match = dbcontext.Matches.Where(mtc => mtc.AwayTeamId == awayTeam.Id && mtc.HomeTeamId == homeTeam.Id && mtc.DayId == day.Id).FirstOrDefault();

                    if (match == null)
                    {
                        match = new Models.Match
                        {
                            HomeTeamId = homeTeam.Id,
                            AwayTeamId = awayTeam.Id,
                            DayId = day.Id,
                            Players = new List<PlayerMatch>()
                        };

                        dbcontext.Matches.Add(match);

                        day.Matches.Add(match);

                        dbcontext.SaveChanges();
                    }

                    int teamIndex = 0;
                    foreach (var tps in teamPlayersContainers)
                    {
                        Debug.WriteLine("\n");
                        var playersList = tps.Descendants("li");
                        foreach (var p in playersList)
                        {
                            string playerName = p.SelectSingleNode("descendant::span[@class='team-player']").InnerText;
                            string playerNumber = p.SelectSingleNode("descendant::span[@class='numero']").InnerText;

                            Player player = null;

                            if (teamIndex == 0)
                            {
                                //HOME TEAM
                                player = dbcontext.Players.Where(pl => pl.Number.Equals(playerNumber) && pl.TeamId == homeTeam.Id).FirstOrDefault();
                            }
                            else
                            {
                                //AWAY TEAM
                                player = dbcontext.Players.Where(pl => pl.Number.Equals(playerNumber) && pl.TeamId == awayTeam.Id).FirstOrDefault();
                            }


                            if (player != null)
                            {
                                PlayerMatch pm = new PlayerMatch
                                {
                                    MatchId = match.Id,
                                    NewspaperId = 1,
                                    PlayerId = player.Id,
                                    Status = 1
                                };

                                dbcontext.PlayerMatches.Add(pm);

                                dbcontext.SaveChanges();
                                match.Players.Add(pm);
                                Debug.WriteLine(playerName + " " + playerNumber);
                            }
                            else
                            {
                                if (teamIndex == 0)
                                {
                                    //HOME TEAM
                                    player = new Player
                                    {
                                        Name = playerName + "\n" + playerName,
                                        Number = playerNumber,
                                        TeamId = homeTeam.Id,
                                        MarketValue = "",
                                        ContractUntil = "",
                                        Nationality = "",
                                        Position = "",
                                        DateOfBirth = ""
                                    };
                                }
                                else
                                {
                                    //AWAY TEAM
                                    player = new Player
                                    {
                                        Name = playerName + "\n" + playerName,
                                        Number = playerNumber,
                                        TeamId = awayTeam.Id,
                                        MarketValue = "",
                                        ContractUntil = "",
                                        Nationality = "",
                                        Position = "",
                                        DateOfBirth = ""
                                    };
                                }

                                dbcontext.Players.Add(player);
                                dbcontext.SaveChanges();

                                PlayerMatch pm = new PlayerMatch
                                {
                                    MatchId = match.Id,
                                    NewspaperId = 1,
                                    PlayerId = player.Id,
                                    Status = 1
                                };

                                dbcontext.PlayerMatches.Add(pm);

                                dbcontext.SaveChanges();
                                match.Players.Add(pm);

                                Debug.WriteLine(playerName + " " + playerNumber + "  PLAYER NOT FOUND --> ADDED!");
                            }


                        }
                        if (teamIndex == 0)
                        {
                            //HOME TEAM
                            Debug.WriteLine("\n");
                            Debug.WriteLine("A disposizione " + homeTeamName);
                            List<HtmlNode> subHome = teamSubPlayersContainer.SelectSingleNode("descendant::div[@class='homeDetails']").Descendants("p").ToList();
                            List<string> substitutions = subHome[0].InnerText.Replace(" ", string.Empty).Replace("&ensp;", string.Empty).Replace("Panchina:", string.Empty).Split(',').ToList();
                            foreach (string s in substitutions)
                            {
                                var numAlpha = new Regex("(?<Alpha>[a-zA-Z]*)(?<Numeric>[0-9]*)");
                                var matching = numAlpha.Match(s);
                                var num = matching.Groups["Numeric"].Value;
                                var name = s.Substring(num.Length + 1);
                                Player subplayer = dbcontext.Players.Where(pl => pl.Number.Equals(num) && pl.TeamId == homeTeam.Id).FirstOrDefault();
                                if (subplayer != null)
                                {
                                    PlayerMatch spmHome = new PlayerMatch
                                    {
                                        MatchId = match.Id,
                                        NewspaperId = 1,
                                        PlayerId = subplayer.Id,
                                        Status = 2
                                    };

                                    dbcontext.PlayerMatches.Add(spmHome);

                                    dbcontext.SaveChanges();
                                    match.Players.Add(spmHome);
                                    Debug.WriteLine(s);
                                }
                                else
                                {
                                    subplayer = new Player
                                    {
                                        Name = name + "\n" + name,
                                        Number = num,
                                        TeamId = homeTeam.Id,
                                        MarketValue = "",
                                        ContractUntil = "",
                                        Nationality = "",
                                        Position = "",
                                        DateOfBirth = ""
                                    };

                                    dbcontext.Players.Add(subplayer);

                                    dbcontext.SaveChanges();

                                    PlayerMatch spmHome = new PlayerMatch
                                    {
                                        MatchId = match.Id,
                                        NewspaperId = 1,
                                        PlayerId = subplayer.Id,
                                        Status = 2
                                    };

                                    dbcontext.PlayerMatches.Add(spmHome);

                                    dbcontext.SaveChanges();

                                    match.Players.Add(spmHome);

                                    Debug.WriteLine(s + "  PLAYER NOT FOUND --> ADDED!");
                                }
                            }
                        }
                        else
                        {
                            //AWAY TEAM
                            Debug.WriteLine("A disposizione " + awayTeamName);
                            List<HtmlNode> subAway = teamSubPlayersContainer.SelectSingleNode("descendant::div[@class='awayDetails']").Descendants("p").ToList();
                            List<string> substitutions = subAway[0].InnerText.Replace(" ", string.Empty).Replace("&ensp;", string.Empty).Replace("Panchina:", string.Empty).Split(',').ToList();
                            foreach (string s in substitutions)
                            {
                                var numAlpha = new Regex("(?<Alpha>[a-zA-Z]*)(?<Numeric>[0-9]*)");
                                var matching = numAlpha.Match(s);
                                var num = matching.Groups["Numeric"].Value;
                                var name = s.Substring(num.Length + 1);
                                Player subplayer = dbcontext.Players.Where(pl => pl.Number.Equals(num) && pl.TeamId == awayTeam.Id).FirstOrDefault();
                                if (subplayer != null)
                                {
                                    PlayerMatch spmAway = new PlayerMatch
                                    {
                                        MatchId = match.Id,
                                        NewspaperId = 1,
                                        PlayerId = subplayer.Id,
                                        Status = 2
                                    };

                                    dbcontext.PlayerMatches.Add(spmAway);

                                    dbcontext.SaveChanges();
                                    match.Players.Add(spmAway);
                                    Debug.WriteLine(s);
                                }
                                else
                                {
                                    subplayer = new Player
                                    {
                                        Name = name + "\n" + name,
                                        Number = num,
                                        TeamId = awayTeam.Id,
                                        MarketValue = "",
                                        ContractUntil = "",
                                        Nationality = "",
                                        Position = "",
                                        DateOfBirth = ""
                                    };

                                    dbcontext.Players.Add(subplayer);

                                    dbcontext.SaveChanges();

                                    PlayerMatch spmHome = new PlayerMatch
                                    {
                                        MatchId = match.Id,
                                        NewspaperId = 1,
                                        PlayerId = subplayer.Id,
                                        Status = 2
                                    };

                                    dbcontext.PlayerMatches.Add(spmHome);

                                    dbcontext.SaveChanges();

                                    match.Players.Add(spmHome);

                                    Debug.WriteLine(s + "  PLAYER NOT FOUND --> ADDED!");
                                }
                            }
                        }

                        teamIndex++;

                    }
                    Debug.WriteLine("\n");
                    Debug.WriteLine("\n");
                }

                dbcontext.SaveChanges();
                Console.WriteLine("GetTeamsFrom__GazzettaDelloSport Succesfully Completed");
            }
            catch (Exception e)
            {
                Console.WriteLine("GetTeamsFrom__GazzettaDelloSport Error (see Exception)");
                Debug.WriteLine(e.ToString());
            }

        }

        public static void GetTeamsFromCorriereDelloSport(PPFormazioniContext dbcontext)
        {
            Console.WriteLine("GetTeamsFrom__CorriereDelloSport...");
            try
            {
                Championship c = dbcontext.Championships.Where(ch => ch.Id == 1).FirstOrDefault();

                HttpClient http = new HttpClient();

                var response = http.GetByteArrayAsync(Constants.PF_CDS_URL);

                String source = Encoding.GetEncoding("utf-8").GetString(response.Result, 0, response.Result.Length - 1);

                source = WebUtility.HtmlDecode(source);

                HtmlDocument res = new HtmlDocument();

                res.LoadHtml(source);

                var matches = res.DocumentNode.SelectSingleNode("descendant::ul[@class='probabili-formazioni-list']").Descendants("li");

                int i = 1;

                Day day = dbcontext.Days.Where(d => d.Number == c.CurrentMatchDayNumber).FirstOrDefault();

                foreach (var m in matches)
                {
                    HttpClient http2 = new HttpClient();

                    var response2 = http2.GetByteArrayAsync(m.Descendants("a").First().Attributes["href"].Value);

                    String source2 = Encoding.GetEncoding("utf-8").GetString(response2.Result, 0, response2.Result.Length - 1);

                    source2 = WebUtility.HtmlDecode(source2);

                    HtmlDocument res2 = new HtmlDocument();

                    res2.LoadHtml(source2);

                    List<HtmlNode> matchDetailContainer = res2.DocumentNode.SelectSingleNode("descendant::table[@class='big probabile-formazione']").Descendants("tr").Where(d => d.Attributes.Contains("class") && d.Attributes["class"].Value.Contains("teams")).ToList();

                    var matchInfo = matchDetailContainer[0];

                    List<HtmlNode> teamsInfo = matchInfo.Descendants("td").ToList();

                    String homeTeamName = teamsInfo[0].SelectSingleNode("descendant::div[@class='team home']").SelectSingleNode("descendant::span").InnerText;

                    String awayTeamName = teamsInfo[1].SelectSingleNode("descendant::div[@class='team away']").SelectSingleNode("descendant::span").InnerText;

                    Debug.WriteLine("Match: " + i++ + " " + homeTeamName.ToUpper() + " - " + awayTeamName.ToUpper());

                    List<HtmlNode> players = res2.DocumentNode.SelectSingleNode("descendant::table[@class='big probabile-formazione']").Descendants("tr").Where(d => d.Descendants("td").Count() != 0 && d.Attributes.Count() == 0).ToList();

                    Team homeTeam = dbcontext.Teams.Where(t => t.FullName.Contains(homeTeamName)).FirstOrDefault();

                    Team awayTeam = dbcontext.Teams.Where(t => t.FullName.Contains(awayTeamName)).FirstOrDefault();

                    Models.Match match = dbcontext.Matches.Where(mtc => mtc.AwayTeamId == awayTeam.Id && mtc.HomeTeamId == homeTeam.Id && mtc.DayId == day.Id).FirstOrDefault();

                    if (match == null)
                    {
                        match = new Models.Match
                        {
                            HomeTeamId = homeTeam.Id,
                            AwayTeamId = awayTeam.Id,
                            DayId = day.Id,
                            Players = new List<PlayerMatch>()
                        };

                        dbcontext.Matches.Add(match);

                        day.Matches.Add(match);

                        dbcontext.SaveChanges();
                    }

                    for (int x = 0; x < players.Count; x++)
                    {

                        HtmlNode playersNode = players[x];

                        string playerName_home = playersNode.SelectSingleNode("descendant::td[@class='a-right']").InnerText;
                        string playerName_away = playersNode.SelectSingleNode("descendant::td[@class='a-left']").InnerText;


                        List<HtmlNode> numbers = playersNode.Descendants("td").Where(d => d.Attributes.Contains("class") && d.Attributes["class"].Value.Contains("a-center white")).ToList();
                        string playerNumber_home = numbers[0].InnerText;
                        string playerNumber_away = numbers[1].InnerText;

                        Player playerHome = dbcontext.Players.Where(pl => pl.Number.Equals(playerNumber_home) && pl.TeamId == homeTeam.Id).FirstOrDefault();
                        Player playerAway = dbcontext.Players.Where(pl => pl.Number.Equals(playerNumber_away) && pl.TeamId == awayTeam.Id).FirstOrDefault();

                        if (x < 11)
                        {
                            if (playerHome != null)
                            {
                                PlayerMatch pmHome = new PlayerMatch
                                {
                                    MatchId = match.Id,
                                    NewspaperId = 2,
                                    PlayerId = playerHome.Id,
                                    Status = 1
                                };

                                dbcontext.PlayerMatches.Add(pmHome);

                                dbcontext.SaveChanges();
                                match.Players.Add(pmHome);
                                Debug.WriteLine(playerName_home + " " + playerNumber_home);
                            }
                            else
                            {
                                playerHome = new Player
                                {
                                    Name = playerName_home + "\n" + playerName_home,
                                    Number = playerNumber_home,
                                    TeamId = homeTeam.Id,
                                    ContractUntil = "",
                                    DateOfBirth = "",
                                    MarketValue = "",
                                    Nationality = "",
                                    Position = ""
                                };

                                dbcontext.Players.Add(playerHome);
                                dbcontext.SaveChanges();

                                PlayerMatch pmHome = new PlayerMatch
                                {
                                    MatchId = match.Id,
                                    NewspaperId = 2,
                                    PlayerId = playerHome.Id,
                                    Status = 1
                                };

                                dbcontext.PlayerMatches.Add(pmHome);

                                dbcontext.SaveChanges();
                                match.Players.Add(pmHome);
                                Debug.WriteLine(playerName_home + " " + playerNumber_home + "  PLAYER NOT FOUND --> ADDED!");
                            }

                            if (playerAway != null)
                            {
                                PlayerMatch pmAway = new PlayerMatch
                                {
                                    MatchId = match.Id,
                                    NewspaperId = 2,
                                    PlayerId = playerAway.Id,
                                    Status = 1
                                };

                                dbcontext.PlayerMatches.Add(pmAway);

                                dbcontext.SaveChanges();
                                match.Players.Add(pmAway);
                                Debug.WriteLine(playerName_away + " " + playerNumber_away);
                            }
                            else
                            {
                                playerAway = new Player
                                {
                                    Name = playerName_away +  "\n" + playerName_away,
                                    Number = playerNumber_away,
                                    TeamId = awayTeam.Id,
                                    ContractUntil = "",
                                    DateOfBirth = "",
                                    MarketValue = "",
                                    Nationality = "",
                                    Position = ""
                                };

                                dbcontext.Players.Add(playerAway);
                                dbcontext.SaveChanges();

                                PlayerMatch pmAway = new PlayerMatch
                                {
                                    MatchId = match.Id,
                                    NewspaperId = 2,
                                    PlayerId = playerAway.Id,
                                    Status = 1
                                };

                                dbcontext.PlayerMatches.Add(pmAway);

                                dbcontext.SaveChanges();
                                match.Players.Add(pmAway);
                                Debug.WriteLine(playerName_away + " " + playerNumber_away + "  PLAYER NOT FOUND --> ADDED!");
                            }

                        }
                        else
                        {
                            //SUBSTITUTIONS
                            if (playerHome != null)
                            {
                                PlayerMatch pmHome = new PlayerMatch
                                {
                                    MatchId = match.Id,
                                    NewspaperId = 2,
                                    PlayerId = playerHome.Id,
                                    Status = 2
                                };

                                dbcontext.PlayerMatches.Add(pmHome);

                                dbcontext.SaveChanges();
                                match.Players.Add(pmHome);
                                Debug.WriteLine(playerName_home + " " + playerNumber_home);
                            }
                            else
                            {
                                playerHome = new Player
                                {
                                    Name = playerName_home + "\n" + playerName_home,
                                    Number = playerNumber_home,
                                    TeamId = homeTeam.Id,
                                    ContractUntil = "",
                                    DateOfBirth = "",
                                    MarketValue = "",
                                    Nationality = "",
                                    Position = ""
                                };

                                dbcontext.Players.Add(playerHome);
                                dbcontext.SaveChanges();

                                PlayerMatch pmHome = new PlayerMatch
                                {
                                    MatchId = match.Id,
                                    NewspaperId = 2,
                                    PlayerId = playerHome.Id,
                                    Status = 2
                                };

                                dbcontext.PlayerMatches.Add(pmHome);

                                dbcontext.SaveChanges();
                                match.Players.Add(pmHome);

                                Debug.WriteLine(playerName_home + " " + playerNumber_home + "  PLAYER NOT FOUND --> ADDED!");
                            }

                            if (playerAway != null)
                            {
                                PlayerMatch pmAway = new PlayerMatch
                                {
                                    MatchId = match.Id,
                                    NewspaperId = 2,
                                    PlayerId = playerAway.Id,
                                    Status = 2
                                };

                                dbcontext.PlayerMatches.Add(pmAway);

                                dbcontext.SaveChanges();
                                match.Players.Add(pmAway);
                                Debug.WriteLine(playerName_away + " " + playerNumber_away);
                            }
                            else
                            {
                                playerAway = new Player
                                {
                                    Name = playerName_away + "\n" + playerName_away,
                                    Number = playerNumber_away,
                                    TeamId = awayTeam.Id,
                                    ContractUntil = "",
                                    DateOfBirth = "",
                                    MarketValue = "",
                                    Nationality = "",
                                    Position = ""
                                };

                                dbcontext.Players.Add(playerAway);
                                dbcontext.SaveChanges();

                                PlayerMatch pmAway = new PlayerMatch
                                {
                                    MatchId = match.Id,
                                    NewspaperId = 2,
                                    PlayerId = playerAway.Id,
                                    Status = 2
                                };

                                dbcontext.PlayerMatches.Add(pmAway);

                                dbcontext.SaveChanges();
                                match.Players.Add(pmAway);

                                Debug.WriteLine(playerName_away + " " + playerNumber_away + "  PLAYER NOT FOUND --> ADDED!");
                            }

                        }
                    }

                }
                dbcontext.SaveChanges();
                Console.WriteLine("GetTeamsFrom__CorriereDelloSport Succesfully Completed");

            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                Console.WriteLine("GetTeamsFrom__CorriereDelloSport Error (See Exception)");
            }
        }


        public static void GetTeamsFromSkySport(PPFormazioniContext dbcontext)
        {
            Console.WriteLine("GetTeamsFrom__SkySport...");
            try
            {
                Championship c = dbcontext.Championships.Where(ch => ch.Id == 1).FirstOrDefault();

                HttpClient http = new HttpClient();

                var response = http.GetByteArrayAsync(Constants.PF_SKYSPORT_URL);

                String source = Encoding.GetEncoding("utf-8").GetString(response.Result, 0, response.Result.Length - 1);

                source = WebUtility.HtmlDecode(source);

                HtmlDocument res = new HtmlDocument();

                res.LoadHtml(source);

                var matches = res.DocumentNode.Descendants("div").Where(d => d.Attributes.Contains("class") && d.Attributes["class"].Value.Contains("formazione-match"));

                Day day = dbcontext.Days.Where(d => d.Number == c.CurrentMatchDayNumber).FirstOrDefault();

                int i = 1;

                foreach (var m in matches)
                {
                    string HomeTeamName = m.SelectSingleNode("descendant::div[@class='left']").SelectSingleNode("descendant::span[@class='name']").InnerText;
                    string AwayTeamName = m.SelectSingleNode("descendant::div[@class='right']").SelectSingleNode("descendant::span[@class='name']").InnerText;

                    string MatchDate = m.SelectSingleNode("descendant::span[@class='date']").InnerText;

                    Debug.WriteLine("Match: " + i++ + " " + HomeTeamName.ToUpper() + " - " + AwayTeamName.ToUpper());

                    Team homeTeam = dbcontext.Teams.Where(t => t.FullName.Contains(HomeTeamName)).FirstOrDefault();

                    Team awayTeam = dbcontext.Teams.Where(t => t.FullName.Contains(AwayTeamName)).FirstOrDefault();

                    Models.Match match = dbcontext.Matches.Where(mtc => mtc.AwayTeamId == awayTeam.Id && mtc.HomeTeamId == homeTeam.Id && mtc.DayId == day.Id).FirstOrDefault();

                    if (match == null)
                    {
                        match = new Models.Match
                        {
                            HomeTeamId = homeTeam.Id,
                            AwayTeamId = awayTeam.Id,
                            DayId = day.Id,
                            MatchDate = MatchDate,
                            Players = new List<PlayerMatch>()
                        };

                        dbcontext.Matches.Add(match);

                        day.Matches.Add(match);

                        dbcontext.SaveChanges();
                    }else
                    {
                        match.MatchDate = MatchDate;

                        dbcontext.SaveChanges();
                    }

                    var homePlayerContainer = m.SelectSingleNode("descendant::div[@class='team-1 left']").SelectSingleNode("descendant::ul[@class='playerslist']").Descendants("li");

                    foreach (var pHome in homePlayerContainer)
                    {
                        string HomePlayerName = pHome.SelectSingleNode("descendant::span[@class='name']").InnerText;
                        string HomePlayerNumber = pHome.SelectSingleNode("descendant::span[@class='number']").InnerText;

                        Player playerHome = dbcontext.Players.Where(pl => pl.Number.Equals(HomePlayerNumber) && pl.TeamId == homeTeam.Id).FirstOrDefault();

                        if (playerHome != null)
                        {
                            PlayerMatch pmHome = new PlayerMatch
                            {
                                MatchId = match.Id,
                                NewspaperId = 3,
                                PlayerId = playerHome.Id,
                                Status = 1
                            };

                            dbcontext.PlayerMatches.Add(pmHome);

                            dbcontext.SaveChanges();
                            match.Players.Add(pmHome);
                            Debug.WriteLine(HomePlayerName + " " + HomePlayerNumber);
                        }
                        else
                        {
                            playerHome = new Player
                            {
                                Name = HomePlayerName +"\n" + HomePlayerName,
                                Number = HomePlayerNumber,
                                TeamId = homeTeam.Id,
                                ContractUntil = "",
                                MarketValue = "",
                                DateOfBirth = "",
                                Nationality = "",
                                Position = ""
                            };

                            dbcontext.Players.Add(playerHome);

                            dbcontext.SaveChanges();

                            PlayerMatch pmHome = new PlayerMatch
                            {
                                MatchId = match.Id,
                                NewspaperId = 3,
                                PlayerId = playerHome.Id,
                                Status = 1
                            };

                            dbcontext.PlayerMatches.Add(pmHome);

                            dbcontext.SaveChanges();

                            match.Players.Add(pmHome);

                            Debug.WriteLine(HomePlayerName + " " + HomePlayerNumber + "  PLAYER NOT FOUND --> ADDED!");
                        }

                    }

                    //SUBSTITUTIONS HOME
                    string subPlayersHome = m.SelectSingleNode("descendant::div[@class='team-1 left']").SelectSingleNode("descendant::dl[@class='otherlist']").SelectSingleNode("descendant::dt[text()='Panchina:']").NextSibling.InnerText;
                    List<string> substitutionsHome = subPlayersHome.Replace(", ", ",").Split(',').ToList();
                    Debug.WriteLine("A disposizione:\n");
                    foreach (string sHome in substitutionsHome)
                    {
                        Player playerHome = dbcontext.Players.Where(pl => pl.Name.Contains(sHome) && pl.TeamId == homeTeam.Id).FirstOrDefault();

                        if (playerHome != null)
                        {
                            PlayerMatch pmHome = new PlayerMatch
                            {
                                MatchId = match.Id,
                                NewspaperId = 3,
                                PlayerId = playerHome.Id,
                                Status = 2
                            };

                            dbcontext.PlayerMatches.Add(pmHome);

                            dbcontext.SaveChanges();
                            match.Players.Add(pmHome);
                            Debug.WriteLine(sHome);
                        }
                        else
                        {
                            playerHome = new Player
                            {
                                Name = sHome + "\n" + sHome,
                                Number = "",
                                TeamId = homeTeam.Id,
                                ContractUntil = "",
                                MarketValue = "",
                                DateOfBirth = "",
                                Nationality = "",
                                Position = ""
                            };

                            dbcontext.Players.Add(playerHome);
                            dbcontext.SaveChanges();

                            PlayerMatch pmHome = new PlayerMatch
                            {
                                MatchId = match.Id,
                                NewspaperId = 3,
                                PlayerId = playerHome.Id,
                                Status = 2
                            };

                            dbcontext.PlayerMatches.Add(pmHome);

                            dbcontext.SaveChanges();
                            match.Players.Add(pmHome);

                            Debug.WriteLine(sHome + "  PLAYER NOT FOUND --> ADDED!");
                        }

                    }

                    var awayPlayerContainer = m.SelectSingleNode("descendant::div[@class='team-2 right']").SelectSingleNode("descendant::ul[@class='playerslist']").Descendants("li");

                    foreach (var pAway in awayPlayerContainer)
                    {
                        string AwayPlayerName = pAway.SelectSingleNode("descendant::span[@class='name']").InnerText;
                        string AwayPlayerNumber = pAway.SelectSingleNode("descendant::span[@class='number']").InnerText;

                        Player playerAway = dbcontext.Players.Where(pl => pl.Number.Equals(AwayPlayerNumber) && pl.TeamId == awayTeam.Id).FirstOrDefault();

                        if (playerAway != null)
                        {
                            PlayerMatch pmAway = new PlayerMatch
                            {
                                MatchId = match.Id,
                                NewspaperId = 3,
                                PlayerId = playerAway.Id,
                                Status = 1
                            };

                            dbcontext.PlayerMatches.Add(pmAway);

                            dbcontext.SaveChanges();
                            match.Players.Add(pmAway);
                            Debug.WriteLine(AwayPlayerName + " " + AwayPlayerNumber);
                        }
                        else
                        {
                            playerAway = new Player
                            {
                                Name = AwayPlayerName + "\n" + AwayPlayerName,
                                Number = AwayPlayerNumber,
                                TeamId = awayTeam.Id,
                                ContractUntil = "",
                                MarketValue = "",
                                DateOfBirth = "",
                                Nationality = "",
                                Position = ""
                            };

                            dbcontext.Players.Add(playerAway);

                            dbcontext.SaveChanges();

                            PlayerMatch pmAway = new PlayerMatch
                            {
                                MatchId = match.Id,
                                NewspaperId = 3,
                                PlayerId = playerAway.Id,
                                Status = 1
                            };

                            dbcontext.PlayerMatches.Add(pmAway);

                            dbcontext.SaveChanges();
                            match.Players.Add(pmAway);

                            Debug.WriteLine(AwayPlayerName + " " + AwayPlayerNumber + "  PLAYER NOT FOUND --> ADDED!");
                        }
                    }

                    //SUBSTITUTIONS AWAY
                    string subPlayersAway = m.SelectSingleNode("descendant::div[@class='team-2 right']").SelectSingleNode("descendant::dl[@class='otherlist']").SelectSingleNode("descendant::dt[text()='Panchina:']").NextSibling.InnerText;
                    List<string> substitutionsAway = subPlayersAway.Replace(", ", ",").Split(',').ToList();
                    Debug.WriteLine("A disposizione\n");
                    foreach (string sAway in substitutionsAway)
                    {
                        Player playerAway = dbcontext.Players.Where(pl => pl.Name.Contains(sAway) && pl.TeamId == awayTeam.Id).FirstOrDefault();

                        if (playerAway != null)
                        {
                            PlayerMatch pmAway = new PlayerMatch
                            {
                                MatchId = match.Id,
                                NewspaperId = 3,
                                PlayerId = playerAway.Id,
                                Status = 2
                            };

                            dbcontext.PlayerMatches.Add(pmAway);

                            dbcontext.SaveChanges();
                            match.Players.Add(pmAway);
                            Debug.WriteLine(sAway);
                        }
                        else
                        {
                            playerAway = new Player
                            {
                                Name = sAway + "\n" + sAway,
                                Number = "",
                                TeamId = awayTeam.Id,
                                ContractUntil = "",
                                MarketValue = "",
                                DateOfBirth = "",
                                Nationality = "",
                                Position = ""
                            };

                            dbcontext.Players.Add(playerAway);

                            dbcontext.SaveChanges();

                            PlayerMatch pmAway = new PlayerMatch
                            {
                                MatchId = match.Id,
                                NewspaperId = 3,
                                PlayerId = playerAway.Id,
                                Status = 2
                            };

                            dbcontext.PlayerMatches.Add(pmAway);

                            dbcontext.SaveChanges();

                            match.Players.Add(pmAway);

                            Debug.WriteLine(sAway + "  PLAYER NOT FOUND --> ADDED!");
                        }
                    }

                }

                dbcontext.SaveChanges();
                Console.WriteLine("GetTeamsFrom__SkySport Succesfully Completed");
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                Console.WriteLine("GetTeamsFrom__SkySport Error (See Exception)");
            }
        }

        public static void NotifyClient(PPFormazioniContext dbcontext)
        {
            Console.WriteLine();
            Console.WriteLine("FCM - Starting Notify Android Client");
            string result = "";
            using (var client = new System.Net.WebClient())
            {
                NotificationMessage nm = new NotificationMessage
                {
                    body = "New Lineups Avaiable",
                    title = "Lineups Updated!"
                };
                NotificationObject no = new NotificationObject
                {
                    body = "New Lineups Avaiable",
                    title = "Lineups Updated"
                };
                NotificationClient notification = new NotificationClient
                {
                    to = "/topics/lineups_update",
                    data = nm,
                    notification = no
                };


                //Save Notification to DB
                List<User> users = dbcontext.Users.ToList();
                foreach(User u in users)
                {
                    dbcontext.Notifications.Add(new Notification
                    {
                         Body = no.body,
                         Title = no.title,
                         UserId = u.Id,
                         CreatedAt = DateTime.Now
                    });
                    dbcontext.SaveChanges();
                }

                string json = Newtonsoft.Json.JsonConvert.SerializeObject(notification);
                client.Headers["Content-Type"] = "application/json";
                client.Headers["Authorization"] = "key=AAAA2yHRE0w:APA91bEVyqARWsF5_HaCUdhfNEA3K1nxkDTOSES2nzYqmj8J2PeAJ2lTRdyrMPEJ7xEjyudcuCjrevvpfFCCAtNNmuTpIbL68j2KaSAYdpxoESap3Uqx1R6yovQOnAy-8ikoyL2iFFBJRPdDQANwvHsjflGIr3bKKA";
                result = client.UploadString("https://fcm.googleapis.com/fcm/send", "POST", json);
            }

            Debug.WriteLine(result);
            Console.WriteLine("FCM - Operation Result: " +  result);
        }



    }

}