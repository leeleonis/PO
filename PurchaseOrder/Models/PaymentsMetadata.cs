using System; 
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using System; 
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using System; 
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace PurchaseOrderSys.Models
{
    
    /// <summary>
    /// Payments class
    /// </summary>
    [MetadataType(typeof(PaymentsMetadata))]
    public  partial class Payments
    {
    
    	/// <summary>
    	/// Payments Metadata class
    	/// </summary>
    	public   class PaymentsMetadata
    	{
    		    
    		/// <summary>
    		/// Is Enable
    		/// </summary>        
    	    [Display(Name = "Payments_IsEnable", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public bool  IsEnable { get; set; }
    
    		    
    		/// <summary>
    		/// ID
    		/// </summary>        
    	    [Display(Name = "Payments_ID", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public int  ID { get; set; }
    
    		    
    		/// <summary>
    		/// SCID
    		/// </summary>        
    	    [Display(Name = "Payments_SCID", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<int>  SCID { get; set; }
    
    		    
    		/// <summary>
    		/// Order ID
    		/// </summary>        
    	    [Display(Name = "Payments_OrderID", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public int  OrderID { get; set; }
    
    		    
    		/// <summary>
    		/// Status
    		/// </summary>        
    	    [Display(Name = "Payments_Status", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public int  Status { get; set; }
    
    		    
    		/// <summary>
    		/// Date
    		/// </summary>        
    	    [Display(Name = "Payments_Date", ResourceType = typeof(App_GlobalResources.Resource))]
            [UIHint("DateTime")]
    		public Nullable<System.DateTime>  Date { get; set; }
    
    		    
    		/// <summary>
    		/// Gateway
    		/// </summary>        
    	    [Display(Name = "Payments_Gateway", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<int>  Gateway { get; set; }
    
    		    
    		/// <summary>
    		/// Total Value
    		/// </summary>        
    	    [Display(Name = "Payments_TotalValue", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public decimal  TotalValue { get; set; }
    
    		    
    		/// <summary>
    		/// Shipping Charge
    		/// </summary>        
    	    [Display(Name = "Payments_ShippingCharge", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public decimal  ShippingCharge { get; set; }
    
    		    
    		/// <summary>
    		/// Insurance Charge
    		/// </summary>        
    	    [Display(Name = "Payments_InsuranceCharge", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public decimal  InsuranceCharge { get; set; }
    
    		    
    		/// <summary>
    		/// Grand Total
    		/// </summary>        
    	    [Display(Name = "Payments_GrandTotal", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public decimal  GrandTotal { get; set; }
    
    		    
    		/// <summary>
    		/// Payment Total
    		/// </summary>        
    	    [Display(Name = "Payments_PaymentTotal", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public decimal  PaymentTotal { get; set; }
    
    		    
    		/// <summary>
    		/// Refund
    		/// </summary>        
    	    [Display(Name = "Payments_Refund", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public decimal  Refund { get; set; }
    
    		    
    		/// <summary>
    		/// Balance
    		/// </summary>        
    	    [Display(Name = "Payments_Balance", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public decimal  Balance { get; set; }
    
    		    
    		/// <summary>
    		/// Transaction ID
    		/// </summary>        
    	    [Display(Name = "Payments_TransactionID", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  TransactionID { get; set; }
    
    		    
    		/// <summary>
    		/// Payment Email
    		/// </summary>        
    	    [Display(Name = "Payments_PaymentEmail", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  PaymentEmail { get; set; }
    
    		    
    		/// <summary>
    		/// Update_by
    		/// </summary>        
    	    [Display(Name = "Payments_Update_by", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  Update_by { get; set; }
    
    		    
    		/// <summary>
    		/// Update_at
    		/// </summary>        
    	    [Display(Name = "Payments_Update_at", ResourceType = typeof(App_GlobalResources.Resource))]
            [UIHint("DateTime")]
    		public Nullable<System.DateTime>  Update_at { get; set; }
    
    		    
    		/// <summary>
    		/// Create_by
    		/// </summary>        
    	    [Display(Name = "Payments_Create_by", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  Create_by { get; set; }
    
    		    
    		/// <summary>
    		/// Create_at
    		/// </summary>        
    	    [Display(Name = "Payments_Create_at", ResourceType = typeof(App_GlobalResources.Resource))]
            [UIHint("DateTime")]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public System.DateTime  Create_at { get; set; }
    
    		    
    	}
    }
    
}
