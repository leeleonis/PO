using System; 
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using System; 
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using System; 
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using System; 
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace inventorySKU.Models
{

    /// <summary>
    /// DirectLine class
    /// </summary>
    [MetadataType(typeof(DirectLineMetadata))]
    public  partial class DirectLine
    {
    
    	/// <summary>
    	/// DirectLine Metadata class
    	/// </summary>
    	public   class DirectLineMetadata
    	{
    		    
    		/// <summary>
    		/// Is Enable
    		/// </summary>        
    	    [Display(Name = "DirectLine_IsEnable", ResourceType = typeof(ViewRes.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ViewRes.Resource))]
    		public bool  IsEnable { get; set; }
    
    		    
    		/// <summary>
    		/// ID
    		/// </summary>        
    	    [Display(Name = "DirectLine_ID", ResourceType = typeof(ViewRes.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ViewRes.Resource))]
    		public int  ID { get; set; }
    
    		    
    		/// <summary>
    		/// Name
    		/// </summary>        
    	    [Display(Name = "DirectLine_Name", ResourceType = typeof(ViewRes.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ViewRes.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ViewRes.Resource))]
    		public string  Name { get; set; }
    
    		    
    		/// <summary>
    		/// Abbreviation
    		/// </summary>        
    	    [Display(Name = "DirectLine_Abbreviation", ResourceType = typeof(ViewRes.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ViewRes.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ViewRes.Resource))]
    		public string  Abbreviation { get; set; }
    
    		    
    		/// <summary>
    		/// Email
    		/// </summary>        
    	    [Display(Name = "DirectLine_Email", ResourceType = typeof(ViewRes.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ViewRes.Resource))]
    		public string  Email { get; set; }
    
    		    
    		/// <summary>
    		/// Contact Name
    		/// </summary>        
    	    [Display(Name = "DirectLine_ContactName", ResourceType = typeof(ViewRes.Resource))]
            [MaxLength(100, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ViewRes.Resource))]
    		public string  ContactName { get; set; }
    
    		    
    		/// <summary>
    		/// Company Name
    		/// </summary>        
    	    [Display(Name = "DirectLine_CompanyName", ResourceType = typeof(ViewRes.Resource))]
            [MaxLength(100, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ViewRes.Resource))]
    		public string  CompanyName { get; set; }
    
    		    
    		/// <summary>
    		/// Phone Number
    		/// </summary>        
    	    [Display(Name = "DirectLine_PhoneNumber", ResourceType = typeof(ViewRes.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ViewRes.Resource))]
    		public string  PhoneNumber { get; set; }
    
    		    
    		/// <summary>
    		/// Country Name
    		/// </summary>        
    	    [Display(Name = "DirectLine_CountryName", ResourceType = typeof(ViewRes.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ViewRes.Resource))]
    		public string  CountryName { get; set; }
    
    		    
    		/// <summary>
    		/// Country Code
    		/// </summary>        
    	    [Display(Name = "DirectLine_CountryCode", ResourceType = typeof(ViewRes.Resource))]
            [MaxLength(3, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ViewRes.Resource))]
    		public string  CountryCode { get; set; }
    
    		    
    		/// <summary>
    		/// City
    		/// </summary>        
    	    [Display(Name = "DirectLine_City", ResourceType = typeof(ViewRes.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ViewRes.Resource))]
    		public string  City { get; set; }
    
    		    
    		/// <summary>
    		/// Street Line1
    		/// </summary>        
    	    [Display(Name = "DirectLine_StreetLine1", ResourceType = typeof(ViewRes.Resource))]
    		public string  StreetLine1 { get; set; }
    
    		    
    		/// <summary>
    		/// Street Line2
    		/// </summary>        
    	    [Display(Name = "DirectLine_StreetLine2", ResourceType = typeof(ViewRes.Resource))]
    		public string  StreetLine2 { get; set; }
    
    		    
    		/// <summary>
    		/// State Name
    		/// </summary>        
    	    [Display(Name = "DirectLine_StateName", ResourceType = typeof(ViewRes.Resource))]
            [MaxLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ViewRes.Resource))]
    		public string  StateName { get; set; }
    
    		    
    		/// <summary>
    		/// Postal Code
    		/// </summary>        
    	    [Display(Name = "DirectLine_PostalCode", ResourceType = typeof(ViewRes.Resource))]
            [MaxLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ViewRes.Resource))]
    		public string  PostalCode { get; set; }
    
    		    
    		/// <summary>
    		/// Create By
    		/// </summary>        
    	    [Display(Name = "DirectLine_CreateBy", ResourceType = typeof(ViewRes.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ViewRes.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ViewRes.Resource))]
    		public string  CreateBy { get; set; }
    
    		    
    		/// <summary>
    		/// Create At
    		/// </summary>        
    	    [Display(Name = "DirectLine_CreateAt", ResourceType = typeof(ViewRes.Resource))]
            [UIHint("DateTime")]
            [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ViewRes.Resource))]
    		public System.DateTime  CreateAt { get; set; }
    
    		    
    		/// <summary>
    		/// Update By
    		/// </summary>        
    	    [Display(Name = "DirectLine_UpdateBy", ResourceType = typeof(ViewRes.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ViewRes.Resource))]
    		public string  UpdateBy { get; set; }
    
    		    
    		/// <summary>
    		/// Update At
    		/// </summary>        
    	    [Display(Name = "DirectLine_UpdateAt", ResourceType = typeof(ViewRes.Resource))]
            [UIHint("DateTime")]
    		public Nullable<System.DateTime>  UpdateAt { get; set; }
    
    		    
    	}
    }
    
}
