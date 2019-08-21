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
    public class PurchaseOrderCMController : BaseController
    {
        // GET: PurchaseOrder
        public ActionResult Index(CreditMemoVMQ CreditMemoVM)
        {
            Session["CreditMemoVM"] = CreditMemoVM;
            return View(CreditMemoVM);
        }
        public ActionResult CreateCM(int ID, int[] Skulist, string CMTypeVal = "CreditNote")
        {
            Session["CMPurchaseNote"] = null;
            Session["CMSkuNumberList"] = null;
            var SID = DateTime.Now.ToString("HHmmssfff");
            ViewBag.SID = SID;
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
                CMType = CMTypeVal,
                WarehouseID = PurchaseOrder.WarehouseID,
                Tax = PurchaseOrder.Tax,
                Currency = PurchaseOrder.Currency
            };
            var PurchaseSKUList = PurchaseOrder.PurchaseSKU.Where(x => x.IsEnable);
            if (Skulist != null && Skulist.Any())
            {
                PurchaseSKUList = PurchaseSKUList.Where(x => Skulist.Contains(x.ID));
            }
            var dataList = PurchaseSKUList.Select(x => new CMSKUVM
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
            Session["CMSkuNumberList" + SID] = dataList.ToList();
            return View(cmvm);
        }
        [HttpPost]
        public ActionResult CreateCM(CreditMemo filter, IEnumerable<HttpPostedFileBase> VendorCM, string SID)
        {
            var CreditMemo = new CreditMemo
            {
                IsEnable = true,
                PurchaseOrderID = filter.PurchaseOrderID,
                CompanyID = filter.CompanyID,
                VendorID = filter.VendorID,
                InvoiceDate = filter.InvoiceDate,
                InvoiceNo = filter.InvoiceNo,
                CMDate = filter.CMDate ?? DateTime.Today,
                CMStatus = filter.CMStatus,
                CMType = filter.CMType,
                ShippingStatus = filter.ShippingStatus,
                ShippedDate = filter.ShippedDate,
                Carrier = filter.Carrier,
                Tracking = filter.Tracking,
                CreditStatus = filter.CreditStatus,
                CreditDate = filter.CreditDate,
                CreditAmount = filter.CreditAmount,
                CreateBy = UserBy,
                CreateAt = DateTime.UtcNow
            };
            db.CreditMemo.Add(CreditMemo);
            var dataList = (List<CMSKUVM>)Session["CMSkuNumberList" + SID];
            if (dataList != null)
            {
                var PurchaseSKUlist = dataList.Select(x => new PurchaseSKU
                {
                    IsEnable = true,
                    Name = x.Name,
                    SkuNo = x.SKU,
                    VendorSKU = string.IsNullOrWhiteSpace(x.VendorSKU) ? x.SKU : x.VendorSKU,
                    QTYOrdered = x.QTYOrdered,
                    QTYReturned = x.QTYReturned,
                    CreditQTY = x.CreditQTY,
                    Credit = x.Credit,
                    QTYReceived = x.QTYReceived,
                    CreateBy = UserBy,
                    CreateAt = DateTime.UtcNow
                });
                foreach (var item in PurchaseSKUlist)
                {
                    CreditMemo.PurchaseSKU.Add(item);
                }
            }
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
            var CMPurchaseNote = (List<PurchaseNote>)Session["CMPurchaseNote" + SID];
            if (CMPurchaseNote != null)
            {
                foreach (var item in CMPurchaseNote)
                {
                    if (item.NoteType != "txt" && item.NoteType != "Url")
                    {
                        item.Note = SaveImg(item.Note, item.NoteType);
                        item.NoteType = "Url";
                    }
                    CreditMemo.PurchaseNote.Add(item);
                }
            }
            db.SaveChanges();
            return RedirectToAction("EditItem", new { CreditMemo.ID });
        }
        [HttpPost]
        public ActionResult DeleteData(int[] IDList)
        {
            var Msg = new List<string>();
            var UpdateAt = DateTime.UtcNow;
            foreach (var ID in IDList)
            {
                var CreditMemo = db.CreditMemo.Find(ID);
                if (CreditMemo.CMStatus == "Opened")
                {
                    CreditMemo.IsEnable = false;
                    CreditMemo.UpdateAt = UpdateAt;
                    CreditMemo.UpdateBy = UserBy;
                    foreach (var item in CreditMemo.PurchaseSKU)
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

            var CreditMemo = db.CreditMemo.Find(ID);
            if (!CreditMemo.WarehouseID.HasValue)
            {
                if (CreditMemo.RMAID.HasValue)
                {
                    CreditMemo.WarehouseID = CreditMemo.RMA.WarehouseID;
                }
                else if (CreditMemo.PurchaseOrderID.HasValue)
                {
                    CreditMemo.WarehouseID = CreditMemo.PurchaseOrder.WarehouseID;
                }
            }
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
            var dataList = CreditMemo.PurchaseSKU.Where(x => x.IsEnable).Select(x => new CMSKUVM
            {
                ID = x.ID,
                ck = x.SkuNo,
                sk = x.SkuNo,
                Name = x.Name,
                SKU = x.SkuNo,
                VendorSKU = x.VendorSKU,
                QTYOrdered = x.QTYOrdered,
                QTYReceived = x.QTYReceived ?? 0,
                QTYReturned = x.QTYReturned.HasValue && x.QTYReturned > 0 ? x.QTYReturned.Value : x.SerialsLlist.Where(y => y.SerialsType == "CM").Sum(y => y.SerialsQTY) * -1,
                CreditQTY = x.CreditQTY ?? 0,
                Credit = x.Credit ?? 0,
                Subtotal = (x.CreditQTY * x.Credit) ?? 0,
                Model = "L"
            });
            if (CreditMemo.ShippingStatus != "Completed")
            {
                var QTYOrderedSum = dataList.Sum(x => x.QTYOrdered);
                var QTYReturnedSum = dataList.Sum(x => x.QTYReturned);
                if (QTYReturnedSum == 0)
                {
                    CreditMemo.ShippingStatus = "Pending";
                }
                else if (QTYOrderedSum == QTYReturnedSum)
                {
                    CreditMemo.ShippingStatus = "Completed";
                }
                else if (QTYOrderedSum > QTYReturnedSum)
                {
                    CreditMemo.ShippingStatus = "PartiallyReturned";
                }
                else if (QTYOrderedSum < QTYReturnedSum)
                {
                    CreditMemo.ShippingStatus = "OverShipped";
                }
                db.SaveChanges();
            }
            if (CreditMemo.CMStatus != "Completed")
            {
                if (CreditMemo.ShippingStatus == "Completed" && CreditMemo.CreditStatus == "Completed")
                {
                    CreditMemo.CMStatus = "Completed";
                }
                else
                {
                    CreditMemo.CMStatus = "Pending";
                }
                db.SaveChanges();
            }
            Session["CMSkuNumberList" + ID] = dataList.ToList();
            return View(CreditMemo);
        }
        [HttpPost]
        public ActionResult EditItem(CreditMemo filter, IEnumerable<HttpPostedFileBase> VendorCM, string SID, bool? saveexit)
        {
            var dt = DateTime.UtcNow;
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
            if (filter.WarehouseID.HasValue) CreditMemo.WarehouseID = filter.WarehouseID;
            CreditMemo.UpdateBy = UserBy;
            CreditMemo.UpdateAt = dt;

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
                            CreateAt = dt
                        });
                    }

                }
            }
            var CMSkuNumberList = (List<CMSKUVM>)Session["CMSkuNumberList" + SID];
            if (CMSkuNumberList != null)
            {
                var PurchaseSKUE = CMSkuNumberList.Where(x => x.Model == "E").Select(x => new PurchaseSKU
                {
                    ID = x.ID.HasValue ? x.ID.Value : 0,
                    Name = x.Name,
                    SkuNo= x.SKU,
                    VendorSKU = string.IsNullOrWhiteSpace(x.VendorSKU) ? x.SKU : x.VendorSKU,
                    QTYOrdered = x.QTYOrdered,
                    CreditQTY=x.CreditQTY,
                    Credit = x.Credit,
                });
                foreach (var item in PurchaseSKUE)
                {
                    var oldPurchaseSKU = CreditMemo.PurchaseSKU.Where(x => x.IsEnable && x.ID == item.ID);
                    if (oldPurchaseSKU.Any() && item.ID != 0)
                    {
                        foreach (var SKUitem in oldPurchaseSKU)
                        {

                            if (SKUitem.CreditQTY != item.CreditQTY)
                            {
                                SKUitem.CreditQTY = item.CreditQTY;
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
                        item.IsEnable = true;
                        item.CreateAt = dt;
                        item.CreateBy = UserBy;
                        CreditMemo.PurchaseSKU.Add(item);
                    }
                }
                var PurchaseSKUlistD = CMSkuNumberList.Where(x => x.Model == "D").Select(x => x.ID);
                foreach (var item in PurchaseSKUlistD)
                {
                    var oldPurchaseSKU = CreditMemo.PurchaseSKU.Where(x => x.IsEnable && x.ID == item);
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


            db.SaveChanges();
            if (saveexit.HasValue && saveexit.Value)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("EditItem", new { filter.ID });
            }
        }
        public ActionResult ReturnItems(int ID)
        {
            var CreditMemo = db.CreditMemo.Find(ID);
            return View(CreditMemo);
        }
        [HttpPost]
        public ActionResult ReturnItems(CreditMemo CreditMemo, List<PostList> QTYReturned)
        {
            var UpdateAt = DateTime.UtcNow;
            var oCreditMemo = db.CreditMemo.Find(CreditMemo.ID);
            if (oCreditMemo.CMType != CreditMemo.CMType)
            {
                oCreditMemo.CMType = CreditMemo.CMType;
            }
            if (oCreditMemo.InvoiceNo != CreditMemo.InvoiceNo)
            {
                oCreditMemo.InvoiceNo = CreditMemo.InvoiceNo;
            }
            oCreditMemo.UpdateAt = UpdateAt;
            oCreditMemo.UpdateBy = UserBy;
            foreach (var QTYReturneditem in QTYReturned)
            {
                if (!string.IsNullOrWhiteSpace(QTYReturneditem.val))
                {
                    var val = 0;
                    if (int.TryParse(QTYReturneditem.val, out val))
                    {
                        foreach (var item in oCreditMemo.PurchaseSKU.Where(x => x.IsEnable && x.ID == QTYReturneditem.ID))
                        {
                            item.QTYReturned = val;
                            //隨機序號做CM
                            CMFreeSerials(item, UpdateAt);
                        }
                    }
                }
            }
            db.SaveChanges();
            return View(CreditMemo);
        }

        private void CMFreeSerials(PurchaseSKU PurchaseSKU, DateTime UpdateAt)
        {

            var count = PurchaseSKU.QTYReturned ?? 0;
            var CMSerialsLlistCount = PurchaseSKU.SerialsLlist.Where(x => x.SerialsType == "CM").Count();//CM數
            if (count > CMSerialsLlistCount)
            {
                var NoUseSerialsLlistCount = db.SerialsLlist.Where(x => x.PurchaseSKU.IsEnable && x.PurchaseSKU.PurchaseOrder.IsEnable && !x.SerialsLlistC.Any() && x.PurchaseSKU.SkuNo == PurchaseSKU.SkuNo).Take(count - CMSerialsLlistCount);         
                foreach (var item in NoUseSerialsLlistCount)
                {
                    var nSerialsLlist = new SerialsLlist
                    {
                        IsEnable = true,
                        PurchaseSKUID = item.PurchaseSKUID,
                        PID = item.ID,
                        SerialsType = "CM",
                        SerialsNo = item.SerialsNo,
                        SerialsQTY = -1,
                        CreateBy = UserBy,
                        CreateAt = UpdateAt
                    };
                    PurchaseSKU.SerialsLlist.Add(nSerialsLlist);
                }
            }
        }

        [HttpPost]
        public ActionResult GetSerialList(int ID)
        {
            var PurchaseSKU = db.PurchaseSKU.Find(ID);
            var SerialsLlist = PurchaseSKU.SerialsLlist.Where(x => x.SerialsType == "CM").ToList();
            var Serials = new List<string>();
            foreach (var item in SerialsLlist)
            {
                var Warehouse = "";
                if (item.PID.HasValue)
                {
                    if ( item.SerialsLlistP.TransferSKUID.HasValue)
                    {
                        Warehouse= item.SerialsLlistP.TransferSKU.Transfer.WarehouseTo.Name;
                    }
                    else
                    {
                        Warehouse = item.SerialsLlistP.PurchaseSKU.PurchaseOrder.WarehousePO.Name;
                    }
                   
                }
                Serials.Add(item.SerialsNo + "　　" + Warehouse);
            }
            var partial = ControlToString("~/Views/Shared/GetSerialList.cshtml", Serials);
            //var partial = Engine.Razor.RunCompile(template, "templateKey", null, new { Name = "World" });
            return Json(new { status = true, partial }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetData(CreditMemoVM filter, string Type, int? DetailID, int page = 1, int rows = 100)
        {
            if (Type == "Master")
            {
                var QCreditMemoVM = (CreditMemoVMQ)Session["CreditMemoVM"];
                int total = 0;
                var listCreditMemo = db.CreditMemo.Where(x => x.IsEnable).AsQueryable();
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
                if (QCreditMemoVM.CMDateS.HasValue)
                {
                    listCreditMemo = listCreditMemo.Where(x => x.CMDate >= QCreditMemoVM.CMDateS);
                }
                if (QCreditMemoVM.CMDateE.HasValue)
                {
                    listCreditMemo = listCreditMemo.Where(x => x.CMDate <= QCreditMemoVM.CMDateE);
                }
                if (!string.IsNullOrWhiteSpace(QCreditMemoVM.CMStatus))
                {
                    listCreditMemo = listCreditMemo.Where(x => x.CMStatus == QCreditMemoVM.CMStatus);
                }
                if (QCreditMemoVM.InvoiceDateS.HasValue)
                {
                    listCreditMemo = listCreditMemo.Where(x => x.InvoiceDate >= QCreditMemoVM.InvoiceDateS);
                }
                if (QCreditMemoVM.InvoiceDateE.HasValue)
                {
                    listCreditMemo = listCreditMemo.Where(x => x.InvoiceDate <= QCreditMemoVM.InvoiceDateE);
                }
                if (!string.IsNullOrWhiteSpace(QCreditMemoVM.CMType))
                {
                    listCreditMemo = listCreditMemo.Where(x => x.CMType == QCreditMemoVM.CMType);
                }
                if (!string.IsNullOrWhiteSpace(QCreditMemoVM.InvoiceNo))
                {
                    listCreditMemo = listCreditMemo.Where(x => x.InvoiceNo == QCreditMemoVM.InvoiceNo);
                }
                if (!string.IsNullOrWhiteSpace(QCreditMemoVM.Creater))
                {
                    listCreditMemo = listCreditMemo.Where(x => x.CreateBy == QCreditMemoVM.Creater);
                }
                if (QCreditMemoVM.CreditDateS.HasValue)
                {
                    listCreditMemo = listCreditMemo.Where(x => x.CreditDate >= QCreditMemoVM.CreditDateS);
                }
                if (QCreditMemoVM.CreditDateE.HasValue)
                {
                    listCreditMemo = listCreditMemo.Where(x => x.CreditDate <= QCreditMemoVM.CreditDateE);
                }
                if (!string.IsNullOrWhiteSpace(QCreditMemoVM.CreditStatus))
                {
                    listCreditMemo = listCreditMemo.Where(x => x.CreditStatus == QCreditMemoVM.CreditStatus);
                }
                if (QCreditMemoVM.ReturnDateS.HasValue)
                {
                    //listCreditMemo = listCreditMemo.Where(x => x.ReturnDate == QCreditMemoVM.ReturnDate);
                }
                if (QCreditMemoVM.ReturnDateE.HasValue)
                {
                    //listCreditMemo = listCreditMemo.Where(x => x.ReturnDate == QCreditMemoVM.ReturnDate);
                }
                if (!string.IsNullOrWhiteSpace(QCreditMemoVM.ReturnStatus))
                {
                    //listCreditMemo = listCreditMemo.Where(x => x.ReturnStatus == QCreditMemoVM.ReturnStatus);
                }
                if (!string.IsNullOrWhiteSpace(QCreditMemoVM.Brand))
                {
                    var Brandlist = db.Brand.Where(x => x.Name.Contains(QCreditMemoVM.Brand)).Select(x => x.ID).ToList();
                    var skuidlist = db.SKU.Where(x => Brandlist.Contains(x.Brand)).Select(x => x.SkuID).ToList();
                    listCreditMemo = listCreditMemo.Where(x => x.PurchaseSKU.Where(y => y.IsEnable && skuidlist.Contains(y.SkuNo)).Any());
                }

                if (!string.IsNullOrWhiteSpace(QCreditMemoVM.Tracking))
                {
                    listCreditMemo = listCreditMemo.Where(x => x.Tracking == QCreditMemoVM.Tracking);
                }
                if (!string.IsNullOrWhiteSpace(QCreditMemoVM.Category))
                {
                    var TypeIDlist = db.SkuTypeLang.Where(x => x.Name.Contains(QCreditMemoVM.Category)).Select(x => x.TypeID).ToList();
                    var skuidlist = db.SKU.Where(x => TypeIDlist.Contains(x.Category)).Select(x => x.SkuID).ToList();
                    listCreditMemo = listCreditMemo.Where(x => x.PurchaseSKU.Where(y => y.IsEnable && skuidlist.Contains(y.SkuNo)).Any());
                }
                if (!string.IsNullOrWhiteSpace(QCreditMemoVM.SKU))
                {
                    listCreditMemo = listCreditMemo.Where(x => x.PurchaseSKU.Where(y => x.IsEnable && y.SkuNo == QCreditMemoVM.SKU).Any());
                }
            
                if (!string.IsNullOrWhiteSpace(QCreditMemoVM.Serial))
                {
                    var PurchaseSKUID = db.SerialsLlist.Where(x => x.SerialsNo == QCreditMemoVM.Serial && x.SerialsType == "CM").Select(x => x.PurchaseSKUID).ToList();
                    listCreditMemo = listCreditMemo.Where(x => x.PurchaseSKU.Where(y => y.IsEnable && PurchaseSKUID.Contains(y.ID)).Any());
                }
                if (!string.IsNullOrWhiteSpace(QCreditMemoVM.POID))
                {
                    var PurchaseOrderID = 0;
                    int.TryParse(QCreditMemoVM.POID, out PurchaseOrderID);
                    listCreditMemo = listCreditMemo.Where(x => x.PurchaseOrderID == PurchaseOrderID);
                }




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
                    CMDate = x.CMDate.Value.ToLocalTime().ToString("yyyy/MM/dd"),
                    QTY = x.PurchaseSKU.Where(y => y.IsEnable).Sum(y => y.QTYOrdered),
                    GrandTotal = x.PurchaseSKU.Where(y => y.IsEnable).Sum(y => (y.QTYOrdered * y.Price)),
                    Balance = x.PurchaseSKU.Where(y => y.IsEnable).Sum(y => (y.QTYOrdered * y.Price)),
                    x.CMStatus,
                    POID = "<a target='_blank' href='" + Url.Action("EditItem", "PurchaseOrderPO", new { id = x.PurchaseOrderID }) + "'>" + x.PurchaseOrderID + "</a>"
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
                int length = rows;
                int start = (page - 1) * length;
                return Json(new { total, rows = dataList.OrderByDescending(x => x.ID).Skip(start).Take(length) }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var PurchaseSKU = db.PurchaseSKU.Where(x => x.IsEnable && x.PurchaseOrderID == DetailID).OrderByDescending(x => x.ID);
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
        public ActionResult Addserials(int? ID)
        {
            var PurchaseSKU = db.PurchaseSKU.Find(ID);
            var companyList = db.Company.AsNoTracking().Where(c => c.IsEnable).ToList();
            ViewBag.Company = companyList.Where(c => !c.ParentID.HasValue).Select(c => new SelectListItem() { Text = c.Name, Value = c.ID.ToString() }).ToList();
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
        public ActionResult DelSerialsNo(string serials, int PurchaseSKUID)
        {
            var PurchaseSKU = db.PurchaseSKU.Find(PurchaseSKUID);
            var SerialsLlist = PurchaseSKU.SerialsLlist.Where(x => x.SerialsNo == serials && x.SerialsType == "CM");
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
            var SerialsLlistCount = PurchaseSKU.SerialsLlist.Where(x => x.SerialsType == "CM").Sum(x => x.SerialsQTY);//計算CM的序號數
            if (SerialsLlistCount >= PurchaseSKU.QTYOrdered)
            {
                return Json(new { status = false, Errmsg = "序號數不可大於退貨數" }, JsonRequestBehavior.AllowGet);
            }
            var SerialsLlist = db.SerialsLlist.Where(x => x.IsEnable && x.SerialsNo == serials);
            
            if (!SerialsLlist.Where(x => x.PurchaseSKU.ID == PurchaseSKU.ID).Any())//檢查序號是否重複，同訂單同序號不能新增
            {
                if (SerialsLlist.Sum(x => x.SerialsQTY) > 0)
                {
                    if (SerialsLlist.Where(x => x.PurchaseSKU.SkuNo == PurchaseSKU.SkuNo).Any())
                    {
                        var oSerialsLlist = SerialsLlist.Where(x => x.PurchaseSKU.SkuNo == PurchaseSKU.SkuNo && x.SerialsQTY > 0 && !x.SerialsLlistC.Any()).OrderByDescending(x => x.ID).FirstOrDefault();
                        if (oSerialsLlist != null)
                        {
                            var PID = oSerialsLlist.ID;
                            var dt = DateTime.UtcNow;
                            var nSerialsLlist = new SerialsLlist
                            {
                                IsEnable = true,
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
                            var TSerialsLlist = SerialsLlist.Where(x => (x.SerialsType == "TransferIn") && x.SerialsQTY > 0 && !x.SerialsLlistC.Any()).OrderByDescending(x => x.ID).FirstOrDefault();
                            if (TSerialsLlist != null)
                            {
                                var PID = TSerialsLlist.ID;
                                var dt = DateTime.UtcNow;
                                var nSerialsLlist = new SerialsLlist
                                {
                                    IsEnable = true,
                                    PurchaseSKUID = PurchaseSKUID,
                                    TransferSKUID= TSerialsLlist.TransferSKUID,
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
                    }
                    else
                    {
                        return Json(new { status = false, Errmsg = "此序號不屬於這個SKU" }, JsonRequestBehavior.AllowGet);
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