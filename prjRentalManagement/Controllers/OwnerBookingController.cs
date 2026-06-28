using System.Net;
using System.Web.Mvc;
using prjRentalManagement.Filters;
using prjRentalManagement.Models;
using prjRentalManagement.Services;

namespace prjRentalManagement.Controllers
{
    /// <summary>Owner booking actions (HTML + AJAX). Authorization: owner session only.</summary>
    [OwnerSessionAuthorize]
    public class OwnerBookingController : Controller
    {
        private readonly DbPropertyRentalEntities db = new DbPropertyRentalEntities();

        public ActionResult Index()
        {
            return RedirectToAction("Index", "OwnerDashboard");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Approve(int id)
        {
            var ownerId = (int)Session["owner"];
            var svc = new BookingWorkflowService(db);
            var r = svc.Approve(ownerId, id);
            TempData[r.Success ? "Message" : "Error"] = r.Message;
            return RedirectToAction("Index", "OwnerDashboard");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Reject(int id)
        {
            var ownerId = (int)Session["owner"];
            var svc = new BookingWorkflowService(db);
            var r = svc.Reject(ownerId, id);
            TempData[r.Success ? "Message" : "Error"] = r.Message;
            return RedirectToAction("Index", "OwnerDashboard");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Complete(int id)
        {
            var ownerId = (int)Session["owner"];
            var svc = new BookingWorkflowService(db);
            var r = svc.CompleteRental(ownerId, id);
            TempData[r.Success ? "Message" : "Error"] = r.Message;
            return RedirectToAction("Index", "OwnerDashboard");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ApproveAjax(int id)
        {
            var ownerId = (int)Session["owner"];
            var svc = new BookingWorkflowService(db);
            var r = svc.Approve(ownerId, id);
            return Json(new { success = r.Success, message = r.Message });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RejectAjax(int id)
        {
            var ownerId = (int)Session["owner"];
            var svc = new BookingWorkflowService(db);
            var r = svc.Reject(ownerId, id);
            return Json(new { success = r.Success, message = r.Message });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CompleteAjax(int id)
        {
            var ownerId = (int)Session["owner"];
            var svc = new BookingWorkflowService(db);
            var r = svc.CompleteRental(ownerId, id);
            return Json(new { success = r.Success, message = r.Message });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
    }
}
