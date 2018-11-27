using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using PurchaseOrderSys.Models;

namespace PurchaseOrderSys.Controllers
{

    //[CheckSession]
    public class AdminGroupsController : BaseController
    {
        string[] EditList = new string[] { "Name", "Auth" };
        public AdminGroupsController()
        {
            ViewBag.menu = db.Menu.Include(m => m.MenuParent);
        }
        // GET: AdminGroups
        public ActionResult Index()
        {
            return View(db.AdminGroup.ToList());
        }

        // GET: AdminGroups/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AdminGroup adminGroup = db.AdminGroup.Find(id);
            if (adminGroup == null)
            {
                return HttpNotFound();
            }
            return View(adminGroup);
        }

        // GET: AdminGroups/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AdminGroups/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name")] AdminGroup adminGroup, Dictionary<string, List<string>> auth)
        {
            adminGroup.CreateBy = "Test";
            adminGroup.CreateAt = DateTime.UtcNow;
            ModelState.Remove("CreateBy");
            adminGroup.Auth = AuthToString(auth);
            if (ModelState.IsValid)
            {
                db.AdminGroup.Add(adminGroup);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(adminGroup);
        }

        // GET: AdminGroups/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AdminGroup adminGroup = db.AdminGroup.Find(id);
            if (adminGroup == null)
            {
                return HttpNotFound();
            }
            return View(adminGroup);
        }

        // POST: AdminGroups/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AdminGroup adminGroup, Dictionary<string, List<string>> auth)
        {
            AdminGroup OadminGroup = db.AdminGroup.Find(adminGroup.ID);
            adminGroup.Auth = AuthToString(auth);
            SetEditDatas(OadminGroup,adminGroup, EditList);
            //db.Entry(OadminGroup).State = EntityState.Modified;
            //db.SaveChanges();
            return RedirectToAction("Index");

        }

        

        // GET: AdminGroups/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AdminGroup adminGroup = db.AdminGroup.Find(id);
            if (adminGroup == null)
            {
                return HttpNotFound();
            }
            return View(adminGroup);
        }

        // POST: AdminGroups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AdminGroup adminGroup = db.AdminGroup.Find(id);
            db.AdminGroup.Remove(adminGroup);
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
