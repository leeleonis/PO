using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PurchaseOrderSys.Models
{
    public static class EnumData
    {
        public enum TimeZone { UTC, EST, TST, PST, GMT, AEST, JST };
        public static Dictionary<TimeZone, string> TimeZoneList()
        {
            return new Dictionary<TimeZone, string>() { { TimeZone.UTC, "UTC" },
                { TimeZone.EST, "Eastern Standard Time" }, { TimeZone.TST, "Taipei Standard Time" }, { TimeZone.PST, "Pacific Standard Time" },
                { TimeZone.GMT, "Greenwich Mean Time" }, { TimeZone.AEST, "AUS Eastern Standard Time" }, { TimeZone.JST, "Tokyo Standard Time" }
            };
        }
        public static Dictionary<string, string> SystemLangList()
        {
            return new Dictionary<string, string>()
            {
                { "zh-TW", "繁體中文" },
                { "en-US", "English" }
            };
        }
        public static Dictionary<string, string> DataLangList()
        {
            return new Dictionary<string, string>()
            {
                { "en-US", "English" },
                { "ja", "日本語" }
            };
        }

        public enum YesNo { No, Yes }

        public enum AttributeProperty { Normal, YesNo, Dimension, Resolution }
        public enum SkuType { Single, Variation, Kit }
        public enum SkuStatus { Inactive, Active }

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
        /// CM 狀態
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> CMStatusDDL()
        {
            var list = new Dictionary<string, string> {
                { "Opened", "Opened" },
                { "Pending", "Pending" },
                 { "Completed", "Completed" }
            };
            return list;
        }
        /// <summary>
        /// 退貨的運輸狀態
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> ShippingStatusDDL()
        {
            var list = new Dictionary<string, string> {
                { "Pending", "Pending" },
                { "PartiallyShipped", "Partially Shipped" },
                { "Completed", "Completed" },
                { "OverShipped", "Over Shipped" }
            };
            return list;
        }
        /// <summary>
        /// 折讓進度
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> CreditStatusDDL()
        {
            var list = new Dictionary<string, string> {
                { "Pending", "Pending" },
                { "PartiallyCredited", "Partially Credited" },
                { "Completed", "Completed" },
                { "OverCredited", "Over Credited" }
            };
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
                { "RMA", "RMA" },
                { "FBA", "FBA" },
                { "DropShip", "Drop Ship"},
                { "Interim", "Interim"},
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