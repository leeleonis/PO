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
            var WarehouseInventoryVM = new WarehouseInventoryVM();
            WarehouseInventoryVM.WarehouseType = Warehouse.Type;
            WarehouseInventoryVM.Fulfillable = Warehouse.Fulfillable;
            WarehouseInventoryVM.Location = Warehouse.Location;
            WarehouseInventoryVM.Countries = Warehouse.Countries;
            WarehouseInventoryVM.Marketplace = Warehouse.Marketplace;
            WarehouseInventoryVM.Company = Warehouse.Company;
            WarehouseInventoryVM.WarehouseVM = GetWarehouseVMList(Warehouse, Product, FulfillableMin, FulfillableMax);
            return View(WarehouseInventoryVM);
        }

        private IEnumerable<WarehouseVM> GetWarehouseVMList(Warehouse Warehouse, string Product, int? FulfillableMin, int? FulfillableMax)
        {
            var SCID = Warehouse.WarehouseSummary.Where(x => x.Type == "SCID").FirstOrDefault()?.Val;
            var Awaitinglist = GetAwaitingCount("", SCID);
            var AllSKUList = db.SKU.Where(x => x.IsEnable && x.Status == 1).Select(x => new { x.SkuID, x.SkuLang.FirstOrDefault().Name }).ToList();
            var WarehouseVM = new List<WarehouseVM>();
            var GroupWarehouseVM = new List<WarehouseVM>();
            if (Warehouse.Type == "Interim")
            {
                var TransferSKU = db.TransferSKU.Where(x => x.IsEnable && x.Transfer.Interim == Warehouse.ID && x.Transfer.Status == "Shipped").Include(x => x.SerialsLlist).ToList();//移倉中
                if (!string.IsNullOrWhiteSpace(Product))
                {
                    TransferSKU = TransferSKU.Where(x => x.SkuNo == Product).ToList();
                }
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
                var NoList = new List<int>();
                var PurchaseSKU = db.PurchaseSKU.Where(x => x.IsEnable && x.PurchaseOrder.IsEnable && x.PurchaseOrder.WarehousePO.ID == Warehouse.ID).Include(x => x.SerialsLlist).ToList();//PO單
                var TransferToSKU = db.TransferSKU.Where(x => x.IsEnable && x.Transfer.IsEnable && x.Transfer.ToWID == Warehouse.ID).Include(x => x.SerialsLlist).ToList();//移倉單入庫
                var TransferFromSKU = db.TransferSKU.Where(x => x.IsEnable && x.Transfer.IsEnable && x.Transfer.FromWID == Warehouse.ID).Include(x => x.SerialsLlist).ToList();//移倉單出庫
                var RMASerialsLlist = db.RMASerialsLlist.Where(x => x.RMASKU.IsEnable && x.RMASKU.RMA.IsEnable && x.WarehouseID == Warehouse.ID).ToList();//RMA單


                if (!string.IsNullOrWhiteSpace(Product))
                {
                    PurchaseSKU = PurchaseSKU.Where(x => x.SkuNo == Product).ToList();
                    TransferToSKU = TransferToSKU.Where(x => x.SkuNo == Product).ToList();
                    TransferFromSKU = TransferFromSKU.Where(x => x.SkuNo == Product).ToList();
                    RMASerialsLlist = RMASerialsLlist.Where(x => (!string.IsNullOrWhiteSpace(x.NewSkuNo) && x.NewSkuNo == Product) || (string.IsNullOrWhiteSpace(x.NewSkuNo) && x.RMASKU.SkuNo == Product)).ToList();
                }
                WarehouseVM.AddRange(PurchaseSKU.Select(x => new WarehouseVM
                {
                    ID = x.ID,
                    Name = x.Name,
                    SKU = x.SkuNo,
                    BackOrdered = x.QTYOrdered ?? 0,
                    POQTY = GetPOQty(x),
                    CMQTY = GetCMQty(x),
                    OrderQTY = GetOrderQTY(x),
                    //TransferInQTY = GetTransferInQTY(x, Warehouse,"", ref NoList),
                    TransferOutQTY = GetTransferOutQTY(x, Warehouse, "", ref NoList),
                    TransferAwaiting = GetTransferAwaiting(x, Warehouse, ref NoList),//等待移倉的數量
                    TransferOutCloseQTY = GetTransferOutCloseQTY(x, Warehouse, ref NoList),//移倉已入庫
                    UnfulfillableRMA = GetUnfulfillableRMA(x, Warehouse, ref NoList),//RMA數量
                    Velocity = GetVelocity(x),
                    DaysOfSupply = 0,
                    Aggregate = 0,//可上架的庫存總數
                    Awaiting = 0,//等待出貨的庫總量
                    Fulfillable = 0,
                    Unfulfillable = 0
                }).ToList());


                WarehouseVM.AddRange(TransferToSKU.Select(x => new WarehouseVM
                {
                    ID = x.ID,
                    Name = x.Name,
                    SKU = x.SkuNo,
                    TransferInQTY = GetTransferInQTY(x, Warehouse, "To", ref NoList),//接入庫當PO
                    TransferOutQTY = GetTransferOutQTY(x, Warehouse, "To", ref NoList) * -1,
                    TransferOutCloseQTY = GetTransferOutCloseQTY(x, Warehouse, ref NoList),//移倉已入庫
                    TransferAwaiting = GetTransferAwaiting(x, Warehouse, ref NoList),//等待移倉的數量
                }).ToList());
                WarehouseVM.AddRange(TransferFromSKU.Select(x => new WarehouseVM
                {
                    ID = x.ID,
                    Name = x.Name,
                    SKU = x.SkuNo,
                    //TransferInQTY = GetTransferInQTY(x, Warehouse, "From", ref NoList),
                    TransferOutQTY = GetTransferOutQTY(x, Warehouse, "From", ref NoList),
                    TransferOutCloseQTY = GetTransferOutCloseQTY(x, Warehouse, ref NoList),
                    TransferAwaiting = GetTransferAwaiting(x, Warehouse, ref NoList),//等待移倉的數量
                }).ToList());
                var NoNewRMASKU = RMASerialsLlist.Where(x => string.IsNullOrWhiteSpace(x.NewSkuNo)).Select(x => x.RMASKU).ToList();//沒有新SKU
                WarehouseVM.AddRange(NoNewRMASKU.Select(x => new WarehouseVM
                {
                    ID = x.ID,
                    Name = x.Name,
                    SKU = x.SkuNo,
                    UnfulfillableRMA = GetRMAInQty(x, x.SkuNo, false, null, Warehouse.ID),
                    TransferInQTY = GetTransferInQTY(x, Warehouse, "", ref NoList),//接入庫當PO
                    TransferOutQTY = GetTransferOutQTY(x, Warehouse, "", ref NoList) * -1,
                    TransferOutCloseQTY = GetTransferOutCloseQTY(x, Warehouse, ref NoList),//移倉已入庫
                    TransferAwaiting = GetTransferAwaiting(x, Warehouse, ref NoList),//等待移倉的數量
                }).ToList());
                var NewRMASKU = RMASerialsLlist.Where(x => !string.IsNullOrWhiteSpace(x.NewSkuNo)).Select(x => x.RMASKU).ToList();//有新SKU
                WarehouseVM.AddRange(NewRMASKU.Select(x => new WarehouseVM
                {
                    ID = x.ID,
                    Name = x.Name,
                    SKU = x.RMASerialsLlist.FirstOrDefault().NewSkuNo,
                    UnfulfillableRMA = GetRMAInQty(x, x.RMASerialsLlist.FirstOrDefault().NewSkuNo, true, null, Warehouse.ID),
                    TransferInQTY = GetTransferInQTY(x, Warehouse, "", ref NoList),//接入庫當PO
                    TransferOutQTY = GetTransferOutQTY(x, Warehouse, "", ref NoList) * -1,
                    TransferOutCloseQTY = GetTransferOutCloseQTY(x, Warehouse, ref NoList),//移倉已入庫
                    TransferAwaiting = GetTransferAwaiting(x, Warehouse, ref NoList),//等待移倉的數量
                }).ToList());

                GroupWarehouseVM = WarehouseVM.GroupBy(x => new { x.SKU }).Select(x => new WarehouseVM //SKU數量總計
                {
                    ID = x.FirstOrDefault().ID,
                    Name = x.FirstOrDefault().Name,
                    SKU = x.Key.SKU,
                    BackOrdered = x.Sum(p => p.BackOrdered),
                    POQTY = x.Sum(p => p.POQTY),
                    CMQTY = x.Sum(p => p.CMQTY),
                    OrderQTY = x.Sum(p => p.OrderQTY),
                    TransferInQTY = x.Sum(p => p.TransferInQTY),
                    TransferOutQTY = x.Sum(p => p.TransferOutQTY),
                    Velocity = x.Sum(p => p.Velocity),
                    TransferAwaiting = x.Sum(p => p.TransferAwaiting),
                    Fulfillable = x.Sum(p => p.Fulfillable),
                    Unfulfillable = x.Sum(p => p.TransferOutCloseQTY),
                    DaysOfSupply = x.Sum(p => p.DaysOfSupply),
                    Aggregate = x.Sum(p => p.Aggregate),
                    Awaiting = x.Sum(p => p.Awaiting),
                    UnfulfillableRMA = x.Sum(p => p.UnfulfillableRMA),
                    //Fulfillable = x.Sum(p => p.Fulfillable),
                }).ToList();
            }
            if (string.IsNullOrWhiteSpace(Product))
            {
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
            }

            foreach (var item in GroupWarehouseVM)
            {
                item.Unfulfillable = Math.Abs(item.Unfulfillable);
                item.Fulfillable = item.POQTY + item.TransferInQTY - item.TransferAwaiting + item.UnfulfillableRMA;
                item.Awaiting = (Awaitinglist.Where(x => x.SKU == item.SKU && x.SCID == SCID).FirstOrDefault()?.QTY ?? 0) - item.TransferAwaiting;
                item.Aggregate = item.Fulfillable - item.Awaiting - item.UnfulfillableRMA;//Aggregate = Fulfillable - Awaiting dispatch
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
            return GroupWarehouseVM.OrderByDescending(x => x.Fulfillable).ThenByDescending(x => x.Awaiting).ToList();
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
            WarehouseInventoryVM.Fulfillable = Warehouse.Fulfillable;
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
            count = PurchaseSKU.SerialsLlist.Where(z => z.SerialsType == "Order" && z.CreateAt >= sdt && z.CreateAt <= sdt).Sum(z => z.SerialsQTY).Value;
            return count;
        }

        /// <summary>
        /// 取RMA數量
        /// </summary>
        /// <param name="SKU"></param>
        /// <param name="WarehouseType">倉庫</param>
        /// <returns></returns>
        private int GetUnfulfillableRMA(dynamic SKU, Warehouse Warehouse, ref List<int> WNoList)
        {
            if (WNoList == null)
            {
                WNoList = new List<int>();
            }
            var NoList = WNoList;
            var count = 0;
            //只找移出倉的ID
            if (SKU.GetType().BaseType.Name == typeof(PurchaseSKU).Name)
            {
                var PurchaseSKU = (PurchaseSKU)SKU;
                var SerialsLlist = PurchaseSKU.SerialsLlist.Where(y => !NoList.Contains(y.ID) && y.SerialsType == "RMA" && !y.SerialsLlistC.Any());
                count = SerialsLlist.Sum(y => y.SerialsQTY).Value;
                WNoList.AddRange(SerialsLlist.Select(x => x.ID));
            }
            else if (SKU.GetType().BaseType.Name == typeof(TransferSKU).Name)
            {
                var TransferSKU = (TransferSKU)SKU;
                var SerialsLlist = TransferSKU.SerialsLlist.Where(y => !NoList.Contains(y.ID) && y.SerialsType == "RMA" && !y.SerialsLlistC.Any());
                count = SerialsLlist.Sum(y => y.SerialsQTY).Value;
                WNoList.AddRange(SerialsLlist.Select(x => x.ID));
            }
            return count;
        }
        /// <summary>
        /// 取移庫入庫序號數量
        /// </summary>
        /// <param name="SKU"></param>
        /// <param name="WarehouseType">倉庫</param>
        /// <returns></returns>
        private int GetTransferOutQTY(dynamic SKU, Warehouse Warehouse, string WarehouseKey, ref List<int> WNoList)
        {
            return GetTransferAwaitingOut(SKU, "Shipped", Warehouse, WarehouseKey, ref WNoList);
        }
        /// <summary>
        /// 待移倉的數量
        /// </summary>
        /// <param name="SKU"></param>
        /// <param name="Warehouse">倉庫</param>
        /// <returns></returns>
        private int GetTransferAwaiting(dynamic SKU, Warehouse Warehouse, ref List<int> WNoList)
        {
            return GetTransferAwaitingOut(SKU, "Requested", Warehouse, "", ref WNoList);
        }
        /// <summary>
        /// 還在運送中的數量
        /// </summary>
        /// <param name="x"></param>
        /// <param name="warehouse"></param>
        /// <returns></returns>
        private int GetTransferOutCloseQTY(dynamic SKU, Warehouse Warehouse, ref List<int> WNoList)
        {
            if (WNoList == null)
            {
                WNoList = new List<int>();
            }
            var NoList = WNoList;
            var count = 0;
            //只找移出倉的ID
            if (SKU.GetType().BaseType.Name == typeof(PurchaseSKU).Name)
            {
                var PurchaseSKU = (PurchaseSKU)SKU;
                var SerialsLlist = PurchaseSKU.SerialsLlist.Where(y => !NoList.Contains(y.ID) && y.SerialsType == "TransferOut" && y.TransferSKU.IsEnable && y.TransferSKU.Transfer.IsEnable && y.TransferSKU.Transfer.FromWID == Warehouse.ID && y.TransferSKU.Transfer.Status == "Shipped" && !y.SerialsLlistC.Any());
                count = SerialsLlist.Sum(y => y.SerialsQTY).Value;
                WNoList.AddRange(SerialsLlist.Select(x => x.ID));
            }
            else if (SKU.GetType().BaseType.Name == typeof(TransferSKU).Name)
            {
                var TransferSKU = (TransferSKU)SKU;
                var SerialsLlist = TransferSKU.SerialsLlist.Where(y => !NoList.Contains(y.ID) && y.SerialsType == "TransferOut" && y.TransferSKU.IsEnable && y.TransferSKU.Transfer.IsEnable && y.TransferSKU.Transfer.FromWID == Warehouse.ID && y.TransferSKU.Transfer.Status == "Shipped" && !y.SerialsLlistC.Any());
                count = SerialsLlist.Sum(y => y.SerialsQTY).Value;
                WNoList.AddRange(SerialsLlist.Select(x => x.ID));
            }
            return count;
        }


        private int GetTransferAwaitingOut(dynamic SKU, string Status, Warehouse Warehouse, string WarehouseKey, ref List<int> WNoList)
        {
            if (WNoList == null)
            {
                WNoList = new List<int>();
            }
            var NoList = WNoList;
            var count = 0;
            //只找移出倉的ID
            if (SKU.GetType().BaseType.Name == typeof(PurchaseSKU).Name)
            {
                var PurchaseSKU = (PurchaseSKU)SKU;
                var SerialsLlist = PurchaseSKU.SerialsLlist.Where(y => !NoList.Contains(y.ID));
                SerialsLlist = SerialsLlist.Where(y => y.SerialsType == "TransferOut");
                SerialsLlist = SerialsLlist.Where(y => y.TransferSKU.IsEnable);
                SerialsLlist = SerialsLlist.Where(y => y.TransferSKU.Transfer.IsEnable);
                if (WarehouseKey == "To")
                {
                    SerialsLlist = SerialsLlist.Where(y => y.TransferSKU.Transfer.ToWID == Warehouse.ID);
                }
                else
                {
                    SerialsLlist = SerialsLlist.Where(y => y.TransferSKU.Transfer.FromWID == Warehouse.ID);
                }
                SerialsLlist = SerialsLlist.Where(y => y.TransferSKU.Transfer.Status == Status);
                SerialsLlist = SerialsLlist.Where(y => !y.SerialsLlistC.Any());
                count = SerialsLlist.Sum(y => y.SerialsQTY).Value;
                WNoList.AddRange(SerialsLlist.Select(x => x.ID));
            }
            else if (SKU.GetType().BaseType.Name == typeof(TransferSKU).Name)
            {
                var TransferSKU = (TransferSKU)SKU;
                //var SerialsLlist = TransferSKU.SerialsLlist;

                var SerialsLlist = TransferSKU.SerialsLlist.Where(y => !NoList.Contains(y.ID));
                SerialsLlist = SerialsLlist.Where(y => y.SerialsType == "TransferOut");
                SerialsLlist = SerialsLlist.Where(y => y.TransferSKU.IsEnable);
                SerialsLlist = SerialsLlist.Where(y => y.TransferSKU.Transfer.IsEnable).ToList();
                if (WarehouseKey == "To")
                {
                    SerialsLlist = SerialsLlist.Where(y => y.TransferSKU.Transfer.ToWID == Warehouse.ID);
                }
                else
                {
                    SerialsLlist = SerialsLlist.Where(y => y.TransferSKU.Transfer.FromWID == Warehouse.ID);
                }
                SerialsLlist = SerialsLlist.Where(y => y.TransferSKU.Transfer.Status == Status).ToList();
                SerialsLlist = SerialsLlist.Where(y => !y.SerialsLlistC.Any()).ToList();
                count = SerialsLlist.Sum(y => y.SerialsQTY).Value;
                WNoList.AddRange(SerialsLlist.Select(x => x.ID));
                //count = TransferSKU.SerialsLlist.Where(y => !NoList.Contains(y.ID) && y.SerialsType == "TransferOut" && y.TransferSKU.IsEnable && y.TransferSKU.Transfer.IsEnable && y.TransferSKU.Transfer.FromWID == Warehouse.ID && y.TransferSKU.Transfer.Status == Status).Sum(y => y.SerialsQTY).Value;
            }
            return count;
        }
        /// <summary>
        /// 取移庫出庫序號數量
        /// </summary>
        /// <param name="SKU"></param>
        /// <param name="Warehouse"></param>
        /// <returns></returns>
        private int GetTransferInQTY(dynamic SKU, Warehouse Warehouse, string WarehouseKey, ref List<int> WNoList)
        {
            if (WNoList == null)
            {
                WNoList = new List<int>();
            }
            var NoList = WNoList;
            var count = 0;
            //只找移入倉的ID
            if (SKU.GetType().BaseType.Name == typeof(PurchaseSKU).Name)
            {
                var PurchaseSKU = (PurchaseSKU)SKU;
                //var SerialsLlist = PurchaseSKU.SerialsLlist;
                var SerialsLlist = PurchaseSKU.SerialsLlist.Where(y => !NoList.Contains(y.ID) && !y.SerialsLlistC.Any()); ;
                SerialsLlist = SerialsLlist.Where(y => y.SerialsType == "TransferIn");
                SerialsLlist = SerialsLlist.Where(y => y.TransferSKU.IsEnable);
                SerialsLlist = SerialsLlist.Where(y => y.TransferSKU.Transfer.IsEnable);
                if (WarehouseKey == "From")
                {
                    SerialsLlist = SerialsLlist.Where(y => y.TransferSKU.Transfer.FromWID == Warehouse.ID);
                }
                else
                {
                    SerialsLlist = SerialsLlist.Where(y => y.TransferSKU.Transfer.ToWID == Warehouse.ID);
                }
                WNoList.AddRange(SerialsLlist.Select(x => x.ID));
                //count = PurchaseSKU.SerialsLlist.Where(y => !NoList.Contains(y.ID) && y.SerialsType == "TransferIn" && y.TransferSKU.IsEnable && y.TransferSKU.Transfer.IsEnable && y.TransferSKU.Transfer.ToWID == Warehouse.ID).Sum(y => y.SerialsQTY).Value;
            }
            else if (SKU.GetType().BaseType.Name == typeof(TransferSKU).Name)
            {
                var TransferSKU = (TransferSKU)SKU;
                var SerialsLlist = TransferSKU.SerialsLlist.Where(y => !NoList.Contains(y.ID));
                SerialsLlist = SerialsLlist.Where(y => !y.SerialsLlistC.Any());
                SerialsLlist = SerialsLlist.Where(y => y.SerialsType == "TransferIn");
                SerialsLlist = SerialsLlist.Where(y => y.TransferSKU.IsEnable);
                SerialsLlist = SerialsLlist.Where(y => y.TransferSKU.Transfer.IsEnable);
                if (WarehouseKey == "From")
                {
                    SerialsLlist = SerialsLlist.Where(y => y.TransferSKU.Transfer.FromWID == Warehouse.ID);
                }
                else
                {
                    SerialsLlist = SerialsLlist.Where(y => y.TransferSKU.Transfer.ToWID == Warehouse.ID);
                }
                count = SerialsLlist.Sum(y => y.SerialsQTY).Value;
                WNoList.AddRange(SerialsLlist.Select(x => x.ID));
            }
            return count;
        }
        /// <summary>
        /// 取PO採購數量
        /// </summary>
        /// <param name="PurchaseSKU"></param>
        /// <returns></returns>
        private int GetOrderQTY(dynamic SKU, List<int> NoList = null)
        {
            if (NoList == null)
            {
                NoList = new List<int>();
            }
            var count = 0;
            //只找移入倉的ID
            if (SKU.GetType().BaseType.Name == typeof(PurchaseSKU).Name)
            {
                var PurchaseSKU = (PurchaseSKU)SKU;
                var SerialsLlist = PurchaseSKU.SerialsLlist.Where(z => !NoList.Contains(z.ID) && z.SerialsType == "Order");
                count = SerialsLlist.Sum(y => y.SerialsQTY).Value;
                //WNoList.AddRange(SerialsLlist.Select(x => x.ID));
            }
            else if (SKU.GetType().BaseType.Name == typeof(TransferSKU).Name)
            {
                var TransferSKU = (TransferSKU)SKU;
                var SerialsLlist = TransferSKU.SerialsLlist.Where(z => !NoList.Contains(z.ID) && z.SerialsType == "Order");
                count = SerialsLlist.Sum(y => y.SerialsQTY).Value;
                //WNoList.AddRange(SerialsLlist.Select(x => x.ID));
            }

            return count;


        }
        /// <summary>
        /// 取PO序號數量
        /// </summary>
        /// <param name="PurchaseSKU"></param>
        /// <returns></returns>
        private int GetPOQty(dynamic SKU, List<int> NoList = null, int? WarehouseID = null)
        {
            if (NoList == null)
            {
                NoList = new List<int>();
            }
            var count = 0;
            if (SKU.GetType().BaseType.Name == typeof(PurchaseSKU).Name)
            {
                var PurchaseSKU = (PurchaseSKU)SKU;
                var SerialsLlist = PurchaseSKU.SerialsLlist.Where(y => !NoList.Contains(y.ID) && y.SerialsType == "PO" && !y.SerialsLlistC.Any());//有輸入直接讀輸入，沒輸入計算序號數
                count = SerialsLlist.Sum(y => y.SerialsQTY) ?? 0;
                //if (PurchaseSKU.QTYFulfilled.HasValue && PurchaseSKU.QTYFulfilled > 0)
                //{
                //    count = PurchaseSKU.QTYFulfilled.Value;
                //}
                //else
                //{
                //}
            }
            else if (SKU.GetType().BaseType.Name == typeof(TransferSKU).Name)
            {
                var TransferSKU = (TransferSKU)SKU;
                var SerialsLlist = TransferSKU.SerialsLlist.Where(y => !NoList.Contains(y.ID) && y.SerialsType == "TransferIn" && !y.SerialsLlistC.Any());//有輸入直接讀輸入，沒輸入計算序號數
                count = SerialsLlist.Sum(y => y.SerialsQTY) ?? 0;
                //WNoList.AddRange(SerialsLlist.Select(x => x.ID));
                //if (TransferSKU.QTYFulfilled.HasValue && TransferSKU.QTYFulfilled > 0)
                //{
                //    count = TransferSKU.QTYFulfilled.Value;
                //}
                //else
                //{
                //}
            }   
            return count;
        }
        /// <summary>
        /// 計算RMAIN的庫存
        /// </summary>
        /// <param name="SKU"></param>
        /// <param name="SKUNO"></param>
        /// <param name="NoList"></param>
        /// <param name="WarehouseID"></param>
        /// <returns></returns>
        private int GetRMAInQty(dynamic SKU, string SKUNO, bool IsNew, List<int> NoList = null, int? WarehouseID = null)
        {
            if (NoList == null)
            {
                NoList = new List<int>();
            }
            var count = 0;
            if (SKU.GetType().BaseType.Name == typeof(RMASKU).Name)
            {
                var RMASKU = (RMASKU)SKU;
                var SerialsLlist = RMASKU.RMASerialsLlist.Where(y => y.WarehouseID == WarehouseID && !NoList.Contains(y.ID) && y.SerialsType == "RMAIn" && !y.RMASerialsLlistC.Any());//有輸入直接讀輸入，沒輸入計算序號數
                if (IsNew)
                {
                    count = SerialsLlist.Where(x => x.NewSkuNo== SKUNO).Sum(y => y.SerialsQTY) ?? 0;
                }
                else
                {
                    count = SerialsLlist.Where(x => string.IsNullOrWhiteSpace(x.NewSkuNo)).Sum(y => y.SerialsQTY) ?? 0;
                }
               
                //WNoList.AddRange(SerialsLlist.Select(x => x.ID));
                //if (TransferSKU.QTYFulfilled.HasValue && TransferSKU.QTYFulfilled > 0)
                //{
                //    count = TransferSKU.QTYFulfilled.Value;
                //}
                //else
                //{
                //}
            }
            return count;
        }

        /// <summary>
        /// 取CM序號數量
        /// </summary>
        /// <param name="PurchaseSKU"></param>
        /// <returns></returns>
        private int GetCMQty(dynamic SKU, List<int> NoList = null)
        {
            if (NoList == null)
            {
                NoList = new List<int>();
            }
            var count = 0;
            if (SKU.GetType().BaseType.Name == typeof(PurchaseSKU).Name)
            {
                var PurchaseSKU = (PurchaseSKU)SKU;
                foreach (var Serialsitem in PurchaseSKU.SerialsLlist.Where(x => !NoList.Contains(x.ID) && x.SerialsLlistC.Any(y => y.SerialsType == "CM")))
                {
                    var SerialsLlist = Serialsitem.SerialsLlistC.Where(x => !NoList.Contains(x.ID) && x.SerialsType == "CM");
                    count += SerialsLlist.Sum(y => y.SerialsQTY).Value;
                    //WNoList.AddRange(SerialsLlist.Select(x => x.ID));
                }
            }
            else if (SKU.GetType().BaseType.Name == typeof(TransferSKU).Name)
            {
                var TransferSKU = (TransferSKU)SKU;
                foreach (var Serialsitem in TransferSKU.SerialsLlist.Where(x => !NoList.Contains(x.ID) && x.SerialsLlistC.Any(y => y.SerialsType == "CM")))
                {
                    var SerialsLlist = Serialsitem.SerialsLlistC.Where(x => !NoList.Contains(x.ID) && x.SerialsType == "CM");
                    count += SerialsLlist.Sum(y => y.SerialsQTY).Value;
                    //WNoList.AddRange(SerialsLlist.Select(x => x.ID));
                }
            }

            return count;
        }

        public ActionResult Statement(string SKU, int WarehouseID)
        {
            ViewBag.WarehouseID = WarehouseID;
            var PurchaseSKU = db.PurchaseSKU.Where(x => x.IsEnable && x.PurchaseOrder.IsEnable && x.SkuNo == SKU);
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
                        ISType = " Order Dispatch";
                        ID = SerialsLlistitem.OrderID;
                        Warehouse = SerialsLlistitem.PurchaseSKU.PurchaseOrder.WarehousePO.Name;
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

        public ActionResult Create()
        {
            return View();
        }
        public ActionResult Serials(string SKU, int WarehouseID)
        {
            ViewBag.WarehouseID = WarehouseID;
            var SKUName = db.SkuLang.Where(x => x.Sku == SKU && x.LangID == LangID).FirstOrDefault().Name;
            var RMASerialsLlist = db.RMASerialsLlist.Where(x => x.WarehouseID == WarehouseID && x.RMASKU.IsEnable && x.RMASKU.RMA.IsEnable && (x.RMASKU.SkuNo == SKU || x.NewSkuNo == SKU)).ToList();
            var PurchaseSKU = db.PurchaseSKU.Where(x => x.IsEnable && x.SkuNo == SKU);
            var CompanyName = PurchaseSKU.FirstOrDefault()?.PurchaseOrder.Company.Name;
            var InventorySerialsItem = new List<InventorySerialsItem>();
            foreach (var Skuitem in PurchaseSKU)
            {
                if (Skuitem.SerialsLlist != null && Skuitem.SerialsLlist.Any(x => x.SerialsType == "PO"))
                {
                    foreach (var SerialsNoitem in Skuitem.SerialsLlist.Where(x => x.SerialsType == "PO").Select(x => x.SerialsNo).Distinct())
                    {
                        var SerialsLlist = Skuitem.SerialsLlist.Where(x => x.SerialsNo == SerialsNoitem).ToList();
                        var Order = SerialsLlist.Where(x => x.OrderID.HasValue).FirstOrDefault()?.OrderID;
                        //var FRMA = RMASerialsLlist.Where(x => x.SerialsNo == SerialsNoitem).FirstOrDefault();
                        //var RMA = FRMA?.RMASKU.RMAID;
                        var PO = SerialsLlist.Where(x => x.PurchaseSKU.PurchaseOrderID.HasValue).FirstOrDefault()?.PurchaseSKU.PurchaseOrderID;
                        var CM = SerialsLlist.Where(x => x.PurchaseSKU.CreditMemoID.HasValue).FirstOrDefault()?.PurchaseSKU.CreditMemoID;
                        var WarehouseName = "";
                        var Location = "";
                        if (Skuitem.PurchaseOrder != null)
                        {
                            WarehouseName = Skuitem.PurchaseOrder.WarehousePO.Name;
                        }
                        else if (Skuitem.CreditMemo != null)
                        {
                            WarehouseName = Skuitem.CreditMemo.PurchaseOrder.WarehousePO.Name;
                        }
                        var Price = SerialsLlist.Where(x => x.PurchaseSKU.Price.HasValue).FirstOrDefault()?.PurchaseSKU.Price;
                        var CreateAt = SerialsLlist.OrderByDescending(x => x.CreateAt).FirstOrDefault()?.CreateAt;
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
                            //RMA = RMA,
                            Serial = SerialsNoitem,
                            Value = Price,
                            Warehouse = WarehouseName,
                            Date = CreateAt.Value.ToLocalTime(),
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
                        Date = item.CreateAt.ToLocalTime(),
                    });
                }
            }
            var InventorySerials = new InventorySerials
            {
                InventorySerialsItem = InventorySerialsItem,
                SKU = SKU,
                SKUName = SKUName,
                CompanyName= CompanyName
            };
            return View(InventorySerials);
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
            var SerialsLlist = db.SerialsLlist.Where(x => x.SerialsNo == ID).OrderByDescending(x => x.CreateAt);
            var RMASerialsLlist = db.RMASerialsLlist.Where(x => x.SerialsNo == ID).OrderByDescending(x => x.CreateAt);
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
                    Warehouse = item.PurchaseSKU.PurchaseOrder.WarehousePO.Name;
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
                    Warehouse = item.PurchaseSKU.CreditMemo.PurchaseOrder.WarehousePO.Name;
                }
                else if (item.SerialsType == "Order")
                {
                    ISType = "Order";
                    SID = item.OrderID;
                    Warehouse = item.PurchaseSKU.PurchaseOrder.WarehousePO.Name;
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
                SerialsLlistVMList.Add(new SerialsLlistVM { Date = CreateAt, ID = SID, ISType = ISType, QTY = QTY, UpdatedBy = UpdatedBy, Warehouse = Warehouse });
            }
            return View(SerialsLlistVMList.OrderByDescending(x=>x.Date));
        }
    }
}