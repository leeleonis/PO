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
            var WarehouseVM = db.PurchaseSKU.Where(x => x.IsEnable&&x.PurchaseOrder.Warehouse1.ID== ID).Select(x => new WarehouseVM {
                ID = x.ID,
                Name = x.Name,
                SKU = x.SkuNo,
                Velocity = x.QTYOrdered.HasValue ? x.QTYOrdered.Value : x.SerialsLlist.Count()
            } );
            return View(WarehouseVM);
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