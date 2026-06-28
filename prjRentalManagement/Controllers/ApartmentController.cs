using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using prjRentalManagement.Helpers;
using prjRentalManagement.Models;

namespace prjRentalManagement.Controllers
{
    public class ApartmentController : Controller
    {
        private readonly DbPropertyRentalEntities db = new DbPropertyRentalEntities();

        public ActionResult Index(string search, string rentStatus)
        {
            if (Session["manager"] == null && Session["tenant"] == null && Session["owner"] == null)
                return RedirectToAction("Index", "Home");

            IQueryable<apartment> apartments = db.apartments
                .Include(a => a.building)
                .Include(a => a.tenant);

            if (Session["manager"] != null)
            {
                // Manager: all apartments
            }
            else if (Session["tenant"] != null)
            {
                var tenantId = (int)Session["tenant"];
                apartments = apartments.Where(a =>
                    a.tenantId == tenantId
                    || db.bookings.Any(b => b.apartmentId == a.apartmentId && b.tenantId == tenantId && b.status == booking.StatusPending)
                    || (a.status == ApartmentStatuses.Available && a.tenantId == null
                        && !db.bookings.Any(b => b.apartmentId == a.apartmentId && b.status == booking.StatusPending && b.tenantId != tenantId)));
            }
            else if (Session["owner"] != null)
            {
                var ownerId = (int)Session["owner"];
                apartments = apartments.Where(a => a.building != null && a.building.ownerId == ownerId);
            }

            int? tenantSessionId = Session["tenant"] != null ? (int)Session["tenant"] : (int?)null;
            apartments = ApartmentListFilter.ApplySearch(apartments, search);
            if (tenantSessionId.HasValue && !string.IsNullOrWhiteSpace(rentStatus) && rentStatus.Trim().Equals("mine", StringComparison.OrdinalIgnoreCase))
            {
                var tid = tenantSessionId.Value;
                apartments = apartments.Where(a =>
                    a.tenantId == tid
                    || db.bookings.Any(b => b.apartmentId == a.apartmentId && b.tenantId == tid && b.status == booking.StatusPending));
            }
            else
                apartments = ApartmentListFilter.ApplyRentStatus(apartments, rentStatus, tenantSessionId);

            var list = apartments.ToList();
            if (tenantSessionId.HasValue)
            {
                var tid = tenantSessionId.Value;
                ViewBag.TenantCanBookApartmentIds = new System.Collections.Generic.HashSet<int>(
                    list.Where(a => BookingAccessRules.CanTenantRequestBooking(db, a, tid)).Select(a => a.apartmentId));
            }
            else
                ViewBag.TenantCanBookApartmentIds = null;

            return View(list);
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var apartment = db.apartments
                .Include(a => a.building)
                .Include(a => a.tenant)
                .Include("bookings")
                .Include("bookings.tenant")
                .FirstOrDefault(a => a.apartmentId == id);
            if (apartment == null)
                return HttpNotFound();

            if (Session["owner"] != null && apartment.building.ownerId != (int)Session["owner"])
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

            if (Session["tenant"] != null)
            {
                var tid = (int)Session["tenant"];
                if (!BookingAccessRules.CanTenantViewApartment(db, apartment, tid))
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                ViewBag.MyPendingBooking = apartment.bookings != null
                    ? apartment.bookings.FirstOrDefault(b => b.tenantId == tid && b.status == booking.StatusPending)
                    : null;
                ViewBag.CanRequestBooking = BookingAccessRules.CanTenantRequestBooking(db, apartment, tid);
            }

            return View(apartment);
        }

