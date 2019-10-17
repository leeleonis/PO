using PurchaseOrderSys.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PurchaseOrderSys.Controllers
{
    public class InventoryController : BaseController
    {
        // GET: Inventory
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(int? WarehouseID, string SKU, string IsInventory)
        {
            var WarehouseVMList = new List<WarehouseVM>();

            int? FulfillableMin = null;
            int? FulfillableMax = null;
            if (IsInventory == "Y")
            {
                FulfillableMin = 1;
            }
            else if (IsInventory == "N")
            {
                FulfillableMax = 0;
            }
            if (WarehouseID.HasValue)
            {
                var Warehouse = db.Warehouse.Find(WarehouseID);
                WarehouseVMList.AddRange(nGetWarehouseVMList(Warehouse, SKU, FulfillableMin, FulfillableMax));
            }
            else
            {
                var WarehouseList = db.Warehouse.Where(x => x.Type != "Interim").ToList();
                foreach (var Warehouseitem in WarehouseList)
                {
                    WarehouseVMList.AddRange(nGetWarehouseVMList(Warehouseitem, SKU, FulfillableMin, FulfillableMax));
                }
            }
            return View(WarehouseVMList);
        }
    }
}