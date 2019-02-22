using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace PurchaseOrderSys.Models
{

    /// <summary>
    /// PriceGroup class
    /// </summary>
    [MetadataType(typeof(PriceGroupMetadata))]
    public  partial class PriceGroup
    {
    
    	/// <summary>
    	/// PriceGroup Metadata class
    	/// </summary>
    	public   class PriceGroupMetadata
    	{
    		    
    		/// <summary>
    		/// Is Used
    		/// </summary>        
    	    [Display(Name = "PriceGroup_IsUsed", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public bool  IsUsed { get; set; }
    
    		    
    		/// <summary>
    		/// ID
    		/// </summary>        
    	    [Display(Name = "PriceGroup_ID", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public int  ID { get; set; }
    
    		    
    		/// <summary>
    		/// Sku
    		/// </summary>        
    	    [Display(Name = "PriceGroup_Sku", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  Sku { get; set; }
    
    		    
    		/// <summary>
    		/// Item ID
    		/// </summary>        
    	    [Display(Name = "PriceGroup_ItemID", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  ItemID { get; set; }
    
    		    
    		/// <summary>
    		/// Marketplace
    		/// </summary>        
    	    [Display(Name = "PriceGroup_Marketplace", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<int>  Marketplace { get; set; }
    
    		    
    		/// <summary>
    		/// Price
    		/// </summary>        
    	    [Display(Name = "PriceGroup_Price", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public decimal  Price { get; set; }
    
    		    
    		/// <summary>
    		/// Max
    		/// </summary>        
    	    [Display(Name = "PriceGroup_Max", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public decimal  Max { get; set; }
    
    		    
    		/// <summary>
    		/// Min
    		/// </summary>        
    	    [Display(Name = "PriceGroup_Min", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public decimal  Min { get; set; }
    
    		    
    		/// <summary>
    		/// Create By
    		/// </summary>        
    	    [Display(Name = "PriceGroup_CreateBy", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  CreateBy { get; set; }
    
    		    
    		/// <summary>
    		/// Create At
    		/// </summary>        
    	    [Display(Name = "PriceGroup_CreateAt", ResourceType = typeof(App_GlobalResources.Resource))]
            [UIHint("DateTime")]
            [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public System.DateTime  CreateAt { get; set; }
    
    		    
    		/// <summary>
    		/// Update By
    		/// </summary>        
    	    [Display(Name = "PriceGroup_UpdateBy", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  UpdateBy { get; set; }
    
    		    
    		/// <summary>
    		/// Update At
    		/// </summary>        
    	    [Display(Name = "PriceGroup_UpdateAt", ResourceType = typeof(App_GlobalResources.Resource))]
            [UIHint("DateTime")]
    		public Nullable<System.DateTime>  UpdateAt { get; set; }
    
    		    
    	}
    }
    
}
