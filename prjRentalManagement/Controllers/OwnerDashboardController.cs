using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using prjRentalManagement.Filters;
using prjRentalManagement.Models;

namespace prjRentalManagement.Controllers
{
    public class OwnerDashboardController : Controller
    {
        private readonly DbPropertyRentalEntities db = new DbPropertyRentalEntities();

        [OwnerSessionAuthorize]
        public ActionResult Index()
        {
            var ownerId = (int)Session["owner"];

            var pending = db.bookings
                .Include(b => b.apartment)
                .Include(b => b.apartment.building)
                .Include(b => b.tenant)
                .Where(b => b.status == booking.StatusPending && b.apartment.building.ownerId == ownerId)
                .OrderByDescending(b => b.requestDate)
                .ToList();

            var active = db.bookings
                .Include(b => b.apartment)
                .Include(b => b.apartment.building)
                .Include(b => b.tenant)
                .Where(b => b.status == booking.StatusConfirmed && b.apartment.building.ownerId == ownerId)
                .OrderByDescending(b => b.requestDate)
                .ToList();

            var vm = new OwnerDashboardViewModel
            {
                TotalApartments = db.apartments.Count(a => a.building.ownerId == ownerId),
                PendingRequestCount = pending.Count,
                ActiveConfirmedCount = active.Count,
                PendingBookings = pending,
                ActiveBookings = active
            };

            return View(vm);
        }

        [ChildActionOnly]
        public ActionResult OwnerNavBadge()
        {
            if (Session["owner"] == null)
                return Content(string.Empty);

            var ownerId = (int)Session["owner"];
            var c = db.bookings.Count(b =>
                b.status == booking.StatusPending &&
                db.apartments.Any(a => a.apartmentId == b.apartmentId &&
                    db.buildings.Any(bl => bl.buildingId == a.buildingId && bl.ownerId == ownerId)));

            if (c <= 0)
                return Content(string.Empty);

            return PartialView("_OwnerNavBadge", c);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
    }
}
