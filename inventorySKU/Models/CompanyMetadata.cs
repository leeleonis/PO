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
    	    [Display(Name = "Company_IsEnable", ResourceType = typeof(ViewRes.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ViewRes.Resource))]
    		public bool  IsEnable { get; set; }
    
    		    
    		/// <summary>
    		/// ID
    		/// </summary>        
    	    [Display(Name = "Company_ID", ResourceType = typeof(ViewRes.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ViewRes.Resource))]
    		public int  ID { get; set; }
    
    		    
    		/// <summary>
    		/// Name
    		/// </summary>        
    	    [Display(Name = "Company_Name", ResourceType = typeof(ViewRes.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ViewRes.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ViewRes.Resource))]
    		public string  Name { get; set; }
    
    		    
    		/// <summary>
    		/// Shandow Suffix
    		/// </summary>        
    	    [Display(Name = "Company_ShandowSuffix", ResourceType = typeof(ViewRes.Resource))]
            [MaxLength(10, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ViewRes.Resource))]
    		public string  ShandowSuffix { get; set; }
    
    		    
    		/// <summary>
    		/// Parent ID
    		/// </summary>        
    	    [Display(Name = "Company_ParentID", ResourceType = typeof(ViewRes.Resource))]
    		public Nullable<int>  ParentID { get; set; }
    
    		    
    		/// <summary>
    		/// Relate ID
    		/// </summary>        
    	    [Display(Name = "Company_RelateID", ResourceType = typeof(ViewRes.Resource))]
    		public Nullable<int>  RelateID { get; set; }
    
    		    
    		/// <summary>
    		/// e Bay Account ID
    		/// </summary>        
    	    [Display(Name = "Company_eBayAccountID", ResourceType = typeof(ViewRes.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ViewRes.Resource))]
    		public string  eBayAccountID { get; set; }
    
    		    
    		/// <summary>
    		/// Amazon Account ID
    		/// </summary>        
    	    [Display(Name = "Company_AmazonAccountID", ResourceType = typeof(ViewRes.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ViewRes.Resource))]
    		public string  AmazonAccountID { get; set; }
    
    		    
    		/// <summary>
    		/// Create By
    		/// </summary>        
    	    [Display(Name = "Company_CreateBy", ResourceType = typeof(ViewRes.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ViewRes.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ViewRes.Resource))]
    		public string  CreateBy { get; set; }
    
    		    
    		/// <summary>
    		/// Create At
    		/// </summary>        
    	    [Display(Name = "Company_CreateAt", ResourceType = typeof(ViewRes.Resource))]
            [UIHint("DateTime")]
            [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ViewRes.Resource))]
    		public System.DateTime  CreateAt { get; set; }
    
    		    
    		/// <summary>
    		/// Update By
    		/// </summary>        
    	    [Display(Name = "Company_UpdateBy", ResourceType = typeof(ViewRes.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ViewRes.Resource))]
    		public string  UpdateBy { get; set; }
    
    		    
    		/// <summary>
    		/// Update At
    		/// </summary>        
    	    [Display(Name = "Company_UpdateAt", ResourceType = typeof(ViewRes.Resource))]
            [UIHint("DateTime")]
    		public Nullable<System.DateTime>  UpdateAt { get; set; }
    
    		    
    	}
    }
    
}
