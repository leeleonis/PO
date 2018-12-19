
using PurchaseOrderSys.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace PurchaseOrderSys.Controllers
{
    public class DispatchWarehouseController : BaseController
    {
        // GET: DispatchWarehouse
        public ActionResult Index()
        {

            var Warehouse = db.Warehouse.Where(x => x.IsEnable);
            return View(Warehouse);
        }
        public ActionResult Create()
        {
          
            ViewBag.Warehouse3PList = new PurchaseOrderSys.Api.Winit_API().Warehouse3P();
            return View();
        }
        [HttpPost]
        public ActionResult Create(Warehouse Warehouse)
        {

            return View();
        }
        public ActionResult Edit(int id)
        {

            return View();
        }
        [HttpPost]
        public ActionResult Edit(Warehouse Warehouse)
        {

            return View();
        }
      
        public ActionResult Delete(int id)
        {

            return View();
        }
    }
}