using System;
using System.Collections.Generic;
using System.Linq;
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
            }
            catch (Exception e)
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

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            try
            {
                User u = dbContext.Users.Where(us => us.Id == id).FirstOrDefault();

                List<UserPlayer> upls = dbContext.UserPlayersLineup.Where(upl => upl.Users.Any(user => user.Id == u.Id)).ToList();

                upls.ForEach(uspl => uspl.Users.Remove(u));

                dbContext.Users.Remove(u);

                dbContext.SaveChanges();
            }
            catch (Exception e)
            {

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

        [HttpGet("{id}/lineup")]
        public IEnumerable<UserPlayer> GetUserLineup(int id)
        {
            try
            {
                User u = dbContext.Users.Where(us => us.Id == id).FirstOrDefault();
                return u.LineupPlayers.ToList();
            }catch(Exception e)
            {
                return null;
            }
        }

        [HttpPost("{id}/lineup")]
        public void SaveLineups(int id, [FromBody]List<PlayerModel> players)
        {
            try
            {
                Championship championship = dbContext.Championships.Where(c => c.Id == 1).FirstOrDefault();

                List<PlayerMatch> CurrentDayPlayersMatch = (from pm in dbContext.PlayerMatches
                                                            join m in dbContext.Matches on pm.MatchId equals m.Id
                                                            join d in dbContext.Days on m.DayId equals d.Id
                                                            where d.Number == championship.CurrentMatchDayNumber
                                                            select pm).ToList();

                User user = dbContext.Users.Where(u => u.Id == id).FirstOrDefault();

                List<UserPlayer> toRemove = dbContext.UserPlayersLineup.Where(ust => ust.Users.Any(us => us.Id == user.Id)).ToList();

                if (toRemove != null)
                {
                    toRemove.ForEach(lp => user.LineupPlayers.Remove(lp));
                    toRemove.ForEach(tr => dbContext.UserPlayersLineup.Remove(tr));
                    dbContext.SaveChanges();
                }
                
                List<UserPlayer> UserPlayers = new List<UserPlayer>();

                foreach (PlayerModel pm in players)
                {
                    Player player = dbContext.Players.Where(pl => pl.Id == pm.Id).FirstOrDefault();
                    
                    UserPlayer upl = dbContext.UserPlayersLineup.Where(up => up.Id == player.Id).FirstOrDefault();
                    if (upl == null)
                    {
                        PlayerMatch plm_cds = CurrentDayPlayersMatch.Where(plm => plm.PlayerId == player.Id && plm.NewspaperId == 1).FirstOrDefault();
                        int cds_status;
                        int ss_status;
                        int gds_status;

                        if (plm_cds != null)
                        {
                            cds_status = plm_cds.Status;
                        }
                        else
                        {
                            cds_status = 2;
                        }

                        PlayerMatch plm_gds = CurrentDayPlayersMatch.Where(plm => plm.PlayerId == player.Id && plm.NewspaperId == 2).FirstOrDefault();
                        if (plm_gds != null)
                        {
                            gds_status = plm_gds.Status;
                        }
                        else
                        {
                            gds_status = 2;
                        }

                        PlayerMatch plm_ss = CurrentDayPlayersMatch.Where(plm => plm.PlayerId == player.Id && plm.NewspaperId == 3).FirstOrDefault();
                        if (plm_ss != null)
                        {
                            ss_status = plm_ss.Status;
                        }
                        else
                        {
                            ss_status = 2;
                        }

                        
                        upl = new UserPlayer
                        {
                            Id = player.Id,
                            Name = player.Name,
                            Number = player.Number,
                            GdS_Status = gds_status,
                            CdS_Status = cds_status,
                            SS_Status = ss_status
                        };
                        dbContext.UserPlayersLineup.Add(upl);
                        dbContext.SaveChanges();
                    }
                    
                    UserPlayers.Add(upl);
                }

                user.LineupPlayers = UserPlayers;

                dbContext.SaveChanges();

            }
            catch(Exception e)
            {

            }
        }

        // POST api/users
        [HttpPost]
        public JsonResult Post([FromBody]string value)
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

                    return new JsonResult(user);

                }
                else return new JsonResult(response.Content);
            }



        }


        [HttpGet("{id}/notifications")]
        public IEnumerable<Notification> getNotifications(int id)
        {
            try
            {
                return dbContext.Notifications.Where(n => n.UserId == id).ToList();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        [HttpDelete("{id}/notifications/{notificationId}")]
        public void Delete(int id, int notificationId)
        {
            List<Notification> user_notifications = dbContext.Notifications.Where(n => n.UserId == id).ToList();

            Notification n_remove = user_notifications.Where(n => n.Id == notificationId).FirstOrDefault();

            if (n_remove != null)
            {
                dbContext.Notifications.Remove(n_remove);

                dbContext.SaveChanges();
            }
        }

      

        #region TEST
        [HttpGet("send")]
        public void SendNotifications(int id)
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

            string saveNotificationResult = "";
            using (var client = new System.Net.WebClient())
            {
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(nm);
                client.Headers["Content-Type"] = "application/json";
                saveNotificationResult = client.UploadString("http://ppformazioni.azurewebsites.net/api/notification", "POST", json);
                //saveNotificationResult = client.UploadString("http://localhost:3099/api/notification", "POST", json);
            }

            string result = "";
            using (var client = new System.Net.WebClient())
            {
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
