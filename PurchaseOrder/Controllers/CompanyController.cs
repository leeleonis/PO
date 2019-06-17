using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using inventorySKU;
using PurchaseOrderSys.Models;

namespace PurchaseOrderSys.Controllers
{
    [CheckSession]
    public class CompanyController : BaseController
    {
        // GET: Company
        public ActionResult Index()
        {
            return View();
        }

        // GET: Company/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Company company = db.Company.Find(id);
            if (company == null)
            {
                return HttpNotFound();
            }
            return View(company);
        }

        // GET: Company/Create
        public ActionResult Create()
        {
            var CompanyList = new List<SelectListItem>() { new SelectListItem() { Text = "Not Choose", Value = "" } };
            CompanyList.AddRange(db.Company.AsNoTracking().Where(c => c.IsEnable).Select(c => new SelectListItem() { Text = c.Name, Value = c.ID.ToString() }).ToList());

            ViewBag.CompanyList = CompanyList;
            return View();
        }

        // POST: Company/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name,ShandowSuffix,ParentID,RelateID,eBayAccountID,AmazonAccountID")] Company company)
        {
            company.IsEnable = true;
            company.CreateAt = DateTime.UtcNow;
            company.CreateBy = "Test";

            db.Company.Add(company);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Company/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Company company = db.Company.Find(id);
            if (company == null)
            {
                return HttpNotFound();
            }
            return View(company);
        }

        // POST: Company/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IsEnable,ID,Name,ShandowSuffix,ParentID,RelateID,eBayAccountID,AmazonAccountID,CreateBy,CreateAt,UpdateBy,UpdateAt")] Company company)
        {
            if (ModelState.IsValid)
            {
                db.Entry(company).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(company);
        }

        // GET: Company/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Company company = db.Company.Find(id);
            if (company == null)
            {
                return HttpNotFound();
            }
            return View(company);
        }

        // POST: Company/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Company company = db.Company.Find(id);
            db.Company.Remove(company);
            db.SaveChanges();
            return RedirectToAction("Index");

        }

        public ActionResult GetData(CompanyFilter filter, int page = 1, int rows = 100)
        {
            int total = 0;
            List<object> dataList = new List<object>();

            var CompanyFilter = db.Company.AsNoTracking().AsQueryable();
            if (filter.ID.HasValue) CompanyFilter = CompanyFilter.Where(c => c.ID.Equals(filter.ID.Value));
            if (!string.IsNullOrEmpty(filter.Name)) CompanyFilter = CompanyFilter.Where(c => c.Name.ToLower().Contains(filter.Name.ToLower()));
            if (!string.IsNullOrEmpty(filter.ShandowSuffix)) CompanyFilter = CompanyFilter.Where(c => c.ShadowSuffix.ToLower().Contains(filter.ShandowSuffix.ToLower()));
            if (filter.ParentID.HasValue) CompanyFilter = CompanyFilter.Where(c => c.ParentID.Value.Equals(filter.ParentID.Value));
            if (filter.RelateID.HasValue) CompanyFilter = CompanyFilter.Where(c => c.RelateID.Value.Equals(filter.RelateID.Value));
            if (!string.IsNullOrEmpty(filter.eBayAccountID)) CompanyFilter = CompanyFilter.Where(c => c.eBayAccountID.ToLower().Contains(filter.eBayAccountID.ToLower()));
            if (!string.IsNullOrEmpty(filter.AmazonAccountID)) CompanyFilter = CompanyFilter.Where(c => c.AmazonAccountID.ToLower().Contains(filter.AmazonAccountID.ToLower()));
            var CompanyList = CompanyFilter.Select(x => new  {
                AmazonAccountID = x.AmazonAccountID,
                CompanyNo = x.CompanyNo,
                eBayAccountID = x.eBayAccountID,
                ID = x.ID,
                Name = x.Name,
                ShandowSuffix = x.ShadowSuffix,
                ParentID = x.ParentID.ToString(),
                RelateID = x.RelateID.ToString(),
                x.CreateAt
            }).ToList();
            if (CompanyList.Any())
            {
                int length = rows;
                int start = (page - 1) * length;
                total = CompanyFilter.Count();
                var results = CompanyList.OrderByDescending(c => c.CreateAt).Skip(start).Take(length).ToList();

                dataList.AddRange(results);
            }
            return Json(new { total, rows = dataList.ToList() }, JsonRequestBehavior.AllowGet);
        }

        //[HttpPost]
        public ActionResult Update(CompanyVM updateData)
        {
            AjaxResult result = new AjaxResult();

            Company Company = db.Company.Find(updateData.ID);
            Company.Name = updateData.Name;
            Company.ShadowSuffix = updateData.ShandowSuffix;
            Company.ParentID = updateData.ParentID;
            Company.RelateID = updateData.RelateID;
            Company.eBayAccountID = updateData.eBayAccountID;
            Company.AmazonAccountID = updateData.AmazonAccountID;
            Company.UpdateAt = DateTime.UtcNow;
            Company.UpdateBy = Session["AdminName"].ToString();
            db.SaveChanges();

            return Json(result, JsonRequestBehavior.AllowGet);
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
