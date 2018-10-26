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
        public ActionResult Index()
        {
            var WarehouseVM = db.PurchaseSKU.Where(x => x.IsEnable).Select(x => new WarehouseVM {
                ID = x.ID,
                Name = x.Name,
                SKU = x.SkuNo,
                Velocity = x.QTYOrdered.HasValue ? x.QTYOrdered.Value : x.SerialsLlist.Count()
            } );
            return View(WarehouseVM);
        }
        public ActionResult Create()
        {
            return View();
        }
    }
}