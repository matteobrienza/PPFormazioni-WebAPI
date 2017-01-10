using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPFormazioniAPI.Models
{
    public class UserPlayer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Number { get; set; }
        public int CdS_Status { get; set; }
        public int GdS_Status { get; set; }
        public int SS_Status { get; set; }
        public virtual List<User> Users { get; set; }
    }
}
