using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PurchaseOrderSys;
using PurchaseOrderSys.Areas.SKUSystem.Models;

namespace inventorySKU.Controllers.Product
{
    [CheckSession]
    public class SkuController : BaseController
    {
        readonly string[] EditList = new string[] { "ParentSku", "Condition", "Category", "Brand", "EAN", "UPC", "Replenishable", "Status" };

        // GET: Sku
        public ActionResult Index()
        {
            return View();
        }

        // GET: Sku/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SKU sku =db.SKU.Find(id);
            if (sku == null)
            {
                return HttpNotFound();
            }
            return View(sku);
        }

        // GET: Sku/Create
        public ActionResult Create()
        {
            var LangID = EnumData.DataLangList().First().Key;
            ViewBag.TypeList = Enum.GetValues(typeof(EnumData.SkuType)).Cast<EnumData.SkuType>().Select(t => new SelectListItem() { Text = t.ToString(), Value = ((int)t).ToString() }).ToList();
            ViewBag.Condition = db.Condition.AsNoTracking().Where(c => c.IsEnable).SelectMany(c => c.ConditionLang.Where(l => l.LangID.Equals(LangID))).Select(l => new SelectListItem() { Text = l.Name, Value = l.ConditionID.ToString() });
            ViewBag.Category = db.SkuType.Where(s => s.IsEnable).SelectMany(s => s.SkuTypeLang.Where(l => l.LangID.Equals(LangID))).Select(l => new SelectListItem() { Text = l.Name, Value = l.TypeID.ToString() });
            ViewBag.Brand = new SelectList(db.Brand.Where(b => b.IsEnable), "ID", "Name");
            return View();
        }

        // POST: Sku/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Sku sku, SkuLang langData)
        {
            if (string.IsNullOrEmpty(sku.ID))
            {
                using (StockKeepingUnit SKU = new StockKeepingUnit())
                {
                    sku.ID = SKU.GetNewSku(sku.Category, sku.Brand);
                }
            }
            else
            {
                if (db.SKU.AsNoTracking().Any(s => s.ID.Equals(sku.ID)))
                {
                    return RedirectToAction("Create");
                }
            }

            sku.CreateAt = DateTime.UtcNow;
            sku.CreateBy = Session["AdminName"].ToString();
           db.SKU.Add(sku);

            langData.Sku = sku.ID;
            langData.LangID = EnumData.DataLangList().Keys.First();
            langData.CreateAt = DateTime.UtcNow;
            langData.CreateBy = Session["AdminName"].ToString();
            db.SkuLang.Add(langData);

            db.SaveChanges();

            if (sku.Condition.Equals(1))
            {
                foreach (var condition in db.Condition.Where(c => c.IsEnable && !c.ID.Equals(sku.Condition)))
                {
                    Sku sku_suffix = new Sku()
                    {
                        IsEnable = true,
                        ID = sku.ID + condition.Suffix,
                        CreateAt = DateTime.UtcNow,
                        CreateBy = Session["AdminName"].ToString()
                    };
                    SetUpdateData(sku_suffix, sku, EditList);
                    sku_suffix.Condition = condition.ID;
                   db.SKU.Add(sku_suffix);

                    db.SkuLang.Add(new SkuLang()
                    {
                        Sku = sku_suffix.ID,
                        LangID = langData.LangID,
                        Name = langData.Name,
                        Models = langData.Models,
                        CreateAt = DateTime.UtcNow,
                        CreateBy = Session["AdminName"].ToString()
                    });
                }

                db.SaveChanges();
            }

            return RedirectToAction("Edit", new { id = sku.ID });
        }

        // GET: Sku/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Sku sku =db.SKU.Find(id);
            if (sku == null) return HttpNotFound();

