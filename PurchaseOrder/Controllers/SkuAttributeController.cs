using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using PurchaseOrderSys.Models;
namespace PurchaseOrderSys.Controllers
{
    //[CheckSession]
    public class SkuAttributeController : BaseController
    {
        // GET: SkuAttribute
        public ActionResult Index()
        {
            return View();
        }

        // GET: SkuAttribute/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SkuAttribute skuAttribute = db.SkuAttribute.Find(id);
            if (skuAttribute == null)
            {
                return HttpNotFound();
            }
            return View(skuAttribute);
        }

        // GET: SkuAttribute/Create
        public ActionResult Create()
        {
            ViewBag.Type = new SelectList(db.SkuAttributeType.Where(t => t.IsEnable), "ID", "Name");
            ViewBag.Property = Enum.GetValues(typeof(EnumData.AttributeProperty)).Cast<EnumData.AttributeProperty>().Select(p => new SelectListItem() { Text = p.ToString(), Value = ((int)p).ToString() }).ToList();
            return View();
        }

        // POST: SkuAttribute/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Type")] SkuAttribute skuAttribute, [Bind(Include = "Name")] SkuAttributeLang langData)
        {
            skuAttribute.IsEnable = true;
            skuAttribute.CreateAt = DateTime.UtcNow;
            skuAttribute.CreateBy = Session["AdminName"].ToString();

            db.SkuAttribute.Add(skuAttribute);
            db.SaveChanges();

            langData.AttrID = skuAttribute.ID;
            langData.LangID = EnumData.DataLangList().Keys.First();
            langData.CreateAt = DateTime.UtcNow;
            langData.CreateBy = Session["AdminName"].ToString();

            db.SkuAttributeLang.Add(langData);
            db.SaveChanges();

            return RedirectToAction("Edit", new { id = skuAttribute.ID });
        }

        // GET: SkuAttribute/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            SkuAttribute skuAttribute = db.SkuAttribute.Find(id);
            if (skuAttribute == null) return HttpNotFound();

