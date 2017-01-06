using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPFormazioniAPI.Models
{
    public class Notification
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
