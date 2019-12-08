using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
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

                var claims = from permission in permissions
                    select new Claim(Permissions.Claim, permission);

                var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "X-Permission"));
                context.User = principal;
            }

            await this.next(context);
        }
    }
}
