using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using prjRentalManagement.Helpers;
using prjRentalManagement.Models;

namespace prjRentalManagement.Controllers
{
    public class BuildingController : Controller
    {
        static readonly string[] PakistanProvinces =
        {
            "Punjab", "Sindh", "Khyber Pakhtunkhwa", "Balochistan",
            "Islamabad Capital Territory", "Gilgit-Baltistan", "Azad Jammu and Kashmir"
        };

        private readonly DbPropertyRentalEntities db = new DbPropertyRentalEntities();

        static void ValidateUnitCounts(building b, ModelStateDictionary modelState)
        {
            if (b.totalUnits.HasValue && b.availableUnits.HasValue && b.availableUnits.Value > b.totalUnits.Value)
                modelState.AddModelError("availableUnits", "Available units cannot be greater than total units.");
        }

        void InitProvinceList(string selected = null)
        {
            ViewBag.ProvinceList = new SelectList(PakistanProvinces, selected);
        }

        public ActionResult Index(string search)
        {
            if (Session["manager"] == null && Session["owner"] == null && Session["tenant"] == null)
                return RedirectToAction("Index", "Home");

            if (Session["tenant"] != null)
                return RedirectToAction("Index", "Apartment");

            IQueryable<building> buildings = db.buildings
                .Include(b => b.manager)
                .Include(b => b.owner)
                .Include(b => b.apartments);

            if (Session["owner"] != null)
            {
                var ownerId = (int)Session["owner"];
                buildings = buildings.Where(b => b.ownerId == ownerId);
            }

            if (!string.IsNullOrEmpty(search))
            {
                buildings = buildings.Where(b =>
                    b.address.Contains(search) ||
                    b.city.Contains(search) ||
                    b.province.Contains(search) ||
                    b.postalCode.Contains(search));
            }

            return View(buildings.ToList());
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var building = db.buildings
                .Include("manager")
                .Include("owner")
                .Include("apartments")
                .Include("apartments.tenant")
                .FirstOrDefault(b => b.buildingId == id);
            if (building == null)
                return HttpNotFound();

            if (Session["owner"] != null && building.ownerId != (int)Session["owner"])
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

            return View(building);
        }

