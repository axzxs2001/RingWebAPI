using RingWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RingWebAPI.Services
{
    public class UserService : IUserService
    {
        public User Login(LoginUser login)
        {
            var users = new List<User> {
               new User { UserName="gsw",Role="admin",Name="桂素伟",Password="111111" },
               new User { UserName="ggg",Role="system",Name="GuiSuWei",Password="111111" },
           };
            return users.SingleOrDefault(s => s.UserName == login.UserName && s.Password == login.Password);
        }
    }
}
