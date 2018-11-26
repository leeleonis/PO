using PurchaseOrderSys.Areas.SKUSystem.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace inventorySKU.Controllers
{
    [CheckSession]
    public class AdminUsersController : BaseController
    {
        public AdminUsersController()
        {
            ViewBag.menu = db.Menu.Include(m => m.MenuParent);
        }
        // GET: AdminUsers
        public ActionResult Index()
        {
            var adminUser = db.AdminUser.Include(a => a.AdminGroup);
            return View(adminUser.ToList());
        }

        // GET: AdminUsers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AdminUser adminUser = db.AdminUser.Find(id);
            if (adminUser == null)
            {
                return HttpNotFound();
            }
            return View(adminUser);
        }

        // GET: AdminUsers/Create
        public ActionResult Create()
        {
            ViewBag.Group = new SelectList(db.AdminGroup, "ID", "Name");
            return View();
        }

        // POST: AdminUsers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Group,Name,Account,Password")] AdminUser adminUser, Dictionary<string, List<string>> auth)
        {
            adminUser.CreateBy = "Test";
            adminUser.CreateAt = DateTime.UtcNow;
            ModelState.Remove("CreateBy");
            if (ModelState.IsValid)
            {
                adminUser.Auth = AuthToString(auth);
                db.AdminUser.Add(adminUser);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Group = new SelectList(db.AdminGroup, "ID", "Name", adminUser.Group);
            return View(adminUser);
        }

        // GET: AdminUsers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AdminUser adminUser = db.AdminUser.Find(id);
            if (adminUser == null)
            {
                return HttpNotFound();
            }
            ViewBag.Group = new SelectList(db.AdminGroup, "ID", "Name", adminUser.Group);
            return View(adminUser);
        }

        // POST: AdminUsers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Group,Name,Account")] AdminUser EadminUser, Dictionary<string, List<string>> auth)
        {
            var oadminUser = db.AdminUser.Find(EadminUser.ID);
            oadminUser.Group = EadminUser.Group;
            oadminUser.Name = EadminUser.Name;
            oadminUser.Account = EadminUser.Account;
            oadminUser.Auth = AuthToString(auth);
            db.Entry(oadminUser).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }



        // GET: AdminUsers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AdminUser adminUser = db.AdminUser.Find(id);
            if (adminUser == null)
            {
                return HttpNotFound();
            }
            return View(adminUser);
        }

        // POST: AdminUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AdminUser adminUser = db.AdminUser.Find(id);
            db.AdminUser.Remove(adminUser);
            db.SaveChanges();
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
    }
}
