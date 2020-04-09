using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Anotar.Serilog;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Wikibus.Common;

namespace Brochures.Wikibus.Org
{
    public class AuthorizationHeaders
    {
        private readonly RequestDelegate next;

        public AuthorizationHeaders(RequestDelegate next)
        {
            this.next = next;
        }

        [UsedImplicitly]
        public async Task Invoke(HttpContext context)
        {
            if (context.User.Identity.IsAuthenticated == false)
            {
                var permissions = from header in context.Request.Headers
                    where header.Key.ToLower() == "x-permission"
                    select header.Value;

                var claims = (from permission in permissions
                    select new Claim(Permissions.Claim, permission)).ToList();

                var userNames = (from header in context.Request.Headers
                    where header.Key.ToLower() == "x-user"
                    select header.Value).FirstOrDefault();
                claims.AddRange(userNames.Select(userName => new Claim(ClaimTypes.NameIdentifier, userName)));

                if (claims.Any())
                {
                    var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "X-Permission"));
                    context.User = principal;

                    LogTo.Debug(
                        "Initializing test user with claims {0}",
                        string.Join(Environment.NewLine, principal.Claims.Select(c => $"{c.Type}: {c.Value}")));
                }
            }

            await this.next(context);
        }
    }
}
