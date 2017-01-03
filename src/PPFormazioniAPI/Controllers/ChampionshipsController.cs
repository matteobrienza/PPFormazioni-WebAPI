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
            }catch(Exception e)
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

        [HttpGet("{id}/players")]
        public IEnumerable<Player> GetPlayers(int id)
        {
            try
            {
                return (from p in dbContext.Players
                       join t in dbContext.Teams on p.TeamId equals t.Id
                       where t.ChampionshipId == id
                       select p).ToList();
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
        public IEnumerable<TeamChampionshipStatsWithDetail> GetStandings(int id)
        {
            try
            {
                return  from s in dbContext.TeamsChampionshipStats
                        join t in dbContext.Teams on s.TeamId equals t.Id
                        where t.ChampionshipId == id
                        select new TeamChampionshipStatsWithDetail{
                             Team = t,
                             Stats = s
                        };
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
