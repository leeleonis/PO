using inventorySKU;
using Newtonsoft.Json;
using PurchaseOrderSys.Models;
using SellerCloud_WebService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace PurchaseOrderSys.Controllers
{
    [CheckSession]
    public class RMAController : BaseController
    {
        protected static bool UpdateSC = true;
        public RMAController()
        {
            SCWS = new SC_WebService(ApiUserName, ApiPassword);
            var TestMod = ConfigurationManager.AppSettings["TestMod"];
            if (TestMod == "true")
            {
                UpdateSC = false;
            }
        }
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
            if (!string.IsNullOrWhiteSpace(RMAVM.SourceCaseID))
            {
                RMAList = RMAList.Where(x => x.SourceCaseID == RMAVM.SourceCaseID);
            }
            if (RMAVM.QID.HasValue)
            {
                RMAList = RMAList.Where(x => x.ID == RMAVM.QID);
            }
            if (!string.IsNullOrWhiteSpace(RMAVM.SCUserID))
            {
                RMAList = RMAList.Where(x => x.SCUserID == RMAVM.SCUserID);
            }
            if (!string.IsNullOrWhiteSpace(RMAVM.SCRMA))
            {
                RMAList = RMAList.Where(x => x.SCRMA == RMAVM.SCRMA);
            }
            if (RMAVM.OrderID.HasValue)
            {
                RMAList = RMAList.Where(x => x.OrderID == RMAVM.OrderID);
            }
            if (!string.IsNullOrWhiteSpace(RMAVM.Serial))
            {
                var SerialsLlist = new List<int>();
                var RMAOrderSerialsLlist = db.RMAOrderSerialsLlist.Where(x => x.SerialsNo == RMAVM.Serial);
                var RMASerialsLlist = db.RMASerialsLlist.Where(x => x.SerialsNo == RMAVM.Serial);
                foreach (var item in RMAOrderSerialsLlist)
                {
                    SerialsLlist.Add(item.RMASKU.RMA.ID);
                }
                foreach (var item in RMASerialsLlist)
                {
                    SerialsLlist.Add(item.RMASKU.RMA.ID);
                }
                RMAList = RMAList.Where(x => SerialsLlist.Contains(x.ID));
            }
            var RMAListo = RMAList.OrderByDescending(x => x.ID).Take(1000);
            foreach (var item in RMAListo)
            {
                if (!item.QDID.HasValue)
                {
                    item.QDID = db.Orders.Where(x => x.IsEnable && x.SCID == item.OrderID).FirstOrDefault()?.ID;
                }        
            }
            db.SaveChanges();
            RMAVM.RMAList = RMAListo;
            return View(RMAVM);
        }
        public ActionResult Create(int? OrderID)
        {
            ViewBag.OrderID = OrderID;
            ViewBag.SID = DateTime.Now.ToString("HHmmssfff");
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
                //SC_WebService SCWS = new SC_WebService("tim@weypro.com", ApiPassword);
                var order = SCWS.Get_OrderData(OrderID).Order;//去SC抓訂單資料
                var SCRMA = SCWS.Get_RMA_by_OrderID(OrderID);//檢查SC上是否有開過RMA

                //var order = OrderData.Order;
                //var Serials = OrderData.Serials;
                order.OrderCreationSourceApplication = SCService.OrderCreationSourceApplicationType.PointOfSale;
                if (!UpdateSC || SCWS.Update_Order(order))
                {
                    int RMAId = 0;
                    if (SCRMA == null)
                    {
                        if (UpdateSC)
                        {
                            RMAId = SCWS.Create_RMA(order.ID);//建立RMAID
                        }
                    }
                    else
                    {
                        RMAId = SCRMA.ID;
                    }
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
                        SCUserID = OrderItemDataitem.eBayUserID,
                        CreateBy = UserBy,
                        CreateAt = CreateAt
                    };
                    db.RMA.Add(newRMA);
                    foreach (var Skuitem in OrderItemDataitem.Items)
                    {
                        var tSKU = Skuitem.SKU.Split('-')[0];
                        var SKURMAList = RMAList.Where(x => x.OrderID == OrderItemDataitem.OrderID && x.SKU == tSKU);
                        var OrderItemID = order.Items.Where(x => x.ProductID.Contains(tSKU)).FirstOrDefault().ID;
                        if (SKURMAList.Any())
                        {
                            var ReasonID = 1;
                            var Reason = SKURMAList.FirstOrDefault().Reason;
                            int.TryParse(Reason, out ReasonID);
                            int RMAItemID = 0;
                            if (SCRMA == null)
                            {
                                if (UpdateSC)
                                {
                                    RMAItemID = SCWS.Create_RMA_Item(OrderID, OrderItemID, RMAId, Skuitem.QTY, ReasonID, "");//建立每個SKU要退貨的數量原因，並取回ID
                                }
                            }
                            else
                            {
                                var SCRMA_Item = SCWS.Get_RMA_Item(OrderID)?.Where(x => x.OriginalOrderItemID == OrderItemID).FirstOrDefault();
                                if (SCRMA_Item == null)//沒資料就新增
                                {
                                    if (UpdateSC)
                                    {
                                        RMAItemID = SCWS.Create_RMA_Item(OrderID, OrderItemID, RMAId, Skuitem.QTY, ReasonID, "");//建立每個SKU要退貨的數量原因，並取回ID
                                    }

                                }
                                else
                                {
                                    //有資料直接取值
                                    RMAItemID = SCRMA_Item.ID;
                                }
                            }
                            var UnitPrice = OrderItemDataitem.Items.Where(x => x.SKU == tSKU).FirstOrDefault()?.UnitPrice;
                            var ProductName = db.SkuLang.Where(x => x.LangID == LangID && x.Sku == tSKU).FirstOrDefault()?.Name;
                            foreach (var RMAListitem in RMAList.Where(x => x.SKU == tSKU))
                            {
                                var newRMASKU = new RMASKU
                                {
                                    IsEnable = true,
                                    Name = ProductName,
                                    SkuNo = tSKU,
                                    QTYOrdered = 1,
                                    ReturnedQTY = 1,
                                    Reason = RMAListitem.Reason,
                                    WarehouseID = RMAListitem.Warehouse,
                                    UnitPrice = UnitPrice,
                                    RMAItemID = RMAItemID,
                                    CreateBy = UserBy,
                                    CreateAt = CreateAt
                                };
                                newRMASKU.RMAOrderSerialsLlist.Add(
                                    new RMAOrderSerialsLlist
                                    {
                                        IsEnable = true,
                                        SerialsNo = RMAListitem.Serial.Trim(),
                                        SerialsQTY = 1,
                                        CreateBy = UserBy,
                                        CreateAt = CreateAt
                                    });
                                newRMA.RMASKU.Add(newRMASKU);
                            }
                        }
                    }
                    if (UpdateSC)
                    {
                        order.OrderCreationSourceApplication = SCService.OrderCreationSourceApplicationType.Default;
                        SCWS.Update_Order(order);
                    }             
                    db.SaveChanges();
                    foreach (var item in db.Orders.Where(x => x.IsEnable && x.SCID == OrderID))
                    {
                        using (var OM = new OrderManagement(item.ID))
                        {
                            item.RMAID = newRMA.ID;
                            OM.ActionLog("Order", "Create RMA - " + item.RMAID);
                        }
                    }
                    db.SaveChanges();
                    if (RedirectID == 0)
                    {
                        RedirectID = newRMA.ID;
                    }
                }
            }
            if (RedirectID == 0)
            {
                return RedirectToAction("Create");
            }
            else
            {
                return RedirectToAction("Edit", new { id = RedirectID });
            }
        }
        public ActionResult ChkRSkuNumberList(int? OrderID, string SourceID, string UserID, string SID)
        {
            var Orderlist = new List<int>();
            var OrderItemData = GetOrderItemData(OrderID, SourceID, UserID, 3);
            if (OrderItemData != null && OrderItemData.Count() > 0)
            {
                Orderlist.AddRange(OrderItemData.Select(x => x.OrderID).OrderByDescending(x=>x));
            }
            var partial = ControlToString("~/Views/RMA/GetOrderList.cshtml", Orderlist);
            //var partial = Engine.Razor.RunCompile(template, "templateKey", null, new { Name = "World" });
            return Json(new { status = true, html = partial, length = Orderlist.Count() }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult RSkuNumberList(int? draw, int? start, int? length, int? OrderID, string SourceID, string UserID, string SID)
        {
            var RMAModelVMList = new List<RMAModelVM>();
            if (OrderID.HasValue || !string.IsNullOrWhiteSpace(SourceID) || !string.IsNullOrWhiteSpace(UserID))
            {
                var OrderItemData = GetOrderItemData(OrderID, SourceID, UserID, 3);
                if (!OrderID.HasValue)
                {
                    OrderID = OrderItemData.LastOrDefault().OrderID;
                }
                var HaveSerialsLlist = db.SerialsLlist.Where(x => x.OrderID == OrderID && ((x.PurchaseSKUID.HasValue && x.PurchaseSKU.IsEnable && x.PurchaseSKU.PurchaseOrder.IsEnable) || (x.TransferSKUID.HasValue && x.TransferSKU.IsEnable && x.TransferSKU.Transfer.IsEnable))).Any();
                if (!HaveSerialsLlist)
                {
                    return Json(new { success = false, errmsg = "系統內無出貨資料" }, JsonRequestBehavior.AllowGet);
                }
                var NoSKU = new List<string>();
                int index = 0;
                if (OrderItemData != null && OrderItemData.Count() > 0)
                {
                    foreach (var item in OrderItemData)
                    {
                        if (item.Items.Count() > 0)
                        {
                            foreach (var SKUitem in item.Items)
                            {
                                var SKUNo = SKUitem.SKU.Split('-')[0];
                                var SKU = db.SKU.Find(SKUNo);
                                if (SKU == null)
                                {
                                    NoSKU.Add(SKUNo);
                                }
                                else
                                {
                                    var sku = SKU.SkuLang.Where(x => x.LangID == LangID && x.Sku == SKUNo).FirstOrDefault();
                                    var ProductName = sku?.Name;
                                    var UPC = sku?.GetSku.UPC + sku?.GetSku.EAN;
                                    var Serial = "";



                                    for (int i = 0; i < SKUitem.QTY; i++)
                                    {
                                        try
                                        {
                                            if (SKUitem.Serials.Any())
                                            {
                                                Serial = SKUitem.Serials[i];
                                            }
                                            else
                                            {
                                                if (string.IsNullOrWhiteSpace(Serial))
                                                {
                                                    Serial = db.SerialsLlist.Where(x => x.IsEnable && x.OrderID == OrderID && ((x.PurchaseSKUID.HasValue && x.PurchaseSKU.SkuNo == SKUNo) || (x.TransferSKUID.HasValue && x.TransferSKU.SkuNo == SKUitem.SKU))).OrderBy(x => x.ID).Skip(i).Take(1).FirstOrDefault()?.SerialsNo;
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {

                                        }
                                        RMAModelVMList.Add(new RMAModelVM { ck = index, Order = item.OrderID, SourceID = item.OrderSourceOrderId, QTY = 1, SKU = SKUNo, ProductName = ProductName, UPC = UPC, Serial = Serial });
                                        index++;
                                    }

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
                        var ReRma = new List<int>();
                        var Orderlist = RMAModelVMList.Select(x => x.Order).Distinct().ToList();
                        var RMAList = db.RMA.Where(x => x.IsEnable && Orderlist.Contains(x.OrderID.Value));
                        foreach (var RMAitem in RMAList)
                        {
                            foreach (var RMASKUitem in RMAitem.RMASKU.Where(x => x.IsEnable))
                            {
                                var chkRMAModelVMList = RMAModelVMList.Where(x => x.Order == RMAitem.OrderID && x.SKU == RMASKUitem.SkuNo).ToList();
                                if (chkRMAModelVMList.Any())
                                {
                                    ReRma.Add(RMAitem.ID);
                                    RMAModelVMList.Remove(chkRMAModelVMList.FirstOrDefault());
                                }
                            }
                        }
                        if (!RMAModelVMList.Any())
                        {
                            var Errmsg = "無可開RMA的訂單";
                            if (ReRma.Any())
                            {
                                Errmsg = "已開過RMA單 " + string.Join(";", ReRma.Distinct());
                            }
                            return Json(new { success = false, errmsg = Errmsg }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    //排除已入PO
                    if (RMAModelVMList.Any())
                    {
                        foreach (var item in RMAModelVMList.ToList())
                        {
                            if (db.SerialsLlist.Where(x => x.SerialsNo == item.Serial && x.SerialsQTY > 0 && !x.SerialsLlistC.Any(y => y.IsEnable)).Any())
                            {
                                RMAModelVMList.Remove(item);
                            }
                        }
                    }
                    Session["RSkuNumberList" + SID] = OrderItemData;
                }
                else
                {
                    return Json(new { success = false, errmsg = "查無訂單資料" }, JsonRequestBehavior.AllowGet);
                }
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
                    var ProductName = db.SkuLang.Where(x => x.LangID == LangID && x.Sku == SKUNo).FirstOrDefault()?.Name;
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
            foreach (var item in RMA.RMASKU.Where(x => x.IsEnable))
            {
                var SKUNo = item.SkuNo;
                var ProductName = db.SkuLang.Where(x => x.LangID == LangID && x.Sku == SKUNo).FirstOrDefault()?.Name;
                foreach (var SerialsLitem in item.RMASerialsLlist.Where(x => x.IsEnable && x.SerialsType == "RMAIn"))
                {
                    RMAModelVMList.Add(new RMAModelVM { QTY = item.ReturnedQTY, SKU = SKUNo, ProductName = ProductName, Warehouse = SerialsLitem.WarehouseID ?? RMA.WarehouseID, Reason = SerialsLitem.Reason, Serial = SerialsLitem.SerialsNo, Carrier = GetSerialListCarrier(SerialsLitem), ReturnTracking = GetSerialListTracking(SerialsLitem) });
                }
            }
           
            var partial = ControlToString("~/Views/Shared/GetRMASKUList.cshtml", RMAModelVMList);
            return Json(new { status = true, partial }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Edit(int id)
        {
            var RMA = db.RMA.Find(id);
            var ShippingList = new FedExApi.Shipping_API().ShippingList().data.shippingMethod;
            var RMASKUList = new List<RMAEdit>();
            foreach (var RMASKUitem in RMA.RMASKU.Where(x => x.IsEnable))
            {
                foreach (var Serialitem in RMASKUitem.RMAOrderSerialsLlist)
                {
                    var nRMAEdit = new RMAEdit();

                    var RMASerial = RMASKUitem.RMASerialsLlist.Where(x => x.SerialsNo == Serialitem.SerialsNo).FirstOrDefault();

                    nRMAEdit.ID = Serialitem.ID;
                    nRMAEdit.SKUID = RMASKUitem.ID;
                    if (RMASerial == null)
                    {
                        nRMAEdit.Model = "E";
                    }
                    else
                    {
                        nRMAEdit.Model = "L";
                    }

                    if (RMASerial != null && !string.IsNullOrWhiteSpace(RMASerial.NewSkuNo))
                    {
                        nRMAEdit.SKU = RMASerial.NewSkuNo;
                    }
                    else
                    {
                        nRMAEdit.SKU = RMASKUitem.SkuNo;
                    }

                    nRMAEdit.QTYOrdered = RMASKUitem.QTYOrdered;
                    nRMAEdit.ProductName = RMASKUitem.SKU.SkuLang.Where(y => y.LangID == LangID).FirstOrDefault()?.Name;
                    nRMAEdit.ReturnedSerialsNo = RMASerial?.SerialsNo.Trim();
                    nRMAEdit.OrderSerialsNo = Serialitem.SerialsNo.Trim();
                    nRMAEdit.UPCEAN = RMASKUitem.SKU.UPC + "/" + RMASKUitem.SKU.EAN;
                    nRMAEdit.Reason = Serialitem.Reason?? RMASKUitem.Reason;
                    nRMAEdit.TrWarehouse = RMASerial?.Warehouse.Name;
                    nRMAEdit.Warehouse = RMASerial?.WarehouseID ?? Serialitem.WarehouseID ?? RMASKUitem.WarehouseID;
                    nRMAEdit.UnitPrice = (RMASKUitem.UnitPrice * RMASKUitem.RMASerialsLlist.Sum(y => y.SerialsQTY)) ?? 0;
                    nRMAEdit.tracking = Serialitem.RMAOrderTracking?.ReturnTracking ?? "";
                    nRMAEdit.Carrier = ShippingList.Where(x => x.value.ToString() == Serialitem.RMAOrderTracking?.Carrier).FirstOrDefault()?.text ?? "";
                    nRMAEdit.RMASerialsLlistID = RMASKUitem.RMASerialsLlist.Where(x => x.IsEnable && x.SerialsNo == Serialitem.SerialsNo).FirstOrDefault()?.ID;
                    nRMAEdit.trackingID = Serialitem.RMAOrderTrackingID ?? 0;
                    nRMAEdit.CMID = GetRMA_CMID(Serialitem.SerialsNo);
                    RMASKUList.Add(nRMAEdit);
                }
            }
            Session["RMAEdit" + id] = RMASKUList.ToList();
            return View(RMA);
        }

        private int? GetRMA_CMID(string SerialsNo)
        {
            int? CMID;
            var RMASerialsLlistall = db.RMASerialsLlist.Where(x => x.IsEnable && x.SerialsNo == SerialsNo && x.SerialsType == "CM");
            var SerialsLlistall = db.SerialsLlist.Where(x => x.IsEnable && x.SerialsNo == SerialsNo && x.SerialsType == "CM");
            CMID = RMASerialsLlistall.FirstOrDefault()?.PurchaseSKU.CreditMemoID;
            if (!CMID.HasValue)
            {
                CMID = SerialsLlistall.FirstOrDefault()?.PurchaseSKU.CreditMemoID;
            }
            return CMID;
        }

        [HttpPost]
        public ActionResult Edit(RMA RMA, List<RMAModelPost> RMAList)
        {
            var dt = DateTime.UtcNow;

            var OldRMA = db.RMA.Find(RMA.ID);
            OldRMA.Status = RMA.Status;
            OldRMA.Action = RMA.Action;
            OldRMA.SourceCaseID = RMA.SourceCaseID;
            OldRMA.RestockingFee = RMA.RestockingFee;
            OldRMA.ReturnShippingCos = RMA.ReturnShippingCos;
            OldRMA.OtherCosts = RMA.OtherCosts;
            //OldRMA.ReturnTracking = RMA.ReturnTracking;
            //OldRMA.Carrier = RMA.Carrier;
            OldRMA.UpdateBy = UserBy;
            OldRMA.UpdateAt = dt;
            if (RMAList != null)
            {
                foreach (var RMAListitem in RMAList)
                {
                    foreach (var RMASKUitem in OldRMA.RMASKU.Where(x => x.IsEnable))
                    {
                        if (RMAListitem.SKUID == RMASKUitem.ID)
                        {
                            foreach (var OrderSerialsLlistitem in RMASKUitem.RMAOrderSerialsLlist.Where(x => x.IsEnable))
                            {
                                if (string.IsNullOrWhiteSpace(OrderSerialsLlistitem.Reason) || RMAListitem.Reason != OrderSerialsLlistitem.Reason)
                                {
                                    OrderSerialsLlistitem.Reason = RMAListitem.Reason;
                                    OrderSerialsLlistitem.UpdateBy = UserBy;
                                    OrderSerialsLlistitem.UpdateAt = dt;
                                }
                                if (!RMAListitem.Warehouse.HasValue || RMAListitem.Warehouse != OrderSerialsLlistitem.WarehouseID)
                                {
                                    OrderSerialsLlistitem.WarehouseID = RMAListitem.Warehouse;
                                    OrderSerialsLlistitem.UpdateBy = UserBy;
                                    OrderSerialsLlistitem.UpdateAt = dt;
                                }
                                if (RMAListitem.ReceiveNo == OrderSerialsLlistitem.SerialsNo)
                                {
                                    //入庫
                                    if (!string.IsNullOrWhiteSpace(RMAListitem.ReceiveNo) && !RMASKUitem.RMASerialsLlist.Where(x => x.IsEnable && x.SerialsNo == RMAListitem.ReceiveNo).Any())
                                    {
                                        var SaveserialMsg = FunSaveserials(RMAListitem.ReceiveNo, RMAListitem.Reason, RMAListitem.SKUID, RMAListitem.Warehouse.Value);
                                        if (!string.IsNullOrWhiteSpace(SaveserialMsg))
                                        {
                                            AlertErrMsg(SaveserialMsg);
                                        }

                                    }
                                }

                            }



                            //if (!string.IsNullOrWhiteSpace(RMAListitem.Reason) && RMAListitem.Reason != RMASKUitem.Reason)
                            //{
                            //    RMASKUitem.Reason = RMAListitem.Reason;
                            //    RMASKUitem.UpdateBy = UserBy;
                            //    RMASKUitem.UpdateAt = dt;
                            //}
                            //if (RMAListitem.Warehouse.HasValue && RMAListitem.Warehouse != RMASKUitem.WarehouseID)
                            //{
                            //    RMASKUitem.WarehouseID = RMAListitem.Warehouse;
                            //    RMASKUitem.UpdateBy = UserBy;
                            //    RMASKUitem.UpdateAt = dt;
                            //}
                        }
                    }
                }
                db.SaveChanges();
            }
            //return RedirectToAction("Index");
            return RedirectToAction("Edit", new { id = RMA.ID });
        }
        [HttpPost]
        public ActionResult ChangeSKU(int oid, string NewSKU)
        {
            var RMASerialsLlist = db.RMASerialsLlist.Find(oid);
            var dt = DateTime.UtcNow;
            RMASerialsLlist.NewSkuNo = RMASerialsLlist.RMASKU.SkuNo.Split('_')[0] + NewSKU;
            RMASerialsLlist.NewSKUCreateAt = dt;
            RMASerialsLlist.NewSKUCreateBy = UserBy;
            db.SaveChanges();
            return Json(new { status = true }, JsonRequestBehavior.AllowGet);
            //}

            //else
            //{
            //    return Json(new { status = false , Errmsg ="沒有可轉換的序號"}, JsonRequestBehavior.AllowGet);
            //}
        }
        [HttpPost]
        public ActionResult CreatGNote(int? ID, int[] IDList, string SID, string Note, List<HttpPostedFileBase> Img)
        {
            try
            {
                var PurchaseNoteList = new List<PurchaseNote>();
                if (ID.HasValue && ID != 0)
                {
                    var RMAOrderTracking = db.RMAOrderTracking.Find(ID);
                    var nPurchaseNote = new PurchaseNote { IsEnable = true, Note = Note, NoteType = "txt", CreateAt = DateTime.UtcNow, CreateBy = UserBy };
                    if (Img != null)
                    {
                        foreach (var Imgitem in Img)
                        {
                            var ImgName = SaveImg(Imgitem);
                            nPurchaseNote.PurchaseNoteImg.Add(new PurchaseNoteImg { IsEnable = true, Img = ImgName, ImgType = Imgitem.ContentType, CreateAt = DateTime.UtcNow, CreateBy = UserBy });
                            //db.SaveChanges();
                            //PurchaseNoteList = RMA.PurchaseNote.ToList();
                        }
                    }
                    RMAOrderTracking.PurchaseNote.Add(nPurchaseNote);
                    db.SaveChanges();
                    PurchaseNoteList = RMAOrderTracking.PurchaseNote.ToList();
                }
                else
                {
                    var RMAOrderSerialsLlist = db.RMAOrderSerialsLlist.Where(x => IDList.Contains(x.ID) && x.IsEnable);
                    var nRMAOrderTracking = new RMAOrderTracking { IsEnable = true, CreateAt = DateTime.UtcNow, CreateBy = UserBy };
                    var nPurchaseNote = new PurchaseNote { IsEnable = true, Note = Note, NoteType = "txt", CreateAt = DateTime.UtcNow, CreateBy = UserBy };
                    if (Img != null)
                    {
                        foreach (var Imgitem in Img)
                        {
                            var ImgName = SaveImg(Imgitem);
                            nPurchaseNote.PurchaseNoteImg.Add(new PurchaseNoteImg { IsEnable = true, Img = ImgName, ImgType = Imgitem.ContentType, CreateAt = DateTime.UtcNow, CreateBy = UserBy });
                            //db.SaveChanges();
                            //PurchaseNoteList = RMA.PurchaseNote.ToList();
                        }
                    }
                    nRMAOrderTracking.PurchaseNote.Add(nPurchaseNote);
                    foreach (var item in RMAOrderSerialsLlist)
                    {
                        if (!item.RMAOrderTrackingID.HasValue)
                        {
                            item.RMAOrderTracking = nRMAOrderTracking;
                            PurchaseNoteList = nRMAOrderTracking.PurchaseNote.ToList();
                        }
                        else
                        {
                            item.RMAOrderTracking.PurchaseNote.Add(nPurchaseNote);
                            PurchaseNoteList = item.RMAOrderTracking.PurchaseNote.ToList();
                        }
                    }
                    db.SaveChanges();
                }
                var datalist = PurchaseNoteList.OrderByDescending(x => x.CreateAt).Select(x => new { CreateAt = x.CreateAt.ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss"), x.CreateBy, x.Note, x.NoteType, PurchaseNoteImg = x.PurchaseNoteImg.Select(y => new PurchaseNoteImg { Img = y.Img, ImgType = y.ImgType }).ToList() }).ToList();
                return Json(new { status = true, datalist = datalist }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, Errmsg = ex.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult CreatNote(int? ID, string SID, string Note, List<HttpPostedFileBase> Img)
        {
            try
            {
                var PurchaseNoteList = new List<PurchaseNote>();
                if (ID.HasValue && ID != 0)
                {
                    var RMA = db.RMA.Find(ID);
                    var nPurchaseNote = new PurchaseNote { IsEnable = true, Note = Note, NoteType = "txt", CreateAt = DateTime.UtcNow, CreateBy = UserBy };
                    if (Img != null)
                    {
                        foreach (var Imgitem in Img)
                        {
                            var ImgName = SaveImg(Imgitem);
                            nPurchaseNote.PurchaseNoteImg.Add(new PurchaseNoteImg { IsEnable = true, Img = ImgName, ImgType = Imgitem.ContentType, CreateAt = DateTime.UtcNow, CreateBy = UserBy });
                            //db.SaveChanges();
                            //PurchaseNoteList = RMA.PurchaseNote.ToList();
                        }
                    }
                    RMA.PurchaseNote.Add(nPurchaseNote);
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
                    var nPurchaseNote = new PurchaseNote { IsEnable = true, Note = Note, NoteType = "txt", CreateAt = DateTime.UtcNow, CreateBy = UserBy };
                    PurchaseNoteList.Add(nPurchaseNote);
                    if (Img != null)
                    {
                        foreach (var Imgitem in Img)
                        {
                            MemoryStream target = new MemoryStream();
                            Imgitem.InputStream.CopyTo(target);
                            byte[] data = target.ToArray();
                            string ImgName = Convert.ToBase64String(data, 0, data.Length);
                            nPurchaseNote.PurchaseNoteImg.Add(new PurchaseNoteImg { IsEnable = true, Img = ImgName, ImgType = "Url", CreateAt = DateTime.UtcNow, CreateBy = UserBy });
                        }
                    }
                    Session["RMAPurchaseNote" + SID] = PurchaseNoteList;
                }
                var datalist = PurchaseNoteList.OrderByDescending(x => x.CreateAt).Select(x => new { CreateAt = x.CreateAt.ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss"), x.CreateBy, x.Note, x.NoteType, PurchaseNoteImg = x.PurchaseNoteImg.Select(y => new PurchaseNoteImg { Img = y.Img, ImgType = y.ImgType }).ToList() }).ToList();
                return Json(new { status = true, datalist = datalist }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, Errmsg = ex.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult CreatNoteImg(int? ID, string SID,List< HttpPostedFileBase> Img)
        {
            try
            {
                if (Img == null)
                {
                    return Json(new { status = false, Errmsg = "沒有圖檔" }, JsonRequestBehavior.AllowGet);
                }
                var PurchaseNoteList = new List<PurchaseNote>();
                var RMA = db.RMA.Find(ID);
                foreach (var Imgitem in Img)
                {


                    var NoteType = Imgitem.ContentType;

                    if (ID.HasValue && ID != 0)
                    {
                        var Note = SaveImg(Imgitem);
                        RMA.PurchaseNote.Add(new PurchaseNote { IsEnable = true, Note = Note, NoteType = "Url", CreateAt = DateTime.UtcNow, CreateBy = UserBy });
                        db.SaveChanges();
                        PurchaseNoteList = RMA.PurchaseNote.ToList();
                    }
                    else
                    {
                        MemoryStream target = new MemoryStream();

                        Imgitem.InputStream.CopyTo(target);
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
            var SerialsLlist = RMASKU.RMASerialsLlist.Where(x => x.IsEnable && x.SerialsType == "RMAIn").Select(x => new RMASerial { Serial = x.SerialsNo, Warehouse = x.Warehouse.Name, Reason = x.Reason, Carrier = GetSerialListCarrier(x), ReturnTracking = GetSerialListTracking(x) }).ToList();
            var partial = ControlToString("~/Views/Shared/GetRMASerialList.cshtml", SerialsLlist);
            //var partial = Engine.Razor.RunCompile(template, "templateKey", null, new { Name = "World" });
            return Json(new { status = true, partial }, JsonRequestBehavior.AllowGet);
        }

        private string GetSerialListTracking(RMASerialsLlist RMASerialsLlist)
        {
            var Tracking = "";
            foreach (var item in RMASerialsLlist.RMASKU.RMAOrderSerialsLlist.Where(x => x.IsEnable && x.SerialsNo == RMASerialsLlist.SerialsNo))
            {
                Tracking = item.ReturnTracking;
            }
            return Tracking;
        }

        private string GetSerialListCarrier(RMASerialsLlist RMASerialsLlist)
        {
            var Carrier = "";
            foreach (var item in RMASerialsLlist.RMASKU.RMAOrderSerialsLlist.Where(x => x.IsEnable && x.SerialsNo == RMASerialsLlist.SerialsNo))
            {
                Carrier = item.Carrier;
            }
            return Carrier;
        }

        public ActionResult Returnedserials(int ID)
        {
            var companyList = db.Company.AsNoTracking().Where(c => c.IsEnable).ToList();
            ViewBag.Company = companyList.Where(c => !c.ParentID.HasValue).Select(c => new SelectListItem() { Text = c.Name, Value = c.ID.ToString() }).ToList();
            var RMASKU = db.RMASKU.Find(ID);
            Session.Remove("RMASKU" + ID);
            return View(RMASKU);
        }
        [HttpPost]
        public ActionResult Returnedserials(RMASKU RMASKU)
        {
            var errormsg = new List<string>();
            var companyList = db.Company.AsNoTracking().Where(c => c.IsEnable).ToList();
            ViewBag.Company = companyList.Where(c => !c.ParentID.HasValue).Select(c => new SelectListItem() { Text = c.Name, Value = c.ID.ToString() }).ToList();
            var oRMASKU = db.RMASKU.Find(RMASKU.ID);
            var sRMASKU = (RMASKU)Session["RMASKU" + RMASKU.ID];
            oRMASKU.ReceivedBy = sRMASKU.ReceivedBy;
            oRMASKU.ReceivedAt = sRMASKU.ReceivedAt;
          
                foreach (var RMASerialitem in sRMASKU.RMASerialsLlist)
                {
                    oRMASKU.RMASerialsLlist.Add(RMASerialitem);
                    var ReturnWarehouseID = 0;
                    if (int.TryParse(db.WarehouseSummary.Where(x => x.Type == "SCID" && x.WarehouseID == RMASerialitem.WarehouseID).FirstOrDefault()?.Val, out ReturnWarehouseID))
                    {
                        //SC加入RMA序號
                        //SC_WebService SCWS = new SC_WebService("tim@weypro.com", ApiPassword);
                        var SCOrderID = oRMASKU.RMA.OrderID.Value;
                        var order = SCWS.Get_OrderData(SCOrderID).Order;//去SC抓訂單資料
                        if (UpdateSC)
                        {
                            order.OrderCreationSourceApplication = SCService.OrderCreationSourceApplicationType.PointOfSale;
                        }
                        if (!UpdateSC || SCWS.Update_Order(order))
                        {
                        foreach (var item in order.Items.Where(x => x.ProductID == oRMASKU.SkuNo))
                        {
                            var OrderItemID = order.Items.Where(x => x.ProductID == item.ProductID).FirstOrDefault().ID;
                            var SCRMA_Item = SCWS.Get_RMA_Item(SCOrderID)?.Where(x => x.OriginalOrderItemID == OrderItemID).FirstOrDefault();
                            if (SCRMA_Item != null)//沒資料就新增
                            {
                                if (SCRMA_Item.QtyReturned > SCRMA_Item.QtyReceived)//比對數量
                                {
                                    if (UpdateSC)
                                    {
                                        try
                                        {
                                            var SerialsNo = "";
                                            if (RMASerialitem.RMASKU.SKU.SerialTracking)//有序號管理就加入序號
                                            {
                                                SerialsNo = RMASerialitem.SerialsNo;
                                            }
                                            SCWS.Receive_RMA_Item(SCOrderID, oRMASKU.RMAItemID.Value, item.ProductID, item.Qty, ReturnWarehouseID, SerialsNo);//RMA入庫
                                        }
                                        catch (Exception ex)
                                        {
                                            errormsg.Add("Receive_RMA_Item錯誤");
                                        }
                                    }
                                }
                            }
                            if (UpdateSC && !errormsg.Any())//沒有錯誤才移除
                            {
                                try
                                {
                                    SCWS.Delete_ItemAllSerials(SCOrderID, item.ID);//SC上的序號移除
                                }
                                catch (Exception)
                                {
                                    errormsg.Add("Delete_ItemSerials錯誤");
                                }
                            }
                        }
                            if (UpdateSC)
                            {
                                order.OrderCreationSourceApplication = SCService.OrderCreationSourceApplicationType.Default;
                                SCWS.Update_Order(order);
                            }
                        }

                    }

                }
                db.SaveChanges();
                Session["RMASKU" + RMASKU.ID] = null;

            ViewBag.errormsg = string.Join("\\n\\r", errormsg);
            return View(oRMASKU);
        }
        public string FunSaveserials(string serials, string Reason, int RMASKUID, int WarehouseID)
        {
            var errormsg = new List<string>();
            var ReturnWarehouseID = 0;
            var dt = DateTime.UtcNow;
            if (int.TryParse(db.WarehouseSummary.Where(x => x.Type == "SCID" && x.WarehouseID == WarehouseID).FirstOrDefault()?.Val, out ReturnWarehouseID))
            {

                var RMASKU = db.RMASKU.Find(RMASKUID);
                if (Session["RMASKU" + RMASKUID] != null)
                {
                    RMASKU = (RMASKU)Session["RMASKU" + RMASKUID];
                }
                var SerialsLlistCount = RMASKU.RMASerialsLlist.Where(x => x.SerialsType == "RMAIn").Sum(x => x.SerialsQTY);//計算RMAIn的序號數
                if (SerialsLlistCount >= RMASKU.QTYOrdered)
                {
                    return "序號不可大於回收數";
                }
                var RMASerialsLlist = db.RMASerialsLlist.Where(x => x.SerialsNo == serials && x.RMASKU.SkuNo == RMASKU.SkuNo && x.SerialsType == "RMAIn" && !x.RMASerialsLlistC.Any());//檢查序號是否重複，同SKU序號不能新增,2019/02/05 加入有已出貨或是CM的紀錄, 就能重新在入庫
                if (!RMASerialsLlist.Any())
                {
                    var HaveOrderData = db.SerialsLlist.Where(x => x.SerialsType == "Order" && x.SerialsNo == serials && (x.PurchaseSKU.SkuNo == RMASKU.SkuNo || x.TransferSKU.SkuNo == RMASKU.SkuNo) && x.OrderID == RMASKU.RMA.OrderID).Any();

                    //if (!HaveOrderData)
                    //{
                    //    HaveOrderData = RMASKU.RMAOrderSerialsLlist.Where(x => x.SerialsNo == serials).Any();
                    //    if (!HaveOrderData)
                    //    {
                    //        var OrderItemData = GetOrderItemData(RMASKU.RMA.OrderID, null, null, 3);
                    //        foreach (var Orderitem in OrderItemData)
                    //        {
                    //            foreach (var item in Orderitem.Items.Where(x => x.SKU == RMASKU.SkuNo))
                    //            {
                    //                HaveOrderData = item.Serials.Where(x => x == serials).Any();
                    //                if (HaveOrderData)
                    //                {
                    //                    RMASKU.RMAOrderSerialsLlist.Add(new RMAOrderSerialsLlist
                    //                    {
                    //                        IsEnable = true,
                    //                        SerialsNo = serials,
                    //                        SerialsQTY = 1,
                    //                        CreateBy = UserBy,
                    //                        CreateAt = dt
                    //                    });
                    //                }
                    //            }
                    //        }
                    //    }
                    //}
                    if (HaveOrderData)
                    {
                        var nSerialsLlistIn = new RMASerialsLlist
                        {
                            IsEnable = true,
                            WarehouseID = WarehouseID,
                            Reason = Reason,
                            SerialsType = "RMAIn",
                            SerialsNo = serials.Trim(),
                            SerialsQTY = 1,
                            ReceivedBy = UserBy,
                            ReceivedAt = dt,
                            CreateBy = UserBy,
                            CreateAt = dt
                        };
                        RMASKU.RMASerialsLlist.Add(nSerialsLlistIn);
                        //加入 return reason = return to shipper+ sellable warehouse 自動移倉入庫
                        if (Reason == "16")
                        {
                            if (db.Warehouse.Find(WarehouseID).IsSellable)//可出貨倉
                            {
                                //開移倉單
                                var nTransfer = new Transfer
                                {
                                    IsEnable = true,
                                    Title = RMASKU.RMA.OrderID + "_return to shipper",
                                    FromWID = WarehouseID,
                                    ToWID = WarehouseID,
                                    Status = "Completed",
                                    Interim = 2,
                                    CreateBy = "RMAAPI",
                                    CreateAt = dt
                                };
                                var nTransferSKU = new TransferSKU
                                {
                                    IsEnable = true,
                                    QTY = 1,
                                    SkuNo = RMASKU.SkuNo,
                                    Name = RMASKU.Name,
                                    CreateBy = "RMAAPI",
                                    CreateAt = dt
                                };

                                foreach (var Serialitem in RMASKU.RMASerialsLlist)
                                {
                                    var nSerialsLlist = new SerialsLlist
                                    {
                                        IsEnable = true,
                                        SerialsNo = Serialitem.SerialsNo.Trim(),
                                        SerialsQTY = 1,
                                        SerialsType = "TransferIn",
                                        CreateBy = "RMAAPI",
                                        CreateAt = dt,
                                        ReceivedBy = "RMAAPI",
                                        ReceivedAt = dt,
                                    };
                                    nTransferSKU.SerialsLlist.Add(nSerialsLlist);
                                    var nRMASerialsLlist = new RMASerialsLlist
                                    {
                                        IsEnable = true,
                                        RMASKUID = RMASKUID,
                                        RMASerialsLlistP = nSerialsLlistIn,
                                        SerialsNo = Serialitem.SerialsNo.Trim(),
                                        SerialsQTY = -1,
                                        SerialsType = "TransferOut",
                                        CreateBy = "RMAAPI",
                                        CreateAt = dt,
                                        ReceivedBy = "RMAAPI",
                                        ReceivedAt = dt,
                                    };
                                    nTransferSKU.RMASerialsLlist.Add(nRMASerialsLlist);
                                }
                                nTransfer.TransferSKU.Add(nTransferSKU);
                                db.Transfer.Add(nTransfer);
                            }
                        }
                        db.SaveChanges();
                        try
                        {
                            if (int.TryParse(db.WarehouseSummary.Where(x => x.Type == "SCID" && x.WarehouseID == WarehouseID).FirstOrDefault()?.Val, out ReturnWarehouseID))
                            {
                                //SC加入RMA序號
                                //SC_WebService SCWS = new SC_WebService("tim@weypro.com", ApiPassword);
                                var SCOrderID = RMASKU.RMA.OrderID.Value;
                                var order = SCWS.Get_OrderData(SCOrderID).Order;//去SC抓訂單資料
                                if (UpdateSC)
                                {
                                    order.OrderCreationSourceApplication = SCService.OrderCreationSourceApplicationType.PointOfSale;
                                }
                                if (!UpdateSC || SCWS.Update_Order(order))
                                {
                                    foreach (var item in order.Items.Where(x => x.ProductID == RMASKU.SkuNo))
                                    {
                                        var OrderItemID = order.Items.Where(x => x.ProductID == item.ProductID).FirstOrDefault().ID;
                                        var SCRMA_Item = SCWS.Get_RMA_Item(SCOrderID)?.Where(x => x.OriginalOrderItemID == OrderItemID).FirstOrDefault();
                                        if (SCRMA_Item != null)//沒資料就新增
                                        {
                                            if (SCRMA_Item.QtyReturned > SCRMA_Item.QtyReceived)//比對數量
                                            {
                                                if (UpdateSC)
                                                {
                                                    try
                                                    {
                                                        var SerialsNo = "";
                                                        if (RMASKU.SKU.SerialTracking)//有序號管理就加入序號
                                                        {
                                                            SerialsNo = serials;
                                                        }
                                                        SCWS.Receive_RMA_Item(SCOrderID, RMASKU.RMAItemID.Value, item.ProductID, item.Qty, ReturnWarehouseID, SerialsNo);//RMA入庫
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        errormsg.Add("Receive_RMA_Item錯誤");
                                                    }
                                                }
                                            }
                                        }
                                        if (UpdateSC && !errormsg.Any())//沒有錯誤才移除
                                        {
                                            try
                                            {
                                                SCWS.Delete_ItemAllSerials(SCOrderID, item.ID);//SC上的序號移除
                                            }
                                            catch (Exception)
                                            {
                                                errormsg.Add("Delete_ItemSerials錯誤");
                                            }
                                        }
                                    }
                                    if (UpdateSC)
                                    {
                                        order.OrderCreationSourceApplication = SCService.OrderCreationSourceApplicationType.Default;
                                        SCWS.Update_Order(order);
                                    }
                                }

                            }
                        }
                        catch (Exception ex)
                        {

                            errormsg.Add("Receive_RMA_Item錯誤");
                        }
                        Session["RMASKU" + RMASKUID] = RMASKU;
                        return "";
                    }
                    else
                    {
                        return "序號沒有出貨資料";
                    }
                }
                else
                {
                    return "序號已經存在";
                }
            }
            else
            {
                return "SCID錯誤";
            }
        }
        public ActionResult Saveserials(string serials, string Reason, int RMASKUID, int WarehouseID)
        {
            var Errmsg = FunSaveserials(serials, Reason, RMASKUID, WarehouseID);
            if (string.IsNullOrWhiteSpace(Errmsg))
            {
                return Json(new { status = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = false, Errmsg }, JsonRequestBehavior.AllowGet);
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
                    ProductName = x.SKU.SkuLang.Where(y => y.LangID == LangID).FirstOrDefault()?.Name,
                    ReturnedSerialsNo = x.RMASerialsLlist.FirstOrDefault()?.SerialsNo,
                    OrderSerialsNo = x.RMAOrderSerialsLlist.FirstOrDefault()?.SerialsNo,
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
        public ActionResult SCID(string id)
        {
            var RMA = db.RMA.Where(x => x.SCRMA == id).FirstOrDefault();

            if (RMA == null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Edit", new { id = RMA.ID });
            }

        }
        public ActionResult DelSerialsNo(int RMASKUID, string serials)
        {
            var Errmsg = "";
            var sRMASKU = (RMASKU)Session["RMASKU" + RMASKUID];
            foreach (var item in sRMASKU.RMASerialsLlist.Where(x=>x.SerialsNo== serials).ToList())
            {
                sRMASKU.RMASerialsLlist.Remove(item);
            }
            Session["RMASKU" + RMASKUID] = sRMASKU;
            return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ReturnLabel(int ID, int[] IDList)
        {
            ViewBag.rmaid = ID;
            TempData["IDList"+ ID] = IDList;
            var RMA = db.RMA.Find(ID);
            var RMAOrderTracking = new RMAOrderTracking();
            if (RMA.RMASKU.Any(x => x.IsEnable))
            {
                var Warehouse = RMA.RMASKU.FirstOrDefault().Warehouse;
                ViewBag.Shippingmethods = Warehouse.WarehouseSummary.Where(x => x.IsEnable && x.Type == "Shippingmethods").FirstOrDefault()?.Val;
                RMAOrderTracking.ToName = Warehouse.Name;
                RMAOrderTracking.ToAddress1 = Warehouse.Address1;
                RMAOrderTracking.ToAddress2 = Warehouse.Address2;
                RMAOrderTracking.ToCity = Warehouse.City;
                RMAOrderTracking.ToState = Warehouse.State;
                RMAOrderTracking.ToPostcode = Warehouse.Postcode;
                RMAOrderTracking.ToCountry = Warehouse.Country;

                var SCOrderID = RMA.OrderID.Value;
                var order = SCWS.Get_OrderData(SCOrderID).Order;//去SC抓訂單資料
                RMAOrderTracking.FromName = order.ShippingAddress.FirstName+" "+ order.ShippingAddress.LastName;
                RMAOrderTracking.FromAddress1 = order.ShippingAddress.StreetLine1;
                RMAOrderTracking.FromAddress2 = order.ShippingAddress.StreetLine2;
                RMAOrderTracking.FromCity = order.ShippingAddress.City;
                RMAOrderTracking.FromState = order.ShippingAddress.StateName;
                RMAOrderTracking.FromPostcode = order.ShippingAddress.PostalCode;
                RMAOrderTracking.FromCountry = order.ShippingAddress.CountryName;
            }
            var html = RenderPartialViewToString("ReturnLabel", RMAOrderTracking);
            return Json(new { status = true, html }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult ReturnLabel(RMAOrderTracking RMAOrderTracking,int rmaid )
        {
            var RMA = db.RMA.Find(rmaid);
            int[] IDList = (int[])TempData["IDList" + rmaid];
            var CreateBy = UserBy;
            var CreateAt = DateTime.UtcNow;
            RMAOrderTracking.CreateAt = CreateAt;
            RMAOrderTracking.CreateBy = CreateBy;
            foreach (var RMASKUitem in RMA.RMASKU.Where(x=>x.IsEnable))
            {
                foreach (var Serialitem in RMASKUitem.RMAOrderSerialsLlist.Where(x=>x.IsEnable&& IDList.Contains(x.ID)))
                {
                    Serialitem.RMAOrderTracking = RMAOrderTracking;
                }
            }
            db.SaveChanges();
            return RedirectToAction("Edit", new { id = rmaid });
        }
        [HttpPost]
        public ActionResult Receiveitem(int id,string Serial)
        {
            var RMA = db.RMA.Find(id);

            return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Trackgroup(int id)
        {
            var RMAOrderTracking = db.RMAOrderTracking.Find(id);
            if (RMAOrderTracking == null)
            {
                RMAOrderTracking = new RMAOrderTracking();
            }
            var html = RenderPartialViewToString("Trackgroup", RMAOrderTracking);
            return Json(new { status = true, html }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult CreatGTracking(int? ID, int[] IDList, string Carrier, string ReturnTracking)
        {
            try
            {
                var PurchaseNoteList = new List<PurchaseNote>();
                if (ID.HasValue && ID != 0)
                {
                    var RMAOrderTracking = db.RMAOrderTracking.Find(ID);
                    RMAOrderTracking.Carrier = Carrier;
                    RMAOrderTracking.ReturnTracking = ReturnTracking;
                    db.SaveChanges();
                }
                else
                {
                    var RMAOrderSerialsLlist = db.RMAOrderSerialsLlist.Where(x => IDList.Contains(x.ID) && x.IsEnable);
                    var nRMAOrderTracking = new RMAOrderTracking { IsEnable = true, Carrier = Carrier, ReturnTracking = ReturnTracking, CreateAt = DateTime.UtcNow, CreateBy = UserBy };
                    foreach (var item in RMAOrderSerialsLlist)
                    {
                        if (!item.RMAOrderTrackingID.HasValue)
                        {
                            item.RMAOrderTracking = nRMAOrderTracking;
                        }
                        else
                        {
                            item.RMAOrderTracking.Carrier = Carrier;
                            item.RMAOrderTracking.ReturnTracking = ReturnTracking;
                            item.RMAOrderTracking.UpdateAt = DateTime.UtcNow;
                            item.RMAOrderTracking.UpdateBy = UserBy;
                        }
                    }
                    db.SaveChanges();
                }
                return Json(new { status = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, Errmsg = ex.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }
        //[HttpPost]
        //public ActionResult ReturnTracking( RMA RMA)
        //{
        //    return View(RMA);
        //}
    }
}