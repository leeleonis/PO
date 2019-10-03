using PurchaseOrderSys.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using inventorySKU;
using System.Data.Entity;
namespace PurchaseOrderSys.Controllers
{
    [CheckSession]
    public class WarehouseInventoryController : BaseController
    {
        // GET: Warehouse
        public ActionResult Index(int ID, string Product, int? FulfillableMin, int? FulfillableMax)
        {
            ViewBag.UpdateAt = db.inventory.OrderByDescending(x => x.CreateAt).FirstOrDefault().CreateAt;
            ViewBag.WarehouseID = ID;
            var Warehouse = db.Warehouse.Find(ID);
            var WarehouseInventoryVM = new WarehouseInventoryVM();
            WarehouseInventoryVM.Name = Warehouse.Name;
            WarehouseInventoryVM.WarehouseType = Warehouse.Type;
            WarehouseInventoryVM.WarehouseVM = Warehouse.inventory.OrderByDescending(x=>x.Fulfillable).Select(x => new WarehouseVM
            {
                Name = x.Warehouse.Name,
                SKU = x.SkuID,
                Aggregate = x.Aggregate,
                Awaiting = x.Awaiting,
                Fulfillable = x.Fulfillable,
                TransferOutQTY = x.TransferOutQTY,
                TransferInQTY = x.TransferInQTY,
                WTransferOutQTY = x.WTransferOutQTY,
                WTransferInQTY = x.WTransferInQTY,
                TransferAwaiting = x.UnfulfillableTransit,
                Velocity = x.TotalVelocity
            });
            WarehouseInventoryVM.Fulfillable = WarehouseInventoryVM.WarehouseVM.Sum(x => x.Fulfillable);
            WarehouseInventoryVM.Awaiting = WarehouseInventoryVM.WarehouseVM.Sum(x => x.Awaiting);
            WarehouseInventoryVM.Aggregate = WarehouseInventoryVM.WarehouseVM.Sum(x => x.Aggregate);
            WarehouseInventoryVM.UnfulfillableTransit = WarehouseInventoryVM.WarehouseVM.Sum(x => x.TransferAwaiting);
            WarehouseInventoryVM.TransferOutQTY = WarehouseInventoryVM.WarehouseVM.Sum(x => x.TransferOutQTY);
            WarehouseInventoryVM.TransferInQTY = WarehouseInventoryVM.WarehouseVM.Sum(x => x.TransferInQTY);
            WarehouseInventoryVM.WTransferOutQTY = WarehouseInventoryVM.WarehouseVM.Sum(x => x.WTransferOutQTY);
            WarehouseInventoryVM.WTransferInQTY = WarehouseInventoryVM.WarehouseVM.Sum(x => x.WTransferInQTY);
            WarehouseInventoryVM.TotalVelocity = WarehouseInventoryVM.WarehouseVM.Sum(x => x.Velocity);
            WarehouseInventoryVM.Location = Warehouse.Location;
            WarehouseInventoryVM.Countries = Warehouse.Countries;
            WarehouseInventoryVM.Marketplace = Warehouse.Marketplace;
            WarehouseInventoryVM.Company = Warehouse.Company;
            return View(WarehouseInventoryVM);
        }
        public ActionResult oIndex(int ID, string Product, int? FulfillableMin, int? FulfillableMax)
        {
            ViewBag.WarehouseID = ID;
            var Warehouse = db.Warehouse.Find(ID);
            var WarehouseInventoryVM = new WarehouseInventoryVM();
            WarehouseInventoryVM.Name = Warehouse.Name;
            WarehouseInventoryVM.WarehouseType = Warehouse.Type;
            WarehouseInventoryVM.WarehouseVM = GetWarehouseVMList(Warehouse, Product, FulfillableMin, FulfillableMax);
            WarehouseInventoryVM.Fulfillable = WarehouseInventoryVM.WarehouseVM.Sum(x=>x.Fulfillable);
            WarehouseInventoryVM.Awaiting = WarehouseInventoryVM.WarehouseVM.Sum(x => x.Awaiting);
            WarehouseInventoryVM.Aggregate = WarehouseInventoryVM.WarehouseVM.Sum(x => x.Aggregate);
            WarehouseInventoryVM.UnfulfillableTransit = WarehouseInventoryVM.WarehouseVM.Sum(x => x.TransferAwaiting);
            WarehouseInventoryVM.TransferOutQTY = WarehouseInventoryVM.WarehouseVM.Sum(x => x.TransferOutQTY);
            WarehouseInventoryVM.TransferInQTY = WarehouseInventoryVM.WarehouseVM.Sum(x => x.TransferInQTY);
            WarehouseInventoryVM.WTransferOutQTY = WarehouseInventoryVM.WarehouseVM.Sum(x => x.WTransferOutQTY);
            WarehouseInventoryVM.WTransferInQTY = WarehouseInventoryVM.WarehouseVM.Sum(x => x.WTransferInQTY);
            WarehouseInventoryVM.TotalVelocity = WarehouseInventoryVM.WarehouseVM.Sum(x => x.Velocity);
            WarehouseInventoryVM.Location = Warehouse.Location;
            WarehouseInventoryVM.Countries = Warehouse.Countries;
            WarehouseInventoryVM.Marketplace = Warehouse.Marketplace;
            WarehouseInventoryVM.Company = Warehouse.Company;
            return View(WarehouseInventoryVM);
        }
        public ActionResult IndexAll()
        {
            ViewBag.UpdateAt = db.inventory.OrderByDescending(x => x.CreateAt).FirstOrDefault().CreateAt;
            var WarehouseAllVMList = db.inventory.Include(x => x.Warehouse).GroupBy(x => x.WarehouseID).Select(x => new WarehouseAllVM
            {
                ID = x.Key,
                Name = x.FirstOrDefault().Warehouse.Name,
                Type = x.FirstOrDefault().Warehouse.Type,
                Aggregate = x.Sum(y => y.Aggregate),
                Awaiting = x.Sum(y => y.Awaiting),
                Fulfillable = x.Sum(y => y.Fulfillable),
                TransferOutQTY = x.Sum(y => y.TransferOutQTY),
                TransferInQTY = x.Sum(y => y.TransferInQTY),
                WTransferOutQTY = x.Sum(y => y.WTransferOutQTY),
                WTransferInQTY = x.Sum(y => y.WTransferInQTY),
            });
            return View(WarehouseAllVMList);
        }
        public ActionResult OIndexAll()
        {
            var WarehouseAllVMList = new List<WarehouseAllVM>();
            var WarehouseList = db.Warehouse.Where(x => x.IsEnable).Include(x=>x.WarehouseSummary).ToList();
            var POSerialsLlist = db.SerialsLlist.AsNoTracking().Where(x => x.IsEnable && !x.SerialsLlistC.Any(y => y.IsEnable) && x.SerialsType == "PO" && x.PurchaseSKU.IsEnable && x.PurchaseSKU.PurchaseOrder.IsEnable).Include(x => x.PurchaseSKU).Include(x => x.PurchaseSKU.PurchaseOrder).ToList();
            var InSerialsLlist = db.SerialsLlist.AsNoTracking().Where(x => x.IsEnable && !x.SerialsLlistC.Any(y => y.IsEnable) && x.SerialsType == "TransferIn" && x.TransferSKU.IsEnable && x.TransferSKU.Transfer.IsEnable).Include(x => x.TransferSKU).Include(x => x.TransferSKU.Transfer).ToList();
            var OutSerialsLlist = db.SerialsLlist.AsNoTracking().Where(x => x.IsEnable && !x.SerialsLlistC.Any(y => y.IsEnable) && x.SerialsType == "TransferOut" && x.TransferSKU.IsEnable && x.TransferSKU.Transfer.IsEnable).Include(x => x.TransferSKU).Include(x => x.TransferSKU.Transfer).ToList();
            var RMAINSerialsLlist = db.RMASerialsLlist.AsNoTracking().Where(x => x.IsEnable && !x.RMASerialsLlistC.Any(y => y.IsEnable) && x.SerialsType == "RMAIn" && x.RMASKU.IsEnable && x.RMASKU.RMA.IsEnable).Include(x => x.RMASKU).Include(x => x.RMASKU.RMA).ToList();
            var RMAOUTSerialsLlist = db.RMASerialsLlist.AsNoTracking().Where(x => x.IsEnable && !x.RMASerialsLlistC.Any(y => y.IsEnable) && x.SerialsType == "TransferOut" && x.RMASKU.IsEnable && x.RMASKU.RMA.IsEnable && x.TransferSKU.IsEnable && x.TransferSKU.Transfer.IsEnable && !x.TransferSKU.SerialsLlist.Where(y => y.IsEnable && y.SerialsType == "TransferIn").Any()).Include(x => x.RMASKU).Include(x => x.RMASKU.RMA).Include(x => x.TransferSKU).Include(x => x.TransferSKU.Transfer).ToList();
            //var OrderSerialsLlist = db.SerialsLlist.AsNoTracking().Where(x => x.IsEnable && x.SerialsType == "Order" && x.CreateAt >= sdt && x.CreateAt <= edt).Include(x => x.PurchaseSKU).Include(x => x.PurchaseSKU.PurchaseOrder).Include(x => x.TransferSKU).Include(x => x.TransferSKU.Transfer).ToList();
            var inventory = new List<inventory>();
            foreach (var Warehouseitem in WarehouseList)
            {
                var SCID = Warehouseitem.WarehouseSummary.Where(x => x.Type == "SCID").FirstOrDefault()?.Val;
                var Awaitinglist = GetAwaitingCount("", SCID);
               //PO
                    var POQTY = POSerialsLlist.Where(x => x.PurchaseSKU.PurchaseOrder.WarehouseID == Warehouseitem.ID ).Sum(x => x.SerialsQTY) ?? 0;
                    //移倉入庫
                    var TinQTY = InSerialsLlist.Where(x => x.TransferSKU.Transfer.ToWID == Warehouseitem.ID).Sum(x => x.SerialsQTY) ?? 0;
                    //移倉已Shipped
                    var FOutQTY = Math.Abs(OutSerialsLlist.Where(x => x.TransferSKU.Transfer.FromWID == Warehouseitem.ID && x.TransferSKU.Transfer.TransferType != "Winit" && (x.TransferSKU.Transfer.Status == "Shipped" || x.TransferSKU.Transfer.Status == "Received") ).Sum(x => x.SerialsQTY) ?? 0);
                    var WFOutQTY = Math.Abs(OutSerialsLlist.Where(x => x.TransferSKU.Transfer.FromWID == Warehouseitem.ID && x.TransferSKU.Transfer.TransferType == "Winit" && (x.TransferSKU.Transfer.Status == "Shipped" || x.TransferSKU.Transfer.Status == "Received") ).Sum(x => x.SerialsQTY) ?? 0);
                    var TOutQTY = Math.Abs(OutSerialsLlist.Where(x => x.TransferSKU.Transfer.ToWID == Warehouseitem.ID && x.TransferSKU.Transfer.TransferType != "Winit" && (x.TransferSKU.Transfer.Status == "Shipped" || x.TransferSKU.Transfer.Status == "Received") ).Sum(x => x.SerialsQTY) ?? 0);
                    var WTOutQTY = Math.Abs(OutSerialsLlist.Where(x => x.TransferSKU.Transfer.ToWID == Warehouseitem.ID && x.TransferSKU.Transfer.TransferType == "Winit" && (x.TransferSKU.Transfer.Status == "Shipped" || x.TransferSKU.Transfer.Status == "Received")).Sum(x => x.SerialsQTY) ?? 0);

                    //移倉未Shipped
                    var UnFOutQTY = Math.Abs(OutSerialsLlist.Where(x => x.TransferSKU.Transfer.FromWID == Warehouseitem.ID && x.TransferSKU.Transfer.TransferType != "Winit" && x.TransferSKU.Transfer.Status == "Requested" ).Sum(x => x.SerialsQTY) ?? 0);
                    var UnWFOutQTY = Math.Abs(OutSerialsLlist.Where(x => x.TransferSKU.Transfer.FromWID == Warehouseitem.ID && x.TransferSKU.Transfer.TransferType == "Winit" && x.TransferSKU.Transfer.Status == "Requested" ).Sum(x => x.SerialsQTY) ?? 0);
                    var UnTOutQTY = Math.Abs(OutSerialsLlist.Where(x => x.TransferSKU.Transfer.ToWID == Warehouseitem.ID && x.TransferSKU.Transfer.TransferType != "Winit" && x.TransferSKU.Transfer.Status == "Requested" ).Sum(x => x.SerialsQTY) ?? 0);
                    var UnWTOutQTY = Math.Abs(OutSerialsLlist.Where(x => x.TransferSKU.Transfer.ToWID == Warehouseitem.ID && x.TransferSKU.Transfer.TransferType == "Winit" && x.TransferSKU.Transfer.Status == "Requested" ).Sum(x => x.SerialsQTY) ?? 0);

                    //待出貨
                    var Awaiting = Awaitinglist.Sum(x => x.QTY);
                    //RMA入庫
                    var RMAINQTY = RMAINSerialsLlist.Where(x => x.WarehouseID == Warehouseitem.ID ).Sum(x => x.SerialsQTY) ?? 0;
                    //RMA移倉已Shipped
                    var FRMAOUTQTY = Math.Abs(RMAOUTSerialsLlist.Where(x => x.TransferSKU.Transfer.FromWID == Warehouseitem.ID && (x.TransferSKU.Transfer.Status == "Shipped" || x.TransferSKU.Transfer.Status == "Received") ).Sum(x => x.SerialsQTY) ?? 0);
                    var TRMAOUTQTY = Math.Abs(RMAOUTSerialsLlist.Where(x => x.TransferSKU.Transfer.ToWID == Warehouseitem.ID && (x.TransferSKU.Transfer.Status == "Shipped" || x.TransferSKU.Transfer.Status == "Received") ).Sum(x => x.SerialsQTY) ?? 0);
                    //RMA移倉未Shipped
                    var UnFRMAOUTQTY = Math.Abs(RMAOUTSerialsLlist.Where(x => x.TransferSKU.Transfer.FromWID == Warehouseitem.ID && x.TransferSKU.Transfer.Status == "Requested" ).Sum(x => x.SerialsQTY) ?? 0);
                    var UnTRMAOUTQTY = Math.Abs(RMAOUTSerialsLlist.Where(x => x.TransferSKU.Transfer.ToWID == Warehouseitem.ID && x.TransferSKU.Transfer.Status == "Requested").Sum(x => x.SerialsQTY) ?? 0);


                //var WarehouseVM = GetWarehouseVMList(Warehouse, null, null, null);
                var tTinQTY = InSerialsLlist.Where(x => x.TransferSKU.Transfer.ToWID == Warehouseitem.ID).GroupBy(x => x.TransferSKU.SkuNo).Select(x => new { x.Key, count = x.Count(), list = x.ToList() }).ToList();
                var tPOQTY = POSerialsLlist.Where(x => x.PurchaseSKU.PurchaseOrder.WarehouseID == Warehouseitem.ID).GroupBy(x => x.PurchaseSKU.SkuNo).Select(x => new { x.Key, count = x.Count(), list = x.ToList() }).ToList();
                var tRMAINQTY = RMAINSerialsLlist.Where(x => x.WarehouseID == Warehouseitem.ID).GroupBy(x => x.RMASKU.SkuNo).Select(x => new { x.Key, count = x.Count(), list = x.ToList() }).ToList();
                WarehouseAllVMList.Add(new WarehouseAllVM
                {
                    ID = Warehouseitem.ID,
                    Name = Warehouseitem.Name,
                    Type = Warehouseitem.Type,
                    Aggregate = POQTY + TinQTY + RMAINQTY + UnFOutQTY + UnWFOutQTY + UnTOutQTY + UnWTOutQTY + UnFRMAOUTQTY + UnTRMAOUTQTY - Awaiting,
                    Awaiting = Awaiting,
                    Fulfillable = POQTY + TinQTY + RMAINQTY + UnFOutQTY + UnWFOutQTY + UnTOutQTY + UnWTOutQTY + UnFRMAOUTQTY + UnTRMAOUTQTY,
                    TransferOutQTY = FOutQTY + FRMAOUTQTY,
                    TransferInQTY = TOutQTY + TRMAOUTQTY,
                    WTransferOutQTY = WFOutQTY,
                    WTransferInQTY = WTOutQTY,
                    //Aggregate = WarehouseVM.Sum(x => x.Aggregate),
                    //Awaiting = WarehouseVM.Sum(x => x.Awaiting),
                    //Fulfillable = WarehouseVM.Sum(x => x.Fulfillable),
                    //TransferOutQTY = WarehouseVM.Sum(x => x.TransferOutQTY)
                });
            }
            return View(WarehouseAllVMList);
        }




