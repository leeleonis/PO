using PurchaseOrderSys.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PurchaseOrderSys.Controllers
{
    public class PurchaseOrderCMController : BaseController
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
        public ActionResult Create(PurchaseOrderPOVM filter)
        {
            var PurchaseOrder = new PurchaseOrder
            {
                IsEnable = true,
                CompanyID = filter.CompanyID,
                VendorID = filter.VendorID,
                PODate = filter.PODate,
                POStatus = filter.POStatus,
                POType = filter.POType,
                CreateBy = UserBy,
                CreateAt = DateTime.UtcNow
            };
            db.PurchaseOrder.Add(PurchaseOrder);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult GetData(PurchaseOrderPOVM filter, string Type, int? DetailID)
        {
            if (Type == "Master")
            {
                int total = 0;
                var listPurchaseOrder = db.PurchaseOrder.AsQueryable();
                if (filter.ID.HasValue)
                {
                    listPurchaseOrder = listPurchaseOrder.Where(x => x.ID == filter.ID);
                }
                if (!string.IsNullOrWhiteSpace(filter.POType))
                {
                    listPurchaseOrder = listPurchaseOrder.Where(x => x.POType == filter.POType);
                }
                if (filter.VendorID.HasValue)
                {
                    listPurchaseOrder = listPurchaseOrder.Where(x => x.VendorID == filter.VendorID);
                }
                if (filter.PODate.HasValue)
                {
                    listPurchaseOrder = listPurchaseOrder.Where(x => x.PODate == filter.PODate);
                }
                if (!string.IsNullOrWhiteSpace(filter.POStatus))
                {
                    listPurchaseOrder = listPurchaseOrder.Where(x => x.POStatus == filter.POStatus);
                }

                var dataList = listPurchaseOrder.ToList().Select(x => new
                {
                    x.ID,
                    x.POType,
                    VendorID = x.VendorID.ToString(),
                    PODate = x.PODate.Value.ToString("yyyy/MM/dd"),
                    QTY = x.PurchaseSKU.Sum(y => y.QTYOrdered),
                    GrandTotal = x.PurchaseSKU.Sum(y => (y.QTYOrdered * y.Price)),
                    x.PaidAmount,
                    Balance = x.PurchaseSKU.Sum(y => (y.QTYOrdered * y.Price)),
                    POStatus = x.POStatus
                });

                if (filter.QTY.HasValue)
                {
                    dataList = dataList.Where(x => x.QTY == filter.QTY);
                }
                if (filter.GrandTotal.HasValue)
                {
                    dataList = dataList.Where(x => x.GrandTotal == filter.GrandTotal);
                }
                if (filter.PaidAmount.HasValue)
                {
                    dataList = dataList.Where(x => x.PaidAmount == filter.PaidAmount);
                }
                if (filter.Balance.HasValue)
                {
                    dataList = dataList.Where(x => x.Balance == filter.Balance);
                }
                total = dataList.Count();
                return Json(new { total, rows = dataList }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var PurchaseSKU = db.PurchaseSKU.Where(x => x.PurchaseOrderID == DetailID);
                return PartialView("Detail", PurchaseSKU);
            }
        }

        public ActionResult GetSelectOption(string[] optionType)
        {
            Dictionary<string, object> optionList = new Dictionary<string, object>();

            foreach (string type in optionType)
            {
                switch (type)
                {
                    case "POTypeDDL":
                        optionList.Add(type, EnumData.POTypeDDL().Select(x => new { text = x.Value, value = x.Key }));
                        break;
                    case "VendorIDDDL":
                        optionList.Add(type, EnumData.VendorDDL().Select(x => new { text = x.Value, value = x.Key }));
                        break;
                    case "CompanyIDDDL":
                        optionList.Add(type, new { });
                        break;
                    case "POStatusDDL":
                        optionList.Add(type, EnumData.POStatusDDL().Select(x => new { text = x.Value, value = x.Key }));
                        break;
                }
            }
            return Json(new { status = true, data = optionList }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Update(PurchaseOrder PurchaseOrder)
        {
            string[] EditList = new string[] { "POType", "POStatus", "VendorID" };
            var oldPurchaseOrder = db.PurchaseOrder.Find(PurchaseOrder.ID);
            SetEditDatas(oldPurchaseOrder, PurchaseOrder, EditList);
            return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreateItem(int ID)
        {
            ViewBag.PurchaseOrderID = ID;
            return View();
        }
        [HttpPost]
        public ActionResult CreateItem(PurchaseSKU PurchaseSKU)
        {

            PurchaseSKU.IsEnable = true;
            PurchaseSKU.CreateBy = UserBy;
            PurchaseSKU.CreateAt = DateTime.UtcNow;
            db.PurchaseSKU.Add(PurchaseSKU);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}