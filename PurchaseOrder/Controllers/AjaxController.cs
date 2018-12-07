﻿using System;
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
        public ActionResult TSkuNumberGet(string Search, int FromWID)
        {
            var PurchaseSKUList = db.PurchaseSKU.Where(x => x.IsEnable && x.PurchaseOrder.IsEnable && x.PurchaseOrder.WarehouseID == FromWID).Select(x => x.SkuNo);
            var dataList = db.SkuLang.Where(x => x.LangID == "zh-tw" && PurchaseSKUList.Contains(x.Sku) && (x.Sku.Contains(Search) || x.Name.Contains(Search))).Take(20).Select(x => new SelectItem { id = x.Sku, text = x.Sku + "_" + x.Name });
            //var dataList = db.SkuLang.Where(x => x.LangID == "zh-tw" && (x.Sku.Contains(Search) || x.Name.Contains(Search))).Take(20).Select(x => new SelectItem { id = x.Sku, text = x.Sku + "_" + x.Name });
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
        public ActionResult TSkuNumberList(int? draw, int? start, int? length, string[] Skulist,int FromWID)
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
                      QTY = 0
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
        public ActionResult CMSkuNumberList(int? draw, int? start, int? length, string[] Skulist)
        {
            var odataList = (List<CMSKUVM>)Session["CMSkuNumberList"];
            if (odataList == null)
            {
                odataList = new List<CMSKUVM>();
            }
            if (Skulist != null && Skulist.Where(x => !string.IsNullOrWhiteSpace(x)).Any())
            {
                var dataList = db.SkuLang.Where(x => x.LangID == "zh-tw" && Skulist.Contains(x.Sku)).Select(x =>
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
                var dataList = db.SkuLang.Where(x => x.LangID == "zh-tw" && Skulist.Contains(x.Sku)).Select(x =>
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
                    var Credit =0;
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
        public ActionResult SkuNumberListEdit(string SKU,string type ,decimal oval, decimal val,int? ID)
        {
            var odataList = (List<PoSKUVM>)Session["SkuNumberList"];
            if (odataList == null)
            {
                odataList = new List<PoSKUVM>();
            }
            foreach (var item in odataList.Where(x=>x.ID== ID || x.SKU== SKU))
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
               
                item.DiscountedPrice = (item.Price - item.Discount);
                item.Subtotal = (item.QTYOrdered * (item.Price - item.Discount));
            }
            Session["SkuNumberList"] = odataList;
            return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult TSkuNumberListQTY(string SKU,int val)
        {
            var odataList = (List<TranSKUVM>)Session["TSkuNumberList"];
            foreach (var item in odataList.Where(x=>x.SKU== SKU))
            {
                item.QTY = val;
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
                            optionList.Add(type, Enum.GetValues(typeof(EnumData.SkuStatus)).Cast<EnumData.SkuStatus>().Select(s => new { text = s.ToString(), value = Convert.ToBoolean((int)s).ToString() }));
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
    }
    internal class AjaxResult
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