using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
    {   /// <summary>
        /// ID
        /// </summary>        
        [Display(Name = "CreditMemo_ID", ResourceType = typeof(App_GlobalResources.Resource))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
        public int ID { get; set; }

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

        public IEnumerable<PurchaseSKU> PurchaseSKU { get; set; }
        public IEnumerable<PurchaseNote> PurchaseNote { get; set; }
    }

    public class CreditMemoVM
    {
        /// <summary>
        /// 公司
        /// </summary>        
        [Display(Name = "CreditMemo_CompanyID", ResourceType = typeof(App_GlobalResources.Resource))]
        [DataGrid(Align = "center", Widths = 150, ColumnsType = "select", Formatter = true)]
        public int? CompanyID { get; set; }

        /// <summary>
        /// 採購單號
        /// </summary>
        [Display(Name = "CreditMemo_ID", ResourceType = typeof(App_GlobalResources.Resource))]
        [DataGrid(Align = "center", Widths = 150, Frozen = true)]
        public int? ID { get; set; }


        /// <summary>
        /// 採購單狀態
        /// CM 狀態, 分別為 Opened, Pending, Completed. 剛開立的 CM 都預設為 Opened.
        /// </summary>
        [Display(Name = "Status", ResourceType = typeof(App_GlobalResources.Resource))]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
        [DataGrid(Align = "center", ColumnsType = "select", Formatter = true)]
        public string CMStatus { get; set; }



        /// <summary>
        /// 採購單類別 
        /// </summary>        
        [Display(Name = "CreditMemo_CMType", ResourceType = typeof(App_GlobalResources.Resource))]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
        [MaxLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
        [DataGrid(Align = "center", Widths = 150, ColumnsType = "select", Formatter = true)]
        public string CMType { get; set; }


        public string Creater { get; set; }

        public string CreditStatus { get; set; }
        public string ReturnStatus { get; set; }
        public string Tracking { get; set; }
        public string SKU { get; set; }
        public string Serial { get; set; }


        /// <summary>
        /// 供應商
        /// </summary>        
        [Display(Name = "CreditMemo_VendorID", ResourceType = typeof(App_GlobalResources.Resource))]
        [DataGrid(Align = "center", Widths = 150, ColumnsType = "select", Formatter = true)]
        public int? VendorID { get; set; }

        /// <summary>
        /// 訂貨日
        /// </summary>        
        [Display(Name = "CreditMemo_CMDate", ResourceType = typeof(App_GlobalResources.Resource))]
        [UIHint("FDate")]
        [DataGrid(Align = "center", Widths = 150, ColumnsType = "input")]
        public Nullable<System.DateTime> CMDate { get; set; }

        /// <summary>
        /// Invoice Date
        /// </summary>        
        [Display(Name = "CreditMemo_InvoiceDate", ResourceType = typeof(App_GlobalResources.Resource))]
        [UIHint("FDate")]
        public Nullable<System.DateTime> InvoiceDate { get; set; }

        /// <summary>
        /// Invoice No
        /// </summary>        
        [Display(Name = "CreditMemo_InvoiceNo", ResourceType = typeof(App_GlobalResources.Resource))]
        [MaxLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
        public string InvoiceNo { get; set; }

        [UIHint("FDate")]
        public Nullable<System.DateTime> CreditDate { get; set; }

        [UIHint("FDate")]
        public Nullable<System.DateTime> ReturnDate { get; set; }
        public string Brand { get; set; }
        public string Category { get; set; }

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
        /// 應付總額
        /// </summary>
        [Display(Name = "Balance", ResourceType = typeof(App_GlobalResources.Resource))]
        [DataGrid(Align = "center", Widths = 150, ColumnsType = "input")]
        public decimal? Balance { get; set; }

        /// <summary>
        /// 採購單號
        /// </summary>
        [Display(Name = "PurchaseOrder_ID", ResourceType = typeof(App_GlobalResources.Resource))]
        [DataGrid(Align = "center", Widths = 150, ColumnsType = "input")]
        public string POID { get; set; }
    }

    public class CreditMemoVMQ : CreditMemoVM
    {
        [UIHint("FDate")]
        public Nullable<System.DateTime> CMDateS { get; set; }
        [UIHint("FDate")]
        public Nullable<System.DateTime> CMDateE { get; set; }
        [UIHint("FDate")]
        public Nullable<System.DateTime> InvoiceDateS { get; set; }
        [UIHint("FDate")]
        public Nullable<System.DateTime> InvoiceDateE { get; set; }
        [UIHint("FDate")]
        public Nullable<System.DateTime> CreditDateS { get; set; }
        [UIHint("FDate")]
        public Nullable<System.DateTime> CreditDateE { get; set; }
        [UIHint("FDate")]
        public Nullable<System.DateTime> ReturnDateS { get; set; }
        [UIHint("FDate")]
        public Nullable<System.DateTime> ReturnDateE { get; set; }
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
        /// Description
        /// </summary>
        [Display(Name = "PurchaseOrder_Description", ResourceType = typeof(App_GlobalResources.Resource))]
        [MaxLength(100, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
        [DataGrid(Align = "center", Widths = 150, ColumnsType = "input")]
        public string Description { get; set; }


        /// <summary>
        /// 採購單類別 
        /// Purchase Order: 採購單. 款項支出, 貨物支入. 
        /// Dropshp Order: 直發單.款項支出, 貨物支入, 再立即發貨.
        /// Credit Memo: 退貨單.款項支入, 貨物支出.
        /// </summary>        
        [Display(Name = "PurchaseOrder_POType", ResourceType = typeof(App_GlobalResources.Resource))]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
        [MaxLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
        [DataGrid(Align = "center", Widths = 150, ColumnsType = "select", Formatter = true)]
        public string POType { get; set; }


        /// <summary>
        /// 供應商
        /// </summary>        
        [Display(Name = "PurchaseOrder_VendorID", ResourceType = typeof(App_GlobalResources.Resource))]
        [DataGrid(Align = "center", Widths = 150, ColumnsType = "select", Formatter = true)]
        public int? VendorID { get; set; }

        /// <summary>
        /// 公司
        /// </summary>        
        [Display(Name = "PurchaseOrder_CompanyID", ResourceType = typeof(App_GlobalResources.Resource))]
        [DataGrid(Align = "center", Widths = 150, ColumnsType = "select", Formatter = true)]
        public int? CompanyID { get; set; }
        /// <summary>
        /// 訂貨日
        /// </summary>        
        [Display(Name = "PurchaseOrder_PODate", ResourceType = typeof(App_GlobalResources.Resource))]
        [UIHint("FDate")]
        [DataGrid(Align = "center", Widths = 150, ColumnsType = "input")]
        public Nullable<System.DateTime> PODate { get; set; }

        /// <summary>
        /// Invoice Date
        /// </summary>        
        [Display(Name = "PurchaseOrder_InvoiceDate", ResourceType = typeof(App_GlobalResources.Resource))]
        [UIHint("FDate")]
        public Nullable<System.DateTime> InvoiceDate { get; set; }

        /// <summary>
        /// Invoice No
        /// </summary>        
        [Display(Name = "PurchaseOrder_InvoiceNo", ResourceType = typeof(App_GlobalResources.Resource))]
        [MaxLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
        public string InvoiceNo { get; set; }


        /// <summary>
        /// Received Date
        /// </summary>        
        [Display(Name = "PurchaseOrder_ReceivedDate", ResourceType = typeof(App_GlobalResources.Resource))]
        [UIHint("FDate")]
        public Nullable<System.DateTime> ReceivedDate { get; set; }

        /// <summary>
        /// Receive Status
        /// </summary>        
        [Display(Name = "PurchaseOrder_ReceiveStatus", ResourceType = typeof(App_GlobalResources.Resource))]
        [MaxLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
        public string ReceiveStatus { get; set; }

        /// <summary>
        /// Payment Date
        /// </summary>        
        [Display(Name = "PurchaseOrder_PaymentDate", ResourceType = typeof(App_GlobalResources.Resource))]
        [UIHint("FDate")]
        public Nullable<System.DateTime> PaymentDate { get; set; }

        /// <summary>
        /// Payment Status
        /// </summary>        
        [Display(Name = "PurchaseOrder_PaymentStatus", ResourceType = typeof(App_GlobalResources.Resource))]
        [MaxLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
        public string PaymentStatus { get; set; }

        /// <summary>
        /// 採購數量
        /// </summary>
        [Display(Name = "QTY", ResourceType = typeof(App_GlobalResources.Resource))]
        [DataGrid(Align = "center", Widths = 150, ColumnsType = "input")]
        public int? QTY { get; set; }

        /// <summary>
        /// 入庫數量
        /// </summary>
        [Display(Name = "PurchaseSKU_QTYReceived", ResourceType = typeof(App_GlobalResources.Resource))]
        [DataGrid(Align = "center", Widths = 150, ColumnsType = "input")]
        public int? QTYReceived { get; set; }

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
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
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

        public string Creater { get; set; }
        public string Brand { get; set; }
        public string Tracking { get; set; }
        public string Category { get; set; }
        public string SKU { get; set; }
        public string Serial { get; set; }

        /// <summary>
        ///CM單號
        /// </summary>
        [Display(Name = "CreditMemo_ID", ResourceType = typeof(App_GlobalResources.Resource))]
        [DataGrid(Align = "left", ColumnsType = "input")]
        public string CMID { get; set; }
    }

    public class PurchaseOrderPOVMQ : PurchaseOrderPOVM
    {
        [UIHint("FDate")]
        public Nullable<System.DateTime> PODateS { get; set; }
        [UIHint("FDate")]
        public Nullable<System.DateTime> PODateE { get; set; }
        [UIHint("FDate")]
        public Nullable<System.DateTime> InvoiceDateS { get; set; }
        [UIHint("FDate")]
        public Nullable<System.DateTime> InvoiceDateE { get; set; }
        [UIHint("FDate")]
        public Nullable<System.DateTime> PaymentDateS { get; set; }
        [UIHint("FDate")]
        public Nullable<System.DateTime> PaymentDateE { get; set; }
        [UIHint("FDate")]
        public Nullable<System.DateTime> ReceivedDateS { get; set; }
        [UIHint("FDate")]
        public Nullable<System.DateTime> ReceivedDateE { get; set; }
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
        public int ck { get; set; }
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
        public string Serial { get; set; }
        public string Label { get; set; }
        public string TWN { get; set; }
        public string Winit { get; set; }
        public string Model { get; set; }
        public int? Prep { get; set; }
        public decimal? Price { get; set; }
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
        public string UPCEAN { get; set; }
        public string Serial { get; set; }
        public int? SerialQTY { get; set; }
        /// <summary>
        /// 開啟序號管理
        /// </summary>
        public bool SerialTracking { get; set; }
        /// <summary>
        /// SKU圖檔路徑
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 規格
        /// </summary>
        public string Size { get; set; }
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
        /// <summary>
        /// 開啟序號管理
        /// </summary>
        public bool SerialTracking { get; set; }
        /// <summary>
        /// SKU圖檔路徑
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 規格
        /// </summary>
        public string Size { get; set; }
    }
    public class WarehouseInventoryVM
    {
        public string Company { get; set; }
        public string WarehouseType { get; set; }
        public string Fulfillable { get; set; }
        public string Location { get; set; }
        public string Countries { get; set; }
        public string Marketplace { get; set; }
        public IEnumerable<WarehouseVM> WarehouseVM { get; set; }
    }
    public class SkuPurchasingVM
    {

        public string Company { get; set; }
        public string SKU { get; set; }
        public string SKUName { get; set; }

        /// <summary>
        /// 查詢所選擇的倉庫庫存相關資料
        /// </summary>
        public int? Inventory { get; set; }
        /// <summary>
        /// 既有的庫存並且屬於可出貨的
        /// </summary>
        public int Fulfillable { get; set; }
        /// <summary>
        /// 等待出貨的庫存數量
        /// </summary>
        public int AwaitingDispatch { get; set; }
        /// <summary>
        /// 可上架的庫存總數
        /// </summary>
        public int Aggregate { get; set; }
        /// <summary>
        /// 不可銷售的庫存總數,只限於 RMA 倉庫裡的庫存數量
        /// </summary>
        public int UnfulfillableRMA { get; set; }
        /// <summary>
        /// 不可銷售的庫存總數,只限於 Transit 倉庫裡的庫存數量
        /// </summary>
        public int UnfulfillableTransit { get; set; }
        /// <summary>
        /// 顯示庫存的總數量 (不包含 Back ordered)
        /// </summary>
        public int TotalInventory { get; set; }
        /// <summary>
        /// 以下採購單 (PO) 的數量,不列入庫存數, 直到收貨完成為止
        /// </summary>
        public int BackOrdered { get; set; }
        /// <summary>
        /// 顯示該品號的所代表的單品數量. 預設為 1.
        /// </summary>
        public int QTYpercase { get; set; }
        /// <summary>
        /// 顯示該產品進貨時一箱的數量. 預設為 1.
        /// </summary>
        public int QTYperbox { get; set; }
        /// <summary>
        /// 顯示最近期的進貨成本
        /// </summary>
        public decimal Latest { get; set; }
        /// <summary>
        /// 計算平均現貨的成本
        /// </summary>
        public decimal Average { get; set; }
        /// <summary>
        /// 顯示歷史紀錄裡的最低進貨成本
        /// </summary>
        public decimal Lowest { get; set; }
        /// <summary>
        /// 顯示歷史紀錄裡的最高進貨成本
        /// </summary>
        public decimal Highest { get; set; }

        /// <summary>
        /// 依照所輸入的天數而計算這時間內的銷售速度
        /// </summary>
        public int Velocity { get; set; }
        /// <summary>
        /// 依照所輸入的天數, 算出平均每日已寄出的數量
        /// </summary>
        public int AveragefulfilledQTY { get; set; }
        /// <summary>
        /// 依照所輸入的天數, 算出平均每日可出貨的庫存數量
        /// </summary>
        public int AveragefulfillableQTY { get; set; }
        /// <summary>
        /// 依照所輸入的天數, 算出每日平均採購的數量
        /// </summary>
        public int AveragePOQTY { get; set; }
        /// <summary>
        /// 依照所輸入的天數內, 算出總出貨的數量
        /// </summary>
        public int TotalFulfilled { get; set; }
        /// <summary>
        /// 依照所輸入的天數內, 算出總採購的數量
        /// </summary>
        public int TotalPO { get; set; }


        /// <summary>
        /// 所有/實體庫存
        /// </summary>
        public int Available { get; set; }
        /// <summary>
        /// 已退貨數
        /// </summary>
        public int CMQTY { get; set; }
        /// <summary>
        /// 移庫入庫數
        /// </summary>
        public int TransferInQTY { get; set; }
        /// <summary>
        /// 移庫出庫數
        /// </summary>
        public int TransferOutQTY { get; set; }
        /// <summary>
        /// 已出貨數
        /// </summary>
        public int OrderQTY { get; set; }
        /// <summary>
        /// 目前PO庫存數
        /// </summary>
        public int POQTY { get; set; }
        /// <summary>
        /// RMA數
        /// </summary>
        public int RMAQTY { get; set; }

        /// <summary>
        /// 等待出貨的庫存數量
        /// </summary>
        public int Awaiting { get; set; }
    }

    public class SkuInventoryVM
    {
        /// <summary>
        /// 倉庫的 ID
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 倉庫的 SCID
        /// </summary>
        public string SCID { get; set; }
        /// <summary>
        /// 倉庫名稱
        /// </summary>
        public string Warehouse { get; set; }
        /// <summary>
        /// 倉庫的性質 (Normal, FBA, DropShip, Interim)
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 所有/實體庫存
        /// </summary>
        public int Available { get; set; }
        /// <summary>
        /// 等待出貨的庫存數量
        /// </summary>
        public int Awaiting { get; set; }
        /// <summary>
        /// 可上架的庫存總數
        /// </summary>
        public int Aggregate { get; set; }
        /// <summary>
        /// 不可銷售的庫存總數
        /// </summary>
        public int Unfulfillable { get; set; }
        /// <summary>
        /// 各個倉庫的庫存數
        /// </summary>
        public int Total { get; set; }
        /// <summary>
        /// 該序號/單品的價值. 算法為 SKU tab (Purchasing) 裡的 PO cost - CM Cost = Value.
        /// </summary>
        public decimal Value { get; set; }

        /// <summary>
        /// 以下採購單 (PO) 的數量
        /// </summary>
        public int BackOrdered { get; set; }
        /// <summary>
        /// 已退貨數
        /// </summary>
        public int CMQTY { get; set; }
        /// <summary>
        /// 移庫入庫數
        /// </summary>
        public int TransferInQTY { get; set; }
        /// <summary>
        /// 移庫出庫數
        /// </summary>
        public int TransferOutQTY { get; set; }
        /// <summary>
        /// 待移倉數量
        /// </summary>
        public int TransferAwaiting { get; set; }
        /// <summary>
        /// 移庫已收貨
        /// </summary>
        public int TransferOutCloseQTY { get; set; }
        /// <summary>
        /// 已出貨數
        /// </summary>
        public int OrderQTY { get; set; }
        /// <summary>
        /// 目前PO庫存數
        /// </summary>
        public int POQTY { get; set; }
        /// <summary>
        /// RMA數
        /// </summary>
        public int RMAQTY { get; set; }
    }

    public class WarehouseAllVM
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        /// <summary>
        /// 可出貨的庫存
        /// </summary>
        public int? Fulfillable { get; set; }
        /// <summary>
        /// 等待出貨的庫總量
        /// </summary>
        public int? Awaiting { get; set; }
        /// <summary>
        /// 可上架的庫存總數
        /// </summary>
        public int? Aggregate { get; set; }
        /// <summary>
        /// 移庫出庫數
        /// </summary>
        public int? TransferOutQTY { get; set; }
    }

    public class WarehouseVM
    {
        public int ID { get; set; }
        public string SKU { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// 訂貨數
        /// </summary>
        public int BackOrdered { get; set; }
        /// <summary>
        /// 顯示30天內的出貨數量
        /// </summary>
        public int Velocity { get; set; }
        /// <summary>
        /// 目前PO庫存數
        /// </summary>
        public int POQTY { get; set; }
        /// <summary>
        /// 已出貨數
        /// </summary>
        public int OrderQTY { get; set; }
        /// <summary>
        /// 已退貨數
        /// </summary>
        public int CMQTY { get; set; }
        /// <summary>
        /// 移庫入庫數
        /// </summary>
        public int TransferInQTY { get; set; }
        /// <summary>
        /// 移庫出庫數
        /// </summary>
        public int TransferOutQTY { get; set; }
        /// <summary>
        /// 移庫已收貨
        /// </summary>
        public int TransferOutCloseQTY { get; set; }
        /// <summary>
        /// 等待移倉的數量
        /// </summary>
        public int TransferAwaiting { get; set; }
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
        public int Awaiting { get; set; }
        /// <summary>
        /// 可上架的庫存總數
        /// </summary>
        public int Aggregate { get; set; }
        /// <summary>
        /// 不可銷售的庫總量
        /// </summary>
        public int Unfulfillable { get; set; }
        /// <summary>
        ///  不可銷售的庫存總數,只限於 RMA 倉庫裡的庫存數量
        /// </summary>
        public int UnfulfillableRMA { get; set; }
    }
    public class CompanyVM
    {
        /// <summary>
        /// Is Enable
        /// </summary>        
        [Display(Name = "Company_IsEnable", ResourceType = typeof(App_GlobalResources.Resource))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
        public bool IsEnable { get; set; }


        /// <summary>
        /// ID
        /// </summary>        
        [Display(Name = "Company_ID", ResourceType = typeof(App_GlobalResources.Resource))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
        public int ID { get; set; }


        /// <summary>
        /// Name
        /// </summary>        
        [Display(Name = "Company_Name", ResourceType = typeof(App_GlobalResources.Resource))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
        [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
        public string Name { get; set; }


        /// <summary>
        /// Shandow Suffix
        /// </summary>        
        [Display(Name = "Company_ShadowSuffix", ResourceType = typeof(App_GlobalResources.Resource))]
        [MaxLength(10, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
        public string ShadowSuffix { get; set; }


        /// <summary>
        /// Parent ID
        /// </summary>        
        [Display(Name = "Company_ParentID", ResourceType = typeof(App_GlobalResources.Resource))]
        public Nullable<int> ParentID { get; set; }


        /// <summary>
        /// Relate ID
        /// </summary>        
        [Display(Name = "Company_RelateID", ResourceType = typeof(App_GlobalResources.Resource))]
        public Nullable<int> RelateID { get; set; }


        /// <summary>
        /// e Bay Account ID
        /// </summary>        
        [Display(Name = "Company_eBayAccountID", ResourceType = typeof(App_GlobalResources.Resource))]
        [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
        public string eBayAccountID { get; set; }


        /// <summary>
        /// Amazon Account ID
        /// </summary>        
        [Display(Name = "Company_AmazonAccountID", ResourceType = typeof(App_GlobalResources.Resource))]
        [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
        public string AmazonAccountID { get; set; }


        /// <summary>
        /// Create By
        /// </summary>        
        [Display(Name = "Company_CreateBy", ResourceType = typeof(App_GlobalResources.Resource))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
        [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
        public string CreateBy { get; set; }


        /// <summary>
        /// Create At
        /// </summary>        
        [Display(Name = "Company_CreateAt", ResourceType = typeof(App_GlobalResources.Resource))]
        [UIHint("DateTime")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
        public System.DateTime CreateAt { get; set; }


        /// <summary>
        /// Update By
        /// </summary>        
        [Display(Name = "Company_UpdateBy", ResourceType = typeof(App_GlobalResources.Resource))]
        [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
        public string UpdateBy { get; set; }


        /// <summary>
        /// Update At
        /// </summary>        
        [Display(Name = "Company_UpdateAt", ResourceType = typeof(App_GlobalResources.Resource))]
        [UIHint("DateTime")]
        public Nullable<System.DateTime> UpdateAt { get; set; }


        /// <summary>
        /// Company No
        /// </summary>        
        [Display(Name = "Company_CompanyNo", ResourceType = typeof(App_GlobalResources.Resource))]
        [MaxLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
        public string CompanyNo { get; set; }
    }

    public class DispatchWarehouseVM
    {

        /// <summary>
        /// Is Sellable
        /// </summary>        
        [Display(Name = "Warehouse_IsSellable", ResourceType = typeof(App_GlobalResources.Resource))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
        public bool IsSellable { get; set; }


        /// <summary>
        /// ID
        /// </summary>        
        [Display(Name = "Warehouse_ID", ResourceType = typeof(App_GlobalResources.Resource))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
        public int ID { get; set; }


        /// <summary>
        /// Name
        /// </summary>        
        [Display(Name = "Warehouse_Name", ResourceType = typeof(App_GlobalResources.Resource))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
        [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
        public string Name { get; set; }


        /// <summary>
        /// Type
        /// </summary>        
        [Display(Name = "Warehouse_Type", ResourceType = typeof(App_GlobalResources.Resource))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
        [MaxLength(10, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
        public string Type { get; set; }

        /// <summary>
        /// Default Dispatch
        /// </summary>        
        [Display(Name = "Warehouse_DefaultDispatch", ResourceType = typeof(App_GlobalResources.Resource))]
        public bool DefaultDispatch { get; set; }

        /// <summary>
        /// Default RMA
        /// </summary>        
        [Display(Name = "Warehouse_DefaultRMA", ResourceType = typeof(App_GlobalResources.Resource))]
        public bool DefaultRMA { get; set; }
    }
    public class AddSKUserialsVM
    {
        public string SKU { get; set; }
        public string serials { get; set; }
        public string ProductName { get; set; }
        public decimal? Price { get; set; }
        public decimal? Discount { get; set; }
    }
    public class TransferItemVM
    {
        public int ID { get; set; }
        public int? WarehouseID { get; set; }
        public string SKU { get; set; }
        public string Name { get; set; }
        public int? QTY { get; set; }
        public List<SerialsLlist> SerialsLlist { get; set; }
        public List<RMASerialsLlist> RMASerialsLlist { get; set; }

    }
    public class PrepTable
    {
        public int ID { get; set; }
        public string SKU { get; set; }
        public string Name { get; set; }
        public string QTY { get; set; }
        public string Serial { get; set; }
        public string Label { get; set; }
        public string Download { get; set; }
        /// <summary>
        /// 開啟序號管理
        /// </summary>
        public bool SerialTracking { get; set; }
        public int? count { get; set; }
        public bool Full { get; set; }
        public int? MaxQTY { get; set; }
    }
    public class GetImgVM
    {
        public int id { get; set; }
        public string key { get; set; }
        public string ImgType { get; set; }
        public int MaxCount { get; set; }
        public List<string> imglist { get; set; }
    }
    public class AwaitingDispatchVM
    {
        public string SCID { get; set; }
        public string SKU { get; set; }
        public int QTY { get; set; }
    }

    public class GetSkuInventoryQTYVM
    {
        public string[] WarehouseIDs { get; set; }
        public string[] Skus { get; set; }
    }
    public class TransferSearchVM
    {
        public string Status { get; set; }
        public string SKU { get; set; }
        public string Serial { get; set; }
        public int? From { get; set; }
        public int? Interim { get; set; }
        public int? To { get; set; }
        public int? Transfer { get; set; }
        public string ExternalTransfer { get; set; }
        public string Title { get; set; }
        public string Carrier { get; set; }
        public string Tracking { get; set; }
        public IEnumerable<Transfer> Transferlist { get; set; }
    }
    public class RMAVM : RMA
    {
        public int? QID { get; set; }
        public string Serial { get; set; }
        public IEnumerable<RMA> RMAList { get; set; }
    }

    public class SKUIData
    {
        public string SKU { get; set; }
        /// <summary>
        /// 單價
        /// </summary>
        public decimal? UnitPrice { get; set; }
        public int QTY { get; set; }
        public List<string> Serials { get; set; }
    }

    public class OrderItemData
    {
        public int OrderID { get; set; }
        /// <summary>
        /// 表示是那個平台 channel
        /// </summary>
        public int OrderSource { get; set; }
        /// <summary>
        /// SourceId
        /// </summary>
        public string OrderSourceOrderId { get; set; }
        public int CompanyID { get; set; }
        public string CountryCode { get; set; }
        public List<SKUIData> Items { get; set; }
        public int WarehouseID { get; set; }
        /// <summary>
        /// 運費
        /// </summary>
        public decimal? FinalShippingFee { get; set; }
        public string eBayUserID { get; set; }
    }
    public class RMAModelVM
    {
        public int ck { get; set; }
        public int Order { get; set; }
        public string SourceID { get; set; }
        public string SKU { get; set; }
        public string ProductName { get; set; }
        public string UPC { get; set; }
        public int? QTY { get; set; }
        public int? RMAQTY { get; set; }
        public string Serial { get; set; }
        public int? Warehouse { get; set; }
        public string Reason { get; set; }
    }
    public class RMAModelPost
    {
        public int ID { get; set; }
        public int OrderID { get; set; }
        public string SKU { get; set; }
        public string Serial { get; set; }
        public string Reason { get; set; }
        public int? Warehouse { get; set; }
    }
    public class RMASerial
    {
        public string Serial { get; set; }
        public string Warehouse { get; set; }
        public string Reason { get; set; }
    }
    public class RMAEdit
    {
        public int ID { get; set; }
        public string SKU { get; set; }
        public string ProductName { get; set; }
        public string UPCEAN { get; set; }
        public int? QTYOrdered { get; set; }
        public string Reason { get; set; }
        public int? Warehouse { get; set; }
        public string OrderSerialsNo { get; set; }
        public string ReturnedSerialsNo { get; set; }
        public string TrWarehouse { get; set; }
        public string Model { get; set; }
        public decimal UnitPrice { get; set; }
    }

    public class SerialsLlistVM
    {
        public DateTime Date { get; set; }
        public string ISType { get; set; }
        public int? ID { get; set; }
        public string Warehouse { get; set; }
        public int? QTY { get; set; }
        public string UpdatedBy { get; set; }

    }
    public class StatementVM
    {
        public string SKU { get; set; }
        public DateTime Date { get; set; }
        public string Supplier { get; set; }
        public string Channel { get; set; }
        public string ISType { get; set; }
        public int? ID { get; set; }
        public string Warehouse { get; set; }
        public string Serial { get; set; }
        public int? QTY { get; set; }
        public int? BalanceAggregate { get; set; }
        public int? BalanceAvailable { get; set; }
        public decimal? ValueAvailable { get; set; }
        public decimal? price { get; set; }
        public int? OrderID { get; set; }
    }

    public class ExcelQTY
    {
        public int? WarehouseID { get; set; }
        public string WarehouseName { get; set; }
        public string SKU { get; set; }
        public string SKUName { get; set; }
        public int? QTY { get; set; }
    }
    public class ExcelSerial
    {
        public int? WarehouseID { get; set; }
        public string WarehouseName { get; set; }
        public string SKU { get; set; }
        public string SKUName { get; set; }
        public string Serial { get; set; }
    }
    public class InventorySerials
    {
        public string CompanyName { get; set; }
        public string SKU { get; set; }
        public string SKUName { get; set; }
        public List<InventorySerialsItem> InventorySerialsItem { get; set; }
    }
    public class InventorySerialsItem
    {
        public string Serial { get; set; }
        public int? Order { get; set; }
        public int? RMA { get; set; }
        public int? PO { get; set; }
        public int? CM { get; set; }
        public int? Transfer { get; set; }
        public int? Stock { get; set; }
        public string IStype { get; set; }
        public string Warehouse { get; set; }
        public string DispatchLocation { get; set; }
        public decimal? Value { get; set; }
        public DateTime? Date { get; set; }
    }

}