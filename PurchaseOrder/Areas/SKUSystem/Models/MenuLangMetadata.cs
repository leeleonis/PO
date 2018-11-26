using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace inventorySKU.Models
{

    /// <summary>
    /// MenuLang class
    /// </summary>
    [MetadataType(typeof(MenuLangMetadata))]
    public  partial class MenuLang
    {
    
    	/// <summary>
    	/// MenuLang Metadata class
    	/// </summary>
    	public   class MenuLangMetadata
    	{
    		    
    		/// <summary>
    		/// Menu ID
    		/// </summary>        
    	    [Display(Name = "MenuLang_MenuID", ResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
    		public int  MenuID { get; set; }
    
    		    
    		/// <summary>
    		/// Lang ID
    		/// </summary>        
    	    [Display(Name = "MenuLang_LangID", ResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
            [MaxLength(10, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
    		public string  LangID { get; set; }
    
    		    
    		/// <summary>
    		/// Name
    		/// </summary>        
    	    [Display(Name = "MenuLang_Name", ResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
    		public string  Name { get; set; }
    
    		    
    		/// <summary>
    		/// Create By
    		/// </summary>        
    	    [Display(Name = "MenuLang_CreateBy", ResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
    		public string  CreateBy { get; set; }
    
    		    
    		/// <summary>
    		/// Create At
    		/// </summary>        
    	    [Display(Name = "MenuLang_CreateAt", ResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
            [UIHint("DateTime")]
            [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
    		public System.DateTime  CreateAt { get; set; }
    
    		    
    		/// <summary>
    		/// Update By
    		/// </summary>        
    	    [Display(Name = "MenuLang_UpdateBy", ResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
    		public string  UpdateBy { get; set; }
    
    		    
    		/// <summary>
    		/// Update At
    		/// </summary>        
    	    [Display(Name = "MenuLang_UpdateAt", ResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
            [UIHint("DateTime")]
    		public Nullable<System.DateTime>  UpdateAt { get; set; }
    
    		    
    	}
    }
    
}
