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
        public ActionResult Index(int ID)
        {
            var PurchaseSKU = db.PurchaseSKU.Find(ID);
            return View(PurchaseSKU);
        }

        public ActionResult GetHistory(string ID)
        {
            var SerialsLlist = db.SerialsLlist.Where(x => x.SerialsNo == ID).OrderByDescending(x => x.CreateAt);
            return View(SerialsLlist);
        }
    }
}