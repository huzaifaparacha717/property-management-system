using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using prjRentalManagement.Models;
using System.Security.Cryptography;
using System.Text;

namespace prjRentalManagement.Controllers
{
    public class ManagerController : Controller
    {
        private DbPropertyRentalEntities db = new DbPropertyRentalEntities();

        // GET: Manager — list all managers (admin session)
        public ActionResult Index(string searchQuery)
        {
            if (Session["manager"] == null)
                return RedirectToAction("Index", "Home");

            var managers = db.managers.AsQueryable();
            if (!string.IsNullOrEmpty(searchQuery))
                managers = managers.Where(m => m.email.Contains(searchQuery));
            return View(managers.ToList());
        }

        // GET: Manager/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["manager"] == null)
                return RedirectToAction("Index", "Home");
            manager manager = db.managers.Find(id);
            if (manager == null)
            {
                return HttpNotFound();
            }
            return View(manager);
        }

        // GET: Manager/Create
        public ActionResult Create()
        {
            if (Session["manager"] == null)
                return RedirectToAction("Index", "Home");
            return View();
        }

        // POST: Manager/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "managerId,name,email,password,phoneNumber")] manager manager)
        {
            if (Session["manager"] == null)
                return RedirectToAction("Index", "Home");

            // Check if the email already exists in the database
            if (db.managers.Any(m => m.email == manager.email))
            {
                // Add a custom error message to the ModelState
                ModelState.AddModelError("email", "This email is already in use. Please use a different email.");
            }

            // Proceed only if the ModelState is valid
            if (ModelState.IsValid)
            {
                // Hash the password
                manager.password = ComputeSha256Hash(manager.password);

                // Save the manager to the database
                db.managers.Add(manager);
                db.SaveChanges();

                // Redirect to the Manager Access page or any other appropriate action
                return RedirectToAction("Index", "Admin");
            }

            // If validation fails, return to the Create view with the current data
            return View(manager);
        }

        // GET: Manager/Edit/5 ->wrap manager info sent to the Edit view through ID
        public ActionResult Edit(int? id)
        {
            if (Session["manager"] == null)
                return RedirectToAction("Index", "Home");

            manager manager = db.managers.Find(id);
            if (manager == null)
            {
                return HttpNotFound();
            }

            return View(manager);
        }

        // POST: Manager/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "managerId,name,email,password,phoneNumber")] manager manager)
        {
            if (Session["manager"] == null)
                return RedirectToAction("Index", "Home");

            // Step 3: Fetch the existing manager record from the database
            var existingManager = db.managers.Find(manager.managerId);
            if (existingManager == null)
            {
                return HttpNotFound(); // Return 404 Error page if the manager doesn't exist
            }

            // Step 4: Check if the email is already used by another manager
            if (db.managers.Any(m => m.email == manager.email && m.managerId != manager.managerId))
            {
                ModelState.AddModelError("email", "This email is already in use by another manager. Please use a different email.");
            }

            // Step 5: Update non-password fields (name, email, phoneNumber)
            existingManager.name = manager.name;
            existingManager.phoneNumber = manager.phoneNumber;

            // Step 6: Handle password updates or retention
            if (string.IsNullOrWhiteSpace(Request.Form["password"]))
            {
                // Retain the existing password if no new password is provided
                ModelState.Remove("password"); // Remove validation error for password
            }
            else
            {
                string newPassword = Request.Form["password"];
                existingManager.password = ComputeSha256Hash(newPassword);
            }

            // Step 7: Save changes to the database
            if (ModelState.IsValid)
            {
                // Update the email only if ModelState is valid
                existingManager.email = manager.email;

                db.Entry(existingManager).State = EntityState.Modified;
                db.SaveChanges();

                // Step 8: Redirect back to the Index page after a successful save
                return RedirectToAction("Index");
            }

            // Step 9: If validation fails, return to the Edit view with the current data
            return View(manager);
        }

        // GET: Manager/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["manager"] == null)
                return RedirectToAction("Index", "Home");
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            manager manager = db.managers.Find(id);
            if (manager == null)
                return HttpNotFound();
            return View(manager);
        }

        // POST: Manager/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            manager manager = db.managers.Find(id);

           if (manager == null)
            {
                return HttpNotFound();
            }
            if (Session["manager"] == null)
                return RedirectToAction("Index", "Home");

            var currentId = (int)Session["manager"];
            db.managers.Remove(manager);
            db.SaveChanges();

            if (currentId == id)
            {
                Session["manager"] = null;
                return RedirectToAction("Login", "ManagerAccess");
            }

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        //Helper method to compute SHA256 hash
        private string ComputeSha256Hash(String rawData)
        {
            //Create a SHA256
            using (SHA256 sha256Hash = SHA256.Create())
            {
                //ComputeHash - returns byte array
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                //Convert byte array to a string
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }

                return builder.ToString();
            }
        }
    }
}
