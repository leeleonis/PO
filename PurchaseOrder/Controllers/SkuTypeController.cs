using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PurchaseOrderSys.Models;

namespace PurchaseOrderSys.Controllers
{
    //[CheckSession]
    public class SkuTypeController : BaseController
    {
        // GET: SkuTypes
        public ActionResult Index()
        {
            return View();
        }

        // GET: SkuTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SkuType skuType = db.SkuType.Find(id);
            if (skuType == null)
            {
                return HttpNotFound();
            }
            return View(skuType);
        }

        // GET: SkuTypes/Create
        public ActionResult Create()
        {
            ViewBag.LangID = EnumData.DataLangList().First().Key;
            ViewBag.AttributeTypeList = db.SkuAttributeType.Include(t => t.SkuAttribute.Select(a => a.SkuAttributeLang)).Where(t => t.IsEnable).OrderBy(t => t.Order).ToList();
            return View();
        }

        // POST: SkuTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IsEnable,NetoID")] SkuType skuType, SkuTypeLang langData, string[] PackageContent, Dictionary<string, int[]> AttributeGroup)
        {
            skuType.AttributeGroup = Newtonsoft.Json.JsonConvert.SerializeObject(AttributeGroup ?? new Dictionary<string, int[]> { });
            skuType.CreateAt = DateTime.UtcNow;
            skuType.CreateBy = Session["AdminName"].ToString();

            langData.TypeID = skuType.ID;
            langData.LangID = EnumData.DataLangList().Keys.First();
            langData.CreateAt = skuType.CreateAt;
            langData.CreateBy = skuType.CreateBy;

            skuType.SkuTypeLang.Add(langData);
            if (PackageContent.Any())
            {
                foreach (string content in PackageContent)
                {
                    PackageContent newPC = new PackageContent() { IsEnable = true, CreateAt = skuType.CreateAt, CreateBy = skuType.CreateBy };
                    newPC.PackageContentLang.Add(new PackageContentLang()
                    {
                        LangID = langData.LangID,
                        Name = content,
                        CreateAt = skuType.CreateAt,
                        CreateBy = skuType.CreateBy
                    });
                    skuType.PackageContent.Add(newPC);
                }
            }
            db.SkuType.Add(skuType);
            db.SaveChanges();

            return RedirectToAction("Edit", new { id = skuType.ID });
        }

        // GET: SkuTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            SkuType skuType = db.SkuType.Find(id);
            if (skuType == null) return HttpNotFound();

