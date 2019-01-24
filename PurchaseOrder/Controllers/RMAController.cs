using PurchaseOrderSys.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PurchaseOrderSys.Controllers
{
    public class RMAController : BaseController
    {
        // GET: RMA
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create (RMA filter)
        {
            return RedirectToAction("Index");
        }
    }
}