            ViewBag.LangID = EnumData.DataLangList().First().Key;
            ViewBag.LangList = EnumData.DataLangList().Select(l => new SelectListItem { Text = l.Value, Value = l.Key });
            ViewBag.Type = new SelectList(db.SkuAttributeType, "ID", "Name", skuAttribute.Type);
            ViewBag.Property = Enum.GetValues(typeof(EnumData.AttributeProperty)).Cast<EnumData.AttributeProperty>().Select(p => new SelectListItem() { Text = p.ToString(), Value = ((int)p).ToString() }).ToList();
            return View(skuAttribute);
        }

        // POST: SkuAttribute/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SkuAttribute updateData, [Bind(Include = "LangID,Name")] SkuAttributeLang langData)
        {
            SkuAttribute attribute = db.SkuAttribute.Find(updateData.ID);
            if (attribute == null) return HttpNotFound();

            attribute.Type = updateData.Type;
            attribute.Property = updateData.Property;
            attribute.UpdateAt = DateTime.UtcNow;
            attribute.UpdateBy = Session["AdminName"].ToString();
            db.Entry(attribute).State = EntityState.Modified;

            if (attribute.SkuAttributeLang.Any(l => l.LangID.Equals(langData.LangID)))
            {
                var attributeLang = attribute.SkuAttributeLang.First(l => l.LangID.Equals(langData.LangID));
                attributeLang.Name = langData.Name;
                attributeLang.UpdateAt = attribute.UpdateAt.Value;
                attributeLang.UpdateBy = attribute.UpdateBy;
                db.Entry(attributeLang).State = EntityState.Modified;
            }
            else
            {
                langData.AttrID = attribute.ID;
                langData.CreateAt = attribute.UpdateAt.Value;
                langData.CreateBy = attribute.UpdateBy;
                db.Entry(langData).State = EntityState.Added;
            }

            db.SaveChanges();

            ViewBag.LangID = langData.LangID;
            ViewBag.LangList = EnumData.DataLangList().Select(l => new SelectListItem { Text = l.Value, Value = l.Key, Selected = l.Key.Equals(langData.LangID) });
            ViewBag.Type = new SelectList(db.SkuAttributeType, "ID", "Name", attribute.Type);
            ViewBag.Property = Enum.GetValues(typeof(EnumData.AttributeProperty)).Cast<EnumData.AttributeProperty>().Select(p => new SelectListItem() { Text = p.ToString(), Value = ((int)p).ToString() }).ToList();
            return View(attribute);
        }

        // GET: SkuAttribute/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SkuAttribute skuAttribute = db.SkuAttribute.Find(id);
            if (skuAttribute == null)
            {
                return HttpNotFound();
            }
            return View(skuAttribute);
        }

        // POST: SkuAttribute/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SkuAttribute skuAttribute = db.SkuAttribute.Find(id);
            db.SkuAttribute.Remove(skuAttribute);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult GetData(SkuAttributeFilter filter, int page = 1, int rows = 100)
        {
            int total = 0;
            List<object> dataList = new List<object>();

            var LangID = !string.IsNullOrEmpty(filter.LangID) ? filter.LangID : EnumData.DataLangList().First().Key;
            var AttributeFilter = db.SkuAttribute.Include(a => a.SkuAttributeLang).AsQueryable();
            if (filter.ID.HasValue) AttributeFilter = AttributeFilter.Where(a => a.ID.Equals(filter.ID.Value));
            if (!string.IsNullOrEmpty(filter.Name)) AttributeFilter = AttributeFilter.Where(a => a.SkuAttributeLang.Any(l => l.LangID.Equals(LangID) && l.Name.ToLower().Contains(filter.Name.ToLower())));
            if (filter.Type.HasValue) AttributeFilter = AttributeFilter.Where(a => a.Type.Equals(filter.Type.Value));
            if (filter.Property.HasValue) AttributeFilter = AttributeFilter.Where(a => a.Property.Equals(filter.Property.Value));

            if (AttributeFilter.Any())
            {
                int length = rows;
                int start = (page - 1) * length;
                total = AttributeFilter.Count();
                var results = AttributeFilter.OrderByDescending(a => a.CreateAt).Skip(start).Take(length).ToList();

                dataList.AddRange(results.Select(a => new
                {
                    a.ID,
                    Name = a.SkuAttributeLang.Any(l => l.LangID.Equals(LangID)) ? a.SkuAttributeLang.First(l => l.LangID.Equals(LangID)).Name : "",
                    a.Type,
                    a.Property
                }).ToList());
            }

            return Json(new { total, rows = dataList }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult FilterAttribute(string filter, string parentSku)
        {
            var langID = EnumData.DataLangList().First().Key;

            var attributeFilter = db.SkuAttribute.Include(a => a.SkuAttributeLang).Where(a => a.IsEnable);
            if (!string.IsNullOrEmpty(parentSku))
            {
                SKU sku = db.SKU.Find(parentSku);
                if(sku != null)
                {
                    int[] attrIDs = JsonConvert.DeserializeObject<Dictionary<int, int[]>>(sku.SkuType.AttributeGroup).SelectMany(g => g.Value).ToArray();
                    attributeFilter = attributeFilter.Where(a => attrIDs.Contains(a.ID));
                }
            }
            var attributeList = attributeFilter.Where(a => a.SkuAttributeLang.Any(l => l.Name.Contains(filter))).ToList();

            var data = attributeList.Select(a => new { value = a.SkuAttributeLang.First(l => l.LangID.Equals(langID)).Name, attrID = a.ID }).ToArray();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Update(SkuAttribute updateData, SkuAttributeLang langData)
        {
            AjaxResult result = new AjaxResult();

            SkuAttribute attribute = db.SkuAttribute.Find(updateData.ID);
            attribute.Type = updateData.Type;
            attribute.Property = updateData.Property;
            attribute.UpdateAt = DateTime.UtcNow;
            attribute.UpdateBy = Session["AdminName"].ToString();
            db.Entry(attribute).State = EntityState.Modified;

            langData.LangID = EnumData.DataLangList().First().Key;
            SkuAttributeLang attributeLang = attribute.SkuAttributeLang.First(l => l.LangID.Equals(langData.LangID));
            attributeLang.Name = langData.Name;
            attributeLang.UpdateAt = attribute.UpdateAt.Value;
            attributeLang.UpdateBy = attribute.UpdateBy;
            db.Entry(attributeLang).State = EntityState.Modified;

            db.SaveChanges();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLangData(int ID, string LangID)
        {
            AjaxResult result = new AjaxResult();

            result.data = new { db.SkuAttributeLang.FirstOrDefault(l => l.AttrID.Equals(ID) && l.LangID.Equals(LangID))?.Name };

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