            ViewBag.LangID = EnumData.DataLangList().First().Key;
            ViewBag.LangList = EnumData.DataLangList().Select(l => new SelectListItem { Text = l.Value, Value = l.Key });
            ViewBag.AttributeTypeList = db.SkuAttributeType.Include(t => t.SkuAttribute.Select(a => a.SkuAttributeLang)).Where(t => t.IsEnable).OrderBy(t => t.Order).ToList();
            return View(skuType);
        }

        // POST: SkuTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,NetoID")] SkuType updateData, [Bind(Include = "LangID,Name")] SkuTypeLang langData, PackageContentLang[] PackageContent, Dictionary<string, int[]> AttributeGroup)
        {
            SkuType skuType = db.SkuType.Find(updateData.ID);
            if (skuType == null) return HttpNotFound();

            skuType.NetoID = updateData.NetoID;
            skuType.AttributeGroup = Newtonsoft.Json.JsonConvert.SerializeObject(AttributeGroup ?? new Dictionary<string, int[]> { });
            skuType.UpdateAt = DateTime.UtcNow;
            skuType.UpdateBy = Session["AdminName"].ToString();

            if (skuType.SkuTypeLang.Any(l => l.LangID.Equals(langData.LangID)))
            {
                var attributeLang = skuType.SkuTypeLang.First(l => l.LangID.Equals(langData.LangID));
                attributeLang.Name = langData.Name;
                attributeLang.UpdateAt = skuType.UpdateAt.Value;
                attributeLang.UpdateBy = skuType.UpdateBy;
            }
            else
            {
                langData.CreateAt = skuType.UpdateAt.Value;
                langData.CreateBy = skuType.UpdateBy;
                skuType.SkuTypeLang.Add(langData);
            }

            if (PackageContent != null && PackageContent.Any())
            {
                foreach (var content in PackageContent)
                {
                    if (content.ItemID != 0)
                    {
                        var contentLang = skuType.PackageContent.First(c => c.ID.Equals(content.ItemID)).PackageContentLang.First(l => l.LangID.Equals(langData.LangID));
                        contentLang.Name = content.Name;
                        contentLang.UpdateAt = skuType.UpdateAt.Value;
                        contentLang.UpdateBy = skuType.UpdateBy;
                    }
                    else
                    {
                        var packageContent = new PackageContent()
                        {
                            IsEnable = true,
                            CreateAt = skuType.UpdateAt.Value,
                            CreateBy = skuType.UpdateBy
                        };

                        packageContent.PackageContentLang.Add(new PackageContentLang()
                        {
                            Name = content.Name,
                            LangID = langData.LangID,
                            CreateAt = skuType.UpdateAt.Value,
                            CreateBy = skuType.UpdateBy
                        });

                        skuType.PackageContent.Add(packageContent);
                    }
                }
            }

            db.SaveChanges();

            ViewBag.LangID = langData.LangID;
            ViewBag.LangList = EnumData.DataLangList().Select(l => new SelectListItem { Text = l.Value, Value = l.Key });
            ViewBag.AttributeTypeList = db.SkuAttributeType.Include(t => t.SkuAttribute.Select(a => a.SkuAttributeLang)).Where(t => t.IsEnable).OrderBy(t => t.Order).ToList();
            return View(skuType);
        }

        // GET: SkuTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SkuType skuType = db.SkuType.Find(id);
            if (skuType == null)
            {
                return HttpNotFound();
            }
            return View(skuType);
        }

        // POST: SkuTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SkuType skuType = db.SkuType.Find(id);
            db.SkuType.Remove(skuType);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult GetData(SkuTypeFilter filter, int page = 1, int rows = 100)
        {
            int total = 0;
            List<object> dataList = new List<object>();

            var LangID = !string.IsNullOrEmpty(filter.LangID) ? filter.LangID : EnumData.DataLangList().First().Key;
            var TypeFilter = db.SkuType.Include(a => a.SkuTypeLang).AsQueryable();
            if (filter.ID.HasValue) TypeFilter = TypeFilter.Where(a => a.ID.Equals(filter.ID.Value));
            if (!string.IsNullOrEmpty(filter.Name)) TypeFilter = TypeFilter.Where(a => a.SkuTypeLang.Any(l => l.LangID.Equals(LangID) && l.Name.ToLower().Contains(filter.Name.ToLower())));
            if (filter.NetoID.HasValue) TypeFilter = TypeFilter.Where(a => a.NetoID.Value.Equals(filter.NetoID.Value));

            if (TypeFilter.Any())
            {
                int length = rows;
                int start = (page - 1) * length;
                total = TypeFilter.Count();
                var results = TypeFilter.OrderByDescending(a => a.CreateAt).Skip(start).Take(length).ToList();

                dataList.AddRange(results.Select(a => new
                {
                    a.ID,
                    Name = a.SkuTypeLang.Any(l => l.LangID.Equals(LangID)) ? a.SkuTypeLang.First(l => l.LangID.Equals(LangID)).Name : "",
                    a.NetoID
                }).ToList());
            }

            return Json(new { total, rows = dataList }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Update(SkuType updateData, SkuTypeLang langData)
        {
            AjaxResult result = new AjaxResult();

            SkuType type = db.SkuType.Find(updateData.ID);
            SetUpdateData(type, updateData, new string[] { "NetoID" });
            db.Entry(type).State = EntityState.Modified;

            langData.LangID = EnumData.DataLangList().First().Key;
            SkuTypeLang typeLang = type.SkuTypeLang.First(l => l.LangID.Equals(langData.LangID));
            SetUpdateData(typeLang, langData, new string[] { "Name" });
            db.Entry(typeLang).State = EntityState.Modified;

            db.SaveChanges();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLangData(int ID, string LangID)
        {
            AjaxResult result = new AjaxResult();

            SkuType type = db.SkuType.Find(ID);
            SkuTypeLang langData = type.SkuTypeLang.FirstOrDefault(l => l.LangID.Equals(LangID));

            var DefaultLangID = EnumData.DataLangList().First().Key;
            result.data = new
            {
                langData?.Name,
                PackageContent = type.PackageContent.Where(c => c.IsEnable).Select(c => c.PackageContentLang.First(l => l.LangID.Equals(c.PackageContentLang.Any(ll => ll.LangID.Equals(LangID)) ? LangID : DefaultLangID))).ToDictionary(l => l.ItemID.ToString(), l => l.Name)
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult RemovePackageContent(int ID)
        {
            AjaxResult result = new AjaxResult();

            try
            {
                var content = db.PackageContent.Find(ID);
                var langList = content.PackageContentLang.ToList();

                db.PackageContentLang.RemoveRange(langList);
                db.PackageContent.Remove(content);
                db.SaveChanges();
            }
            catch(Exception e)
            {
                result.status = false;
                result.message = e.InnerException != null && !string.IsNullOrEmpty(e.InnerException.Message) ? e.InnerException.Message : e.Message;
            }

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
