using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace inventorySKU.Models
{

    /// <summary>
    /// Company class
    /// </summary>
    [MetadataType(typeof(CompanyMetadata))]
    public  partial class Company
    {
    
    	/// <summary>
    	/// Company Metadata class
    	/// </summary>
    	public   class CompanyMetadata
    	{
    		    
    		/// <summary>
    		/// Is Enable
    		/// </summary>        
    	    [Display(Name = "Company_IsEnable", ResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
    		public bool  IsEnable { get; set; }
    
    		    
    		/// <summary>
    		/// ID
    		/// </summary>        
    	    [Display(Name = "Company_ID", ResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
    		public int  ID { get; set; }
    
    		    
    		/// <summary>
    		/// Name
    		/// </summary>        
    	    [Display(Name = "Company_Name", ResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
    		public string  Name { get; set; }
    
    		    
    		/// <summary>
    		/// Shandow Suffix
    		/// </summary>        
    	    [Display(Name = "Company_ShandowSuffix", ResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
            [MaxLength(10, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
    		public string  ShandowSuffix { get; set; }
    
    		    
    		/// <summary>
    		/// Parent ID
    		/// </summary>        
    	    [Display(Name = "Company_ParentID", ResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
    		public Nullable<int>  ParentID { get; set; }
    
    		    
    		/// <summary>
    		/// Relate ID
    		/// </summary>        
    	    [Display(Name = "Company_RelateID", ResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
    		public Nullable<int>  RelateID { get; set; }
    
    		    
    		/// <summary>
    		/// e Bay Account ID
    		/// </summary>        
    	    [Display(Name = "Company_eBayAccountID", ResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
    		public string  eBayAccountID { get; set; }
    
    		    
    		/// <summary>
    		/// Amazon Account ID
    		/// </summary>        
    	    [Display(Name = "Company_AmazonAccountID", ResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
    		public string  AmazonAccountID { get; set; }
    
    		    
    		/// <summary>
    		/// Create By
    		/// </summary>        
    	    [Display(Name = "Company_CreateBy", ResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
    		public string  CreateBy { get; set; }
    
    		    
    		/// <summary>
    		/// Create At
    		/// </summary>        
    	    [Display(Name = "Company_CreateAt", ResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
            [UIHint("DateTime")]
            [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
    		public System.DateTime  CreateAt { get; set; }
    
    		    
    		/// <summary>
    		/// Update By
    		/// </summary>        
    	    [Display(Name = "Company_UpdateBy", ResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
    		public string  UpdateBy { get; set; }
    
    		    
    		/// <summary>
    		/// Update At
    		/// </summary>        
    	    [Display(Name = "Company_UpdateAt", ResourceType = typeof(PurchaseOrderSys.App_GlobalResources.Resource))]
            [UIHint("DateTime")]
    		public Nullable<System.DateTime>  UpdateAt { get; set; }
    
    		    
    	}
    }
    
}