            var LangID = EnumData.DataLangList().First().Key;
            ViewBag.LangID = LangID;
            ViewBag.LangList = EnumData.DataLangList().Select(l => new SelectListItem { Text = l.Value, Value = l.Key });
            ViewBag.TypeList = Enum.GetValues(typeof(EnumData.SkuType)).Cast<EnumData.SkuType>().Select(t => new SelectListItem() { Text = t.ToString(), Value = ((int)t).ToString() }).ToList();
            ViewBag.Condition = db.Condition.Where(c => c.IsEnable).SelectMany(c => c.ConditionLang.Where(l => l.LangID.Equals(LangID))).Select(l => new SelectListItem() { Text = l.Name, Value = l.ConditionID.ToString() });
            ViewBag.Category = db.SkuType.Where(s => s.IsEnable).SelectMany(s => s.SkuTypeLang.Where(l => l.LangID.Equals(LangID))).Select(l => new SelectListItem() { Text = l.Name, Value = l.TypeID.ToString() });
            ViewBag.Brand = new SelectList(db.Brand.Where(b => b.IsEnable), "ID", "Name");
            ViewBag.AttributeTypeList = db.SkuAttributeType.Include(t => t.SkuAttribute).Where(t => t.IsEnable).OrderBy(t => t.Order).ToList();
            return View(sku);
        }

        // POST: Sku/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Sku updateData, SkuLang langData)
        {
            Sku sku =db.SKU.Find(updateData.ID);
            if (sku == null) return HttpNotFound();

            SetUpdateData(sku, updateData, EditList);
            db.Entry(sku).State = EntityState.Modified;

            if (sku.SkuLang.Any(l => l.LangID.Equals(langData.LangID)))
            {
                SkuLang skuLang = sku.SkuLang.First(l => l.LangID.Equals(langData.LangID));
                SetUpdateData(skuLang, langData, new string[] { "Name", "Models" });
                db.Entry(skuLang).State = EntityState.Modified;
            }
            else
            {
                langData.Sku = sku.ID;
                langData.CreateAt = sku.UpdateAt.Value;
                langData.CreateBy = sku.UpdateBy;
                db.Entry(langData).State = EntityState.Added;
            }

            db.SaveChanges();

            var LangID = EnumData.DataLangList().First().Key;
            ViewBag.LangID = LangID;
            ViewBag.LangList = EnumData.DataLangList().Select(l => new SelectListItem { Text = l.Value, Value = l.Key, Selected = l.Key.Equals(langData.LangID) });
            ViewBag.TypeList = Enum.GetValues(typeof(EnumData.SkuType)).Cast<EnumData.SkuType>().Select(t => new SelectListItem() { Text = t.ToString(), Value = ((int)t).ToString() }).ToList();
            ViewBag.Condition = db.Condition.Where(c => c.IsEnable).SelectMany(c => c.ConditionLang.Where(l => l.LangID.Equals(LangID))).Select(l => new SelectListItem() { Text = l.Name, Value = l.ConditionID.ToString() });
            ViewBag.Category = db.SkuType.Where(s => s.IsEnable).SelectMany(s => s.SkuTypeLang.Where(l => l.LangID.Equals(LangID))).Select(l => new SelectListItem() { Text = l.Name, Value = l.TypeID.ToString() });
            ViewBag.Brand = new SelectList(db.Brand.Where(b => b.IsEnable), "ID", "Name");
            ViewBag.AttributeTypeList = db.SkuAttributeType.Where(t => t.IsEnable).OrderBy(t => t.Order).ToList();
            return View(sku);
        }

        // GET: Sku/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sku sku =db.SKU.Find(id);
            if (sku == null)
            {
                return HttpNotFound();
            }
            return View(sku);
        }

        // POST: Sku/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Sku sku =db.SKU.Find(id);
           db.SKU.Remove(sku);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult GetData(SkuFilter filter, int page = 1, int rows = 100)
        {
            int total = 0;
            List<object> dataList = new List<object>();

            var LangID = !string.IsNullOrEmpty(filter.LangID) ? filter.LangID : EnumData.DataLangList().First().Key;
            var SkuFilter =db.SKU.Include(s => s.SkuLang).AsNoTracking().AsQueryable();
            if (!string.IsNullOrEmpty(filter.ID)) SkuFilter = SkuFilter.Where(s => s.ID.Contains(filter.ID));
            if (!string.IsNullOrEmpty(filter.ParentSku)) SkuFilter = SkuFilter.Where(s => s.ParentSku.Contains(filter.ParentSku));
            if (!string.IsNullOrEmpty(filter.Name)) SkuFilter = SkuFilter.Where(s => s.SkuLang.Any(l => l.LangID.Equals(LangID) && l.Name.ToLower().Contains(filter.Name.ToLower())));
            if (filter.Condition.HasValue) SkuFilter = SkuFilter.Where(s => s.Condition.Equals(filter.Condition.Value));
            if (filter.Category.HasValue) SkuFilter = SkuFilter.Where(s => s.Category.Equals(filter.Category.Value));
            if (!string.IsNullOrEmpty(filter.UPC)) SkuFilter = SkuFilter.Where(s => s.UPC.Contains(filter.UPC));
            if (!string.IsNullOrEmpty(filter.EAN)) SkuFilter = SkuFilter.Where(s => s.EAN.Contains(filter.EAN));
            if (filter.Replenishable.HasValue) SkuFilter = SkuFilter.Where(s => s.Replenishable.Equals(filter.Replenishable.Value));
            if (filter.Status.HasValue) SkuFilter = SkuFilter.Where(s => s.Status.Equals(filter.Status.Value));

            if (SkuFilter.Any())
            {
                int length = rows;
                int start = (page - 1) * length;
                total = SkuFilter.Count();
                var results = SkuFilter.OrderByDescending(s => s.ID).Skip(start).Take(length).ToList();

                dataList.AddRange(results.Select(s => new
                {
                    s.ID,
                    s.ParentSku,
                    Name = s.SkuLang.Any(l => l.LangID.Equals(LangID)) ? s.SkuLang.First(l => l.LangID.Equals(LangID)).Name : "",
                    s.Condition,
                    s.Category,
                    s.UPC,
                    s.EAN,
                    s.Replenishable,
                    s.Status
                }).ToList());
            }

            return Json(new { total, rows = dataList }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Update(Sku updateData, SkuLang langData)
        {
            AjaxResult result = new AjaxResult();

            Sku sku =db.SKU.Find(updateData.ID);
            SetUpdateData(sku, updateData, EditList);
            db.Entry(sku).State = EntityState.Modified;

            langData.LangID = EnumData.DataLangList().First().Key;
            SkuLang skuLang = sku.SkuLang.First(l => l.LangID.Equals(langData.LangID));
            SetUpdateData(skuLang, langData, new string[] { "Name" });
            db.Entry(skuLang).State = EntityState.Modified;

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
