using System.Linq;
using Argolis.Hydra.Nancy;
using Nancy;
using Nancy.Security;
using Wikibus.Common;

namespace Wikibus.Nancy
{
    public static class PermissionsExtensions
    {
        public static void RequiresPermissions(this NancyModule module, params string[] permissions)
        {
            module.RequiresClaims(claim => claim.Type == "permissions" && permissions.Contains(claim.Value));
        }

        public static bool HasPermission(this NancyContextWrapper context, string permission)
        {
            return context?.Current?.CurrentUser?.HasPermission(permission) == true;
        }
    }
}
