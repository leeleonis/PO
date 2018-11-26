using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace inventorySKU.Models
{

    /// <summary>
    /// AdminUser class
    /// </summary>
    [MetadataType(typeof(AdminUserMetadata))]
    public  partial class AdminUser
    {
    
    	/// <summary>
    	/// AdminUser Metadata class
    	/// </summary>
    	public   class AdminUserMetadata
    	{
    		    
    		/// <summary>
    		/// Is Enable
    		/// </summary>        
    	    [Display(Name = "AdminUser_IsEnable", ResourceType = typeof(ViewRes.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ViewRes.Resource))]
    		public bool  IsEnable { get; set; }
    
    		    
    		/// <summary>
    		/// ID
    		/// </summary>        
    	    [Display(Name = "AdminUser_ID", ResourceType = typeof(ViewRes.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ViewRes.Resource))]
    		public int  ID { get; set; }
    
    		    
    		/// <summary>
    		/// Group
    		/// </summary>        
    	    [Display(Name = "AdminUser_Group", ResourceType = typeof(ViewRes.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ViewRes.Resource))]
    		public int  Group { get; set; }
    
    		    
    		/// <summary>
    		/// Name
    		/// </summary>        
    	    [Display(Name = "AdminUser_Name", ResourceType = typeof(ViewRes.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ViewRes.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ViewRes.Resource))]
    		public string  Name { get; set; }
    
    		    
    		/// <summary>
    		/// Account
    		/// </summary>        
    	    [Display(Name = "AdminUser_Account", ResourceType = typeof(ViewRes.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ViewRes.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ViewRes.Resource))]
    		public string  Account { get; set; }
    
    		    
    		/// <summary>
    		/// Password
    		/// </summary>        
    	    [Display(Name = "AdminUser_Password", ResourceType = typeof(ViewRes.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ViewRes.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ViewRes.Resource))]
    		public string  Password { get; set; }
    
    		    
    		/// <summary>
    		/// Api User Name
    		/// </summary>        
    	    [Display(Name = "AdminUser_ApiUserName", ResourceType = typeof(ViewRes.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ViewRes.Resource))]
    		public string  ApiUserName { get; set; }
    
    		    
    		/// <summary>
    		/// Api Password
    		/// </summary>        
    	    [Display(Name = "AdminUser_ApiPassword", ResourceType = typeof(ViewRes.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ViewRes.Resource))]
    		public string  ApiPassword { get; set; }
    
    		    
    		/// <summary>
    		/// Time Zone
    		/// </summary>        
    	    [Display(Name = "AdminUser_TimeZone", ResourceType = typeof(ViewRes.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ViewRes.Resource))]
    		public int  TimeZone { get; set; }
    
    		    
    		/// <summary>
    		/// Allow Warehouse
    		/// </summary>        
    	    [Display(Name = "AdminUser_AllowWarehouse", ResourceType = typeof(ViewRes.Resource))]
    		public string  AllowWarehouse { get; set; }
    
    		    
    		/// <summary>
    		/// Auth
    		/// </summary>        
    	    [Display(Name = "AdminUser_Auth", ResourceType = typeof(ViewRes.Resource))]
    		public string  Auth { get; set; }
    
    		    
    		/// <summary>
    		/// Create By
    		/// </summary>        
    	    [Display(Name = "AdminUser_CreateBy", ResourceType = typeof(ViewRes.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ViewRes.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ViewRes.Resource))]
    		public string  CreateBy { get; set; }
    
    		    
    		/// <summary>
    		/// Create At
    		/// </summary>        
    	    [Display(Name = "AdminUser_CreateAt", ResourceType = typeof(ViewRes.Resource))]
            [UIHint("DateTime")]
            [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ViewRes.Resource))]
    		public System.DateTime  CreateAt { get; set; }
    
    		    
    		/// <summary>
    		/// Update By
    		/// </summary>        
    	    [Display(Name = "AdminUser_UpdateBy", ResourceType = typeof(ViewRes.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ViewRes.Resource))]
    		public string  UpdateBy { get; set; }
    
    		    
    		/// <summary>
    		/// Update At
    		/// </summary>        
    	    [Display(Name = "AdminUser_UpdateAt", ResourceType = typeof(ViewRes.Resource))]
            [UIHint("DateTime")]
    		public Nullable<System.DateTime>  UpdateAt { get; set; }
    
    		    
    	}
    }
    
}
