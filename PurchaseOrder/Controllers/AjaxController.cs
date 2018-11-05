using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using PurchaseOrderSys.Models;

namespace PurchaseOrderSys.Controllers
{
    public class AjaxController : BaseController
    {
        public ActionResult SkuNumberGet(string Search)
        {
            var dataList = db.SkuLang.Where(x => x.LangID == "zh-tw" && (x.Sku.Contains(Search) || x.Name.Contains(Search))).Take(20).Select(x => new SelectItem { id = x.Sku, text = x.Sku + "_" + x.Name });
            return Json(new { items = dataList }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetSkuNumberList(int? draw, int? start, int? length, int ID)
        {
            var odataList = db.PurchaseSKU.Where(x => x.PurchaseOrderID == ID).Select(x => new PoSKUVM
            {
                ID=x.ID,
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
        public ActionResult TSkuNumberList(int? draw, int? start, int? length, string[] Skulist)
        {
            var odataList = (List<TranSKUVM>)Session["TSkuNumberList"];
            if (odataList == null)
            {
                odataList = new List<TranSKUVM>();
            }
            if (Skulist != null && Skulist.Where(x => !string.IsNullOrWhiteSpace(x)).Any())
            {
                var dataList = db.SkuLang.Where(x => x.LangID == "zh-tw" && Skulist.Contains(x.Sku)).Select(x =>
                new TranSKUVM
                {
                    ck = x.Sku,
                    sk = x.Sku,
                    SKU = x.Sku,
                    ProductName = x.Name,

                }
                ).ToList();
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
                var dataList = db.SkuLang.Where(x => x.LangID == "zh-tw" && Skulist.Contains(x.Sku)).Select(x =>
                new PoSKUVM
                {
                    ck = x.Sku,
                    sk = x.Sku,
                    SKU = x.Sku,
                    Name = x.Name,
                    VendorSKU = "",

                }
                ).ToList();
                foreach (var item in dataList)
                {
                    var QTYOrdered = RandomVal(100, 1000);
                    var QTYFulfilled = RandomVal(100, 1000);
                    var Price = RandomVal(1000, 30000);
                    var Discount = RandomVal(0, 100);
                    var Credit = RandomVal(0, 100);
                    item.QTYOrdered = QTYOrdered;
                    item.QTYFulfilled = QTYFulfilled;
                    item.Price = Price;
                    item.Discount = Discount;
                    item.Credit = Credit;
                    item.DiscountedPrice = (Price - Discount);
                    item.Subtotal = (QTYOrdered * (Price - Discount));
                }
                odataList.AddRange(dataList);
                Session["SkuNumberList"] = odataList;
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
        [HttpPost]
        public ActionResult SkuNumberListEdit(string SKU,string type ,decimal oval, decimal val,int? ID)
        {
            var odataList = (List<PoSKUVM>)Session["SkuNumberList"];
            switch (type)
            {
                case "Price":
                    foreach (var item in odataList.Where(x => (x.ID== ID || x.SKU == SKU) && x.Price == oval))
                    {
                        item.Price = val;
                        item.DiscountedPrice = (item.Price - item.Discount);
                        item.Subtotal = (item.QTYOrdered * (item.Price - item.Discount));
                    }
                    break;
                case "Discount":
                    foreach (var item in odataList.Where(x => (x.ID == ID || x.SKU == SKU) && x.Discount == oval))
                    {
                        item.Discount = val;
                        item.DiscountedPrice = (item.Price - item.Discount);
                        item.Subtotal = (item.QTYOrdered * (item.Price - item.Discount));
                    }
                    break;
                case "Credit":
                    foreach (var item in odataList.Where(x => (x.ID == ID || x.SKU == SKU) && x.Credit == oval))
                    {
                        item.Credit = val;
                        item.DiscountedPrice = (item.Price - item.Discount);
                        item.Subtotal = (item.QTYOrdered * (item.Price - item.Discount));
                    }
                    break;
            }    
            Session["SkuNumberList"] = odataList;
            return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        }
    }
}