        public ActionResult IndexN(int ID, string Product, int? FulfillableMin, int? FulfillableMax)
        {
            ViewBag.WarehouseID = ID;
            var Warehouse = db.Warehouse.Find(ID);
            var SCID = Warehouse.WarehouseSummary.Where(x => x.Type == "SCID").FirstOrDefault().Val;
            var Awaitinglist = GetAwaitingCount("", SCID);
            var WarehouseVM = new List<WarehouseVM>();
            var WarehouseInventoryVM = new WarehouseInventoryVM();
            var AllSKUList = db.SKU.Where(x => x.IsEnable && x.Status == 1).Select(x => new { x.SkuID, x.SkuLang.FirstOrDefault().Name }).ToList();
            if (!string.IsNullOrWhiteSpace(Product))
            {
                AllSKUList = AllSKUList.Where(x => x.SkuID == Product).ToList();
            }
            WarehouseInventoryVM.WarehouseType = Warehouse.Type;
            //WarehouseInventoryVM.Fulfillable = Warehouse.Fulfillable;
            WarehouseInventoryVM.Location = Warehouse.Location;
            WarehouseInventoryVM.Countries = Warehouse.Countries;
            WarehouseInventoryVM.Marketplace = Warehouse.Marketplace;
            if (Warehouse.Type == "Interim")
            {
            }
            else
            {
                var SerialsLlist = db.SerialsLlist.Where(x => (x.PurchaseSKU.IsEnable && x.PurchaseSKU.PurchaseOrder.IsEnable && x.PurchaseSKU.PurchaseOrder.WarehouseID == ID) || (x.TransferSKU.IsEnable && x.TransferSKU.Transfer.IsEnable && x.TransferSKU.Transfer.ToWID == ID)).ToList();


                foreach (var item in AllSKUList)
                {
                    var SerialsLlistitem = SerialsLlist.Where(x => (x.PurchaseSKU != null && x.PurchaseSKU.SkuNo == item.SkuID) || (x.TransferSKU != null && x.TransferSKU.SkuNo == item.SkuID)).ToList();
                    var nWarehouseVM = new WarehouseVM
                    {
                        Name = item.Name,
                        SKU = item.SkuID,
                        POQTY = SerialsLlistitem.Where(x => x.SerialsType == "PO").Sum(x => x.SerialsQTY).Value,
                        CMQTY = SerialsLlistitem.Where(x => x.SerialsType == "CM").Sum(x => x.SerialsQTY).Value,
                        OrderQTY = SerialsLlistitem.Where(x => x.SerialsType == "Order").Sum(x => x.SerialsQTY).Value,
                        TransferInQTY = SerialsLlistitem.Where(x => x.SerialsType == "TransferIn").Sum(x => x.SerialsQTY).Value,
                        TransferOutQTY = SerialsLlistitem.Where(x => x.SerialsType == "TransferOut").Sum(x => x.SerialsQTY).Value,
                        Velocity = 0,
                        DaysOfSupply = 0,
                        Aggregate = 0,//可上架的庫存總數
                        Awaiting = 0,//等待出貨的庫總量
                        Fulfillable = 0,
                        Unfulfillable = 0
                    };
                    WarehouseVM.Add(nWarehouseVM);
                }
                foreach (var item in WarehouseVM)
                {
                    item.Fulfillable = item.POQTY + item.CMQTY + item.OrderQTY + item.TransferInQTY + item.TransferOutQTY;
                    item.Unfulfillable = item.TransferOutQTY + item.TransferOutCloseQTY;
                    item.Awaiting = (Awaitinglist.Where(x => x.SKU == item.SKU && x.SCID == SCID).FirstOrDefault()?.QTY ?? 0) - item.TransferAwaiting;
                    item.Aggregate = item.Fulfillable - item.Awaiting;//Aggregate = Fulfillable - Awaiting dispatch
                    item.DaysOfSupply = item.Aggregate != 0 && item.Velocity != 0 ? item.Aggregate / item.Velocity / 30 : 0;  //Days of supply 算法: Aggregate / Velocity (30 days) / 30
                                                                                                                              //item.Fulfillable = item.Awaiting + item.Aggregate; //Fulfillable = Awaiting dispatch + Aggregate 2018/12/28 拿掉公式
                }
                WarehouseInventoryVM.WarehouseVM = WarehouseVM;
            }
            return View(WarehouseInventoryVM);
        }


