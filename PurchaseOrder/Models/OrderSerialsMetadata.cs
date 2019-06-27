using System; 
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace PurchaseOrderSys.Models
{
    
    /// <summary>
    /// OrderSerials class
    /// </summary>
    [MetadataType(typeof(OrderSerialsMetadata))]
    public  partial class OrderSerials
    {
    
    	/// <summary>
    	/// OrderSerials Metadata class
    	/// </summary>
    	public   class OrderSerialsMetadata
    	{
    		    
    		/// <summary>
    		/// Is Enable
    		/// </summary>        
    	    [Display(Name = "OrderSerials_IsEnable", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public bool  IsEnable { get; set; }
    
    		    
    		/// <summary>
    		/// Serial Number
    		/// </summary>        
    	    [Display(Name = "OrderSerials_SerialNumber", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  SerialNumber { get; set; }
    
    		    
    		/// <summary>
    		/// Order ID
    		/// </summary>        
    	    [Display(Name = "OrderSerials_OrderID", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public int  OrderID { get; set; }
    
    		    
    		/// <summary>
    		/// Item ID
    		/// </summary>        
    	    [Display(Name = "OrderSerials_ItemID", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public int  ItemID { get; set; }
    
    		    
    		/// <summary>
    		/// Sku
    		/// </summary>        
    	    [Display(Name = "OrderSerials_Sku", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  Sku { get; set; }
    
    		    
    		/// <summary>
    		/// Update_by
    		/// </summary>        
    	    [Display(Name = "OrderSerials_Update_by", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  Update_by { get; set; }
    
    		    
    		/// <summary>
    		/// Update_at
    		/// </summary>        
    	    [Display(Name = "OrderSerials_Update_at", ResourceType = typeof(App_GlobalResources.Resource))]
            [UIHint("DateTime")]
    		public Nullable<System.DateTime>  Update_at { get; set; }
    
    		    
    		/// <summary>
    		/// Create_by
    		/// </summary>        
    	    [Display(Name = "OrderSerials_Create_by", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  Create_by { get; set; }
    
    		    
    		/// <summary>
    		/// Create_at
    		/// </summary>        
    	    [Display(Name = "OrderSerials_Create_at", ResourceType = typeof(App_GlobalResources.Resource))]
            [UIHint("DateTime")]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public System.DateTime  Create_at { get; set; }
    
    		    
    	}
    }
    
}
