using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PPFormazioniAPI.DAL;
using PPFormazioniAPI.Models;

namespace PPFormazioniAPI.Controllers


{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        public PPFormazioniContext dbContext;

        public ValuesController(PPFormazioniContext c)
        {
            dbContext = c;
        }
        // GET api/values
        [HttpGet]
        public string Get()
        {
            return "PPFormazioni WEB-API Running...";
        }
    }
}
