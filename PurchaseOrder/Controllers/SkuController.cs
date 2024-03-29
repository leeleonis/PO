﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using inventorySKU;
using PurchaseOrderSys.Models;

namespace PurchaseOrderSys.Controllers
{
    [CheckSession]
    public class SkuController : BaseController
    {
        readonly string[] EditList = new string[] { "ParentSku", "Condition", "Category", "Brand", "EAN", "UPC", "Replenishable", "SerialTracking", "Battery", "Status", "DisplayPageUrl" };
        readonly string[] LangList = new string[] { "Name", "Model", "Description", "PackageContent", "SpecContent", "FeatureContent" };
        readonly string[] LogisticList = new string[] { "BoxID", "Price", "OriginCountry", "CaseWidth", "CaseLength", "CaseHeight", "CaseWeight", "ShippingWidth", "ShippingLength", "ShippingHeight", "ShippingWeight" };

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
            SKU sku = db.SKU.Find(id);
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
            ViewBag.Company = db.Company.Where(c => c.IsEnable && !c.ParentID.HasValue).Select(c => new SelectListItem() { Text = c.Name, Value = c.ID.ToString() }).ToList();
            ViewBag.TypeList = Enum.GetValues(typeof(EnumData.SkuType)).Cast<EnumData.SkuType>().Select(t => new SelectListItem() { Text = t.ToString(), Value = ((int)t).ToString() }).ToList();
            ViewBag.Condition = db.Condition.AsNoTracking().Where(c => c.IsEnable).SelectMany(c => c.ConditionLang.Where(l => l.LangID.Equals(LangID))).OrderBy(l => l.Name).Select(l => new SelectListItem() { Text = l.Name, Value = l.ConditionID.ToString() });
            ViewBag.Category = db.SkuType.Where(s => s.IsEnable).SelectMany(s => s.SkuTypeLang.Where(l => l.LangID.Equals(LangID))).OrderBy(l => l.Name).Select(l => new SelectListItem() { Text = l.Name, Value = l.TypeID.ToString() });
            ViewBag.Brand = new SelectList(db.Brand.Where(b => b.IsEnable).OrderBy(b => b.Name), "ID", "Name");
            return View();
        }

        // POST: Sku/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SKU sku, SkuLang langData, string Copy)
        {
            if (db.SKU.AsNoTracking().Any(s => s.SkuID.Equals(sku.SkuID)))
            {
                TempData["ErrorMsg"] = string.Format("SKU【{0}】已存在", sku.SkuID);

                return RedirectToAction("Create");
            }

            SKU copySku = null;
            using (StockKeepingUnit SKU = new StockKeepingUnit())
            {
                sku.CreateAt = DateTime.UtcNow;
                sku.CreateBy = Session["AdminName"].ToString();

                if (!string.IsNullOrEmpty(Copy))
                {
                    copySku = db.SKU.Find(Copy);

                    if (copySku == null)
                    {
                        TempData["ErrorMsg"] = string.Format("繼承 SKU【{0}】找不到", sku.SkuID);

                        return RedirectToAction("Create");
                    }

                    sku.Type = copySku.Type;
                    sku.Condition = copySku.Condition;
                    sku.ParentSku = copySku.ParentSku;
                    sku.ParentShadow = copySku.ParentShadow;
                    SKU.SkuInherit(sku, copySku);
                }

                langData.LangID = EnumData.DataLangList().Keys.First();
                sku = SKU.CreateSku(sku, langData);

                try
                {
                    if (copySku != null)
                    {
                        copySku.UpdateAt = sku.CreateAt;
                        copySku.UpdateBy = sku.UpdateBy;
                        SKU.SkuInherit(sku, copySku, copySku.SkuLang.First(l => l.LangID.Equals(langData.LangID)));

                        if (copySku.Logistic != null)
                        {
                            copySku.Logistic.UpdateAt = sku.CreateAt;
                            copySku.Logistic.UpdateBy = sku.CreateBy;
                            SKU.LogisticInherit(sku.Logistic, copySku.Logistic);
                        }

                        foreach (var attr in copySku.Sku_Attribute)
                        {
                            db.Sku_Attribute.Add(new Sku_Attribute()
                            {
                                IsDiverse = attr.IsDiverse,
                                Sku = sku.SkuID,
                                AttrID = attr.AttrID,
                                LangID = attr.LangID,
                                Value = attr.Value,
                                eBay = attr.eBay,
                                Html = attr.Html,
                                CreateAt = sku.CreateAt,
                                CreateBy = sku.CreateBy
                            });
                        }

                        foreach (var packageContent in copySku.Sku_PackageContent)
                        {
                            db.Sku_PackageContent.Add(new Sku_PackageContent()
                            {
                                Sku = sku.SkuID,
                                ItemID = packageContent.ItemID,
                                LangID = packageContent.LangID,
                                Model = packageContent.Model,
                                Html = packageContent.Html,
                                CreateAt = sku.CreateAt,
                                CreateBy = sku.CreateBy
                            });
                        }

                        db.SaveChanges();

                        SKU.UpdateSku_Suffix(sku.SkuLang.First(l => l.LangID.Equals(langData.LangID)));
                    }

                    JobProcess job = new JobProcess(string.Format("新增品號【{0}】資料至NETO、SC", sku.SkuID));
                    try
                    {
                        job.StatusLog(EnumData.TaskStatus.執行中);

                        job.AddLog("開始新增品號資料");

                        SKU.SC_Api = new SellerCloud_WebService.SC_WebService(ApiUserName, ApiPassword);
                        SKU.CreateSkuToNeto();
                        SKU.CreateSkuToSC();

                        job.AddLog("完成品號資料新增");

                        job.StatusLog(EnumData.TaskStatus.執行完);
                    }
                    catch (Exception ex)
                    {
                        job.Fail(ex.InnerException?.Message ?? ex.Message);
                    }

                }
                catch (Exception e)
                {
                    TempData["ErrorMsg"] = e.InnerException?.Message ?? e.Message;
                }
            }

            if (sku.Type.Equals((byte)EnumData.SkuType.Variation))
            {
                List<SKU> variationList = db.SKU.Where(s => !s.SkuID.Equals(sku.SkuID) && string.IsNullOrEmpty(s.ParentSku) && s.Type.Equals((byte)EnumData.SkuType.Single) && s.Category.Equals(sku.Category) && s.Brand.Equals(sku.Brand) && s.Condition.Equals(sku.Condition)).ToList();

                foreach (SKU childSku in variationList)
                {
                    childSku.ParentSku = sku.SkuID;
                    childSku.UpdateAt = sku.CreateAt;
                    childSku.UpdateBy = sku.CreateBy;
                }

                db.SaveChanges();
            }

            return RedirectToAction("Edit", new { id = sku.SkuID });
        }

