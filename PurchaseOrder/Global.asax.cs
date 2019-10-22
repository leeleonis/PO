
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using PurchaseOrderSys.Models;

namespace PurchaseOrderSys
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            Application["RefreshInventory"] = "N";
            //System.Web.ModelBinding.ModelMetadataProviders.Current  = new CustomModelMetadataProvider();
        }
        protected void Application_BeginRequest(Object sender, EventArgs e)
        {
            //將 Cookies 的 MyLang 取出，主要是要指定語系
            var sLang = "zh-TW";
            HttpCookie Lang = Request.Cookies["Lang"];
            if (Lang != null)
            {
                sLang = Lang.Value;
            }
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(sLang);
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(sLang);
        }
    }
}
