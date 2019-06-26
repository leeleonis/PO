using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace PurchaseOrderSys.Models
{
    public static class EnumData
    {
        public enum CarrierType { Other, DHL, FedEx, UPS, USPS, Winit, IDS, Sendle };
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
        public enum SkuType { Single, Variation, Kit, Shadow }
        public enum SkuStatus { Inactive, Active }

        /// <summary>
        /// 幣別
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> CurrencyDDL()
        {
            var list = new Dictionary<string, string>();
            using (var db = new PurchaseOrderEntities())
            {
                list = db.Currency.ToDictionary(y => y.Code, y => y.Code);
            }
            return list;
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
                //{ "CreditMemo", App_GlobalResources.Resource.POTypeCreditMemoDDL }
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
        /// 付款狀態
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
        /// Transfer狀態
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> TransferStatusDDL()
        {
            return new Dictionary<string, string>()
            {
                { "Pending", App_GlobalResources.Resource.TransferStatusPendingDDL  },
                { "Requested", App_GlobalResources.Resource.TransferStatusRequestedDDL },
                { "Shipped", App_GlobalResources.Resource.TransferStatusShippedDDL  },
                { "Received", App_GlobalResources.Resource.TransferStatusReceivedDDL  },
                { "Completed", App_GlobalResources.Resource.TransferStatusCompletedDDL  }
            };
        }

        /// <summary>
        /// 倉庫列表
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> WarehouseDDL()
        {
            var list = new Dictionary<string, string>();
            using (var db = new PurchaseOrderEntities())
            {
                list = db.Warehouse.Where(x => x.Type != "Interim").ToDictionary(y => y.ID.ToString(), y => y.Name);
                //list = db.VendorLIst.ToDictionary(y => y.ID.ToString(), y => y.VendorNo + "_" + y.Name);
            }
            return list;
        }
        /// <summary>
        /// Interim倉庫列表
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> InterimDDL()
        {
            var list = new Dictionary<string, string>();
            using (var db = new PurchaseOrderEntities())
            {
                list = db.Warehouse.Where(x => x.Type == "Interim").ToDictionary(y => y.ID.ToString(), y => y.Name);
                //list = db.VendorLIst.ToDictionary(y => y.ID.ToString(), y => y.VendorNo + "_" + y.Name);
            }
            return list;
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
                list = db.VendorLIst.ToDictionary(y => y.ID.ToString(), y => y.Name);
                //list = db.VendorLIst.ToDictionary(y => y.ID.ToString(), y => y.VendorNo + "_" + y.Name);
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
                list = db.Company.ToDictionary(y => y.ID.ToString(), y => y.Name);
                //list = db.Company.ToDictionary(y => y.ID.ToString(), y => y.CompanyNo + "_" + y.Name);
            }
            return list;
        }

        /// <summary>
        /// 折讓類型
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> CMTypeDDL()
        {
            var list = new Dictionary<string, string> { { "CreditNote", "Credit Note" }, { "Replacement", "Replacement" } };
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
        /// </summary>@
        /// <returns></returns>
        public static Dictionary<string, string> WarehousePurviewDLL()
        {
            var list = new Dictionary<string, string> { { "Requested", "Requested" }, { "Shipped", "Shipped" }, { "Received", "Received" } };
            return list;
        }
        /// <summary>
        /// RMA狀態
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> RMAStatusDDL()
        {
            return new Dictionary<string, string>()
            {
                { "Open", "Open" },
                { "Authorized", "Authorized" },
                { "Received", "Received" },
                { "Fraud", "Fraud"},
                { "Closed", "Closed"}
            };
        }
        /// <summary>
        /// RMA狀態 內部的動作
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> RMAActionDDL()
        {
            return new Dictionary<string, string>()
            {
                { "None", "None" },
                { "Refund", "Refund" },
                { "Replacement", "Replacement" },
                { "Repair", "Repair"}
            };
        }
        public static Dictionary<int, string> eBayTitle()
        {
            return new Dictionary<int, string>()
            {
                { 2, "eBay Title (qualitydeals-usa)" },
                { 19, "eBay Title AU" },
                { 50, "eBay Title AU 2" },
                { 24, "eBay Title DE" },
                { 25, "eBay Title ES" },
                { 23, "eBay Title FR" },
                { 21, "eBay Title IT" },
                { 20, "eBay Title US" },
                { 22, "eBay Title US GBH" }
            };
        }
        public static Dictionary<string, string> CountryList()
        {

            var CultureListInf = CultureInfo.GetCultures(CultureTypes.SpecificCultures).Where(x => x.LCID != 4096).Select(x => new Country(x.LCID)).GroupBy(c => c.ID).Select(c => c.First()).OrderBy(x => x.Name);
            var CultureList = CultureListInf.ToDictionary(x => x.TwoCode, x => x.Name);
            return CultureList;
        }
        public static Dictionary<string, string> ChannelList()
        {
            var list = new Dictionary<string, string>();
            foreach (var item in Enum.GetValues(typeof(SCService.OrderSource1)))
            {
                list.Add(Convert.ToInt32(item).ToString(), item.ToString());
            }
            return list;
        }
        public static Dictionary<string, string> Renewitem()
        {
            var list = new Dictionary<string, string>();
            using (var db = new PurchaseOrderEntities())
            {
                list = db.Condition.ToDictionary(y => y.Suffix ?? "", y => y.ConditionLang.Where(x => x.LangID == "en-US").First()?.Name);
            }
            return list;
        }
        public static Dictionary<string, string> RMAStatusList()
        {
            return new Dictionary<string, string>()
            {
               { "Open", "Open" },
               { "Authorized", "Authorized" },
               { "Received", "Received" },
               { "Closed", "Closed" },
               { "Fraud", "Fraud" }
            };
        }
        public static Dictionary<string, string> RMAActionList()
        {
            return new Dictionary<string, string>()
            {
               { "None", "None" },
               { "Refund", "Refund" },
               { "Replacement", "Replacement" },
               { "Repair", "Repair" },
            };
        }
        public static Dictionary<int, string> RMAReasonList()
        {
            return new Dictionary<int, string>()
            {
                { 0, "Damaged" },
                { 13, "Fraud" },
                { 22, "No Longer Needed" },
                { 29,  "Package Never Arrive" },
                { 16, "Return to shipper" },
                { 1, "Defective" },
                { 70, "Website description is inaccurate" },
                //{ 4,  "Exchange" },//2019/04/15 SKYPE exchange 改成Package never
                { 19, "Warranty" },
                { 3, "Other" }
            };
        }

        public enum OrderType { Normal, Dropship, DirectLine, FBA };
        public static Dictionary<byte, string> OrderTypeList()
        {
            string[] Status = new string[] { "Normal", "Dropship", "Direct Line", "FBA" };

            return Enum.GetValues(typeof(OrderType)).Cast<OrderType>().ToDictionary(s => (byte)s, s => Status[(byte)s]);
        }

        public enum OrderAddressType { Sold, Shipped, Billed }

        public enum OrderChannel { eBay, Amazon, FBA, Website };
        public static Dictionary<byte, string> OrderChannelList(bool bySC = false)
        {
            byte[] SC_Chennel = new byte[] { 1, 4, 14, 6 };

            return Enum.GetValues(typeof(OrderChannel)).Cast<OrderChannel>().ToDictionary(s => bySC ? SC_Chennel[(byte)s] : (byte)s, s => s.ToString());
        }

        public enum OrderStatus { InProcess, OnHold, Completed, Cancel };
        public static Dictionary<byte, string> OrderStatusList(bool bySC = false)
        {
            byte[] SC_Status = new byte[] { 2, 5, 3, 0 };
            string[] Status = new string[] { "In Process", "On Hold", "Completed", "Cancel" };

            return Enum.GetValues(typeof(OrderStatus)).Cast<OrderStatus>().ToDictionary(s => bySC ? SC_Status[(byte)s] : (byte)s, s => Status[(byte)s]);
        }

        public enum OrderPaymentStatus { None, Partial, Full, OverPaid, Refunded };
        public static Dictionary<byte, string> OrderPaymentStatusList(bool bySC = false)
        {
            byte[] SC_Status = new byte[] { 0, 4, 2, 9, 5 };
            string[] Status = new string[] { "None", "Partial", "Full", "Over Paid", "Refunded" };

            return Enum.GetValues(typeof(OrderPaymentStatus)).Cast<OrderPaymentStatus>().ToDictionary(s => bySC ? SC_Status[(byte)s] : (byte)s, s => Status[(byte)s]);
        }

        public enum OrderFulfilledStatus { Unfulfilled, Partial, Fulfilled };
        public enum OrderShippingStatus { Unmanage, Awaiting, Dispatch, Fulfilled }

        public enum Export { 正式, 簡易 };
        public enum ExportMethod { 外貨復出口, 國貨出口 };
        public static Dictionary<byte, string> ExportMethodList()
        {
            string[] Method = new string[] { "G3-81 (外貨復出口)", "G5-02 (國貨出口)" };

            return Enum.GetValues(typeof(ExportMethod)).Cast<ExportMethod>().ToDictionary(s => (byte)s, s => Method[(byte)s]);
        }
    }
}