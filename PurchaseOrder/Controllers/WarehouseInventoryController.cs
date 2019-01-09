using PurchaseOrderSys.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PurchaseOrderSys.Controllers
{
    public class WarehouseInventoryController : BaseController
    {
        // GET: Warehouse
        public ActionResult Index(int ID)
        {
            var SCID = db.WarehouseSummary.Where(x => x.WarehouseID == ID && x.Type == "SCID").FirstOrDefault().Val;
            var edt = DateTime.UtcNow;
            var sdt = edt.AddDays(-30);
            var PurchaseSKU = db.PurchaseSKU.Where(x => x.IsEnable && x.PurchaseOrder.Warehouse1.ID == ID).ToList();
            var WarehouseInventoryVM = new WarehouseInventoryVM();
            if (PurchaseSKU.Any())
            {
                var WarehouseVM = PurchaseSKU.Select(x => new WarehouseVM
                {
                    ID = x.ID,
                    Name = x.Name,
                    SKU = x.SkuNo,
                    POQTY = x.QTYOrdered.HasValue ? x.QTYOrdered.Value : x.SerialsLlist.Where(y => y.SerialsType == "PO").Sum(y => y.SerialsQTY) ?? 0,//有輸入直接讀輸入，沒輸入計算序號數

                    CMQTY = GetCMQty(x),
                    OrderQTY = x.SerialsLlist.Where(z => z.SerialsType == "Order").Sum(z => z.SerialsQTY).Value,
                    TransferInQTY = x.SerialsLlist.Where(y => y.SerialsType == "TransferIn").Sum(y => y.SerialsQTY).Value,
                    TransferOutQTY = x.SerialsLlist.Where(y => y.SerialsType == "TransferOut").Sum(y => y.SerialsQTY).Value,
                    Velocity = x.SerialsLlist.Where(z => z.SerialsType == "Order" && z.CreateAt >= sdt && z.CreateAt <= sdt).Sum(z => z.SerialsQTY).Value,
                    DaysOfSupply = 0,
                    Aggregate = 0,//可上架的庫存總數
                    Awaiting = 0,//等待出貨的庫總量
                    Fulfillable = 0,
                    Unfulfillable = 0
                }).ToList();
                foreach (var item in WarehouseVM)
                {
                    item.Fulfillable = item.POQTY + item.CMQTY + item.OrderQTY + item.TransferInQTY + item.TransferOutQTY;
                    item.Unfulfillable = item.TransferInQTY - item.TransferOutQTY;
                    item.Awaiting = GetAwaitingCount(item.SKU,SCID);
                    item.Aggregate = item.Fulfillable - item.Awaiting;//Aggregate = Fulfillable - Awaiting dispatch
                    item.DaysOfSupply = item.Aggregate != 0 && item.Velocity != 0 ? item.Aggregate / item.Velocity / 30 : 0;  //Days of supply 算法: Aggregate / Velocity (30 days) / 30
                                                                                                                              //item.Fulfillable = item.Awaiting + item.Aggregate; //Fulfillable = Awaiting dispatch + Aggregate 2018/12/28 拿掉公式
                }
                WarehouseInventoryVM.Company = PurchaseSKU.FirstOrDefault().PurchaseOrder.Company.Name;
                WarehouseInventoryVM.WarehouseType = PurchaseSKU.FirstOrDefault().PurchaseOrder.Warehouse1.Type;
                WarehouseInventoryVM.Fulfillable = PurchaseSKU.FirstOrDefault().PurchaseOrder.Warehouse1.Fulfillable;
                WarehouseInventoryVM.Location = PurchaseSKU.FirstOrDefault().PurchaseOrder.Warehouse1.Location;
                WarehouseInventoryVM.Countries = PurchaseSKU.FirstOrDefault().PurchaseOrder.Warehouse1.Countries;
                WarehouseInventoryVM.Marketplace = PurchaseSKU.FirstOrDefault().PurchaseOrder.Warehouse1.Marketplace;
                WarehouseInventoryVM.WarehouseVM = WarehouseVM;
            }
            return View(WarehouseInventoryVM);
        }

        private int GetCMQty(PurchaseSKU PurchaseSKU)
        {
            var count = 0;
            foreach (var Serialsitem in PurchaseSKU.SerialsLlist.Where(x=>x.SerialsLlistC.Any(y => y.SerialsType == "CM")))
            {
                count += Serialsitem.SerialsLlistC.Where(x => x.SerialsType == "CM").Sum(x => x.SerialsQTY).Value;
            }
            return count;
        }

        public ActionResult Statement(string SKU)
        {
            var PurchaseSKU = db.PurchaseSKU.Where(x=>x.SkuNo== SKU);
            return View(PurchaseSKU);
        }

        public ActionResult Create()
        {
            return View();
        }
        public ActionResult Serials(string SKU)
        {
            var PurchaseSKU = db.PurchaseSKU.Where(x => x.SkuNo == SKU);
            return View(PurchaseSKU);
        }
        public ActionResult Inventory(string SKU)
        {
            var SkuInventoryVM = new List<SkuInventoryVM>();
            var PurchaseSKU = db.PurchaseSKU.Where(x => x.SkuNo == SKU && x.PurchaseOrderID.HasValue && x.PurchaseOrder.WarehouseID.HasValue);

            foreach (var item in PurchaseSKU)
            {
                SkuInventoryVM.Add(new SkuInventoryVM
                {
                    ID = item.PurchaseOrder.Warehouse1.ID,
                    Warehouse = item.PurchaseOrder.Warehouse1.Name,
                    Type = item.PurchaseOrder.Warehouse1.Type,
                    SCID = GetSCID(item),
                    POQTY = item.QTYOrdered.HasValue ? item.QTYOrdered.Value : item.SerialsLlist.Where(y => y.SerialsType == "PO").Sum(y => y.SerialsQTY) ?? 0,//有輸入直接讀輸入，沒輸入計算序號數
                    Available = 0,
                    Aggregate = 0,//可上架的庫存總數
                    Awaiting = GetAwaitingCount(item.SkuNo, GetSCID(item)),//等待出貨的庫總量
                    Unfulfillable = 0,
                    Total = 0,
                    BackOrdered = item.QTYOrdered ?? 0,
                    CMQTY = GetCMQty(item),
                    TransferInQTY = item.SerialsLlist.Where(y => y.SerialsType == "TransferIn").Sum(y => y.SerialsQTY).Value,
                    TransferOutQTY = item.SerialsLlist.Where(y => y.SerialsType == "TransferOut").Sum(y => y.SerialsQTY).Value,
                    OrderQTY = item.SerialsLlist.Where(z => z.SerialsType == "Order").Sum(z => z.SerialsQTY).Value,
                });
            }
            foreach (var item in SkuInventoryVM)
            {
                item.Available = item.POQTY + item.CMQTY + item.OrderQTY + item.TransferInQTY + item.TransferOutQTY;
                item.Unfulfillable = item.TransferInQTY - item.TransferOutQTY;
                item.Aggregate = item.Available - item.Awaiting;//Aggregate = Fulfillable - Awaiting dispatch

            }
            return View(SkuInventoryVM);
        }

        private string GetSCID(PurchaseSKU PurchaseSKU)
        {
            return PurchaseSKU.PurchaseOrder.Warehouse1?.WarehouseSummary.FirstOrDefault(x => x.Type == "SCID")?.Val;
        }

        public ActionResult Purchasing(string SKU,int? Inventory, int? Velocity)
        {
            var Warehouselist = db.Warehouse.Where(x => x.IsEnable).Select(x => new SelectListItem { Text = x.Name, Value = x.ID.ToString() }).ToList();
            ViewBag.Warehouselist = Warehouselist;
            var edt = DateTime.UtcNow;
            var sdt = edt.AddDays(-30);
            var PurchaseSKU = db.PurchaseSKU.Where(x => x.SkuNo == SKU && x.SerialsLlist.Any(y => y.SerialsType == "PO"));
            var SkuPurchasingVM = new SkuPurchasingVM
            {
                SKU = SKU,
            };
            if (Inventory.HasValue)
            {
                PurchaseSKU = PurchaseSKU.Where(x => x.PurchaseOrder.WarehouseID == Inventory);
            }
            if (PurchaseSKU.Any())
            {
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
                        var SCID = PurchaseSKUitem.PurchaseOrder.Warehouse1?.WarehouseSummary.Where(x => x.Type == "SCID").FirstOrDefault().Val;
                        SkuPurchasingVM.BackOrdered += PurchaseSKUitem.QTYOrdered ?? 0;
                        SkuPurchasingVM.Awaiting = GetAwaitingCount(SkuPurchasingVM.SKU, SCID);
                        SkuPurchasingVM.POQTY += PurchaseSKUitem.QTYOrdered.HasValue ? PurchaseSKUitem.QTYOrdered.Value : PurchaseSKUitem.SerialsLlist.Where(y => y.SerialsType == "PO").Sum(y => y.SerialsQTY) ?? 0;//有輸入直接讀輸入，沒輸入計算序號數
                        SkuPurchasingVM.CMQTY += GetCMQty(PurchaseSKUitem);
                        SkuPurchasingVM.OrderQTY += PurchaseSKUitem.SerialsLlist.Where(z => z.SerialsType == "Order").Sum(z => z.SerialsQTY).Value;
                        SkuPurchasingVM.TransferInQTY += PurchaseSKUitem.SerialsLlist.Where(y => y.SerialsType == "TransferIn").Sum(y => y.SerialsQTY).Value;
                        SkuPurchasingVM.TransferOutQTY += PurchaseSKUitem.SerialsLlist.Where(y => y.SerialsType == "TransferOut").Sum(y => y.SerialsQTY).Value;
                        SkuPurchasingVM.Velocity += PurchaseSKUitem.SerialsLlist.Where(z => z.SerialsType == "Order" && z.CreateAt >= sdt && z.CreateAt <= sdt).Sum(z => z.SerialsQTY).Value;
                        SkuPurchasingVM.UnfulfillableRMA += PurchaseSKUitem.SerialsLlist.Where(y => y.SerialsType == "RMA").Sum(y => y.SerialsQTY).Value;
                    }
                }
                SkuPurchasingVM.Fulfillable = SkuPurchasingVM.POQTY + SkuPurchasingVM.CMQTY + SkuPurchasingVM.OrderQTY + SkuPurchasingVM.TransferInQTY + SkuPurchasingVM.TransferOutQTY + SkuPurchasingVM.Awaiting;
                SkuPurchasingVM.Aggregate = SkuPurchasingVM.Available - SkuPurchasingVM.Awaiting;//Aggregate = Fulfillable - Awaiting dispatch
                SkuPurchasingVM.UnfulfillableTransit = SkuPurchasingVM.TransferInQTY + SkuPurchasingVM.TransferOutQTY;
                SkuPurchasingVM.TotalInventory = SkuPurchasingVM.POQTY + SkuPurchasingVM.CMQTY + SkuPurchasingVM.OrderQTY + SkuPurchasingVM.TransferInQTY + SkuPurchasingVM.TransferOutQTY;
                SkuPurchasingVM.QTYperbox = 1;
                SkuPurchasingVM.QTYpercase = 1;
                SkuPurchasingVM.Latest = PurchaseSKU.OrderByDescending(x => x.CreateAt).FirstOrDefault().Price.Value;
                SkuPurchasingVM.Average = PurchaseSKU.Average(x => x.Price).Value;
                SkuPurchasingVM.Lowest = PurchaseSKU.OrderBy(x => x.Price).FirstOrDefault().Price.Value;
                SkuPurchasingVM.Highest = PurchaseSKU.OrderByDescending(x => x.Price).FirstOrDefault().Price.Value;
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
        public ActionResult GetHistory(string ID)
        {
            var SerialsLlist = db.SerialsLlist.Where(x => x.SerialsNo == ID).OrderByDescending(x => x.CreateAt);
            return View(SerialsLlist);
        }
    }
}