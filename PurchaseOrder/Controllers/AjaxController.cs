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
        protected static bool UpdateSC = true;
        public AjaxController()
        {
            var TestMod = System.Configuration.ConfigurationManager.AppSettings["TestMod"];
            if (TestMod == "true")
            {
                UpdateSC = false;
            }
        }
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
            var dataList = db.SkuLang.Where(x => x.LangID == LangID && (x.Sku.Contains(Search) || x.Name.Contains(Search))).Take(20).Select(x => new SelectItem { id = x.Sku, text = x.Sku + "_" + x.Name });
            return Json(new { items = dataList }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult TSkuNumberGet(string Search, int FromWID,string Key)
        { 
            var SKUList = new List<string>();
            SKUList = SearchSkuByWarehouse(Search, FromWID);
            var dataList = db.SkuLang.Where(x => x.LangID == LangID && SKUList.Contains(x.Sku) && (x.Sku.Contains(Search) || x.Name.Contains(Search))).Take(20).Select(x => new SelectItem { id = x.Sku, text = x.Sku + "_" + x.Name });
            //var dataList = db.SkuLang.Where(x => x.LangID == "zh-tw" && (x.Sku.Contains(Search) || x.Name.Contains(Search))).Take(20).Select(x => new SelectItem { id = x.Sku, text = x.Sku + "_" + x.Name });  
            return Json(new { items = dataList }, JsonRequestBehavior.AllowGet);
        }

 

        public ActionResult CMSkuNumberGet(string Search, int PurchaseOrderID)
        {
            var PurchaseSKUList = db.PurchaseSKU.Where(x => x.IsEnable && x.PurchaseOrder.IsEnable && x.PurchaseOrderID == PurchaseOrderID && x.SerialsLlist.Sum(y => y.SerialsQTY) > 0).Select(x => x.SkuNo);
            var dataList = db.SkuLang.Where(x => x.LangID == LangID && PurchaseSKUList.Contains(x.Sku) && (x.Sku.Contains(Search) || x.Name.Contains(Search))).Take(20).Select(x => new SelectItem { id = x.Sku, text = x.Sku + "_" + x.Name });
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
                    Url = x.SKU.SkuPicture.Where(y => y.PictureType == "Logistic").FirstOrDefault()?.FileName,
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
            foreach (var item in PurchaseSKU)
            {
                var ss = item.SKU;
            }
            try
            {
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
            catch (Exception ex)
            {

                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult TSkuNumberList(int? draw, int? start, int? length, List<string> Skulist, int? FromWID, string SID, int? ToWID)
        {
            var ErrMsg = "";
            var odataList = (List<TranSKUVM>)Session["TSkuNumberList" + SID];
            if (odataList == null)
            {
                odataList = new List<TranSKUVM>();
            }
            if (FromWID.HasValue)
            {
                if (Skulist != null)
                {
                    foreach (var item in odataList.Where(x => Skulist.Contains(x.SKU)))
                    {
                        item.QTY++;
                        item.Model = "E";
                        Skulist.Remove(item.SKU);
                    }
                }
                if (Skulist != null && Skulist.Where(x => !string.IsNullOrWhiteSpace(x)).Any())
                {
                    //從PO單取SKU
                    var dataList = new List<TranSKUVM>();
                    dataList.AddRange(db.PurchaseSKU.Where(x => x.IsEnable && x.PurchaseOrder.IsEnable && x.PurchaseOrder.WarehouseID == FromWID && Skulist.Contains(x.SkuNo)).Select(x =>
                      new TranSKUVM
                      {
 
                          sk = x.SkuNo,
                          SKU = x.SkuNo,
                          ProductName = x.SKU.SkuLang.Where(y=>y.LangID== LangID).FirstOrDefault().Name,
                          QTY = 1,
                          Model = "E"
                      }).Distinct().ToList());
                    //從移倉單選取SKU
                    if (!dataList.Any())
                    {
                        dataList.AddRange(db.TransferSKU.Where(x => x.IsEnable && x.Transfer.IsEnable && x.Transfer.ToWID == FromWID && Skulist.Contains(x.SkuNo)).Select(x =>
                        new TranSKUVM
                        {
       
                            sk = x.SkuNo,
                            SKU = x.SkuNo,
                            ProductName = x.SKU.SkuLang.Where(y => y.LangID == LangID).FirstOrDefault().Name,
                            QTY = 1,
                            Model = "E"
                        }).Distinct().ToList());
                    }
                    //從RMA單選取SKU
                    if (!dataList.Any())
                    {
                        dataList.AddRange(db.RMASerialsLlist.Where(x => x.RMASKU.IsEnable && x.RMASKU.RMA.IsEnable && x.WarehouseID == FromWID && Skulist.Contains(x.RMASKU.SkuNo)).Select(x =>
                        new TranSKUVM
                        {
            
                            sk = x.RMASKU.SkuNo,
                            SKU = x.RMASKU.SkuNo,
                            ProductName = x.RMASKU.SKU.SkuLang.Where(y => y.LangID == LangID).FirstOrDefault().Name,
                            QTY = 1,
                            Model = "E"
                        }).Distinct().ToList());
                    }
                    if (!dataList.Any())
                    {
                        dataList.AddRange(db.RMASerialsLlist.Where(x => x.RMASKU.IsEnable && x.RMASKU.RMA.IsEnable && x.WarehouseID == FromWID && Skulist.Contains(x.NewSkuNo)).Select(x =>
                         new TranSKUVM
                         {
                  
                             sk = x.NewSkuNo,
                             SKU = x.NewSkuNo,
                             ProductName = x.RMASKU.SKU.SkuLang.Where(y => y.LangID == LangID).FirstOrDefault().Name,
                             QTY = 1,
                             Model = "E"
                         }).Distinct().ToList());
                    }

                    odataList.AddRange(dataList);
                }
                var index =0;
                if (ToWID.HasValue)//檢查Winit SKU
                {
                    var Warehouse = db.Warehouse.Find(ToWID);
                    if (Warehouse.Type== "Winit")
                    {
                        foreach (var item in odataList.ToList())
                        {
                            var WSKUList = new NewApi.Winit_API().SKUList(item.SKU + "-" + Warehouse.WinitWarehouse);
                            if (!WSKUList.list.Any())
                            {
                                odataList.Remove(item);
                                ErrMsg += "Winit無此SKU：" + item.SKU + "-" + Warehouse.WinitWarehouse + Environment.NewLine;
                            }
                           
                        }
                    }
                }
                foreach (var item in odataList)
                {
                    item.ck= index++;
                }
                Session["TSkuNumberList" + SID] = odataList;
                odataList = odataList.Where(x => x.Model != "D").ToList();
            }
            int recordsTotal = odataList.Count();
            var returnObj =
            new
            {
                ErrMsg,
                draw,
                recordsTotal,
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
        public ActionResult SkuNumberList(int? draw, int? start, int? length, string SID, int? VendorID, string[] Skulist)
        {
            var ErrMsg = "";

            var odataList = (List<PoSKUVM>)Session["SkuNumberList" + SID];
            if (odataList == null)
            {
                odataList = new List<PoSKUVM>();
            }
            if (Skulist != null && Skulist.Where(x => !string.IsNullOrWhiteSpace(x)).Any())
            {
                var dataList = db.SkuLang.Where(x => x.LangID == LangID && Skulist.Contains(x.Sku)).AsEnumerable().Select(x =>
                new PoSKUVM
                {
                    ck = x.Sku,
                    sk = x.Sku,
                    SKU = x.Sku,
                    Name = GetNameSize(x),
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
            }
            if (VendorID.HasValue)
            {
                var SkuIDlist = db.SKU.Where(x => x.GetBrand.VendorLIst.Where(y => y.ID == VendorID).Any()).Select(x => x.SkuID).ToList();//檢查供應商是否可以用SKU;
                var NotAddSKU = odataList.Where(x => !SkuIDlist.Contains(x.SKU)).Select(x => x.SKU).ToList();//不能用的SKU
                if (NotAddSKU.Any())
                {
                    var BrandNameList = db.SKU.Where(x => NotAddSKU.Contains(x.SkuID)).Select(x => x.GetBrand.Name);
                    ErrMsg = string.Format("This supplier isn't approved for these brands: {0}. Choose an approved supplier instead.", string.Join(",", BrandNameList));
                }
                odataList = odataList.Where(x => SkuIDlist.Contains(x.SKU)).ToList();
            }
            Session["SkuNumberList" + SID] = odataList;
            odataList = odataList.Where(x => x.Model != "D").ToList();
            int recordsTotal = odataList.Count();
            var returnObj =
            new
            {
                ErrMsg = ErrMsg,
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
        public ActionResult TSkuNumberListQTY(int id, int val, string SID)
        {
            var odataList = (List<TranSKUVM>)Session["TSkuNumberList" + SID];
            foreach (var item in odataList.Where(x => x.ID == id|| x.ck == id))
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
                var PurchaseSKUs = db.PurchaseSKU.Where(x => x.SkuNo == Serialsitem.SkuNo && x.PurchaseOrder.IsEnable && x.IsEnable && x.SKU.SerialTracking);//無開序號管理才能任意取
                PurchaseSKUs = PurchaseSKUs.Where(x => x.SerialsLlist.Where(y => y.SerialsQTY > 0 && (y.SerialsType == "PO" || y.SerialsType == "TransferIn"|| y.SerialsType == "DropshpOrderIn") && !y.SerialsLlistC.Any()).Any());
                if (SerialsLlist.Any())
                {
                    SerialsLlist = SerialsLlist.Where(x => x.SerialsQTY > 0 && (x.SerialsType == "PO" || x.SerialsType == "TransferIn" || x.SerialsType == "DropshpOrderIn") && !x.SerialsLlistC.Any());//PO和TransferIn才能出貨
                    if (SerialsLlist.Sum(x => x.SerialsQTY) > 0)
                    {
                        var nSerials = new SerialsLlist
                        {
                            IsEnable = true,
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
                else if (PurchaseSKUs.Any())
                {
                    var PurchaseSKU = PurchaseSKUs.FirstOrDefault();
                    var PSerialsLlist = PurchaseSKU.SerialsLlist.Where(x => x.SerialsQTY > 0 && (x.SerialsType == "PO" || x.SerialsType == "TransferIn") && !x.SerialsLlistC.Any()).FirstOrDefault();//PO和TransferIn才能出貨
                    if (PSerialsLlist != null)
                    {
                        var nSerials = new SerialsLlist
                        {
                            IsEnable = true,
                            OrderID = Serialsitem.OrderID,
                            PurchaseSKUID = PurchaseSKUs.FirstOrDefault().ID,
                            PID = PSerialsLlist.ID,
                            SerialsNo = PSerialsLlist.SerialsNo,
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
                        NoDataList.Add(Serialsitem.OrderID + "：(SKU：" + Serialsitem.SkuNo + ";Serial：無序號產品)");
                    }
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
                GetImgVM.MaxCount = 3;
                var PurchaseSKU = db.PurchaseSKU.Find(id);
                //俢改2019/03/11 SKYPE，改成品號內的圖檔
                foreach (var item in PurchaseSKU.SKU.SkuPicture.Where(x=> x.PictureType == "Logistic"))
                {
                    GetImgVM.imglist.Add("../../Uploads/" + item.FileName);
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
                            //2019/05/29  SKYPE，修改成3張圖
                            var Url = SaveImg(file, "~/Uploads/Sku/" + PurchaseSKU.SkuNo);
                            Url = Url.Replace("/Uploads/", "");
                            PurchaseSKU.SKU.SkuPicture.Add(new SkuPicture
                            {
                                IsMain = false,
                                PictureType = "Logistic",
                                FileSize = file.ContentLength,
                                FileName = Url,
                                CreateBy = UserBy,
                                CreateAt = DateTime.UtcNow,
                                UpdateAt = DateTime.UtcNow,
                                UpdateBy = UserBy
                            });

                            #region 停用
                            ////俢改2019/03/11 SKYPE，改成品號內的圖檔
                            //var Url = SaveImg(file, "~/Uploads/Sku/" + PurchaseSKU.SkuNo);
                            //Url = Url.Replace("/Uploads/", "");
                            //if (PurchaseSKU.SKU.SkuPicture == null || MaxCount > PurchaseSKU.SKU.SkuPicture.Count())
                            //{
                            //    //PurchaseSKU.SKU.SkuPicture
                            //    PurchaseSKU.SKU.SkuPicture.Add(new SkuPicture
                            //    {
                            //        IsMain = false,
                            //        PictureType = "Logistic",
                            //         FileSize= file.ContentLength,
                            //        FileName = Url,
                            //        CreateBy = UserBy,
                            //        CreateAt = DateTime.UtcNow,
                            //        UpdateAt = DateTime.UtcNow,
                            //        UpdateBy = UserBy
                            //    });
                            //}
                            //else
                            //{
                            //    var SkuPicture = PurchaseSKU.SKU.SkuPicture.OrderBy(x => x.UpdateAt).FirstOrDefault();
                            //    SkuPicture.FileName = Url;
                            //    SkuPicture.FileSize = file.ContentLength;
                            //    SkuPicture.UpdateAt = DateTime.UtcNow;
                            //    SkuPicture.UpdateBy = UserBy;
                            //}
                            //var Url = SaveImg(file);
                            //PurchaseSKU.ImgFile.Add(new ImgFile
                            //{
                            //    IsEnable = true,
                            //    ImgType = ImgType,
                            //    Url = Url,
                            //    CreateBy = UserBy,
                            //    CreateAt = DateTime.UtcNow
                            //});
                            #endregion
                        }
                    }
                    db.SaveChanges();
                    //只留3張，剩下的刪除
                    var saveID = PurchaseSKU.SKU.SkuPicture.Where(x => x.PictureType == "Logistic").OrderByDescending(x => x.CreateAt).Take(3).Select(x => x.ID);
                    foreach (var item in PurchaseSKU.SKU.SkuPicture.Where(x => !saveID.Contains(x.ID) && x.PictureType == "Logistic").ToList())
                    {
                        if (DeleteImg("/Uploads/" + item.FileName))
                        {
                            try
                            {
                                db.SkuPicture.Remove(item);
                                db.SaveChanges();
                            }
                            catch (Exception ex)
                            {

                                throw;
                            }

                        }
                    }
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
                        //檢查SC上的資料並寫入
                        AddSerialListToSC(PurchaseSKU, ExcelSerialslist);
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
                        if (!string.IsNullOrWhiteSpace(SKU))
                        {
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
                            if (!SKU.SerialTracking && item.QTYOrdered > 0)//有開序號管理
                            {
                                return Json(new { status = false, Msg = "SKU：" + item.Key.SKU + "沒開啟序號管理. 請至操作頁面開啟." }, JsonRequestBehavior.AllowGet);
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
                            //檢查SC上的資料並寫入
                            CreatSCPObyExcel(PurchaseOrder);
                          
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
            else if (key == "TransferExcel")//SKU及序號上傳
            {
                var dt = DateTime.UtcNow;
                var AddSKUserialsVMList = new List<AddSKUserialsVM>();
                using (var package = new ExcelPackage(ExcelFile.InputStream))
                {

                    ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                    int col = 1;

                    for (int row = 2; worksheet.Cells[row, col].Value != null; row++)
                    {
                        var SKU = worksheet.Cells[row, 1].Value?.ToString();
                        var serials = worksheet.Cells[row, 2].Value?.ToString();

                        AddSKUserialsVMList.Add(new AddSKUserialsVM
                        {
                            serials = serials,
                            SKU = SKU
                        });
                    }
                }
                var groupserials = AddSKUserialsVMList.GroupBy(x => x.SKU).Select(x => new { x.Key, item = x });
                if (groupserials.Any())
                {
                    var Transfer = db.Transfer.Find(id);
                    var FromWID = Transfer.FromWID;
                    if (FromWID.HasValue)
                    {
                        var ErrSKU = new List<string>();
                        var Errserial = new List<string>();
                        var Repserial = new List<string>();
                        var Noserial = new List<string>();
                        foreach (var Gserialitem in groupserials)
                        {
                            var SKUList = new List<string>();
                            SKUList = SearchSkuByWarehouse(Gserialitem.Key, FromWID.Value);
                            if (SKUList.Any())
                            {
                                var SKU = db.SKU.Find(Gserialitem.Key)?.SkuLang.Where(x => x.LangID == LangID).FirstOrDefault();
                                var nTransferSKU = Transfer.TransferSKU.Where(x=>x.SkuNo== Gserialitem.Key).FirstOrDefault();
                                if (nTransferSKU == null)//沒有資料就新增
                                {
                                    nTransferSKU = new TransferSKU
                                    {
                                        IsEnable = true,
                                        QTY = Gserialitem.item.Count(),
                                        SkuNo = Gserialitem.Key,
                                        Name = SKU.Name,
                                        CreateBy = UserBy,
                                        CreateAt = dt
                                    };
                                    Transfer.TransferSKU.Add(nTransferSKU);
                                }
                                else
                                {
                                    nTransferSKU.QTY = Gserialitem.item.Count();
                                    nTransferSKU.UpdateBy = UserBy;
                                    nTransferSKU.UpdateAt = dt;
                                }
                                var SerialTracking = SKU.GetSku.SerialTracking;
                                foreach (var item in Gserialitem.item)
                                {
                                    var SerialsLlist = db.SerialsLlist.Where(x => !x.SerialsLlistC.Any() && x.SerialsQTY > 0);
                                    if (SerialTracking)
                                    {
                                        SerialsLlist= SerialsLlist.Where(x => x.SerialsNo == item.serials);//找到序號
                                    }
                                    else
                                    {
                                        SerialsLlist = SerialsLlist.Where(x => x.PurchaseSKU.SkuNo == SKU.GetSku.SkuID);//找到序號
                                    }
                                    SerialsLlist = SerialsLlist.Where(x => (x.TransferSKUID.HasValue && x.TransferSKU.Transfer.ToWID == FromWID) || (x.PurchaseSKUID.HasValue && x.PurchaseSKU.PurchaseOrder.WarehouseID == FromWID)).Take(1);
                                    var RMASerialsLlist = db.RMASerialsLlist.Where(x => x.SerialsNo == item.serials && !x.RMASerialsLlistC.Any() && x.SerialsQTY > 0 && x.WarehouseID == FromWID);//找到RMA序號
                                    if (SerialsLlist.Any() || RMASerialsLlist.Any())
                                    {
                                        foreach (var Serial in SerialsLlist)
                                        {
                                            if (nTransferSKU.SerialsLlist != null && nTransferSKU.SerialsLlist.Where(x => x.SerialsNo == Serial.SerialsNo).Any())
                                            {
                                                Repserial.Add(Serial.SerialsNo);
                                            }
                                            else
                                            {
                                                var nSerialsLlist = new SerialsLlist
                                                {
                                                    IsEnable = true,
                                                    PurchaseSKUID = Serial.PurchaseSKUID,
                                                    PID = Serial.ID,
                                                    SerialsNo = Serial.SerialsNo,
                                                    SerialsQTY = -1,
                                                    SerialsType = "TransferOut",
                                                    CreateBy = UserBy,
                                                    CreateAt = dt,
                                                    ReceivedBy = UserBy,
                                                    ReceivedAt = dt,
                                                };
                                                nTransferSKU.SerialsLlist.Add(nSerialsLlist);
                                            }

                                        }
                                        foreach (var Serial in RMASerialsLlist)
                                        {
                                            if (nTransferSKU.SerialsLlist != null && nTransferSKU.SerialsLlist.Where(x => x.SerialsNo == Serial.SerialsNo).Any())
                                            {
                                                Repserial.Add(Serial.SerialsNo);
                                            }
                                            else
                                            {
                                                var nSerialsLlist = new SerialsLlist
                                                {
                                                    IsEnable = true,
                                                    SerialsNo = Serial.SerialsNo,
                                                    SerialsQTY = 1,
                                                    SerialsType = "TransferOut",
                                                    CreateBy = UserBy,
                                                    CreateAt = dt,
                                                    ReceivedBy = UserBy,
                                                    ReceivedAt = dt,
                                                };
                                                nTransferSKU.SerialsLlist.Add(nSerialsLlist);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (SerialTracking)
                                        {
                                            Errserial.Add(item.serials);
                                        }
                                        else
                                        {
                                            Noserial.Add(item.SKU);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                ErrSKU.Add(Gserialitem.Key);
                            }
                        }
                        if (Repserial.Any())
                        {
                            return Json(new { status = false, Msg = string.Join(Environment.NewLine, Repserial.Distinct()) + Environment.NewLine + "序號重複" }, JsonRequestBehavior.AllowGet);
                        }
                        if (ErrSKU.Any())
                        {
                            return Json(new { status = false, Msg = string.Join(Environment.NewLine, ErrSKU.Distinct()) + Environment.NewLine + "出貨倉無此SKU" }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            if (Errserial.Any() || Noserial.Any())
                            {
                                string Emsg = "";
                                if (Errserial.Any())
                                {
                                    Emsg = Emsg + string.Join(Environment.NewLine, Errserial.Distinct()) + Environment.NewLine + "出貨倉無此序號";
                                }
                                if (Noserial.Any())
                                {
                                    Emsg = Emsg + "此SKU" + Environment.NewLine + string.Join(Environment.NewLine, Noserial.Distinct()) + Environment.NewLine + "無可移倉的序號";
                                }
                                return Json(new
                                {
                                    status = false,
                                    Msg = Emsg
                                }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                Transfer.UpdateBy = UserBy;
                                Transfer.UpdateAt = dt;
                                Transfer.Status = "Requested";
                                db.SaveChanges();
                            }
                        }
                    }
                    else
                    {
                        return Json(new { status = false, Msg = "移倉單未設定出貨倉" }, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(new { status = true, reload = true }, JsonRequestBehavior.AllowGet);
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
                    ImagePath = s.SkuPicture.Where(x => x.PictureType == "Logistic").Select(x => x.FileName)
                }).ToList();
            }
            catch (Exception e)
            {
                result.status = false;
                result.message = e.InnerException != null && !string.IsNullOrEmpty(e.InnerException.Message) ? e.InnerException.Message : e.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
       // [HttpPost]
        public ActionResult CreateALLRMA(int OrderID, int ReturnWarehouseID)
        {
            AjaxResult result = new AjaxResult();
            var Reason = 16;
            var dt = DateTime.UtcNow;
            var SerialsLlist = db.SerialsLlist.Where(x => x.OrderID == OrderID && x.SerialsType == "Order");
            var WarehouseID = db.WarehouseSummary.Where(x => x.Type == "SCID" && x.Val == ReturnWarehouseID.ToString()).FirstOrDefault()?.WarehouseID;
            if (WarehouseID.HasValue)
            {
                try
                {
                    //建立SC上的RMA資料
                    SC_WebService SCWS = new SC_WebService(ApiUserName, ApiPassword);
                    var order = SCWS.Get_OrderData(OrderID).Order;//去SC抓訂單資料
                    var SCRMA = SCWS.Get_RMA_by_OrderID(OrderID);//檢查SC上是否有開過RMA
                    if (UpdateSC)
                    {
                        order.OrderCreationSourceApplication = SCService.OrderCreationSourceApplicationType.PointOfSale;
                    }
                 
                    if (!UpdateSC || SCWS.Update_Order(order))
                    {
                        int RMAId = 0;
                        if (SCRMA == null)
                        {
                            if (UpdateSC)
                            {
                                RMAId = SCWS.Create_RMA(order.ID);//建立RMAID
                            }
                        }
                        else
                        {
                            RMAId = SCRMA.ID;
                        }
                        //建立RMA資料
                        var newRMA = new RMA
                        {
                            IsEnable = true,
                            OrderID = OrderID,
                            SourceID = order.OrderSourceOrderId,
                            CompanyID = order.CompanyId,
                            Country = GetCountryCode(order.ShippingAddress.CountryCode),
                            Status = "Received",
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
                        if (order.Items == null || order.Items.Count() == 0)
                        {
                            result.SetError("Items無資料");
                        }
                        foreach (var item in order.Items)
                        {
                            var SKU = db.SKU.Find(item.ProductID)?.SkuLang.Where(x => x.LangID == LangID).FirstOrDefault();
                            if (SKU == null)
                            {
                                result.SetError(item.ProductID + "：無此SKU資料");
                                return Json(result, JsonRequestBehavior.AllowGet);
                            }
                            var SKUSerialsLlist = SerialsLlist.Where(x => x.PurchaseSKU.SkuNo == item.ProductID).Select(x => x.SerialsNo);
                            string serialsList = string.Join(",", SKUSerialsLlist);

                            int RMAItemID = 0;
                            var OrderItemID = order.Items.Where(x => x.ProductID == item.ProductID).FirstOrDefault().ID;
                            if (UpdateSC)
                            {
                                if (SCRMA == null)
                                {
                                    RMAItemID = SCWS.Create_RMA_Item(item.OrderID, item.ID, RMAId, item.Qty, Reason, "");//建立每個SKU要退貨的數量原因，並取回ID
                                    SCWS.Receive_RMA_Item(RMAId, RMAItemID, item.ProductID, item.Qty, ReturnWarehouseID, serialsList);//RMA入庫
                                }
                                else
                                {
                                    var SCRMA_Item = SCWS.Get_RMA_Item(OrderID)?.Where(x => x.OriginalOrderItemID == OrderItemID).FirstOrDefault();
                                    if (SCRMA_Item == null)//沒資料就新增
                                    {
                                        RMAItemID = SCWS.Create_RMA_Item(item.OrderID, item.ID, RMAId, item.Qty, Reason, "");//建立每個SKU要退貨的數量原因，並取回ID
                                        SCWS.Receive_RMA_Item(RMAId, RMAItemID, item.ProductID, item.Qty, ReturnWarehouseID, serialsList);//RMA入庫
                                    }
                                    else
                                    {
                                        //有資料直接取值
                                        RMAItemID = SCRMA_Item.ID;
                                        if (SCRMA_Item.QtyReturned > SCRMA_Item.QtyReceived)//比對數量
                                        {
                                            SCWS.Receive_RMA_Item(RMAId, RMAItemID, item.ProductID, item.Qty, ReturnWarehouseID, serialsList);//RMA入庫
                                        }
                                    }
                                }
                                //SCWS.Delete_ItemSerials(item.OrderID, item.ID);//SC上的序號移除
                            }
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
                                    RMAItemID = RMAItemID,
                                    CreateBy = "RMAAPI",
                                    CreateAt = dt,
                                };
                                newRMASKU.RMASerialsLlist.Add(new RMASerialsLlist
                                {
                                    IsEnable = true,
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
                                newRMASKU.RMAOrderSerialsLlist.Add(new RMAOrderSerialsLlist
                                {
                                    IsEnable = true,
                                    SerialsNo = Serialitem,
                                    SerialsQTY = 1,
                                    CreateBy = "RMAAPI",
                                    CreateAt = dt,
                                });
                                newRMA.RMASKU.Add(newRMASKU);
                            }
                        }
                        if (UpdateSC)
                        {
                            order.OrderCreationSourceApplication = OrderCreationSourceApplicationType.Default;
                            SCWS.Update_Order(order);
                        }
                        result.data = RMAId;
                        db.RMA.Add(newRMA);
                        db.SaveChanges();
                        //開移倉單
                        var nTransfer = new Transfer
                        {
                            IsEnable = true,
                            Title = OrderID + "_Cancel ",
                            FromWID = WarehouseID,
                            ToWID = WarehouseID,
                            Status = "Completed",
                            Interim = 2,
                            CreateBy = "RMAAPI",
                            CreateAt = dt
                        };
                        foreach (var RMASKUitem in newRMA.RMASKU)
                        {
                            var nTransferSKU = new TransferSKU
                            {
                                IsEnable = true,
                                QTY = 1,
                                SkuNo = RMASKUitem.SkuNo,
                                Name = RMASKUitem.Name,
                                CreateBy = "RMAAPI",
                                CreateAt = dt
                            };

                            foreach (var Serialitem in RMASKUitem.RMASerialsLlist)
                            {
                                var nSerialsLlist = new SerialsLlist
                                {
                                    IsEnable = true,
                                    SerialsNo= Serialitem.SerialsNo,
                                    SerialsQTY=1,
                                    SerialsType= "TransferIn",
                                    CreateBy = "RMAAPI",
                                    CreateAt = dt,
                                    ReceivedBy = "RMAAPI",
                                    ReceivedAt = dt,
                                };
                                nTransferSKU.SerialsLlist.Add(nSerialsLlist);
                                var nRMASerialsLlist = new RMASerialsLlist
                                {
                                    IsEnable = true,
                                    RMASKUID= RMASKUitem.ID,
                                    PID= Serialitem.ID,
                                    SerialsNo = Serialitem.SerialsNo,
                                    SerialsQTY = 1,
                                    SerialsType = "TransferOut",
                                    CreateBy = "RMAAPI",
                                    CreateAt = dt,
                                    ReceivedBy = "RMAAPI",
                                    ReceivedAt = dt,
                                };
                                nTransferSKU.RMASerialsLlist.Add(nRMASerialsLlist);
                            }
                            nTransfer.TransferSKU.Add(nTransferSKU);
                        }
                        db.Transfer.Add(nTransfer);
                        db.SaveChanges();
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
        public ActionResult CreateDropShipPO(DropshpOrderBySC DropshpOrderBySC)
        {
            SCWS = new SC_WebService(ApiUserName, ApiPassword);
            AjaxResult result = new AjaxResult();
            try
            {
                if (DropshpOrderBySC.PuchaseID == 0)//沒有值就新增
                {
                    var CreateAt = DateTime.UtcNow;
                    var CompanyID = db.Company.Where(x => x.CompanySCID == DropshpOrderBySC.CompanyID).FirstOrDefault()?.ID;
                    if (!CompanyID.HasValue)
                    {
                        result.SetError("SCID" + DropshpOrderBySC.CompanyID + "沒有對應的公司");
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    var Vendor = db.VendorLIst.Where(x => x.SCID == DropshpOrderBySC.VendorID).FirstOrDefault();
                    if (Vendor == null)
                    {
                        result.SetError("SCID" + DropshpOrderBySC.VendorID + "沒有對應的供應商");
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    var WarehouseID = db.WarehouseSummary.Where(x => x.Type == "SCID" && x.Val == DropshpOrderBySC.DefaultWarehouseID.ToString()).FirstOrDefault()?.WarehouseID;
                    if (!WarehouseID.HasValue)
                    {
                        result.SetError("SCID" + DropshpOrderBySC.DefaultWarehouseID + "沒有對應的倉庫");
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }


                    var nPurchaseOrder = new Models.PurchaseOrder
                    {
                        IsEnable = true,
                        Tracking = DropshpOrderBySC.TrackingNumber,
                        WarehouseID = WarehouseID,
                        CompanyID = CompanyID,
                        VendorID = Vendor.ID,
                        PODate = DateTime.Today,
                        POStatus = "Opened",
                        POType = "DropshpOrder",
                        Currency = Vendor.Currency,
                        Tax = Vendor.Tax,
                        Description = DropshpOrderBySC.PurchaseTitle,
                        InvoiceNo = DropshpOrderBySC.Invoice,
                        CreateBy = UserBy,
                        CreateAt = CreateAt
                    };
                    if (DropshpOrderBySC.Items != null)
                    {
                        foreach (var item in DropshpOrderBySC.Items)
                        {
                            var SkuLang = db.SKU.Find(item.Sku).SkuLang.Where(x => x.LangID == LangID).FirstOrDefault();
                            var nPurchaseSKU = new PurchaseSKU
                            {
                                IsEnable = true,
                                Name = SkuLang.Name,
                                SkuNo = item.Sku,
                                VendorSKU = item.Sku,
                                QTYOrdered = item.Qty,
                                CreateBy = UserBy,
                                CreateAt = CreateAt
                            };
                            nPurchaseOrder.PurchaseSKU.Add(nPurchaseSKU);
                            if (item.SerialNumber != null)
                            {
                                foreach (var SerialNumberitem in item.SerialNumber)
                                {
                                    var dt = DateTime.UtcNow;
                                    var nSerialsLlistIn = new SerialsLlist
                                    {
                                        IsEnable = true,
                                        SerialsType = "DropshpOrderIn",
                                        SerialsNo = SerialNumberitem,
                                        SerialsQTY = 1,
                                        ReceivedBy = UserBy,
                                        ReceivedAt = dt,
                                        CreateBy = UserBy,
                                        CreateAt = dt
                                    };
                                    nPurchaseSKU.SerialsLlist.Add(nSerialsLlistIn);
                                    //var nSerialsLlistOut = new SerialsLlist
                                    //{
                                    //    IsEnable = true,
                                    //    PurchaseSKUID = nPurchaseSKU.ID,
                                    //    SerialsType = "DropshpOrderOut",
                                    //    SerialsNo = SerialNumberitem,
                                    //    SerialsQTY = -1,
                                    //    ReceivedBy = UserBy,
                                    //    ReceivedAt = dt,
                                    //    CreateBy = UserBy,
                                    //    CreateAt = dt
                                    //};
                                    //nSerialsLlistIn.SerialsLlistC.Add(nSerialsLlistOut);
                                }
                            }
                        }
                    }
                    db.PurchaseOrder.Add(nPurchaseOrder);
                    db.SaveChanges();
                    var SCPurchase = CreatPObySC(nPurchaseOrder);
                    result.data = SCPurchase.ID;
                    nPurchaseOrder.SCPurchaseID = SCPurchase.ID;
                    db.SaveChanges();
                    CreatAndEditPOSKUbySC(nPurchaseOrder);
                    foreach (var PurchaseSKU in nPurchaseOrder.PurchaseSKU)
                    {
                        foreach (var serial in PurchaseSKU.SerialsLlist.Where(x => x.SerialsType == "DropshpOrderIn"))
                        {
                            AddSerialToSC(PurchaseSKU, serial.SerialsNo);
                        }
                    }

                }
                else//有值就更新
                {
                    var oPurchaseOrder = db.PurchaseOrder.Where(x => x.SCPurchaseID == DropshpOrderBySC.PuchaseID).FirstOrDefault();
                    if (oPurchaseOrder != null)
                    {
                        oPurchaseOrder.InvoiceNo = DropshpOrderBySC.Invoice;
                        foreach (var item in DropshpOrderBySC.Items)
                        {
                            var oPurchaseSKU = oPurchaseOrder.PurchaseSKU.Where(x => x.SkuNo == item.Sku).FirstOrDefault();
                            if (oPurchaseSKU != null && item.SerialNumber != null)
                            {
                                foreach (var SerialNumberitem in item.SerialNumber)
                                {
                                    var dt = DateTime.UtcNow;
                                    var nSerialsLlistIn = new SerialsLlist
                                    {
                                        IsEnable = true,
                                        SerialsType = "DropshpOrderIn",
                                        SerialsNo = SerialNumberitem,
                                        SerialsQTY = 1,
                                        ReceivedBy = UserBy,
                                        ReceivedAt = dt,
                                        CreateBy = UserBy,
                                        CreateAt = dt
                                    };
                                    oPurchaseSKU.SerialsLlist.Add(nSerialsLlistIn);
                                    //var nSerialsLlistOut = new SerialsLlist
                                    //{
                                    //    IsEnable = true,
                                    //    PurchaseSKUID = oPurchaseSKU.ID,
                                    //    SerialsType = "DropshpOrderOut",
                                    //    SerialsNo = SerialNumberitem,
                                    //    SerialsQTY = -1,
                                    //    ReceivedBy = UserBy,
                                    //    ReceivedAt = dt,
                                    //    CreateBy = UserBy,
                                    //    CreateAt = dt
                                    //};
                                    //nSerialsLlistIn.SerialsLlistC.Add(nSerialsLlistOut);
                                }
                            }
                        }
                        db.SaveChanges();
                        foreach (var PurchaseSKU in oPurchaseOrder.PurchaseSKU)
                        {
                            foreach (var serial in PurchaseSKU.SerialsLlist.Where(x => x.SerialsType == "DropshpOrderIn"))
                            {
                                AddSerialToSC(PurchaseSKU, serial.SerialsNo);
                            }
                        }
                        result.data = oPurchaseOrder.SCPurchaseID;
                    }
                }
            }
            catch (Exception ex)
            {
                result.SetError(ex.ToString());
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }
    public class DropshpOrderBySC
    {
        public int PuchaseID { get; set; }
        public int CompanyID { get; set; }
        public int VendorID { get; set; }
        public string PurchaseTitle { get; set; }
        public string Invoice { get; set; }
        public int DefaultWarehouseID { get; set; }
        public string TrackingNumber { get; set; }
        public string Memo { get; set; }
        public DropshpOrderBySCItem[] Items { get; set; }
    }
    public class DropshpOrderBySCItem
    {
        public string Sku { get; set; }
        public int Qty { get; set; }
        public string[] SerialNumber { get; set; }
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