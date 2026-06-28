using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace prjRentalManagement.Filters
{
    /// <summary>Ensures an owner is logged in (session). Use on owner-only booking/dashboard actions.</summary>
    public sealed class OwnerSessionAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            return httpContext.Session != null && httpContext.Session["owner"] != null;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary(new { controller = "Home", action = "Index" }));
        }
    }
}
