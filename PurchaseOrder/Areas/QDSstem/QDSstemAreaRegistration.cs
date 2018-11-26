using System.Web.Mvc;

namespace PurchaseOrderSys.Areas.QDSstem
{
    public class QDSstemAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "QDSstem";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "QDSstem_default",
                "QDSstem/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}