using System; 
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using System;
using System.ComponentModel.DataAnnotations;


namespace PurchaseOrderSys.Models
{

    /// <summary>
    /// Orders class
    /// </summary>
    [MetadataType(typeof(OrdersMetadata))]
    public  partial class Orders
    {
    
    	/// <summary>
    	/// Orders Metadata class
    	/// </summary>
    	public   class OrdersMetadata
    	{
    		    
    		/// <summary>
    		/// Order ID
    		/// </summary>        
    	    [Display(Name = "Orders_OrderID", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public int  OrderID { get; set; }
    
    		    
    		/// <summary>
    		/// Parent Order ID
    		/// </summary>        
    	    [Display(Name = "Orders_ParentOrderID", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<int>  ParentOrderID { get; set; }
    
    		    
    		/// <summary>
    		/// Client ID
    		/// </summary>        
    	    [Display(Name = "Orders_ClientID", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<int>  ClientID { get; set; }
    
    		    
    		/// <summary>
    		/// Company ID
    		/// </summary>        
    	    [Display(Name = "Orders_CompanyID", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<int>  CompanyID { get; set; }
    
    		    
    		/// <summary>
    		/// User ID
    		/// </summary>        
    	    [Display(Name = "Orders_UserID", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<int>  UserID { get; set; }
    
    		    
    		/// <summary>
    		/// User Name
    		/// </summary>        
    	    [Display(Name = "Orders_UserName", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(100, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  UserName { get; set; }
    
    		    
    		/// <summary>
    		/// Site Code
    		/// </summary>        
    	    [Display(Name = "Orders_SiteCode", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<int>  SiteCode { get; set; }
    
    		    
    		/// <summary>
    		/// Time Of Order
    		/// </summary>        
    	    [Display(Name = "Orders_TimeOfOrder", ResourceType = typeof(App_GlobalResources.Resource))]
            [UIHint("DateTime")]
    		public Nullable<System.DateTime>  TimeOfOrder { get; set; }
    
    		    
    		/// <summary>
    		/// Sub Total
    		/// </summary>        
    	    [Display(Name = "Orders_SubTotal", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<decimal>  SubTotal { get; set; }
    
    		    
    		/// <summary>
    		/// Shipping Total
    		/// </summary>        
    	    [Display(Name = "Orders_ShippingTotal", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<decimal>  ShippingTotal { get; set; }
    
    		    
    		/// <summary>
    		/// Order Discounts Total
    		/// </summary>        
    	    [Display(Name = "Orders_OrderDiscountsTotal", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<decimal>  OrderDiscountsTotal { get; set; }
    
    		    
    		/// <summary>
    		/// Grand Total
    		/// </summary>        
    	    [Display(Name = "Orders_GrandTotal", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<decimal>  GrandTotal { get; set; }
    
    		    
    		/// <summary>
    		/// Status Code
    		/// </summary>        
    	    [Display(Name = "Orders_StatusCode", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<int>  StatusCode { get; set; }
    
    		    
    		/// <summary>
    		/// Payment Status
    		/// </summary>        
    	    [Display(Name = "Orders_PaymentStatus", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<int>  PaymentStatus { get; set; }
    
    		    
    		/// <summary>
    		/// Shipping Status
    		/// </summary>        
    	    [Display(Name = "Orders_ShippingStatus", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<int>  ShippingStatus { get; set; }
    
    		    
    		/// <summary>
    		/// Ship Date
    		/// </summary>        
    	    [Display(Name = "Orders_ShipDate", ResourceType = typeof(App_GlobalResources.Resource))]
            [UIHint("DateTime")]
    		public Nullable<System.DateTime>  ShipDate { get; set; }
    
    		    
    		/// <summary>
    		/// Final Shipping Fee
    		/// </summary>        
    	    [Display(Name = "Orders_FinalShippingFee", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<decimal>  FinalShippingFee { get; set; }
    
    		    
    		/// <summary>
    		/// Shipping Address
    		/// </summary>        
    	    [Display(Name = "Orders_ShippingAddress", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<int>  ShippingAddress { get; set; }
    
    		    
    		/// <summary>
    		/// Order Currency Code
    		/// </summary>        
    	    [Display(Name = "Orders_OrderCurrencyCode", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<int>  OrderCurrencyCode { get; set; }
    
    		    
    		/// <summary>
    		/// Order Source
    		/// </summary>        
    	    [Display(Name = "Orders_OrderSource", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<int>  OrderSource { get; set; }
    
    		    
    		/// <summary>
    		/// Order Source Order Id
    		/// </summary>        
    	    [Display(Name = "Orders_OrderSourceOrderId", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  OrderSourceOrderId { get; set; }
    
    		    
    		/// <summary>
    		/// Order Source Order Total
    		/// </summary>        
    	    [Display(Name = "Orders_OrderSourceOrderTotal", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<decimal>  OrderSourceOrderTotal { get; set; }
    
    		    
    		/// <summary>
    		/// e Bay User ID
    		/// </summary>        
    	    [Display(Name = "Orders_eBayUserID", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  eBayUserID { get; set; }
    
    		    
    		/// <summary>
    		/// e Bay Sales Record Number
    		/// </summary>        
    	    [Display(Name = "Orders_eBaySalesRecordNumber", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  eBaySalesRecordNumber { get; set; }
    
    		    
    		/// <summary>
    		/// Shipping Service Selected
    		/// </summary>        
    	    [Display(Name = "Orders_ShippingServiceSelected", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(150, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  ShippingServiceSelected { get; set; }
    
    		    
    		/// <summary>
    		/// Rush Order
    		/// </summary>        
    	    [Display(Name = "Orders_RushOrder", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<bool>  RushOrder { get; set; }
    
    		    
    		/// <summary>
    		/// Invoice Printed
    		/// </summary>        
    	    [Display(Name = "Orders_InvoicePrinted", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<bool>  InvoicePrinted { get; set; }
    
    		    
    		/// <summary>
    		/// Invoice Printed Date
    		/// </summary>        
    	    [Display(Name = "Orders_InvoicePrintedDate", ResourceType = typeof(App_GlobalResources.Resource))]
            [UIHint("DateTime")]
    		public Nullable<System.DateTime>  InvoicePrintedDate { get; set; }
    
    		    
    		/// <summary>
    		/// Shipping Carrier
    		/// </summary>        
    	    [Display(Name = "Orders_ShippingCarrier", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  ShippingCarrier { get; set; }
    
    		    
    		/// <summary>
    		/// Shipping Country
    		/// </summary>        
    	    [Display(Name = "Orders_ShippingCountry", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  ShippingCountry { get; set; }
    
    		    
    		/// <summary>
    		/// Package Type
    		/// </summary>        
    	    [Display(Name = "Orders_PackageType", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  PackageType { get; set; }
    
    		    
    		/// <summary>
    		/// Station ID
    		/// </summary>        
    	    [Display(Name = "Orders_StationID", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<int>  StationID { get; set; }
    
    		    
    		/// <summary>
    		/// Customer Service Status
    		/// </summary>        
    	    [Display(Name = "Orders_CustomerServiceStatus", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<int>  CustomerServiceStatus { get; set; }
    
    		    
    		/// <summary>
    		/// Tax Rate
    		/// </summary>        
    	    [Display(Name = "Orders_TaxRate", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<decimal>  TaxRate { get; set; }
    
    		    
    		/// <summary>
    		/// Tax Total
    		/// </summary>        
    	    [Display(Name = "Orders_TaxTotal", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<decimal>  TaxTotal { get; set; }
    
    		    
    		/// <summary>
    		/// Google Order Number
    		/// </summary>        
    	    [Display(Name = "Orders_GoogleOrderNumber", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  GoogleOrderNumber { get; set; }
    
    		    
    		/// <summary>
    		/// Is In Dispute
    		/// </summary>        
    	    [Display(Name = "Orders_IsInDispute", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<bool>  IsInDispute { get; set; }
    
    		    
    		/// <summary>
    		/// Dispute Started On
    		/// </summary>        
    	    [Display(Name = "Orders_DisputeStartedOn", ResourceType = typeof(App_GlobalResources.Resource))]
            [UIHint("DateTime")]
    		public Nullable<System.DateTime>  DisputeStartedOn { get; set; }
    
    		    
    		/// <summary>
    		/// Paypal Fee Total
    		/// </summary>        
    	    [Display(Name = "Orders_PaypalFeeTotal", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<decimal>  PaypalFeeTotal { get; set; }
    
    		    
    		/// <summary>
    		/// Posting Fee Total
    		/// </summary>        
    	    [Display(Name = "Orders_PostingFeeTotal", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<decimal>  PostingFeeTotal { get; set; }
    
    		    
    		/// <summary>
    		/// Final Value Total
    		/// </summary>        
    	    [Display(Name = "Orders_FinalValueTotal", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<decimal>  FinalValueTotal { get; set; }
    
    		    
    		/// <summary>
    		/// Order Item Count
    		/// </summary>        
    	    [Display(Name = "Orders_OrderItemCount", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<int>  OrderItemCount { get; set; }
    
    		    
    		/// <summary>
    		/// Order Qty Total
    		/// </summary>        
    	    [Display(Name = "Orders_OrderQtyTotal", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<int>  OrderQtyTotal { get; set; }
    
    		    
    		/// <summary>
    		/// Shipping Weight Total Oz
    		/// </summary>        
    	    [Display(Name = "Orders_ShippingWeightTotalOz", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<int>  ShippingWeightTotalOz { get; set; }
    
    		    
    		/// <summary>
    		/// Is Confirmed
    		/// </summary>        
    	    [Display(Name = "Orders_IsConfirmed", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<bool>  IsConfirmed { get; set; }
    
    		    
    		/// <summary>
    		/// Confirm By
    		/// </summary>        
    	    [Display(Name = "Orders_ConfirmBy", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<int>  ConfirmBy { get; set; }
    
    		    
    		/// <summary>
    		/// Confirm On
    		/// </summary>        
    	    [Display(Name = "Orders_ConfirmOn", ResourceType = typeof(App_GlobalResources.Resource))]
            [UIHint("DateTime")]
    		public Nullable<System.DateTime>  ConfirmOn { get; set; }
    
    		    
    		/// <summary>
    		/// Marketting Source ID
    		/// </summary>        
    	    [Display(Name = "Orders_MarkettingSourceID", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<int>  MarkettingSourceID { get; set; }
    
    		    
    		/// <summary>
    		/// Shipped By
    		/// </summary>        
    	    [Display(Name = "Orders_ShippedBy", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<int>  ShippedBy { get; set; }
    
    		    
    		/// <summary>
    		/// Instructions
    		/// </summary>        
    	    [Display(Name = "Orders_Instructions", ResourceType = typeof(App_GlobalResources.Resource))]
    		public string  Instructions { get; set; }
    
    		    
    		/// <summary>
    		/// Sync On
    		/// </summary>        
    	    [Display(Name = "Orders_SyncOn", ResourceType = typeof(App_GlobalResources.Resource))]
            [UIHint("DateTime")]
    		public Nullable<System.DateTime>  SyncOn { get; set; }
    
    		    
    	}
    }
    
}
