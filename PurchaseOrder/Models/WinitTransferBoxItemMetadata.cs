using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace PurchaseOrderSys.Models
{

    /// <summary>
    /// WinitTransferBoxItem class
    /// </summary>
    [MetadataType(typeof(WinitTransferBoxItemMetadata))]
    [Serializable]
    public  partial class WinitTransferBoxItem
    {
    
    	/// <summary>
    	/// WinitTransferBoxItem Metadata class
    	/// </summary>
    	public   class WinitTransferBoxItemMetadata
    	{
    		    
    		/// <summary>
    		/// Winit Transfer Box ID
    		/// </summary>        
    	    [Display(Name = "WinitTransferBoxItem_WinitTransferBoxID", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public int  WinitTransferBoxID { get; set; }
    
    		    
    		/// <summary>
    		/// Serials Llist ID
    		/// </summary>        
    	    [Display(Name = "WinitTransferBoxItem_SerialsLlistID", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public int  SerialsLlistID { get; set; }
    
    		    
    		/// <summary>
    		/// Bar Code
    		/// </summary>        
    	    [Display(Name = "WinitTransferBoxItem_BarCode", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  BarCode { get; set; }
    
    		    
    		/// <summary>
    		/// Create By
    		/// </summary>        
    	    [Display(Name = "WinitTransferBoxItem_CreateBy", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  CreateBy { get; set; }
    
    		    
    		/// <summary>
    		/// Create At
    		/// </summary>        
    	    [Display(Name = "WinitTransferBoxItem_CreateAt", ResourceType = typeof(App_GlobalResources.Resource))]
            [UIHint("DateTime")]
            [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public System.DateTime  CreateAt { get; set; }
    
    		    
    		/// <summary>
    		/// Update By
    		/// </summary>        
    	    [Display(Name = "WinitTransferBoxItem_UpdateBy", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  UpdateBy { get; set; }
    
    		    
    		/// <summary>
    		/// Update At
    		/// </summary>        
    	    [Display(Name = "WinitTransferBoxItem_UpdateAt", ResourceType = typeof(App_GlobalResources.Resource))]
            [UIHint("DateTime")]
    		public Nullable<System.DateTime>  UpdateAt { get; set; }
    
    		    
    		/// <summary>
    		/// Value
    		/// </summary>        
    	    [Display(Name = "WinitTransferBoxItem_Value", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<decimal>  Value { get; set; }
    
    		    
    	}
    }
    
}
