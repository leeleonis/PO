using System.Web.Mvc;

namespace PurchaseOrderSys.Areas.SKUSystem
{
    public class SKUSystemAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "SKUSystem";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "SKUSystem_default",
                "SKUSystem/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}