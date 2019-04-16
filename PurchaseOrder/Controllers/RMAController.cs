﻿using Newtonsoft.Json;
using PurchaseOrderSys.Models;
using SellerCloud_WebService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace PurchaseOrderSys.Controllers
{
    public class RMAController : BaseController
    {
        // GET: RMA
        public ActionResult Index(RMAVM RMAVM)
        {
            var RMAList = db.RMA.Where(x => x.IsEnable);
            if (RMAVM.CompanyID.HasValue)
            {
                RMAList = RMAList.Where(x => x.CompanyID == RMAVM.CompanyID);
            }
            if (RMAVM.Channel.HasValue)
            {
                RMAList = RMAList.Where(x => x.Channel == RMAVM.Channel);
            }
            if (!string.IsNullOrWhiteSpace(RMAVM.Country))
            {
                RMAList = RMAList.Where(x => x.Country == RMAVM.Country);
            }
            if (!string.IsNullOrWhiteSpace(RMAVM.SourceID))
            {
                RMAList = RMAList.Where(x => x.SourceID == RMAVM.SourceID);
            }
           
            //if (RMAVM.ID.HasValue)
            //{

            //}

            RMAVM.RMAList = RMAList.OrderByDescending(x => x.ID);
            return View(RMAVM);
        }
        public ActionResult Create()
        {
            ViewBag.SID = DateTime.Now.ToString("HHmmss");
            return View();
        }
        [HttpPost]
        public ActionResult Create(string SID, List<RMAModelPost> RMAList)
        {
            var RedirectID = 0;
            var OrderItemDataList = (List<OrderItemData>)Session["RSkuNumberList" + SID];
            var CreateAt = DateTime.UtcNow;
            foreach (var OrderItemDataitem in OrderItemDataList)
            {
                var OrderID = OrderItemDataitem.OrderID;
                var SourceID = OrderItemDataitem.OrderSourceOrderId;
                var CompanyID = OrderItemDataitem.CompanyID;
                var Country = OrderItemDataitem.CountryCode;
                var Channel = OrderItemDataitem.OrderSource;
                var FinalShippingFee = OrderItemDataitem.FinalShippingFee;
                var WarehouseID = db.WarehouseSummary.AsQueryable().Where(x => x.Type == "SCID" && x.Val.Contains(OrderItemDataitem.WarehouseID.ToString())).FirstOrDefault()?.WarehouseID;
                //開SC的RMA
                SC_WebService SCWS = new SC_WebService("tim@weypro.com", "timfromweypro");
                SCService.Order order = SCWS.Get_OrderData(OrderID).Order;//去SC抓訂單資料
                order.OrderCreationSourceApplication = SCService.OrderCreationSourceApplicationType.PointOfSale;
                if (SCWS.Update_Order(order))
                {
                    int RMAId = SCWS.Create_RMA(order.ID);//建立RMAID
                    var newRMA = new RMA
                    {
                        IsEnable = true,
                        OrderID = OrderID,
                        SourceID = SourceID,
                        CompanyID = CompanyID,
                        Country = Country,
                        Channel = Channel,
                        WarehouseID = WarehouseID,
                        FinalShippingFee = FinalShippingFee,
                        SCRMA = RMAId.ToString(),
                        CreateBy = UserBy,
                        CreateAt = CreateAt
                    };
                    db.RMA.Add(newRMA);



                    var RMAListselect = RMAList.Where(x => !string.IsNullOrWhiteSpace(x.SKU));
                    if (RMAListselect.Any())
                    {
                        foreach (var RMAListitem in RMAList)
                        {
                            //int RMAItemID = SCWS.Create_RMA_Item(OrderID, item.ID, RMAId, item.Qty, Reason, "");//建立每個SKU要退貨的數量
                            var skulist = OrderItemDataitem.Items.Where(x => x.SKU == RMAListitem.SKU).Select(x => new { x.SKU, x.QTY }).Distinct();//取單價
                            if (skulist.Any())
                            {

                                foreach (var Skuitem in skulist)
                                {
                                    var UnitPrice = OrderItemDataitem.Items.Where(x => x.SKU == Skuitem.SKU).FirstOrDefault()?.UnitPrice;
                                    var ProductName = db.SkuLang.Where(x => x.LangID == "en-US" && x.Sku == Skuitem.SKU).FirstOrDefault()?.Name;
                                    var newRMASKU = new RMASKU
                                    {
                                        IsEnable = true,
                                        Name = ProductName,
                                        SkuNo = Skuitem.SKU,
                                        QTYOrdered = Skuitem.QTY,
                                        ReturnedQTY = 1,
                                        Reason = RMAListitem.Reason,
                                        WarehouseID = RMAListitem.Warehouse,
                                        UnitPrice = UnitPrice,
                                        RMAItemID = RMAItemID,
                                        CreateBy = UserBy,
                                        CreateAt = CreateAt
                                    };
                                    newRMA.RMASKU.Add(newRMASKU);
                                }
                            }
                        }

                        db.SaveChanges();
                        if (RedirectID == 0)
                        {
                            RedirectID = newRMA.ID;
                        }
                    }
                }

                else
                {

                }
            }
            return RedirectToAction("Edit", new { id = RedirectID });
        }
        public ActionResult RSkuNumberList(int? draw, int? start, int? length, int? OrderID, string SourceID, string UserID, string SID)
        {
            var RMAModelVMList = new List<RMAModelVM>();
            var OrderItemData = GetOrderItemData(OrderID, SourceID, UserID, 3);
            var NoSKU = new List<string>();
            if (OrderItemData != null && OrderItemData.Count() > 0)
            {
                foreach (var item in OrderItemData)
                {
                    if (item.Items.Count() > 0)
                    {
                        foreach (var SKUitem in item.Items)
                        {
                            var SKUNo = SKUitem.SKU;
                            var SKU = db.SKU.Find(SKUNo);
                            if (SKU == null)
                            {
                                NoSKU.Add(SKUNo);
                            }
                            else
                            {
                                var sku = SKU.SkuLang.Where(x => x.LangID == "en-US" && x.Sku == SKUNo).FirstOrDefault();
                                var ProductName = sku?.Name;
                                var UPC = sku?.GetSku.UPC + sku?.GetSku.EAN;
                                RMAModelVMList.Add(new RMAModelVM { ck = item.OrderID, Order = item.OrderID, SourceID = item.OrderSourceOrderId, QTY = 1, SKU = SKUNo, ProductName = ProductName, UPC = UPC });
                            }
                        }
                    }
                }
                if (NoSKU.Any())
                {
                    return Json(new { success = false, errmsg = string.Join(";", NoSKU) + "無SKU資料請先增SKU" }, JsonRequestBehavior.AllowGet);
                }
                //過濾已開RMA的SKU
                if (RMAModelVMList.Any())
                {
                    var Orderlist = RMAModelVMList.Select(x => x.Order).Distinct().ToList();
                    var RMAList = db.RMA.Where(x => x.IsEnable && Orderlist.Contains(x.OrderID.Value));
                    foreach (var RMAitem in RMAList)
                    {
                        foreach (var RMASKUitem in RMAitem.RMASKU.Where(x => x.IsEnable))
                        {
                            var chkRMAModelVMList = RMAModelVMList.Where(x => x.Order == RMAitem.OrderID && x.SKU == RMASKUitem.SkuNo).ToList();
                            if (chkRMAModelVMList.Any())
                            {
                                RMAModelVMList.Remove(chkRMAModelVMList.FirstOrDefault());
                            }
                        }
                    }
                    if (!RMAModelVMList.Any())
                    {
                        return Json(new { success = false, errmsg = "無可開RMA的訂單" }, JsonRequestBehavior.AllowGet);
                    }
                }

                Session["RSkuNumberList" + SID] = OrderItemData;
            }
            else
            {
                return Json(new { success = false, errmsg = "查無訂單資料" }, JsonRequestBehavior.AllowGet);
            }
            int recordsTotal = RMAModelVMList.Count();
            var returnObj = new
            {
                success = true,
                draw = draw,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = RMAModelVMList//分頁後的資料 
            };
            return Json(returnObj, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult GetSKUList(int OrderID)
        {
            var RMAModelVMList = new List<RMAModelVM>();
            var OrderItemData = GetOrderItemData(OrderID, "", "", 3);
            foreach (var item in OrderItemData)
            {
                foreach (var SKUitemList in item.Items)
                {
                    var SKUNo = SKUitemList.SKU;
                    var ProductName = db.SkuLang.Where(x => x.LangID == "en-US" && x.Sku == SKUNo).FirstOrDefault()?.Name;
                    RMAModelVMList.Add(new RMAModelVM { ck = item.OrderID, Order = item.OrderID, QTY = item.Items.FirstOrDefault().QTY, SKU = SKUNo, ProductName = ProductName});
                }
            }
            var partial = ControlToString("~/Views/Shared/GetRMASKUList.cshtml", RMAModelVMList);
            return Json(new { status = true, partial }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult GetRMASKUList(int ID)
        {
            var RMAModelVMList = new List<RMAModelVM>();
            var RMA = db.RMA.Find(ID);
            foreach (var item in RMA.RMASKU)
            {
                var SKUNo = item.SkuNo;
                var ProductName = db.SkuLang.Where(x => x.LangID == "en-US" && x.Sku == SKUNo).FirstOrDefault()?.Name;
                RMAModelVMList.Add(new RMAModelVM { QTY = item.ReturnedQTY, SKU = SKUNo, ProductName = ProductName, Warehouse = item.WarehouseID, Reason=item.Reason });
            }
           
            var partial = ControlToString("~/Views/Shared/GetRMASKUList.cshtml", RMAModelVMList);
            return Json(new { status = true, partial }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Edit(int id)
        {
            var RMA = db.RMA.Find(id);

            var RMASKUList = RMA.RMASKU.Where(x => x.IsEnable).Select(x => new RMAEdit
            {
                ID = x.ID,
                Model = "L",
                SKU = x.SkuNo,
                QTYOrdered = x.QTYOrdered,
                ProductName = x.SKU.SkuLang.Where(y => y.LangID == "en-US").FirstOrDefault()?.Name,
                SerialsNo = x.RMASerialsLlist.FirstOrDefault()?.SerialsNo,
                UPCEAN = x.SKU.UPC + "/" + x.SKU.EAN,
                Reason = x.Reason,
                TrWarehouse = x.RMASerialsLlist.FirstOrDefault()?.Warehouse.Name,
                Warehouse = x.WarehouseID,
                UnitPrice = (x.UnitPrice * x.RMASerialsLlist.Sum(y=>y.SerialsQTY)) ?? 0
            });
            Session["RMAEdit" + id] = RMASKUList.ToList();
            return View(RMA);
        }
        [HttpPost]
        public ActionResult Edit(RMA RMA, List<RMAModelPost> RMAList)
        {
            var OldRMA = db.RMA.Find(RMA.ID);
            OldRMA.Status = RMA.Status;
            OldRMA.Action = RMA.Action;
            OldRMA.SourceCaseID = RMA.SourceCaseID;
            OldRMA.RestockingFee = RMA.RestockingFee;
            OldRMA.ReturnShippingCos = RMA.ReturnShippingCos;
            OldRMA.OtherCosts = RMA.OtherCosts;
            OldRMA.UpdateBy = UserBy;
            OldRMA.UpdateAt = DateTime.UtcNow;
            foreach (var RMAListitem in RMAList)
            {
                foreach (var RMASKUitem in OldRMA.RMASKU.Where(x => x.IsEnable))
                {
                    if (RMAListitem.ID == RMASKUitem.ID.ToString())
                    {
                        if (!string.IsNullOrWhiteSpace(RMAListitem.Reason) && RMAListitem.Reason != RMASKUitem.Reason)
                        {
                            RMASKUitem.Reason = RMAListitem.Reason;
                            RMASKUitem.UpdateBy = UserBy;
                            RMASKUitem.UpdateAt = DateTime.UtcNow;
                        }
                        if (RMAListitem.Warehouse.HasValue && RMAListitem.Warehouse != RMASKUitem.WarehouseID)
                        {
                            RMASKUitem.WarehouseID = RMAListitem.Warehouse;
                            RMASKUitem.UpdateBy = UserBy;
                            RMASKUitem.UpdateAt = DateTime.UtcNow;
                        }
                    }
                }
            }
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult ChangeSKU(int OldSKU, string[] NewSKU)
        {
            return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult CreatNote(int? ID, string SID, string Note)
        {
            try
            {
                var PurchaseNoteList = new List<PurchaseNote>();
                if (ID.HasValue && ID != 0)
                {
                    var RMA = db.RMA.Find(ID);
                    RMA.PurchaseNote.Add(new PurchaseNote { IsEnable = true, Note = Note, NoteType = "txt", CreateAt = DateTime.UtcNow, CreateBy = UserBy });
                    db.SaveChanges();
                    PurchaseNoteList = RMA.PurchaseNote.ToList();
                }
                else
                {
                    PurchaseNoteList = (List<PurchaseNote>)Session["RMAPurchaseNote" + SID];
                    if (PurchaseNoteList == null)
                    {
                        PurchaseNoteList = new List<PurchaseNote>();
                    }

                    PurchaseNoteList.Add(new PurchaseNote { IsEnable = true, Note = Note, NoteType = "txt", CreateAt = DateTime.UtcNow, CreateBy = UserBy });
                    Session["RMAPurchaseNote" + SID] = PurchaseNoteList;
                }
                return Json(new { status = true, datalist = PurchaseNoteList.OrderByDescending(x => x.CreateAt).Select(x => new { CreateAt = x.CreateAt.ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss"), x.CreateBy, x.Note, x.NoteType }).ToList() }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, Errmsg = ex.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult CreatNoteImg(int? ID, string SID, HttpPostedFileBase Img)
        {
            try
            {
                if (Img == null)
                {
                    return Json(new { status = false, Errmsg = "沒有圖檔" }, JsonRequestBehavior.AllowGet);
                }
                var NoteType = Img.ContentType;
                var PurchaseNoteList = new List<PurchaseNote>();
                if (ID.HasValue && ID != 0)
                {
                    var Note = SaveImg(Img);
                    var RMA = db.RMA.Find(ID);
                    RMA.PurchaseNote.Add(new PurchaseNote { IsEnable = true, Note = Note, NoteType = "Url", CreateAt = DateTime.UtcNow, CreateBy = UserBy });
                    db.SaveChanges();
                    PurchaseNoteList = RMA.PurchaseNote.ToList();
                }
                else
                {
                    MemoryStream target = new MemoryStream();

                    Img.InputStream.CopyTo(target);
                    byte[] data = target.ToArray();
                    string Note = Convert.ToBase64String(data, 0, data.Length);
                    PurchaseNoteList = (List<PurchaseNote>)Session["RMAPurchaseNote" + SID];
                    if (PurchaseNoteList == null)
                    {
                        PurchaseNoteList = new List<PurchaseNote>();
                    }

                    PurchaseNoteList.Add(new PurchaseNote { IsEnable = true, Note = Note, NoteType = NoteType, CreateAt = DateTime.UtcNow, CreateBy = UserBy });
                    Session["RMAPurchaseNote" + SID] = PurchaseNoteList;
                }
                return Json(new { status = true, datalist = PurchaseNoteList.OrderByDescending(x => x.CreateAt).Select(x => new { CreateAt = x.CreateAt.ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss"), x.CreateBy, x.Note, x.NoteType }).ToList() }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, Errmsg = ex.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult GetSerialList(int ID)
        {
            var RMASKU = db.RMASKU.Find(ID);
            var SerialsLlist = RMASKU.RMASerialsLlist.Where(x => x.SerialsType == "RMAIn").Select(x => new RMASerial { Serial = x.SerialsNo, Warehouse = x.Warehouse.Name, Reason = x.Reason }).ToList();
            var partial = ControlToString("~/Views/Shared/GetRMASerialList.cshtml", SerialsLlist);
            //var partial = Engine.Razor.RunCompile(template, "templateKey", null, new { Name = "World" });
            return Json(new { status = true, partial }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Returnedserials(int ID)
        {
            var companyList = db.Company.AsNoTracking().Where(c => c.IsEnable).ToList();
            ViewBag.Company = companyList.Where(c => !c.ParentID.HasValue).Select(c => new SelectListItem() { Text = c.Name, Value = c.ID.ToString() }).ToList();
            var RMASKU = db.RMASKU.Find(ID);
            return View(RMASKU);
        }
        public ActionResult Saveserials(string serials,string Reason, int RMASKUID, int WarehouseID)
        {
            var RMASKU = db.RMASKU.Find(RMASKUID);
            var SerialsLlistCount = RMASKU.RMASerialsLlist.Where(x => x.SerialsType == "RMAIn").Sum(x => x.SerialsQTY);//計算RMAIn的序號數
            if (SerialsLlistCount >= RMASKU.QTYOrdered)
            {
                return Json(new { status = false, Errmsg = "序號不可大於回收數" }, JsonRequestBehavior.AllowGet);
            }
            var RMASerialsLlist = db.RMASerialsLlist.Where(x => x.SerialsNo == serials && x.RMASKU.SkuNo == RMASKU.SkuNo);//檢查序號是否重複，同SKU序號不能新增,2019/02/05 加入有已出貨或是CM的紀錄, 就能重新在入庫
            if (!RMASerialsLlist.Any())
            {

                var OrderData = db.SerialsLlist.Where(x => x.SerialsType == "Order" && x.SerialsNo == serials && x.PurchaseSKU.SkuNo == RMASKU.SkuNo && x.OrderID == RMASKU.RMA.OrderID);

                if (OrderData.Any())
                {
                    var dt = DateTime.UtcNow;
                    var nSerialsLlistIn = new RMASerialsLlist
                    {
                        WarehouseID = WarehouseID,
                        Reason = Reason,
                        SerialsType = "RMAIn",
                        SerialsNo = serials,
                        SerialsQTY = 1,
                        ReceivedBy = UserBy,
                        ReceivedAt = dt,
                        CreateBy = UserBy,
                        CreateAt = dt
                    };
                    RMASKU.RMASerialsLlist.Add(nSerialsLlistIn);
                    RMASKU.ReceivedBy = UserBy;
                    RMASKU.ReceivedAt = dt;
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
                    return Json(new { status = false, Errmsg = "序號沒有出貨資料" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { status = false, Errmsg = "序號已經存在" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult RemoveData(int[] IDList, int ID)
        {
            var Errmsg = "";
            if (IDList != null && IDList.Any())
            {
                foreach (var item in IDList)
                {

                    var RMASKU = db.RMASKU.Find(item);
                    if (RMASKU.RMASerialsLlist.Any())
                    {
                        Errmsg += "【" + RMASKU.SkuNo + "】已有序號不能刪除；";
                    }
                    else
                    {
                        var dt = DateTime.UtcNow;
                        RMASKU.IsEnable = false;
                        RMASKU.UpdateAt = dt;
                        RMASKU.UpdateBy = UserBy;
                        db.SaveChanges();
                    }


                }
                var RMA = db.RMA.Find(ID);

                var RMASKUList = RMA.RMASKU.Where(x => x.IsEnable).Select(x => new RMAEdit
                {
                    ID = x.ID,
                    Model = "L",
                    SKU = x.SkuNo,
                    QTYOrdered = x.QTYOrdered,
                    ProductName = x.SKU.SkuLang.Where(y => y.LangID == "en-US").FirstOrDefault()?.Name,
                    SerialsNo = x.RMASerialsLlist.FirstOrDefault()?.SerialsNo,
                    UPCEAN = x.SKU.UPC + "/" + x.SKU.EAN,
                    Reason = x.Reason,
                    TrWarehouse = x.RMASerialsLlist.FirstOrDefault()?.Warehouse.Name,
                    Warehouse = x.WarehouseID
                });
                Session["RMAEdit" + ID] = RMASKUList.ToList();
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
        public ActionResult GetEditSKUData(int ID)
        {
            try
            {
                var RMAModelVMList = (List<RMAEdit>)Session["RMAEdit" + ID];
                var recordsTotal = RMAModelVMList.Count();
                var returnObj = new
                {
                    success = true,
                    recordsTotal = recordsTotal,
                    recordsFiltered = recordsTotal,
                    data = RMAModelVMList//分頁後的資料 
                };
                return Json(returnObj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                throw;
            }


        }
        public ActionResult EditSKUData(int[] IDList, int ID)
        {
            var Errmsg = "";
            if (IDList != null && IDList.Any())
            {
                var RMAModelVMList = (List<RMAEdit>)Session["RMAEdit" + ID];
                foreach (var item in IDList)
                {
                    foreach (var odataListitem in RMAModelVMList.Where(x => x.ID == item))
                    {
                        odataListitem.Model = "E";
                    }
                }
                Session["RMAEdit" + ID] = RMAModelVMList;
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
    }
}