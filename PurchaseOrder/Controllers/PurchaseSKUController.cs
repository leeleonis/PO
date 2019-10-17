using PurchaseOrderSys.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using inventorySKU;
namespace PurchaseOrderSys.Controllers
{
    [CheckSession]
    public class PurchaseSKUController : BaseController
    {
        // GET: PurchaseOrder
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(PurchaseSKUVM filter)
        {
            var PurchaseSKU = new PurchaseSKU
            {
            };
            db.PurchaseSKU.Add(PurchaseSKU);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult GetData(PurchaseSKUVM filter, string Type, int? DetailID)
        {
            if (Type == "Master")
            {
                int total = 0;
                var listPurchaseSKU = db.PurchaseSKU.AsQueryable();
                var dataList = listPurchaseSKU.ToList().Select(x => new PurchaseSKUVM
                {
                    Discount = x.Discount,
                    DiscountedPrice = x.DiscountedPrice,
                    ID = x.ID,
                    Name = x.Name,
                    Price = x.Price,
                    QTYOrdered = x.QTYOrdered,
                    QTYFulfilled = x.SerialsLlist.Any() ? x.SerialsLlist.Count(): x.QTYFulfilled,
                    SkuNo = x.SkuNo,
                    VendorSKU = x.VendorSKU
                });
                total = dataList.Count();
                return Json(new { total, rows = dataList }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var SerialsLlist = db.SerialsLlist.Where(x => x.PurchaseSKUID == DetailID);
                return PartialView("Detail", SerialsLlist);
            }
        }

        public ActionResult GetSelectOption(string[] optionType)
        {
            Dictionary<string, object> optionList = new Dictionary<string, object>();
            if (optionType != null)
            {
                foreach (string type in optionType)
                {
                    switch (type)
                    {
                        case "POTypeDDL":

                            break;
                        case "VendorIDDDL":

                            break;
                        case "CompanyIDDDL":
                            optionList.Add(type, new { });
                            break;
                        case "POStatusDDL":

                            break;
                    }
                }
            }
            return Json(new { status = true, data = optionList }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Update(PurchaseSKU PurchaseOrder)
        {
            string[] EditList = new string[] { "POType", "POStatus", "VendorID" };
            var oldPurchaseOrder = db.PurchaseOrder.Find(PurchaseOrder.ID);
            //SetEditDatas(oldPurchaseOrder, PurchaseOrder, EditList);
            return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CreateItem(int ID)
        {
            var PurchaseSKU = db.PurchaseSKU.Find(ID);
            ViewBag.PurchaseSKUID = ID;
            ViewBag.SKUNO = PurchaseSKU.SkuNo;
            ViewBag.SerialsLlist = PurchaseSKU.SerialsLlist.Select(x=>x.SerialsNo);
            return View();
        }
        [HttpPost]
        public ActionResult CreateItem(SerialsLlist SerialsLlist)
        {
            var SerialsLlists = db.SerialsLlist.Where(x => x.SerialsNo == SerialsLlist.SerialsNo);
            if (SerialsLlists.Any())
            {
                return Json(new { status = false, Msg= "Serials重複："+ SerialsLlist.SerialsNo }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                SerialsLlist.CreateBy =  Session["AdminName"].ToString();
                SerialsLlist.CreateAt = DateTime.UtcNow;
                db.SerialsLlist.Add(SerialsLlist);
                db.SaveChanges();
                return Json(new { status = true }, JsonRequestBehavior.AllowGet);
            }
          
        }
    }
}
