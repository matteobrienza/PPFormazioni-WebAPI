using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PPFormazioniAPI.Models;
using PPFormazioniAPI.DAL;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace PPFormazioniAPI.Controllers
{
    [Route("api/[controller]")]
    public class ChampionshipController : Controller
    {
        public PPFormazioniContext dbContext;
        public ChampionshipController(PPFormazioniContext c)
        {
            dbContext = c;
        }
        // GET api/championship
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
    }
}
