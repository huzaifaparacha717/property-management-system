using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using prjRentalManagement.Helpers;
using prjRentalManagement.Models;

namespace prjRentalManagement.Controllers
{
    public class HomeController : Controller
    {
        readonly DbPropertyRentalEntities db = new DbPropertyRentalEntities();

        public ActionResult Index(string search, string rentStatus)
        {
            int? tenantId = Session["tenant"] != null ? (int)Session["tenant"] : (int?)null;

            IQueryable<apartment> q = db.apartments
                .Include(a => a.building)
                .OrderByDescending(a => a.apartmentId);

            if (tenantId.HasValue)
            {
                var tid = tenantId.Value;
                q = q.Where(a =>
                    a.tenantId == tid
                    || db.bookings.Any(b => b.apartmentId == a.apartmentId && b.tenantId == tid && b.status == booking.StatusPending)
                    || (a.status == "Available" && a.tenantId == null
                        && !db.bookings.Any(b => b.apartmentId == a.apartmentId && b.status == booking.StatusPending && b.tenantId != tid)));
            }

            q = ApartmentListFilter.ApplySearch(q, search);
            if (tenantId.HasValue && !string.IsNullOrWhiteSpace(rentStatus) && rentStatus.Trim().Equals("mine", StringComparison.OrdinalIgnoreCase))
            {
                var tid = tenantId.Value;
                q = q.Where(a =>
                    a.tenantId == tid
                    || db.bookings.Any(b => b.apartmentId == a.apartmentId && b.tenantId == tid && b.status == booking.StatusPending));
            }
            else
                q = ApartmentListFilter.ApplyRentStatus(q, rentStatus, tenantId);

            return View(q.ToList());
        }

        public ActionResult Logout()
        {
            Session["owner"] = null;
            Session["manager"] = null;
            Session["tenant"] = null;
            return RedirectToAction("Index");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
    }
}