        // GET: Sku/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            SKU sku = db.SKU.Find(id);
            if (sku == null) return HttpNotFound();

            var companyList = db.Company.AsNoTracking().Where(c => c.IsEnable).ToList();

            var LangID = EnumData.DataLangList().First().Key;
            ViewBag.LangID = LangID;
            ViewBag.LangList = EnumData.DataLangList().Select(l => new SelectListItem { Text = l.Value, Value = l.Key });
            ViewBag.Company = companyList.Where(c => !c.ParentID.HasValue).Select(c => new SelectListItem() { Text = c.Name, Value = c.ID.ToString() }).ToList();
            ViewBag.TypeList = Enum.GetValues(typeof(EnumData.SkuType)).Cast<EnumData.SkuType>().Select(t => new SelectListItem() { Text = t.ToString(), Value = ((int)t).ToString() }).ToList();
            ViewBag.Condition = db.Condition.Where(c => c.IsEnable).SelectMany(c => c.ConditionLang.Where(l => l.LangID.Equals(LangID))).OrderBy(l => l.Name).Select(l => new SelectListItem() { Text = l.Name, Value = l.ConditionID.ToString() }).ToList();
            ViewBag.Category = db.SkuType.Where(s => s.IsEnable).SelectMany(s => s.SkuTypeLang.Where(l => l.LangID.Equals(LangID))).OrderBy(l => l.Name).Select(l => new SelectListItem() { Text = l.Name, Value = l.TypeID.ToString() }).ToList();
            ViewBag.Brand = new SelectList(db.Brand.Where(b => b.IsEnable).OrderBy(b => b.Name), "ID", "Name");
            ViewBag.CompanyList = companyList;
            ViewBag.MarketList = db.Marketplace.Where(m => m.IsEnable).ToList();
            ViewBag.BoxTypeList = db.BoxType.Select(b => new SelectListItem() { Text = b.Name, Value = b.ID.ToString() }).ToList();

            var warehouseList = db.Warehouse.Where(w => w.IsEnable).ToList();
            ViewBag.WarehouseList = warehouseList;
            ViewBag.PurchaseSku = db.PurchaseSKU.Where(s => s.IsEnable && s.SkuNo.Equals(id) && s.PurchaseOrder.IsEnable).ToList();
            ViewBag.TransferSku = db.TransferSKU.Where(s => s.IsEnable && s.SkuNo.Equals(id)).ToList();
            ViewBag.RMASku = db.RMASKU.Where(s => s.IsEnable && s.SkuNo.Equals(id) && s.RMA.IsEnable).ToList();
            ViewBag.AwaitingList = GetAwaitingCount(new string[] { id }, warehouseList.SelectMany(w => w.WarehouseSummary.Where(ws => ws.IsEnable && ws.Type.Equals("SCID"))).Select(w => w.Val).ToArray());

