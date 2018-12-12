using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PurchaseOrderSys.Models
{
    /// <summary>
    /// 登入用
    /// </summary>
    public class LoginViewModels
    {
        /// <summary>
        /// 帳號
        /// </summary>
        [Display(Name = "Username", ResourceType = typeof(App_GlobalResources.Resource))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
        public string Username { get; set; }
        /// <summary>
        /// 密碼
        /// </summary>
        [Display(Name = "Password", ResourceType = typeof(App_GlobalResources.Resource))]
        [MinLength(3, ErrorMessageResourceName = "MinLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
    public class MenuAuth
    {
        public int MenuID { get; set; }
        public List<string> AuthList { get; set; }
    }
    public class CMVM
    {
        /// <summary>
        /// Purchase Order ID
        /// </summary>        
        [Display(Name = "CMReplacement_PurchaseOrderID", ResourceType = typeof(App_GlobalResources.Resource))]
        public Nullable<int> PurchaseOrderID { get; set; }


        /// <summary>
        /// Company ID
        /// </summary>        
        [Display(Name = "CMReplacement_CompanyID", ResourceType = typeof(App_GlobalResources.Resource))]
        public Nullable<int> CompanyID { get; set; }


        /// <summary>
        /// Vendor ID
        /// </summary>        
        [Display(Name = "CMReplacement_VendorID", ResourceType = typeof(App_GlobalResources.Resource))]
        public Nullable<int> VendorID { get; set; }


        /// <summary>
        /// Invoice Date
        /// </summary>        
        [Display(Name = "CMReplacement_InvoiceDate", ResourceType = typeof(App_GlobalResources.Resource))]
        [UIHint("FDate")]
        public Nullable<System.DateTime> InvoiceDate { get; set; }


        /// <summary>
        /// Invoice No
        /// </summary>        
        [Display(Name = "CMReplacement_InvoiceNo", ResourceType = typeof(App_GlobalResources.Resource))]
        [MaxLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
        public string InvoiceNo { get; set; }


        /// <summary>
        /// CMStatus
        /// </summary>        
        [Display(Name = "CMReplacement_CMStatus", ResourceType = typeof(App_GlobalResources.Resource))]
        [MaxLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
        public string CMStatus { get; set; }


        /// <summary>
        /// CMType
        /// </summary>        
        [Display(Name = "CMReplacement_CMType", ResourceType = typeof(App_GlobalResources.Resource))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
        [MaxLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
        public string CMType { get; set; }


        /// <summary>
        /// Replacement PO
        /// </summary>        
        [Display(Name = "CMReplacement_ReplacementPO", ResourceType = typeof(App_GlobalResources.Resource))]
        [MaxLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
        public string ReplacementPO { get; set; }


        /// <summary>
        /// CMDate
        /// </summary>        
        [Display(Name = "CMReplacement_CMDate", ResourceType = typeof(App_GlobalResources.Resource))]
        [UIHint("FDate")]
        public Nullable<System.DateTime> CMDate { get; set; }


        /// <summary>
        /// Shipping Status
        /// </summary>        
        [Display(Name = "CMReplacement_ShippingStatus", ResourceType = typeof(App_GlobalResources.Resource))]
        [MaxLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
        public string ShippingStatus { get; set; }


        /// <summary>
        /// Shipped Date
        /// </summary>        
        [Display(Name = "CMReplacement_ShippedDate", ResourceType = typeof(App_GlobalResources.Resource))]
        [UIHint("FDate")]
        public Nullable<System.DateTime> ShippedDate { get; set; }


        /// <summary>
        /// Carrier
        /// </summary>        
        [Display(Name = "CMReplacement_Carrier", ResourceType = typeof(App_GlobalResources.Resource))]
        [MaxLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
        public string Carrier { get; set; }


        /// <summary>
        /// Tracking
        /// </summary>        
        [Display(Name = "CMReplacement_Tracking", ResourceType = typeof(App_GlobalResources.Resource))]
        [MaxLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
        public string Tracking { get; set; }


        /// <summary>
        /// Credit Status
        /// </summary>        
        [Display(Name = "CMReplacement_CreditStatus", ResourceType = typeof(App_GlobalResources.Resource))]
        [MaxLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
        public string CreditStatus { get; set; }


        /// <summary>
        /// Credit Date
        /// </summary>        
        [Display(Name = "CMReplacement_CreditDate", ResourceType = typeof(App_GlobalResources.Resource))]
        [UIHint("FDate")]
        public Nullable<System.DateTime> CreditDate { get; set; }


        /// <summary>
        /// Credit Amount
        /// </summary>        
        [Display(Name = "CMReplacement_CreditAmount", ResourceType = typeof(App_GlobalResources.Resource))]
        public Nullable<decimal> CreditAmount { get; set; }

        /// <summary>
        /// Warehouse
        /// </summary>        
        [Display(Name = "PurchaseOrder_Warehouse", ResourceType = typeof(App_GlobalResources.Resource))]
        [MaxLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
        public int? WarehouseID { get; set; }


        /// <summary>
        /// Currency
        /// </summary>        
        [Display(Name = "PurchaseOrder_Currency", ResourceType = typeof(App_GlobalResources.Resource))]
        [MaxLength(10, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
        public string Currency { get; set; }


        /// <summary>
        /// Tax
        /// </summary>        
        [Display(Name = "PurchaseOrder_Tax", ResourceType = typeof(App_GlobalResources.Resource))]
        public Nullable<decimal> Tax { get; set; }

        public IEnumerable< PurchaseSKU> PurchaseSKU { get; set; }
        public IEnumerable<CMCreditNote> CMCreditNote { get; set; }
    }
    public class PurchaseOrderPOVM
    {
        /// <summary>
        /// 採購單號
        /// </summary>
        [Display(Name = "PurchaseOrder_ID", ResourceType = typeof(App_GlobalResources.Resource))]
        [DataGrid(Align = "center", Widths = 150, Frozen = true)]
        public int? ID { get; set; }

        /// <summary>
        /// 採購單類別 
        /// Purchase Order: 採購單. 款項支出, 貨物支入. 
        /// Dropshp Order: 直發單.款項支出, 貨物支入, 再立即發貨.
        /// Credit Memo: 退貨單.款項支入, 貨物支出.
        /// </summary>        
        [Display(Name = "PurchaseOrder_POType", ResourceType = typeof(App_GlobalResources.Resource))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
        [MaxLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
        [DataGrid(Align = "center", Widths = 150, ColumnsType = "select", Formatter = true)]
        public string POType { get; set; }


        /// <summary>
        /// 供應商
        /// </summary>        
        [Display(Name = "PurchaseOrder_VendorID", ResourceType = typeof(App_GlobalResources.Resource))]
        [DataGrid(Align = "center", ColumnsType = "select", Formatter = true)]
        public int? VendorID { get; set; }

        /// <summary>
        /// 公司
        /// </summary>        
        [Display(Name = "PurchaseOrder_CompanyID", ResourceType = typeof(App_GlobalResources.Resource))]
        [DataGrid(Align = "center", ColumnsType = "select", Formatter = true)]
        public int? CompanyID { get; set; }
        /// <summary>
        /// 訂貨日
        /// </summary>        
        [Display(Name = "PurchaseOrder_PODate", ResourceType = typeof(App_GlobalResources.Resource))]
        [UIHint("DateTime")]
        [DataGrid(Align = "center", Widths = 150, ColumnsType = "input")]
        public Nullable<System.DateTime> PODate { get; set; }

        /// <summary>
        /// 採購數量
        /// </summary>
        [Display(Name = "QTY", ResourceType = typeof(App_GlobalResources.Resource))]
        [DataGrid(Align = "center", Widths = 150, ColumnsType = "input")]
        public int? QTY { get; set; }

        /// <summary>
        /// 採購總金額
        /// </summary>
        [Display(Name = "GrandTotal", ResourceType = typeof(App_GlobalResources.Resource))]
        [DataGrid(Align = "center", Widths = 150, ColumnsType = "input")]
        public decimal? GrandTotal { get; set; }

        /// <summary>
        /// 採購已付金額
        /// </summary>
        [Display(Name = "PaidAmount", ResourceType = typeof(App_GlobalResources.Resource))]
        [DataGrid(Align = "center", Widths = 150, ColumnsType = "input")]
        public decimal? PaidAmount { get; set; }

        /// <summary>
        /// 應付總額
        /// Blance  > 0, Payment Status = Partially Paid. 
        /// Balance > 0, Payment Status = Over Paid.
        /// Balance = 0, Payment Status = Completed.
        /// </summary>
        [Display(Name = "Balance", ResourceType = typeof(App_GlobalResources.Resource))]
        [DataGrid(Align = "center", Widths = 150, ColumnsType = "input")]
        public decimal? Balance { get; set; }

        /// <summary>
        /// 採購單狀態
        /// PO 狀態, 分別為 Opened, Pending, Completed. 剛開立的 PO 都預設為 Opened.
        /// </summary>
        [Display(Name = "Status", ResourceType = typeof(App_GlobalResources.Resource))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
        [DataGrid(Align = "center", ColumnsType = "select", Formatter = true)]
        public string POStatus { get; set; }

        /// <summary>
        /// Warehouse
        /// </summary>        
        [Display(Name = "PurchaseOrder_Warehouse", ResourceType = typeof(App_GlobalResources.Resource))]
        public int? WarehouseID { get; set; }


        /// <summary>
        /// 幣別
        /// </summary>        
        [Display(Name = "PurchaseOrder_Currency", ResourceType = typeof(App_GlobalResources.Resource))]
        [MaxLength(10, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
        public string Currency { get; set; }


        /// <summary>
        /// 稅率
        /// </summary>        
        [Display(Name = "PurchaseOrder_Tax", ResourceType = typeof(App_GlobalResources.Resource))]
        [MaxLength(10, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
        public decimal? Tax { get; set; }
    }
    public class PurchaseSKUVM
    {
        /// <summary>
        /// ID
        /// </summary>        
        [Display(Name = "PurchaseSKU_ID", ResourceType = typeof(App_GlobalResources.Resource))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
        [DataGrid(Align = "center", Widths = 150, Frozen = true)]
        public int ID { get; set; }


        /// <summary>
        /// Sku No
        /// </summary>        
        [Display(Name = "PurchaseSKU_SkuNo", ResourceType = typeof(App_GlobalResources.Resource))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
        [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
        [DataGrid(Align = "center", Widths = 150, ColumnsType = "input")]
        public string SkuNo { get; set; }


        /// <summary>
        /// Sku No
        /// </summary>        
        [Display(Name = "PurchaseSKU_VendorSKU", ResourceType = typeof(App_GlobalResources.Resource))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
        [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
        [DataGrid(Align = "center", Widths = 150, ColumnsType = "input")]
        public string VendorSKU { get; set; }

        /// <summary>
        /// Name
        /// </summary>        
        [Display(Name = "PurchaseSKU_Name", ResourceType = typeof(App_GlobalResources.Resource))]
        [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
        [DataGrid(Align = "center", Widths = 150, ColumnsType = "input")]
        public string Name { get; set; }

        /// <summary>
        /// 採購人員輸入的訂購數量
        /// </summary>        
        [Display(Name = "PurchaseSKU_QTYOrdered", ResourceType = typeof(App_GlobalResources.Resource))]
        [DataGrid(Align = "center", Widths = 150, ColumnsType = "input")]
        public Nullable<int> QTYOrdered { get; set; }


        /// <summary>
        /// 倉庫人員在 Receiving 視窗 輸入入庫的數量
        /// </summary>        
        [Display(Name = "PurchaseSKU_QTYFulfilled", ResourceType = typeof(App_GlobalResources.Resource))]
        [DataGrid(Align = "center", Widths = 150, ColumnsType = "input")]
        public Nullable<int> QTYFulfilled { get; set; }


        /// <summary>
        /// 單品進貨價
        /// </summary>        
        [Display(Name = "PurchaseSKU_Price", ResourceType = typeof(App_GlobalResources.Resource))]
        [DataGrid(Align = "center", Widths = 150, ColumnsType = "input")]
        public Nullable<decimal> Price { get; set; }


        /// <summary>
        /// 單品折讓金額
        /// </summary>        
        [Display(Name = "PurchaseSKU_Discount", ResourceType = typeof(App_GlobalResources.Resource))]
        [DataGrid(Align = "center", Widths = 150, ColumnsType = "input")]
        public Nullable<decimal> Discount { get; set; }


        /// <summary>
        /// 單品折讓後的出貨價
        /// </summary>        
        [Display(Name = "PurchaseSKU_DiscountedPrice", ResourceType = typeof(App_GlobalResources.Resource))]
        [DataGrid(Align = "center", Widths = 150, ColumnsType = "input")]
        public Nullable<decimal> DiscountedPrice { get; set; }
    }

    public class PostList
    {
        public int ID { get; set; }
        public string val { get; set; }
    }
    public class TranSKUVM
    {
        public int? ID { get; set; }
        public string ck { get; set; }
        public string sk { get; set; }
        /// <summary>
        /// 品號
        /// </summary>
        public string SKU { get; set; }
        /// <summary>
        /// 品名
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// 移倉數量
        /// </summary>
        public int? QTY { get; set; }
        public decimal? TotalReceive { get; set; }
        public string TWN { get; set; }
        public string Winit { get; set; }
    }
    public class CMSKUVM
    {
        public int? ID { get; set; }
        public string ck { get; set; }
        public string sk { get; set; }
        /// <summary>
        /// 品號
        /// </summary>
        public string SKU { get; set; }
        /// <summary>
        /// 供應商品號
        /// </summary>
        public string VendorSKU { get; set; }
        /// <summary>
        /// 品名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 訂購數量
        /// </summary>
        public int? QTYOrdered { get; set; }
        /// <summary>
        /// 退回數量
        /// </summary>
        public int? QTYReturned { get; set; }

        public int? CreditQTY { get; set; }
        /// <summary>
        /// 下個月折讓金額
        /// </summary>
        public decimal? Credit { get; set; }
        /// <summary>
        /// 應付總額
        /// </summary>
        public decimal? Subtotal { get; set; }

        public int? QTYReceived { get; set; }
        public int? Balance { get; set; }
        public string Model { get; set; }
    }
    public class PoSKUVM
    {
        public int? ID { get; set; }
        public string ck { get; set; }
        public string sk { get; set; }
        /// <summary>
        /// 品號
        /// </summary>
        public string SKU { get; set; }
        /// <summary>
        /// 供應商品號
        /// </summary>
        public string VendorSKU { get; set; }
        /// <summary>
        /// 品名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 訂購數量
        /// </summary>
        public int? QTYOrdered { get; set; }
        /// <summary>
        /// 廠商出貨數量
        /// </summary>
        public int? QTYFulfilled { get; set; }
        /// <summary>
        /// 進貨價
        /// </summary>
        public decimal? Price { get; set; }
        /// <summary>
        /// 折讓金額
        /// </summary>
        public decimal? Discount { get; set; }
        /// <summary>
        /// 下個月折讓金額
        /// </summary>
        public decimal? Credit { get; set; }
        /// <summary>
        /// 單品折讓後的出貨價
        /// </summary>
        public decimal? DiscountedPrice { get; set; }
        /// <summary>
        /// 應付總額
        /// </summary>
        public decimal? Subtotal { get; set; }

        public string UPCEAN { get; set; }

        public int? QTYReceived { get; set; }
        public int? QTYReturned { get; set; }
        public string Serial { get; set; }

        public int? SerialQTY { get; set; }
        public string Model { get; set; }
    }
    public class WarehouseVM
    {
        public int ID { get; set; }
        public string SKU { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// 顯示30天內的出貨數量
        /// </summary>
        public int Velocity { get; set; }
        /// <summary>
        /// 依照當下庫存數, 算出能維持的天數
        /// </summary>
        public int DaysOfSupply { get; set; }
        /// <summary>
        /// 可出貨的庫存
        /// </summary>
        public int Fulfillable { get; set; }
        /// <summary>
        /// 等待出貨的庫總量
        /// </summary>
        public decimal Awaiting { get; set; }
        /// <summary>
        /// 可上架的庫存總數
        /// </summary>
        public int Aggregate { get; set; }
        /// <summary>
        /// 不可銷售的庫總量
        /// </summary>
        public int Unfulfillable { get; set; }
    }
    public class PrepVM
    {
        public string SKU { get; set; }
        public string Name { get; set; }
        public int? QTY { get; set; }
        public List<string> SerialsLlist { get; set; }
    }
    public class PrepTable
    {
        public string SKU { get; set; }
        public string Name { get; set; }
        public string QTY { get; set; }
        public string Serial { get; set; }
        public string Label { get; set; }
        public string Download { get; set; }
    }

}