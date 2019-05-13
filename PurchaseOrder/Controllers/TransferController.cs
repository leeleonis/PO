﻿using PurchaseOrderSys.Models;
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
            var SID = DateTime.Now.ToString("HHmmss");
            ViewBag.SID = SID;
            Session["TSkuNumberList" + SID] = null;
            return View();
        }
        [HttpPost]
        public ActionResult Create(Transfer Transfer, string SID)
        {
            var CreateBy = UserBy;
            var CreateAt = DateTime.UtcNow;
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
            foreach (var item in Transfer.TransferSKU.Where(x=>x.IsEnable))
            {
                TranSKUVMList.Add(new TranSKUVM
                {
                    ID = item.ID,
                    ck = item.SkuNo,
                    sk = item.SkuNo,
                    SKU = item.SkuNo,
                    ProductName = item.SKU.SkuLang.Where(x => x.LangID == "en-US").FirstOrDefault()?.Name,
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
                NoPrepSerials.Add(item.SKU + " " + item.ProductName + "X"+item.Prep);             
            }
            ViewBag.NoPrepSerials = NoPrepSerials;
            Session["TSkuNumberList" + SID] = TranSKUVMList;
            return View(Transfer);
        }

        private int? PrepSerialChk(TransferSKU item)
        {
            var tQTY = item.QTY;
            var SerialsLlist = item.SerialsLlist.Where(x => x.SerialsType == "TransferOut").Sum(x=>x.SerialsQTY);
            var RMASerialsLlist = item.RMASerialsLlist.Where(x => x.SerialsType == "TransferOut").Sum(x => x.SerialsQTY);
            return SerialsLlist + RMASerialsLlist - tQTY;


        }

        private string GetSerialMulti(TransferSKU item)
        {
            var SerialsLlist = item.SerialsLlist.Where(x => x.SerialsType == "TransferOut");
            var RMASerialsLlist = item.RMASerialsLlist.Where(x => x.SerialsType == "TransferOut");
            if (SerialsLlist.Any() && RMASerialsLlist.Any())
            {
                return "Multi";
            }
            else if (SerialsLlist.Any())
            {
                if (SerialsLlist.Count() > 1)
                {
                    return "Multi";
                }
                else
                {
                    return SerialsLlist.FirstOrDefault().SerialsNo;
                }
            }
            else if (RMASerialsLlist.Any())
            {
                if (RMASerialsLlist.Count() > 1)
                {
                    return "Multi";
                }
                else
                {
                    return RMASerialsLlist.FirstOrDefault().SerialsNo;
                }
            }
            else
            {
                return "";
            }
        }

        [HttpPost]
        public ActionResult Edit(Transfer Transfer, string SID, bool? saveexit)
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
                var TransferSKUList = oTransfer.TransferSKU.Where(x => x.IsEnable && x.SkuNo == item.SKU);
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
            var SerialsLlist = TransferSKU.SerialsLlist.Where(x => x.SerialsType == "TransferOut").Select(x => new { x.SerialsNo, Status = GetType(x) }).ToDictionary(x => x.SerialsNo, x => x.Status).ToList();

            SerialsLlist.AddRange(TransferSKU.RMASerialsLlist.Where(x => x.SerialsType == "TransferOut").Select(x => new { x.SerialsNo, Status = GetType(x) }).ToDictionary(x => x.SerialsNo, x => x.Status).ToList());
            var partial = ControlToString("~/Views/Transfer/GetSerialList.cshtml", SerialsLlist);
            //var partial = Engine.Razor.RunCompile(template, "templateKey", null, new { Name = "World" });
            return Json(new { status = true, partial }, JsonRequestBehavior.AllowGet);
        }

        private string GetType(SerialsLlist SerialsLlist)
        {
            var returnval = "";
            if (SerialsLlist.SerialsLlistC.Where(x => x.SerialsType == "TransferIn").Any())
            {
                returnval = "已入庫";
            }
            else if (SerialsLlist.TransferSKU.Transfer.Status == "Requested")
            {
                returnval = "待出庫";
            }
            else if (SerialsLlist.TransferSKU.Transfer.Status == "Shipped")
            {
                returnval = "已出庫";
            }
            return returnval;
        }
        private string GetType(RMASerialsLlist RMASerialsLlist)
        {
            var returnval = "";
            var SerialsNo = RMASerialsLlist.SerialsNo;
            if (RMASerialsLlist.TransferSKU.SerialsLlist.Where(x => x.SerialsType == "TransferIn" && x.SerialsNo== SerialsNo).Any())
            {
                returnval = "已入庫";
            }
            else if (RMASerialsLlist.TransferSKU.Transfer.Status == "Requested")
            {
                returnval = "待出庫";
            }
            else if (RMASerialsLlist.TransferSKU.Transfer.Status == "Shipped")
            {
                returnval = "已出庫";
            }
            return returnval;
        }



        public ActionResult Delete(int ID)
        {
            return View();
        }
        public ActionResult Receive(int ID)
        {
            var oTransfer = db.Transfer.Find(ID);
            var ReceiveVMList = new List<TransferItemVM>();
            foreach (var item in oTransfer.TransferSKU.Where(x=>x.IsEnable))
            {
                ReceiveVMList.Add(new TransferItemVM
                {
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
        public ActionResult Receive(int ID, bool? saveexit)
        {
            var CreateBy = UserBy;
            var CreateAt = DateTime.UtcNow;
            var ReceiveVMList = (List<TransferItemVM>)Session["ReceiveVMList" + ID];
            var oTransfer = db.Transfer.Find(ID);
            foreach (var item in oTransfer.TransferSKU.Where(x=>x.IsEnable))
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
            foreach (var item in oTransfer.TransferSKU.Where(x => x.IsEnable))
            {
                PrepVMList.Add(new TransferItemVM
                {
                    WarehouseID = oTransfer.FromWID,
                    SKU = item.SkuNo,
                    Name = item.Name,
                    QTY = item.QTY,
                    SerialsLlist = item.SerialsLlist.Where(x => x.SerialsType == "TransferOut").ToList(), //一般序號移倉
                    RMASerialsLlist = item.RMASerialsLlist.Where(x => x.SerialsType == "TransferOut").ToList() //RMA序號移倉
                });
            }
            Session["PrepVMList" + ID] = PrepVMList;
            return View(oTransfer);
        }

        [HttpPost]
        public ActionResult Prep(int ID, List<PostList> Prep, bool? saveexit)
        {
            var CreateBy = UserBy;
            var CreateAt = DateTime.UtcNow;
            var PrepVMList = (List<TransferItemVM>)Session["PrepVMList" + ID];
            var oTransfer = db.Transfer.Find(ID);
            foreach (var item in oTransfer.TransferSKU.Where(x=>x.IsEnable))
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

                //RMA
                foreach (var PrepVM in PrepVMList.Where(x => x.SKU == item.SkuNo))
                {
                    var nRMASerialsLlist = PrepVM.RMASerialsLlist.Select(x => new RMASerialsLlist
                    {
                        IsEnable = true,
                        TransferSKUID = item.ID,
                        PID = x.ID,
                        WarehouseID = x.WarehouseID,
                        CreateAt = CreateAt,
                        CreateBy = CreateBy,
                        UpdateAt = CreateAt,
                        UpdateBy = CreateBy,
                        //OrderID = x.OrderID,
                        RMASKUID = x.RMASKUID,
                        //RMAID = x.RMAID,
                        NewSkuNo = x.NewSkuNo,
                        SerialsNo = x.SerialsNo,
                        SerialsQTY = -1,
                        SerialsType = "TransferOut"//等待移倉
                    }).ToList();
                    //db.SerialsLlist.AddRange(nSerialsLlist);
                    foreach (var Serial in nRMASerialsLlist)
                    {
                        if (!item.RMASerialsLlist.Where(x => x.SerialsNo == Serial.SerialsNo).Any())
                        {
                            item.RMASerialsLlist.Add(Serial);
                        }
                    }
                }
                //無序號
                //var TransferOutCount = item.SerialsLlist.Where(x => x.SerialsType == "TransferOut").Count();
                //foreach (var Prepitem in Prep)
                //{
                //    var PrepCount = 0;
                //    if (int.TryParse(Prepitem.val, out PrepCount))
                //    {
                //        if (PrepCount > TransferOutCount)
                //        {
                //            var TransferOutlist = db.SerialsLlist.Where(x => !x.SerialsLlistC.Any() && x.PurchaseSKU.IsEnable && x.PurchaseSKU.PurchaseOrder.IsEnable && x.PurchaseSKU.PurchaseOrder.WarehouseID == oTransfer.FromWID).Take(PrepCount - TransferOutCount).ToList();
                //            var nSerialsLlist = TransferOutlist.Select(x => new SerialsLlist
                //            {
                //                TransferSKUID = item.ID,
                //                PID = x.ID,
                //                CreateAt = CreateAt,
                //                CreateBy = CreateBy,
                //                UpdateAt = CreateAt,
                //                UpdateBy = CreateBy,
                //                OrderID = x.OrderID,
                //                PurchaseSKUID = x.PurchaseSKUID,
                //                RMAID = x.RMAID,
                //                SerialsNo = x.SerialsNo,
                //                SerialsQTY = -1,
                //                SerialsType = "TransferOut"//等待移倉
                //            }).ToList();
                //            foreach (var Serial in nSerialsLlist)
                //            {
                //                if (!item.SerialsLlist.Where(x => x.SerialsNo == Serial.SerialsNo).Any())
                //                {
                //                    item.SerialsLlist.Add(Serial);
                //                }
                //            }
                //        }
                //    }
                //}
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
        public ActionResult PrepSaveserials(string serials, string SID)
        {
            var PrepVMList = (List<TransferItemVM>)Session["PrepVMList" + SID];
            var WarehouseID = PrepVMList.FirstOrDefault().WarehouseID;
            var SerialsLlist = db.SerialsLlist.Where(x => x.SerialsNo == serials && !x.SerialsLlistC.Any() && x.SerialsQTY > 0);//找到序號
            SerialsLlist = SerialsLlist.Where(x => (x.TransferSKUID.HasValue && x.TransferSKU.Transfer.ToWID == WarehouseID) || (x.PurchaseSKUID.HasValue && x.PurchaseSKU.PurchaseOrder.WarehouseID == WarehouseID));
            var RMASerialsLlist = db.RMASerialsLlist.Where(x => x.SerialsNo == serials && !x.RMASerialsLlistC.Any() && x.SerialsQTY > 0&&x.WarehouseID== WarehouseID);//找到RMA序號
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

                    var PrepVM = PrepVMList.Where(x => x.SKU == SkuNo).FirstOrDefault();
                    if (PrepVM != null)
                    {
                        if (PrepVM.QTY > PrepVM.SerialsLlist.Count() + PrepVM.RMASerialsLlist.Count())
                        {
                            if (!PrepVMList.Where(x => x.SerialsLlist.Where(y => y.SerialsNo == serials).Any()).Any()&& !PrepVMList.Where(x => x.RMASerialsLlist.Where(y => y.SerialsNo == serials).Any()).Any())
                            {
                                PrepVM.SerialsLlist.Add(Serial);
                                Session["PrepVMList" + SID] = PrepVMList;
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
                                    Session["PrepVMList" + SID] = PrepVMList;
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
        public ActionResult ReceiveSaveserials(int id, string serials)
        {
            var SID = id;
            var ReceiveVMList = (List<TransferItemVM>)Session["ReceiveVMList" + SID];
            var SerialsLlist = db.SerialsLlist.Where(x => x.TransferSKU.TransferID == id && x.SerialsNo == serials && !x.SerialsLlistC.Any());
            var RMASerialsLlist = db.RMASerialsLlist.Where(x => x.TransferSKU.TransferID == id && x.SerialsNo == serials && !x.RMASerialsLlistC.Any());
            if (SerialsLlist.Sum(x => x.SerialsQTY) > 0)//檔序號數量>0
            {
                var SkuNolist = ReceiveVMList.Select(x => x.SKU).Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();
                if (SerialsLlist.Where(x => SkuNolist.Contains(x.TransferSKU.SkuNo)).Any())//檢查是否存在相同的sku而且序號相同
                {
                    return Json(new { status = false, Errmsg = "序號已存在，不能重複入庫" }, JsonRequestBehavior.AllowGet);
                }
            }

            var SerialsLlistGET = SerialsLlist.Where(x => x.SerialsType == "TransferOut");//找到序號
            var RMASerialsLlistGET = RMASerialsLlist.Where(x => x.SerialsType == "TransferOut");//找到序號
            if (SerialsLlistGET.Any())
            {
                var Serial = SerialsLlistGET.FirstOrDefault();
                var ReceiveVM = ReceiveVMList.Where(x => x.SKU == Serial.TransferSKU.SkuNo).FirstOrDefault();
                if (ReceiveVM != null)
                {
                    if (ReceiveVM.QTY > ReceiveVM.SerialsLlist.Count())
                    {
                        if (!ReceiveVMList.Where(x => x.SerialsLlist.Where(y => y.SerialsNo == serials).Any()).Any())
                        {
                            ReceiveVM.SerialsLlist.Add(Serial);
                            Session["ReceiveVMList" + SID] = ReceiveVMList;
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
            else if (RMASerialsLlistGET.Any())
            {
                var RMASerial = RMASerialsLlistGET.FirstOrDefault();
                var ReceiveVM = ReceiveVMList.Where(x => x.SKU == RMASerial.TransferSKU.SkuNo).FirstOrDefault();
                if (ReceiveVM != null)
                {
                    if (ReceiveVM.QTY > ReceiveVM.SerialsLlist.Count())
                    {
                        if (!ReceiveVMList.Where(x => x.SerialsLlist.Where(y => y.SerialsNo == serials).Any()).Any())
                        {
                            ReceiveVM.RMASerialsLlist.Add(RMASerial);
                            Session["ReceiveVMList" + SID] = ReceiveVMList;
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
        public ActionResult PrepVMList(string SID)
        {
            var PrepVMList = (List<TransferItemVM>)Session["PrepVMList"+ SID];
            var PrepTableList = new List<PrepTable>();
            if (PrepVMList != null)
            {
                foreach (var item in PrepVMList)
                {
                    if (item.SerialsLlist.Any()|| item.RMASerialsLlist.Any()) 
                    {
                        foreach (var SerialsLlistitem in item.SerialsLlist)//一般
                        {
                            PrepTableList.Add(new PrepTable { SKU = item.SKU, Name = item.Name, Serial = SerialsLlistitem.SerialsNo, QTY = (item.SerialsLlist.Count() + item.RMASerialsLlist.Count()) + "/" + item.QTY, SerialTracking = GetSerialTracking(item.SKU) });
                        }
                   
                        foreach (var RMASerialsLlistitem in item.RMASerialsLlist)//RMA
                        {
                            PrepTableList.Add(new PrepTable { SKU = item.SKU, Name = item.Name, Serial = RMASerialsLlistitem.SerialsNo, QTY =  (item.SerialsLlist.Count() + item.RMASerialsLlist.Count()) + "/" + item.QTY, SerialTracking = GetSerialTracking(item.SKU) });
                        }
                    }
                    else
                    {
                        PrepTableList.Add(new PrepTable { SKU = item.SKU, Name = item.Name, QTY = (item.SerialsLlist.Count() + item.RMASerialsLlist.Count()) + "/" + item.QTY, SerialTracking = GetSerialTracking(item.SKU) });
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
        public ActionResult ReceiveVMList(string SID)
        {
            var ReceiveVMList = (List<TransferItemVM>)Session["ReceiveVMList" + SID];
            var PrepTableList = new List<PrepTable>();
            if (ReceiveVMList != null)
            {
                foreach (var item in ReceiveVMList)
                {
                    if (item.SerialsLlist.Any()|| item.RMASerialsLlist.Any())
                    {
                        foreach (var SerialsLlistitem in item.SerialsLlist)
                        {
                            PrepTableList.Add(new PrepTable { SKU = item.SKU, Name = item.Name, Serial = SerialsLlistitem.SerialsNo, QTY = item.SerialsLlist.Count() + "/" + item.QTY, SerialTracking = GetSerialTracking(item.SKU), count = (item.QTY - item.SerialsLlist.Count()) });
                        }
                        foreach (var RMASerialsLlistitem in item.RMASerialsLlist)
                        {
                            PrepTableList.Add(new PrepTable { SKU = item.SKU, Name = item.Name, Serial = RMASerialsLlistitem.SerialsNo, QTY = item.RMASerialsLlist.Count() + "/" + item.QTY, SerialTracking = GetSerialTracking(item.SKU), count = (item.QTY - item.RMASerialsLlist.Count()) });
                        }
                    }
                    else
                    {
                        PrepTableList.Add(new PrepTable { SKU = item.SKU, Name = item.Name, QTY = item.SerialsLlist.Count() + "/" + item.QTY, SerialTracking = GetSerialTracking(item.SKU), count = (item.QTY - item.SerialsLlist.Count()) });
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

        private bool GetSerialTracking(string SKU)
        {
            return db.SKU.Find(SKU).SerialTracking;
        }

        public ActionResult Requested(int ID)
        {
            var Transfer = db.Transfer.Find(ID);
            if (Transfer.Status == "Pending")
            {
                Transfer.Status = "Requested";
            }
            else
            {
                if (Transfer.TransferSKU.Where(x=>x.SerialsLlist.Any()).Any())
                {
                    return Json(new { status = false, Errmsg = "已輸入序號，無法修改" }, JsonRequestBehavior.AllowGet);
                }
                Transfer.Status = "Pending";
            }
           
            Transfer.UpdateBy = UserBy;
            Transfer.UpdateAt = DateTime.UtcNow;
            db.SaveChanges();

            return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Ship(int ID)
        {
            var Transfer= db.Transfer.Find(ID);
            Transfer.Status = "Shipped";
            Transfer.UpdateBy = UserBy;
            Transfer.UpdateAt = DateTime.UtcNow;
            db.SaveChanges();

            return RedirectToAction("Edit", new { ID });
            //return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Received(int ID)
        {
            var Transfer = db.Transfer.Find(ID);
            Transfer.Status = "Received";
            Transfer.UpdateBy = UserBy;
            Transfer.UpdateAt = DateTime.UtcNow;
            db.SaveChanges();
            return RedirectToAction("Edit", new { ID });
            //return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Completed(int ID)
        {
            var Transfer = db.Transfer.Find(ID);
            Transfer.Status = "Completed";
            Transfer.UpdateBy = UserBy;
            Transfer.UpdateAt = DateTime.UtcNow;
            db.SaveChanges();

            return RedirectToAction("Edit", new { ID });
            //return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult RemoveData(string[] IDList, string SID)
        {
            var Errmsg = "";
            if (IDList != null && IDList.Any())
            {
                var odataList = (List<TranSKUVM>)Session["TSkuNumberList" + SID];
                foreach (var item in IDList)
                {
                    foreach (var odataListitem in odataList.Where(x => x.ID.ToString() == item || x.SKU == item))
                    {
                        if (odataListitem.ID.HasValue)
                        {
                            var TransferSKU = db.TransferSKU.Find(odataListitem.ID);
                            if (TransferSKU.SerialsLlist.Any())
                            {
                                Errmsg += "【" + TransferSKU.SkuNo + "】已有序號不能刪除；";
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
                Session["TSkuNumberList" + SID] = odataList;
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