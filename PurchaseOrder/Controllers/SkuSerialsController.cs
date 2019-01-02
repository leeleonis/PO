using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PurchaseOrderSys.Controllers
{
    public class SkuSerialsController : BaseController
    {
        // GET: SkuSerials
        public ActionResult Index()
        {
            var PurchaseSKU = db.PurchaseSKU.Find(12);
            return View(PurchaseSKU);
        }
    }
}