        public ActionResult Statement(string SKU, int WarehouseID)
        {
            ViewBag.WarehouseID = WarehouseID;
            var PurchaseSKU = db.PurchaseSKU.Where(x => x.IsEnable && x.PurchaseOrder.IsEnable && x.SkuNo == SKU);
            var TransferSKU = db.TransferSKU.Where(x => x.IsEnable && x.Transfer.IsEnable && x.SkuNo == SKU);
            var NoNewRmaSKU = db.RMASKU.AsEnumerable().Where(x => x.IsEnable && x.RMA.IsEnable && x.SkuNo == SKU && x.RMASerialsLlist.Where(y => string.IsNullOrWhiteSpace(y.NewSkuNo)).Any());
            var NewRmaSKU = db.RMASKU.AsEnumerable().Where(x => x.IsEnable && x.RMA.IsEnable && x.RMASerialsLlist.Where(y => !string.IsNullOrWhiteSpace(y.NewSkuNo) && y.NewSkuNo == SKU).Any());
            var Awaitinglist = GetAwaitingCount(SKU, "");
            var AwaitingConut = Awaitinglist.Sum(x => x.QTY);
            var StatementVM = new List<StatementVM>();
            var Company = PurchaseSKU.FirstOrDefault()?.PurchaseOrder.Company.Name;
            var Name = PurchaseSKU.FirstOrDefault()?.Name;
            ViewBag.Company = Company;
            ViewBag.Name = Name;
            foreach (var item in PurchaseSKU)
            {
                var SerialsLlist = item.SerialsLlist.Where(y => !(y.SerialsType == "TransferOut" || y.SerialsType == "TransferIn") || ((y.SerialsType == "TransferOut" || y.SerialsType == "TransferIn") && y.TransferSKU.IsEnable && y.TransferSKU.Transfer.IsEnable));
                var Available = SerialsLlist.Where(x => !(x.SerialsType == "TransferOut" && x.TransferSKU.Transfer.Status == "Requested")).Sum(y => y.SerialsQTY);//實際
                var Aggregate = SerialsLlist.Sum(y => y.SerialsQTY);//可出貨
                foreach (var SerialsLlistitem in SerialsLlist.OrderByDescending(x => x.ID))
                {

                    var itemSKU = SerialsLlistitem.PurchaseSKU.SkuNo;
                    var Date = SerialsLlistitem.CreateAt.ToLocalTime();
                    var Supplier = "";
                    var Channel = "";
                    var ISType = "";
                    int? ID = null;
                    var Warehouse = "";
                    var Serial = "";
                    int? QTY;
                    int? BalanceAggregate;
                    int? BalanceAvailable;
                    decimal? ValueAvailable;
                    var OrderID = SerialsLlistitem.OrderID;
                    if (OrderID.HasValue)
                    {
                      
                    }
                    else
                    {
                        OrderID = SerialsLlistitem.SerialsLlistC.Where(x => x.OrderID.HasValue).FirstOrDefault()?.OrderID;
                    }
                    var price = SerialsLlistitem.PurchaseSKU.Price;
                    if (SerialsLlistitem.SerialsType == "PO")
                    {
                        Supplier = SerialsLlistitem.PurchaseSKU.PurchaseOrder.VendorLIst.Name;
                        ISType = "PO";
                        ID = SerialsLlistitem.PurchaseSKU.PurchaseOrder.ID;
                        Warehouse = SerialsLlistitem.PurchaseSKU.PurchaseOrder.WarehousePO.Name;
                    }
                    else if (SerialsLlistitem.SerialsType == "CM")
                    {
                        Supplier = SerialsLlistitem.PurchaseSKU.CreditMemo.PurchaseOrder.VendorLIst.Name;
                        ISType = "Credit Memo";
                        ID = SerialsLlistitem.PurchaseSKU.CreditMemo?.ID;
                        Warehouse = SerialsLlistitem.PurchaseSKU.CreditMemo.PurchaseOrder.WarehousePO.Name;
                    }
                    else if (SerialsLlistitem.SerialsType == "Order")
                    {
                        Supplier = SerialsLlistitem.PurchaseSKU.PurchaseOrder.VendorLIst.Name;
                        ISType = "Order Dispatch";
                        ID = SerialsLlistitem.OrderID;
                        Warehouse = GetOrderWarehouseName(SerialsLlistitem);
                    }
                    else if (SerialsLlistitem.SerialsType == "TransferOut")
                    {
                        Supplier = SerialsLlistitem.PurchaseSKU.PurchaseOrder.VendorLIst.Name;
                        if (SerialsLlistitem.TransferSKU.Transfer.Status == "Requested")
                        {
                            ISType = "Transfer(Wait)";
                        }
                        else
                        {
                            ISType = "Transfer(Out)";
                        }
                        ID = (SerialsLlistitem.TransferSKU.TransferID);
                        Warehouse = SerialsLlistitem.TransferSKU.Transfer.WarehouseFrom.Name;
                    }
                    else if (SerialsLlistitem.SerialsType == "TransferIn")
                    {
                        Supplier = SerialsLlistitem.PurchaseSKU.PurchaseOrder.VendorLIst.Name;
                        ISType = "Transfer(In)";
                        ID = SerialsLlistitem.TransferSKU.TransferID;
                        Warehouse = SerialsLlistitem.TransferSKU.Transfer.WarehouseTo.Name;
                    }
                    else if (SerialsLlistitem.SerialsType == "RMA")
                    {

                    }
                    else if (SerialsLlistitem.SerialsType == "Change")
                    {

                    }
                    Serial = SerialsLlistitem.SerialsNo;
                    QTY = SerialsLlistitem.SerialsQTY;
                    BalanceAggregate = Aggregate - AwaitingConut;
                    BalanceAvailable = Available;
                    ValueAvailable = Available * price;
                    if (SerialsLlistitem.SerialsType == "TransferOut" && SerialsLlistitem.TransferSKU.Transfer.Status == "Requested")
                    {
                        Aggregate -= SerialsLlistitem.SerialsQTY.Value;
                    }
                    else
                    {
                        Aggregate -= SerialsLlistitem.SerialsQTY.Value;
                        Available -= SerialsLlistitem.SerialsQTY.Value;
                    }
                    StatementVM.Add(new StatementVM
                    {
                        SKU = itemSKU,
                        OrderID = OrderID,
                        Date = Date,
                        Supplier = Supplier,
                        ID = ID,
                        Channel = Channel,
                        ISType = ISType,
                        Warehouse = Warehouse,
                        Serial = Serial,
                        QTY = QTY,
                        price = price,
                        BalanceAggregate = BalanceAggregate,
                        BalanceAvailable = BalanceAvailable,
                        ValueAvailable = ValueAvailable
                    });
                }


            }
            foreach (var item in TransferSKU)
            {
                var SerialsLlist = item.SerialsLlist.Where(y => !(y.SerialsType == "TransferOut" || y.SerialsType == "TransferIn") || ((y.SerialsType == "TransferOut" || y.SerialsType == "TransferIn") && y.TransferSKU.IsEnable && y.TransferSKU.Transfer.IsEnable));
                var Available = SerialsLlist.Where(x => !(x.SerialsType == "TransferOut" && x.TransferSKU.Transfer.Status == "Requested")).Sum(y => y.SerialsQTY);//實際
                var Aggregate = SerialsLlist.Sum(y => y.SerialsQTY);//可出貨
                foreach (var SerialsLlistitem in SerialsLlist.OrderByDescending(x => x.ID))
                {

                    var itemSKU = SerialsLlistitem.TransferSKU.SkuNo;
                    var Date = SerialsLlistitem.CreateAt.ToLocalTime();
                    var Supplier = "";
                    var Channel = "";
                    var ISType = "";
                    int? ID = null;
                    var Warehouse = "";
                    var Serial = "";
                    int? QTY;
                    int? BalanceAggregate;
                    int? BalanceAvailable;
                    decimal? ValueAvailable;
                    var OrderID = SerialsLlistitem.OrderID;
                    if (OrderID.HasValue)
                    {

                    }
                    else
                    {
                        OrderID = SerialsLlistitem.SerialsLlistC.Where(x => x.OrderID.HasValue).FirstOrDefault()?.OrderID;
                    }
                    var price = SerialsLlistitem.PurchaseSKU?.Price;
                    if (SerialsLlistitem.SerialsType == "PO")
                    {
                        Supplier = SerialsLlistitem.PurchaseSKU.PurchaseOrder.VendorLIst.Name;
                        ISType = "PO";
                        ID = SerialsLlistitem.PurchaseSKU.PurchaseOrder.ID;
                        Warehouse = SerialsLlistitem.PurchaseSKU.PurchaseOrder.WarehousePO.Name;
                    }
                    else if (SerialsLlistitem.SerialsType == "CM")
                    {
                        Supplier = SerialsLlistitem.PurchaseSKU.CreditMemo.PurchaseOrder.VendorLIst.Name;
                        ISType = "Credit Memo";
                        ID = SerialsLlistitem.PurchaseSKU.CreditMemo?.ID;
                        Warehouse = SerialsLlistitem.PurchaseSKU.CreditMemo.PurchaseOrder.WarehousePO.Name;
                    }
                    else if (SerialsLlistitem.SerialsType == "Order")
                    {
                        Supplier = SerialsLlistitem.PurchaseSKU.PurchaseOrder.VendorLIst.Name;
                        ISType = "Order Dispatch";
                        ID = SerialsLlistitem.OrderID;
                        Warehouse = GetOrderWarehouseName(SerialsLlistitem);
                    }
                    else if (SerialsLlistitem.SerialsType == "TransferOut")
                    {
                        Supplier = SerialsLlistitem.PurchaseSKU.PurchaseOrder.VendorLIst.Name;
                        if (SerialsLlistitem.TransferSKU.Transfer.Status == "Requested")
                        {
                            ISType = "Transfer(Wait)";
                        }
                        else
                        {
                            ISType = "Transfer(Out)";
                        }
                        ID = (SerialsLlistitem.TransferSKU.TransferID);
                        Warehouse = SerialsLlistitem.TransferSKU.Transfer.WarehouseFrom.Name;
                    }
                    else if (SerialsLlistitem.SerialsType == "TransferIn")
                    {
                        Supplier = SerialsLlistitem.PurchaseSKU?.PurchaseOrder.VendorLIst.Name;
                        ISType = "Transfer(In)";
                        ID = SerialsLlistitem.TransferSKU.TransferID;
                        Warehouse = SerialsLlistitem.TransferSKU.Transfer.WarehouseTo.Name;
                    }
                    else if (SerialsLlistitem.SerialsType == "RMA")
                    {

                    }
                    else if (SerialsLlistitem.SerialsType == "Change")
                    {

                    }
                    Serial = SerialsLlistitem.SerialsNo;
                    QTY = SerialsLlistitem.SerialsQTY;
                    BalanceAggregate = Aggregate - AwaitingConut;
                    BalanceAvailable = Available;
                    ValueAvailable = Available * price;
                    if (SerialsLlistitem.SerialsType == "TransferOut" && SerialsLlistitem.TransferSKU.Transfer.Status == "Requested")
                    {
                        Aggregate -= SerialsLlistitem.SerialsQTY.Value;
                    }
                    else
                    {
                        Aggregate -= SerialsLlistitem.SerialsQTY.Value;
                        Available -= SerialsLlistitem.SerialsQTY.Value;
                    }
                    StatementVM.Add(new StatementVM
                    {
                        SKU = itemSKU,
                        OrderID = OrderID,
                        Date = Date,
                        Supplier = Supplier,
                        ID = ID,
                        Channel = Channel,
                        ISType = ISType,
                        Warehouse = Warehouse,
                        Serial = Serial,
                        QTY = QTY,
                        price = price,
                        BalanceAggregate = BalanceAggregate,
                        BalanceAvailable = BalanceAvailable,
                        ValueAvailable = ValueAvailable
                    });
                }
            }
            var ChannelList = EnumData.ChannelList();
            foreach (var item in NoNewRmaSKU)
            {
                var RMASerialsLlist = item.RMASerialsLlist.Where(y => !(y.SerialsType == "TransferOut" || y.SerialsType == "TransferIn") || ((y.SerialsType == "TransferOut" || y.SerialsType == "TransferIn") && y.TransferSKU.IsEnable && y.TransferSKU.Transfer.IsEnable)).ToList();
                var Available = RMASerialsLlist.Where(x => !(x.SerialsType == "TransferOut" && x.TransferSKU.Transfer.Status == "Requested")).Sum(y => y.SerialsQTY);//實際
                var Aggregate = 0;//可出貨
                foreach (var SerialsLlistitem in RMASerialsLlist.OrderByDescending(x => x.ID))
                {
                    var itemSKU = SerialsLlistitem.RMASKU.SkuNo;
                    var Date = SerialsLlistitem.CreateAt.ToLocalTime();
                    var Supplier = "";
                    var Channel = ChannelList[SerialsLlistitem.RMASKU.RMA.Channel.ToString()];
                    var ISType = "";
                    int? ID = null;
                    var Warehouse = "";
                    var Serial = "";
                    int? QTY;
                    int? BalanceAggregate;
                    int? BalanceAvailable;
                    decimal? ValueAvailable;
                    var OrderID = SerialsLlistitem.RMASKU.RMA.OrderID;
                    var price = SerialsLlistitem.RMASKU.UnitPrice;
                    if (SerialsLlistitem.SerialsType == "RMAIn")
                    {
                        // Supplier = SerialsLlistitem.RMASKU.RMA.VendorLIst.Name;
                        ISType = "RMAIn";
                        ID = SerialsLlistitem.RMASKU.RMAID;
                        Warehouse = SerialsLlistitem.Warehouse.Name;
                    }
                    else if (SerialsLlistitem.SerialsType == "TransferOut")
                    {
                        ISType = "Transfer(Out)";
                        ID = SerialsLlistitem.TransferSKU.TransferID;
                        Warehouse = SerialsLlistitem.TransferSKU.Transfer.WarehouseFrom.Name;
                    }
                    Serial = SerialsLlistitem.SerialsNo;
                    QTY = SerialsLlistitem.SerialsQTY;
                    BalanceAggregate = Aggregate;
                    BalanceAvailable = Available;
                    ValueAvailable = Available * price;

                    StatementVM.Add(new StatementVM
                    {
                        SKU = itemSKU,
                        OrderID = OrderID,
                        Date = Date,
                        Supplier = Supplier,
                        ID = ID,
                        Channel = Channel,
                        ISType = ISType,
                        Warehouse = Warehouse,
                        Serial = Serial,
                        QTY = QTY,
                        price = price,
                        BalanceAggregate = BalanceAggregate,
                        BalanceAvailable = BalanceAvailable,
                        ValueAvailable = ValueAvailable
                    });
                }
            }
            foreach (var item in NewRmaSKU)
            {
                var RMASerialsLlist = item.RMASerialsLlist.Where(y => y.NewSkuNo == SKU && !(y.SerialsType == "TransferOut" || y.SerialsType == "TransferIn") || ((y.SerialsType == "TransferOut" || y.SerialsType == "TransferIn") && y.TransferSKU.IsEnable && y.TransferSKU.Transfer.IsEnable)).ToList();
                var Available = RMASerialsLlist.Where(x => !(x.SerialsType == "TransferOut" && x.TransferSKU.Transfer.Status == "Requested")).Sum(y => y.SerialsQTY);//實際
                var Aggregate = 0;//可出貨
                foreach (var SerialsLlistitem in RMASerialsLlist.OrderByDescending(x => x.ID))
                {
                    var itemSKU = SerialsLlistitem.NewSkuNo;
                    var Date = SerialsLlistitem.CreateAt.ToLocalTime();
                    var Supplier = "";
                    var Channel = ChannelList[SerialsLlistitem.RMASKU.RMA.Channel.ToString()];
                    var ISType = "";
                    int? ID = null;
                    var Warehouse = "";
                    var Serial = "";
                    int? QTY;
                    int? BalanceAggregate;
                    int? BalanceAvailable;
                    decimal? ValueAvailable;
                    var OrderID = SerialsLlistitem.RMASKU.RMA.OrderID;
                    var price = SerialsLlistitem.RMASKU.UnitPrice;
                    if (SerialsLlistitem.SerialsType == "RMAIn")
                    {
                        // Supplier = SerialsLlistitem.RMASKU.RMA.VendorLIst.Name;
                        ISType = "RMAIn";
                        ID = SerialsLlistitem.RMASKU.RMAID;
                        Warehouse = SerialsLlistitem.Warehouse.Name;
                    }
                    Serial = SerialsLlistitem.SerialsNo;
                    QTY = SerialsLlistitem.SerialsQTY;
                    BalanceAggregate = Aggregate;
                    BalanceAvailable = Available;
                    ValueAvailable = Available * price;

                    StatementVM.Add(new StatementVM
                    {
                        SKU = itemSKU,
                        OrderID = OrderID,
                        Date = Date,
                        Supplier = Supplier,
                        ID = ID,
                        Channel = Channel,
                        ISType = ISType,
                        Warehouse = Warehouse,
                        Serial = Serial,
                        QTY = QTY,
                        price = price,
                        BalanceAggregate = BalanceAggregate,
                        BalanceAvailable = BalanceAvailable,
                        ValueAvailable = ValueAvailable
                    });
                }
            }
            var StatementVMPO = StatementVM.Where(x => x.ISType == "PO").ToList();//2019/4/9 SKYPE 金額直接使用 PO進價
            foreach (var POitem in StatementVMPO)
            {
                var price = POitem.price;
                var Supplier = POitem.Supplier;
                foreach (var VMitem in StatementVM.Where(x => x.SKU == POitem.SKU&&x.OrderID== POitem.OrderID))
                {
                    VMitem.price = price;//2019/4/9 SKYPE 加入單價 PO進價
                    VMitem.ValueAvailable = VMitem.BalanceAvailable * price;
                    VMitem.Supplier = Supplier;
                }
            }
            return View(StatementVM.OrderByDescending(x => x.Date));
        }

