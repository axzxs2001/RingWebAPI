using RingWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RingWebAPI.Services
{
    public interface IUserService
    {
        User Login(LoginUser login);
    }
}
