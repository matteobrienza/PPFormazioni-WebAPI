using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPFormazioniAPI.Models
{
    public class UserLineup
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public List<UserPlayerLineup> Players { get; set; }
    }

    public class UserPlayerLineup
    {
        public int Id { get; set; }
        public int Name { get; set; }
        public int Number { get; set; }
        public int CdS_Status { get; set; }
        public int GdS_Status { get; set; }
        public int SS_Status { get; set; }
    }
}
