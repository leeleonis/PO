using System; 
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace PurchaseOrderSys.Models
{
    
    /// <summary>
    /// inventory class
    /// </summary>
    [MetadataType(typeof(inventoryMetadata))]
    public  partial class inventory
    {
    
    	/// <summary>
    	/// inventory Metadata class
    	/// </summary>
    	public   class inventoryMetadata
    	{
    		    
    		/// <summary>
    		/// Warehouse ID
    		/// </summary>        
    	    [Display(Name = "inventory_WarehouseID", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public int  WarehouseID { get; set; }
    
    		    
    		/// <summary>
    		/// Sku ID
    		/// </summary>        
    	    [Display(Name = "inventory_SkuID", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  SkuID { get; set; }
    
    		    
    		/// <summary>
    		/// Fulfillable
    		/// </summary>        
    	    [Display(Name = "inventory_Fulfillable", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public int  Fulfillable { get; set; }
    
    		    
    		/// <summary>
    		/// Awaiting
    		/// </summary>        
    	    [Display(Name = "inventory_Awaiting", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public int  Awaiting { get; set; }
    
    		    
    		/// <summary>
    		/// Unfulfillable Transit
    		/// </summary>        
    	    [Display(Name = "inventory_UnfulfillableTransit", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public int  UnfulfillableTransit { get; set; }
    
    		    
    		/// <summary>
    		/// Total Velocity
    		/// </summary>        
    	    [Display(Name = "inventory_TotalVelocity", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public int  TotalVelocity { get; set; }
    
    		    
    		/// <summary>
    		/// Aggregate
    		/// </summary>        
    	    [Display(Name = "inventory_Aggregate", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public int  Aggregate { get; set; }
    
    		    
    		/// <summary>
    		/// Transfer Out QTY
    		/// </summary>        
    	    [Display(Name = "inventory_TransferOutQTY", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public int  TransferOutQTY { get; set; }
    
    		    
    		/// <summary>
    		/// Transfer In QTY
    		/// </summary>        
    	    [Display(Name = "inventory_TransferInQTY", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public int  TransferInQTY { get; set; }
    
    		    
    		/// <summary>
    		/// WTransfer Out QTY
    		/// </summary>        
    	    [Display(Name = "inventory_WTransferOutQTY", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public int  WTransferOutQTY { get; set; }
    
    		    
    		/// <summary>
    		/// WTransfer In QTY
    		/// </summary>        
    	    [Display(Name = "inventory_WTransferInQTY", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public int  WTransferInQTY { get; set; }
    
    		    
    	}
    }
    
}