        private string GetOrderWarehouseName(SerialsLlist serialsLlistitem)
        {
            if (serialsLlistitem.SerialsLlistP == null)
            {
                if (serialsLlistitem.TransferSKUID.HasValue)
                {
                    return serialsLlistitem.TransferSKU.Transfer.WarehouseTo.Name;
                }
                else if (serialsLlistitem.PurchaseSKUID.HasValue)
                {
                    return serialsLlistitem.PurchaseSKU.PurchaseOrder.WarehousePO.Name;
                }
                else
                {
                    return "";
                }
            }
            else
            {


                if (serialsLlistitem.SerialsLlistP.TransferSKUID.HasValue)
                {
                    return serialsLlistitem.SerialsLlistP.TransferSKU.Transfer.WarehouseTo.Name;
                }
                else if (serialsLlistitem.SerialsLlistP.PurchaseSKUID.HasValue)
                {
                    return serialsLlistitem.SerialsLlistP.PurchaseSKU.PurchaseOrder.WarehousePO.Name;
                }
                else
                {
                    return "";
                }
            }
        }
        public ActionResult Create()
        {
            return View();
        }
        public ActionResult Serials(string SKU, int WarehouseID)
        {
            ViewBag.WarehouseID = WarehouseID;
            var SKUName = db.SkuLang.Where(x => x.Sku == SKU && x.LangID == LangID).FirstOrDefault().Name;
            var RMASerialsLlist = db.RMASerialsLlist.Where(x => x.RMASKU.IsEnable && x.RMASKU.RMA.IsEnable && (x.RMASKU.SkuNo == SKU || x.NewSkuNo == SKU)).ToList();
            var PurchaseSKU = db.PurchaseSKU.Where(x => x.IsEnable && x.SkuNo == SKU);
            var TransferSKU = db.TransferSKU.Where(x => x.IsEnable && x.SkuNo == SKU);
            var CompanyName = PurchaseSKU.FirstOrDefault()?.PurchaseOrder.Company.Name;
            var InventorySerialsItem = new List<InventorySerialsItem>();
            foreach (var Skuitem in PurchaseSKU)
            {
                if (Skuitem.SerialsLlist != null && Skuitem.SerialsLlist.Any(x => x.SerialsType == "PO"))
                {
                    foreach (var SerialsNoitem in Skuitem.SerialsLlist.Where(x => x.SerialsType == "PO").Select(x => x.SerialsNo).Distinct())
                    {
                        var SerialsLlist = db.SerialsLlist.Where(x => x.SerialsNo == SerialsNoitem).OrderByDescending(x => x.CreateAt);
                        var Order = SerialsLlist.Where(x => x.OrderID.HasValue).FirstOrDefault()?.OrderID;
                        //var FRMA = RMASerialsLlist.Where(x => x.SerialsNo == SerialsNoitem).FirstOrDefault();
                        //var RMA = FRMA?.RMASKU.RMAID;
                        var PO = SerialsLlist.Where(x => x.PurchaseSKU.PurchaseOrderID.HasValue).FirstOrDefault()?.PurchaseSKU.PurchaseOrderID;
                        var CM = SerialsLlist.Where(x => x.PurchaseSKU.CreditMemoID.HasValue).FirstOrDefault()?.PurchaseSKU.CreditMemoID;
                        var Transfer =SerialsLlist.Where(x => x.TransferSKUID.HasValue).FirstOrDefault()?.TransferSKU.TransferID;
                        var WarehouseName = "";
                        var Location = "";
                        var Stock = 0;
                        var lastSerial = SerialsLlist.FirstOrDefault();//最後一筆資料
                        WarehouseName = lastWarehouse(lastSerial);
                        Stock = lastSerial.SerialsQTY ?? 0;

                        var Price = SerialsLlist.Where(x => x.PurchaseSKU.Price.HasValue).FirstOrDefault()?.PurchaseSKU.Price;
                        var CreateAt = lastSerial?.CreateAt;
                        //if (FRMA?.CreateAt > CreateAt)
                        //{
                        //    CreateAt = FRMA?.CreateAt;
                        //}
                        if (Order.HasValue)
                        {
                            Location = Skuitem.PurchaseOrder.WarehousePO.Name;
                        }
                        InventorySerialsItem.Add(new InventorySerialsItem
                        {
                            CM = CM,
                            PO = PO,
                            DispatchLocation = Location,
                            Order = Order,
                            Transfer = Transfer,
                            Stock = Stock,
                            //RMA = RMA,
                            Serial = SerialsNoitem,
                            Value = Price,
                            Warehouse = WarehouseName,
                            Date = CreateAt.Value.ToLocalTime(),
                            IStype = lastSerial.SerialsType
                        });
                    }
                }
            }
            foreach (var TransferSKUitem in TransferSKU)
            {
                foreach (var item in TransferSKUitem.SerialsLlist)
                {
                    var QInventorySerials = InventorySerialsItem.Where(x => x.Serial == item.SerialsNo);
                    if (QInventorySerials.Any())
                    {
                        foreach (var Serialitem in QInventorySerials)
                        {
                            Serialitem.Transfer = TransferSKUitem.TransferID;
                            if (item.CreateAt.ToLocalTime() > Serialitem.Date)
                            {
                                Serialitem.Date = item.CreateAt.ToLocalTime();
                                Serialitem.DispatchLocation = TransferSKUitem.Transfer.WarehouseTo.Name;
                                Serialitem.Warehouse = TransferSKUitem.Transfer.WarehouseFrom.Name;
                                Serialitem.IStype = item.SerialsType;
                            }
                        }
                    }
                    else
                    {
                        InventorySerialsItem.Add(new InventorySerialsItem
                        {

                            DispatchLocation = TransferSKUitem.Transfer.WarehouseTo.Name,
                            Order = item.OrderID,
                            //RMA = item.RMASKU.RMAID,
                            Serial = item.SerialsNo,
                            //Value = item.RMASKU.UnitPrice,
                            Warehouse = TransferSKUitem.Transfer.WarehouseFrom.Name,
                            Date = item.CreateAt.ToLocalTime(),
                            IStype = item.SerialsType
                        });
                    }
                }
            }
               
            foreach (var item in RMASerialsLlist)
            {
             var QInventorySerials=  InventorySerialsItem.Where(x => x.Serial == item.SerialsNo);
                if (QInventorySerials.Any())
                {
                    foreach (var Serialitem in QInventorySerials)
                    {
                        Serialitem.RMA = item.RMASKU.RMAID;
                        if (item.CreateAt.ToLocalTime() > Serialitem.Date)
                        {
                            Serialitem.Date = item.CreateAt.ToLocalTime();
                            Serialitem.Stock = item.SerialsQTY;
                            Serialitem.Warehouse = item.Warehouse.Name;
                            Serialitem.IStype = item.SerialsType;
                            Serialitem.Transfer = item.TransferSKU?.TransferID;
                        }
                    }
                }
                else
                {
                    InventorySerialsItem.Add(new InventorySerialsItem
                    {
                        DispatchLocation = item.Warehouse.Name,
                        Order = item.RMASKU.RMA.OrderID,
                        RMA = item.RMASKU.RMAID,
                        Serial = item.SerialsNo,
                        Value = item.RMASKU.UnitPrice,
                        Warehouse = item.Warehouse.Name,
                        Stock = item.SerialsQTY,
                        Date = item.CreateAt.ToLocalTime(),
                        IStype = item.SerialsType,
                        Transfer = item.TransferSKU?.TransferID,
                    });
                }
            }
            var InventorySerials = new InventorySerials
            {
                InventorySerialsItem = InventorySerialsItem.OrderByDescending(x=>x.Date).ToList(),
                SKU = SKU,
                SKUName = SKUName,
                CompanyName= CompanyName
            };
            return View(InventorySerials);
        }
        private string lastWarehouse(SerialsLlist lastSerial)
        {
            var WarehouseName = "";
            if (lastSerial.SerialsType == "PO")
            {
                WarehouseName = lastSerial.PurchaseSKU.PurchaseOrder.WarehousePO.Name;
            }
            else if (lastSerial.SerialsType == "TransferIn")
            {
                WarehouseName = lastSerial.TransferSKU.Transfer.WarehouseTo.Name;
            }
            else if (lastSerial.SerialsType == "TransferOut")
            {
                WarehouseName = lastSerial.TransferSKU.Transfer.WarehouseFrom.Name;
            }
            else if (lastSerial.SerialsType == "CM" || lastSerial.SerialsType == "Order")
            {
                WarehouseName = lastWarehouse(lastSerial.SerialsLlistP);
            }
            return WarehouseName;
        }

