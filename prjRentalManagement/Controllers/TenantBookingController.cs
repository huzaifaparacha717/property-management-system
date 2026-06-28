using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using prjRentalManagement.Filters;
using prjRentalManagement.Models;

namespace prjRentalManagement.Controllers
{
    [TenantSessionAuthorize]
    public class TenantBookingController : Controller
    {
        private readonly DbPropertyRentalEntities db = new DbPropertyRentalEntities();

        public ActionResult Index()
        {
            var tenantId = (int)Session["tenant"];
            var list = db.bookings
                .Include(b => b.apartment)
                .Include(b => b.apartment.building)
                .Where(b => b.tenantId == tenantId)
                .OrderByDescending(b => b.requestDate)
                .ToList();
            return View(list);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
    }
}