        public ActionResult Create()
        {
            if (Session["manager"] != null)
            {
                InitProvinceList();
                ViewBag.managerId = new SelectList(db.managers, "managerId", "name");
                ViewBag.ownerId = new SelectList(db.owners, "ownerId", "name");
                return View();
            }

            if (Session["owner"] != null)
            {
                if (!db.managers.Any())
                {
                    ViewBag.Error = "No manager is registered in the system. An administrator must create a manager account first.";
                    InitProvinceList();
                    return View("CreateOwner");
                }
                InitProvinceList();
                return View("CreateOwner");
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "buildingId,address,city,province,postalCode,ownerId,managerId,totalUnits,availableUnits")] building building, HttpPostedFileBase photoFile)
        {
            if (Session["manager"] != null)
            {
                var path = BuildingImageHelper.TrySaveBuildingImage(photoFile, ModelState);
                if (!string.IsNullOrEmpty(path))
                    building.imagePath = path;

                ValidateUnitCounts(building, ModelState);
                if (ModelState.IsValid)
                {
                    db.buildings.Add(building);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                InitProvinceList(building.province);
                ViewBag.managerId = new SelectList(db.managers, "managerId", "name", building.managerId);
                ViewBag.ownerId = new SelectList(db.owners, "ownerId", "name", building.ownerId);
                return View(building);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("CreateOwner")]
        public ActionResult CreateOwnerPost([Bind(Include = "address,city,province,postalCode,totalUnits,availableUnits")] building building, HttpPostedFileBase photoFile)
        {
            if (Session["owner"] == null)
                return RedirectToAction("Index", "Home");

            var mgr = db.managers.OrderBy(m => m.managerId).FirstOrDefault();
            if (mgr == null)
            {
                ModelState.AddModelError("", "No manager is available. Please contact an administrator.");
                InitProvinceList(building.province);
                return View("CreateOwner", building);
            }

            building.ownerId = (int)Session["owner"];
            building.managerId = mgr.managerId;

            var path = BuildingImageHelper.TrySaveBuildingImage(photoFile, ModelState);
            if (!string.IsNullOrEmpty(path))
                building.imagePath = path;

            ValidateUnitCounts(building, ModelState);

            if (ModelState.IsValid)
            {
                db.buildings.Add(building);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            InitProvinceList(building.province);
            return View("CreateOwner", building);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var building = db.buildings.Find(id);
            if (building == null)
                return HttpNotFound();

            if (Session["manager"] != null)
            {
                InitProvinceList(building.province);
                ViewBag.ownerId = new SelectList(db.owners, "ownerId", "name", building.ownerId);
                return View(building);
            }

            if (Session["owner"] != null && building.ownerId == (int)Session["owner"])
            {
                InitProvinceList(building.province);
                return View("EditOwner", building);
            }

            return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "buildingId,address,city,province,postalCode,ownerId,managerId,totalUnits,availableUnits")] building incoming, HttpPostedFileBase photoFile)
        {
            if (Session["manager"] == null)
                return RedirectToAction("Index", "Home");

            var existing = db.buildings.Find(incoming.buildingId);
            if (existing == null)
                return HttpNotFound();

            var path = BuildingImageHelper.TrySaveBuildingImage(photoFile, ModelState);
            if (!string.IsNullOrEmpty(path))
            {
                BuildingImageHelper.DeleteIfExists(existing.imagePath);
                existing.imagePath = path;
            }

            existing.address = incoming.address;
            existing.city = incoming.city;
            existing.province = incoming.province;
            existing.postalCode = incoming.postalCode;
            existing.ownerId = incoming.ownerId;
            existing.totalUnits = incoming.totalUnits;
            existing.availableUnits = incoming.availableUnits;

            ValidateUnitCounts(existing, ModelState);

            if (ModelState.IsValid)
            {
                db.Entry(existing).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            InitProvinceList(existing.province);
            ViewBag.ownerId = new SelectList(db.owners, "ownerId", "name", existing.ownerId);
            return View(existing);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("EditOwner")]
        public ActionResult EditOwnerPost([Bind(Include = "buildingId,address,city,province,postalCode,totalUnits,availableUnits")] building incoming, HttpPostedFileBase photoFile)
        {
            if (Session["owner"] == null)
                return RedirectToAction("Index", "Home");

            var existing = db.buildings.Find(incoming.buildingId);
            if (existing == null || existing.ownerId != (int)Session["owner"])
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

            var path = BuildingImageHelper.TrySaveBuildingImage(photoFile, ModelState);
            if (!string.IsNullOrEmpty(path))
            {
                BuildingImageHelper.DeleteIfExists(existing.imagePath);
                existing.imagePath = path;
            }

            existing.address = incoming.address;
            existing.city = incoming.city;
            existing.province = incoming.province;
            existing.postalCode = incoming.postalCode;
            existing.totalUnits = incoming.totalUnits;
            existing.availableUnits = incoming.availableUnits;

            ValidateUnitCounts(existing, ModelState);

            if (ModelState.IsValid)
            {
                db.Entry(existing).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            InitProvinceList(existing.province);
            return View("EditOwner", existing);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var building = db.buildings.Find(id);
            if (building == null)
                return HttpNotFound();

            if (Session["manager"] != null)
                return View(building);

            if (Session["owner"] != null && building.ownerId == (int)Session["owner"])
                return View(building);

            return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var building = db.buildings.Find(id);
            if (building == null)
                return HttpNotFound();

            var isManager = Session["manager"] != null;
            var isOwner = Session["owner"] != null && building.ownerId == (int)Session["owner"];
            if (!isManager && !isOwner)
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

            BuildingImageHelper.DeleteIfExists(building.imagePath);
            db.buildings.Remove(building);
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
