﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using OfficeOpenXml;
using PurchaseOrderSys.Models;

namespace PurchaseOrderSys.Controllers
{
    public class AjaxController : BaseController
    {
        [HttpPost]
        public ActionResult Language(string Lang)
        {
            if (!string.IsNullOrEmpty(Lang))
            {
                HttpCookie LangCookie = Request.Cookies["Lang"];
                if (LangCookie != null)
                {
                    LangCookie.Value = Lang.Trim();
                }
                else
                {
                    LangCookie = new HttpCookie("Lang");
                    LangCookie.Value = Lang.Trim();
                    LangCookie.Expires.AddDays(30);
                }
                Response.Cookies.Add(LangCookie);
                //System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(Lang);
                //System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(Lang);
            }
            return Json(new { status = true, Lang }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult MenuListAuth(int id, string mod, int? Gid)
        {
            ViewBag.mod = mod;
            var Menu = db.Menu.Where(x => x.IsEnable).Include(m => m.MenuParent).AsQueryable();//選單
            if (Gid.HasValue)
            {
                var UserAuth = db.AdminUser.Find(id).Auth;//使用者
                var AdminGroup = db.AdminGroup.Find(Gid);//群組
                if (mod == "UE" || mod == "UI")
                {
                    var AuthItem = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(AdminGroup.Auth).Where(x => x.Value.Any()).Select(x => int.Parse(x.Key));
                    Menu = Menu.Where(x => x.MenuChild.Where(y => AuthItem.Contains(y.MenuID)).Any());
                }
                ViewBag.menu = Menu;
                if (!string.IsNullOrWhiteSpace(UserAuth))
                {
                    ViewBag.UserAuth = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(UserAuth);
                }
                return View(AdminGroup);
            }
            else
            {
                var AdminGroup = db.AdminGroup.Find(id);//群組
                if (mod == "UE")
                {
                    var AuthItem = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(AdminGroup.Auth).Where(x => x.Value.Any()).Select(x => int.Parse(x.Key));
                    Menu = Menu.Where(x => x.MenuChild.Where(y => AuthItem.Contains(y.MenuID)).Any());
                }
                ViewBag.menu = Menu;
                return View(AdminGroup);
            }

        }

        public ActionResult SkuNumberGet(string Search)
        {
            var dataList = db.SkuLang.Where(x => x.LangID == "en-US" && (x.Sku.Contains(Search) || x.Name.Contains(Search))).Take(20).Select(x => new SelectItem { id = x.Sku, text = x.Sku + "_" + x.Name });
            return Json(new { items = dataList }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult TSkuNumberGet(string Search, int FromWID)
        {
            var PurchaseSKUList = db.PurchaseSKU.Where(x => x.IsEnable && x.PurchaseOrder.IsEnable && x.PurchaseOrder.WarehouseID == FromWID).Select(x => x.SkuNo);
            var dataList = db.SkuLang.Where(x => x.LangID == "en-US" && PurchaseSKUList.Contains(x.Sku) && (x.Sku.Contains(Search) || x.Name.Contains(Search))).Take(20).Select(x => new SelectItem { id = x.Sku, text = x.Sku + "_" + x.Name });
            //var dataList = db.SkuLang.Where(x => x.LangID == "zh-tw" && (x.Sku.Contains(Search) || x.Name.Contains(Search))).Take(20).Select(x => new SelectItem { id = x.Sku, text = x.Sku + "_" + x.Name });
            return Json(new { items = dataList }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetSkuNumberList(int? draw, int? start, int? length, int ID)
        {
            var odataList = db.PurchaseSKU.Where(x => x.PurchaseOrderID == ID && x.IsEnable).Select(x => new PoSKUVM
            {
                ID = x.ID,
                ck = x.SkuNo,
                sk = x.SkuNo,
                SKU = x.SkuNo,
                Name = x.Name,
                VendorSKU = x.VendorSKU,
                UPCEAN = "",
                QTYOrdered = x.QTYOrdered,
                QTYFulfilled = x.QTYFulfilled,
                QTYReceived = x.QTYReceived,
                QTYReturned = x.QTYReturned,
                Serial = x.SerialsLlist.Any() ? "Yes" : "No",
                SerialQTY = x.SerialsLlist.Count(),
            }).ToList();
            int recordsTotal = odataList.Count();
            var returnObj =
            new
            {
                draw = draw,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = odataList//分頁後的資料 
            };
            return Json(returnObj, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetCMSkuNumberList(int? draw, int? start, int? length, int ID)
        {
            var odataList = db.PurchaseSKU.Where(x => x.CreditMemoID == ID && x.IsEnable).Select(x => new CMSKUVM
            {
                ID = x.ID,
                ck = x.SkuNo,
                sk = x.SkuNo,
                SKU = x.SkuNo,
                Name = x.Name,
                VendorSKU = x.VendorSKU,
                UPCEAN = "",
                QTYOrdered = x.QTYOrdered,
                QTYReceived = x.QTYReceived,
                QTYReturned = x.QTYReturned,
                Serial = x.SerialsLlist.Any() ? "Yes" : "No",
                SerialQTY = x.SerialsLlist.Count()
            }).ToList();
            int recordsTotal = odataList.Count();
            var returnObj =
            new
            {
                draw = draw,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = odataList//分頁後的資料 
            };
            return Json(returnObj, JsonRequestBehavior.AllowGet);
        }
        public ActionResult TSkuNumberList(int? draw, int? start, int? length, string[] Skulist, int FromWID)
        {
            var odataList = (List<TranSKUVM>)Session["TSkuNumberList"];
            if (odataList == null)
            {
                odataList = new List<TranSKUVM>();
            }
            if (Skulist != null && Skulist.Where(x => !string.IsNullOrWhiteSpace(x)).Any())
            {
                var dataList = db.PurchaseSKU.Where(x => x.IsEnable && x.PurchaseOrder.IsEnable && x.PurchaseOrder.WarehouseID == FromWID && Skulist.Contains(x.SkuNo)).Select(x =>
                  new TranSKUVM
                  {
                      ck = x.SkuNo,
                      sk = x.SkuNo,
                      SKU = x.SkuNo,
                      ProductName = x.Name,
                      QTY = 0,
                      Model = "E"
                  }
                ).Distinct().ToList();
                //foreach (var item in dataList)
                //{
                //    var QTYOrdered = RandomVal(100, 1000);
                //    var QTYFulfilled = RandomVal(100, 1000);
                //    var Price = RandomVal(1000, 30000);
                //    var Discount = RandomVal(0, 100);
                //    var Credit = RandomVal(0, 100);

                //}
                odataList.AddRange(dataList);
                Session["TSkuNumberList"] = odataList;
            }
            odataList = odataList.Where(x => x.Model != "D").ToList();
            int recordsTotal = odataList.Count();
            var returnObj =
            new
            {
                draw = draw,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = odataList//分頁後的資料 
            };
            return Json(returnObj, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CMSkuNumberList(int? draw, int? start, int? length, string[] Skulist)
        {
            var odataList = (List<CMSKUVM>)Session["CMSkuNumberList"];
            if (odataList == null)
            {
                odataList = new List<CMSKUVM>();
            }
            if (Skulist != null && Skulist.Where(x => !string.IsNullOrWhiteSpace(x)).Any())
            {
                var dataList = db.SkuLang.Where(x => x.LangID == "en-US" && Skulist.Contains(x.Sku)).Select(x =>
                new CMSKUVM
                {
                    ck = x.Sku,
                    sk = x.Sku,
                    SKU = x.Sku,
                    Name = x.Name,
                    VendorSKU = "",
                    QTYOrdered = 0,
                    QTYReceived = 0,
                    QTYReturned = 0,
                    CreditQTY = 0,
                    Credit = 0,
                    Subtotal = 0,
                    Model = "E"
                }
                ).ToList();

                odataList.AddRange(dataList);
                Session["CMSkuNumberList"] = odataList;
            }
            odataList = odataList.Where(x => x.Model != "D").ToList();
            int recordsTotal = odataList.Count();
            var returnObj =
            new
            {
                draw = draw,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = odataList//分頁後的資料 
            };
            return Json(returnObj, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SkuNumberList(int? draw, int? start, int? length, string[] Skulist)
        {
            var odataList = (List<PoSKUVM>)Session["SkuNumberList"];
            if (odataList == null)
            {
                odataList = new List<PoSKUVM>();
            }
            if (Skulist != null && Skulist.Where(x => !string.IsNullOrWhiteSpace(x)).Any())
            {
                var dataList = db.SkuLang.Where(x => x.LangID == "en-US" && Skulist.Contains(x.Sku)).Select(x =>
                new PoSKUVM
                {
                    ck = x.Sku,
                    sk = x.Sku,
                    SKU = x.Sku,
                    Name = x.Name,
                    VendorSKU = "",
                    Model = "E"
                }
                ).ToList();
                foreach (var item in dataList)
                {
                    var QTYOrdered = 0;
                    var QTYFulfilled = 0;
                    var Price = 0;
                    var Discount = 0;
                    var Credit = 0;
                    item.QTYOrdered = QTYOrdered;
                    item.QTYFulfilled = QTYFulfilled;
                    item.Price = Price;
                    item.Discount = Discount;
                    item.Credit = Credit;
                    item.DiscountedPrice = (Price - Discount- Credit);
                    item.Subtotal = (QTYOrdered * (Price - Discount));
                }
                odataList.AddRange(dataList);
                Session["SkuNumberList"] = odataList;
            }
            odataList = odataList.Where(x => x.Model != "D").ToList();
            int recordsTotal = odataList.Count();
            var returnObj =
            new
            {
                draw = draw,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = odataList//分頁後的資料 
            };
            return Json(returnObj, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult SkuNumberListEdit(string SKU, string type, decimal? oval, decimal? val, int? ID)
        {
            var odataList = (List<PoSKUVM>)Session["SkuNumberList"];
            if (odataList == null)
            {
                odataList = new List<PoSKUVM>();
            }

            foreach (var item in odataList.Where(x => ((x.ID.HasValue && x.ID != 0 && x.ID == ID) ||(!x.ID.HasValue && x.ID != 0 && x.SKU == SKU))))
            {
                switch (type)
                {
                    case "Price":
                        item.Price = val;
                        break;
                    case "Discount":
                        item.Discount = val;
                        break;
                    case "Credit":
                        item.Credit = val;
                        break;
                    case "QTYOrdered":
                        item.QTYOrdered = Convert.ToInt32(val);
                        break;
                }

                item.DiscountedPrice = ((item.Price??0 )- (item.Discount ?? 0) - (item.Credit ?? 0));
                item.Subtotal = ((item.QTYOrdered ?? 0) * ((item.Price ?? 0) - (item.Discount ?? 0)));
            }
            Session["SkuNumberList"] = odataList;
            return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult CMSkuNumberListEdit(string SKU, string type, decimal oval, decimal val, int? ID)
        {
            var odataList = (List<CMSKUVM>)Session["CMSkuNumberList"];
            if (odataList == null)
            {
                odataList = new List<CMSKUVM>();
            }
            foreach (var item in odataList.Where(x => ((x.ID.HasValue && x.ID != 0 && x.ID == ID) || (!x.ID.HasValue && x.ID != 0 && x.SKU == SKU))))
            {
                switch (type)
                {
                    case "QTYOrdered":
                        item.QTYOrdered = Convert.ToInt32(val); ;
                        break;
                    case "QTYReturned":
                        item.QTYReturned = Convert.ToInt32(val); ;
                        break;
                    case "CreditQTY":
                        item.CreditQTY = Convert.ToInt32(val);
                        break;
                    case "Credit":
                        item.Credit = Convert.ToInt32(val); ;
                        break;
                    case "QTYReceived":
                        item.QTYReceived = Convert.ToInt32(val); ;
                        break;
                }
                item.Subtotal = (item.CreditQTY * item.Credit) ?? 0;
            }
            Session["CMSkuNumberList"] = odataList;
            return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult TSkuNumberListQTY(string SKU, int val)
        {
            var odataList = (List<TranSKUVM>)Session["TSkuNumberList"];
            foreach (var item in odataList.Where(x => x.SKU == SKU))
            {
                item.QTY = val;
                item.Model = "E";
            }
            Session["TSkuNumberList"] = odataList;
            return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult GetSelectOption(string[] optionType)
        {
            AjaxResult result = new AjaxResult();

            try
            {
                if (!optionType.Any()) throw new Exception("沒有給項目!");

                var LangID = EnumData.DataLangList().First().Key;
                Dictionary<string, object> optionList = new Dictionary<string, object>();

                foreach (string type in optionType)
                {
                    switch (type)
                    {
                        case "ApprevedForExport":
                        case "Replenishable":
                            optionList.Add(type, Enum.GetValues(typeof(EnumData.YesNo)).Cast<EnumData.YesNo>().Select(e => new { text = e.ToString(), value = Convert.ToBoolean((int)e).ToString() }));
                            break;
                        case "CompanyOption":
                            var companyList = new List<object>() { new { text = "Not Choose", value = "" } };
                            companyList.AddRange(db.Company.AsNoTracking().Where(c => c.IsEnable).Select(c => new { text = c.Name, value = c.ID.ToString() }));
                            optionList.Add(type, companyList);
                            break;
                        case "SkuAttributeType":
                            optionList.Add(type, db.SkuAttributeType.AsNoTracking().Where(t => t.IsEnable).Select(t => new { text = t.Name, value = t.ID.ToString() }));
                            break;
                        case "SkuAttributeProperty":
                            optionList.Add(type, Enum.GetValues(typeof(EnumData.AttributeProperty)).Cast<EnumData.AttributeProperty>().Select(p => new { text = p.ToString(), value = ((int)p).ToString() }));
                            break;
                        case "SkuCondition":
                            optionList.Add(type, db.ConditionLang.AsNoTracking().Where(l => l.LangID.Equals(LangID)).Select(l => new { text = l.Name, value = l.ConditionID.ToString() }));
                            break;
                        case "SkuType":
                            optionList.Add(type, db.SkuTypeLang.AsNoTracking().Where(l => l.LangID.Equals(LangID)).Select(l => new { text = l.Name, value = l.TypeID.ToString() }));
                            break;
                        case "SkuStatus":
                            optionList.Add(type, Enum.GetValues(typeof(EnumData.SkuStatus)).Cast<EnumData.SkuStatus>().Select(s => new { text = s.ToString(), value = (byte)s }));
                            break;
                        case "Company":
                            optionList.Add(type, db.Company.Where(c => c.IsEnable).Select(c => new { text = c.Name, value = c.ID.ToString() }));
                            break;
                        case "Currency":
                            optionList.Add(type, db.Currency.Select(c => new { text = c.Code + " - " + c.Name, value = c.ID.ToString() }));
                            break;
                        case "NetoGroup":
                            var netGroupList = new List<object>() { new { text = "Not Choose", value = "" } };
                            netGroupList.AddRange(db.NetoGroup.Select(c => new { text = c.Name, value = c.ID.ToString() }));
                            optionList.Add(type, netGroupList);
                            break;
                    }
                }

                result.data = optionList;
            }
            catch (Exception e)
            {
                result.SetError(e.InnerException != null && !string.IsNullOrEmpty(e.InnerException.Message) ? e.InnerException.Message : e.Message);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult OrderLogList(List<OrderLog> OrderLog)
        {
            AjaxResult result = new AjaxResult();

            //OrderLog = new List<OrderLog>();
            //OrderLog.Add(new OrderLog { OrderID = 5563222, SCID = "109", SkuNo = "106008065", Qty = 1, State = "Completed", Date = DateTime.UtcNow });
            try
            {
                if (OrderLog != null && OrderLog.Any())
                {
                    var SCIDList = db.WarehouseSummary.Where(x => x.IsEnable && x.Type == "SCID").Select(x => new { x.WarehouseID, SCID = x.Val }).ToList();
                    foreach (var item in OrderLog)
                    {
                        if (!item.WarehouseID.HasValue)
                        {
                            item.WarehouseID = SCIDList.Where(x => x.SCID == item.SCID).FirstOrDefault()?.WarehouseID;
                        }
                        //db.OrderLog.Add(new OrderLog
                        //{
                        //    Date = item.Date,
                        //    OrderID = item.OrderID,
                        //    Qty = item.Qty,
                        //    SCID = item.SCID,
                        //    SkuNo = item.SkuNo,
                        //    State = item.State,
                        //    WarehouseID = item.WarehouseID
                        //});
                    }
                    db.OrderLog.AddRange(OrderLog);
                    db.SaveChanges();
                }
                else
                {
                    result.SetError("沒有資料");
                }
            }
            catch (Exception ex)
            {

                result.SetError(ex.ToString());
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ShipmentByOrder(List<ShipmentOrder> ShipmentOrder)
        {
            AjaxResult result = new AjaxResult();
            var NoDataList = new List<string>();
            foreach (var Serialsitem in ShipmentOrder)
            {
                var QTY = 0;
                if (Serialsitem.QTY > 0)
                {
                    QTY = -1 * Serialsitem.QTY;
                }
                else
                {
                    QTY = Serialsitem.QTY;
                }
                var SerialsLlist = db.SerialsLlist.Where(x => x.SerialsNo == Serialsitem.SerialsNo);//檢查是否有序號
                var PurchaseSKU = db.PurchaseSKU.Where(x => x.SkuNo == Serialsitem.SkuNo && x.IsEnable);
                if (SerialsLlist.Any())
                {
                    SerialsLlist = SerialsLlist.Where(x => x.SerialsQTY > 0 && (x.SerialsType == "PO" || x.SerialsType == "TransferIn") && !x.SerialsLlistC.Any());//PO和TransferIn才能出貨
                    if (SerialsLlist.Sum(x => x.SerialsQTY) > 0)
                    {
                        var nSerials = new SerialsLlist
                        {
                            OrderID = Serialsitem.OrderID,
                            PurchaseSKUID = SerialsLlist.FirstOrDefault().PurchaseSKUID,
                            PID = SerialsLlist.FirstOrDefault().ID,
                            SerialsNo = Serialsitem.SerialsNo,
                            SerialsType = "Order",
                            SerialsQTY = QTY,
                            CreateAt = DateTime.UtcNow,
                            CreateBy = "APIUser",
                        };
                        db.SerialsLlist.Add(nSerials);
                        db.SaveChanges();
                    }
                    else
                    {
                        result.SetError("此序號已經出貨");
                    }
                }
                else if (PurchaseSKU.Any())
                {
                    //if (!PurchaseSKU.Where(x=>x.SerialsLlist.Where(y=>y.SerialsNo== Serialsitem.SerialsNo).Any()).Any())
                    //{

                    //}
                    var nSerials = new SerialsLlist
                    {
                        OrderID = Serialsitem.OrderID,
                        PurchaseSKUID = PurchaseSKU.FirstOrDefault().ID,
                        SerialsNo = Serialsitem.SerialsNo,
                        SerialsType = "Order",
                        SerialsQTY = QTY,
                        CreateAt = DateTime.UtcNow,
                        CreateBy = "APIUser",
                    };
                    db.SerialsLlist.Add(nSerials);
                    db.SaveChanges();
                }
                else
                {
                    NoDataList.Add(Serialsitem.OrderID + "：(SKU：" + Serialsitem.SkuNo + ";Serial：" + Serialsitem.SerialsNo + ")");
                }
            }
            if (NoDataList.Any())
            {
                result.SetError("查無序號及貨號：" + string.Join(",", NoDataList));
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult GetImg(int id, string key, string ImgType)
        {
            var GetImgVM = new GetImgVM
            {
                id = id,
                ImgType = ImgType,
                key = key,
                imglist = new List<string>()
            };
            if (key == "SKU")
            {
                var PurchaseSKU = db.PurchaseSKU.Find(id);
                if (PurchaseSKU.ImgFile.Any())
                {
                    GetImgVM.imglist = PurchaseSKU.ImgFile.Where(x => x.IsEnable && x.ImgType == ImgType).Select(x => x.Url).ToList();
                }
            }
            return View(GetImgVM);
        }
        [HttpPost]
        public ActionResult ImgUpload(int id, string key, string ImgType, IEnumerable<HttpPostedFileBase> ImgFile)
        {
            if (key == "SKU")
            {
                var PurchaseSKU = db.PurchaseSKU.Find(id);
                if (ImgFile != null && ImgFile.Any())
                {
                    foreach (var file in ImgFile)
                    {
                        if (file != null)
                        {
                            var Url = SaveImg(file);
                            PurchaseSKU.ImgFile.Add(new ImgFile
                            {
                                IsEnable = true,
                                ImgType = ImgType,
                                Url = Url,
                                CreateBy = UserBy,
                                CreateAt = DateTime.UtcNow
                            });
                        }
                    }
                    db.SaveChanges();
                }
            }
            return Json(new { status = true }, JsonRequestBehavior.AllowGet);
            //return RedirectToAction("GetImg", new { id, key, ImgType });
        }

        public ActionResult SerialsExcel(int id, string key)
        {
            ViewBag.id = id;
            ViewBag.id = key;
            return View();
        }
        [HttpPost]
        public ActionResult UpSerialsExcel(int id, string key, HttpPostedFileBase ExcelFile)
        {
            if (key == "Addserials")//序號上傳
            {
                var ExcelSerialslist = new List<string>();
                using (var package = new ExcelPackage(ExcelFile.InputStream))
                {

                    ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                    int col = 1;

                    for (int row = 2; worksheet.Cells[row, col].Value != null; row++)
                    {
                        ExcelSerialslist.Add(worksheet.Cells[row, col].Value.ToString());
                    }
                }
                var PurchaseSKU = db.PurchaseSKU.Find(id);

                //比對資料，並寫回資料庫
                string Err = SerialsAddCheck(PurchaseSKU, ExcelSerialslist, "");
                if (string.IsNullOrWhiteSpace(Err))
                {
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {

                        return Json(new { status = false, Msg = ex.ToString() }, JsonRequestBehavior.AllowGet);
                    }
                  
                    return Json(new { status = true , reload =true}, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { status = false, Msg = Err }, JsonRequestBehavior.AllowGet);
                }
            }
            else if (key == "AddSKUserials")//SKU及序號上傳
            {
                var PurchaseOrder = db.PurchaseOrder.Find(id);
                PurchaseOrder.ReceiveStatus = "Pending";
                using (var package = new ExcelPackage(ExcelFile.InputStream))
                {
                    string Err = "";
                    var dt = DateTime.UtcNow;
                    var AddSKUserialsVMList = new List<AddSKUserialsVM>();
                    var EditSKUserialsVMList = new List<AddSKUserialsVM>();
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                    for (int row = 2; worksheet.Cells[row, 1].Value != null; row++)
                    {
                        var SKU = worksheet.Cells[row, 1].Value?.ToString();
                        var serials = worksheet.Cells[row, 2].Value?.ToString();
                        var ProductName = worksheet.Cells[row, 3].Value?.ToString();
                        var Price = 0m;
                        decimal.TryParse(worksheet.Cells[row, 4].Value?.ToString(), out Price);
                        var Discount = 0m;
                        decimal.TryParse(worksheet.Cells[row, 5].Value?.ToString(), out Discount);
                        if (PurchaseOrder.PurchaseSKU.Where(x => x.IsEnable && x.SkuNo == SKU && x.Price == Price).Any())
                        {
                            EditSKUserialsVMList.Add(new AddSKUserialsVM
                            {
                                Discount = Discount,
                                Price = Price,
                                ProductName = ProductName,
                                serials = serials,
                                SKU = SKU
                            });
                        }
                        else
                        {
                            AddSKUserialsVMList.Add(new AddSKUserialsVM
                            {
                                Discount = Discount,
                                Price = Price,
                                ProductName = ProductName,
                                serials = serials,
                                SKU = SKU
                            });
                        }
                    }
                    #region 直接匯入序號

                    if (EditSKUserialsVMList.Any())
                    {
                        var Editchkserials = EditSKUserialsVMList.GroupBy(x => x.serials).Select(x => new { key = x.Key, count = x.Count() }).Where(x => x.count > 1).ToList();
                        if (Editchkserials.Any())
                        {
                            var list = Editchkserials.Select(x => x.key);
                            return Json(new { status = false, Msg = "序號有複：" + string.Join(",", list) }, JsonRequestBehavior.AllowGet);
                        }
                        var EditGroupSKU = EditSKUserialsVMList.GroupBy(x => new { x.SKU, x.Price, x.Discount }).Select(x => new { x.Key, QTYOrdered = x.Count(), Groupitem = x.Select(y => y.serials).ToList() }).ToList();
                        foreach (var item in EditGroupSKU)
                        {
                            var SKU = db.SKU.Find(item.Key.SKU);
                            if (SKU == null)
                            {
                                return Json(new { status = false, Msg = "SKU：" + item.Key.SKU + "不存在" }, JsonRequestBehavior.AllowGet);
                            }
                            try
                            {
                                var oPurchaseSKU = PurchaseOrder.PurchaseSKU.Where(x => x.IsEnable && x.SkuNo == item.Key.SKU && x.Price == item.Key.Price).FirstOrDefault();
                                //比對資料，並寫回資料庫
                                Err = SerialsAddCheck(oPurchaseSKU, item.Groupitem, PurchaseOrder.POType);
                                if (!string.IsNullOrWhiteSpace(Err))
                                {
                                    return Json(new { status = false, Msg = Err }, JsonRequestBehavior.AllowGet);
                                }
                                // PurchaseOrder.PurchaseSKU.Add(oPurchaseSKU);
                            }
                            catch (Exception ex)
                            {

                                return Json(new { status = false, Msg = ex.ToString() }, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                    #endregion
                    #region 新增
                    if (AddSKUserialsVMList.Any())
                    {

                        var Addchkserials = AddSKUserialsVMList.GroupBy(x => x.serials).Select(x => new { key = x.Key, count = x.Count() }).Where(x => x.count > 1).ToList();
                        if (Addchkserials.Any())
                        {
                            var list = Addchkserials.Select(x => x.key);
                            return Json(new { status = false, Msg = "序號有複：" + string.Join(",", list) }, JsonRequestBehavior.AllowGet);
                        }
                        var AddGroupSKU = AddSKUserialsVMList.GroupBy(x => new { x.SKU, x.Price, x.Discount }).Select(x => new { x.Key, QTYOrdered = x.Count(), Groupitem = x.Select(y => y.serials).ToList() }).ToList();

                        foreach (var item in AddGroupSKU)
                        {
                            var SKU = db.SKU.Find(item.Key.SKU);
                            if (SKU == null)
                            {
                                return Json(new { status = false, Msg = "SKU：" + item.Key.SKU + "不存在" }, JsonRequestBehavior.AllowGet);
                            }
                            try
                            {
                                var nPurchaseSKU = new PurchaseSKU
                                {
                                    IsEnable = true,
                                    SkuNo = item.Key.SKU,
                                    VendorSKU = item.Key.SKU,
                                    Name = SKU?.SkuLang.FirstOrDefault()?.Name,
                                    Price = item.Key.Price,
                                    Discount = item.Key.Discount,
                                    QTYOrdered = item.QTYOrdered,
                                    CreateBy = UserBy,
                                    CreateAt = dt

                                };
                                //比對資料，並寫回資料庫
                                Err = SerialsAddCheck(nPurchaseSKU, item.Groupitem, PurchaseOrder.POType);
                                if (!string.IsNullOrWhiteSpace(Err))
                                {
                                    return Json(new { status = false, Msg = Err }, JsonRequestBehavior.AllowGet);
                                }
                                PurchaseOrder.PurchaseSKU.Add(nPurchaseSKU);
                            }
                            catch (Exception ex)
                            {

                                return Json(new { status = false, Msg = ex.ToString() }, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                    #endregion
                    if (string.IsNullOrWhiteSpace(Err))
                    {
                        try
                        {
                            db.SaveChanges();
                            return Json(new { status = true, reload = true }, JsonRequestBehavior.AllowGet);
                        }
                        catch (Exception ex)
                        {

                            return Json(new { status = false, Msg = ex.ToString() }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        return Json(new { status = false, Msg = Err }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            else
            {
                return Json(new { status = false, Msg = "參數錯誤" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult GateVendorCurrency(int id)
        {
            var VendorLIst = db.VendorLIst.Find(id);
            if (VendorLIst!=null)
            {
                return Json(new { status = true, VendorLIst.Currency, VendorLIst.Tax }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = false, Msg = "查無資料" }, JsonRequestBehavior.AllowGet);
            }
        }

    }


    public class ShipmentOrder
    {
        public int OrderID { get; set; }
        public string SkuNo { get; set; }
        public string SerialsNo { get; set; }
        public int QTY { get; set; }
    }
    public class AjaxResult
    {
        public bool status { get; set; }
        public string message { get; set; }
        public object data { get; set; }

        public AjaxResult()
        {
            status = true;
        }

        public void SetError(string msg)
        {
            status = false;
            message = msg;
        }
    }
}