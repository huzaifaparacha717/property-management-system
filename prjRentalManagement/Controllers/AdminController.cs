using System.Web.Mvc;

namespace prjRentalManagement.Controllers
{
    /// <summary>Manager-only admin dashboard (manage all entities).</summary>
    public class AdminController : Controller
    {
        public ActionResult Index()
        {
            if (Session["manager"] == null)
                return RedirectToAction("Login", "ManagerAccess");
            return View();
        }
    }
}
