using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace prjRentalManagement.Filters
{
    public sealed class TenantSessionAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            return httpContext.Session != null && httpContext.Session["tenant"] != null;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary(new { controller = "TenantAccess", action = "Login" }));
        }
    }
}