        public ActionResult Inventory(string SKU, int WarehouseID)
        {
            var NoList = new List<int>();
            ViewBag.WarehouseID = WarehouseID;
            var SkuInventoryVM = new List<SkuInventoryVM>();

            var Warehouselist = db.Warehouse.Where(x => x.IsEnable && x.Type != "Interim");
            foreach (var Warehouse in Warehouselist)
            {
                var NSkuInventoryVM = new SkuInventoryVM
                {
                    ID = Warehouse.ID,
                    Warehouse = Warehouse.Name,
                    Type = Warehouse.Type,
                };
                var WarehouseVM = GetWarehouseVMList(Warehouse, SKU, null, null);
                foreach (var item in WarehouseVM)
                {
                    NSkuInventoryVM.BackOrdered += item.BackOrdered;
                    NSkuInventoryVM.Awaiting += item.Awaiting; ;
                    NSkuInventoryVM.POQTY += item.POQTY;
                    NSkuInventoryVM.CMQTY += item.CMQTY;
                    NSkuInventoryVM.OrderQTY += item.OrderQTY;
                    NSkuInventoryVM.TransferInQTY += item.TransferInQTY;
                    NSkuInventoryVM.TransferOutQTY += item.TransferOutQTY;
                    NSkuInventoryVM.TransferAwaiting += item.TransferAwaiting;
                    NSkuInventoryVM.Available += item.Fulfillable;
                    NSkuInventoryVM.Aggregate += NSkuInventoryVM.Available - NSkuInventoryVM.Awaiting;
                }
                SkuInventoryVM.Add(NSkuInventoryVM);
            }


            return View(SkuInventoryVM);
        }
        public ActionResult InventoryOld(string SKU, int WarehouseID)
        {
            var NoList = new List<int>();
            ViewBag.WarehouseID = WarehouseID;
            //var Warehouse = db.Warehouse.Find(WarehouseID);
            var SkuInventoryVM = new List<SkuInventoryVM>();
            var PurchaseSKU = db.PurchaseSKU.Where(x => x.IsEnable && x.SkuNo == SKU && x.PurchaseOrderID.HasValue && x.PurchaseOrder.WarehouseID.HasValue).ToList();
            var TransferSKUTo = db.TransferSKU.Where(x => x.IsEnable && x.SkuNo == SKU && x.TransferID.HasValue && x.Transfer.ToWID.HasValue).ToList();
            var TransferSKUFrom = db.TransferSKU.Where(x => x.IsEnable && x.SkuNo == SKU && x.TransferID.HasValue && x.Transfer.FromWID.HasValue).ToList();
            //取待出貨
            var AwaitingCountlist = GetAwaitingCount(SKU, "");
            foreach (var item in PurchaseSKU)
            {
                SkuInventoryVM.Add(new SkuInventoryVM
                {
                    ID = item.PurchaseOrder.WarehousePO.ID,
                    Warehouse = item.PurchaseOrder.WarehousePO.Name,
                    Type = item.PurchaseOrder.WarehousePO.Type,
                    SCID = GetSCID(item),
                    POQTY = GetPOQty(item),
                    CMQTY = GetCMQty(item),
                    OrderQTY = GetOrderQTY(item),
                    TransferInQTY = GetTransferInQTY(item, item.PurchaseOrder.WarehousePO, "", ref NoList),
                    TransferOutQTY = GetTransferOutQTY(item, item.PurchaseOrder.WarehousePO, "", ref NoList),
                    TransferAwaiting = GetTransferAwaiting(item, item.PurchaseOrder.WarehousePO, ref NoList),//等待移倉的數量
                    TransferOutCloseQTY = GetTransferOutCloseQTY(item, item.PurchaseOrder.WarehousePO, ref NoList),//移倉已入庫
                    BackOrdered = item.QTYOrdered ?? 0,
                    Total = PurchaseSKU.Sum(x => x.SerialsLlist.Sum(t => t.SerialsQTY)) ?? 0,
                    Unfulfillable = 0,
                    Available = 0,
                    Aggregate = 0,//可上架的庫存總數
                                  // Awaiting = GetAwaitingCount(item.SkuNo, GetSCID(item)),//等待出貨的庫總量
                                  //TransferInQTY = GetTransferInQTY(item),
                    Value = GetValue(item)
                });
            }
            foreach (var item in TransferSKUTo)
            {
                SkuInventoryVM.Add(new SkuInventoryVM
                {
                    ID = item.Transfer.WarehouseTo.ID,
                    Warehouse = item.Transfer.WarehouseTo.Name,
                    Type = item.Transfer.WarehouseTo.Type,
                    TransferInQTY = GetTransferInQTY(item, item.Transfer.WarehouseTo, "To", ref NoList),//接入庫當PO
                    TransferOutQTY = GetTransferOutQTY(item, item.Transfer.WarehouseTo, "To", ref NoList),
                    TransferOutCloseQTY = GetTransferOutCloseQTY(item, item.Transfer.WarehouseTo, ref NoList),//移倉已入庫
                    TransferAwaiting = GetTransferAwaiting(item, item.Transfer.WarehouseTo, ref NoList),//等待移倉的數量
                });
                //NoList.AddRange(item.SerialsLlist.Select(x => x.ID));
            }
            foreach (var item in TransferSKUFrom)
            {
                SkuInventoryVM.Add(new SkuInventoryVM
                {
                    ID = item.Transfer.WarehouseFrom.ID,
                    Warehouse = item.Transfer.WarehouseFrom.Name,
                    Type = item.Transfer.WarehouseFrom.Type,
                    TransferInQTY = GetTransferInQTY(item, item.Transfer.WarehouseFrom, "From", ref NoList),//接入庫當PO
                    //TransferOutQTY = GetTransferOutQTY(item, item.Transfer.WarehouseFrom, NoList),
                    TransferOutCloseQTY = GetTransferOutCloseQTY(item, item.Transfer.WarehouseFrom, ref NoList),//移倉已入庫
                    //TransferAwaiting = GetTransferAwaiting(item, item.Transfer.WarehouseFrom, NoList),//等待移倉的數量
                });
            }
            var GroupSkuInventoryVM = SkuInventoryVM.GroupBy(x => new { x.ID }).Select(x => new SkuInventoryVM //SKU數量總計
            {
                ID = x.FirstOrDefault().ID,
                Warehouse = x.FirstOrDefault().Warehouse,
                Type = x.FirstOrDefault().Type,
                SCID = x.FirstOrDefault().SCID,
                BackOrdered = x.Sum(p => p.BackOrdered),
                POQTY = x.Sum(p => p.POQTY),
                CMQTY = x.Sum(p => p.CMQTY),
                OrderQTY = x.Sum(p => p.OrderQTY),
                TransferInQTY = x.Sum(p => p.TransferInQTY),
                TransferOutQTY = x.Sum(p => p.TransferOutQTY),
                RMAQTY = x.Sum(p => p.RMAQTY),
                Unfulfillable = x.Sum(p => p.TransferOutCloseQTY),
                TransferAwaiting = x.Sum(p => p.TransferAwaiting),

            }).ToList();

            foreach (var item in GroupSkuInventoryVM)
            {
                item.Unfulfillable = Math.Abs(item.Unfulfillable);
                item.Available = item.POQTY - item.TransferAwaiting;
                item.Awaiting = (AwaitingCountlist.Where(x => x.SKU == SKU && x.SCID == item.SCID).FirstOrDefault()?.QTY ?? 0) - item.TransferAwaiting;
                item.Aggregate = item.Available - item.Awaiting;
            }
            return View(GroupSkuInventoryVM);
        }

