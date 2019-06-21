using System; 
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Newtonsoft.Json;
namespace PurchaseOrderSys.Models
{

    /// <summary>
    /// PurchaseOrder class
    /// </summary>
    [MetadataType(typeof(PurchaseOrderMetadata))]
    public  partial class PurchaseOrder
    {
    
    	/// <summary>
    	/// PurchaseOrder Metadata class
    	/// </summary>
    	public   class PurchaseOrderMetadata
    	{
    		    
    		/// <summary>
    		/// Is Enable
    		/// </summary>        
    	    [Display(Name = "PurchaseOrder_IsEnable", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public bool  IsEnable { get; set; }
    
    		    
    		/// <summary>
    		/// ID
    		/// </summary>        
    	    [Display(Name = "PurchaseOrder_ID", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public int  ID { get; set; }
    
    		    
    		/// <summary>
    		/// Company ID
    		/// </summary>        
    	    [Display(Name = "PurchaseOrder_CompanyID", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
            public Nullable<int>  CompanyID { get; set; }
    
    		    
    		/// <summary>
    		/// Vendor ID
    		/// </summary>        
    	    [Display(Name = "PurchaseOrder_VendorID", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
            public Nullable<int>  VendorID { get; set; }
    
    		    
    		/// <summary>
    		/// POType
    		/// </summary>        
    	    [Display(Name = "PurchaseOrder_POType", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  POType { get; set; }
    
    		    
    		/// <summary>
    		/// PODate
    		/// </summary>        
    	    [Display(Name = "PurchaseOrder_PODate", ResourceType = typeof(App_GlobalResources.Resource))]
            [UIHint("FDate")]
    		public Nullable<System.DateTime>  PODate { get; set; }
    
    		    
    		/// <summary>
    		/// Receive Status
    		/// </summary>        
    	    [Display(Name = "PurchaseOrder_ReceiveStatus", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  ReceiveStatus { get; set; }
    
    		    
    		/// <summary>
    		/// Received Date
    		/// </summary>        
    	    [Display(Name = "PurchaseOrder_ReceivedDate", ResourceType = typeof(App_GlobalResources.Resource))]
            [UIHint("FDate")]
    		public Nullable<System.DateTime>  ReceivedDate { get; set; }
    
    		    
    		/// <summary>
    		/// Shipped Date
    		/// </summary>        
    	    [Display(Name = "PurchaseOrder_ShippedDate", ResourceType = typeof(App_GlobalResources.Resource))]
            [UIHint("FDate")]
    		public Nullable<System.DateTime>  ShippedDate { get; set; }
    
    		    
    		/// <summary>
    		/// Carrier
    		/// </summary>        
    	    [Display(Name = "PurchaseOrder_Carrier", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  Carrier { get; set; }
    
    		    
    		/// <summary>
    		/// Tracking
    		/// </summary>        
    	    [Display(Name = "PurchaseOrder_Tracking", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  Tracking { get; set; }
    
    		    
    		/// <summary>
    		/// Payment Status
    		/// </summary>        
    	    [Display(Name = "PurchaseOrder_PaymentStatus", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  PaymentStatus { get; set; }
    
    		    
    		/// <summary>
    		/// Payment Date
    		/// </summary>        
    	    [Display(Name = "PurchaseOrder_PaymentDate", ResourceType = typeof(App_GlobalResources.Resource))]
            [UIHint("FDate")]
    		public Nullable<System.DateTime>  PaymentDate { get; set; }
    
    		    
    		/// <summary>
    		/// Paid Amount
    		/// </summary>        
    	    [Display(Name = "PurchaseOrder_PaidAmount", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<decimal>  PaidAmount { get; set; }
    
    		    
    		/// <summary>
    		/// Create By
    		/// </summary>        
    	    [Display(Name = "PurchaseOrder_CreateBy", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  CreateBy { get; set; }
    
    		    
    		/// <summary>
    		/// Create At
    		/// </summary>        
    	    [Display(Name = "PurchaseOrder_CreateAt", ResourceType = typeof(App_GlobalResources.Resource))]
            [UIHint("FDate")]
            [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public System.DateTime  CreateAt { get; set; }
    
    		    
    		/// <summary>
    		/// Update By
    		/// </summary>        
    	    [Display(Name = "PurchaseOrder_UpdateBy", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  UpdateBy { get; set; }
    
    		    
    		/// <summary>
    		/// Update At
    		/// </summary>        
    	    [Display(Name = "PurchaseOrder_UpdateAt", ResourceType = typeof(App_GlobalResources.Resource))]
            [UIHint("DateTime")]
    		public Nullable<System.DateTime>  UpdateAt { get; set; }
    
    		    
    		/// <summary>
    		/// Received By
    		/// </summary>        
    	    [Display(Name = "PurchaseOrder_ReceivedBy", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  ReceivedBy { get; set; }
    
    		    
    		/// <summary>
    		/// Received At
    		/// </summary>        
    	    [Display(Name = "PurchaseOrder_ReceivedAt", ResourceType = typeof(App_GlobalResources.Resource))]
            [UIHint("FDate")]
    		public Nullable<System.DateTime>  ReceivedAt { get; set; }
    
    		    
    		/// <summary>
    		/// POStatus
    		/// </summary>        
    	    [Display(Name = "PurchaseOrder_POStatus", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  POStatus { get; set; }
    
    		    
    		/// <summary>
    		/// Invoice Date
    		/// </summary>        
    	    [Display(Name = "PurchaseOrder_InvoiceDate", ResourceType = typeof(App_GlobalResources.Resource))]
            [UIHint("FDate")]
    		public Nullable<System.DateTime>  InvoiceDate { get; set; }
    
    		    
    		/// <summary>
    		/// Invoice No
    		/// </summary>        
    	    [Display(Name = "PurchaseOrder_InvoiceNo", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  InvoiceNo { get; set; }
    
    		    
    		/// <summary>
    		/// Payment Proof
    		/// </summary>        
    	    [Display(Name = "PurchaseOrder_PaymentProof", ResourceType = typeof(App_GlobalResources.Resource))]
    		public string  PaymentProof { get; set; }
    
    		    
    		/// <summary>
    		/// Warehouse
    		/// </summary>        
    	    [Display(Name = "PurchaseOrder_Warehouse", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  Warehouse { get; set; }
    
    		    
    		/// <summary>
    		/// Currency
    		/// </summary>        
    	    [Display(Name = "PurchaseOrder_Currency", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(10, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  Currency { get; set; }
    
    		    
    		/// <summary>
    		/// Tax
    		/// </summary>        
    	    [Display(Name = "PurchaseOrder_Tax", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<decimal>  Tax { get; set; }
    
    		    
    		/// <summary>
    		/// Warehouse ID
    		/// </summary>        
    	    [Display(Name = "PurchaseOrder_WarehouseID", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<int>  WarehouseID { get; set; }
    
    		    
    		/// <summary>
    		/// Shipping Cost
    		/// </summary>        
    	    [Display(Name = "PurchaseOrder_ShippingCost", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<decimal>  ShippingCost { get; set; }
    
    		    
    		/// <summary>
    		/// Other
    		/// </summary>        
    	    [Display(Name = "PurchaseOrder_Other", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<decimal>  Other { get; set; }

            [JsonIgnore()]
            public virtual Company Company { get; set; }
        }
    }
    
}
