using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
#if (!NoAuthenticate)
using RingWebAPI.Models;
using RingWebAPI.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
#endif
namespace RingWebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {

        private readonly ILogger<HomeController> _logger;
#if (NoAuthenticate)
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
#else
        private readonly IUserService _userService;
        private readonly PermissionRequirement _permissionRequirement;
        public HomeController(ILogger<HomeController> logger, IUserService userService, PermissionRequirement permissionRequirement)
        {
            _permissionRequirement = permissionRequirement;
            _userService = userService;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost("/api/login")]
        public IActionResult Login([FromBody] LoginUser loginUser)
        {
            _logger.LogInformation($"{loginUser.UserName} 登录");
            var user = _userService.Login(loginUser);
            if (user == null)
            {
                return new JsonResult(new
                {
                    Status = false,
                    Message = "认证失败"
                });
            }
            else
            {
                //如果是基于用户的授权策略，这里要添加用户;如果是基于角色的授权策略，这里要添加角色
                var claims = new Claim[] {
                    new Claim(ClaimTypes.Name,user.Name),
                    new Claim(ClaimTypes.NameIdentifier,user.UserName),
                    new Claim(ClaimTypes.Role, user.Role),
                    new Claim(ClaimTypes.Expiration, DateTime.Now.AddSeconds(_permissionRequirement.Expiration.TotalSeconds).ToString())
                };
                var token = JwtToken.BuildJwtToken(claims, _permissionRequirement);
                return new JsonResult(token);

            }
        }
#endif

#if (Policy)
        [Authorize("Permission")]
        [HttpGet("/adminapi")]
        public string GetAdminAPI()
        {
            return "get adminapi";
        }
        [Authorize("Permission")]
        [HttpGet("/systemapi")]
        public string GetSystemAPI()
        {
            return "get systemapi";
        }
        [Authorize("Permission")]
        [HttpPost("/adminapi")]
        public string PostAdminAPI()
        {
            return "post adminapi";
        }
        [Authorize("Permission")]
        [HttpPost("/systemapi")]
        public string PostSystemAPI()
        {
            return "post systemapi";
        }
#endif
#if (Role)
        [Authorize(Roles ="admin")]
        [HttpGet("/adminapi")]
        public string GetAdminAPI()
        {
            return "get adminapi";
        }
        [Authorize(Roles ="system")]
        [HttpGet("/systemapi")]
        public string GetSystemAPI()
        {
            return "get systemapi";
        }
#endif


    }
}
