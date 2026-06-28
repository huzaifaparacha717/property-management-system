using System.Linq;
using System.Web.Mvc;
using prjRentalManagement.Helpers;
using prjRentalManagement.Models;

namespace prjRentalManagement.Controllers
{
    public class OwnerAccessController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("Login");
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View(new owner());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string email, string password)
        {
            var log = new owner
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
                var isValid = context.owners.Any(x => x.email == log.email && x.password == hashedPassword);
                if (isValid)
                {
                    Session["owner"] = context.owners.Single(i => i.email == log.email).ownerId;
                    Session["manager"] = null;
                    Session["tenant"] = null;
                    return RedirectToAction("Index", "Owner");
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
            return View(new owner());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register([Bind(Include = "name,email,password,phoneNumber")] owner owner)
        {
            if (owner == null)
                owner = new owner();

            if (string.IsNullOrWhiteSpace(owner.name) && string.IsNullOrWhiteSpace(owner.email)
                && string.IsNullOrWhiteSpace(owner.password) && string.IsNullOrWhiteSpace(owner.phoneNumber))
            {
                ModelState.Clear();
                ModelState.AddModelError(string.Empty, "Please enter credentials.");
                return View(owner);
            }

            using (var db = new DbPropertyRentalEntities())
            {
                if (db.owners.Any(o => o.email == owner.email))
                    ModelState.AddModelError("email", "This email is already registered.");

                if (!ModelState.IsValid)
                    return View(owner);

                owner.password = PasswordHasher.Sha256(owner.password);
                db.owners.Add(owner);
                db.SaveChanges();
                TempData["Message"] = "Registration successful. Please sign in.";
                return RedirectToAction("Login");
            }
        }
    }
}