        [HttpGet]
        public ActionResult Book(int? id)
        {
            if (Session["tenant"] == null)
                return RedirectToAction("Login", "TenantAccess");
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var apartment = db.apartments.Include(a => a.building).FirstOrDefault(a => a.apartmentId == id);
            if (apartment == null)
                return HttpNotFound();
            if (Session["tenant"] == null)
                return RedirectToAction("Login", "TenantAccess");
            var tenantId = (int)Session["tenant"];
            if (!BookingAccessRules.CanTenantRequestBooking(db, apartment, tenantId))
            {
                TempData["Error"] = "You cannot request a booking for this apartment right now.";
                return RedirectToAction("Details", new { id });
            }

            return View(apartment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Book")]
        public ActionResult BookConfirmed(int id)
        {
            if (Session["tenant"] == null)
                return RedirectToAction("Login", "TenantAccess");

            var apartment = db.apartments.Include(a => a.building).FirstOrDefault(a => a.apartmentId == id);
            if (apartment == null)
                return HttpNotFound();
            var tenantId = (int)Session["tenant"];
            if (!BookingAccessRules.CanTenantRequestBooking(db, apartment, tenantId))
            {
                TempData["Error"] = "This apartment is not available for a new booking request.";
                return RedirectToAction("Index");
            }

            db.bookings.Add(new booking
            {
                apartmentId = id,
                tenantId = tenantId,
                status = booking.StatusPending,
                requestDate = DateTime.Now
            });

            var tenant = db.tenants.Find(tenantId);
            var building = apartment.building;
            if (building != null && tenant != null)
            {
                var who = string.IsNullOrWhiteSpace(tenant.name) ? tenant.email : tenant.name;
                var note = $"New booking request: {who} · apt #{apartment.apartmentNo} @ {building.address}, {building.city}.";
                if (note.Length > 100)
                    note = note.Substring(0, 100);

                db.messageOwners.Add(new messageOwner
                {
                    ownerId = building.ownerId,
                    managerId = building.managerId,
                    message = note
                });
            }

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                TempData["Error"] = "Could not submit request. Please try again.";
                return RedirectToAction("Index");
            }
            TempData["Message"] = "Booking request sent. The owner will approve or reject it from their dashboard.";
            return RedirectToAction("Details", new { id });
        }

        public ActionResult Create(int? buildingId)
        {
            if (Session["manager"] != null)
            {
                ViewBag.buildingId = new SelectList(db.buildings, "buildingId", "address", buildingId);
                ViewBag.tenantId = new SelectList(db.tenants, "tenantId", "name");
                return View(new apartment { status = ApartmentStatuses.Available });
            }

            if (Session["owner"] != null)
            {
                var ownerId = (int)Session["owner"];
                var buildings = db.buildings.Where(b => b.ownerId == ownerId).ToList();
                if (!buildings.Any())
                {
                    TempData["Error"] = "Add at least one building first.";
                    return RedirectToAction("Index", "Building");
                }
                int? selected = buildingId.HasValue && buildings.Any(b => b.buildingId == buildingId.Value)
                    ? buildingId
                    : null;
                ViewBag.buildingId = new SelectList(buildings, "buildingId", "address", selected);
                return View("CreateOwner");
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "apartmentId,apartmentNo,nbRooms,price,status,buildingId,tenantId")] apartment apartment)
        {
            if (Session["manager"] != null)
            {
                if (ModelState.IsValid)
                {
                    db.apartments.Add(apartment);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                ViewBag.buildingId = new SelectList(db.buildings, "buildingId", "address", apartment.buildingId);
                ViewBag.tenantId = new SelectList(db.tenants, "tenantId", "name", apartment.tenantId);
                return View(apartment);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("CreateOwner")]
        public ActionResult CreateOwnerPost([Bind(Include = "apartmentNo,nbRooms,price,status,buildingId")] apartment apartment)
        {
            if (Session["owner"] == null)
                return RedirectToAction("Index", "Home");

            var ownerId = (int)Session["owner"];
            var building = db.buildings.Find(apartment.buildingId);
            if (building == null || building.ownerId != ownerId)
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

            apartment.tenantId = null;
            if (string.IsNullOrWhiteSpace(apartment.status))
                apartment.status = ApartmentStatuses.Available;

            if (ModelState.IsValid)
            {
                db.apartments.Add(apartment);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.buildingId = new SelectList(db.buildings.Where(b => b.ownerId == ownerId), "buildingId", "address", apartment.buildingId);
            return View("CreateOwner", apartment);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var apartment = db.apartments.Include(a => a.building).FirstOrDefault(a => a.apartmentId == id);
            if (apartment == null)
                return HttpNotFound();

            if (Session["manager"] != null)
            {
                ViewBag.buildingId = new SelectList(db.buildings, "buildingId", "address", apartment.buildingId);
                ViewBag.tenantId = new SelectList(db.tenants, "tenantId", "name", apartment.tenantId);
                return View(apartment);
            }

            if (Session["owner"] != null && apartment.building.ownerId == (int)Session["owner"])
            {
                ViewBag.buildingId = new SelectList(
                    db.buildings.Where(b => b.ownerId == (int)Session["owner"]),
                    "buildingId", "address", apartment.buildingId);
                return View("EditOwner", apartment);
            }

            return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "apartmentId,apartmentNo,nbRooms,price,status,buildingId,tenantId")] apartment incoming)
        {
            if (Session["manager"] == null)
                return RedirectToAction("Index", "Home");

            if (ModelState.IsValid)
            {
                db.Entry(incoming).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.buildingId = new SelectList(db.buildings, "buildingId", "address", incoming.buildingId);
            ViewBag.tenantId = new SelectList(db.tenants, "tenantId", "name", incoming.tenantId);
            return View(incoming);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("EditOwner")]
        public ActionResult EditOwnerPost([Bind(Include = "apartmentId,apartmentNo,nbRooms,price,status,buildingId")] apartment incoming)
        {
            if (Session["owner"] == null)
                return RedirectToAction("Index", "Home");

            var existing = db.apartments.Include(a => a.building).FirstOrDefault(a => a.apartmentId == incoming.apartmentId);
            if (existing == null || existing.building.ownerId != (int)Session["owner"])
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

            var newBuilding = db.buildings.Find(incoming.buildingId);
            if (newBuilding == null || newBuilding.ownerId != (int)Session["owner"])
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

            existing.apartmentNo = incoming.apartmentNo;
            existing.nbRooms = incoming.nbRooms;
            existing.price = incoming.price;
            existing.status = incoming.status;
            existing.buildingId = incoming.buildingId;

            if (ModelState.IsValid)
            {
                db.Entry(existing).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.buildingId = new SelectList(
                db.buildings.Where(b => b.ownerId == (int)Session["owner"]),
                "buildingId", "address", existing.buildingId);
            return View("EditOwner", existing);
        }

        public ActionResult Delete(int? id)
        {
            if (Session["manager"] == null)
                return RedirectToAction("Index", "Home");
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var apartment = db.apartments.Find(id);
            if (apartment == null)
                return HttpNotFound();
            return View(apartment);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["manager"] == null)
                return RedirectToAction("Index", "Home");

            var apartment = db.apartments.Find(id);
            if (apartment == null)
                return HttpNotFound();

            if (db.eventOwners.Any(e => e.apartmentId == id))
            {
                TempData["ErrorMessage"] = "Delete related events first.";
                return RedirectToAction("Delete", new { id });
            }

            if (db.bookings.Any(b => b.apartmentId == id))
            {
                TempData["ErrorMessage"] = "Delete related booking records first.";
                return RedirectToAction("Delete", new { id });
            }

            db.apartments.Remove(apartment);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }

    }
}
