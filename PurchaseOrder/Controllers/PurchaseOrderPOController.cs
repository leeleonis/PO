using PurchaseOrderSys.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using inventorySKU;
using PurchaseOrderSys.Helpers;
using OfficeOpenXml;
using SellerCloud_WebService;

namespace PurchaseOrderSys.Controllers
{
    [CheckSession]
    public class PurchaseOrderPOController : BaseController
    {
        // GET: PurchaseOrder
        public ActionResult Index(PurchaseOrderPOVMQ PurchaseOrderPOVM)
        {
            Session["PurchaseOrderPOVM"] = PurchaseOrderPOVM;
            return View(PurchaseOrderPOVM);
        }
        public ActionResult Create(string POTypeVal = "PurchaseOrder")
        {

            //SCWS = new SC_WebService(ApiUserName, ApiPassword);
            //var Company = SCWS.Get_AllCompany();
            //var CompanyID = Company.FirstOrDefault().ID;
            //var VendorData = SCWS.Get_Vendor_All(CompanyID);
            //var Vendor = VendorData.Where(x => x.DisplayName.Contains("Senao"));
            Session["SkuNumberList"] = null;
            Session["POPurchaseNote"] = null;
            ViewBag.SID = DateTime.Now.ToString("HHmmss");
            var PurchaseOrder = new PurchaseOrder();
            PurchaseOrder.POType = POTypeVal;
            return View(PurchaseOrder);
        }
        [HttpPost]
        public ActionResult Create(PurchaseOrder filter, IEnumerable<HttpPostedFileBase> VendorInvoice, IEnumerable<HttpPostedFileBase> PaymentProofList, string SID)
        {
            var CreateAt = DateTime.UtcNow;
            var nPurchaseOrder = new PurchaseOrder
            {
                IsEnable = true,
                WarehouseID = filter.WarehouseID,
                CompanyID = filter.CompanyID,
                VendorID = filter.VendorID,
                PODate = DateTime.Today,
                POStatus = filter.POStatus,
                POType = filter.POType,
                ReceiveStatus = filter.ReceiveStatus,
                PaymentStatus = filter.PaymentStatus,
                PaymentDate = filter.PaymentDate,
                PaidAmount = filter.PaidAmount,
                Currency = filter.Currency,
                InvoiceDate = filter.InvoiceDate,
                InvoiceNo = filter.InvoiceNo,
                Tax = filter.Tax,
                CreateBy = UserBy,
                CreateAt = CreateAt
            };   
            db.PurchaseOrder.Add(nPurchaseOrder);
            var dataList = (List<PoSKUVM>)Session["SkuNumberList" + SID];
            if (dataList != null)
            {
                var PurchaseSKUlist = dataList.Where(x => x.Model == "E").Select(x => new PurchaseSKU
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
                    CreateAt = CreateAt
                });
                foreach (var item in PurchaseSKUlist)
                {
                    nPurchaseOrder.PurchaseSKU.Add(item);
                }
            }
            var PurchaseNoteLlist = (List<PurchaseNote>)Session["POPurchaseNote" + SID];
            if (PurchaseNoteLlist != null)
            {
                foreach (var item in PurchaseNoteLlist)
                {
                    if (item.NoteType != "txt" && item.NoteType != "Url")
                    {
                        item.Note = SaveImg(item.Note, item.NoteType);
                        item.NoteType = "Url";
                    }
                    nPurchaseOrder.PurchaseNote.Add(item);
                }
            }


            if (VendorInvoice != null && VendorInvoice.Any())
            {
                foreach (var file in VendorInvoice)
                {
                    if (file != null)
                    {
                        var Url = SaveImg(file);
                        nPurchaseOrder.ImgFile.Add(new ImgFile
                        {
                            IsEnable = true,
                            ImgType = "VendorInvoice",
                            Url = Url,
                            CreateBy = UserBy,
                            CreateAt = CreateAt
                        });
                    }

                }
            }
            if (PaymentProofList != null && PaymentProofList.Any())
            {
                foreach (var file in PaymentProofList)
                {
                    if (file != null)
                    {
                        var Url = SaveImg(file);
                        nPurchaseOrder.ImgFile.Add(new ImgFile
                        {
                            IsEnable = true,
                            ImgType = "PaymentProof",
                            Url = Url,
                            CreateBy = UserBy,
                            CreateAt = CreateAt
                        });
                    }
                }
            }

            try
            {
                db.SaveChanges();
                ////存檔後建立SC的資料
                //var SCPurchase = CreatPObySC(nPurchaseOrder);
                //nPurchaseOrder.SCPurchaseID = SCPurchase.ID;
                //db.SaveChanges();
                //CreatAndEditPOSKUbySC(nPurchaseOrder);
                
            }
            catch (DbEntityValidationException ex)
            {

                var e = ex;
            }
            catch(Exception ex)
            {

            }

