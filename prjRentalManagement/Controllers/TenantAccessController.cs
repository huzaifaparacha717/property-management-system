using System.Linq;
using System.Web.Mvc;
using prjRentalManagement.Helpers;
using prjRentalManagement.Models;

namespace prjRentalManagement.Controllers
{
    public class TenantAccessController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("Login");
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View(new tenant());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string email, string password)
        {
            var log = new tenant
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

            using (var context = new DbPropertyRentalEntities())
            {
                var hashedPassword = PasswordHasher.Sha256(log.password);
                var isValid = context.tenants.Any(x => x.email == log.email && x.password == hashedPassword);
                if (isValid)
                {
                    Session["tenant"] = context.tenants.Single(i => i.email == log.email).tenantId;
                    Session["owner"] = null;
                    Session["manager"] = null;
                    return RedirectToAction("Index", "Tenant");
                }

                ModelState.Clear();
                ViewData["LoginError"] = "Invalid email or password.";
                log.password = null;
                return View(log);
            }
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View(new tenant());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register([Bind(Include = "name,email,password,phoneNumber")] tenant tenant)
        {
            if (tenant == null)
                tenant = new tenant();

            if (string.IsNullOrWhiteSpace(tenant.name) && string.IsNullOrWhiteSpace(tenant.email)
                && string.IsNullOrWhiteSpace(tenant.password) && string.IsNullOrWhiteSpace(tenant.phoneNumber))
            {
                ModelState.Clear();
                ModelState.AddModelError(string.Empty, "Please enter credentials.");
                return View(tenant);
            }

            using (var db = new DbPropertyRentalEntities())
            {
                if (db.tenants.Any(t => t.email == tenant.email))
                    ModelState.AddModelError("email", "This email is already registered.");

                if (!ModelState.IsValid)
                    return View(tenant);

                tenant.password = PasswordHasher.Sha256(tenant.password);
                db.tenants.Add(tenant);
                db.SaveChanges();
                TempData["Message"] = "Registration successful. Please sign in.";
                return RedirectToAction("Login");
            }
        }
    }
}
