using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace inventorySKU.Models
{

    /// <summary>
    /// SkuLang class
    /// </summary>
    [MetadataType(typeof(SkuLangMetadata))]
    public  partial class SkuLang
    {
    
    	/// <summary>
    	/// SkuLang Metadata class
    	/// </summary>
    	public   class SkuLangMetadata
    	{
    		    
    		/// <summary>
    		/// Sku
    		/// </summary>        
    	    [Display(Name = "SkuLang_Sku", ResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
    		public string  Sku { get; set; }
    
    		    
    		/// <summary>
    		/// Lang ID
    		/// </summary>        
    	    [Display(Name = "SkuLang_LangID", ResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
            [MaxLength(10, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
    		public string  LangID { get; set; }
    
    		    
    		/// <summary>
    		/// Name
    		/// </summary>        
    	    [Display(Name = "SkuLang_Name", ResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
            [MaxLength(100, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
    		public string  Name { get; set; }
    
    		    
    		/// <summary>
    		/// Models
    		/// </summary>        
    	    [Display(Name = "SkuLang_Models", ResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
    		public string  Models { get; set; }
    
    		    
    		/// <summary>
    		/// Create By
    		/// </summary>        
    	    [Display(Name = "SkuLang_CreateBy", ResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
    		public string  CreateBy { get; set; }
    
    		    
    		/// <summary>
    		/// Create At
    		/// </summary>        
    	    [Display(Name = "SkuLang_CreateAt", ResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
            [UIHint("DateTime")]
            [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
    		public System.DateTime  CreateAt { get; set; }
    
    		    
    		/// <summary>
    		/// Update By
    		/// </summary>        
    	    [Display(Name = "SkuLang_UpdateBy", ResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
    		public string  UpdateBy { get; set; }
    
    		    
    		/// <summary>
    		/// Update At
    		/// </summary>        
    	    [Display(Name = "SkuLang_UpdateAt", ResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
            [UIHint("DateTime")]
    		public Nullable<System.DateTime>  UpdateAt { get; set; }
    
    		    
    	}
    }
    
}
