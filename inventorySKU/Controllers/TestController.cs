using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.Entity;
using inventorySKU.Models;

namespace inventorySKU.Controllers
{
    public class TestController : BaseController
    {
        public void Test()
        {
        }

        public void StockKeepingUnit()
        {
            StockKeepingUnit SKU = new StockKeepingUnit("1001000BD");
            var skuList = SKU.CompareVariationSku();
        }
    }
}