using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPFormazioniAPI.Models
{
    public class User
    {
        public int Id { get; set; }
        public string PhoneNumber { get; set; }
        public string Username { get; set; }
        public string Avatar { get; set; }
        public string AuthToken { get; set; } //API AUTH PURPOSE
        public string Created_At { get; set; }
    }
}
