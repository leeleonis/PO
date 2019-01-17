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
        public ActionResult Index(int ID)
        {
            ViewBag.WarehouseID = ID;
            var SCID = db.WarehouseSummary.Where(x => x.WarehouseID == ID && x.Type == "SCID").FirstOrDefault().Val;
            var AllSKUList = db.SKU.Where(x => x.IsEnable && x.Status == 1).Select(x => new { x.SkuID, x.SkuLang.FirstOrDefault().Name }).ToList();
            var PurchaseSKU = db.PurchaseSKU.Where(x => x.IsEnable && x.PurchaseOrder.Warehouse1.ID == ID).Include(x => x.SerialsLlist).ToList();
            var WarehouseInventoryVM = new WarehouseInventoryVM();

            var Awaitinglist = GetAwaitingCount("", SCID);
            var WarehouseVM = PurchaseSKU.Select(x => new WarehouseVM
            {
                ID = x.ID,
                Name = x.Name,
                SKU = x.SkuNo,
                POQTY = GetPOQty(x),
                CMQTY = GetCMQty(x),
                OrderQTY = GetOrderQTY(x),
                TransferInQTY = GetTransferInQTY(x),
                TransferOutQTY = GetTransferOutQTY(x),
                Velocity = GetVelocity(x), 
                DaysOfSupply = 0,
                Aggregate = 0,//可上架的庫存總數
                Awaiting = 0,//等待出貨的庫總量
                Fulfillable = 0,
                Unfulfillable = 0
            }).ToList();

            var GroupWarehouseVM = WarehouseVM.GroupBy(x => new { x.SKU }).Select(x => new WarehouseVM //SKU數量總計
            {
                ID = x.FirstOrDefault().ID,
                Name = x.FirstOrDefault().Name,
                SKU = x.Key.SKU,
                POQTY = x.Sum(p => p.POQTY),
                CMQTY = x.Sum(p => p.CMQTY),
                OrderQTY = x.Sum(p => p.OrderQTY),
                TransferInQTY = x.Sum(p => p.TransferInQTY),
                TransferOutQTY = x.Sum(p => p.TransferOutQTY),
                Velocity = x.Sum(p => p.Velocity),
                //DaysOfSupply = x.Sum(p => p.DaysOfSupply),
                //Aggregate = x.Sum(p => p.Aggregate),
                //Awaiting = x.Sum(p => p.Awaiting),
                //Fulfillable = x.Sum(p => p.Fulfillable),
                //Unfulfillable = x.Sum(p => p.Unfulfillable),
            }).ToList();
            //把所有的SKU放進去
            foreach (var item in AllSKUList)
            {
                if (!GroupWarehouseVM.Where(x => x.SKU == item.SkuID).Any())
                {
                    GroupWarehouseVM.Add(new WarehouseVM
                    {
                        ID = 0,
                        SKU = item.SkuID,
                        Name = item.Name,
                        POQTY = 0,
                        CMQTY = 0,
                        OrderQTY = 0,
                        TransferInQTY = 0,
                        TransferOutQTY = 0,
                        Velocity = 0,
                        DaysOfSupply = 0,
                        Aggregate = 0,//可上架的庫存總數
                        Awaiting = 0,//等待出貨的庫總量
                        Fulfillable = 0,
                        Unfulfillable = 0
                    });
                }
            }
            foreach (var item in GroupWarehouseVM)
            {
                item.Fulfillable = item.POQTY + item.CMQTY + item.OrderQTY + item.TransferInQTY + item.TransferOutQTY;
                item.Unfulfillable = item.TransferInQTY + item.TransferOutQTY;
                item.Awaiting = Awaitinglist.Where(x => x.SKU == item.SKU && x.SCID == SCID).FirstOrDefault()?.QTY ?? 0;
                item.Aggregate = item.Fulfillable - item.Awaiting;//Aggregate = Fulfillable - Awaiting dispatch
                item.DaysOfSupply = item.Aggregate != 0 && item.Velocity != 0 ? item.Aggregate / item.Velocity / 30 : 0;  //Days of supply 算法: Aggregate / Velocity (30 days) / 30
                                                                                                                          //item.Fulfillable = item.Awaiting + item.Aggregate; //Fulfillable = Awaiting dispatch + Aggregate 2018/12/28 拿掉公式
            }

            WarehouseInventoryVM.Company = PurchaseSKU.FirstOrDefault()?.PurchaseOrder.Company.Name;
            WarehouseInventoryVM.WarehouseType = PurchaseSKU.FirstOrDefault()?.PurchaseOrder.Warehouse1.Type;
            WarehouseInventoryVM.Fulfillable = PurchaseSKU.FirstOrDefault()?.PurchaseOrder.Warehouse1.Fulfillable;
            WarehouseInventoryVM.Location = PurchaseSKU.FirstOrDefault()?.PurchaseOrder.Warehouse1.Location;
            WarehouseInventoryVM.Countries = PurchaseSKU.FirstOrDefault()?.PurchaseOrder.Warehouse1.Countries;
            WarehouseInventoryVM.Marketplace = PurchaseSKU.FirstOrDefault()?.PurchaseOrder.Warehouse1.Marketplace;
            WarehouseInventoryVM.WarehouseVM = GroupWarehouseVM.OrderByDescending(x => x.Fulfillable).ThenByDescending(x => x.Awaiting);

            return View(WarehouseInventoryVM);
        }
        /// <summary>
        /// 取30天內訂單出貨數量
        /// </summary>
        /// <param name="PurchaseSKU"></param>
        /// <returns></returns>
        private int GetVelocity(PurchaseSKU PurchaseSKU)
        {
            var edt = DateTime.UtcNow;
            var sdt = edt.AddDays(-30);
            var count = 0;
            count= PurchaseSKU.SerialsLlist.Where(z => z.SerialsType == "Order" && z.CreateAt >= sdt && z.CreateAt <= sdt).Sum(z => z.SerialsQTY).Value;
            return count;
        }
        /// <summary>
        /// 取移庫入庫序號數量
        /// </summary>
        /// <param name="PurchaseSKU"></param>
        /// <returns></returns>
        private int GetTransferOutQTY(PurchaseSKU PurchaseSKU)
        {
            var count = 0;
            count= PurchaseSKU.SerialsLlist.Where(y => y.SerialsType == "TransferOut").Sum(y => y.SerialsQTY).Value;
            return count;
        }
        /// <summary>
        /// 取移庫出庫序號數量
        /// </summary>
        /// <param name="PurchaseSKU"></param>
        /// <returns></returns>
        private int GetTransferInQTY(PurchaseSKU PurchaseSKU)
        {
            var count = 0;
            count = PurchaseSKU.SerialsLlist.Where(y => y.SerialsType == "TransferIn").Sum(y => y.SerialsQTY).Value;
            return count;
        }
        /// <summary>
        /// 取PO採購數量
        /// </summary>
        /// <param name="PurchaseSKU"></param>
        /// <returns></returns>
        private int GetOrderQTY(PurchaseSKU PurchaseSKU)
        {
            var count = 0;
            count = PurchaseSKU.SerialsLlist.Where(z => z.SerialsType == "Order").Sum(z => z.SerialsQTY).Value;
            return count;
        }
        /// <summary>
        /// 取PO序號數量
        /// </summary>
        /// <param name="PurchaseSKU"></param>
        /// <returns></returns>
        private int GetPOQty(PurchaseSKU PurchaseSKU)
        {
            var count = 0;
            if (PurchaseSKU.QTYFulfilled.HasValue)
            {
                count = PurchaseSKU.QTYFulfilled.Value;
            }
            else
            {
                count = PurchaseSKU.SerialsLlist.Where(y => y.SerialsType == "PO").Sum(y => y.SerialsQTY) ?? 0;//有輸入直接讀輸入，沒輸入計算序號數
            }
            return count;
        }
        /// <summary>
        /// 取CM序號數量
        /// </summary>
        /// <param name="PurchaseSKU"></param>
        /// <returns></returns>
        private int GetCMQty(PurchaseSKU PurchaseSKU)
        {
            var count = 0;
            foreach (var Serialsitem in PurchaseSKU.SerialsLlist.Where(x=>x.SerialsLlistC.Any(y => y.SerialsType == "CM")))
            {
                count += Serialsitem.SerialsLlistC.Where(x => x.SerialsType == "CM").Sum(x => x.SerialsQTY).Value;
            }
            return count;
        }

        public ActionResult Statement(string SKU,int WarehouseID)
        {
            ViewBag.WarehouseID = WarehouseID;
            var PurchaseSKU = db.PurchaseSKU.Where(x => x.IsEnable && x.SkuNo== SKU);
            ViewBag.Awaitinglist = GetAwaitingCount(SKU, "");
            return View(PurchaseSKU);
        }

        public ActionResult Create()
        {
            return View();
        }
        public ActionResult Serials(string SKU, int WarehouseID)
        {
            ViewBag.WarehouseID = WarehouseID;
            var PurchaseSKU = db.PurchaseSKU.Where(x => x.IsEnable && x.SkuNo == SKU);
            return View(PurchaseSKU);
        }
        public ActionResult Inventory(string SKU, int WarehouseID)
        {
            ViewBag.WarehouseID = WarehouseID;
            var SkuInventoryVM = new List<SkuInventoryVM>();
            var PurchaseSKU = db.PurchaseSKU.Where(x => x.IsEnable && x.SkuNo == SKU && x.PurchaseOrderID.HasValue && x.PurchaseOrder.WarehouseID.HasValue);
            //取待出貨
            var AwaitingCountlist = GetAwaitingCount(SKU, "");
            foreach (var item in PurchaseSKU)
            {
                SkuInventoryVM.Add(new SkuInventoryVM
                {
                    ID = item.PurchaseOrder.Warehouse1.ID,
                    Warehouse = item.PurchaseOrder.Warehouse1.Name,
                    Type = item.PurchaseOrder.Warehouse1.Type,
                    SCID = GetSCID(item),
                    POQTY = GetPOQty(item),
                    Available = 0,
                    Aggregate = 0,//可上架的庫存總數
                                  // Awaiting = GetAwaitingCount(item.SkuNo, GetSCID(item)),//等待出貨的庫總量
                    Unfulfillable = 0,
                    Total = 0,
                    BackOrdered = item.QTYOrdered ?? 0,
                    CMQTY = GetCMQty(item),
                    TransferInQTY = GetTransferInQTY(item),
                    TransferOutQTY = GetTransferOutQTY(item),
                    OrderQTY = GetOrderQTY(item)
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

            }).ToList();

            foreach (var item in GroupSkuInventoryVM)
            {
                item.Awaiting = AwaitingCountlist.Where(x => x.SKU == SKU && x.SCID == item.SCID).FirstOrDefault()?.QTY ?? 0;
                item.Available = item.POQTY + item.CMQTY + item.OrderQTY + item.TransferInQTY + item.TransferOutQTY;
                item.Unfulfillable = item.TransferInQTY - item.TransferOutQTY;
                item.Aggregate = item.Available - item.Awaiting;//Aggregate = Fulfillable - Awaiting dispatch
            }
            return View(GroupSkuInventoryVM);
        }
        /// <summary>
        /// 取SCID
        /// </summary>
        /// <param name="PurchaseSKU"></param>
        /// <returns></returns>
        private string GetSCID(PurchaseSKU PurchaseSKU)
        {
            return PurchaseSKU.PurchaseOrder.Warehouse1?.WarehouseSummary.FirstOrDefault(x => x.Type == "SCID")?.Val;
        }

        public ActionResult Purchasing(string SKU,int? Inventory, int? Velocity, int WarehouseID)
        {
            ViewBag.WarehouseID = WarehouseID;
            var PurchaseSKU = db.PurchaseSKU.Where(x => x.IsEnable && x.SkuNo == SKU && x.SerialsLlist.Any(y => y.SerialsType == "PO"));
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
                        var SCID = PurchaseSKUitem.PurchaseOrder.Warehouse1?.WarehouseSummary.Where(x => x.Type == "SCID").FirstOrDefault().Val;
                        SkuPurchasingVM.BackOrdered += PurchaseSKUitem.QTYOrdered ?? 0;
                        SkuPurchasingVM.Awaiting += AwaitingCountlist.Where(x => x.SKU == SKU && x.SCID == SCID).FirstOrDefault()?.QTY ?? 0;
                        SkuPurchasingVM.POQTY += PurchaseSKUitem.QTYOrdered.HasValue ? PurchaseSKUitem.QTYOrdered.Value : PurchaseSKUitem.SerialsLlist.Where(y => y.SerialsType == "PO").Sum(y => y.SerialsQTY) ?? 0;//有輸入直接讀輸入，沒輸入計算序號數
                        SkuPurchasingVM.CMQTY += GetCMQty(PurchaseSKUitem);
                        SkuPurchasingVM.OrderQTY += GetOrderQTY(PurchaseSKUitem);
                        SkuPurchasingVM.TransferInQTY += GetTransferInQTY(PurchaseSKUitem);
                        SkuPurchasingVM.TransferOutQTY += GetTransferOutQTY(PurchaseSKUitem);
                        SkuPurchasingVM.Velocity += GetVelocity(PurchaseSKUitem);
                        SkuPurchasingVM.UnfulfillableRMA += PurchaseSKUitem.SerialsLlist.Where(y => y.SerialsType == "RMA").Sum(y => y.SerialsQTY).Value;
                    }
                }
              

                SkuPurchasingVM.Fulfillable = SkuPurchasingVM.POQTY + SkuPurchasingVM.CMQTY + SkuPurchasingVM.OrderQTY + SkuPurchasingVM.TransferInQTY + SkuPurchasingVM.TransferOutQTY - SkuPurchasingVM.Awaiting;
                SkuPurchasingVM.Aggregate = SkuPurchasingVM.Fulfillable - SkuPurchasingVM.Awaiting;//Aggregate = Fulfillable - Awaiting dispatch
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
        /// <summary>
        /// 序號清單
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ActionResult GetHistory(string ID)
        {
            var SerialsLlist = db.SerialsLlist.Where(x => x.SerialsNo == ID).OrderByDescending(x => x.CreateAt);
            return View(SerialsLlist);
        }
    }
}