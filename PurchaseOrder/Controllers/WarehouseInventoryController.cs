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
            ViewBag.WarehouseID = ID;
            var Warehouse = db.Warehouse.Find(ID);
            var SCID = Warehouse.WarehouseSummary.Where(x => x.Type == "SCID").FirstOrDefault().Val;
            var Awaitinglist = GetAwaitingCount("", SCID);
            var WarehouseVM = new List<WarehouseVM>();
            var GroupWarehouseVM =new List<WarehouseVM>();
            var WarehouseInventoryVM = new WarehouseInventoryVM();
            var AllSKUList = db.SKU.Where(x => x.IsEnable && x.Status == 1).Select(x => new { x.SkuID, x.SkuLang.FirstOrDefault().Name }).ToList();

            WarehouseInventoryVM.WarehouseType = Warehouse.Type;
            WarehouseInventoryVM.Fulfillable = Warehouse.Fulfillable;
            WarehouseInventoryVM.Location = Warehouse.Location;
            WarehouseInventoryVM.Countries = Warehouse.Countries;
            WarehouseInventoryVM.Marketplace = Warehouse.Marketplace;

            if (Warehouse.Type == "Interim")
            {
                var TransferSKU = db.TransferSKU.Where(x => x.IsEnable && x.Transfer.Interim == ID && x.Transfer.Status == "Shipped").Include(x => x.SerialsLlist).ToList();//移倉中


                WarehouseVM.AddRange(TransferSKU.Select(x => new WarehouseVM
                {
                    ID = x.ID,
                    Name = x.Name,
                    SKU = x.SkuNo,
                    POQTY = x.SerialsLlist.Where(y => y.SerialsType == "TransferOut" && !y.SerialsLlistC.Any()).Sum(y => y.SerialsQTY) * -1 ?? 0//借用
                }).ToList());
                GroupWarehouseVM = WarehouseVM.GroupBy(x => new { x.SKU }).Select(x => new WarehouseVM //SKU數量總計
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
                    TransferAwaiting = x.Sum(p => p.TransferAwaiting),
                    //DaysOfSupply = x.Sum(p => p.DaysOfSupply),
                    //Aggregate = x.Sum(p => p.Aggregate),
                    //Awaiting = x.Sum(p => p.Awaiting),
                    //Fulfillable = x.Sum(p => p.Fulfillable),
                    //Unfulfillable = x.Sum(p => p.Unfulfillable),
                }).ToList();
            }
            else
            {
                var PurchaseSKU = db.PurchaseSKU.Where(x => x.IsEnable && x.PurchaseOrder.WarehousePO.ID == ID).Include(x => x.SerialsLlist).ToList();//PO單
                var TransferSKU = db.TransferSKU.Where(x => x.IsEnable && x.Transfer.ToWID == ID).Include(x => x.SerialsLlist).ToList();//移倉單入庫
                WarehouseInventoryVM.Company = PurchaseSKU.FirstOrDefault()?.PurchaseOrder.Company.Name;

               
                if (!string.IsNullOrWhiteSpace(Product))
                {
                    PurchaseSKU = PurchaseSKU.Where(x => x.SkuNo == Product).ToList();
                    TransferSKU = TransferSKU.Where(x => x.SkuNo == Product).ToList();
                }

                WarehouseVM.AddRange(PurchaseSKU.Select(x => new WarehouseVM
                {
                    ID = x.ID,
                    Name = x.Name,
                    SKU = x.SkuNo,
                    POQTY = GetPOQty(x),
                    CMQTY = GetCMQty(x),
                    OrderQTY = GetOrderQTY(x),
                    //TransferInQTY = GetTransferInQTY(x, Warehouse),
                    TransferOutQTY = GetTransferOutQTY(x, Warehouse),
                    TransferAwaiting = GetTransferAwaiting(x, Warehouse),//等待移倉的數量
                    Velocity = GetVelocity(x),
                    DaysOfSupply = 0,
                    Aggregate = 0,//可上架的庫存總數
                    Awaiting = 0,//等待出貨的庫總量
                    Fulfillable = 0,
                    Unfulfillable = 0
                }).ToList());
                WarehouseVM.AddRange(TransferSKU.Select(x => new WarehouseVM
                {
                    ID = x.ID,
                    Name = x.Name,
                    SKU = x.SkuNo,
                    TransferInQTY = GetTransferInQTY(x),
                }).ToList());
                GroupWarehouseVM = WarehouseVM.GroupBy(x => new { x.SKU }).Select(x => new WarehouseVM //SKU數量總計
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
                    TransferAwaiting = x.Sum(p => p.TransferAwaiting),
                    //DaysOfSupply = x.Sum(p => p.DaysOfSupply),
                    //Aggregate = x.Sum(p => p.Aggregate),
                    //Awaiting = x.Sum(p => p.Awaiting),
                    //Fulfillable = x.Sum(p => p.Fulfillable),
                    //Unfulfillable = x.Sum(p => p.Unfulfillable),
                }).ToList();
            }
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
                item.Awaiting = (Awaitinglist.Where(x => x.SKU == item.SKU && x.SCID == SCID).FirstOrDefault()?.QTY ?? 0) - item.TransferAwaiting;
                item.Aggregate = item.Fulfillable - item.Awaiting;//Aggregate = Fulfillable - Awaiting dispatch
                item.DaysOfSupply = item.Aggregate != 0 && item.Velocity != 0 ? item.Aggregate / item.Velocity / 30 : 0;  //Days of supply 算法: Aggregate / Velocity (30 days) / 30
                                                                                                                          //item.Fulfillable = item.Awaiting + item.Aggregate; //Fulfillable = Awaiting dispatch + Aggregate 2018/12/28 拿掉公式
            }
            if (FulfillableMin.HasValue)
            {
                GroupWarehouseVM = GroupWarehouseVM.Where(x => x.Aggregate >= FulfillableMin).ToList();
            }
            if (FulfillableMax.HasValue)
            {
                GroupWarehouseVM = GroupWarehouseVM.Where(x => x.Aggregate <= FulfillableMax).ToList();
            }
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
        /// <param name="WarehouseType">倉庫</param>
        /// <returns></returns>
        private int GetTransferOutQTY(PurchaseSKU PurchaseSKU, Warehouse Warehouse)
        {
            return GetTransferAwaitingOut(PurchaseSKU, "Shipped", Warehouse);
        }
        /// <summary>
        /// 待移倉的數量
        /// </summary>
        /// <param name="PurchaseSKU"></param>
        /// <param name="Warehouse">倉庫</param>
        /// <returns></returns>
        private int GetTransferAwaiting(PurchaseSKU PurchaseSKU, Warehouse Warehouse)
        {
            return GetTransferAwaitingOut( PurchaseSKU, "Requested", Warehouse);
        }
        private int GetTransferAwaitingOut(PurchaseSKU PurchaseSKU,string Status, Warehouse Warehouse)
        {
            var count = 0;
            //只找移出倉的ID
            count = PurchaseSKU.SerialsLlist.Where(y => y.SerialsType == "TransferOut" && y.TransferSKU.IsEnable && y.TransferSKU.Transfer.IsEnable && y.TransferSKU.Transfer.FromWID == Warehouse.ID && y.TransferSKU.Transfer.Status == Status).Sum(y => y.SerialsQTY).Value;
            return count;
        }

        /// <summary>
        /// 取移庫出庫序號數量
        /// </summary>
        /// <param name="TransferSKU"></param>
        /// <returns></returns>
        private int GetTransferInQTY(TransferSKU TransferSKU)
        {
            var count = 0;
            //只找移入倉的ID
            count = TransferSKU.SerialsLlist.Where(y => y.SerialsType == "TransferIn" && y.TransferSKU.IsEnable && y.TransferSKU.Transfer.IsEnable).Sum(y => y.SerialsQTY).Value;
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
            if (PurchaseSKU.QTYFulfilled.HasValue && PurchaseSKU.QTYFulfilled > 0)
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
            //var Warehouse = db.Warehouse.Find(WarehouseID);
            var SkuInventoryVM = new List<SkuInventoryVM>();
            var PurchaseSKU = db.PurchaseSKU.Where(x => x.IsEnable && x.SkuNo == SKU && x.PurchaseOrderID.HasValue && x.PurchaseOrder.WarehouseID.HasValue);
            var TransferSKU = db.TransferSKU.Where(x => x.IsEnable && x.SkuNo == SKU && x.TransferID.HasValue && x.Transfer.ToWID.HasValue);
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
                    Available = 0,
                    Aggregate = 0,//可上架的庫存總數
                                  // Awaiting = GetAwaitingCount(item.SkuNo, GetSCID(item)),//等待出貨的庫總量
                    Unfulfillable = 0,
                    Total = PurchaseSKU.Sum(x => x.SerialsLlist.Sum(t => t.SerialsQTY)) ?? 0,
                    BackOrdered = item.QTYOrdered ?? 0,
                    CMQTY = GetCMQty(item),
                    //TransferInQTY = GetTransferInQTY(item),
                    TransferOutQTY = GetTransferOutQTY(item, item.PurchaseOrder.WarehousePO),
                    TransferAwaiting = GetTransferAwaiting(item, item.PurchaseOrder.WarehousePO),//等待移倉的數量
                    OrderQTY = GetOrderQTY(item),
                    Value = GetValue(item)
                });
            }
            foreach (var item in TransferSKU)
            {
                SkuInventoryVM.Add(new SkuInventoryVM
                {
                    ID = item.Transfer.WarehouseTo.ID,
                    Warehouse = item.Transfer.WarehouseTo.Name,
                    Type = item.Transfer.WarehouseTo.Type,
                    TransferInQTY = GetTransferInQTY(item),
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
                item.Awaiting = (AwaitingCountlist.Where(x => x.SKU == SKU && x.SCID == item.SCID).FirstOrDefault()?.QTY ?? 0) - item.TransferAwaiting;
                item.Available = item.POQTY + item.CMQTY + item.OrderQTY + item.TransferInQTY + item.TransferOutQTY;
                item.Unfulfillable = item.TransferInQTY + item.TransferOutQTY;
                item.Aggregate = item.Available - item.Awaiting;//Aggregate = Fulfillable - Awaiting dispatch
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

        public ActionResult Purchasing(string SKU,int? Inventory, int? Velocity, int WarehouseID)
        {
            ViewBag.WarehouseID = WarehouseID;
            //var Warehouse = db.Warehouse.Find(WarehouseID);
            var PurchaseSKU = db.PurchaseSKU.Where(x => x.IsEnable && x.SkuNo == SKU && x.SerialsLlist.Any(y => y.SerialsType == "PO"));
            var TransferSKU = db.TransferSKU.Where(x => x.IsEnable && x.SkuNo == SKU && x.SerialsLlist.Any(y => y.SerialsType == "TransferIn"));
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
                        var SCID = PurchaseSKUitem.PurchaseOrder.WarehousePO?.WarehouseSummary.Where(x => x.Type == "SCID").FirstOrDefault().Val;
                        SkuPurchasingVM.BackOrdered += PurchaseSKUitem.QTYOrdered ?? 0;
                        SkuPurchasingVM.Awaiting += (AwaitingCountlist.Where(x => x.SKU == SKU && x.SCID == SCID).FirstOrDefault()?.QTY ?? 0) - GetTransferAwaiting(PurchaseSKUitem, PurchaseSKUitem.PurchaseOrder.WarehousePO);
                        SkuPurchasingVM.POQTY += PurchaseSKUitem.QTYOrdered.HasValue ? PurchaseSKUitem.QTYOrdered.Value : PurchaseSKUitem.SerialsLlist.Where(y => y.SerialsType == "PO").Sum(y => y.SerialsQTY) ?? 0;//有輸入直接讀輸入，沒輸入計算序號數
                        SkuPurchasingVM.CMQTY += GetCMQty(PurchaseSKUitem);
                        SkuPurchasingVM.OrderQTY += GetOrderQTY(PurchaseSKUitem);
                        //SkuPurchasingVM.TransferInQTY += GetTransferInQTY(PurchaseSKUitem);
                        SkuPurchasingVM.TransferOutQTY += GetTransferOutQTY(PurchaseSKUitem, PurchaseSKUitem.PurchaseOrder.WarehousePO);
                        SkuPurchasingVM.Velocity += GetVelocity(PurchaseSKUitem);
                        SkuPurchasingVM.UnfulfillableRMA += PurchaseSKUitem.SerialsLlist.Where(y => y.SerialsType == "RMA").Sum(y => y.SerialsQTY).Value;
                    }
                }
                foreach (var TransferSKUitem in TransferSKU)
                {
                    if (TransferSKUitem.SerialsLlist.Any())
                    {
                        SkuPurchasingVM.TransferInQTY += GetTransferInQTY(TransferSKUitem);
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