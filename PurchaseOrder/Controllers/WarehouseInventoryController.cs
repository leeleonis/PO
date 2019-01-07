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
            var edt = DateTime.Today;
            var sdt = edt.AddDays(-30);
            var PurchaseSKU = db.PurchaseSKU.Where(x => x.IsEnable && x.PurchaseOrder.Warehouse1.ID == ID);
            var WarehouseInventoryVM = new WarehouseInventoryVM();
            if (PurchaseSKU.Any())
            {
                var WarehouseVM = PurchaseSKU.Select(x => new WarehouseVM
                {
                    ID = x.ID,
                    Name = x.Name,
                    SKU = x.SkuNo,
                    Velocity = x.SerialsLlist.Where(y => y.SerialsQTY == -1 && !y.TransferSKUID.HasValue && x.CreateAt >= sdt && x.CreateAt <= sdt).Sum(y => y.SerialsQTY) ?? 0,
                    DaysOfSupply = 0,
                    Aggregate = 0,
                    Awaiting = 0,
                    Fulfillable = x.QTYOrdered.HasValue ? x.QTYOrdered.Value : x.SerialsLlist.Sum(y => y.SerialsQTY) ?? 0,
                    Unfulfillable = 0
                }).ToList();
                foreach (var item in WarehouseVM)
                {
                    item.Aggregate = item.Fulfillable - item.Awaiting;//Aggregate = Fulfillable - Awaiting dispatch
                    item.DaysOfSupply = item.Aggregate != 0 && item.Velocity != 0 ? item.Aggregate / item.Velocity / 30 : 0;  //Days of supply 算法: Aggregate / Velocity (30 days) / 30
                                                                                                                              //item.Fulfillable = item.Awaiting + item.Aggregate; //Fulfillable = Awaiting dispatch + Aggregate 2018/12/28 拿掉公式
                }
                WarehouseInventoryVM.Company = PurchaseSKU.FirstOrDefault().PurchaseOrder.Company.Name;
                WarehouseInventoryVM.WarehouseType = PurchaseSKU.FirstOrDefault().PurchaseOrder.Warehouse1.Type;
                WarehouseInventoryVM.Fulfillable = "";
                WarehouseInventoryVM.Location = "";
                WarehouseInventoryVM.Countries = "";
                WarehouseInventoryVM.Marketplace = "";
                WarehouseInventoryVM.WarehouseVM = WarehouseVM;
            }
            return View(WarehouseInventoryVM);
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
            var PurchaseSKU = db.PurchaseSKU.Where(x => x.SkuNo == SKU);
            //var SCInventoryService = new Api.SC_API().SCInventoryService(PurchaseSKU.SkuNo);
            //foreach (var item in PurchaseSKU.PurchaseOrder.Warehouse1)
            //{

            //}

            return View(PurchaseSKU);
        }
        public ActionResult Purchasing(string SKU)
        {
            var PurchaseSKU = db.PurchaseSKU.Where(x => x.SkuNo == SKU);
            var SkuPurchasingVM = new SkuPurchasingVM
            {

                SKU = SKU
            };
            if (PurchaseSKU.Any())
            {
                SkuPurchasingVM.Company = PurchaseSKU.FirstOrDefault()?.PurchaseOrder.Company.Name;
                SkuPurchasingVM.SKUName = PurchaseSKU.FirstOrDefault()?.Name;
                SkuPurchasingVM.Aggregate = 0;
                SkuPurchasingVM.Fulfillable =0;
                SkuPurchasingVM.AwaitingDispatch = 0;
                SkuPurchasingVM.UnfulfillableRMA = 0;
                SkuPurchasingVM.UnfulfillableTransit = 0;
                SkuPurchasingVM.TotalInventory = 0;
                foreach (var PurchaseSKUitem in PurchaseSKU)
                {
                    if (PurchaseSKUitem.SerialsLlist.Any())
                    {
                        SkuPurchasingVM.Aggregate += PurchaseSKUitem.SerialsLlist.Where(y => !y.SerialsLlistC.Any() && !y.TransferSKUID.HasValue && !y.RMAID.HasValue && y.SerialsQTY > 0).Sum(y => y.SerialsQTY).Value;
                        SkuPurchasingVM.Fulfillable += PurchaseSKUitem.SerialsLlist.Where(y => !y.SerialsLlistC.Any() && !y.TransferSKUID.HasValue && !y.RMAID.HasValue && y.SerialsQTY > 0).Sum(y => y.SerialsQTY).Value;
                        SkuPurchasingVM.UnfulfillableRMA += PurchaseSKUitem.SerialsLlist.Where(y => !y.SerialsLlistC.Any() && !y.TransferSKUID.HasValue && y.RMAID.HasValue).Sum(y => y.SerialsQTY).Value;
                        SkuPurchasingVM.UnfulfillableTransit += PurchaseSKUitem.SerialsLlist.Where(y => !y.SerialsLlistC.Any() && y.TransferSKUID.HasValue && !y.RMAID.HasValue).Sum(y => y.SerialsQTY).Value;
                        SkuPurchasingVM.TotalInventory += PurchaseSKUitem.SerialsLlist.Where(y => !y.SerialsLlistC.Any() && !y.TransferSKUID.HasValue && !y.RMAID.HasValue).Sum(y => y.SerialsQTY).Value;
                    }

                }
                SkuPurchasingVM.BackOrdered = PurchaseSKU.Sum(x => x.QTYOrdered).Value;
                SkuPurchasingVM.QTYperbox = 1;
                SkuPurchasingVM.QTYpercase = 1;
                SkuPurchasingVM.Latest = PurchaseSKU.OrderByDescending(x => x.CreateAt).FirstOrDefault().Price.Value;
                SkuPurchasingVM.Average = PurchaseSKU.Average(x => x.Price).Value;
                SkuPurchasingVM.Lowest = PurchaseSKU.OrderBy(x => x.Price).FirstOrDefault().Price.Value;
                SkuPurchasingVM.Highest = PurchaseSKU.OrderByDescending(x => x.Price).FirstOrDefault().Price.Value;
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