        /// <summary>
        /// 該序號/單品的價值
        /// </summary>
        /// <param name="PurchaseSKU"></param>
        /// <returns></returns>
        private Decimal GetValue(PurchaseSKU PurchaseSKU)
        {
            return (PurchaseSKU.Price - PurchaseSKU.Discount) ?? 0;
        }

        /// <summary>
        /// 取SCID
        /// </summary>
        /// <param name="PurchaseSKU"></param>
        /// <returns></returns>
        private string GetSCID(PurchaseSKU PurchaseSKU)
        {
            return PurchaseSKU.PurchaseOrder.WarehousePO?.WarehouseSummary.FirstOrDefault(x => x.Type == "SCID")?.Val;
        }

        public ActionResult Purchasing(string SKU, int? Inventory, int? Velocity, int WarehouseID)
        {
            ViewBag.WarehouseID = WarehouseID;
            var PurchaseSKU = db.PurchaseSKU.Where(x => x.IsEnable && x.SkuNo == SKU);
            var SkuPurchasingVM = new SkuPurchasingVM
            {
                SKU = SKU,
                Inventory = Inventory,
                Company = PurchaseSKU.FirstOrDefault()?.PurchaseOrder.Company.Name,
                SKUName = PurchaseSKU.FirstOrDefault()?.Name,
            };
            if (Inventory.HasValue)
            {
                var Warehouse = db.Warehouse.Find(Inventory);
                var WarehouseVM = GetWarehouseVMList(Warehouse, SKU, null, null);
                foreach (var item in WarehouseVM)
                {
                    SkuPurchasingVM.BackOrdered += item.BackOrdered;
                    SkuPurchasingVM.Awaiting += item.Awaiting; ;
                    SkuPurchasingVM.POQTY += item.POQTY;
                    SkuPurchasingVM.CMQTY += item.CMQTY;
                    SkuPurchasingVM.OrderQTY += item.OrderQTY;
                    SkuPurchasingVM.TransferInQTY += item.TransferInQTY;
                    SkuPurchasingVM.TransferOutQTY += item.TransferOutQTY;
                    SkuPurchasingVM.Velocity += item.Velocity;
                    SkuPurchasingVM.UnfulfillableRMA += item.UnfulfillableRMA;
                    SkuPurchasingVM.Fulfillable += item.Fulfillable;
                }

            }
            else
            {
                var Warehouselist = db.Warehouse.Where(x => x.IsEnable && x.Type != "Interim");
                foreach (var Warehouse in Warehouselist)
                {
                    var WarehouseVM = GetWarehouseVMList(Warehouse, SKU, null, null);
                    foreach (var item in WarehouseVM)
                    {
                        SkuPurchasingVM.BackOrdered += item.BackOrdered;
                        SkuPurchasingVM.Awaiting += item.Awaiting; ;
                        SkuPurchasingVM.POQTY += item.POQTY;
                        SkuPurchasingVM.CMQTY += item.CMQTY;
                        SkuPurchasingVM.OrderQTY += item.OrderQTY;
                        SkuPurchasingVM.TransferInQTY += item.TransferInQTY;
                        SkuPurchasingVM.TransferOutQTY += item.TransferOutQTY;
                        SkuPurchasingVM.Velocity += item.Velocity;
                        SkuPurchasingVM.UnfulfillableRMA += item.UnfulfillableRMA;
                        SkuPurchasingVM.Fulfillable += item.Fulfillable;
                    }
                }
            }
            //SkuPurchasingVM.Fulfillable = SkuPurchasingVM.POQTY + SkuPurchasingVM.CMQTY + SkuPurchasingVM.OrderQTY + SkuPurchasingVM.TransferInQTY + SkuPurchasingVM.TransferOutQTY;
            SkuPurchasingVM.Aggregate = SkuPurchasingVM.Fulfillable - SkuPurchasingVM.Awaiting;//Aggregate = Fulfillable - Awaiting dispatch
            SkuPurchasingVM.UnfulfillableTransit = SkuPurchasingVM.TransferInQTY + SkuPurchasingVM.TransferOutQTY;
            SkuPurchasingVM.TotalInventory = SkuPurchasingVM.POQTY + SkuPurchasingVM.CMQTY + SkuPurchasingVM.UnfulfillableRMA + SkuPurchasingVM.TransferInQTY + SkuPurchasingVM.TransferOutQTY;
            SkuPurchasingVM.QTYperbox = 1;
            SkuPurchasingVM.QTYpercase = 1;
            return View(SkuPurchasingVM);
        }



