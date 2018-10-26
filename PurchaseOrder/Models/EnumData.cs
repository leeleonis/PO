using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PurchaseOrderSys.Models
{
    public static class EnumData
    {
        /// <summary>
        /// 幣別
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> CurrencyDDL()
        {
            return new Dictionary<string, string>()
            {
                { "TWD", App_GlobalResources.Resource.CurrencyTWD },
                { "AUD", App_GlobalResources.Resource.CurrencyAUD },
                { "USD", App_GlobalResources.Resource.CurrencyUSD }
            };
        }

        /// <summary>
        /// 採購單的種類
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> POTypeDDL()
        {
            return new Dictionary<string, string>()
            {
                { "PurchaseOrder", App_GlobalResources.Resource.POTypePurchaseOrderDDL },
                { "DropshpOrder", App_GlobalResources.Resource.POTypeDropshpOrderDDL },
                { "CreditMemo", App_GlobalResources.Resource.POTypeCreditMemoDDL }
            };
        }
        /// <summary>
        /// 採購單的狀態
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> POStatusDDL()
        {
            return new Dictionary<string, string>()
            {
                { "Opened", App_GlobalResources.Resource.POStatusOpenedDDL },
                { "Pending", App_GlobalResources.Resource.POStatusPendingDDL },
                { "Completed", App_GlobalResources.Resource.POStatusCompletedDDL }
            };
        }
        /// <summary>
        /// 收貨狀態
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> ReceiveStatusDDL()
        {
            return new Dictionary<string, string>()
            {
                { "Pending", App_GlobalResources.Resource. ReceiveStatusPendingDDL },
                { "PartiallyReceived", App_GlobalResources.Resource. ReceiveStatusPartiallyReceivedDDL },
                { "Completed", App_GlobalResources.Resource. ReceiveStatusCompletedDDL },
                { "OverReceived", App_GlobalResources.Resource. ReceiveStatusOverReceivedDDL }
            };
        }
        /// <summary>
        /// 付款狀況
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> PaymentStatusDDL()
        {
            return new Dictionary<string, string>()
            {
                { "Pending", App_GlobalResources.Resource.PaymentStatusPendingDDL  },
                { "PartiallyPaid", App_GlobalResources.Resource. PaymentStatusPartiallyPaidDDL },
                { "Completed", App_GlobalResources.Resource.PaymentStatusCompletedDDL  },
                { "OverPaid", App_GlobalResources.Resource.PaymentStatusOverPaidDDL  }
            };
        }
        /// <summary>
        /// 供應商列表
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> VendorDDL()
        {
            var list = new Dictionary<string, string>();
            using (var db = new PurchaseOrderEntities())
            {
                list = db.VendorLIst.ToDictionary(y => y.ID.ToString(), y => y.VendorNo + "_" + y.Name);
            }
            return list;
        }
        /// <summary>
        /// 採購公司列表
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> CompanyDDL()
        {
           var list = new Dictionary<string, string>();
            using (var db = new PurchaseOrderEntities())
            {
                list = db.Company.ToDictionary(y => y.ID.ToString(), y => y.CompanyNo + "_" + y.Name);
            }
            return list;
        }

        /// <summary>
        /// 折讓類型
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> CMTypeDDL()
        {
            var list = new Dictionary<string, string>{{ "CreditNote", "Credit Note" },{ "Replacement", "Replacement" } };
            return list;
        }

        /// <summary>
        /// 倉庫類型
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> warehouseTypeDDL()
        {
            return new Dictionary<string, string>()
            {
                { "Normal", "Normal" },
                { "DropShip", "DropShip"},
                { "FBA", "FBA" },
                { "Winit", "Winit" }
            };
        }
        /// <summary>
        /// 倉庫權限
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> WarehousePurviewDLL()
        {
            var list = new Dictionary<string, string> { { "Requested", "Requested" }, { "Shipped", "Shipped" }, { "Received", "Received" } };
            return list;
        }
    }
}