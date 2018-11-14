﻿using PurchaseOrderSys.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PurchaseOrderSys.Controllers
{
    public class PurchaseOrderPOController : BaseController
    {
        // GET: PurchaseOrder
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Create()
        {
            Session["SkuNumberList"] = null;
            Session["PurchaseNote"] = null;
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
            var dataList = (List<PoSKUVM>)Session["SkuNumberList"];
            if (dataList != null)
            {
                var PurchaseSKUlist = dataList.Select(x => new PurchaseSKU
                {
                    IsEnable = true,
                    Name = x.Name,
                    SkuNo = x.SKU,
                    VendorSKU = string.IsNullOrWhiteSpace(x.VendorSKU) ? x.SKU : x.VendorSKU,
                    QTYOrdered = x.QTYOrdered,
                    QTYFulfilled = x.QTYFulfilled,
                    Price = x.Price,
                    Discount = x.Discount,
                    DiscountedPrice = x.DiscountedPrice,
                    Credit = x.Credit,
                    CreateBy = UserBy,
                    CreateAt = DateTime.UtcNow
                });
                foreach (var item in PurchaseSKUlist)
                {
                    PurchaseOrder.PurchaseSKU.Add(item);
                }
            }
            var PurchaseNoteLlist = (List<PurchaseNote>)Session["PurchaseNote"];
            if (PurchaseNoteLlist != null)
            {
                foreach (var item in PurchaseNoteLlist)
                {
                    PurchaseOrder.PurchaseNote.Add(item);
                }
            }
            try
            {
                db.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {

                var e = ex;
            }

            return RedirectToAction("Index");
        }
        public ActionResult GetData(PurchaseOrderPOVM filter, string Type, int? DetailID)
        {
            if (Type == "Master")
            {
                int total = 0;
                var listPurchaseOrder = db.PurchaseOrder.Where(x=>x.IsEnable);
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
                return Json(new { total, rows = dataList.OrderByDescending(x => x.ID) }, JsonRequestBehavior.AllowGet);
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
        public ActionResult EditItem(int ID, string POTypeVal)
        {
            var PurchaseOrder = db.PurchaseOrder.Find(ID);
            var dataList = PurchaseOrder.PurchaseSKU.Select(x => new PoSKUVM
            {
                ID = x.ID,
                ck = x.SkuNo,
                sk = x.SkuNo,
                Name = x.Name,
                SKU = x.SkuNo,
                VendorSKU = x.VendorSKU,
                QTYOrdered = x.QTYOrdered,
                QTYFulfilled = x.QTYFulfilled,
                QTYReceived = x.QTYReceived,
                QTYReturned = x.QTYReturned,
                Price = x.Price,
                Discount = x.Discount,
                DiscountedPrice = x.DiscountedPrice,
                Credit = x.Credit,
                Subtotal = (x.QTYOrdered * (x.Price - x.Discount)),
                SerialQTY = x.SerialsLlist.Count()
            });
            if (!string.IsNullOrWhiteSpace(POTypeVal))
            {
                PurchaseOrder.POType = POTypeVal;
            }

            Session["SkuNumberList"] = dataList.ToList();
            var Warehouselist = db.Warehouse.Where(x => x.IsEnable).Select(x => new SelectListItem { Text = x.Name, Value = x.ID.ToString() }).ToList();
            ViewBag.Warehouselist = Warehouselist;
            return View(PurchaseOrder);
        }
        [HttpPost]
        public ActionResult EditItem(PurchaseOrderPOVM filter)
        {
            var dt = DateTime.UtcNow;
            var PurchaseOrder = db.PurchaseOrder.Find(filter.ID);

            PurchaseOrder.CompanyID = filter.CompanyID;
            PurchaseOrder.VendorID = filter.VendorID;
            PurchaseOrder.PODate = filter.PODate;
            PurchaseOrder.POStatus = filter.POStatus;
            PurchaseOrder.POType = filter.POType;
            PurchaseOrder.WarehouseID = filter.WarehouseID;
            PurchaseOrder.UpdateBy = UserBy;
            PurchaseOrder.UpdateAt = dt;

            var dataList = (List<PoSKUVM>)Session["SkuNumberList"];
            if (dataList != null)
            {
                var PurchaseSKUlist = dataList.Select(x => new PurchaseSKU
                {
                    ID=x.ID.Value,
                    IsEnable = true,
                    Name = x.Name,
                    SkuNo = x.SKU,
                    VendorSKU = string.IsNullOrWhiteSpace(x.VendorSKU) ? x.SKU : x.VendorSKU,
                    QTYOrdered = x.QTYOrdered,
                    QTYFulfilled = x.QTYFulfilled,
                    Price = x.Price,
                    Discount = x.Discount,
                    DiscountedPrice = x.DiscountedPrice,
                    Credit = x.Credit,
                });
                foreach (var item in PurchaseSKUlist)
                {
                    foreach (var SKUitem in PurchaseOrder.PurchaseSKU.Where(x => x.ID == item.ID))
                    {
                        if (SKUitem.Price != item.Price)
                        {
                            SKUitem.Price = item.Price;
                            SKUitem.UpdateBy = UserBy;
                            SKUitem.UpdateAt = dt;
                        }
                        if (SKUitem.Discount != item.Discount)
                        {
                            SKUitem.Discount = item.Discount;
                            SKUitem.UpdateBy = UserBy;
                            SKUitem.UpdateAt = dt;
                        }
                        if (SKUitem.Credit != item.Credit)
                        {
                            SKUitem.Credit = item.Credit;
                            SKUitem.UpdateBy = UserBy;
                            SKUitem.UpdateAt = dt;
                        }
                    }
                }
            }
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult ReceiveItems(int ID)
        {
            var PurchaseOrder = db.PurchaseOrder.Find(ID);
            return View(PurchaseOrder);
        }
        [HttpPost]
        public ActionResult ReceiveItems(PurchaseOrder PurchaseOrder, List<PostList> QTYReceived)
        {
            var oPurchaseOrder = db.PurchaseOrder.Find(PurchaseOrder.ID);
            if (oPurchaseOrder.POType!= PurchaseOrder.POType)
            {
                oPurchaseOrder.POType = PurchaseOrder.POType;
            }
            if (oPurchaseOrder.InvoiceNo != PurchaseOrder.InvoiceNo)
            {
                oPurchaseOrder.InvoiceNo = PurchaseOrder.InvoiceNo;
            }

            foreach (var QTYReceiveditem in QTYReceived)
            {
                if (!string.IsNullOrWhiteSpace(QTYReceiveditem.val))
                {
                    var val = 0;
                    if (int.TryParse(QTYReceiveditem.val, out val))
                    {
                        foreach (var item in oPurchaseOrder.PurchaseSKU.Where(x => x.ID == QTYReceiveditem.ID))
                        {
                            item.QTYReceived = val;
                        }
                    }
                }
            }
            db.SaveChanges();
            return View(PurchaseOrder);
        }
        public ActionResult Addserials(int ID)
        {
            var PurchaseSKU = db.PurchaseSKU.Find(ID);
            return View(PurchaseSKU);
        }
        public ActionResult Saveserials(string serials, int PurchaseSKUID)
        {
            var PurchaseSKU = db.PurchaseSKU.Find(PurchaseSKUID);
            var SerialsLlistCount = PurchaseSKU.SerialsLlist.Where(x => x.SerialsType == 1 && x.SerialsLlistC.Count() == 0).Count();

            if (SerialsLlistCount >= PurchaseSKU.QTYOrdered)
            {
                return Json(new { status = false, Errmsg = "序號不可大於採購數" }, JsonRequestBehavior.AllowGet);
            }
            var SerialsLlist = db.SerialsLlist.Where(x => x.SerialsNo == serials);
            if (SerialsLlist.Count() == 0 || SerialsLlist.Sum(x => x.SerialsType) == 0)
            {
                var dt = DateTime.UtcNow;
                var nSerialsLlist = new SerialsLlist
                {
                    PurchaseSKUID = PurchaseSKUID,
                    SerialsNo = serials,
                    SerialsType = 1,
                    ReceivedBy = UserBy,
                    ReceivedAt = dt,
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
                return Json(new { status = false, Errmsg = "序號已經存在" }, JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public ActionResult CreatNote(int? ID, string Note)
        {
            try
            {
                var PurchaseNoteList = new List<PurchaseNote>();
                if (ID.HasValue)
                {
                    var PurchaseOrder = db.PurchaseOrder.Find(ID);
                    PurchaseOrder.PurchaseNote.Add(new PurchaseNote { IsEnable = true, Note = Note, CreateAt = DateTime.UtcNow, CreateBy = UserBy });
                    db.SaveChanges();
                    PurchaseNoteList = PurchaseOrder.PurchaseNote.ToList();
                }
                else
                {
                    PurchaseNoteList = (List<PurchaseNote>)Session["PurchaseNote"];
                    if (PurchaseNoteList == null)
                    {
                        PurchaseNoteList = new List<PurchaseNote>();
                    }

                    PurchaseNoteList.Add(new PurchaseNote { IsEnable = true, Note = Note, CreateAt = DateTime.UtcNow, CreateBy = UserBy });
                    Session["PurchaseNote"] = PurchaseNoteList;
                }
                return Json(new { status = true, datalist = PurchaseNoteList.OrderByDescending(x => x.CreateAt).Select(x => new { CreateAt = x.CreateAt.ToString("yyyy/MM/dd HH:mm:ss"), x.CreateBy }).ToList() }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, Errmsg = ex.ToString() }, JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult CreatePO()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreatePO(PurchaseOrderPOVM filter)
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
        public ActionResult CreateCM(int ID)
        {
            var PurchaseOrder = db.PurchaseOrder.Find(ID);
            var cmvm = new CMVM
            {
                PurchaseOrderID = PurchaseOrder.ID,
                CompanyID = PurchaseOrder.CompanyID,
                VendorID = PurchaseOrder.VendorID,
                InvoiceDate = PurchaseOrder.InvoiceDate,
                InvoiceNo = PurchaseOrder.InvoiceNo,
                ShippedDate = PurchaseOrder.ShippedDate,
                Carrier = PurchaseOrder.Carrier,
                Tracking = PurchaseOrder.Tracking,
                PurchaseSKU = PurchaseOrder.PurchaseSKU.Where(x => x.IsEnable).ToList()
            };
            return View(cmvm);
        }
        [HttpPost]
        public ActionResult CreateCM(PurchaseOrderPOVM filter)
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
        [HttpPost]
        public ActionResult CopyData(int[] IDList)
        {
            var CreateAt = DateTime.UtcNow;
            foreach (var ID in IDList)
            {
                var PurchaseOrder = db.PurchaseOrder.Find(ID);
                var nPurchaseOrder = new PurchaseOrder
                {
                    IsEnable = true,
                    CompanyID = PurchaseOrder.CompanyID,
                    VendorID = PurchaseOrder.VendorID,
                    POStatus = PurchaseOrder.POStatus,
                    POType = PurchaseOrder.POType,
                    PODate = PurchaseOrder.PODate,
                    ReceiveStatus = PurchaseOrder.ReceiveStatus,
                    ReceivedDate = PurchaseOrder.ReceivedDate,
                    ShippedDate = PurchaseOrder.ShippedDate,
                    Carrier = PurchaseOrder.Carrier,
                    Tracking = PurchaseOrder.Tracking,
                    InvoiceDate = PurchaseOrder.InvoiceDate,
                    InvoiceNo = PurchaseOrder.InvoiceNo,
                    PaymentProof = PurchaseOrder.PaymentProof,
                    PaymentStatus = PurchaseOrder.PaymentStatus,
                    PaymentDate = PurchaseOrder.PaymentDate,
                    PaidAmount = PurchaseOrder.PaidAmount,
                    Warehouse = PurchaseOrder.Warehouse,
                    Currency = PurchaseOrder.Currency,
                    Tax = PurchaseOrder.Tax,
                    CreateBy = UserBy,
                    CreateAt = CreateAt,
                };
                db.PurchaseOrder.Add(nPurchaseOrder);
            }
            db.SaveChanges();
            return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult DeleteData(int[] IDList)
        {
            var UpdateAt = DateTime.UtcNow;
            foreach (var ID in IDList)
            {
                var PurchaseOrder = db.PurchaseOrder.Find(ID);
                PurchaseOrder.IsEnable = false;
                PurchaseOrder.UpdateAt = UpdateAt;
                PurchaseOrder.UpdateBy = UserBy;
            }
            db.SaveChanges();
            return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        }
        //[HttpPost]
        public ActionResult GetSerialList(int ID)
        {
            var PurchaseSKU = db.PurchaseSKU.Find(ID);
            var SerialsLlist = PurchaseSKU.SerialsLlist.Where(x => x.SerialsType == 1 && x.SerialsLlistC.Count() == 0).Select(x => x.SerialsNo).ToList();
            var partial = ControlToString("~/Views/PurchaseOrderPO/GetSerialList.cshtml", SerialsLlist);
            //var partial = Engine.Razor.RunCompile(template, "templateKey", null, new { Name = "World" });
            return Json(new { status = true, partial }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult RemoveData(string[] IDList)
        {

            foreach (var item in IDList)
            {

            }
            return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        }
        
    }
}