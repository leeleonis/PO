using PurchaseOrderSys.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PurchaseOrderSys.Controllers
{
    public class WarehouseController : BaseController
    {
        // GET: Warehouse
        public ActionResult Index()
        {
          
            return View();
        }
        public ActionResult Create()
        {
            return View();
        }
    }
}