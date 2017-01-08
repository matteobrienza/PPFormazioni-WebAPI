using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPFormazioniAPI.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public DateTime CreatedAt { get; set; }
        public int UserId { get; set; }
    }


    public class NotificationClient
    {
        public string to { get; set; }
        public NotificationMessage data { get; set; }
        public NotificationObject notification { get; set; }
    }

    public class NotificationMessage
    {
        public string body { get; set; }
        public string title { get; set; }
    }

    public class NotificationObject
    {
        public string body { get; set; }
        public string title { get; set; }
    }
}
