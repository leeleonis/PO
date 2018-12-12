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
        public ActionResult Index(CreditMemoVM CreditMemoVM)
        {
            Session["CreditMemoVM"] = CreditMemoVM;
            return View(CreditMemoVM);
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
        public ActionResult GetData(CreditMemoVM filter, string Type, int? DetailID)
        {
            if (Type == "Master")
            {
                var QCreditMemoVM = (CreditMemoVM)Session["CreditMemoVM"];
                int total = 0;
                var listCreditMemo = db.CreditMemo.AsQueryable();
                //綱頁查詢
                if (QCreditMemoVM.CompanyID.HasValue)
                {
                    listCreditMemo = listCreditMemo.Where(x => x.CompanyID == QCreditMemoVM.CompanyID);
                }
                if (QCreditMemoVM.VendorID.HasValue)
                {
                    listCreditMemo = listCreditMemo.Where(x => x.VendorID == QCreditMemoVM.VendorID);
                }
                if (QCreditMemoVM.ID.HasValue)
                {
                    listCreditMemo = listCreditMemo.Where(x => x.ID == QCreditMemoVM.ID);
                }
                if (QCreditMemoVM.CMDate.HasValue)
                {
                    listCreditMemo = listCreditMemo.Where(x => x.CMDate == QCreditMemoVM.CMDate);
                }
                if (!string.IsNullOrWhiteSpace(QCreditMemoVM.CMStatus))
                {
                    listCreditMemo = listCreditMemo.Where(x => x.CMStatus == QCreditMemoVM.CMStatus);
                }
                if (QCreditMemoVM.InvoiceDate.HasValue)
                {
                    listCreditMemo = listCreditMemo.Where(x => x.InvoiceDate == QCreditMemoVM.InvoiceDate);
                }
                if (!string.IsNullOrWhiteSpace(QCreditMemoVM.CMType))
                {
                    listCreditMemo = listCreditMemo.Where(x => x.CMType == QCreditMemoVM.CMType);
                }
                if (!string.IsNullOrWhiteSpace(QCreditMemoVM.InvoiceNo))
                {
                    listCreditMemo = listCreditMemo.Where(x => x.InvoiceNo == QCreditMemoVM.InvoiceNo);
                }
                //if (!string.IsNullOrWhiteSpace(QCreditMemoVM.Creater))
                //{
                //    listCreditMemo = listCreditMemo.Where(x => x.Creater == QCreditMemoVM.Creater);
                //}
                //if (QCreditMemoVM.PaymentDate.HasValue)
                //{
                //    listCreditMemo = listCreditMemo.Where(x => x.PaymentDate == QCreditMemoVM.PaymentDate);
                //}
                //if (!string.IsNullOrWhiteSpace(QCreditMemoVM.PaymentStatus))
                //{
                //    listCreditMemo = listCreditMemo.Where(x => x.PaymentStatus == QCreditMemoVM.PaymentStatus);
                //}
                //if (QCreditMemoVM.ReceivedDate.HasValue)
                //{
                //    listCreditMemo = listCreditMemo.Where(x => x.ReceivedDate == QCreditMemoVM.ReceivedDate);
                //}
                //if (!string.IsNullOrWhiteSpace(QCreditMemoVM.ReceiveStatus))
                //{
                //    listCreditMemo = listCreditMemo.Where(x => x.ReceiveStatus == QCreditMemoVM.ReceiveStatus);
                //}
                if (!string.IsNullOrWhiteSpace(QCreditMemoVM.Tracking))
                {
                    listCreditMemo = listCreditMemo.Where(x => x.Tracking == QCreditMemoVM.Tracking);
                }
                //if (!string.IsNullOrWhiteSpace(QCreditMemoVM.Brand))
                //{
                //    listCreditMemo = listCreditMemo.Where(x => x.Brand == QCreditMemoVM.Brand);
                //}
                //if (!string.IsNullOrWhiteSpace(QCreditMemoVM.SKU))
                //{
                //    listCreditMemo = listCreditMemo.Where(x => x.SKU == QCreditMemoVM.SKU);
                //}
                //if (!string.IsNullOrWhiteSpace(QCreditMemoVM.Category))
                //{
                //    listCreditMemo = listCreditMemo.Where(x => x.Category == QCreditMemoVM.Category);
                //}
                //if (!string.IsNullOrWhiteSpace(QCreditMemoVM.Serial))
                //{
                //    listCreditMemo = listCreditMemo.Where(x => x.Serial == QCreditMemoVM.Serial);
                //}





                //表格查詢
                if (filter.ID.HasValue)
                {
                    listCreditMemo = listCreditMemo.Where(x => x.ID == filter.ID);
                }
                if (!string.IsNullOrWhiteSpace(filter.CMType))
                {
                    listCreditMemo = listCreditMemo.Where(x => x.CMType == filter.CMType);
                }
                if (filter.VendorID.HasValue)
                {
                    listCreditMemo = listCreditMemo.Where(x => x.VendorID == filter.VendorID);
                }
                if (filter.CMDate.HasValue)
                {
                    listCreditMemo = listCreditMemo.Where(x => x.CMDate == filter.CMDate);
                }
                if (!string.IsNullOrWhiteSpace(filter.CMStatus))
                {
                    listCreditMemo = listCreditMemo.Where(x => x.CMStatus == filter.CMStatus);
                }

                var dataList = listCreditMemo.ToList().Select(x => new
                {
                    x.ID,
                    x.CMType,
                    VendorID = x.VendorID.ToString(),
                    CMDate = x.CMDate.Value.ToString("yyyy/MM/dd"),
                    QTY = x.PurchaseSKU.Sum(y => y.QTYOrdered),
                    GrandTotal = x.PurchaseSKU.Sum(y => (y.QTYOrdered * y.Price)),
                    Balance = x.PurchaseSKU.Sum(y => (y.QTYOrdered * y.Price)),
                    x.CMStatus, 
                     
                });

                if (filter.QTY.HasValue)
                {
                    dataList = dataList.Where(x => x.QTY == filter.QTY);
                }
                if (filter.GrandTotal.HasValue)
                {
                    dataList = dataList.Where(x => x.GrandTotal == filter.GrandTotal);
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
                    case "CMTypeDDL":
                        optionList.Add(type, EnumData.CMTypeDDL().Select(x => new { text = x.Value, value = x.Key }));
                        break;
                    case "VendorIDDDL":
                        optionList.Add(type, EnumData.VendorDDL().Select(x => new { text = x.Value, value = x.Key }));
                        break;
                    case "CompanyIDDDL":
                        optionList.Add(type, EnumData.CompanyDDL().Select(x => new { text = x.Value, value = x.Key }));
                        break;
                    case "CMStatusDDL":
                        optionList.Add(type, EnumData.CMStatusDDL().Select(x => new { text = x.Value, value = x.Key }));
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