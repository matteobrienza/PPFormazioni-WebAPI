using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using PPFormazioniAPI.Models;
using PPFormazioniAPI.DAL;

namespace PPFormazioniAPI.Controllers
{
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private PPFormazioniContext dbContext;

        public UsersController(PPFormazioniContext c)
        {
            dbContext = c;
        }

        // GET: api/users
        [HttpGet]
        public IEnumerable<User> Get()
        {
            try
            {
                return dbContext.Users.ToList();
            }catch(Exception e)
            {
                return null;
            }
        }

        // GET api/users/5
        [HttpGet("{id}")]
        public User Get(int id)
        {
            try
            {
                return dbContext.Users.Where(u => u.Id == id).FirstOrDefault();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public class PostModel
        {
            public List<PlayerModel> Lineup { get; set; }
        }

        public class PlayerModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public bool Selected { get; set; }
        }
        

        [HttpPost("{id}/lineup")]
        public ActionResult SaveLineups([FromBody]PostModel Lineup)
        {
            return null;
        }

        // POST api/users
        [HttpPost]
        public User Post([FromBody]string value)
        {

            IHeaderDictionary headers = HttpContext.Request.Headers;
            string xauth_credentials = headers["X-Verify-Credentials-Authorization"];
            string xauth_provider = headers["X-Auth-Service-Provider"];

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", xauth_credentials);

                var response = client.GetAsync(xauth_provider).Result;

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = response.Content;

                    string responseString = responseContent.ReadAsStringAsync().Result;

                    JObject digitsClient = JObject.Parse(responseString);

                    string phoneNumber = (string)digitsClient["phone_number"];

                    User user = dbContext.Users.Where(u => u.PhoneNumber.Equals(phoneNumber)).FirstOrDefault();

                    if (user == null)
                    {
                        user = new User
                        {
                            PhoneNumber = phoneNumber,
                            Created_At = (string)digitsClient["created_at"],
                            AuthToken = "",
                            Avatar = ""
                        };

                        dbContext.Users.Add(user);
                        dbContext.SaveChanges();

                        user.Username = "User_" + user.Id;

                        dbContext.SaveChanges();

                    }

                    return user;

                }
                else return null;
            }



        }
        

        [HttpGet("{id}/notifications")]
        public IEnumerable<Notification> getNotifications(int id)
        {
            try
            {
                return dbContext.Notifications.Where(n => n.UserId == id).ToList();
            }catch(Exception e)
            {
                return null;
            }
        }

        [HttpDelete("{id}/notifications/{notificationId}")]
        public void Delete(int id, int notificationId)
        {
            List<Notification> user_notifications = dbContext.Notifications.Where(n => n.UserId == id).ToList();

            Notification n_remove = user_notifications.Where(n => n.Id == notificationId).FirstOrDefault();

            if(n_remove != null)
            {
                dbContext.Notifications.Remove(n_remove);

                dbContext.SaveChanges();
            }
        }


        #region TEST
        [HttpGet("{id}/send")]
        public void SendNotifications(int id)
        {
            string result = "";
            using (var client = new System.Net.WebClient())
            {
                NotificationMessage nm = new NotificationMessage
                {
                    body = "This is a Test Notification",
                    title = "TEST NOTIFICATION"
                };
                NotificationObject no = new NotificationObject
                {
                    body = "This is a Test Notification",
                    title = "TEST NOTIFICATION"
                };
                NotificationClient notification = new NotificationClient
                {
                    to = "/topics/lineups_update",
                    data = nm,
                    notification = no
                };


                //Save Notification to DB
                List<User> users = dbContext.Users.ToList();
                foreach (User u in users)
                {
                    dbContext.Notifications.Add(new Notification
                    {
                        Body = no.body,
                        Title = no.title,
                        UserId = u.Id,
                        CreatedAt = DateTime.Now
                    });
                    dbContext.SaveChanges();
                }

                string json = Newtonsoft.Json.JsonConvert.SerializeObject(notification);
                client.Headers["Content-Type"] = "application/json";
                client.Headers["Authorization"] = "key=AAAA2yHRE0w:APA91bEVyqARWsF5_HaCUdhfNEA3K1nxkDTOSES2nzYqmj8J2PeAJ2lTRdyrMPEJ7xEjyudcuCjrevvpfFCCAtNNmuTpIbL68j2KaSAYdpxoESap3Uqx1R6yovQOnAy-8ikoyL2iFFBJRPdDQANwvHsjflGIr3bKKA";
                result = client.UploadString("https://fcm.googleapis.com/fcm/send", "POST", json);
            }

            System.Diagnostics.Debug.WriteLine(result);
            Console.WriteLine(result);
            
        }
        #endregion
    }
}
