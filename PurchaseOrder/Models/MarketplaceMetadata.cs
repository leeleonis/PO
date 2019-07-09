using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace PurchaseOrderSys.Models
{

    /// <summary>
    /// Marketplace class
    /// </summary>
    [MetadataType(typeof(MarketplaceMetadata))]
    public  partial class Marketplace
    {
    
    	/// <summary>
    	/// Marketplace Metadata class
    	/// </summary>
    	public   class MarketplaceMetadata
    	{
    		    
    		/// <summary>
    		/// Is Enable
    		/// </summary>        
    	    [Display(Name = "Marketplace_IsEnable", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public bool  IsEnable { get; set; }
    
    		    
    		/// <summary>
    		/// ID
    		/// </summary>        
    	    [Display(Name = "Marketplace_ID", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public int  ID { get; set; }
    
    		    
    		/// <summary>
    		/// Full Name
    		/// </summary>        
    	    [Display(Name = "Marketplace_FullName", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  FullName { get; set; }
    
    		    
    		/// <summary>
    		/// Global ID
    		/// </summary>        
    	    [Display(Name = "Marketplace_GlobalID", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  GlobalID { get; set; }
    
    		    
    		/// <summary>
    		/// Country Code
    		/// </summary>        
    	    [Display(Name = "Marketplace_CountryCode", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(4, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  CountryCode { get; set; }
    
    		    
    		/// <summary>
    		/// Company ID
    		/// </summary>        
    	    [Display(Name = "Marketplace_CompanyID", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<int>  CompanyID { get; set; }
    
    		    
    		/// <summary>
    		/// Currency ID
    		/// </summary>        
    	    [Display(Name = "Marketplace_CurrencyID", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<int>  CurrencyID { get; set; }
    
    		    
    		/// <summary>
    		/// Neto Group
    		/// </summary>        
    	    [Display(Name = "Marketplace_NetoGroup", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<int>  NetoGroup { get; set; }
    
    		    
    		/// <summary>
    		/// Status
    		/// </summary>        
    	    [Display(Name = "Marketplace_Status", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<bool>  Status { get; set; }
    
    		    
    		/// <summary>
    		/// Create By
    		/// </summary>        
    	    [Display(Name = "Marketplace_CreateBy", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  CreateBy { get; set; }
    
    		    
    		/// <summary>
    		/// Create At
    		/// </summary>        
    	    [Display(Name = "Marketplace_CreateAt", ResourceType = typeof(App_GlobalResources.Resource))]
            [UIHint("DateTime")]
            [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public System.DateTime  CreateAt { get; set; }
    
    		    
    		/// <summary>
    		/// Update By
    		/// </summary>        
    	    [Display(Name = "Marketplace_UpdateBy", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  UpdateBy { get; set; }
    
    		    
    		/// <summary>
    		/// Update At
    		/// </summary>        
    	    [Display(Name = "Marketplace_UpdateAt", ResourceType = typeof(App_GlobalResources.Resource))]
            [UIHint("DateTime")]
    		public Nullable<System.DateTime>  UpdateAt { get; set; }
    
    		    
    	}
    }
    
}
