using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace inventorySKU.Models
{

    /// <summary>
    /// Warehouse class
    /// </summary>
    [MetadataType(typeof(WarehouseMetadata))]
    public  partial class Warehouse
    {
    
    	/// <summary>
    	/// Warehouse Metadata class
    	/// </summary>
    	public   class WarehouseMetadata
    	{
    		    
    		/// <summary>
    		/// Is Enable
    		/// </summary>        
    	    [Display(Name = "Warehouse_IsEnable", ResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
    		public bool  IsEnable { get; set; }
    
    		    
    		/// <summary>
    		/// Is Default
    		/// </summary>        
    	    [Display(Name = "Warehouse_IsDefault", ResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
    		public bool  IsDefault { get; set; }
    
    		    
    		/// <summary>
    		/// Is Sellable
    		/// </summary>        
    	    [Display(Name = "Warehouse_IsSellable", ResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
    		public bool  IsSellable { get; set; }
    
    		    
    		/// <summary>
    		/// ID
    		/// </summary>        
    	    [Display(Name = "Warehouse_ID", ResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
    		public int  ID { get; set; }
    
    		    
    		/// <summary>
    		/// Name
    		/// </summary>        
    	    [Display(Name = "Warehouse_Name", ResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
    		public string  Name { get; set; }
    
    		    
    		/// <summary>
    		/// Type
    		/// </summary>        
    	    [Display(Name = "Warehouse_Type", ResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
    		public int  Type { get; set; }
    
    		    
    		/// <summary>
    		/// Winit Warehouse
    		/// </summary>        
    	    [Display(Name = "Warehouse_WinitWarehouse", ResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
    		public string  WinitWarehouse { get; set; }
    
    		    
    		/// <summary>
    		/// Create By
    		/// </summary>        
    	    [Display(Name = "Warehouse_CreateBy", ResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
    		public string  CreateBy { get; set; }
    
    		    
    		/// <summary>
    		/// Create At
    		/// </summary>        
    	    [Display(Name = "Warehouse_CreateAt", ResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
            [UIHint("DateTime")]
            [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
    		public System.DateTime  CreateAt { get; set; }
    
    		    
    		/// <summary>
    		/// Update By
    		/// </summary>        
    	    [Display(Name = "Warehouse_UpdateBy", ResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
    		public string  UpdateBy { get; set; }
    
    		    
    		/// <summary>
    		/// Update At
    		/// </summary>        
    	    [Display(Name = "Warehouse_UpdateAt", ResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
            [UIHint("DateTime")]
    		public Nullable<System.DateTime>  UpdateAt { get; set; }
    
    		    
    	}
    }
    
}