            return RedirectToAction("EditItem", new { nPurchaseOrder.ID });
        }
        public ActionResult GetData(PurchaseOrderPOVM filter, string Type, int? DetailID, int page = 1, int rows = 100)
        {
            if (Type == "Master")
            {
                var QPurchaseOrderPOVM = (PurchaseOrderPOVMQ)Session["PurchaseOrderPOVM"];
                int total = 0;
                var listPurchaseOrder = db.PurchaseOrder.Where(x => x.IsEnable);
                //綱頁查詢
                //listPurchaseOrder= QueryData(listPurchaseOrder, QPurchaseOrderPOVM);

                //try
                //{
                //    var sl = listPurchaseOrder.ToList();
                //}
                //catch (Exception ex)
                //{

                // var s=   ex.ToString();
                //}

                if (QPurchaseOrderPOVM.CompanyID.HasValue)
                {
                    listPurchaseOrder = listPurchaseOrder.Where(x => x.CompanyID == QPurchaseOrderPOVM.CompanyID);
                }
                if (QPurchaseOrderPOVM.VendorID.HasValue)
                {
                    listPurchaseOrder = listPurchaseOrder.Where(x => x.VendorID == QPurchaseOrderPOVM.VendorID);
                }
                if (QPurchaseOrderPOVM.ID.HasValue)
                {
                    listPurchaseOrder = listPurchaseOrder.Where(x => x.ID == QPurchaseOrderPOVM.ID);
                }
                if (QPurchaseOrderPOVM.PODateS.HasValue)
                {
                    listPurchaseOrder = listPurchaseOrder.Where(x => x.PODate >= QPurchaseOrderPOVM.PODateS);
                }
                if (QPurchaseOrderPOVM.PODateE.HasValue)
                {
                    listPurchaseOrder = listPurchaseOrder.Where(x => x.PODate <= QPurchaseOrderPOVM.PODateE);
                }
                if (!string.IsNullOrWhiteSpace(QPurchaseOrderPOVM.POStatus))
                {
                    listPurchaseOrder = listPurchaseOrder.Where(x => x.POStatus == QPurchaseOrderPOVM.POStatus);
                }
                if (QPurchaseOrderPOVM.InvoiceDateS.HasValue)
                {
                    listPurchaseOrder = listPurchaseOrder.Where(x => x.InvoiceDate >= QPurchaseOrderPOVM.InvoiceDateS);
                }
                if (QPurchaseOrderPOVM.InvoiceDateE.HasValue)
                {
                    listPurchaseOrder = listPurchaseOrder.Where(x => x.InvoiceDate <= QPurchaseOrderPOVM.InvoiceDateE);
                }
                if (!string.IsNullOrWhiteSpace(QPurchaseOrderPOVM.POType))
                {
                    listPurchaseOrder = listPurchaseOrder.Where(x => x.POType == QPurchaseOrderPOVM.POType);
                }
                if (!string.IsNullOrWhiteSpace(QPurchaseOrderPOVM.InvoiceNo))
                {
                    listPurchaseOrder = listPurchaseOrder.Where(x => x.InvoiceNo == QPurchaseOrderPOVM.InvoiceNo);
                }
                if (!string.IsNullOrWhiteSpace(QPurchaseOrderPOVM.Creater))
                {
                    listPurchaseOrder = listPurchaseOrder.Where(x => x.CreateBy == QPurchaseOrderPOVM.Creater);
                }
                if (QPurchaseOrderPOVM.PaymentDateS.HasValue)
                {
                    listPurchaseOrder = listPurchaseOrder.Where(x => x.PaymentDate >= QPurchaseOrderPOVM.PaymentDateS);
                }
                if (QPurchaseOrderPOVM.PaymentDateE.HasValue)
                {
                    listPurchaseOrder = listPurchaseOrder.Where(x => x.PaymentDate <= QPurchaseOrderPOVM.PaymentDateE);
                }
                if (!string.IsNullOrWhiteSpace(QPurchaseOrderPOVM.PaymentStatus))
                {
                    listPurchaseOrder = listPurchaseOrder.Where(x => x.PaymentStatus == QPurchaseOrderPOVM.PaymentStatus);
                }
                if (QPurchaseOrderPOVM.ReceivedDateS.HasValue)
                {
                    listPurchaseOrder = listPurchaseOrder.Where(x => x.ReceivedDate >= QPurchaseOrderPOVM.ReceivedDateS);
                }
                if (QPurchaseOrderPOVM.ReceivedDateE.HasValue)
                {
                    listPurchaseOrder = listPurchaseOrder.Where(x => x.ReceivedDate <= QPurchaseOrderPOVM.ReceivedDateE);
                }
                if (!string.IsNullOrWhiteSpace(QPurchaseOrderPOVM.ReceiveStatus))
                {
                    listPurchaseOrder = listPurchaseOrder.Where(x => x.ReceiveStatus == QPurchaseOrderPOVM.ReceiveStatus);
                }
                if (!string.IsNullOrWhiteSpace(QPurchaseOrderPOVM.Tracking))
                {
                    listPurchaseOrder = listPurchaseOrder.Where(x => x.Tracking == QPurchaseOrderPOVM.Tracking);
                }
                if (!string.IsNullOrWhiteSpace(QPurchaseOrderPOVM.Brand))
                {
                    var Brandlist = db.Brand.Where(x => x.Name.Contains(QPurchaseOrderPOVM.Brand)).Select(x => x.ID).ToList();
                    var skuidlist = db.SKU.Where(x => Brandlist.Contains(x.Brand)).Select(x => x.SkuID).ToList();
                    listPurchaseOrder = listPurchaseOrder.Where(x => x.PurchaseSKU.Where(y => y.IsEnable && skuidlist.Contains(y.SkuNo)).Any());
                }
                if (!string.IsNullOrWhiteSpace(QPurchaseOrderPOVM.SKU))
                {
                    listPurchaseOrder = listPurchaseOrder.Where(x => x.PurchaseSKU.Where(y => y.IsEnable && y.SkuNo == QPurchaseOrderPOVM.SKU).Any());
                }
                if (!string.IsNullOrWhiteSpace(QPurchaseOrderPOVM.Category))
                {
                    var TypeIDlist = db.SkuTypeLang.Where(x => x.Name.Contains(QPurchaseOrderPOVM.Category)).Select(x => x.TypeID).ToList();
                    var skuidlist = db.SKU.Where(x => TypeIDlist.Contains(x.Category)).Select(x => x.SkuID).ToList();
                    listPurchaseOrder = listPurchaseOrder.Where(x => x.PurchaseSKU.Where(y => y.IsEnable && skuidlist.Contains(y.SkuNo)).Any());
                }
                if (!string.IsNullOrWhiteSpace(QPurchaseOrderPOVM.Serial))
                {
                    var PurchaseSKUID = db.SerialsLlist.Where(x => x.SerialsNo == QPurchaseOrderPOVM.Serial && x.SerialsType == "PO").Select(x => x.PurchaseSKUID).ToList();
                    listPurchaseOrder = listPurchaseOrder.Where(x => x.PurchaseSKU.Where(y => y.IsEnable && PurchaseSKUID.Contains(y.ID)).Any());
                }
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
                    VendorID = x.VendorID?.ToString(),
                    PODate = x.PODate?.ToLocalTime().ToString("yyyy/MM/dd"),
                    QTY = x.PurchaseSKU.Where(y => y.IsEnable).Sum(y => y.QTYOrdered),
                    QTYReceived = x.PurchaseSKU.Where(y => y.IsEnable).Sum(y => GetQTYReceived(y)),
                    GrandTotal = x.PurchaseSKU.Where(y => y.IsEnable).Sum(y => (y.QTYOrdered * y.Price)),
                    x.PaidAmount,
                    Balance = x.PurchaseSKU.Where(y => y.IsEnable).Sum(y => (y.QTYOrdered * y.Price)),
                    x.POStatus,
                    x.Description,
                    CMID= GetCMData(x)
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
                if (filter.QTYReceived.HasValue)
                {
                    dataList = dataList.Where(x => x.QTYReceived == filter.QTYReceived);
                }
                try
                {
                    total = dataList.Count();
                }
                catch (Exception ex)
                {

                    throw;
                }
                

                int length = rows;
                int start = (page - 1) * length;
                return Json(new { total, rows = dataList.OrderByDescending(x => x.ID).Skip(start).Take(length) }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var PurchaseSKU = db.PurchaseSKU.Where(x => x.PurchaseOrderID == DetailID);
                return PartialView("Detail", PurchaseSKU);
            }
        }

        private object GetCMData(PurchaseOrder PurchaseOrder)
        {
            var CreditMemolist = PurchaseOrder.CreditMemo.Where(x => x.IsEnable);
            if (CreditMemolist.Any())
            {
                var linkList = "<a target='_blank' href='" + Url.Action("index", "PurchaseOrderCM", new { POID = PurchaseOrder.ID }) + "'>Yes</a>";
                return linkList;
            }
            else
            {
                return "No";
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
                            optionList.Add(type, EnumData.POTypeDDL().Select(x => new { text = x.Value, value = x.Key }));
                            break;
                        case "VendorIDDDL":
                            optionList.Add(type, EnumData.VendorDDL().Select(x => new { text = x.Value, value = x.Key }));
                            break;
                        case "CompanyIDDDL":
                            optionList.Add(type, EnumData.CompanyDDL().Select(x => new { text = x.Value, value = x.Key }));
                            break;
                        case "POStatusDDL":
                            optionList.Add(type, EnumData.POStatusDDL().Select(x => new { text = x.Value, value = x.Key }));
                            break;
                    }
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
            var dataList = PurchaseOrder.PurchaseSKU.Where(x => x.IsEnable).Select(x => new PoSKUVM
            {
                ID = x.ID,
                ck = x.SkuNo,
                sk = x.SkuNo,
                Name = x.Name,
                SKU = x.SkuNo,
                VendorSKU = x.VendorSKU,
                QTYOrdered = x.QTYOrdered,
                QTYFulfilled = x.QTYFulfilled,
                QTYReceived = GetQTYReceived(x),
                QTYReturned = x.QTYReturned,
                Price = x.Price ?? 0,
                Discount = x.Discount ?? 0,
                DiscountedPrice = ((x.Price ?? 0) - (x.Discount ?? 0) - (x.Credit ?? 0)),
                Credit = x.Credit ?? 0,
                Subtotal = ((x.QTYOrdered ?? 0) * ((x.Price ?? 0) - (x.Discount ?? 0))),
                SerialQTY = x.SerialsLlist.Where(y => y.SerialsType == "PO").Sum(y => y.SerialsQTY),
                Model = "L"
            });
            if (!string.IsNullOrWhiteSpace(POTypeVal))
            {
                PurchaseOrder.POType = POTypeVal;
            }
            if (PurchaseOrder.PaymentStatus != "Completed")
            {
                var PaidAmount = PurchaseOrder.PaidAmount;
                if (PaidAmount.HasValue)
                {
                    var TotalRefunded = 0;
                    var Total = dataList.Sum(x => x.Subtotal);
                    var GrandTotal = (Total ?? 0) - (PurchaseOrder.ShippingCost ?? 0) + (PurchaseOrder.Other ?? 0);
                    var Balance = PaidAmount - GrandTotal + TotalRefunded;
                    if (Balance == 0)
                    {
                        PurchaseOrder.PaymentStatus = "Completed";
                    }
                    else if (Balance < 0)
                    {
                        PurchaseOrder.PaymentStatus = "PartiallyPaid";
                    }
                    else if (Balance > 0)
                    {
                        PurchaseOrder.PaymentStatus = "OverPaid";
                    }
                    db.SaveChanges();
                }
            }
            var QTYOrdered = PurchaseOrder.PurchaseSKU.Where(x => x.IsEnable).Sum(x => x.QTYOrdered);
            var ReceiveQty = dataList.Sum(x => x.SerialQTY);
            if (PurchaseOrder.ReceiveStatus != "Completed")
            {
                if (QTYOrdered == ReceiveQty && ReceiveQty > 1)
                {
                    PurchaseOrder.ReceiveStatus = "Completed";
                }
                else if (QTYOrdered > ReceiveQty && ReceiveQty > 1)
                {
                    PurchaseOrder.ReceiveStatus = "PartiallyReceived";
                }
                else if (QTYOrdered < ReceiveQty && ReceiveQty > 1)
                {
                    PurchaseOrder.ReceiveStatus = "OverReceived";
                }
                else
                {
                    PurchaseOrder.ReceiveStatus = "Pending";
                }
                db.SaveChanges();
            }
            if (PurchaseOrder.POStatus != "Completed")
            {
                if (PurchaseOrder.PaymentStatus == "Completed" && PurchaseOrder.ReceiveStatus == "Completed")
                {
                    PurchaseOrder.POStatus = "Completed";
                }
                else if (PurchaseOrder.PaymentStatus == "Completed" || PurchaseOrder.ReceiveStatus == "Completed")
                {
                    PurchaseOrder.POStatus = "Pending";
                }
                else
                {
                    PurchaseOrder.POStatus = "Opened";
                }
                db.SaveChanges();
            }

            Session["SkuNumberList" + ID] = dataList.ToList();

            return View(PurchaseOrder);
        }

        private int GetQTYReceived(PurchaseSKU PurchaseSKU)
        {
            var QTYReceived = 0;
            if (PurchaseSKU.SerialsLlist.Any())
            {
                QTYReceived = PurchaseSKU.SerialsLlist.Where(x => x.SerialsType == "PO").Sum(x => x.SerialsQTY).Value;
            }
            else
            {
                QTYReceived = PurchaseSKU.QTYReceived ?? 0;
            }
            return QTYReceived;
        }

        [HttpPost]
        public ActionResult EditItem(PurchaseOrder filter, IEnumerable<HttpPostedFileBase> VendorInvoice, IEnumerable<HttpPostedFileBase> PaymentProofList, bool? saveexit,bool? UpdateSC)
        {
            var dt = DateTime.UtcNow;
            var PurchaseOrder = db.PurchaseOrder.Find(filter.ID);

            PurchaseOrder.CompanyID = filter.CompanyID;
            PurchaseOrder.VendorID = filter.VendorID;
            PurchaseOrder.PODate = filter.PODate;
            PurchaseOrder.POStatus = filter.POStatus;
            PurchaseOrder.POType = filter.POType;
            PurchaseOrder.ReceiveStatus = filter.ReceiveStatus;
            PurchaseOrder.ReceivedDate = filter.ReceivedDate;
            PurchaseOrder.InvoiceDate = filter.InvoiceDate;
            PurchaseOrder.InvoiceNo = filter.InvoiceNo;
            PurchaseOrder.PaymentDate = filter.PaymentDate;
            PurchaseOrder.PaymentStatus = filter.PaymentStatus;
            PurchaseOrder.ShippedDate = filter.ShippedDate;
            PurchaseOrder.PaidAmount = filter.PaidAmount;
            PurchaseOrder.Carrier = filter.Carrier;
            PurchaseOrder.PaymentProof = filter.PaymentProof;
            PurchaseOrder.Tracking = filter.Tracking;
            // PurchaseOrder.VendorInvoice = filter.VendorInvoice;
            PurchaseOrder.WarehouseID = filter.WarehouseID;
            PurchaseOrder.Currency = filter.Currency;
            PurchaseOrder.Tax = filter.Tax;
            PurchaseOrder.ShippingCost = filter.ShippingCost;
            PurchaseOrder.Other = filter.Other;
            PurchaseOrder.UpdateBy = UserBy;
            PurchaseOrder.UpdateAt = dt;

            var dataList = (List<PoSKUVM>)Session["SkuNumberList" + filter.ID];
            if (dataList != null)
            {
                var PurchaseSKUlistE = dataList.Where(x => x.Model == "E").Select(x => new PurchaseSKU
                {
                    ID = x.ID.HasValue ? x.ID.Value : 0,
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
                foreach (var item in PurchaseSKUlistE)
                {
                    var oldPurchaseSKU = PurchaseOrder.PurchaseSKU.Where(x => x.IsEnable && x.ID == item.ID);
                    if (oldPurchaseSKU.Any() && item.ID != 0)
                    {
                        foreach (var SKUitem in oldPurchaseSKU)
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
                            if (SKUitem.QTYOrdered != item.QTYOrdered)
                            {
                                SKUitem.QTYOrdered = item.QTYOrdered;
                                SKUitem.UpdateBy = UserBy;
                                SKUitem.UpdateAt = dt;
                            }
                        }
                    }
                    else
                    {
                        item.CreateAt = dt;
                        item.CreateBy = UserBy;
                        PurchaseOrder.PurchaseSKU.Add(item);
                    }
                }
                var PurchaseSKUlistD = dataList.Where(x => x.Model == "D").Select(x => x.ID);
                foreach (var item in PurchaseSKUlistD)
                {
                    var oldPurchaseSKU = PurchaseOrder.PurchaseSKU.Where(x => x.IsEnable && x.ID == item);
                    if (oldPurchaseSKU.Any())
                    {
                        foreach (var SKUitem in oldPurchaseSKU)
                        {
                            SKUitem.IsEnable = false;
                            SKUitem.UpdateBy = UserBy;
                            SKUitem.UpdateAt = dt;
                        }
                        //db.SaveChanges();
                    }
                }
            }
            if (VendorInvoice != null && VendorInvoice.Any())
            {
                foreach (var file in VendorInvoice)
                {
                    if (file != null)
                    {
                        var Url = SaveImg(file);
                        PurchaseOrder.ImgFile.Add(new ImgFile
                        {
                            IsEnable = true,
                            ImgType = "VendorInvoice",
                            Url = Url,
                            CreateBy = UserBy,
                            CreateAt = DateTime.UtcNow
                        });
                    }

                }
            }
            if (PaymentProofList != null && PaymentProofList.Any())
            {
                foreach (var file in PaymentProofList)
                {
                    if (file != null)
                    {
                        var Url = SaveImg(file);
                        PurchaseOrder.ImgFile.Add(new ImgFile
                        {
                            IsEnable = true,
                            ImgType = "PaymentProof",
                            Url = Url,
                            CreateBy = UserBy,
                            CreateAt = DateTime.UtcNow
                        });
                    }
                }
            }
            try
            {
                db.SaveChanges();
                if (UpdateSC.HasValue && UpdateSC.Value)
                {
                    //檢查SC上的資料
                    if (!PurchaseOrder.SCPurchaseID.HasValue)
                    {
                        var SCPurchase = CreatPObySC(PurchaseOrder);
                        PurchaseOrder.SCPurchaseID = SCPurchase.ID;
                        db.SaveChanges();
                    }
                    else
                    {
                        UpdatePObySC(PurchaseOrder);
                    }
                  
                    CreatAndEditPOSKUbySC(PurchaseOrder);
                    foreach (var PurchaseSKU in PurchaseOrder.PurchaseSKU)
                    {
                        foreach (var item in PurchaseSKU.SerialsLlist)
                        {
                            AddSerialToSC(PurchaseSKU, item.SerialsNo);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                
                var s = ex.ToString();
            }
            if (saveexit.HasValue && saveexit.Value)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("EditItem", new { filter.ID });
            }
        }

        public ActionResult ReceiveItems(int ID)
        {
            var PurchaseOrder = db.PurchaseOrder.Find(ID);
            return View(PurchaseOrder);
        }
        [HttpPost]
        public ActionResult ReceiveItems(PurchaseOrder PurchaseOrder, List<PostList> QTYReceived)
        {
            var UpdateAt = DateTime.UtcNow;
            var oPurchaseOrder = db.PurchaseOrder.Find(PurchaseOrder.ID);
            if (oPurchaseOrder.POType != PurchaseOrder.POType)
            {
                oPurchaseOrder.POType = PurchaseOrder.POType;
            }
            if (oPurchaseOrder.InvoiceNo != PurchaseOrder.InvoiceNo)
            {
                oPurchaseOrder.InvoiceNo = PurchaseOrder.InvoiceNo;
            }
            oPurchaseOrder.UpdateAt = UpdateAt;
            oPurchaseOrder.UpdateBy = UserBy;
            var UpdateSKUList = new List<PurchaseSKU>();
            foreach (var QTYReceiveditem in QTYReceived)
            {
                if (!string.IsNullOrWhiteSpace(QTYReceiveditem.val))
                {
                    var val = 0;
                    if (int.TryParse(QTYReceiveditem.val, out val))
                    {
                        foreach (var item in oPurchaseOrder.PurchaseSKU.Where(x => x.ID == QTYReceiveditem.ID && x.IsEnable))
                        {
                            
                            item.QTYReceived = val;
                            //產生隨機序號
                            UpdateSKUList.Add(GetFreeSerials(item, UpdateAt));
                        }
                    }

                }
            }
            db.SaveChanges();
            foreach (var PurchaseSKU in UpdateSKUList)//有新增才更新SC
            {
                foreach (var serial in PurchaseSKU.SerialsLlist)
                {
                    AddSerialToSC(PurchaseSKU, serial.SerialsNo);
                }
               
            }

         
            return View(oPurchaseOrder);
        }

        private PurchaseSKU GetFreeSerials(PurchaseSKU PurchaseSKU, DateTime UpdateAt)
        {
            var nPurchaseSKU = new PurchaseSKU {SkuNo= PurchaseSKU.SkuNo };
            var count = PurchaseSKU.QTYReceived;
            var SerialsLlistCount = PurchaseSKU.SerialsLlist.Where(x => x.SerialsType == "PO").Count();
            if (count > SerialsLlistCount)
            {
                for (int i = SerialsLlistCount; i < count; i++)
                {
                    var dt = DateTime.UtcNow;
                    var nSerialsLlist = new SerialsLlist
                    {
                        IsEnable = true,
                        SerialsNo = "POA" + PurchaseSKU.ID + dt.ToString("MMddHHmmssfff") + i,
                        SerialsType = "PO",
                        SerialsQTY = 1,
                        ReceivedBy = UserBy,
                        ReceivedAt = UpdateAt,
                        CreateBy = UserBy,
                        CreateAt = UpdateAt
                    };
                    PurchaseSKU.SerialsLlist.Add(nSerialsLlist);
                    nPurchaseSKU.SerialsLlist.Add(nSerialsLlist);
                }
            }
            return nPurchaseSKU;
        }

        public ActionResult Addserials(int ID)
        {
            var PurchaseSKU = db.PurchaseSKU.Find(ID);
            if (string.IsNullOrWhiteSpace(PurchaseSKU.UPCEAN))
            {
                var SKUlist = db.SKU.Where(x => x.SkuID == PurchaseSKU.SkuNo).ToList();
                SKUlist = SKUlist.Where(x => !string.IsNullOrWhiteSpace(x.EAN) || (!string.IsNullOrWhiteSpace(x.UPC) && x.UPC != "Does not apply")).ToList();
                if (SKUlist.Any())
                {
                    PurchaseSKU.UPCEAN = SKUlist.FirstOrDefault().UPC;
                    db.SaveChanges();
                }
            }
            if (PurchaseSKU.ReceivedDate != PurchaseSKU.SerialsLlist.OrderByDescending(x => x.ReceivedAt).FirstOrDefault()?.ReceivedAt)
            {
                PurchaseSKU.ReceivedDate = PurchaseSKU.SerialsLlist.OrderByDescending(x => x.ReceivedAt).FirstOrDefault()?.ReceivedAt;
                db.SaveChanges();
            }
            var companyList = db.Company.AsNoTracking().Where(c => c.IsEnable).ToList();
            ViewBag.Company = companyList.Where(c => !c.ParentID.HasValue).Select(c => new SelectListItem() { Text = c.Name, Value = c.ID.ToString() }).ToList();
            return View(PurchaseSKU);
        }
        [HttpPost]
        public ActionResult Addserials(int ID, string UPCEAN, DateTime? ReceivedDate)
        {
            var PurchaseSKU = db.PurchaseSKU.Find(ID);
            if (!string.IsNullOrEmpty(UPCEAN))
            {
                PurchaseSKU.UPCEAN = UPCEAN;
            }
            if (ReceivedDate.HasValue)
            {
                PurchaseSKU.ReceivedDate = ReceivedDate;
            }
            db.SaveChanges();
            return RedirectToAction("Addserials", new { id = ID });
        }
        public ActionResult DelSerialsNo(string serials, int PurchaseSKUID)
        {
            var PurchaseSKU = db.PurchaseSKU.Find(PurchaseSKUID);
            var SerialsLlist = PurchaseSKU.SerialsLlist.Where(x => x.SerialsNo == serials && x.SerialsType == "PO");
            if (SerialsLlist.Any())
            {
                SerialsLlist = SerialsLlist.Where(x => !x.SerialsLlistC.Any());
                if (SerialsLlist.Any())
                {
                    foreach (var item in SerialsLlist.ToList())
                    {
                        db.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                    }
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
                    return Json(new { status = false, Errmsg = "序號不在倉庫內" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { status = false, Errmsg = "查無序號" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Saveserials(string serials, int PurchaseSKUID)
        {
            var PurchaseSKU = db.PurchaseSKU.Find(PurchaseSKUID);
            var SerialsLlistCount = PurchaseSKU.SerialsLlist.Where(x => x.SerialsType == "PO").Sum(x => x.SerialsQTY);//計算PO的序號數
            if (SerialsLlistCount >= PurchaseSKU.QTYOrdered)
            {
                return Json(new { status = false, Errmsg = "序號不可大於採購數" }, JsonRequestBehavior.AllowGet);
            }
            if (PurchaseSKU.PurchaseOrder.POType == "DropshpOrder")//直發一入一出
            {
                var SerialsLlist = db.SerialsLlist.Where(x => x.SerialsNo == serials && x.PurchaseSKU.SkuNo == PurchaseSKU.SkuNo && !x.SerialsLlistC.Any() && (x.SerialsType == "PO" || x.SerialsType == "TransferIn"));//檢查序號是否重複，同SKU序號不能新增,2019/02/05 加入有已出貨或是CM的紀錄, 就能重新在入庫
                if (!SerialsLlist.Any())
                {
                    var dt = DateTime.UtcNow;
                    var nSerialsLlistIn = new SerialsLlist
                    {
                        IsEnable = true,
                        SerialsType = "DropshpOrderIn",
                        SerialsNo = serials,
                        SerialsQTY = 1,
                        ReceivedBy = UserBy,
                        ReceivedAt = dt,
                        CreateBy = UserBy,
                        CreateAt = dt
                    };
                    PurchaseSKU.SerialsLlist.Add(nSerialsLlistIn);
                    var nSerialsLlistOut = new SerialsLlist
                    {
                        IsEnable = true,
                        PurchaseSKUID = PurchaseSKUID,
                        SerialsType = "DropshpOrderOut",
                        SerialsNo = serials,
                        SerialsQTY = -1,
                        ReceivedBy = UserBy,
                        ReceivedAt = dt,
                        CreateBy = UserBy,
                        CreateAt = dt
                    };
                    nSerialsLlistIn.SerialsLlistC.Add(nSerialsLlistOut);
                    try
                    {
                        db.SaveChanges();
                        AddSerialToSC(PurchaseSKU, serials);
                        DelSerialToSC(PurchaseSKU, serials);
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
            else
            {
                var SerialsLlist = db.SerialsLlist.Where(x => x.SerialsNo == serials && x.PurchaseSKU.SkuNo == PurchaseSKU.SkuNo && !x.SerialsLlistC.Any() && (x.SerialsType == "PO" || x.SerialsType == "TransferIn"));//檢查序號是否重複，同SKU序號不能新增,2019/02/05 加入有已出貨或是CM的紀錄, 就能重新在入庫
                if (!SerialsLlist.Any())
                {
                    var dt = DateTime.UtcNow;
                    var nSerialsLlist = new SerialsLlist
                    {
                        IsEnable = true,
                        PurchaseSKUID = PurchaseSKUID,
                        SerialsType = "PO",
                        SerialsNo = serials,
                        SerialsQTY = 1,
                        ReceivedBy = UserBy,
                        ReceivedAt = dt,
                        CreateBy = UserBy,
                        CreateAt = dt
                    };
                    db.SerialsLlist.Add(nSerialsLlist);
                    try
                    {
                        db.SaveChanges();
                        //SC入庫
                        AddSerialToSC(PurchaseSKU, serials);
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
        }
        [HttpPost]
        public ActionResult CreatNote(int? ID,string SID, string Note)
        {
            try
            {
                var PurchaseNoteList = new List<PurchaseNote>();
                if (ID.HasValue && ID != 0)
                {
                    var PurchaseOrder = db.PurchaseOrder.Find(ID);
                    PurchaseOrder.PurchaseNote.Add(new PurchaseNote { IsEnable = true, Note = Note, NoteType = "txt", CreateAt = DateTime.UtcNow, CreateBy = UserBy });
                    db.SaveChanges();
                    PurchaseNoteList = PurchaseOrder.PurchaseNote.ToList();
                }
                else
                {
                    PurchaseNoteList = (List<PurchaseNote>)Session["POPurchaseNote" + SID];
                    if (PurchaseNoteList == null)
                    {
                        PurchaseNoteList = new List<PurchaseNote>();
                    }

                    PurchaseNoteList.Add(new PurchaseNote { IsEnable = true, Note = Note, NoteType = "txt", CreateAt = DateTime.UtcNow, CreateBy = UserBy });
                    Session["POPurchaseNote" + SID] = PurchaseNoteList;
                }
                return Json(new { status = true, datalist = PurchaseNoteList.OrderByDescending(x => x.CreateAt).Select(x => new { CreateAt = x.CreateAt.ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss"), x.CreateBy, x.Note, x.NoteType }).ToList() }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, Errmsg = ex.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult CreatNoteImg(int? ID,string SID, HttpPostedFileBase Img)
        {
            try
            {
                if (Img == null)
                {
                    return Json(new { status = false, Errmsg ="沒有圖檔" }, JsonRequestBehavior.AllowGet);
                }
                var NoteType = Img.ContentType;
                var PurchaseNoteList = new List<PurchaseNote>();
                if (ID.HasValue && ID != 0)
                {
                    var Note = SaveImg(Img);
                    var PurchaseOrder = db.PurchaseOrder.Find(ID);
                    PurchaseOrder.PurchaseNote.Add(new PurchaseNote { IsEnable = true, Note = Note, NoteType = "Url", CreateAt = DateTime.UtcNow, CreateBy = UserBy });
                    db.SaveChanges();
                    PurchaseNoteList = PurchaseOrder.PurchaseNote.ToList();
                }
                else
                {
                    MemoryStream target = new MemoryStream();

                    Img.InputStream.CopyTo(target);
                    byte[] data = target.ToArray();
                    string Note = Convert.ToBase64String(data, 0, data.Length);
                    PurchaseNoteList = (List<PurchaseNote>)Session["POPurchaseNote"+ SID];
                    if (PurchaseNoteList == null)
                    {
                        PurchaseNoteList = new List<PurchaseNote>();
                    }

                    PurchaseNoteList.Add(new PurchaseNote { IsEnable = true, Note = Note, NoteType = NoteType, CreateAt = DateTime.UtcNow, CreateBy = UserBy });
                    Session["POPurchaseNote"+ SID] = PurchaseNoteList;
                }
                return Json(new { status = true, datalist = PurchaseNoteList.OrderByDescending(x => x.CreateAt).Select(x => new { CreateAt = x.CreateAt.ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss"), x.CreateBy, x.Note, x.NoteType }).ToList() }, JsonRequestBehavior.AllowGet);
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
            return RedirectToAction("Edit", new { PurchaseOrder.ID });
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
            var Msg = new List<string>();
            var UpdateAt = DateTime.UtcNow;
            foreach (var ID in IDList)
            {
                var PurchaseOrder = db.PurchaseOrder.Find(ID);
                PurchaseOrder.IsEnable = false;
                PurchaseOrder.UpdateAt = UpdateAt;
                PurchaseOrder.UpdateBy = UserBy;
                foreach (var item in PurchaseOrder.PurchaseSKU)
                {

                    if (item.SerialsLlist.Any())
                    {
                        Msg.Add(item.SkuNo);
                    }
                    else
                    {
                        item.IsEnable = false;
                        item.UpdateAt = UpdateAt;
                        item.UpdateBy = UserBy;
                    }
                }
            }
            if (!Msg.Any())
            {
                db.SaveChanges();
                return Json(new { status = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = false, Msg = "下列SKU有序號：" + string.Join(";", Msg) }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult GetSerialList(int ID)
        {
            var PurchaseSKU = db.PurchaseSKU.Find(ID);
            var SerialsLlist = PurchaseSKU.SerialsLlist.Where(x => x.SerialsType == "PO"|| x.SerialsType == "DropshpOrderIn").Select(x => x.SerialsNo).ToList();
            var partial = ControlToString("~/Views/Shared/GetSerialList.cshtml", SerialsLlist);
            //var partial = Engine.Razor.RunCompile(template, "templateKey", null, new { Name = "World" });
            return Json(new { status = true, partial }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult RemoveData(string[] IDList,string SID)
        {
            var Errmsg = "";
            if (IDList != null && IDList.Any())
            {
                var odataList = (List<PoSKUVM>)Session["SkuNumberList" + SID];
                foreach (var item in IDList)
                {
                    foreach (var odataListitem in odataList.Where(x => x.ID.ToString() == item || x.SKU == item))
                    {
                        if (odataListitem.ID.HasValue)
                        {


                            var PurchaseSKU = db.PurchaseSKU.Find(odataListitem.ID);
                            if (PurchaseSKU.SerialsLlist.Any())
                            {
                                Errmsg += "【" + PurchaseSKU.SkuNo + "】已有序號不能刪除；";
                            }
                            else
                            {
                                odataListitem.Model = "D";
                            }
                        }
                        else
                        {
                            odataListitem.Model = "D";
                        }
                    }
                }
                Session["SkuNumberList" + SID] = odataList;
            }
            else
            {
                Errmsg = "沒有選取SKU";
            }
            if (string.IsNullOrWhiteSpace(Errmsg))
            {
                return Json(new { status = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = false, Errmsg }, JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult EditSKUData(string[] IDList,string SID)
        {
            var Errmsg = "";
            if (IDList != null && IDList.Any())
            {
                var odataList = (List<PoSKUVM>)Session["SkuNumberList" + SID];
                foreach (var item in IDList)
                {
                    foreach (var odataListitem in odataList.Where(x => x.ID.ToString() == item || x.SKU == item))
                    {
                        odataListitem.Model = "E";
                    }
                }
            }
            else
            {
                Errmsg = "沒有選取SKU";
            }
            if (string.IsNullOrWhiteSpace(Errmsg))
            {
                return Json(new { status = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = false, Errmsg }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult CreateByExcel(int id, HttpPostedFileBase PaymentProofList)
        {
            return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult PoEmail(int id)
        {
            var PurchaseOrder = db.PurchaseOrder.Find(id);
            var Email = PurchaseOrder.VendorLIst?.Email;
            var EmailCC = PurchaseOrder.VendorLIst?.EmailCC;


            if (string.IsNullOrWhiteSpace(Email))
            {
                return Json(new { status = false, Msg = "沒有Email" }, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrWhiteSpace(EmailCC))
            {
                return Json(new { status = false, Msg = "沒有EmailCC" }, JsonRequestBehavior.AllowGet);
            }
            Stream ExcelStream = CreateExcel(PurchaseOrder);
            string MailFrom = "";
            string[] MailTos = Email.Split(';');
            string[] Ccs = EmailCC.Split(';');
            string MailSub = PurchaseOrder.Company.Name + "  PO 採購單 #" + PurchaseOrder.ID + " 給 " + PurchaseOrder.VendorLIst.Name + " (" + DateTime.Today.ToString("yyyy/MM/dd") + ")";
            string MailBody = RenderPartialViewToString("~/Views/Shared/EmailPo.cshtml", PurchaseOrder);
            bool isBodyHtml = true;
            string[] filePaths = null;
            List<Tuple<Stream, string>> filePaths2 = new List<Tuple<Stream, string>>();
            filePaths2.Add(new Tuple<Stream, string>(ExcelStream, "POExcel.xlsx"));
            bool deleteFileAttachment = false;
            var status = MyHelps.Mail_Send(MailFrom, MailTos, Ccs, MailSub, MailBody, isBodyHtml, filePaths, filePaths2, deleteFileAttachment);

            return Json(new { status = status }, JsonRequestBehavior.AllowGet);
        }

        private Stream CreateExcel(PurchaseOrder PurchaseOrder)
        {
            var PurchaseSKUlist = PurchaseOrder.PurchaseSKU.Where(x => x.IsEnable);
            var Price = PurchaseSKUlist.Sum(x => (x.Price * x.QTYOrdered) ?? 0);
            var Discount = PurchaseSKUlist.Sum(x => (x.Price - x.Discount - x.Credit) ?? 0);
            var Credit = PurchaseSKUlist.Sum(x => x.Credit ?? 0);
            //在記憶體中建立一個Excel物件
            ExcelPackage ep = new ExcelPackage();
            //加入一個Sheet
            ep.Workbook.Worksheets.Add("PoList");
            //取得剛剛加入的Sheet(實體Sheet就叫MySheet)
            ExcelWorksheet sheet1 = ep.Workbook.Worksheets["PoList"];//取得Sheet1 
            sheet1.Cells[1, 1].Value = PurchaseOrder.Company.Name + "  PO 採購單 #" + PurchaseOrder.ID + " 給 " + PurchaseOrder.VendorLIst.Name + " (" + DateTime.Today.ToString("yyyy/MM/dd") + ")";
            sheet1.Cells["A1:J1"].Merge = true;
            sheet1.Cells[2, 1].Value = "以下是本公司今日採購的內容.請填入:";
            sheet1.Cells["A2:J2"].Merge = true;
            sheet1.Cells[3, 1].Value = "出貨價：" + Price.ToString("N2");
            sheet1.Cells["A3:J3"].Merge = true;
            sheet1.Cells[4, 1].Value = "現折金額：" + Discount.ToString("N2");
            sheet1.Cells["A4:J4"].Merge = true;
            sheet1.Cells[5, 1].Value = "後折金額：" + Credit.ToString("N2");
            sheet1.Cells["A5:J5"].Merge = true;
            sheet1.Cells[6, 1].Value = "SKU";
            sheet1.Cells[6, 2].Value = "Vendor SKU";
            sheet1.Cells[6, 3].Value = "Name";
            sheet1.Cells[6, 4].Value = "QTY Ordered";
            sheet1.Cells[6, 5].Value = "QTY Received";
            sheet1.Cells[6, 6].Value = "Price";
            sheet1.Cells[6, 7].Value = "Discount";
            sheet1.Cells[6, 8].Value = "Credit";
            sheet1.Cells[6, 9].Value = "Discounted Price";
            sheet1.Cells[6, 10].Value = "Subtotal(currency)";
            for (int j = 1; j <= 10; j++)
            {
                sheet1.Cells[6, j].AutoFitColumns();
            }


            int i = 7;
            var NameLen = 0;
            foreach (var item in PurchaseSKUlist)
            {
                var DiscountedPrice = (item.Price - item.Discount - item.Credit) ?? 0;
                var Subtotal = item.QTYOrdered * (item.Price - item.Discount) ?? 0;
                
                sheet1.Cells[i, 1].Value = item.SkuNo;
                sheet1.Cells[i, 2].Value = item.VendorSKU;
                sheet1.Cells[i, 3].Value = item.Name;
                sheet1.Cells[i, 4].Value = item.QTYOrdered;
                sheet1.Cells[i, 5].Value = item.QTYReceived;
                sheet1.Cells[i, 6].Value = item.Price.Value.ToString("N2");
                sheet1.Cells[i, 7].Value = item.Discount;
                sheet1.Cells[i, 8].Value = item.Credit;
                sheet1.Cells[i, 9].Value = DiscountedPrice.ToString("N2");
                sheet1.Cells[i, 10].Value = Subtotal.ToString("N2");

                if (item.Name.Length > NameLen)
                {
                    NameLen = item.Name.Length;
                    for (int j = 1; j <= 10; j++)
                    {
                        sheet1.Cells[6, j].AutoFitColumns();
                    }
                }
                i++;
            }
            Stream OutputStream = new MemoryStream(ep.GetAsByteArray());
            return OutputStream;
        }
    }
}