            return View(sku);
        }

        // POST: Sku/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SKU updateData, SkuLang langData, List<Dictionary<string, string>> eBayTitle, int[] DiverseAttribute, Sku_Attribute[] VariationValue, KitSku[] KitSku, Sku_PackageContent[] SkuContent, string[] KeyFeature, Sku_Attribute[] AttributeValue, HttpPostedFileBase picture, PriceGroup[] PriceGroup, Logistic Logistic, HttpPostedFileBase[] LogisticImg = null, bool Sync = false)
        {
            SKU sku = db.SKU.Find(updateData.SkuID);
            if (sku == null) return HttpNotFound();

            sku.eBayTitle = JsonConvert.SerializeObject(eBayTitle.ToDictionary(t => int.Parse(t["misc"]), t => t["title"]));
            SetUpdateData(sku, updateData, EditList);

            if (sku.SkuLang.Any(l => l.LangID.Equals(langData.LangID)))
            {
                SkuLang skuLang = sku.SkuLang.First(l => l.LangID.Equals(langData.LangID));
                SetUpdateData(skuLang, langData, LangList);
                skuLang.KeyFeature = JsonConvert.SerializeObject(KeyFeature);
            }
            else
            {
                langData.Sku = sku.SkuID;
                langData.KeyFeature = JsonConvert.SerializeObject(KeyFeature);
                langData.CreateAt = sku.UpdateAt.Value;
                langData.CreateBy = sku.UpdateBy;
                db.SkuLang.Add(langData);
            }

            if (sku.Logistic != null)
            {
                var logisticData = sku.Logistic;
                SetUpdateData(logisticData, Logistic, LogisticList);
            }
            else
            {
                Logistic.Sku = sku.SkuID;
                Logistic.CreateAt = sku.UpdateAt.Value;
                Logistic.CreateBy = sku.UpdateBy;
                db.Logistic.Add(Logistic);
            }

            db.SaveChanges();

            if (VariationValue != null && VariationValue.Any())
            {
                List<Sku_Attribute> attributeList;
                ViewDataDictionary viewData = new ViewDataDictionary() { { "LangID", langData.LangID } };
                viewData.Add("AttributeTypeList", db.SkuAttributeType.Where(t => t.IsEnable).OrderBy(t => t.Order).ToList());

                foreach (var variationList in VariationValue.GroupBy(a => a.Sku))
                {
                    if (VariationValue.Where(a => !a.Sku.Equals(variationList.Key)).GroupBy(a => a.Sku).All(a => !CompareValue(variationList.ToList(), a.ToList())))
                    {
                        SkuLang variationSkuLang;
                        if (!variationList.Key.Contains("newSku"))
                        {
                            variationSkuLang = db.SKU.Find(variationList.Key).SkuLang.First(l => l.LangID.Equals(langData.LangID));
                            variationSkuLang.Description = langData.Description;
                            variationSkuLang.PackageContent = langData.PackageContent;

                            attributeList = db.Sku_Attribute.Where(a => a.Sku.Equals(variationList.Key) && a.LangID.Equals(langData.LangID)).ToList();

                            foreach (var skuAttr in variationList)
                            {
                                if (attributeList.Any(a => a.AttrID.Equals(skuAttr.AttrID)))
                                {
                                    var updateAttr = attributeList.First(a => a.AttrID.Equals(skuAttr.AttrID));
                                    updateAttr.Value = skuAttr.Value;
                                    updateAttr.UpdateAt = sku.UpdateAt.Value;
                                    updateAttr.UpdateBy = sku.UpdateBy;
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
                            SKU newSku;
                            using (StockKeepingUnit SKU = new StockKeepingUnit())
                            {
                                newSku = new SKU()
                                {
                                    IsEnable = true,
                                    Type = (byte)EnumData.SkuType.Single,
                                    Condition = sku.Condition,
                                    ParentSku = sku.SkuID,
                                    CreateAt = sku.UpdateAt.Value,
                                    CreateBy = sku.CreateBy
                                };
                                SKU.SkuInherit(newSku, sku);

                                SkuLang newLang = new SkuLang()
                                {
                                    LangID = langData.LangID,
                                    Name = langData.Name + " " + string.Join(" ", variationList.Select(a => a.Value).ToArray()),
                                    Model = langData.Model,
                                    Description = langData.Description,
                                    PackageContent = langData.PackageContent,
                                    FeatureContent = langData.FeatureContent,
                                    SpecContent = langData.SpecContent,
                                    KeyFeature = langData.KeyFeature
                                };

                                newSku = SKU.CreateSku(newSku, newLang);

                                JobProcess job = new JobProcess(string.Format("新增品號【{0}】資料至NETO、SC", newSku.SkuID));
                                try
                                {
                                    job.StatusLog(EnumData.TaskStatus.執行中);

                                    job.AddLog("開始新增品號資料");

                                    SKU.SC_Api = new SellerCloud_WebService.SC_WebService(ApiUserName, ApiPassword);
                                    SKU.CreateSkuToNeto();
                                    SKU.CreateSkuToSC();

                                    job.AddLog("完成品號資料新增");

                                    job.StatusLog(EnumData.TaskStatus.執行完);
                                }
                                catch (Exception ex)
                                {
                                    job.Fail(ex.InnerException?.Message ?? ex.Message);
                                }

                                variationSkuLang = newLang;
                            }

                            db.Sku_Attribute.AddRange(sku.Sku_Attribute.Where(a => !a.IsDiverse).Select(a => new Sku_Attribute()
                            {
                                Sku = newSku.SkuID,
                                LangID = a.LangID,
                                AttrID = a.AttrID,
                                Html = a.Html,
                                eBay = a.eBay,
                                CreateAt = sku.UpdateAt.Value,
                                CreateBy = sku.UpdateBy
                            }));

                            db.Sku_Attribute.AddRange(variationList.Select(a => new Sku_Attribute()
                            {
                                Sku = newSku.SkuID,
                                LangID = a.LangID,
                                AttrID = a.AttrID,
                                Value = a.Value,
                                CreateAt = sku.UpdateAt.Value,
                                CreateBy = sku.UpdateBy
                            }));
                        }

                        db.SaveChanges();

                        variationSkuLang.SpecContent = RenderViewToString(ControllerContext, "_SpecContent", variationSkuLang.GetSku, viewData);
                    }
                    else
                    {
                        throw new Exception(string.Format("SKU {0} have same attributes!", variationList.Key));
                    }
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

            if (SkuContent != null && SkuContent.Any())
            {
                foreach (var content in SkuContent)
                {
                    var contentModel = sku.Sku_PackageContent.FirstOrDefault(c => c.ItemID.Equals(content.ItemID) && c.LangID.Equals(content.LangID));
                    if (contentModel != null)
                    {
                        contentModel.Model = content.Model;
                        contentModel.Html = content.Html;
                        content.UpdateAt = sku.UpdateAt;
                        content.UpdateBy = sku.UpdateBy;
                    }
                    else
                    {
                        content.CreateAt = sku.UpdateAt.Value;
                        content.CreateBy = sku.UpdateBy;
                        sku.Sku_PackageContent.Add(content);
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
                    foreach (SKU childSku in db.SKU.Where(s => s.IsEnable && s.ParentSku.Equals(sku.SkuID)).ToList())
                    {
                        attributeData.AddRange(attributeData.Where(a => !a.IsDiverse).Select(a => new Sku_Attribute()
                        {
                            Sku = childSku.SkuID,
                            LangID = a.LangID,
                            AttrID = a.AttrID,
                            Value = a.Value,
                            Html = a.Html,
                            eBay = a.eBay
                        }));

                        attributeList.AddRange(childSku.Sku_Attribute.Where(a => a.LangID.Equals(langData.LangID) && !DiverseAttribute.Contains(a.AttrID)).ToList());
                    }
                }

                // 新增Spec
                List<Sku_Attribute> newAttributeList = attributeData.Except(attributeList).ToList();
                foreach (var newAttr in newAttributeList)
                {
                    newAttr.CreateAt = sku.UpdateAt.Value;
                    newAttr.CreateBy = sku.UpdateBy;
                }
                db.Sku_Attribute.AddRange(newAttributeList);

                // 更新Spec
                foreach (var updateAttr in attributeList)
                {
                    var update = attributeData.FirstOrDefault(a => a.Sku.Equals(updateAttr.Sku) && a.AttrID.Equals(updateAttr.AttrID));
                    if (update != null)
                    {
                        updateAttr.IsDiverse = update.IsDiverse;
                        updateAttr.Value = update.Value;
                        updateAttr.Html = update.Html;
                        updateAttr.eBay = update.eBay;
                        updateAttr.UpdateAt = sku.UpdateAt.Value;
                        updateAttr.UpdateBy = sku.UpdateBy;
                    }
                }

                // 刪除Spec
                db.Sku_Attribute.RemoveRange(attributeList.Except(attributeData).ToList());

                db.SaveChanges();
            }

            if (picture != null && picture.ContentLength > 0)
            {
                var mainPicture = db.SkuPicture.FirstOrDefault(p => p.IsMain && p.Sku.Equals(sku.SkuID));
                if (mainPicture != null)
                {
                    System.IO.File.Delete(string.Format(Server.MapPath("~/Uploads/{0}"), mainPicture.FileName));
                    db.SkuPicture.Remove(mainPicture);
                }

                UploadPicture(sku.SkuID, new HttpPostedFileBase[] { picture }, true);
            }

            if (PriceGroup != null && PriceGroup.Any())
            {
                foreach (var price in PriceGroup)
                {
                    if (!price.ID.Equals(0))
                    {
                        var updatePrice = sku.PriceGroup.First(p => p.ID.Equals(price.ID));
                        updatePrice.IsUsed = price.IsUsed;
                        updatePrice.ItemID = price.ItemID;
                        if (!price.Price.Equals(0)) updatePrice.Price = price.Price;
                        updatePrice.Max = price.Max;
                        updatePrice.Min = price.Min;
                        updatePrice.UpdateAt = sku.UpdateAt;
                        updatePrice.UpdateBy = sku.UpdateBy;
                    }
                    else
                    {
                        price.CreateAt = sku.UpdateAt.Value;
                        price.CreateBy = sku.UpdateBy;
                        sku.PriceGroup.Add(price);
                    }
                }
                db.SaveChanges();
            }

            if (LogisticImg != null)
            {
                foreach (var image in LogisticImg.Where(img => img != null && img.ContentLength > 0).Take(3))
                {
                    var fileExtension = Path.GetExtension(image.FileName);
                    var fileName = Guid.NewGuid().ToString() + fileExtension;
                    var filePath = Server.MapPath("~/Uploads/Sku/" + sku.SkuID);
                    if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);

                    var path = Path.Combine(filePath, fileName);
                    image.SaveAs(path);

                    sku.SkuPicture.Add(new SkuPicture()
                    {
                        IsMain = false,
                        Sku = sku.SkuID,
                        PictureType = "Logistic",
                        FileName = string.Format("Sku/{0}/{1}", sku.SkuID, fileName),
                        FileSize = image.ContentLength,
                        CreateAt = DateTime.UtcNow,
                        CreateBy = Session["AdminName"].ToString()
                    });
                }

                db.SaveChanges();

                foreach (var image in sku.SkuPicture.OrderByDescending(p => p.ID).Skip(3))
                {
                    if (System.IO.File.Exists(Server.MapPath("~/Uploads/" + string.Format("Sku/{0}/{1}", sku.SkuID, image.FileName))))
                    {
                        System.IO.File.Delete(string.Format(Server.MapPath("~/Uploads/" + string.Format("Sku/{0}/{1}", sku.SkuID, image.FileName))));
                    }
                    db.SkuPicture.Remove(image);
                }

                db.SaveChanges();
            }

            try
            {
                if (sku.Type.Equals((byte)EnumData.SkuType.Single) && sku.Condition.Equals(1))
                {
                    using (StockKeepingUnit SKU = new StockKeepingUnit(sku))
                    {
                        SKU.UpdateSku_Suffix(langData);
                    }
                }
                else if (sku.Type.Equals((byte)EnumData.SkuType.Variation))
                {
                    using (StockKeepingUnit SKU = new StockKeepingUnit())
                    {
                        foreach (var childSku in db.SKU.Where(s => s.IsEnable && s.ParentSku.Equals(sku.SkuID)))
                        {
                            SKU.SetSkuData(childSku.SkuID);
                            SKU.UpdateSku_Suffix();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                TempData["ErrorMsg"] = e.InnerException?.Message ?? e.Message;
            }

            if (Sync)
            {
                using (StockKeepingUnit SKU = new StockKeepingUnit(sku))
                {
                    JobProcess job = new JobProcess(string.Format("更新品號【{0}】資料至NETO、SC", sku.SkuID));
                    try
                    {
                        job.StatusLog(EnumData.TaskStatus.執行中);

                        job.AddLog("開始更新品號資料");

                        SKU.SC_Api = new SellerCloud_WebService.SC_WebService(ApiUserName, ApiPassword);
                        SKU.UpdateSkuToNeto();
                        SKU.UpdateSkuToSC();
                        job.AddLog("完成品號資料更新");

                        if (sku.Type.Equals((byte)EnumData.SkuType.Variation))
                        {
                            SKU.UpdateVariationToNeto();
                            job.AddLog("完成品號Variaction更新");
                        }

                        if (sku.Type.Equals((byte)EnumData.SkuType.Single) && sku.Condition.Equals(1))
                        {
                            foreach (var condition in db.Condition.Where(c => c.IsEnable && !c.ID.Equals(sku.Condition)).ToList())
                            {
                                SKU.SetSkuData(sku.SkuID + condition.Suffix);
                                SKU.UpdateSkuToNeto();
                                SKU.UpdateSkuToSC();
                            }
                            job.AddLog("完成品號U品更新");
                        }

                        job.StatusLog(EnumData.TaskStatus.執行完);
                    }
                    catch (Exception ex)
                    {
                        job.Fail(ex.InnerException?.Message ?? ex.Message);
                    }
                }
            }

            var LangID = EnumData.DataLangList().First().Key;
            ViewBag.LangID = langData.LangID;
            ViewBag.LangList = EnumData.DataLangList().Select(l => new SelectListItem { Text = l.Value, Value = l.Key, Selected = l.Key.Equals(langData.LangID) });
            ViewBag.Company = db.Company.Where(c => c.IsEnable && !c.ParentID.HasValue).Select(c => new SelectListItem() { Text = c.Name, Value = c.ID.ToString() }).ToList();
            ViewBag.TypeList = Enum.GetValues(typeof(EnumData.SkuType)).Cast<EnumData.SkuType>().Select(t => new SelectListItem() { Text = t.ToString(), Value = ((int)t).ToString() }).ToList();
            ViewBag.Condition = db.Condition.Where(c => c.IsEnable).SelectMany(c => c.ConditionLang.Where(l => l.LangID.Equals(LangID))).OrderBy(l => l.Name).Select(l => new SelectListItem() { Text = l.Name, Value = l.ConditionID.ToString() });
            ViewBag.Category = db.SkuType.Where(s => s.IsEnable).SelectMany(s => s.SkuTypeLang.Where(l => l.LangID.Equals(LangID))).OrderBy(l => l.Name).Select(l => new SelectListItem() { Text = l.Name, Value = l.TypeID.ToString() });
            ViewBag.Brand = new SelectList(db.Brand.Where(b => b.IsEnable).OrderBy(b => b.Name), "ID", "Name");
            ViewBag.AttributeTypeList = db.SkuAttributeType.Where(t => t.IsEnable).OrderBy(t => t.Order).ToList();
            ViewBag.CompanyList = db.Company.AsNoTracking().Where(c => c.IsEnable).ToList();
            ViewBag.MarketList = db.Marketplace.Where(m => m.IsEnable).ToList();
            ViewBag.BoxTypeList = db.BoxType.Select(b => new SelectListItem() { Text = b.Name, Value = b.ID.ToString() }).ToList();

            var warehouseList = db.Warehouse.Where(w => w.IsEnable).ToList();
            ViewBag.WarehouseList = warehouseList;
            ViewBag.PurchaseSku = db.PurchaseSKU.Where(s => s.IsEnable && s.SkuNo.Equals(sku.SkuID)).ToList();
            ViewBag.TransferSku = db.TransferSKU.Where(s => s.IsEnable && s.SkuNo.Equals(sku.SkuID)).ToList();
            ViewBag.RMASku = db.RMASKU.Where(s => s.IsEnable && s.SkuNo.Equals(sku.SkuID)).ToList();
            ViewBag.AwaitingList = GetAwaitingCount(new string[] { sku.SkuID }, warehouseList.SelectMany(w => w.WarehouseSummary.Where(ws => ws.IsEnable && ws.Type.Equals("SCID"))).Select(w => w.Val).ToArray());

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

            var pictureList = db.SkuPicture.Where(p => !p.IsMain && p.Sku.Equals(sku) && p.PictureType.Equals("Pictrue")).OrderBy(p => p.Order).ToList();
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
                int Order = db.SKU.Find(ID).SkuPicture.Max(p => p.Order) + 1;

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
                            PictureType = "Picture",
                            FileName = string.Format("Sku/{0}/{1}", ID, fileName),
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

                System.IO.File.Delete(string.Format(Server.MapPath("~/Uploads/{0}"), picture.FileName));

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
            SKU sku = db.SKU.Find(id);
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
            SKU sku = db.SKU.Find(id);
            db.SKU.Remove(sku);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult GetData(SkuFilter filter, int page = 1, int rows = 100)
        {
            int total = 0;
            List<object> dataList = new List<object>();

            var LangID = !string.IsNullOrEmpty(filter.LangID) ? filter.LangID : EnumData.DataLangList().First().Key;
            var SkuFilter = db.SKU.Include(s => s.SkuLang).AsNoTracking().AsQueryable();
            if (!string.IsNullOrEmpty(filter.SkuID)) SkuFilter = SkuFilter.Where(s => s.SkuID.Contains(filter.SkuID));
            if (!string.IsNullOrEmpty(filter.ParentSku)) SkuFilter = SkuFilter.Where(s => s.ParentSku.Contains(filter.ParentSku));
            if (!string.IsNullOrEmpty(filter.Name)) SkuFilter = SkuFilter.Where(s => s.SkuLang.Any(l => l.LangID.Equals(LangID) && l.Name.ToLower().Contains(filter.Name.ToLower())));
            if (filter.Category.HasValue) SkuFilter = SkuFilter.Where(s => s.Category.Equals(filter.Category.Value));
            if (!string.IsNullOrEmpty(filter.UPC)) SkuFilter = SkuFilter.Where(s => s.UPC.Contains(filter.UPC));
            if (!string.IsNullOrEmpty(filter.EAN)) SkuFilter = SkuFilter.Where(s => s.EAN.Contains(filter.EAN));
            if (filter.Replenishable.HasValue) SkuFilter = SkuFilter.Where(s => s.Replenishable.Equals(filter.Replenishable.Value));
            if (filter.SerialTracking.HasValue) SkuFilter = SkuFilter.Where(s => s.SerialTracking.Equals(filter.SerialTracking.Value));
            if (filter.Battery.HasValue) SkuFilter = SkuFilter.Where(s => s.Battery.Equals(filter.Battery.Value));
            if (filter.Status.HasValue) SkuFilter = SkuFilter.Where(s => s.Status.Equals(filter.Status.Value));

            if (SkuFilter.Any())
            {
                int length = rows;
                int start = (page - 1) * length;
                total = SkuFilter.Count();
                var results = SkuFilter.OrderByDescending(s => s.SkuID).Skip(start).Take(length).ToList();

                dataList.AddRange(results.Select(s => new
                {
                    s.SkuID,
                    s.ParentSku,
                    Name = s.SkuLang.Any(l => l.LangID.Equals(LangID)) ? s.SkuLang.First(l => l.LangID.Equals(LangID)).Name : "",
                    s.Condition,
                    s.Category,
                    s.Brand,
                    s.UPC,
                    s.EAN,
                    s.Replenishable,
                    s.SerialTracking,
                    s.Battery,
                    s.Status
                }).ToList());
            }

            return Json(new { total, rows = dataList }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SearchSkuData(string term)
        {
            var list = new List<object>();

            if (!string.IsNullOrEmpty(term))
            {
                var LangID = EnumData.DataLangList().First().Key;
                var skuList = db.SKU.Include(s => s.SkuLang).Where(s => s.SkuID.Contains(term) || s.SkuLang.Any(l => l.Name.Contains(term))).ToList();
                if (skuList.Any())
                {
                    list.AddRange(skuList.OrderBy(s => s.SkuID).Select(s => new { label = s.SkuID + " - " + s.SkuLang.FirstOrDefault()?.Name ?? "Not found sku name", value = s.SkuID }).ToList());
                }
            }

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetParent(SkuFilter data, string filter)
        {
            int total = 0;
            List<object> dataList = new List<object>();

            string LangID = EnumData.DataLangList().First().Key;
            var SkuFilter = db.SKU.Include(s => s.SkuLang).AsNoTracking().AsQueryable();
            if (data.Type.HasValue) SkuFilter = SkuFilter.Where(s => s.Type.Equals(data.Type.Value));
            if (data.Condition.HasValue) SkuFilter = SkuFilter.Where(s => s.Condition.Equals(data.Condition.Value));
            if (data.Category.HasValue) SkuFilter = SkuFilter.Where(s => s.Category.Equals(data.Category.Value));
            if (data.Brand.HasValue) SkuFilter = SkuFilter.Where(s => s.Brand.Equals(data.Brand.Value));
            if (!string.IsNullOrEmpty(filter)) SkuFilter = SkuFilter.Where(s => s.SkuID.Contains(filter) || s.SkuLang.Any(l => l.Name.ToLower().Contains(filter.ToLower())));

            if (SkuFilter.Any())
            {
                total = SkuFilter.Count();
                var results = SkuFilter.OrderByDescending(s => s.SkuID).ToList();

                dataList.AddRange(results.Select(s => new
                {
                    s.SkuID,
                    s.SkuLang.FirstOrDefault(l => l.LangID.Equals(LangID))?.Name
                }).ToList());
            }

            return Json(new { total, rows = dataList }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FilterSku(string filter, string parentSku)
        {
            var langID = EnumData.DataLangList().First().Key;

            var skuFilter = db.SKU.Include(s => s.SkuLang).Where(s => s.IsEnable);
            if (!string.IsNullOrEmpty(parentSku)) skuFilter = skuFilter.Where(s => s.ParentSku.Equals(parentSku));
            var skuList = skuFilter.Where(s => s.SkuID.Contains(filter) || s.SkuLang.Any(l => l.Name.Contains(filter))).ToList();

            var data = skuList.Select(s => new { name = s.SkuLang.First(l => l.LangID.Equals(langID)).Name, value = s.SkuID }).ToArray();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Update(SKU updateData, SkuLang langData)
        {
            AjaxResult result = new AjaxResult();

            SKU sku = db.SKU.Find(updateData.SkuID);
            SetUpdateData(sku, updateData, EditList);
            db.Entry(sku).State = EntityState.Modified;

            langData.LangID = EnumData.DataLangList().First().Key;
            SkuLang skuLang = sku.SkuLang.First(l => l.LangID.Equals(langData.LangID));
            SetUpdateData(skuLang, langData, new string[] { "Name" });
            db.Entry(skuLang).State = EntityState.Modified;

            db.SaveChanges();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveSync(SKU updateData, SkuLang langData)
        {
            AjaxResult result = new AjaxResult();

            SKU sku = db.SKU.Find(updateData.SkuID);
            SetUpdateData(sku, updateData, EditList);
            db.Entry(sku).State = EntityState.Modified;

            langData.LangID = EnumData.DataLangList().First().Key;
            SkuLang skuLang = sku.SkuLang.First(l => l.LangID.Equals(langData.LangID));
            SetUpdateData(skuLang, langData, new string[] { "Name" });
            db.Entry(skuLang).State = EntityState.Modified;

            db.SaveChanges();

            using (StockKeepingUnit SKU = new StockKeepingUnit(sku))
            {
                JobProcess job = new JobProcess(string.Format("更新品號【{0}】資料至NETO、SC", sku.SkuID));
                try
                {
                    job.StatusLog(EnumData.TaskStatus.執行中);

                    job.AddLog("開始更新品號資料");

                    SKU.SC_Api = new SellerCloud_WebService.SC_WebService(ApiUserName, ApiPassword);
                    SKU.UpdateSkuToNeto();
                    SKU.UpdateSkuToSC();
                    job.AddLog("完成品號資料更新");

                    if (sku.Type.Equals((byte)EnumData.SkuType.Variation))
                    {
                        SKU.UpdateVariationToNeto();
                        job.AddLog("完成品號Variaction更新");
                    }

                    if (sku.Type.Equals((byte)EnumData.SkuType.Single) && sku.Condition.Equals(1))
                    {
                        foreach (var condition in db.Condition.Where(c => c.IsEnable && !c.ID.Equals(sku.Condition)).ToList())
                        {
                            SKU.SetSkuData(sku.SkuID + condition.Suffix);
                            SKU.UpdateSkuToNeto();
                            SKU.UpdateSkuToSC();
                        }
                        job.AddLog("完成品號U品更新");
                    }

                    job.StatusLog(EnumData.TaskStatus.執行完);
                }
                catch (Exception ex)
                {
                    job.Fail(ex.InnerException?.Message ?? ex.Message);
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLangData(string ID, string LangID)
        {
            AjaxResult result = new AjaxResult();

            SKU sku = db.SKU.Find(ID);
            SkuLang langData = sku.SkuLang.FirstOrDefault(l => l.LangID.Equals(LangID));

            ViewDataDictionary viewData = new ViewDataDictionary() { { "LangID", LangID } };
            viewData.Add("AttributeTypeList", db.SkuAttributeType.Where(t => t.IsEnable).OrderBy(t => t.Order).ToList());
            ViewBag.AttributeHtml = RenderViewToString(ControllerContext, "_SingleAttribute", sku, viewData);

            result.data = new
            {
                langData?.Name,
                langData?.Model,
                langData?.Description,
                langData?.PackageContent,
                langData?.FeatureContent,
                langData?.SpecContent,
                VariationList = sku.Type.Equals((byte)EnumData.SkuType.Variation) ? RenderViewToString(ControllerContext, "_VariationAttribute", sku, viewData) : "",
                KitList = sku.Type.Equals((byte)EnumData.SkuType.Kit) ? RenderViewToString(ControllerContext, "_SkuKit", sku, viewData) : "",
                ContentList = RenderViewToString(ControllerContext, "_Content", sku, viewData),
                FeatureList = RenderViewToString(ControllerContext, "_KeyFeature", sku, viewData),
                AttributeList = RenderViewToString(ControllerContext, "_SingleAttribute", sku, viewData)
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult RemoveVariation(string ID)
        {
            AjaxResult result = new AjaxResult();

            SKU sku = db.SKU.Find(ID);

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

        [HttpPost]
        public ActionResult RemoveKit(string ID, string ParentKit)
        {
            AjaxResult result = new AjaxResult();

            SKU sku = db.SKU.Find(ParentKit);

            try
            {
                if (sku == null) throw new Exception("Not found parent sku!");

                var kit = sku.GetKit.First(k => k.Sku.Equals(ID));
                sku.GetKit.Remove(kit);

                db.SaveChanges();
            }
            catch (Exception e)
            {
                result.status = false;
                result.message = e.InnerException != null ? e.InnerException.Message ?? e.Message : e.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPackageContent(string ID, string LangID, Sku_PackageContent[] SkuContent)
        {
            AjaxResult result = new AjaxResult();

            SKU sku = db.SKU.Find(ID);

            try
            {
                if (sku == null) throw new Exception("Not found sku!");

                if (SkuContent != null && SkuContent.Any())
                {
                    foreach (var content in SkuContent)
                    {
                        var contentModel = sku.Sku_PackageContent.FirstOrDefault(c => c.ItemID.Equals(content.ItemID) && c.LangID.Equals(content.LangID));
                        if (contentModel != null)
                        {
                            contentModel.Model = content.Model;
                            contentModel.Html = content.Html;
                            content.UpdateAt = sku.UpdateAt;
                            content.UpdateBy = sku.UpdateBy;
                        }
                        else
                        {
                            content.CreateAt = sku.UpdateAt.Value;
                            content.CreateBy = sku.UpdateBy;
                            sku.Sku_PackageContent.Add(content);
                        }
                    }
                    db.SaveChanges();
                }

                ViewDataDictionary viewData = new ViewDataDictionary() { { "LangID", LangID } };

                result.data = RenderViewToString(ControllerContext, "_PackageContent", sku, viewData);
            }
            catch (Exception e)
            {
                result.status = false;
                result.message = e.InnerException != null ? e.InnerException.Message ?? e.Message : e.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetFeatureContent(string ID, string LangID, string[] KeyFeature)
        {
            AjaxResult result = new AjaxResult();

            SKU sku = db.SKU.Find(ID);

            try
            {
                if (sku == null) throw new Exception("Not found sku!");

                if (KeyFeature != null && KeyFeature.Any())
                {
                    sku.SkuLang.First(l => l.LangID.Equals(LangID)).KeyFeature = JsonConvert.SerializeObject(KeyFeature);
                    db.SaveChanges();
                }

                result.data = "<p>Key Features</p><ul>" + (KeyFeature.Any() ? string.Join("", KeyFeature.Select(f => string.Format("<li><p>{0}</p></li>", f)).ToArray()) : "") + "</ul>";
            }
            catch (Exception e)
            {
                result.status = false;
                result.message = e.InnerException != null ? e.InnerException.Message ?? e.Message : e.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSpecContent(string ID, string LangID, Sku_Attribute[] AttributeValue)
        {
            AjaxResult result = new AjaxResult();

            SKU sku = db.SKU.Find(ID);

            try
            {
                if (sku == null) throw new Exception("Not found sku!");

                if (AttributeValue != null && AttributeValue.Any())
                {
                    List<Sku_Attribute> attributeData = AttributeValue.ToList();
                    List<Sku_Attribute> attributeList = sku.Sku_Attribute.Where(a => a.LangID.Equals(LangID)).ToList();
                    if (sku.Type.Equals((byte)EnumData.SkuType.Variation))
                    {
                        foreach (SKU childSku in db.SKU.Where(s => s.IsEnable && s.ParentSku.Equals(sku.SkuID)).ToList())
                        {
                            attributeData.AddRange(AttributeValue.Where(a => !a.IsDiverse).Select(a => new Sku_Attribute()
                            {
                                Sku = childSku.SkuID,
                                LangID = a.LangID,
                                AttrID = a.AttrID,
                                Value = a.Value,
                                Html = a.Html,
                                eBay = a.eBay
                            }));

                            attributeList.AddRange(childSku.Sku_Attribute.Where(a => a.LangID.Equals(LangID) && !a.IsDiverse).ToList());
                        }
                    }

                    // 新增Spec
                    List<Sku_Attribute> newAttributeList = attributeData.Except(attributeList).ToList();
                    foreach (var newAttr in newAttributeList)
                    {
                        newAttr.CreateAt = sku.UpdateAt.Value;
                        newAttr.CreateBy = sku.UpdateBy;
                    }
                    db.Sku_Attribute.AddRange(newAttributeList);

                    // 更新Spec
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

                ViewDataDictionary viewData = new ViewDataDictionary() { { "LangID", LangID } };
                viewData.Add("AttributeTypeList", db.SkuAttributeType.Where(t => t.IsEnable).OrderBy(t => t.Order).ToList());

                result.data = RenderViewToString(ControllerContext, "_SpecContent", sku, viewData);
            }
            catch (Exception e)
            {
                result.status = false;
                result.message = e.InnerException != null ? e.InnerException.Message ?? e.Message : e.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBoxData(int ID)
        {
            AjaxResult result = new AjaxResult();

            BoxType boxType = db.BoxType.Find(ID);

            try
            {
                if (boxType == null) throw new Exception("Not found box!");

                result.data = RenderViewToString(ControllerContext, "_BoxData", boxType);
            }
            catch (Exception e)
            {
                result.status = false;
                result.message = e.InnerException != null ? e.InnerException.Message ?? e.Message : e.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPurchaseSerial(string ID, int? WarehouseID, int? Days)
        {
            AjaxResult result = new AjaxResult();

            SKU sku = db.SKU.Find(ID);

            try
            {
                if (sku == null) throw new Exception("Not found sku!");

                var skuList = sku.PurchaseSKU.Where(s => s.IsEnable && ((s.PurchaseOrderID.HasValue && s.PurchaseOrder.IsEnable) || (s.CreditMemoID.HasValue && s.CreditMemo.IsEnable))).ToList();
                var SerialList = skuList.SelectMany(s => s.SerialsLlist).Where(s => !(s.TransferSKUID.HasValue && (!s.TransferSKU.IsEnable || !s.TransferSKU.Transfer.IsEnable))).OrderByDescending(s => s.ID).ToList();

                var warehouseList = db.Warehouse.Where(w => w.IsEnable).OrderBy(w => w.Name).ToList();
                ViewDataDictionary viewData = new ViewDataDictionary() { { "SerialData", SerialList }, { "WarehouseID", WarehouseID }, { "Days", Days } };
                viewData.Add("WarehouseList", warehouseList);
                viewData.Add("AwaitingList", GetAwaitingCount(new string[] { sku.SkuID }, warehouseList.SelectMany(w => w.WarehouseSummary.Where(ws => ws.IsEnable && ws.Type.Equals("SCID"))).Select(w => w.Val).ToArray()));

                result.data = RenderViewToString(ControllerContext, "_PurchaseSerial", sku, viewData);
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
