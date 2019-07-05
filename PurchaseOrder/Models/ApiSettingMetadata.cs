using System; 
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace PurchaseOrderSys.Models
{
    
    /// <summary>
    /// ApiSetting class
    /// </summary>
    [MetadataType(typeof(ApiSettingMetadata))]
    public  partial class ApiSetting
    {
    
    	/// <summary>
    	/// ApiSetting Metadata class
    	/// </summary>
    	public   class ApiSettingMetadata
    	{
    		    
    		/// <summary>
    		/// Is Enable
    		/// </summary>        
    	    [Display(Name = "ApiSetting_IsEnable", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public bool  IsEnable { get; set; }
    
    		    
    		/// <summary>
    		/// Is Test
    		/// </summary>        
    	    [Display(Name = "ApiSetting_IsTest", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public bool  IsTest { get; set; }
    
    		    
    		/// <summary>
    		/// ID
    		/// </summary>        
    	    [Display(Name = "ApiSetting_ID", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public int  ID { get; set; }
    
    		    
    		/// <summary>
    		/// Name
    		/// </summary>        
    	    [Display(Name = "ApiSetting_Name", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  Name { get; set; }
    
    		    
    		/// <summary>
    		/// Type
    		/// </summary>        
    	    [Display(Name = "ApiSetting_Type", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<byte>  Type { get; set; }
    
    		    
    		/// <summary>
    		/// Account ID
    		/// </summary>        
    	    [Display(Name = "ApiSetting_AccountID", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  AccountID { get; set; }
    
    		    
    		/// <summary>
    		/// Api Key
    		/// </summary>        
    	    [Display(Name = "ApiSetting_ApiKey", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  ApiKey { get; set; }
    
    		    
    		/// <summary>
    		/// Api Password
    		/// </summary>        
    	    [Display(Name = "ApiSetting_ApiPassword", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  ApiPassword { get; set; }
    
    		    
    		/// <summary>
    		/// Api Account
    		/// </summary>        
    	    [Display(Name = "ApiSetting_ApiAccount", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  ApiAccount { get; set; }
    
    		    
    		/// <summary>
    		/// Api Meter
    		/// </summary>        
    	    [Display(Name = "ApiSetting_ApiMeter", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  ApiMeter { get; set; }
    
    		    
    		/// <summary>
    		/// Api Hub
    		/// </summary>        
    	    [Display(Name = "ApiSetting_ApiHub", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  ApiHub { get; set; }
    
    		    
    		/// <summary>
    		/// Update By
    		/// </summary>        
    	    [Display(Name = "ApiSetting_UpdateBy", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  UpdateBy { get; set; }
    
    		    
    		/// <summary>
    		/// Update At
    		/// </summary>        
    	    [Display(Name = "ApiSetting_UpdateAt", ResourceType = typeof(App_GlobalResources.Resource))]
            [UIHint("DateTime")]
    		public Nullable<System.DateTime>  UpdateAt { get; set; }
    
    		    
    		/// <summary>
    		/// Create By
    		/// </summary>        
    	    [Display(Name = "ApiSetting_CreateBy", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  CreateBy { get; set; }
    
    		    
    		/// <summary>
    		/// Create At
    		/// </summary>        
    	    [Display(Name = "ApiSetting_CreateAt", ResourceType = typeof(App_GlobalResources.Resource))]
            [UIHint("DateTime")]
            [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public System.DateTime  CreateAt { get; set; }
    
    		    
    	}
    }
    
}
