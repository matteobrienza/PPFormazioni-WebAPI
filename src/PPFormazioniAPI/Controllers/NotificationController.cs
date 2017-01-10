using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PPFormazioniAPI.Models;
using PPFormazioniAPI.DAL;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace PPFormazioniAPI.Controllers
{
   

    [Route("api/[controller]")]
    public class NotificationController : Controller
    {
        private PPFormazioniContext dbContext;

        public NotificationController(PPFormazioniContext c)
        {
            dbContext = c;
        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }
        
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }
        
        [HttpPost]
        public string Post([FromBody]NotificationMessage notification)
        {
            try
            {
                //Save Notification to DB
                List<User> users = dbContext.Users.ToList();
                foreach (User u in users)
                {
                    dbContext.Notifications.Add(new Notification
                    {
                        Body = notification.body,
                        Title = notification.title,
                        UserId = u.Id,
                        CreatedAt = DateTime.Now
                    });
                }
                dbContext.SaveChanges();
                return "Success";
            }
            catch
            {
                return "Failed";
            }
            
        }
        
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }
        
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
