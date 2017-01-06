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
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/users/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
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

        // PUT api/users/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/users/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
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
                    body = "New Lineups Avaiable",
                    title = "Lineups Updated!"
                };
                NotificationObject no = new NotificationObject
                {
                    body = "New Lineups Avaiable",
                    title = "Lineups Updated"
                };
                Notification notification = new Notification
                {
                    to = "/topics/lineups_update",
                    data = nm,
                    notification = no
                };
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
