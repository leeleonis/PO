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
    public class TransferController : BaseController
    {
        // GET: Transfer
        public ActionResult Index(TransferSearchVM TransferSearchVM)
        {
            var Transferlist = db.Transfer.Where(x => x.IsEnable);


            if (!string.IsNullOrWhiteSpace(TransferSearchVM.Status))
            {
                Transferlist = Transferlist.Where(x => x.Status == TransferSearchVM.Status);
            }
            if (!string.IsNullOrWhiteSpace(TransferSearchVM.SKU))
            {
                Transferlist = Transferlist.Where(x => x.TransferSKU.Where(y => y.IsEnable && y.SkuNo == TransferSearchVM.SKU).Any());
            }
            if (!string.IsNullOrWhiteSpace(TransferSearchVM.Serial))
            {
                Transferlist = Transferlist.Where(x => x.TransferSKU.Where(y => y.IsEnable && y.SerialsLlist.Where(z => z.SerialsNo == TransferSearchVM.Serial).Any()).Any());
            }
            if (TransferSearchVM.From.HasValue)
            {
                Transferlist = Transferlist.Where(x => x.FromWID == TransferSearchVM.From);
            }
            if (TransferSearchVM.Interim.HasValue)
            {
                Transferlist = Transferlist.Where(x => x.Interim == TransferSearchVM.Interim);
            }
            if (TransferSearchVM.To.HasValue)
            {
                Transferlist = Transferlist.Where(x => x.ToWID == TransferSearchVM.To);
            }
            if (TransferSearchVM.Transfer.HasValue)
            {
                Transferlist = Transferlist.Where(x => x.ID == TransferSearchVM.Transfer);
            }
            if (!string.IsNullOrWhiteSpace(TransferSearchVM.ExternalTransfer))
            {
                Transferlist = Transferlist.Where(x => x.ExternalTra == TransferSearchVM.ExternalTransfer);
            }
            if (!string.IsNullOrWhiteSpace(TransferSearchVM.Title))
            {
                Transferlist = Transferlist.Where(x => x.Title == TransferSearchVM.Title);
            }
            if (!string.IsNullOrWhiteSpace(TransferSearchVM.Carrier))
            {
                Transferlist = Transferlist.Where(x => x.Carrier == TransferSearchVM.Carrier);
            }
            if (!string.IsNullOrWhiteSpace(TransferSearchVM.Tracking))
            {
                Transferlist = Transferlist.Where(x => x.Tracking == TransferSearchVM.Tracking);
            }
            TransferSearchVM.Transferlist = Transferlist;

            return View(TransferSearchVM);
        }
        public ActionResult Create()
        {

            Session["TSkuNumberList"] = null;
            return View();
        }
        [HttpPost]
        public ActionResult Create(Transfer Transfer)
        {
            var CreateBy = UserBy;
            var CreateAt = DateTime.UtcNow;
            Transfer.CreateBy = CreateBy;
            Transfer.CreateAt = CreateAt;
            var odataList = (List<TranSKUVM>)Session["TSkuNumberList"];
            if (odataList != null)
            {
                foreach (var item in odataList)
                {
                    Transfer.TransferSKU.Add(new TransferSKU { SkuNo = item.SKU, QTY = item.QTY, CreateBy = CreateBy, CreateAt = CreateAt });
                }
            }
            db.Transfer.Add(Transfer);
            db.SaveChanges();
            Session["TSkuNumberList"] = null;
            return RedirectToAction("Index");
        }
        public ActionResult Edit(int ID)
        {

            var Transfer = db.Transfer.Find(ID);
            var TranSKUVMList = new List<TranSKUVM>();
            foreach (var item in Transfer.TransferSKU)
            {
                TranSKUVMList.Add(new TranSKUVM
                {
                    ck = item.SkuNo,
                    sk = item.SkuNo,
                    SKU = item.SkuNo,
                    ProductName = item.Name,
                    QTY = item.QTY
                });
            }

            Session["TSkuNumberList"] = TranSKUVMList;
            return View(Transfer);
        }
        [HttpPost]
        public ActionResult Edit(Transfer Transfer, bool? saveexit)
        {
            var dt = DateTime.UtcNow;
            var oTransfer = db.Transfer.Find(Transfer.ID);
            oTransfer.ExternalTra = Transfer.ExternalTra;
            oTransfer.Title = Transfer.Title;
            oTransfer.FromWID = Transfer.FromWID;
            oTransfer.ToWID = Transfer.ToWID;
            oTransfer.Status = Transfer.Status;
            oTransfer.Interim = Transfer.Interim;
            oTransfer.Carrier = Transfer.Carrier;
            oTransfer.Tracking = Transfer.Tracking;
            oTransfer.UpdateBy = UserBy;
            oTransfer.UpdateAt = dt;

            var odataList = (List<TranSKUVM>)Session["TSkuNumberList"];

            foreach (var item in odataList.Where(x => x.Model == "E"))
            {
                var TransferSKUList = oTransfer.TransferSKU.Where(x => x.SkuNo == item.SKU);
                if (TransferSKUList.Any())
                {
                    foreach (var SKUitem in TransferSKUList)
                    {
                        if (SKUitem.QTY != item.QTY)
                        {
                            SKUitem.QTY = item.QTY;
                            SKUitem.UpdateBy = UserBy;
                            SKUitem.UpdateAt = dt;
                        }
                    }
                }
                else
                {
                    oTransfer.TransferSKU.Add(new TransferSKU { SkuNo = item.SKU, Name = item.ProductName, QTY = item.QTY, CreateBy = UserBy, CreateAt = dt });
                }

            }
            foreach (var item in odataList.Where(x => x.Model == "D"))
            {
                var TransferSKUList = oTransfer.TransferSKU.Where(x => x.SkuNo == item.SKU);
                if (TransferSKUList.Any())
                {
                    foreach (var SKUitem in TransferSKUList)
                    {
                        SKUitem.IsEnable = false;
                        SKUitem.UpdateBy = UserBy;
                        SKUitem.UpdateAt = dt;
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
                return RedirectToAction("Edit", new { Transfer.ID });
            }
        }
        public ActionResult GetSerialList(int ID)
        {
            var TransferSKU = db.TransferSKU.Find(ID);
            var SerialsLlist = TransferSKU.SerialsLlist.Where(x => x.SerialsQTY == 1 && x.SerialsLlistC.Count() == 0).Select(x => x.SerialsNo).ToList();
            var partial = ControlToString("~/Views/Transfer/GetSerialList.cshtml", SerialsLlist);
            //var partial = Engine.Razor.RunCompile(template, "templateKey", null, new { Name = "World" });
            return Json(new { status = true, partial }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Delete(int ID)
        {
            return View();
        }
        public ActionResult Receive(int ID)
        {
            var oTransfer = db.Transfer.Find(ID);
            var ReceiveVMList = new List<TransferItemVM>();
            foreach (var item in oTransfer.TransferSKU)
            {
                ReceiveVMList.Add(new TransferItemVM { SKU = item.SkuNo, Name = item.Name, QTY = item.QTY, SerialsLlist = item.SerialsLlist.Where(x => x.SerialsType == "TransferIn").ToList() });
            }
            Session["ReceiveVMList"] = ReceiveVMList;
            return View(oTransfer);
        }
        [HttpPost]
        public ActionResult Receive(int ID, bool? saveexit)
        {
            var CreateBy = UserBy;
            var CreateAt = DateTime.UtcNow;
            var ReceiveVMList = (List<TransferItemVM>)Session["ReceiveVMList"];
            var oTransfer = db.Transfer.Find(ID);
            foreach (var item in oTransfer.TransferSKU)
            {
                foreach (var ReceiveVM in ReceiveVMList.Where(x => x.SKU == item.SkuNo))
                {
                    var nSerialsLlist = ReceiveVM.SerialsLlist.Select(x => new SerialsLlist
                    {
                        TransferSKUID = item.ID,
                        PID = x.ID,
                        CreateAt = CreateAt,
                        CreateBy = CreateBy,
                        OrderID = x.OrderID,
                        PurchaseSKUID = x.PurchaseSKUID,
                        RMAID = x.RMAID,
                        SerialsNo = x.SerialsNo,
                        SerialsQTY = 1,
                        SerialsType = "TransferIn"
                    }).ToList();
                    //db.SerialsLlist.AddRange(nSerialsLlist);
                    foreach (var Serial in nSerialsLlist)
                    {
                        if (!item.SerialsLlist.Where(x => x.SerialsNo == Serial.SerialsNo && x.SerialsType == "TransferIn").Any())
                        {
                            item.SerialsLlist.Add(Serial);
                        }
                    }
                }
            }
            try
            {
                db.SaveChanges();
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
                return RedirectToAction("Receive", new { ID });
            }
        }
        public ActionResult Prep(int ID)
        {
            var oTransfer = db.Transfer.Find(ID);
            var PrepVMList = new List<TransferItemVM>();
            foreach (var item in oTransfer.TransferSKU)
            {
                PrepVMList.Add(new TransferItemVM { SKU = item.SkuNo, Name = item.Name, QTY = item.QTY, SerialsLlist = item.SerialsLlist.Where(x => x.SerialsType == "TransferOut").ToList() });
            }
            Session["PrepVMList"] = PrepVMList;
            return View(oTransfer);
        }

        [HttpPost]
        public ActionResult Prep(int ID, bool? saveexit)
        {
            var CreateBy = UserBy;
            var CreateAt = DateTime.UtcNow;
            var PrepVMList = (List<TransferItemVM>)Session["PrepVMList"];
            var oTransfer = db.Transfer.Find(ID);
            foreach (var item in oTransfer.TransferSKU)
            {
                foreach (var PrepVM in PrepVMList.Where(x => x.SKU == item.SkuNo))
                {
                    var nSerialsLlist = PrepVM.SerialsLlist.Select(x => new SerialsLlist
                    {
                        TransferSKUID= item.ID,
                        PID = x.ID,
                        CreateAt = CreateAt,
                        CreateBy = CreateBy,
                        OrderID = x.OrderID,
                        PurchaseSKUID = x.PurchaseSKUID,
                        RMAID = x.RMAID,
                        SerialsNo = x.SerialsNo,
                        SerialsQTY = -1,
                        SerialsType = "TransferOut"
                    }).ToList();
                    //db.SerialsLlist.AddRange(nSerialsLlist);
                    foreach (var Serial in nSerialsLlist)
                    {
                        if (!item.SerialsLlist.Where(x => x.SerialsNo == Serial.SerialsNo && x.SerialsType == "TransferOut").Any())
                        {
                            item.SerialsLlist.Add(Serial);
                        }
                    }
                }
            }
            try
            {
                db.SaveChanges();
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
                return RedirectToAction("Prep", new { ID });
            }
        }
        public ActionResult PrepSaveserials(string serials)
        {
            var PrepVMList = (List<TransferItemVM>)Session["PrepVMList"];

            var SerialsLlist = db.SerialsLlist.Where(x => x.SerialsNo == serials && !x.SerialsLlistC.Any() && x.SerialsQTY > 0);//找到序號
            if (SerialsLlist.Any())
            {
                if (SerialsLlist.Where(x => x.SerialsQTY > 0 && !x.SerialsLlistC.Any()).Any())
                {
                    var Serial = SerialsLlist.FirstOrDefault();
                    var PrepVM = PrepVMList.Where(x => x.SKU == Serial.PurchaseSKU.SkuNo).FirstOrDefault();
                    if (PrepVM != null)
                    {
                        if (PrepVM.QTY > PrepVM.SerialsLlist.Count())
                        {
                            if (!PrepVMList.Where(x => x.SerialsLlist.Where(y => y.SerialsNo == serials).Any()).Any())
                            {
                                PrepVM.SerialsLlist.Add(Serial);
                                Session["PrepVMList"] = PrepVMList;
                                return Json(new { status = true }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                return Json(new { status = false, Errmsg = "序號已在清單" }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            return Json(new { status = false, Errmsg = "移倉數量超過" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        return Json(new { status = false, Errmsg = "序號無對應的SKU" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { status = false, Errmsg = "序號不在倉庫，此序號不能移倉" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { status = false, Errmsg = "序號不存在，此序號不能移倉" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult ReceiveSaveserials(string serials)
        {
            var ReceiveVMList = (List<TransferItemVM>)Session["ReceiveVMList"];

            var SerialsLlist = db.SerialsLlist.Where(x => x.SerialsNo == serials && x.SerialsType == "TransferOut");//找到序號
            if (SerialsLlist.Any())
            {
                var Serial = SerialsLlist.FirstOrDefault();
                var ReceiveVM = ReceiveVMList.Where(x => x.SKU == Serial.TransferSKU.SkuNo).FirstOrDefault();
                if (ReceiveVM != null)
                {
                    if (ReceiveVM.QTY > ReceiveVM.SerialsLlist.Count())
                    {
                        if (!ReceiveVMList.Where(x => x.SerialsLlist.Where(y => y.SerialsNo == serials).Any()).Any())
                        {
                            ReceiveVM.SerialsLlist.Add(Serial);
                            Session["ReceiveVMList"] = ReceiveVMList;
                            return Json(new { status = true }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new { status = false, Errmsg = "序號已在清單" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        return Json(new { status = false, Errmsg = "移倉數量超過" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { status = false, Errmsg = "序號無對應的SKU" }, JsonRequestBehavior.AllowGet);
                }

            }
            else
            {
                return Json(new { status = false, Errmsg = "序號不存在，此序號不能移倉" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult PrepVMList()
        {
            var PrepVMList = (List<TransferItemVM>)Session["PrepVMList"];
            var PrepTableList = new List<PrepTable>();
            if (PrepVMList != null)
            {
                foreach (var item in PrepVMList)
                {
                    if (item.SerialsLlist.Any())
                    {
                        foreach (var SerialsLlistitem in item.SerialsLlist)
                        {
                            PrepTableList.Add(new PrepTable { SKU = item.SKU, Name = item.Name, Serial = SerialsLlistitem.SerialsNo, QTY = item.QTY + "/" + item.SerialsLlist.Count() });
                        }
                    }
                    else
                    {
                        PrepTableList.Add(new PrepTable { SKU = item.SKU, Name = item.Name, QTY = item.QTY + "/" + item.SerialsLlist.Count() });
                    }
                }
            }
            int recordsTotal = PrepTableList.Count();
            var returnObj =
           new
           {
               recordsFiltered = recordsTotal,
               data = PrepTableList//分頁後的資料 
           };
            return Json(returnObj, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ReceiveVMList()
        {
            var ReceiveVMList = (List<TransferItemVM>)Session["ReceiveVMList"];
            var PrepTableList = new List<PrepTable>();
            if (ReceiveVMList != null)
            {
                foreach (var item in ReceiveVMList)
                {
                    if (item.SerialsLlist.Any())
                    {
                        foreach (var SerialsLlistitem in item.SerialsLlist)
                        {
                            PrepTableList.Add(new PrepTable { SKU = item.SKU, Name = item.Name, Serial = SerialsLlistitem.SerialsNo, QTY = item.QTY + "/" + item.SerialsLlist.Count() });
                        }
                    }
                    else
                    {
                        PrepTableList.Add(new PrepTable { SKU = item.SKU, Name = item.Name, QTY = item.QTY + "/" + item.SerialsLlist.Count() });
                    }
                }
            }
            int recordsTotal = PrepTableList.Count();
            var returnObj =
           new
           {
               recordsFiltered = recordsTotal,
               data = PrepTableList//分頁後的資料 
           };
            return Json(returnObj, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Requst(int ID)
        {
            var Transfer = db.Transfer.Find(ID);
            Transfer.Status = "Requested";
            Transfer.UpdateBy = UserBy;
            Transfer.UpdateAt = DateTime.UtcNow;
            db.SaveChanges();

            return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Ship(int ID)
        {
          
            return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        }
        //public ActionResult GetShippingList(int WarehouseID)
        //{
        //    if (TempData["ShippingList"] != null)
        //    {
        //        TempData["ShippingList"] = new PurchaseOrderSys.Api.Shipping_API().ShippingList();
        //    }



        //    return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        //}
    }
}