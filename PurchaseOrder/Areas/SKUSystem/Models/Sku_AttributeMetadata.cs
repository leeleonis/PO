using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace inventorySKU.Models
{

    /// <summary>
    /// Sku_Attribute class
    /// </summary>
    [MetadataType(typeof(Sku_AttributeMetadata))]
    public  partial class Sku_Attribute
    {
    
    	/// <summary>
    	/// Sku_Attribute Metadata class
    	/// </summary>
    	public   class Sku_AttributeMetadata
    	{
    		    
    		/// <summary>
    		/// Sku
    		/// </summary>        
    	    [Display(Name = "Sku_Attribute_Sku", ResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
    		public string  Sku { get; set; }
    
    		    
    		/// <summary>
    		/// Attr ID
    		/// </summary>        
    	    [Display(Name = "Sku_Attribute_AttrID", ResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
    		public int  AttrID { get; set; }
    
    		    
    		/// <summary>
    		/// Lang ID
    		/// </summary>        
    	    [Display(Name = "Sku_Attribute_LangID", ResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
            [MaxLength(10, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
    		public string  LangID { get; set; }
    
    		    
    		/// <summary>
    		/// Value
    		/// </summary>        
    	    [Display(Name = "Sku_Attribute_Value", ResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
    		public string  Value { get; set; }
    
    		    
    		/// <summary>
    		/// Create By
    		/// </summary>        
    	    [Display(Name = "Sku_Attribute_CreateBy", ResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
    		public string  CreateBy { get; set; }
    
    		    
    		/// <summary>
    		/// Create At
    		/// </summary>        
    	    [Display(Name = "Sku_Attribute_CreateAt", ResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
            [UIHint("DateTime")]
            [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
    		public System.DateTime  CreateAt { get; set; }
    
    		    
    		/// <summary>
    		/// Update By
    		/// </summary>        
    	    [Display(Name = "Sku_Attribute_UpdateBy", ResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
    		public string  UpdateBy { get; set; }
    
    		    
    		/// <summary>
    		/// Update At
    		/// </summary>        
    	    [Display(Name = "Sku_Attribute_UpdateAt", ResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
            [UIHint("DateTime")]
    		public Nullable<System.DateTime>  UpdateAt { get; set; }
    
    		    
    	}
    }
    
}