        public ActionResult PurchasingOld(string SKU, int? Inventory, int? Velocity, int WarehouseID)
        {
            ViewBag.WarehouseID = WarehouseID;
            //var Warehouse = db.Warehouse.Find(WarehouseID);
            //var PurchaseSKU = db.PurchaseSKU.Where(x => x.IsEnable && x.SkuNo == SKU && x.SerialsLlist.Any(y => y.SerialsType == "PO")).ToList();
            //var TransferSKU = db.TransferSKU.Where(x => x.IsEnable && x.SkuNo == SKU && x.SerialsLlist.Any(y => y.SerialsType == "TransferIn")).ToList();
            var PurchaseSKU = db.PurchaseSKU.Where(x => x.IsEnable && x.PurchaseOrder.IsEnable && x.SkuNo == SKU).ToList();//PO單
            var TransferToSKU = db.TransferSKU.Where(x => x.IsEnable && x.Transfer.IsEnable && x.SkuNo == SKU).ToList();//移倉單入庫
            var TransferFromSKU = db.TransferSKU.Where(x => x.IsEnable && x.Transfer.IsEnable && x.SkuNo == SKU).ToList();//移倉單出庫
            var SkuPurchasingVM = new SkuPurchasingVM
            {
                SKU = SKU,
            };
            if (Inventory.HasValue)
            {
                PurchaseSKU = PurchaseSKU.Where(x => x.IsEnable && x.PurchaseOrder.IsEnable && x.PurchaseOrder.WarehousePO.ID == Inventory).ToList();//PO單
                TransferToSKU = TransferToSKU.Where(x => x.IsEnable && x.Transfer.IsEnable && x.Transfer.ToWID == Inventory).ToList();//移倉單入庫
                TransferFromSKU = TransferFromSKU.Where(x => x.IsEnable && x.Transfer.IsEnable && x.Transfer.FromWID == Inventory).ToList();//移倉單出庫
                //PurchaseSKU = PurchaseSKU.Where(x => x.PurchaseOrder.WarehouseID == Inventory).ToList();
                //TransferSKU = TransferSKU.Where(x => x.Transfer.ToWID == Inventory).ToList();
            }
            if (PurchaseSKU.Any() || TransferToSKU.Any() || TransferFromSKU.Any())
            {
                var NoList = new List<int>();
                //取待出貨
                var AwaitingCountlist = GetAwaitingCount(SKU, "");

                SkuPurchasingVM.Inventory = Inventory;
                SkuPurchasingVM.Company = PurchaseSKU.FirstOrDefault()?.PurchaseOrder.Company.Name;
                SkuPurchasingVM.SKUName = PurchaseSKU.FirstOrDefault()?.Name;
                SkuPurchasingVM.Aggregate = 0;
                SkuPurchasingVM.Fulfillable = 0;
                SkuPurchasingVM.AwaitingDispatch = 0;
                SkuPurchasingVM.UnfulfillableRMA = 0;
                SkuPurchasingVM.UnfulfillableTransit = 0;
                SkuPurchasingVM.TotalInventory = 0;
                foreach (var PurchaseSKUitem in PurchaseSKU)
                {
                    if (PurchaseSKUitem.SerialsLlist.Any())
                    {
                        var SCID = PurchaseSKUitem.PurchaseOrder.WarehousePO?.WarehouseSummary.Where(x => x.Type == "SCID").FirstOrDefault().Val;
                        SkuPurchasingVM.BackOrdered += PurchaseSKUitem.QTYOrdered ?? 0;
                        SkuPurchasingVM.Awaiting += (AwaitingCountlist.Where(x => x.SKU == SKU && x.SCID == SCID).FirstOrDefault()?.QTY ?? 0) - GetTransferAwaiting(PurchaseSKUitem, PurchaseSKUitem.PurchaseOrder.WarehousePO, ref NoList);
                        SkuPurchasingVM.POQTY += PurchaseSKUitem.QTYOrdered.HasValue ? PurchaseSKUitem.QTYOrdered.Value : PurchaseSKUitem.SerialsLlist.Where(y => y.SerialsType == "PO").Sum(y => y.SerialsQTY) ?? 0;//有輸入直接讀輸入，沒輸入計算序號數
                        SkuPurchasingVM.CMQTY += GetCMQty(PurchaseSKUitem, NoList);
                        SkuPurchasingVM.OrderQTY += GetOrderQTY(PurchaseSKUitem, NoList);
                        //SkuPurchasingVM.TransferInQTY += GetTransferInQTY(PurchaseSKUitem, NoList);
                        SkuPurchasingVM.TransferOutQTY += GetTransferOutQTY(PurchaseSKUitem, PurchaseSKUitem.PurchaseOrder.WarehousePO, "", ref NoList);
                        SkuPurchasingVM.Velocity += GetVelocity(PurchaseSKUitem);
                        SkuPurchasingVM.UnfulfillableRMA += PurchaseSKUitem.SerialsLlist.Where(y => y.SerialsType == "RMA").Sum(y => y.SerialsQTY).Value;
                    }
                }
                foreach (var TransferSKUitem in TransferToSKU.Where(x => x.IsEnable))
                {
                    if (TransferSKUitem.SerialsLlist.Any())
                    {
                        SkuPurchasingVM.TransferInQTY += GetTransferInQTY(TransferSKUitem, TransferSKUitem.Transfer.WarehouseTo, "To", ref NoList);
                        SkuPurchasingVM.Awaiting += (-GetTransferAwaiting(TransferSKUitem, TransferSKUitem.Transfer.WarehouseTo, ref NoList));
                        SkuPurchasingVM.TransferOutQTY += GetTransferOutQTY(TransferSKUitem, TransferSKUitem.Transfer.WarehouseTo, "To", ref NoList);
                    }
                }
                foreach (var TransferSKUitem in TransferFromSKU.Where(x => x.IsEnable))
                {
                    if (TransferSKUitem.SerialsLlist.Any())
                    {
                        SkuPurchasingVM.TransferInQTY += GetTransferInQTY(TransferSKUitem, TransferSKUitem.Transfer.WarehouseTo, "From", ref NoList);
                        SkuPurchasingVM.Awaiting += (-GetTransferAwaiting(TransferSKUitem, TransferSKUitem.Transfer.WarehouseTo, ref NoList));
                        SkuPurchasingVM.TransferOutQTY += GetTransferOutQTY(TransferSKUitem, TransferSKUitem.Transfer.WarehouseTo, "From", ref NoList);
                    }
                }
                SkuPurchasingVM.Fulfillable = SkuPurchasingVM.POQTY + SkuPurchasingVM.CMQTY + SkuPurchasingVM.OrderQTY + SkuPurchasingVM.TransferInQTY + SkuPurchasingVM.TransferOutQTY;
                SkuPurchasingVM.Aggregate = SkuPurchasingVM.Fulfillable - SkuPurchasingVM.Awaiting;//Aggregate = Fulfillable - Awaiting dispatch
                SkuPurchasingVM.UnfulfillableTransit = SkuPurchasingVM.TransferInQTY + SkuPurchasingVM.TransferOutQTY;
                SkuPurchasingVM.TotalInventory = SkuPurchasingVM.POQTY + SkuPurchasingVM.CMQTY + SkuPurchasingVM.OrderQTY + SkuPurchasingVM.TransferInQTY + SkuPurchasingVM.TransferOutQTY;
                SkuPurchasingVM.QTYperbox = 1;
                SkuPurchasingVM.QTYpercase = 1;
                if (PurchaseSKU.Any())
                {
                    SkuPurchasingVM.Latest = PurchaseSKU.OrderByDescending(x => x.CreateAt).FirstOrDefault().Price.Value;
                    SkuPurchasingVM.Average = PurchaseSKU.Average(x => x.Price).Value;
                    SkuPurchasingVM.Lowest = PurchaseSKU.OrderBy(x => x.Price).FirstOrDefault().Price.Value;
                    SkuPurchasingVM.Highest = PurchaseSKU.OrderByDescending(x => x.Price).FirstOrDefault().Price.Value;
                }

                if (Velocity.HasValue)
                {
                    SkuPurchasingVM.Velocity = Velocity.Value;//公式？
                    SkuPurchasingVM.AveragefulfilledQTY = 0;//公式？
                    SkuPurchasingVM.AveragePOQTY = 0;//公式？
                    SkuPurchasingVM.TotalFulfilled = 0;//公式？
                    SkuPurchasingVM.TotalPO = 0;//公式？
                }

            }
            return View(SkuPurchasingVM);
        }
        /// <summary>
        /// 序號清單
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ActionResult GetHistory(string ID)
        {
            var SerialsLlistVMList = new List<SerialsLlistVM>();
            var SerialsLlist = db.SerialsLlist.Where(x => x.IsEnable && x.SerialsNo == ID).OrderByDescending(x => x.CreateAt).ThenByDescending(x => x.ID);
            var RMASerialsLlist = db.RMASerialsLlist.Where(x => x.IsEnable && x.SerialsNo == ID).OrderByDescending(x => x.CreateAt).ThenByDescending(x => x.ID);
            foreach (var item in SerialsLlist)
            {

                var CreateAt = item.CreateAt;
                var QTY = item.SerialsQTY;
                var UpdatedBy = item.CreateBy;
                string ISType = "";
                int? SID = 0;
                string Warehouse = "";
                if (item.SerialsType == "TransferIn")
                {
                    ISType = "Transfer(In)";
                    SID = item.TransferSKU.TransferID;
                    Warehouse = item.TransferSKU.Transfer.WarehouseTo.Name;
                }
                else if (item.SerialsType == "TransferOut")
                {
                    ISType = "Transfer(Out)";
                    SID = item.TransferSKU.TransferID;
                    Warehouse = item.TransferSKU.Transfer.WarehouseFrom.Name;
                }
                else if (item.SerialsType == "PO")
                {
                    ISType = item.PurchaseSKU.PurchaseOrder.POType;
                    SID = item.PurchaseSKU.PurchaseOrderID;
                    Warehouse = item.PurchaseSKU.PurchaseOrder.WarehousePO.Name;
                }
                else if (item.SerialsType == "CM")
                {
                    ISType = item.PurchaseSKU.CreditMemo.CMType;
                    SID = item.PurchaseSKU.CreditMemoID;
                    if (item.SerialsLlistP.TransferSKUID.HasValue)
                    {
                        Warehouse = item.SerialsLlistP.TransferSKU.Transfer.WarehouseTo.Name;
                    }
                    else
                    {
                        Warehouse = item.SerialsLlistP.PurchaseSKU.PurchaseOrder.WarehousePO.Name;
                    }
                }
                else if (item.SerialsType == "Order")
                {
                    ISType = "Order";
                    SID = item.OrderID;
                    if (item.SerialsLlistP.TransferSKUID.HasValue)
                    {
                        Warehouse = item.TransferSKU.Transfer.WarehouseTo.Name;
                    }
                    else
                    {
                        Warehouse = item.PurchaseSKU.PurchaseOrder.WarehousePO.Name;
                    }
                }
                SerialsLlistVMList.Add(new SerialsLlistVM { Date = CreateAt, ID = SID, ISType = ISType, QTY = QTY, UpdatedBy = UpdatedBy, Warehouse = Warehouse });
            }
            foreach (var item in RMASerialsLlist)
            {

                var CreateAt = item.CreateAt;
                var QTY = item.SerialsQTY;
                var UpdatedBy = item.CreateBy;
                string ISType = "";
                int? SID = 0;
                string Warehouse = "";
                if (item.SerialsType == "RMAIn")
                {
                    ISType = "RMA";
                    SID = item.RMASKU.RMAID;
                    Warehouse = item.Warehouse.Name;
                }
                else if (item.SerialsType == "TransferOut")
                {
                    ISType = "Transfer(Out)";
                    SID = item.TransferSKU.TransferID;
                    Warehouse = item.TransferSKU.Transfer.WarehouseFrom.Name;
                }
                else if (item.SerialsType == "CM")
                {
                    ISType = item.PurchaseSKU.CreditMemo.CMType;
                    SID = item.PurchaseSKU.CreditMemoID;
                    if (item.RMASerialsLlistP.TransferSKUID.HasValue)
                    {
                        Warehouse = item.RMASerialsLlistP.TransferSKU.Transfer.WarehouseTo.Name;
                    }
                    else
                    {
                        Warehouse = item.RMASerialsLlistP.Warehouse?.Name ?? item.RMASerialsLlistP.RMASKU.RMA.Warehouse.Name;
                    }
                }
                SerialsLlistVMList.Add(new SerialsLlistVM { Date = CreateAt, ID = SID, ISType = ISType, QTY = QTY, UpdatedBy = UpdatedBy, Warehouse = Warehouse });
            }
            return View(SerialsLlistVMList.OrderByDescending(x=>x.Date));
        }
    }
}