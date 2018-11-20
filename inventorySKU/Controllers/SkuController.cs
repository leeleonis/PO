using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using inventorySKU.Models;

namespace inventorySKU.Controllers
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
            Sku sku = db.Sku.Find(id);
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

            if (db.Sku.AsNoTracking().Any(s => s.ID.Equals(sku.ID))) return RedirectToAction("Create");

            using (StockKeepingUnit SKU = new StockKeepingUnit())
            {
                sku.CreateAt = DateTime.UtcNow;
                sku.CreateBy = Session["AdminName"].ToString();
                langData.LangID = EnumData.DataLangList().Keys.First();
                sku = SKU.CreateSku(sku, langData);
            }

            if (sku.Type.Equals((byte)EnumData.SkuType.Variation))
            {
                List<Sku> variationList = db.Sku.Where(s => !s.ID.Equals(sku.ID) && string.IsNullOrEmpty(s.ParentSku) && s.Type.Equals((byte)EnumData.SkuType.Single) && s.Category.Equals(sku.Category) && s.Brand.Equals(sku.Brand) && s.Condition.Equals(sku.Condition)).ToList();

                foreach (Sku childSku in variationList)
                {
                    childSku.ParentSku = sku.ID;
                    db.Entry(childSku).State = EntityState.Modified;
                }

                db.SaveChanges();
            }

            return RedirectToAction("Edit", new { id = sku.ID });
        }

        // GET: Sku/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Sku sku = db.Sku.Find(id);
            if (sku == null) return HttpNotFound();

            var LangID = EnumData.DataLangList().First().Key;
            ViewBag.LangID = LangID;
            ViewBag.LangList = EnumData.DataLangList().Select(l => new SelectListItem { Text = l.Value, Value = l.Key });
            ViewBag.TypeList = Enum.GetValues(typeof(EnumData.SkuType)).Cast<EnumData.SkuType>().Select(t => new SelectListItem() { Text = t.ToString(), Value = ((int)t).ToString() }).ToList();
            ViewBag.Condition = db.Condition.Where(c => c.IsEnable).SelectMany(c => c.ConditionLang.Where(l => l.LangID.Equals(LangID))).Select(l => new SelectListItem() { Text = l.Name, Value = l.ConditionID.ToString() }).ToList();
            ViewBag.Category = db.SkuType.Where(s => s.IsEnable).SelectMany(s => s.SkuTypeLang.Where(l => l.LangID.Equals(LangID))).Select(l => new SelectListItem() { Text = l.Name, Value = l.TypeID.ToString() }).ToList();
            ViewBag.Brand = new SelectList(db.Brand.Where(b => b.IsEnable), "ID", "Name");
            ViewBag.CompanyList = db.Company.AsNoTracking().Where(c => c.IsEnable).ToList();

            return View(sku);
        }

        // POST: Sku/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Sku updateData, SkuLang langData, int[] DiverseAttribute, Sku_Attribute[] VariationValue, Sku_Attribute[] AttributeValue, KitSku[] KitSku, HttpPostedFileBase picture)
        {
            Sku sku = db.Sku.Find(updateData.ID);
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
                db.SkuLang.Add(langData);
            }

            db.SaveChanges();

            if (VariationValue != null && VariationValue.Any())
            {
                List<Sku_Attribute> attributeList;

                foreach (var variationList in VariationValue.GroupBy(a => a.Sku))
                {
                    if (VariationValue.Where(a => !a.Sku.Equals(variationList.Key)).GroupBy(a => a.Sku).All(a => !CompareValue(variationList.ToList(), a.ToList())))
                    {
                        if (!variationList.Key.Contains("newSku"))
                        {
                            attributeList = db.Sku_Attribute.Where(a => a.Sku.Equals(variationList.Key) && a.LangID.Equals(langData.LangID)).ToList();

                            foreach (var skuAttr in variationList)
                            {
                                if (attributeList.Any(a => a.AttrID.Equals(skuAttr.AttrID)))
                                {
                                    var updateAttr = attributeList.First(a => a.AttrID.Equals(skuAttr.AttrID));
                                    updateAttr.Value = skuAttr.Value;
                                    updateAttr.UpdateAt = sku.UpdateAt.Value;
                                    updateAttr.UpdateBy = sku.UpdateBy;
                                    db.Entry(updateAttr).State = EntityState.Modified;
                                }
                                else
                                {
                                    skuAttr.CreateAt = sku.UpdateAt.Value;
                                    skuAttr.CreateBy = sku.UpdateBy;
                                    db.Sku_Attribute.Add(skuAttr);
                                }
                            }
                        }
                        else
                        {
                            Sku newSku;
                            using (StockKeepingUnit SKU = new StockKeepingUnit())
                            {
                                newSku = new Sku()
                                {
                                    IsEnable = true,
                                    Type = (byte)EnumData.SkuType.Single,
                                    ParentSku = sku.ID,
                                    Condition = 1,
                                    Category = sku.Category,
                                    Brand = sku.Brand,
                                    Status = (byte)EnumData.SkuStatus.Inactive,
                                    CreateAt = sku.UpdateAt.Value,
                                    CreateBy = sku.UpdateBy
                                };

                                SkuLang newLang = new SkuLang()
                                {
                                    LangID = langData.LangID,
                                    Name = langData.Name,
                                    Models = langData.Models,
                                    CreateAt = sku.UpdateAt.Value,
                                    CreateBy = sku.UpdateBy
                                };

                                newSku = SKU.CreateSku(newSku, newLang);
                            }

                            db.Sku_Attribute.AddRange(sku.Sku_Attribute.Where(a => !a.IsDiverse).Select(a => new Sku_Attribute()
                            {
                                Sku = newSku.ID,
                                LangID = a.LangID,
                                AttrID = a.AttrID,
                                Html = a.Html,
                                eBay = a.eBay,
                                CreateAt = sku.UpdateAt.Value,
                                CreateBy = sku.UpdateBy
                            }));

                            db.Sku_Attribute.AddRange(variationList.Select(a => new Sku_Attribute()
                            {
                                Sku = newSku.ID,
                                LangID = a.LangID,
                                AttrID = a.AttrID,
                                Value = a.Value,
                                CreateAt = sku.UpdateAt.Value,
                                CreateBy = sku.UpdateBy
                            }));
                        }
                    }
                    else
                    {
                        throw new Exception(string.Format("SKU {0} have same attributes!", variationList.Key));
                    }
                }

                db.SaveChanges();
            }

            if (AttributeValue != null && AttributeValue.Any())
            {
                DiverseAttribute = DiverseAttribute ?? sku.Sku_Attribute.Where(a => a.IsDiverse).Select(a => a.AttrID).Distinct().ToArray();
                if (DiverseAttribute.Any())
                {
                    foreach (var skuAttr in AttributeValue.Where(a => DiverseAttribute.Contains(a.AttrID)))
                    {
                        skuAttr.IsDiverse = true;
                    }
                }

                List<Sku_Attribute> attributeData = AttributeValue.ToList();
                List<Sku_Attribute> attributeList = sku.Sku_Attribute.Where(a => a.LangID.Equals(langData.LangID)).ToList();
                if (sku.Type.Equals((byte)EnumData.SkuType.Variation))
                {
                    foreach (Sku childSku in db.Sku.Where(s => s.IsEnable && s.ParentSku.Equals(sku.ID)).ToList())
                    {
                        attributeData.AddRange(AttributeValue.Where(a => !a.IsDiverse).Select(a => new Sku_Attribute()
                        {
                            Sku = childSku.ID,
                            LangID = a.LangID,
                            AttrID = a.AttrID,
                            Value = a.Value,
                            Html = a.Html,
                            eBay = a.eBay
                        }));

                        attributeList.AddRange(childSku.Sku_Attribute.Where(a => a.LangID.Equals(langData.LangID) && !DiverseAttribute.Contains(a.AttrID)).ToList());
                    }
                }

                List<Sku_Attribute> newAttributeList = attributeData.Except(attributeList).ToList();
                foreach (var newAttr in newAttributeList)
                {
                    newAttr.CreateAt = sku.UpdateAt.Value;
                    newAttr.CreateBy = sku.UpdateBy;
                }
                db.Sku_Attribute.AddRange(newAttributeList);

                foreach (var updateAttr in attributeList)
                {
                    var update = attributeData.First(a => a.Sku.Equals(updateAttr.Sku) && a.AttrID.Equals(updateAttr.AttrID));
                    updateAttr.IsDiverse = update.IsDiverse;
                    updateAttr.Value = update.Value;
                    updateAttr.Html = update.Html;
                    updateAttr.eBay = update.eBay;
                    updateAttr.UpdateAt = sku.UpdateAt.Value;
                    updateAttr.UpdateBy = sku.UpdateBy;
                    db.Entry(updateAttr).State = EntityState.Modified;
                }

                db.SaveChanges();
            }

            if (KitSku != null && KitSku.Any())
            {
                foreach (var kit in KitSku)
                {
                    if (sku.GetKit.Any(k => k.Sku.Equals(kit.Sku)))
                    {
                        var updateKit = sku.GetKit.First(k => k.Sku.Equals(kit.Sku));
                        updateKit.Qty = kit.Qty;
                        updateKit.UpdateAt = sku.UpdateAt.Value;
                        updateKit.UpdateBy = sku.UpdateBy;
                        db.Entry(updateKit).State = EntityState.Modified;
                    }
                    else
                    {
                        kit.CreateAt = sku.UpdateAt.Value;
                        kit.CreateBy = sku.UpdateBy;
                        db.KitSku.Add(kit);
                    }
                }
                db.SaveChanges();
            }

            if (picture != null && picture.ContentLength > 0)
            {
                var mainPicture = db.SkuPicture.FirstOrDefault(p => p.IsMain && p.Sku.Equals(sku.ID));
                if (mainPicture != null)
                {
                    System.IO.File.Delete(string.Format(Server.MapPath("~/Uploads/Sku/{0}/{1}"), mainPicture.Sku, mainPicture.FileName));
                    db.SkuPicture.Remove(mainPicture);
                }

                UploadPicture(sku.ID, new HttpPostedFileBase[] { picture }, true);
            }

            var LangID = EnumData.DataLangList().First().Key;
            ViewBag.LangID = langData.LangID;
            ViewBag.LangList = EnumData.DataLangList().Select(l => new SelectListItem { Text = l.Value, Value = l.Key, Selected = l.Key.Equals(langData.LangID) });
            ViewBag.TypeList = Enum.GetValues(typeof(EnumData.SkuType)).Cast<EnumData.SkuType>().Select(t => new SelectListItem() { Text = t.ToString(), Value = ((int)t).ToString() }).ToList();
            ViewBag.Condition = db.Condition.Where(c => c.IsEnable).SelectMany(c => c.ConditionLang.Where(l => l.LangID.Equals(LangID))).Select(l => new SelectListItem() { Text = l.Name, Value = l.ConditionID.ToString() });
            ViewBag.Category = db.SkuType.Where(s => s.IsEnable).SelectMany(s => s.SkuTypeLang.Where(l => l.LangID.Equals(LangID))).Select(l => new SelectListItem() { Text = l.Name, Value = l.TypeID.ToString() });
            ViewBag.Brand = new SelectList(db.Brand.Where(b => b.IsEnable), "ID", "Name");
            ViewBag.AttributeTypeList = db.SkuAttributeType.Where(t => t.IsEnable).OrderBy(t => t.Order).ToList();
            ViewBag.CompanyList = db.Company.AsNoTracking().Where(c => c.IsEnable).ToList();

            return View(sku);
        }

        private bool CompareValue(List<Sku_Attribute> list, List<Sku_Attribute> target)
        {
            try
            {
                var filter = target.AsQueryable();
                foreach (var attr in list)
                {
                    filter = filter.Where(a => a.AttrID.Equals(attr.AttrID) && a.Value.Equals(attr.Value));
                }
                return filter.Any();
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public ActionResult SkuPicture(string sku)
        {
            AjaxResult result = new AjaxResult();

            var pictureList = db.SkuPicture.Where(p => !p.IsMain && p.Sku.Equals(sku)).OrderBy(p => p.Order).ToList();
            result.data = pictureList.Select(p => new { p.ID, name = p.FileName, size = p.FileSize, p.Order }).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdatePicture(int ID, int Order)
        {
            AjaxResult result = new AjaxResult();

            try
            {
                var picture = db.SkuPicture.Find(ID);

                if (picture == null) throw new Exception("Not find picture!");

                picture.Order = Order;
                db.Entry(picture).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception e)
            {
                result.status = false;
                result.message = e.InnerException != null ? e.InnerException.Message ?? e.Message : e.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UploadPicture(string ID, HttpPostedFileBase[] files, bool isMain = false)
        {
            AjaxResult result = new AjaxResult();

            List<SkuPicture> pictureList = new List<SkuPicture>();

            // Verify that the user selected a file
            if (files != null && files.Any(f => f.ContentLength > 0))
            {
                int Order = db.Sku.Find(ID).SkuPicture.Max(p => p.Order) + 1;

                foreach (var file in files.Where(f => f.ContentLength > 0))
                {
                    try
                    {
                        var fileExtension = Path.GetExtension(file.FileName);
                        var fileName = Guid.NewGuid().ToString() + fileExtension;
                        var filePath = Server.MapPath("~/Uploads/Sku/" + ID);
                        if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);

                        var path = Path.Combine(filePath, fileName);
                        file.SaveAs(path);

                        var picture = new SkuPicture
                        {
                            IsMain = isMain,
                            Sku = ID,
                            FileName = fileName,
                            FileSize = file.ContentLength,
                            Order = Order++,
                            CreateAt = DateTime.UtcNow,
                            CreateBy = Session["AdminName"].ToString()
                        };

                        db.Entry(picture).State = EntityState.Added;
                        pictureList.Add(picture);
                    }
                    catch (Exception e)
                    {
                        result.message += e.InnerException != null ? e.InnerException.Message ?? e.Message : e.Message;
                    }
                }

                db.SaveChanges();
                result.data = pictureList.Select(p => new { p.ID, name = p.FileName, size = p.FileSize, p.Order }).ToList();
            }
            // redirect back to the index action to show the form once again

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult RemovePicture(int ID)
        {
            AjaxResult result = new AjaxResult();

            try
            {
                var picture = db.SkuPicture.Find(ID);

                if (picture == null) throw new Exception("Not find picture!");

                System.IO.File.Delete(string.Format(Server.MapPath("~/Uploads/Sku/{0}/{1}"), picture.Sku, picture.FileName));

                db.SkuPicture.Remove(picture);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                result.status = false;
                result.message = e.InnerException != null ? e.InnerException.Message ?? e.Message : e.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // GET: Sku/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sku sku = db.Sku.Find(id);
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
            Sku sku = db.Sku.Find(id);
            db.Sku.Remove(sku);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult GetData(SkuFilter filter, int page = 1, int rows = 100)
        {
            int total = 0;
            List<object> dataList = new List<object>();

            var LangID = !string.IsNullOrEmpty(filter.LangID) ? filter.LangID : EnumData.DataLangList().First().Key;
            var SkuFilter = db.Sku.Include(s => s.SkuLang).AsNoTracking().AsQueryable();
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

        public ActionResult GetParent(SkuFilter data, string filter)
        {
            int total = 0;
            List<object> dataList = new List<object>();

            string LangID = EnumData.DataLangList().First().Key;
            var SkuFilter = db.Sku.Include(s => s.SkuLang).AsNoTracking().AsQueryable();
            if (data.Type.HasValue) SkuFilter = SkuFilter.Where(s => s.Type.Equals(data.Type.Value));
            if (data.Condition.HasValue) SkuFilter = SkuFilter.Where(s => s.Condition.Equals(data.Condition.Value));
            if (data.Category.HasValue) SkuFilter = SkuFilter.Where(s => s.Category.Equals(data.Category.Value));
            if (data.Brand.HasValue) SkuFilter = SkuFilter.Where(s => s.Brand.Equals(data.Brand.Value));
            if (!string.IsNullOrEmpty(filter)) SkuFilter = SkuFilter.Where(s => s.ID.Contains(filter) || s.SkuLang.Any(l => l.Name.ToLower().Contains(filter.ToLower())));

            if (SkuFilter.Any())
            {
                total = SkuFilter.Count();
                var results = SkuFilter.OrderByDescending(s => s.ID).ToList();

                dataList.AddRange(results.Select(s => new
                {
                    s.ID,
                    s.SkuLang.FirstOrDefault(l => l.LangID.Equals(LangID))?.Name
                }).ToList());
            }

            return Json(new { total, rows = dataList }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FilterSku(string filter, string parentSku)
        {
            var langID = EnumData.DataLangList().First().Key;

            var skuFilter = db.Sku.Include(s => s.SkuLang).Where(s => s.IsEnable);
            if (!string.IsNullOrEmpty(parentSku)) skuFilter = skuFilter.Where(s => s.ParentSku.Equals(parentSku));
            var skuList = skuFilter.Where(s => s.ID.Contains(filter) || s.SkuLang.Any(l => l.Name.Contains(filter))).ToList();

            var data = skuList.Select(s => new { name = s.SkuLang.First(l => l.LangID.Equals(langID)).Name, value = s.ID }).ToArray();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Update(Sku updateData, SkuLang langData)
        {
            AjaxResult result = new AjaxResult();

            Sku sku = db.Sku.Find(updateData.ID);
            SetUpdateData(sku, updateData, EditList);
            db.Entry(sku).State = EntityState.Modified;

            langData.LangID = EnumData.DataLangList().First().Key;
            SkuLang skuLang = sku.SkuLang.First(l => l.LangID.Equals(langData.LangID));
            SetUpdateData(skuLang, langData, new string[] { "Name" });
            db.Entry(skuLang).State = EntityState.Modified;

            db.SaveChanges();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLangData(string ID, string LangID)
        {
            AjaxResult result = new AjaxResult();

            Sku sku = db.Sku.Find(ID);
            SkuLang langData = sku.SkuLang.FirstOrDefault(l => l.LangID.Equals(LangID));

            ViewDataDictionary viewData = new ViewDataDictionary() { { "LangID", LangID } };
            viewData.Add("AttributeTypeList", db.SkuAttributeType.Where(t => t.IsEnable).OrderBy(t => t.Order).ToList());
            ViewBag.AttributeHtml = RenderViewToString(ControllerContext, "_SingleAttribute", sku, viewData);

            result.data = new
            {
                langData?.Name,
                langData?.Models,
                VariationList = sku.Type.Equals((byte)EnumData.SkuType.Variation) ? RenderViewToString(ControllerContext, "_VariationAttribute", sku, viewData) : "",
                KitList = sku.Type.Equals((byte)EnumData.SkuType.Kit) ? RenderViewToString(ControllerContext, "_SkuKit", sku, viewData) : "",
                AttributeList = RenderViewToString(ControllerContext, "_SingleAttribute", sku, viewData)
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult RemoveVariation(string ID)
        {
            AjaxResult result = new AjaxResult();

            Sku sku = db.Sku.Find(ID);

            try
            {
                if (sku == null) throw new Exception("Not found sku!");

                sku.ParentSku = null;
                db.Entry(sku).State = EntityState.Modified;

                db.SaveChanges();
            }
            catch (Exception e)
            {
                result.status = false;
                result.message = e.InnerException != null ? e.InnerException.Message ?? e.Message : e.Message;
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
