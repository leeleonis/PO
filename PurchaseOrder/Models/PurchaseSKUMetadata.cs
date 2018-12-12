using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace PurchaseOrderSys.Models
{

    /// <summary>
    /// PurchaseSKU class
    /// </summary>
    [MetadataType(typeof(PurchaseSKUMetadata))]
    public  partial class PurchaseSKU
    {
    
    	/// <summary>
    	/// PurchaseSKU Metadata class
    	/// </summary>
    	public   class PurchaseSKUMetadata
    	{
    		    
    		/// <summary>
    		/// Is Enable
    		/// </summary>        
    	    [Display(Name = "PurchaseSKU_IsEnable", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public bool  IsEnable { get; set; }
    
    		    
    		/// <summary>
    		/// ID
    		/// </summary>        
    	    [Display(Name = "PurchaseSKU_ID", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public int  ID { get; set; }
    
    		    
    		/// <summary>
    		/// Name
    		/// </summary>        
    	    [Display(Name = "PurchaseSKU_Name", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(200, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  Name { get; set; }
    
    		    
    		/// <summary>
    		/// Purchase Order ID
    		/// </summary>        
    	    [Display(Name = "PurchaseSKU_PurchaseOrderID", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<int>  PurchaseOrderID { get; set; }
    
    		    
    		/// <summary>
    		/// Sku No
    		/// </summary>        
    	    [Display(Name = "PurchaseSKU_SkuNo", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  SkuNo { get; set; }
    
    		    
    		/// <summary>
    		/// QTYOrdered
    		/// </summary>        
    	    [Display(Name = "PurchaseSKU_QTYOrdered", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<int>  QTYOrdered { get; set; }
    
    		    
    		/// <summary>
    		/// QTYFulfilled
    		/// </summary>        
    	    [Display(Name = "PurchaseSKU_QTYFulfilled", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<int>  QTYFulfilled { get; set; }
    
    		    
    		/// <summary>
    		/// Price
    		/// </summary>        
    	    [Display(Name = "PurchaseSKU_Price", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<decimal>  Price { get; set; }
    
    		    
    		/// <summary>
    		/// Discount
    		/// </summary>        
    	    [Display(Name = "PurchaseSKU_Discount", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<decimal>  Discount { get; set; }
    
    		    
    		/// <summary>
    		/// Discounted Price
    		/// </summary>        
    	    [Display(Name = "PurchaseSKU_DiscountedPrice", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<decimal>  DiscountedPrice { get; set; }
    
    		    
    		/// <summary>
    		/// Shipping Cost
    		/// </summary>        
    	    [Display(Name = "PurchaseSKU_ShippingCost", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<decimal>  ShippingCost { get; set; }
    
    		    
    		/// <summary>
    		/// Other
    		/// </summary>        
    	    [Display(Name = "PurchaseSKU_Other", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<decimal>  Other { get; set; }
    
    		    
    		/// <summary>
    		/// Total Refunded
    		/// </summary>        
    	    [Display(Name = "PurchaseSKU_TotalRefunded", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<decimal>  TotalRefunded { get; set; }
    
    		    
    		/// <summary>
    		/// Create By
    		/// </summary>        
    	    [Display(Name = "PurchaseSKU_CreateBy", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  CreateBy { get; set; }
    
    		    
    		/// <summary>
    		/// Create At
    		/// </summary>        
    	    [Display(Name = "PurchaseSKU_CreateAt", ResourceType = typeof(App_GlobalResources.Resource))]
            [UIHint("DateTime")]
            [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public System.DateTime  CreateAt { get; set; }
    
    		    
    		/// <summary>
    		/// Update By
    		/// </summary>        
    	    [Display(Name = "PurchaseSKU_UpdateBy", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  UpdateBy { get; set; }
    
    		    
    		/// <summary>
    		/// Update At
    		/// </summary>        
    	    [Display(Name = "PurchaseSKU_UpdateAt", ResourceType = typeof(App_GlobalResources.Resource))]
            [UIHint("DateTime")]
    		public Nullable<System.DateTime>  UpdateAt { get; set; }
    
    		    
    		/// <summary>
    		/// Received By
    		/// </summary>        
    	    [Display(Name = "PurchaseSKU_ReceivedBy", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  ReceivedBy { get; set; }
    
    		    
    		/// <summary>
    		/// Received At
    		/// </summary>        
    	    [Display(Name = "PurchaseSKU_ReceivedAt", ResourceType = typeof(App_GlobalResources.Resource))]
            [UIHint("DateTime")]
    		public Nullable<System.DateTime>  ReceivedAt { get; set; }
    
    		    
    		/// <summary>
    		/// Vendor SKU
    		/// </summary>        
    	    [Display(Name = "PurchaseSKU_VendorSKU", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  VendorSKU { get; set; }
    
    		    
    		/// <summary>
    		/// Credit
    		/// </summary>        
    	    [Display(Name = "PurchaseSKU_Credit", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<decimal>  Credit { get; set; }
    
    		    
    		/// <summary>
    		/// CMCredit Note ID
    		/// </summary>        
    	    [Display(Name = "PurchaseSKU_CMCreditNoteID", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<int>  CMCreditNoteID { get; set; }
    
    		    
    		/// <summary>
    		/// CMReplacement ID
    		/// </summary>        
    	    [Display(Name = "PurchaseSKU_CMReplacementID", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<int>  CMReplacementID { get; set; }
    
    		    
    		/// <summary>
    		/// Credit QTY
    		/// </summary>        
    	    [Display(Name = "PurchaseSKU_CreditQTY", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<int>  CreditQTY { get; set; }
    
    		    
    		/// <summary>
    		/// QTYReturned
    		/// </summary>        
    	    [Display(Name = "PurchaseSKU_QTYReturned", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<int>  QTYReturned { get; set; }
    
    		    
    		/// <summary>
    		/// QTYReceived
    		/// </summary>        
    	    [Display(Name = "PurchaseSKU_QTYReceived", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<int>  QTYReceived { get; set; }
    
    		    
    	}
    }
    
}
