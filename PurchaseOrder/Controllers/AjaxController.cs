using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using OfficeOpenXml;
using PurchaseOrderSys.Models;
using PurchaseOrderSys.SCService;
using SellerCloud_WebService;

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
            var SKUList = new List<string>();
            //一般入庫
            var PurchaseSKUList = db.PurchaseSKU.Where(x => x.IsEnable && x.PurchaseOrder.IsEnable && x.PurchaseOrder.WarehouseID == FromWID).Select(x => x.SkuNo).ToList();
            SKUList.AddRange(PurchaseSKUList);
            //移倉入庫
            var TransferSKUList = db.TransferSKU.Where(x => x.IsEnable && x.Transfer.IsEnable && x.Transfer.ToWID == FromWID).Select(x => x.SkuNo).ToList();//只找移入的SKU
            SKUList.AddRange(TransferSKUList);
            //RMA入庫
            var RMAferSKUList = db.RMASerialsLlist.Where(x => x.RMASKU.IsEnable && x.RMASKU.RMA.IsEnable && x.WarehouseID == FromWID).Select(x => x.RMASKU.SkuNo).ToList();
            SKUList.AddRange(RMAferSKUList);

            SKUList = SKUList.Distinct().ToList();
            var dataList = db.SkuLang.Where(x => x.LangID == "en-US" && SKUList.Contains(x.Sku) && (x.Sku.Contains(Search) || x.Name.Contains(Search))).Take(20).Select(x => new SelectItem { id = x.Sku, text = x.Sku + "_" + x.Name });
            //var dataList = db.SkuLang.Where(x => x.LangID == "zh-tw" && (x.Sku.Contains(Search) || x.Name.Contains(Search))).Take(20).Select(x => new SelectItem { id = x.Sku, text = x.Sku + "_" + x.Name });
            return Json(new { items = dataList }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CMSkuNumberGet(string Search, int PurchaseOrderID)
        {
            var PurchaseSKUList = db.PurchaseSKU.Where(x => x.IsEnable && x.PurchaseOrder.IsEnable && x.PurchaseOrderID == PurchaseOrderID && x.SerialsLlist.Sum(y => y.SerialsQTY) > 0).Select(x => x.SkuNo);
            var dataList = db.SkuLang.Where(x => x.LangID == "en-US" && PurchaseSKUList.Contains(x.Sku) && (x.Sku.Contains(Search) || x.Name.Contains(Search))).Take(20).Select(x => new SelectItem { id = x.Sku, text = x.Sku + "_" + x.Name });
            //var dataList = db.SkuLang.Where(x => x.LangID == "zh-tw" && (x.Sku.Contains(Search) || x.Name.Contains(Search))).Take(20).Select(x => new SelectItem { id = x.Sku, text = x.Sku + "_" + x.Name });
            return Json(new { items = dataList }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetSkuNumberList(int? draw, int? start, int? length, int ID)
        {

            var PurchaseSKU = db.PurchaseSKU.Where(x => x.PurchaseOrderID == ID && x.IsEnable).ToList();
            try
            {
                var odataList = PurchaseSKU.Select(x => new PoSKUVM
                {
                    ID = x.ID,
                    ck = x.SkuNo,
                    sk = x.SkuNo,
                    SKU = x.SkuNo,
                    Name = x.Name,
                    VendorSKU = x.VendorSKU,
                    UPCEAN = x.UPCEAN,
                    QTYOrdered = x.QTYOrdered,
                    QTYFulfilled = x.QTYFulfilled,
                    QTYReceived = x.QTYReceived,
                    QTYReturned = x.QTYReturned,
                    Serial = x.SKU.SerialTracking ? "Yes" : "No",
                    SerialQTY = x.SerialsLlist.Where(y => y.SerialsType == "PO").Sum(y => y.SerialsQTY),
                    SerialTracking = x.SKU.SerialTracking,
                    Url = x.SKU.Logistic?.ImagePath,
                    Size = GetSize(x)
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
            catch (Exception ex)
            {

                return Json(ex.ToString(), JsonRequestBehavior.AllowGet);
            }

        }
        /// <summary>
        /// 取得產品規格資訊
        /// </summary>
        /// <param name="PurchaseSKU">PurchaseSKU</param>
        /// <returns></returns>
        private string GetSize(PurchaseSKU PurchaseSKU)
        {
            TagBuilder divHtml = new TagBuilder("div");
            var Size = "";
            var inch = 0.0393700787;
            var lbs = 0.00220462262;
            if (PurchaseSKU.SKU.Logistic == null)
            {
                divHtml.AddCssClass("nosize");
                Size += "Length：0(mm) ；0(inch)<br/>";
                Size += "Width：0(mm) ；0(inch)<br/>";
                Size += "Height：0(mm) ；0(inch)<br/>";
                Size += "Weight：0(g） ；0(lbs)";
            }
            else
            {
                if (PurchaseSKU.SKU.Logistic.ShippingLength == 0 || PurchaseSKU.SKU.Logistic.ShippingWidth == 0 || PurchaseSKU.SKU.Logistic.ShippingHeight == 0 || PurchaseSKU.SKU.Logistic.ShippingWeight == 0)
                {
                    divHtml.AddCssClass("nosize");
                }
                Size += "Length：" + PurchaseSKU.SKU.Logistic.ShippingLength + "(mm) ；" + (PurchaseSKU.SKU.Logistic.ShippingLength * inch).ToString("f2") + "(inch)<br/>";
                Size += "Width：" + PurchaseSKU.SKU.Logistic.ShippingWidth + "(mm) ；" + (PurchaseSKU.SKU.Logistic.ShippingWidth * inch).ToString("f2") + "(inch)<br/>";
                Size += "Height：" + PurchaseSKU.SKU.Logistic.ShippingHeight + "(mm) ；" + (PurchaseSKU.SKU.Logistic.ShippingHeight * inch).ToString("f2") + "(inch)<br/>";
                Size += "Weight：" + PurchaseSKU.SKU.Logistic.ShippingWeight + "(g） ；" + (PurchaseSKU.SKU.Logistic.ShippingWeight * lbs).ToString("f2") + "(lbs)";
            }
            divHtml.InnerHtml = Size;
            return divHtml.ToString();
        }

        public ActionResult GetCMSkuNumberList(int? draw, int? start, int? length, int ID)
        {
            var PurchaseSKU = db.PurchaseSKU.Where(x => x.CreditMemoID == ID && x.IsEnable).ToList();
            var odataList = PurchaseSKU.Select(x => new CMSKUVM
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
                SerialQTY = x.SerialsLlist.Count(),
                SerialTracking = x.SKU.SerialTracking,
                Url = x.SKU.Logistic?.ImagePath,
                Size = GetSize(x)
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
        public ActionResult TSkuNumberList(int? draw, int? start, int? length, string[] Skulist, int? FromWID, string SID)
        {
            var odataList = (List<TranSKUVM>)Session["TSkuNumberList" + SID];
            if (odataList == null)
            {
                odataList = new List<TranSKUVM>();
            }
            if (FromWID.HasValue)
            {
                if (Skulist != null && Skulist.Where(x => !string.IsNullOrWhiteSpace(x)).Any())
                {
                    //從PO單取SKU
                    var dataList = new List<TranSKUVM>();
                    dataList.AddRange(db.PurchaseSKU.Where(x => x.IsEnable && x.PurchaseOrder.IsEnable && x.PurchaseOrder.WarehouseID == FromWID && Skulist.Contains(x.SkuNo)).Select(x =>
                      new TranSKUVM
                      {
                          ck = x.SkuNo,
                          sk = x.SkuNo,
                          SKU = x.SkuNo,
                          ProductName = x.Name,
                          QTY = 0,
                          Model = "E"
                      }).Distinct().ToList());
                    //從移倉單選取SKU
                    if (!dataList.Any())
                    {
                        dataList.AddRange(db.TransferSKU.Where(x => x.IsEnable && x.Transfer.IsEnable && x.Transfer.ToWID == FromWID && Skulist.Contains(x.SkuNo)).Select(x =>
                        new TranSKUVM
                        {
                            ck = x.SkuNo,
                            sk = x.SkuNo,
                            SKU = x.SkuNo,
                            ProductName = x.Name,
                            QTY = 0,
                            Model = "E"
                        }).Distinct().ToList());
                    }
                    //從RMA單選取SKU
                    if (!dataList.Any())
                    {
                        dataList.AddRange(db.RMASerialsLlist.Where(x => x.RMASKU.IsEnable && x.RMASKU.RMA.IsEnable && x.WarehouseID == FromWID && Skulist.Contains(x.RMASKU.SkuNo)).Select(x =>
                        new TranSKUVM
                        {
                            ck = x.RMASKU.SkuNo,
                            sk = x.RMASKU.SkuNo,
                            SKU = x.RMASKU.SkuNo,
                            ProductName = x.RMASKU.Name,
                            QTY = 0,
                            Model = "E"
                        }).Distinct().ToList());
                    }


                    odataList.AddRange(dataList);
                    Session["TSkuNumberList" + SID] = odataList;
                }
                odataList = odataList.Where(x => x.Model != "D").ToList();
            }
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
        public ActionResult CMSkuNumberList(int? draw, int? start, int? length, string[] Skulist, int PurchaseOrderID, string SID)
        {
            var odataList = (List<CMSKUVM>)Session["CMSkuNumberList" + SID];
            if (odataList == null)
            {
                odataList = new List<CMSKUVM>();
            }
            if (Skulist != null && Skulist.Where(x => !string.IsNullOrWhiteSpace(x)).Any())
            {
                var PoSKUList = db.PurchaseOrder.Find(PurchaseOrderID).PurchaseSKU.Where(x => x.IsEnable && Skulist.Contains(x.SkuNo)).ToList();
                var dataList = PoSKUList.Select(x =>
                new CMSKUVM
                {
                    ck = x.SkuNo,
                    sk = x.SkuNo,
                    SKU = x.SkuNo,
                    Name = x.Name,
                    VendorSKU = x.VendorSKU,
                    QTYOrdered = x.QTYOrdered,
                    QTYReceived = 0,
                    QTYReturned = 0,
                    CreditQTY = 0,
                    Credit = 0,
                    Subtotal = 0,
                    Model = "E"
                }
                ).ToList();

                odataList.AddRange(dataList);
                Session["CMSkuNumberList" + SID] = odataList;
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
        public ActionResult SkuNumberList(int? draw, int? start, int? length, string SID, string[] Skulist)
        {
            var odataList = (List<PoSKUVM>)Session["SkuNumberList" + SID];
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
                    item.DiscountedPrice = (Price - Discount - Credit);
                    item.Subtotal = (QTYOrdered * (Price - Discount));
                }
                odataList.AddRange(dataList);
                Session["SkuNumberList" + SID] = odataList;
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
        public ActionResult SkuNumberListEdit(string SKU, string type, decimal? oval, decimal? val, int? ID, string SID)
        {
            var odataList = (List<PoSKUVM>)Session["SkuNumberList" + SID];
            if (odataList == null)
            {
                odataList = new List<PoSKUVM>();
            }

            foreach (var item in odataList.Where(x => ((x.ID.HasValue && x.ID != 0 && x.ID == ID) || (!x.ID.HasValue && x.ID != 0 && x.SKU == SKU))))
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

                item.DiscountedPrice = ((item.Price ?? 0) - (item.Discount ?? 0) - (item.Credit ?? 0));
                item.Subtotal = ((item.QTYOrdered ?? 0) * ((item.Price ?? 0) - (item.Discount ?? 0)));
            }
            Session["SkuNumberList" + SID] = odataList;
            return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult CMSkuNumberListEdit(string SKU, string type, decimal oval, decimal val, int? ID, string SID)
        {
            var odataList = (List<CMSKUVM>)Session["CMSkuNumberList" + SID];
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
            Session["CMSkuNumberList" + SID] = odataList;
            return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult TSkuNumberListQTY(string SKU, int val, string SID)
        {
            var odataList = (List<TranSKUVM>)Session["TSkuNumberList" + SID];
            foreach (var item in odataList.Where(x => x.SKU == SKU))
            {
                item.QTY = val;
                item.Model = "E";
            }
            Session["TSkuNumberList" + SID] = odataList;
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
                            optionList.Add(type, Enum.GetValues(typeof(EnumData.SkuStatus)).Cast<EnumData.SkuStatus>().Select(s => new { text = s.ToString(), value = ((byte)s).ToString() }));
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
                        //item.Date = item.Date.Value.ToUniversalTime();
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
                        result.SetError(Serialsitem.SerialsNo + "：此序號已經出貨");
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
                //俢改2019/03/11 SKYPE，改成品號內的圖檔
                if (PurchaseSKU.SKU.Logistic != null)
                {
                    GetImgVM.imglist.Add("../../Uploads/" + PurchaseSKU.SKU.Logistic.ImagePath);
                }
                //if (PurchaseSKU.ImgFile.Any())
                //{
                //    GetImgVM.imglist = PurchaseSKU.ImgFile.Where(x => x.IsEnable && x.ImgType == ImgType).Select(x => x.Url).ToList();
                //}
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
                            //俢改2019/03/11 SKYPE，改成品號內的圖檔
                            var Url = SaveImg(file, "~/Uploads/Sku/" + PurchaseSKU.SkuNo);
                            Url = Url.Replace("/Uploads/", "");
                            if (PurchaseSKU.SKU.Logistic == null)
                            {
                                PurchaseSKU.SKU.Logistic = new Logistic
                                {
                                    ImagePath = Url,
                                    BoxID = 1,
                                    CaseHeight = 0,
                                    CaseLength = 0,
                                    CaseWeight = 0,
                                    CaseWidth = 0,
                                    ShippingHeight = 0,
                                    ShippingLength = 0,
                                    ShippingWeight = 0,
                                    ShippingWidth = 0,
                                    CreateBy = UserBy,
                                    CreateAt = DateTime.UtcNow
                                };
                            }
                            else
                            {
                                PurchaseSKU.SKU.Logistic.ImagePath = Url;
                                PurchaseSKU.SKU.Logistic.UpdateAt = DateTime.UtcNow;
                                PurchaseSKU.SKU.Logistic.UpdateBy = UserBy;
                            }
                            //var Url = SaveImg(file);
                            //PurchaseSKU.ImgFile.Add(new ImgFile
                            //{
                            //    IsEnable = true,
                            //    ImgType = ImgType,
                            //    Url = Url,
                            //    CreateBy = UserBy,
                            //    CreateAt = DateTime.UtcNow
                            //});
                        }
                    }
                    db.SaveChanges();
                }
            }
            return Json(new { status = true, reload = true }, JsonRequestBehavior.AllowGet);
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

                    return Json(new { status = true, reload = true }, JsonRequestBehavior.AllowGet);
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
            if (VendorLIst != null)
            {
                return Json(new { status = true, VendorLIst.Currency, VendorLIst.Tax }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = false, Msg = "查無資料" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult GetSkuData(List<string> IDs)
        {
            AjaxResult result = new AjaxResult();

            try
            {
                string LangID = EnumData.DataLangList().First().Key;
                List<SKU> sku = db.SKU.Include(s => s.SkuLang).Include(s => s.SkuType).Where(s => s.IsEnable && IDs.Contains(s.SkuID)).ToList();

                result.data = sku.Select(s => new
                {
                    Sku = s.SkuID,
                    s.SkuLang.First(l => l.LangID.Equals(LangID)).Name,
                    Width = (s.Logistic?.ShippingWidth ?? 0).ToString(),
                    Height = (s.Logistic?.ShippingHeight ?? 0).ToString(),
                    Length = (s.Logistic?.ShippingLength ?? 0).ToString(),
                    Weight = (s.Logistic?.ShippingWeight ?? 500).ToString(),
                    s.SkuType.HSCode,
                    ImagePath = s.Logistic?.ImagePath ?? ""
                }).ToList();
            }
            catch (Exception e)
            {
                result.status = false;
                result.message = e.InnerException != null && !string.IsNullOrEmpty(e.InnerException.Message) ? e.InnerException.Message : e.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult CreateALLRMA(int OrderID, int ReturnWarehouseID)
        {
            AjaxResult result = new AjaxResult();
            var Reason = 16;
            var dt = DateTime.UtcNow;
            var SerialsLlist = db.SerialsLlist.Where(x => x.OrderID == OrderID && x.SerialsType == "Order");
            var WarehouseID = db.WarehouseSummary.Where(x => x.Type == "SCID" && x.Val == ReturnWarehouseID.ToString()).FirstOrDefault()?.ID;
            if (WarehouseID.HasValue)
            {
                try
                {
                    //建立SC上的RMA資料
                    SC_WebService SCWS = new SC_WebService("tim@weypro.com", "timfromweypro");
                    SCService.Order order = SCWS.Get_OrderData(OrderID).Order;//去SC抓訂單資料
                    order.OrderCreationSourceApplication = SCService.OrderCreationSourceApplicationType.PointOfSale;
                    if (SCWS.Update_Order(order))
                    {
                        int RMAId = SCWS.Create_RMA(order.ID);//建立RMAID
                        //建立RMA資料
                        var newRMA = new RMA
                        {
                            IsEnable=true,
                            OrderID = OrderID,
                            SourceID = order.OrderSourceOrderId,
                            CompanyID = order.CompanyId,
                            Country = GetCountryCode(order.ShippingAddress.CountryCode),
                            Status = "Closed",
                            Action = "None",
                            Channel = (int)order.OrderSource,
                            WarehouseID = WarehouseID,
                            FinalShippingFee = order.Packages.FirstOrDefault()?.FinalShippingFee ?? 0,
                            SCRMA = RMAId.ToString(),
                            CreateBy = "RMAAPI",
                            CreateAt = dt,
                            UpdateBy = "RMAAPI",
                            UpdateAt = dt
                        };

                        foreach (var item in order.Items)
                        {
                            var SKU = db.SKU.Find(item.ProductID)?.SkuLang.Where(x => x.LangID == "en-US").FirstOrDefault();
                            if (SKU == null)
                            {
                                result.SetError(item.ProductID + "：無此SKU資料");
                                return Json(result, JsonRequestBehavior.AllowGet);
                            }
                            var SKUSerialsLlist = SerialsLlist.Where(x => x.PurchaseSKU.SkuNo == item.ProductID).Select(x => x.SerialsNo);
                            string serialsList = string.Join(",", SKUSerialsLlist);
                            int RMAItemID = SCWS.Create_RMA_Item(item.OrderID, item.ID, RMAId, item.Qty, Reason, "");//建立每個SKU要退貨的數量原因，並取回ID
                            SCWS.Receive_RMA_Item(RMAId, RMAItemID, item.ProductID, item.Qty, ReturnWarehouseID, serialsList);//RMA入庫
                            SCWS.Delete_ItemSerials(item.OrderID, item.ID);//SC上的序號移除

                            //建立RMASKU資料
                            var UnitPrice = 0m;
                            decimal.TryParse(item.UnitPrice, out UnitPrice);
                            foreach (var Serialitem in SKUSerialsLlist)
                            {
                                var newRMASKU = new RMASKU
                                {
                                    IsEnable = true,
                                    Name = SKU.Name,
                                    SkuNo = SKU.Sku,
                                    QTYOrdered = 1,
                                    ReturnedQTY = 1,
                                    WarehouseID = WarehouseID,
                                    Reason = Reason.ToString(),
                                    UnitPrice = UnitPrice,
                                    RMAItemID= RMAItemID,
                                    CreateBy = "RMAAPI",
                                    CreateAt = dt,
                                };
                                newRMASKU.RMASerialsLlist.Add(new RMASerialsLlist
                                {
                                    SerialsNo = Serialitem,
                                    SerialsQTY = 1,
                                    Reason = Reason.ToString(),
                                    WarehouseID = WarehouseID,
                                    SerialsType = "RMAIn",
                                    CreateBy = "RMAAPI",
                                    CreateAt = dt,
                                    UpdateBy = "RMAAPI",
                                    UpdateAt = dt,
                                });
                                newRMA.RMASKU.Add(newRMASKU);
                            }
                        }
                        order.OrderCreationSourceApplication = OrderCreationSourceApplicationType.Default;
                        SCWS.Update_Order(order);
                        result.data = RMAId;
                        db.RMA.Add(newRMA);
                        db.SaveChanges();
                        //
                    }
                    else
                    {
                        result.SetError("無訂單資料");
                    }

                }
                catch (Exception ex)
                {
                    result.SetError(ex.ToString());
                }
            }
            else
            {
                result.SetError("SCID查無對應的倉庫");
            }
            return Json(result, JsonRequestBehavior.AllowGet);
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