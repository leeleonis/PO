using System; 
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace PurchaseOrderSys.Models
{
    
    /// <summary>
    /// CreditMemo class
    /// </summary>
    [MetadataType(typeof(CreditMemoMetadata))]
    public  partial class CreditMemo
    {
    
    	/// <summary>
    	/// CreditMemo Metadata class
    	/// </summary>
    	public   class CreditMemoMetadata
    	{
    		    
    		/// <summary>
    		/// Is Enable
    		/// </summary>        
    	    [Display(Name = "CreditMemo_IsEnable", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public bool  IsEnable { get; set; }
    
    		    
    		/// <summary>
    		/// ID
    		/// </summary>        
    	    [Display(Name = "CreditMemo_ID", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public int  ID { get; set; }
    
    		    
    		/// <summary>
    		/// Purchase Order ID
    		/// </summary>        
    	    [Display(Name = "CreditMemo_PurchaseOrderID", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<int>  PurchaseOrderID { get; set; }
    
    		    
    		/// <summary>
    		/// Company ID
    		/// </summary>        
    	    [Display(Name = "CreditMemo_CompanyID", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<int>  CompanyID { get; set; }
    
    		    
    		/// <summary>
    		/// Vendor ID
    		/// </summary>        
    	    [Display(Name = "CreditMemo_VendorID", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<int>  VendorID { get; set; }
    
    		    
    		/// <summary>
    		/// Invoice Date
    		/// </summary>        
    	    [Display(Name = "CreditMemo_InvoiceDate", ResourceType = typeof(App_GlobalResources.Resource))]
            [UIHint("FDate")]
            public Nullable<System.DateTime>  InvoiceDate { get; set; }
    
    		    
    		/// <summary>
    		/// Invoice No
    		/// </summary>        
    	    [Display(Name = "CreditMemo_InvoiceNo", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  InvoiceNo { get; set; }
    
    		    
    		/// <summary>
    		/// CMStatus
    		/// </summary>        
    	    [Display(Name = "CreditMemo_CMStatus", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  CMStatus { get; set; }
    
    		    
    		/// <summary>
    		/// CMType
    		/// </summary>        
    	    [Display(Name = "CreditMemo_CMType", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  CMType { get; set; }
    
    		    
    		/// <summary>
    		/// CMDate
    		/// </summary>        
    	    [Display(Name = "CreditMemo_CMDate", ResourceType = typeof(App_GlobalResources.Resource))]
            [UIHint("FDate")]
            public Nullable<System.DateTime>  CMDate { get; set; }
    
    		    
    		/// <summary>
    		/// Shipping Status
    		/// </summary>        
    	    [Display(Name = "CreditMemo_ShippingStatus", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  ShippingStatus { get; set; }
    
    		    
    		/// <summary>
    		/// Shipped Date
    		/// </summary>        
    	    [Display(Name = "CreditMemo_ShippedDate", ResourceType = typeof(App_GlobalResources.Resource))]
            [UIHint("FDate")]
            public Nullable<System.DateTime>  ShippedDate { get; set; }
    
    		    
    		/// <summary>
    		/// Carrier
    		/// </summary>        
    	    [Display(Name = "CreditMemo_Carrier", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  Carrier { get; set; }
    
    		    
    		/// <summary>
    		/// Tracking
    		/// </summary>        
    	    [Display(Name = "CreditMemo_Tracking", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  Tracking { get; set; }
    
    		    
    		/// <summary>
    		/// Credit Status
    		/// </summary>        
    	    [Display(Name = "CreditMemo_CreditStatus", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  CreditStatus { get; set; }
    
    		    
    		/// <summary>
    		/// Credit Date
    		/// </summary>        
    	    [Display(Name = "CreditMemo_CreditDate", ResourceType = typeof(App_GlobalResources.Resource))]
            [UIHint("FDate")]
            public Nullable<System.DateTime>  CreditDate { get; set; }
    
    		    
    		/// <summary>
    		/// Credit Amount
    		/// </summary>        
    	    [Display(Name = "CreditMemo_CreditAmount", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<decimal>  CreditAmount { get; set; }
    
    		    
    		/// <summary>
    		/// Create By
    		/// </summary>        
    	    [Display(Name = "CreditMemo_CreateBy", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  CreateBy { get; set; }
    
    		    
    		/// <summary>
    		/// Create At
    		/// </summary>        
    	    [Display(Name = "CreditMemo_CreateAt", ResourceType = typeof(App_GlobalResources.Resource))]
            [UIHint("DateTime")]
            [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public System.DateTime  CreateAt { get; set; }
    
    		    
    		/// <summary>
    		/// Update By
    		/// </summary>        
    	    [Display(Name = "CreditMemo_UpdateBy", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  UpdateBy { get; set; }
    
    		    
    		/// <summary>
    		/// Update At
    		/// </summary>        
    	    [Display(Name = "CreditMemo_UpdateAt", ResourceType = typeof(App_GlobalResources.Resource))]
            [UIHint("DateTime")]
    		public Nullable<System.DateTime>  UpdateAt { get; set; }
    
    		    
    		/// <summary>
    		/// Received By
    		/// </summary>        
    	    [Display(Name = "CreditMemo_ReceivedBy", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  ReceivedBy { get; set; }
    
    		    
    		/// <summary>
    		/// Received At
    		/// </summary>        
    	    [Display(Name = "CreditMemo_ReceivedAt", ResourceType = typeof(App_GlobalResources.Resource))]
            [UIHint("DateTime")]
    		public Nullable<System.DateTime>  ReceivedAt { get; set; }
    
    		    
    	}
    }
    
}
