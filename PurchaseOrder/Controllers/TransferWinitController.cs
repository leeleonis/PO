using PurchaseOrderSys.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PurchaseOrderSys.Controllers
{
    public class TransferWinitController : BaseController
    {
        // GET: TransferWinit
        public ActionResult Index(TransferSearchVM TransferSearchVM)
        {
            var Transferlist = db.Transfer.Where(x => x.IsEnable && x.TransferType == "Winit");
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
            var SID = DateTime.Now.ToString("HHmmssfff");
            ViewBag.SID = SID;
            Session["TSkuNumberList" + SID] = null;
            return View();
        }
        [HttpPost]
        public ActionResult Create(Transfer Transfer, string SID)
        {
            var CreateBy = UserBy;
            var CreateAt = DateTime.UtcNow;
            Transfer.TransferType = "Winit";
            Transfer.CreateBy = CreateBy;
            Transfer.CreateAt = CreateAt;
            var odataList = (List<TranSKUVM>)Session["TSkuNumberList" + SID];
            if (odataList != null)
            {
                foreach (var item in odataList.Where(x => x.Model == "E"))
                {
                    Transfer.TransferSKU.Add(new TransferSKU { IsEnable = true, SkuNo = item.SKU, Name = item.ProductName, QTY = item.QTY, CreateBy = CreateBy, CreateAt = CreateAt });
                }
            }
            db.Transfer.Add(Transfer);
            db.SaveChanges();
            Session["TSkuNumberList" + SID] = null;
            return RedirectToAction("Edit", new { Transfer.ID });
        }
        public ActionResult Edit(int ID)
        {
            var NoPrepSerials = new List<string>();
            var SID = ID;
            var Transfer = db.Transfer.Find(ID);
            var TranSKUVMList = new List<TranSKUVM>();
            foreach (var item in Transfer.TransferSKU.Where(x => x.IsEnable))
            {
                TranSKUVMList.Add(new TranSKUVM
                {
                    ID = item.ID,
                    ck = item.ID,
                    sk = item.SkuNo,
                    SKU = item.SkuNo,
                    ProductName = item.SKU.SkuLang.Where(x => x.LangID == LangID).FirstOrDefault()?.Name,
                    QTY = item.QTY,
                    TotalReceive = item.SerialsLlist.Where(x => x.SerialsType == "TransferIn").Sum(x => x.SerialsQTY),
                    Serial = GetSerialMulti(item),
                    TWN = item.Transfer.WarehouseFrom?.Name,
                    Winit = item.Transfer.WarehouseTo?.Name,
                    Model = "L",
                    Prep = PrepSerialChk(item)
                });
            }
            foreach (var item in TranSKUVMList.Where(x => x.Prep != 0))
            {
                NoPrepSerials.Add(item.SKU + " " + item.ProductName + "X" + item.Prep);
            }
            ViewBag.NoPrepSerials = NoPrepSerials;
            Session["TSkuNumberList" + SID] = TranSKUVMList;
            return View(Transfer);
        }
        [HttpPost]
        public ActionResult Edit(Transfer Transfer, string SID, bool? saveexit, bool? Requestedval)
        {
            var dt = DateTime.UtcNow;
            var oTransfer = db.Transfer.Find(Transfer.ID);
            if (!string.IsNullOrWhiteSpace(Transfer.ExternalTra)) oTransfer.ExternalTra = Transfer.ExternalTra;
            if (!string.IsNullOrWhiteSpace(Transfer.Title)) oTransfer.Title = Transfer.Title;
            if (Transfer.FromWID.HasValue) oTransfer.FromWID = Transfer.FromWID;
            if (Transfer.ToWID.HasValue) oTransfer.ToWID = Transfer.ToWID;
            if (!string.IsNullOrWhiteSpace(Transfer.Status)) oTransfer.Status = Transfer.Status;
            if (Transfer.Interim.HasValue) oTransfer.Interim = Transfer.Interim;
            if (!string.IsNullOrWhiteSpace(Transfer.Carrier)) oTransfer.Carrier = Transfer.Carrier;
            if (!string.IsNullOrWhiteSpace(Transfer.Tracking)) oTransfer.Tracking = Transfer.Tracking;


            oTransfer.UpdateBy = UserBy;
            oTransfer.UpdateAt = dt;

            var odataList = (List<TranSKUVM>)Session["TSkuNumberList" + SID];

            foreach (var item in odataList.Where(x => x.Model == "E"))
            {
                var TransferSKUList = oTransfer.TransferSKU.Where(x => x.IsEnable && x.ID == item.ID);
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
                    oTransfer.TransferSKU.Add(new TransferSKU { IsEnable = true, SkuNo = item.SKU, Name = item.ProductName, QTY = item.QTY, CreateBy = UserBy, CreateAt = dt });
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

            if (Requestedval.HasValue && Requestedval.Value)
            {
                if (oTransfer.Status == "Pending" && oTransfer.Status != oTransfer.Status)
                {
                    oTransfer.Status = "Requested";
                    //Winit API 取S BARCODE

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
        public ActionResult Prep(int ID)
        {
            //var WinitPrepTable = new WinitPrepTable { ID = ID, BoxID = "testBOXID" };
            var oTransfer = db.Transfer.Find(ID);
            var PrepVMList = new List<TransferItemVM>();
            foreach (var item in oTransfer.TransferSKU.Where(x => x.IsEnable))
            {
                PrepVMList.Add(new TransferItemVM
                {
                    ID = item.ID,
                    WarehouseID = oTransfer.FromWID,
                    SKU = item.SkuNo,
                    Name = item.Name,
                    QTY = item.QTY,
                    SerialsLlist = item.SerialsLlist.Where(x => x.SerialsType == "TransferOut").ToList(), //一般序號移倉
                    RMASerialsLlist = item.RMASerialsLlist.Where(x => x.SerialsType == "TransferOut").ToList() //RMA序號移倉
                });
            }
            Session["WinitPrepVMList" + ID] = PrepVMList;
            if (oTransfer.WinitTransfer == null)
            {
                var CreateBy = UserBy;
                var CreateAt = DateTime.UtcNow;
                var WinitTransfer = new WinitTransfer { IsEnable = true, CreateBy = CreateBy, CreateAt = CreateAt };
                WinitTransfer.WinitTransferBox.Add(new WinitTransferBox { IsEnable = true, CreateBy = CreateBy, CreateAt = CreateAt });
                oTransfer.WinitTransfer = WinitTransfer;
            }
            Session["WinitTransferBox" + ID] = oTransfer.WinitTransfer.WinitTransferBox.ToList();
            return View(oTransfer);
        }

        [HttpPost]
        public ActionResult Prep(int ID, List<PostList> Prep, bool? saveexit)
        {
            Prep = Prep.Where(x => !string.IsNullOrWhiteSpace(x.val)).Distinct().ToList();
            var CreateBy = UserBy;
            var CreateAt = DateTime.UtcNow;
            var PrepVMList = (List<TransferItemVM>)Session["WinitPrepVMList" + ID];
            var oTransfer = db.Transfer.Find(ID);
            foreach (var item in oTransfer.TransferSKU.Where(x => x.IsEnable))
            {
                //一般
                foreach (var PrepVM in PrepVMList.Where(x => x.SKU == item.SkuNo))
                {
                    var nSerialsLlist = PrepVM.SerialsLlist.Select(x => new SerialsLlist
                    {
                        IsEnable = true,
                        TransferSKUID = item.ID,
                        PID = x.ID,
                        CreateAt = CreateAt,
                        CreateBy = CreateBy,
                        UpdateAt = CreateAt,
                        UpdateBy = CreateBy,
                        //OrderID = x.OrderID,
                        PurchaseSKUID = x.PurchaseSKUID,
                        //RMAID = x.RMAID,
                        SerialsNo = x.SerialsNo,
                        SerialsQTY = -1,
                        SerialsType = "TransferOut"//等待移倉
                    }).ToList();
                    //db.SerialsLlist.AddRange(nSerialsLlist);
                    foreach (var Serial in nSerialsLlist)
                    {
                        if (!item.SerialsLlist.Where(x => x.SerialsNo == Serial.SerialsNo).Any())
                        {
                            item.SerialsLlist.Add(Serial);
                        }
                    }
                }
                //無序號
                if (!item.SKU.SerialTracking)//不用序號
                {
                    var TransferOutCount = item.SerialsLlist.Where(x => x.SerialsType == "TransferOut").Count();
                    foreach (var Prepitem in Prep.Where(x => x.ID == item.ID))
                    {
                        var PrepCount = 0;
                        if (int.TryParse(Prepitem.val, out PrepCount))
                        {
                            if (PrepCount > item.QTY)
                            {
                                PrepCount = item.QTY.Value;
                            }
                            if (PrepCount > TransferOutCount)
                            {
                                var SerialsType = new List<string> { "PO", "TransferIn" };
                                var TransferOutlist = db.SerialsLlist.Where(x => !x.SerialsLlistC.Any() && x.PurchaseSKU.IsEnable && x.PurchaseSKU.PurchaseOrder.IsEnable && x.PurchaseSKU.PurchaseOrder.WarehouseID == oTransfer.FromWID && x.PurchaseSKU.SkuNo == item.SkuNo && SerialsType.Contains(x.SerialsType)).Take(PrepCount - TransferOutCount).ToList();
                                var nSerialsLlist = TransferOutlist.Select(x => new SerialsLlist
                                {
                                    IsEnable = true,
                                    TransferSKUID = item.ID,
                                    PID = x.ID,
                                    CreateAt = CreateAt,
                                    CreateBy = CreateBy,
                                    UpdateAt = CreateAt,
                                    UpdateBy = CreateBy,
                                    OrderID = x.OrderID,
                                    PurchaseSKUID = x.PurchaseSKUID,
                                    SerialsNo = x.SerialsNo,
                                    SerialsQTY = -1,
                                    SerialsType = "TransferOut"//等待移倉
                                }).ToList();
                                foreach (var Serial in nSerialsLlist)
                                {
                                    if (!item.SerialsLlist.Where(x => x.SerialsNo == Serial.SerialsNo).Any())
                                    {
                                        item.SerialsLlist.Add(Serial);
                                    }
                                }
                            }
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
                return RedirectToAction("Edit", new { ID });
            }
            else
            {
                return RedirectToAction("Prep", new { ID });
            }
        }
        public ActionResult Receive(int ID)
        {
            var oTransfer = db.Transfer.Find(ID);
            var ReceiveVMList = new List<TransferItemVM>();
            foreach (var item in oTransfer.TransferSKU.Where(x => x.IsEnable))
            {
                ReceiveVMList.Add(new TransferItemVM
                {
                    ID = item.ID,
                    SKU = item.SkuNo,
                    Name = item.Name,
                    QTY = item.QTY,
                    SerialsLlist = item.SerialsLlist.Where(x => x.SerialsType == "TransferIn").ToList(),//一般
                    RMASerialsLlist = item.RMASerialsLlist.Where(x => x.SerialsType == "TransferIn").ToList()//RMA
                });
            }
            Session["ReceiveVMList" + ID] = ReceiveVMList;
            return View(oTransfer);
        }
        [HttpPost]
        public ActionResult Receive(int ID, List<PostList> Prep, bool? saveexit)
        {
            Prep = Prep.Where(x => !string.IsNullOrWhiteSpace(x.val)).Distinct().ToList();
            var CreateBy = UserBy;
            var CreateAt = DateTime.UtcNow;
            var ReceiveVMList = (List<TransferItemVM>)Session["ReceiveVMList" + ID];
            var oTransfer = db.Transfer.Find(ID);
            foreach (var item in oTransfer.TransferSKU.Where(x => x.IsEnable))
            {
                foreach (var ReceiveVM in ReceiveVMList.Where(x => x.SKU == item.SkuNo))
                {
                    if (oTransfer.Status != "Received")
                    {
                        oTransfer.Status = "Received";
                        oTransfer.UpdateAt = CreateAt;
                        oTransfer.UpdateBy = CreateBy;
                    }
                    //一般
                    var nSerialsLlist = ReceiveVM.SerialsLlist.Select(x => new SerialsLlist
                    {
                        IsEnable = true,
                        TransferSKUID = item.ID,
                        PID = x.ID,
                        CreateAt = CreateAt,
                        CreateBy = CreateBy,
                        ReceivedAt = CreateAt,
                        ReceivedBy = CreateBy,
                        //OrderID = x.OrderID,
                        PurchaseSKUID = x.PurchaseSKUID,
                        //RMAID = x.RMAID,
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

                    //RMA
                    var nRMASerialsLlist = ReceiveVM.RMASerialsLlist.Select(x => new SerialsLlist
                    {
                        IsEnable = true,
                        TransferSKUID = item.ID,
                        PID = x.ID,
                        CreateAt = CreateAt,
                        CreateBy = CreateBy,
                        ReceivedAt = CreateAt,
                        ReceivedBy = CreateBy,
                        //OrderID = x.OrderID,
                        //PurchaseSKUID = x.PurchaseSKUID,
                        //RMAID = x.RMAID,
                        SerialsNo = x.SerialsNo,
                        SerialsQTY = 1,
                        SerialsType = "TransferIn"
                    }).ToList();
                    //db.SerialsLlist.AddRange(nSerialsLlist);
                    foreach (var Serial in nRMASerialsLlist)
                    {
                        if (!item.SerialsLlist.Where(x => x.SerialsNo == Serial.SerialsNo && x.SerialsType == "TransferIn").Any())
                        {
                            item.SerialsLlist.Add(Serial);
                        }
                    }
                }
                //無序號
                if (!item.SKU.SerialTracking)//不用序號
                {
                    var TransferOutCount = item.SerialsLlist.Where(x => x.SerialsType == "TransferIn").Count();
                    foreach (var Prepitem in Prep.Where(x => x.ID == item.ID))
                    {
                        var PrepCount = 0;
                        if (int.TryParse(Prepitem.val, out PrepCount))
                        {
                            if (PrepCount > item.QTY)
                            {
                                PrepCount = item.QTY.Value;
                            }
                            if (PrepCount > TransferOutCount)
                            {
                                var TransferOutlist = db.SerialsLlist.Where(x => !x.SerialsLlistC.Any() && x.PurchaseSKU.IsEnable && x.PurchaseSKU.PurchaseOrder.IsEnable && x.PurchaseSKU.PurchaseOrder.WarehouseID == oTransfer.FromWID && x.PurchaseSKU.SkuNo == item.SkuNo && x.SerialsType == "TransferOut").Take(PrepCount - TransferOutCount).ToList();
                                foreach (var SerialOut in TransferOutlist)
                                {
                                    SerialOut.SerialsLlistC.Add(new SerialsLlist
                                    {
                                        IsEnable = true,
                                        TransferSKUID = item.ID,
                                        PID = SerialOut.ID,
                                        CreateAt = CreateAt,
                                        CreateBy = CreateBy,
                                        ReceivedAt = CreateAt,
                                        ReceivedBy = CreateBy,
                                        //OrderID = x.OrderID,
                                        //PurchaseSKUID = x.PurchaseSKUID,
                                        //RMAID = x.RMAID,
                                        SerialsNo = SerialOut.SerialsNo,
                                        SerialsQTY = 1,
                                        SerialsType = "TransferIn"
                                    });
                                }
                            }
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
        public ActionResult PrepSaveserials(string serials, string SID,int boxitemset)
        {
            var PrepVMList = (List<TransferItemVM>)Session["WinitPrepVMList" + SID];
            var WinitTransferBoxList = (List<WinitTransferBox>)Session["WinitTransferBox" + SID];
            var WarehouseID = PrepVMList.FirstOrDefault().WarehouseID;
            var SerialsLlist = db.SerialsLlist.Where(x => x.SerialsNo == serials && !x.SerialsLlistC.Any() && x.SerialsQTY > 0);//找到序號
            SerialsLlist = SerialsLlist.Where(x => (x.TransferSKUID.HasValue && x.TransferSKU.Transfer.ToWID == WarehouseID) || (x.PurchaseSKUID.HasValue && x.PurchaseSKU.PurchaseOrder.WarehouseID == WarehouseID));
            var RMASerialsLlist = db.RMASerialsLlist.Where(x => x.SerialsNo == serials && !x.RMASerialsLlistC.Any() && x.SerialsQTY > 0 && x.WarehouseID == WarehouseID);//找到RMA序號
            if (SerialsLlist.Any())
            {
                if (SerialsLlist.Where(x => x.SerialsQTY > 0 && !x.SerialsLlistC.Any()).Any())
                {
                    var Serial = SerialsLlist.FirstOrDefault();
                    var SkuNo = "";
                    if (Serial.PurchaseSKUID.HasValue)
                    {
                        SkuNo = Serial.PurchaseSKU.SkuNo;
                    }
                    else if (Serial.TransferSKUID.HasValue)
                    {
                        SkuNo = Serial.TransferSKU.SkuNo;
                    }

                    var PrepVM = PrepVMList.Where(x => x.SKU == SkuNo && x.QTY > x.SerialsLlist.Count()).FirstOrDefault();
                    if (PrepVM != null)
                    {
                        if (PrepVM.QTY > PrepVM.SerialsLlist.Count() + PrepVM.RMASerialsLlist.Count())
                        {
                            if (!PrepVMList.Where(x => x.SerialsLlist.Where(y => y.SerialsNo == serials).Any()).Any() && !PrepVMList.Where(x => x.RMASerialsLlist.Where(y => y.SerialsNo == serials).Any()).Any())
                            {
                                PrepVM.SerialsLlist.Add(Serial);
                                var WinitTransferBox = WinitTransferBoxList.Skip(boxitemset).Take(1).FirstOrDefault();
                                WinitTransferBox.WinitTransferBoxItem.Add(new WinitTransferBoxItem
                                {
                                    SerialsLlistID = Serial.ID,
                                    SerialsNo= Serial.SerialsNo,
                                    SkuNo = Serial.PurchaseSKU.SkuNo,
                                    Name = Serial.PurchaseSKU.Name,
                                    Weight = Serial.PurchaseSKU.SKU.Logistic?.ShippingWeight ?? 0
                                });
                                Session["WinitPrepVMList" + SID] = PrepVMList;
                                Session["WinitTransferBox" + SID] = WinitTransferBoxList;
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
            else if (RMASerialsLlist.Any())
            {
                if (RMASerialsLlist.Where(x => x.SerialsQTY > 0 && !x.RMASerialsLlistC.Any()).Any())
                {
                    var RMASerial = RMASerialsLlist.FirstOrDefault();
                    var RMAPrepVM = PrepVMList.Where(x => x.SKU == RMASerial.RMASKU.SkuNo || x.SKU == RMASerial.NewSkuNo).FirstOrDefault();
                    try
                    {
                        if (RMAPrepVM != null)
                        {
                            if (RMAPrepVM.QTY > RMAPrepVM.SerialsLlist.Count() + RMAPrepVM.RMASerialsLlist.Count())
                            {
                                if (!PrepVMList.Where(x => x.SerialsLlist.Where(y => y.SerialsNo == serials).Any()).Any() && !PrepVMList.Where(x => x.RMASerialsLlist.Where(y => y.SerialsNo == serials).Any()).Any())
                                {
                                    RMAPrepVM.RMASerialsLlist.Add(RMASerial);
                                    Session["WinitPrepVMList" + SID] = PrepVMList;
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
                    catch (Exception ex)
                    {

                        throw;
                    }
                }
                else
                {
                    return Json(new { status = false, Errmsg = "序號不在倉庫，此序號不能移倉" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { status = false, Errmsg = "序號不存在或是已在清單" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult AddBox(int ID)
        {
            var WinitTransferBoxList = (List<WinitTransferBox>)Session["WinitTransferBox" + ID];
            WinitTransferBoxList.Add(new WinitTransferBox());
            Session["WinitTransferBox" + ID] = WinitTransferBoxList;
            return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult BoxValChange(int ID,string name,string val)
        {
            var index = 0;
            var boxNo = 0;
            var newval = 0m;
            var keylist = name.Split('_');
            var WinitTransferBoxList = (List<WinitTransferBox>)Session["WinitTransferBox" + ID];
            int.TryParse(keylist[1], out boxNo);
            decimal.TryParse(val, out newval);
            foreach (var item in WinitTransferBoxList)
            {
                if (index == boxNo)
                {
                    switch (keylist[0])
                    {
                        case "Length":
                            item.Length = newval;
                            break;
                        case "Width":
                            item.Width = newval;
                            break;
                        case "Heigth":
                            item.Heigth = newval;
                            break;
                        default:
                            break;
                    }
                }
                index++;
            }
            Session["WinitTransferBox" + ID] = WinitTransferBoxList;
            return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult BoxChange(int ID)
        {
            var WinitTransferBoxList = (List<WinitTransferBox>)Session["WinitTransferBox" + ID];
            var html = RenderPartialViewToString("Boxitem", WinitTransferBoxList);
            var set = WinitTransferBoxList.Count() - 1;
            return Json(new { status = true, html, set }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DelSerialsNo(string serial, int ID)
        {
            var WinitTransferBoxList = (List<WinitTransferBox>)Session["WinitTransferBox" + ID];
            var PrepVMList = (List<TransferItemVM>)Session["WinitPrepVMList" + ID];
            foreach (var Boxitem in WinitTransferBoxList)
            {
                foreach (var item in Boxitem.WinitTransferBoxItem.ToList())
                {
                    if (item.SerialsNo.Trim() == serial.Trim())
                    {
                        Boxitem.WinitTransferBoxItem.Remove(item);
                    }
                }
            }
            foreach (var Prepitem in PrepVMList)
            {
                foreach (var item in Prepitem.SerialsLlist.ToList())
                {
                    if (item.SerialsNo.Trim() == serial.Trim())
                    {
                        Prepitem.SerialsLlist.Remove(item);
                    }
                }
            }
            Session["WinitTransferBox" + ID] = WinitTransferBoxList;
            Session["WinitPrepVMList" + ID] = PrepVMList;
            return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        }

    }
}