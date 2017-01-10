using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using PPFormazioniAPI.Models;
using PPFormazioniAPI.DAL;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace PPFormazioniAPI.Controllers
{
    [Route("api/[controller]")]
    public class ChampionshipsController : Controller
    {
        public PPFormazioniContext dbContext;

        public ChampionshipsController(PPFormazioniContext c)
        {
            dbContext = c;
        }
        // GET api/championships
        [HttpGet]
        public IEnumerable<Championship> Get()
        {
            try
            {
                return dbContext.Championships.ToList();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        // GET api/championships/5
        [HttpGet("{id}")]
        public Championship Get(int id)
        {
            try
            {
                return dbContext.Championships.Where(c => c.Id == id).FirstOrDefault();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        [HttpGet("{id}/days")]
        public IEnumerable<Day> GetDays(int id)
        {
            try
            {
                Championship c = dbContext.Championships.Where(ch => ch.Id == id).FirstOrDefault();
                return dbContext.Days.Where(d => d.ChampionshipId == id).OrderByDescending(md => md.Number).ToList();
            }
            catch (Exception e)
            {
                return null;
            }
        }


        [HttpGet("{id}/days/{dayId}")]
        public Day GetDay(int id, int dayId)
        {
            try
            {
                Championship c = dbContext.Championships.Where(ch => ch.Id == id).FirstOrDefault();
                return dbContext.Days.Where(d => d.ChampionshipId == id && d.Id == dayId).FirstOrDefault();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        // GET api/championships/5/teams
        [HttpGet("{id}/teams")]
        public IEnumerable<Team> GetTeams(int id)
        {
            try
            {
                return dbContext.Championships.Where(c => c.Id == id).FirstOrDefault().Teams;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        // GET api/championships/5/teams/1
        [HttpGet("{id}/teams/{teamId}")]
        public Team GetTeam(int id, int teamId)
        {
            try
            {
                return dbContext.Championships.Where(c => c.Id == id).FirstOrDefault().Teams.Where(t => t.Id == teamId).FirstOrDefault();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        [HttpGet("{id}/newspapers")]
        public IEnumerable<Newspaper> GetNewspapers(int id)
        {
            try
            {

                return dbContext.Newspapers.Where(n => n.ChampionshipId == id).ToList();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        [HttpGet("{id}/newspapers/{newspaperId}")]
        public Newspaper GetNewspaper(int id, int newspaperId)
        {
            try
            {

                return dbContext.Newspapers.Where(n => n.ChampionshipId == id && n.Id == newspaperId).FirstOrDefault();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        [HttpGet("{id}/newspapers/{newspaperId}/matches")]
        public IEnumerable<Match> GetNewspaperMatches(int id, int newspaperId)
        {
            try
            {
                Newspaper newspaper = dbContext.Newspapers.Where(n => n.ChampionshipId == id && n.Id == newspaperId).FirstOrDefault();

                if (newspaper != null)
                {
                    return (from pm in dbContext.PlayerMatches
                            join m in dbContext.Matches on pm.MatchId equals m.Id
                            where pm.NewspaperId == newspaper.Id
                            select m).ToList();
                }
                else return new List<Match>();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        [HttpGet("{id}/newspapers/{newspaperId}/matches/{matchId}")]
        public Match GetNewspaperMatch(int id, int newspaperId, int matchId)
        {
            try
            {
                Newspaper newspaper = dbContext.Newspapers.Where(n => n.ChampionshipId == id && n.Id == newspaperId).FirstOrDefault();
                
                Match m = dbContext.Matches.Where(mat => mat.Id == matchId).FirstOrDefault();

                List<PlayerMatch> plMatch = (from pm in dbContext.PlayerMatches
                                             join mt in dbContext.Matches on pm.MatchId equals mt.Id
                                             where pm.NewspaperId == newspaper.Id && mt.Id == matchId
                                             select pm).ToList();
                return new Match
                {
                    Id = m.Id,
                    AwayTeam = m.AwayTeam,
                    AwayTeamId = m.AwayTeamId,
                    HomeTeam = m.HomeTeam,
                    HomeTeamId = m.HomeTeamId,
                    DayId = m.DayId,
                    MatchDate = m.MatchDate,
                    Players = plMatch
                };
            }
            catch (Exception e)
            {
                return null;
            }
        }
        
        [HttpGet("{id}/players")]
        public IEnumerable<Player> GetPlayersByRole(int id, [FromQuery] string[] role)
        {
            try
            {
                if(role.Length != 0)
                {
                    List<Player> result = new List<Player>();
                    foreach(string position in role)
                    {
                        List<Player> r = (from p in dbContext.Players
                                  join t in dbContext.Teams on p.TeamId equals t.Id
                                  where t.ChampionshipId == id && p.Position.Contains(position)
                                  select p).ToList();
                        result.AddRange(r);
                    }
                    return result;
                }
                else
                {
                    return (from p in dbContext.Players
                            join t in dbContext.Teams on p.TeamId equals t.Id
                            where t.ChampionshipId == id
                            select p).ToList();
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }

        [HttpGet("{id}/players/{playerId}")]
        public Player GetPlayers(int id, int playerId)
        {
            try
            {
                return (from p in dbContext.Players
                        join t in dbContext.Teams on p.TeamId equals t.Id
                        where t.ChampionshipId == id && p.Id == playerId
                        select p).FirstOrDefault();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        [HttpGet("{id}/standings")]
        public IEnumerable<TeamChampionshipStatsWithDetail> GetAllStandings(int id)
        {
            try
            {
                return (from s in dbContext.TeamsChampionshipStats
                        join t in dbContext.Teams on s.TeamId equals t.Id
                        where t.ChampionshipId == id
                        select new TeamChampionshipStatsWithDetail
                        {
                            Team = t,
                            Stats = s
                        }).ToList();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        [HttpGet("{id}/standings/{teamId}")]
        public TeamChampionshipStatsWithDetail GetStandings(int id, int teamId)
        {
            try
            {
                return (from s in dbContext.TeamsChampionshipStats
                        join t in dbContext.Teams on s.TeamId equals t.Id
                        where t.ChampionshipId == id && t.Id == teamId
                        select new TeamChampionshipStatsWithDetail
                        {
                            Team = t,
                            Stats = s
                        }).FirstOrDefault();
            }
            catch (Exception e)
            {
                return null;
            }
        }

    }
}
