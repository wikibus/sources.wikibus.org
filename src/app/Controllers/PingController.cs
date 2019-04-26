using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace app.Controllers
{
    [Route("api")]
    public class PingController : Controller
    {
        [HttpGet]
        [Route("ping")]
        public string Ping()
        {
            return "Poong";
        }

        [Authorize]
        [HttpGet]
        [Route("ping/secure")]
        public string PingSecured()
        {
            return "All good. You only get this message if you are authenticated.";
        }

        [Authorize]
        [HttpGet("claims")]
        public object Claims()
        {
            return User.Claims.Select(c =>
                new
                {
                    Type = c.Type,
                    Value = c.Value
                });
        }
    }
}