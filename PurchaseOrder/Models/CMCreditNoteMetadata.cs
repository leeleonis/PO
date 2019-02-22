using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace PurchaseOrderSys.Models
{

    /// <summary>
    /// CMCreditNote class
    /// </summary>
    [MetadataType(typeof(CMCreditNoteMetadata))]
    public  partial class CMCreditNote
    {
    
    	/// <summary>
    	/// CMCreditNote Metadata class
    	/// </summary>
    	public   class CMCreditNoteMetadata
    	{
    		    
    		/// <summary>
    		/// Is Enable
    		/// </summary>        
    	    [Display(Name = "CMCreditNote_IsEnable", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public bool  IsEnable { get; set; }
    
    		    
    		/// <summary>
    		/// ID
    		/// </summary>        
    	    [Display(Name = "CMCreditNote_ID", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public int  ID { get; set; }
    
    		    
    		/// <summary>
    		/// Purchase Order ID
    		/// </summary>        
    	    [Display(Name = "CMCreditNote_PurchaseOrderID", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<int>  PurchaseOrderID { get; set; }
    
    		    
    		/// <summary>
    		/// Company ID
    		/// </summary>        
    	    [Display(Name = "CMCreditNote_CompanyID", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<int>  CompanyID { get; set; }
    
    		    
    		/// <summary>
    		/// Vendor ID
    		/// </summary>        
    	    [Display(Name = "CMCreditNote_VendorID", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<int>  VendorID { get; set; }
    
    		    
    		/// <summary>
    		/// Invoice Date
    		/// </summary>        
    	    [Display(Name = "CMCreditNote_InvoiceDate", ResourceType = typeof(App_GlobalResources.Resource))]
            [UIHint("DateTime")]
    		public Nullable<System.DateTime>  InvoiceDate { get; set; }
    
    		    
    		/// <summary>
    		/// Invoice No
    		/// </summary>        
    	    [Display(Name = "CMCreditNote_InvoiceNo", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  InvoiceNo { get; set; }
    
    		    
    		/// <summary>
    		/// CMStatus
    		/// </summary>        
    	    [Display(Name = "CMCreditNote_CMStatus", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  CMStatus { get; set; }
    
    		    
    		/// <summary>
    		/// CMType
    		/// </summary>        
    	    [Display(Name = "CMCreditNote_CMType", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  CMType { get; set; }
    
    		    
    		/// <summary>
    		/// CMDate
    		/// </summary>        
    	    [Display(Name = "CMCreditNote_CMDate", ResourceType = typeof(App_GlobalResources.Resource))]
            [UIHint("DateTime")]
    		public Nullable<System.DateTime>  CMDate { get; set; }
    
    		    
    		/// <summary>
    		/// Shipping Status
    		/// </summary>        
    	    [Display(Name = "CMCreditNote_ShippingStatus", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  ShippingStatus { get; set; }
    
    		    
    		/// <summary>
    		/// Shipped Date
    		/// </summary>        
    	    [Display(Name = "CMCreditNote_ShippedDate", ResourceType = typeof(App_GlobalResources.Resource))]
            [UIHint("DateTime")]
    		public Nullable<System.DateTime>  ShippedDate { get; set; }
    
    		    
    		/// <summary>
    		/// Carrier
    		/// </summary>        
    	    [Display(Name = "CMCreditNote_Carrier", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  Carrier { get; set; }
    
    		    
    		/// <summary>
    		/// Tracking
    		/// </summary>        
    	    [Display(Name = "CMCreditNote_Tracking", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  Tracking { get; set; }
    
    		    
    		/// <summary>
    		/// Credit Status
    		/// </summary>        
    	    [Display(Name = "CMCreditNote_CreditStatus", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  CreditStatus { get; set; }
    
    		    
    		/// <summary>
    		/// Credit Date
    		/// </summary>        
    	    [Display(Name = "CMCreditNote_CreditDate", ResourceType = typeof(App_GlobalResources.Resource))]
            [UIHint("DateTime")]
    		public Nullable<System.DateTime>  CreditDate { get; set; }
    
    		    
    		/// <summary>
    		/// Credit Amount
    		/// </summary>        
    	    [Display(Name = "CMCreditNote_CreditAmount", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<decimal>  CreditAmount { get; set; }
    
    		    
    		/// <summary>
    		/// Create By
    		/// </summary>        
    	    [Display(Name = "CMCreditNote_CreateBy", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  CreateBy { get; set; }
    
    		    
    		/// <summary>
    		/// Create At
    		/// </summary>        
    	    [Display(Name = "CMCreditNote_CreateAt", ResourceType = typeof(App_GlobalResources.Resource))]
            [UIHint("DateTime")]
            [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public System.DateTime  CreateAt { get; set; }
    
    		    
    		/// <summary>
    		/// Update By
    		/// </summary>        
    	    [Display(Name = "CMCreditNote_UpdateBy", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  UpdateBy { get; set; }
    
    		    
    		/// <summary>
    		/// Update At
    		/// </summary>        
    	    [Display(Name = "CMCreditNote_UpdateAt", ResourceType = typeof(App_GlobalResources.Resource))]
            [UIHint("DateTime")]
    		public Nullable<System.DateTime>  UpdateAt { get; set; }
    
    		    
    		/// <summary>
    		/// Received By
    		/// </summary>        
    	    [Display(Name = "CMCreditNote_ReceivedBy", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  ReceivedBy { get; set; }
    
    		    
    		/// <summary>
    		/// Received At
    		/// </summary>        
    	    [Display(Name = "CMCreditNote_ReceivedAt", ResourceType = typeof(App_GlobalResources.Resource))]
            [UIHint("DateTime")]
    		public Nullable<System.DateTime>  ReceivedAt { get; set; }
    
    		    
    	}
    }
    
}
