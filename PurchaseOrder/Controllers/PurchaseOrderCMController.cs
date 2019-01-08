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
        public ActionResult EditItem(int ID)
        {
            var Warehouselist = db.Warehouse.Where(x => x.IsEnable).Select(x => new SelectListItem { Text = x.Name, Value = x.ID.ToString() }).ToList();
            ViewBag.Warehouselist = Warehouselist;
            var CreditMemo = db.CreditMemo.Find(ID);
            //var cmvm = new CMVM
            //{
            //    PurchaseOrderID = CreditMemo.ID,
            //    CompanyID = CreditMemo.CompanyID,
            //    VendorID = CreditMemo.VendorID,
            //    InvoiceDate = CreditMemo.InvoiceDate,
            //    InvoiceNo = CreditMemo.InvoiceNo,
            //    ShippedDate = CreditMemo.ShippedDate,
            //    Carrier = CreditMemo.Carrier,
            //    Tracking = CreditMemo.Tracking,
            //    CMType = CreditMemo.CMType,
            //    WarehouseID = CreditMemo.PurchaseOrder.WarehouseID,
            //    Tax = CreditMemo.PurchaseOrder.Tax,
            //    Currency = CreditMemo.PurchaseOrder.Currency
            //};
            var dataList = CreditMemo.PurchaseSKU.Select(x => new CMSKUVM
            {
                ID = x.ID,
                ck = x.SkuNo,
                sk = x.SkuNo,
                Name = x.Name,
                SKU = x.SkuNo,
                VendorSKU = x.VendorSKU,
                QTYOrdered = x.QTYOrdered,
                QTYReceived = x.QTYReceived ?? 0,
                QTYReturned = x.QTYReturned ?? 0,
                CreditQTY = x.CreditQTY ?? 0,
                Credit = x.Credit ?? 0,
                Subtotal = (x.CreditQTY * x.Credit) ?? 0,
                Model = "L"
            });
            Session["CMSkuNumberList"] = dataList.ToList();
            return View(CreditMemo);
        }
        [HttpPost]
        public ActionResult EditItem(CreditMemo filter, IEnumerable<HttpPostedFileBase> VendorCM)
        {
            var CreditMemo = db.CreditMemo.Find(filter.ID);
            if (filter.CompanyID.HasValue) CreditMemo.CompanyID = filter.CompanyID;
            if (filter.VendorID.HasValue) CreditMemo.VendorID = filter.VendorID;
            if (filter.InvoiceDate.HasValue) CreditMemo.InvoiceDate = filter.InvoiceDate;
            if (!string.IsNullOrWhiteSpace(filter.InvoiceNo)) CreditMemo.InvoiceNo = filter.InvoiceNo;
            if (!string.IsNullOrWhiteSpace(filter.CMStatus)) CreditMemo.CMStatus = filter.CMStatus;
            if (!string.IsNullOrWhiteSpace(filter.CMType)) CreditMemo.CMType = filter.CMType;
            if (filter.CMDate.HasValue) CreditMemo.CMDate = filter.CMDate;
            if (!string.IsNullOrWhiteSpace(filter.ShippingStatus)) CreditMemo.ShippingStatus = filter.ShippingStatus;
            if (filter.ShippedDate.HasValue) CreditMemo.ShippedDate = filter.ShippedDate;
            if (!string.IsNullOrWhiteSpace(filter.Carrier)) CreditMemo.Carrier = filter.Carrier;
            if (!string.IsNullOrWhiteSpace(filter.Tracking)) CreditMemo.Tracking = filter.Tracking;
            if (!string.IsNullOrWhiteSpace(filter.CreditStatus)) CreditMemo.CreditStatus = filter.CreditStatus;
            if (filter.CreditDate.HasValue) CreditMemo.CreditDate = filter.CreditDate;
            if (filter.CreditAmount.HasValue) CreditMemo.CreditAmount = filter.CreditAmount;
            CreditMemo.UpdateBy = UserBy;
            CreditMemo.UpdateAt = DateTime.UtcNow;

            if (VendorCM != null && VendorCM.Any())
            {
                foreach (var file in VendorCM)
                {
                    if (file != null)
                    {
                        var Url = SaveImg(file);
                        CreditMemo.ImgFile.Add(new ImgFile
                        {
                            IsEnable = true,
                            ImgType = "VendorCM",
                            Url = Url,
                            CreateBy = UserBy,
                            CreateAt = DateTime.UtcNow
                        });
                    }

                }
            }
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult ReturnItems(int ID)
        {
            var CreditMemo = db.CreditMemo.Find(ID);
            return View(CreditMemo);
        }
        [HttpPost]
        public ActionResult ReturnItems(CreditMemo CreditMemo, List<PostList> QTYReturned)
        {
            var oCreditMemo = db.CreditMemo.Find(CreditMemo.ID);
            if (oCreditMemo.CMType != CreditMemo.CMType)
            {
                oCreditMemo.CMType = CreditMemo.CMType;
            }
            if (oCreditMemo.InvoiceNo != CreditMemo.InvoiceNo)
            {
                oCreditMemo.InvoiceNo = CreditMemo.InvoiceNo;
            }

            foreach (var QTYReturneditem in QTYReturned)
            {
                if (!string.IsNullOrWhiteSpace(QTYReturneditem.val))
                {
                    var val = 0;
                    if (int.TryParse(QTYReturneditem.val, out val))
                    {
                        foreach (var item in oCreditMemo.PurchaseSKU.Where(x => x.ID == QTYReturneditem.ID))
                        {
                            item.QTYReturned = val;
                        }
                    }
                }
            }
            db.SaveChanges();
            return View(CreditMemo);
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
                return Json(new { total, rows = dataList.OrderByDescending(x => x.ID) }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var PurchaseSKU = db.PurchaseSKU.Where(x => x.PurchaseOrderID == DetailID).OrderByDescending(x => x.ID);
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
        public ActionResult Addserials(int ID)
        {
            var PurchaseSKU = db.PurchaseSKU.Find(ID);
            return View(PurchaseSKU);
        }
        [HttpPost]
        public ActionResult Addserials(int ID, string UPCEAN, DateTime? ShippedDate)
        {
            var PurchaseSKU = db.PurchaseSKU.Find(ID);
            if (!string.IsNullOrEmpty(UPCEAN))
            {
                PurchaseSKU.UPCEAN = UPCEAN;
            }
            if (ShippedDate.HasValue)
            {
                PurchaseSKU.ReceivedDate = ShippedDate;
            }
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Saveserials(string serials, int PurchaseSKUID)
        {
            var PurchaseSKU = db.PurchaseSKU.Find(PurchaseSKUID);
            var SerialsLlistCount = PurchaseSKU.SerialsLlist.Where(x => x.SerialsType == "CM").Sum(x => x.SerialsQTY);//計算CM的序號數
            if (SerialsLlistCount >= PurchaseSKU.QTYReturned)
            {
                return Json(new { status = false, Errmsg = "序號數不可大於退貨數" }, JsonRequestBehavior.AllowGet);
            }
            var SerialsLlist = db.SerialsLlist.Where(x => x.SerialsNo == serials);
            
            if (!SerialsLlist.Where(x => x.PurchaseSKU.CreditMemoID == PurchaseSKU.CreditMemoID).Any())//檢查序號是否重複，同訂單同序號不能新增
            {
                if (SerialsLlist.Sum(x => x.SerialsQTY) > 0)
                {
                    var PID = SerialsLlist.Where(x => x.SerialsQTY > 0).FirstOrDefault().ID;
                    var dt = DateTime.UtcNow;
                    var nSerialsLlist = new SerialsLlist
                    {
                        PurchaseSKUID = PurchaseSKUID,
                        PID = PID,
                        SerialsType = "CM",
                        SerialsNo = serials,
                        SerialsQTY = -1,
                        CreateBy = UserBy,
                        CreateAt = dt
                    };
                    db.SerialsLlist.Add(nSerialsLlist);

                    try
                    {
                        db.SaveChanges();
                        return Json(new { status = true }, JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception ex)
                    {
                        return Json(new { status = false, Errmsg = ex.ToString() }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { status = false, Errmsg = "此序號不在倉庫內" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { status = false, Errmsg = "序號已經存在" }, JsonRequestBehavior.AllowGet);
            }

        }
    }
}