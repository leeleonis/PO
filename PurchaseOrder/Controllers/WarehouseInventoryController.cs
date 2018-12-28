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
                    Velocity = x.SerialsLlist.Where(y => y.SerialsType == -1 && !y.TransferSKUID.HasValue && x.CreateAt >= sdt && x.CreateAt <= sdt).Sum(y => y.SerialsType) ?? 0,
                    DaysOfSupply = 0,
                    Aggregate = 0,
                    Awaiting = 0,
                    Fulfillable = x.QTYOrdered.HasValue ? x.QTYOrdered.Value : x.SerialsLlist.Sum(y => y.SerialsType) ?? 0,
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
        public ActionResult Statement(int ID)
        {
            var PurchaseSKU = db.PurchaseSKU.Find(ID);
            return View(PurchaseSKU);
        }

        public ActionResult Create()
        {
            return View();
        }
    }
}