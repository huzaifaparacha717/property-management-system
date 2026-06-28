using System.Linq;
using System.Web.Mvc;
using prjRentalManagement.Helpers;
using prjRentalManagement.Models;

namespace prjRentalManagement.Controllers
{
    public class ManagerAccessController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("Login");
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View(new manager());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string email, string password)
        {
            var log = new manager
            {
                email = email?.Trim() ?? string.Empty,
                password = password ?? string.Empty
            };

            if (string.IsNullOrWhiteSpace(log.email) || string.IsNullOrWhiteSpace(log.password))
            {
                ModelState.Clear();
                ViewData["LoginError"] = "Please enter credentials.";
                log.password = null;
                return View(log);
            }

            var hashedPassword = PasswordHasher.Sha256(log.password);

            using (var context = new DbPropertyRentalEntities())
            {
                var storedManager = context.managers.SingleOrDefault(x => x.email == log.email);

                if (storedManager != null && storedManager.password == hashedPassword)
                {
                    Session["manager"] = storedManager.managerId;
                    Session["owner"] = null;
                    Session["tenant"] = null;
                    return RedirectToAction("Index", "Admin");
                }

                ModelState.Clear();
                ViewData["LoginError"] = "Invalid email or password.";
                log.password = null;
                return View(log);
            }
        }
    }
}
