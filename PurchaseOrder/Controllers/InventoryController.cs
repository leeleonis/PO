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
        public ActionResult Index(Warehouse Warehouse, string Product, int? FulfillableMin, int? FulfillableMax)
        {
            var WarehouseVMList = GetWarehouseVMList(Warehouse, Product, FulfillableMin, FulfillableMax);
            return View(WarehouseVMList);
        }
    }
}