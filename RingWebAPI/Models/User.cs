using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RingWebAPI.Models
{
    public class User
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
    }
}
