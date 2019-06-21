using System; 
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PurchaseOrderSys.Models
{
    
    /// <summary>
    /// OrderLog class
    /// </summary>
    [MetadataType(typeof(OrderLogMetadata))]
    public  partial class OrderLog
    {
    
    	/// <summary>
    	/// OrderLog Metadata class
    	/// </summary>
    	public   class OrderLogMetadata
    	{
    		    
    		/// <summary>
    		/// ID
    		/// </summary>        
    	    [Display(Name = "OrderLog_ID", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public int  ID { get; set; }
    
    		    
    		/// <summary>
    		/// SCID
    		/// </summary>        
    	    [Display(Name = "OrderLog_SCID", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(10, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  SCID { get; set; }
    
    		    
    		/// <summary>
    		/// Sku No
    		/// </summary>        
    	    [Display(Name = "OrderLog_SkuNo", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  SkuNo { get; set; }
    
    		    
    		/// <summary>
    		/// Qty
    		/// </summary>        
    	    [Display(Name = "OrderLog_Qty", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<int>  Qty { get; set; }
    
    		    
    		/// <summary>
    		/// State
    		/// </summary>        
    	    [Display(Name = "OrderLog_State", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  State { get; set; }
    
    		    
    		/// <summary>
    		/// Date
    		/// </summary>        
    	    [Display(Name = "OrderLog_Date", ResourceType = typeof(App_GlobalResources.Resource))]
            [UIHint("DateTime")]
    		public Nullable<System.DateTime>  Date { get; set; }
    
    		    
    		/// <summary>
    		/// Warehouse ID
    		/// </summary>        
    	    [Display(Name = "OrderLog_WarehouseID", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<int>  WarehouseID { get; set; }
    
    		    
    		/// <summary>
    		/// Order ID
    		/// </summary>        
    	    [Display(Name = "OrderLog_OrderID", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<int>  OrderID { get; set; }
    
    		    
    	}
    }
    
}
