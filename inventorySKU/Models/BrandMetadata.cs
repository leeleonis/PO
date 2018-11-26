using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace inventorySKU.Models
{

    /// <summary>
    /// Brand class
    /// </summary>
    [MetadataType(typeof(BrandMetadata))]
    public  partial class Brand
    {
    
    	/// <summary>
    	/// Brand Metadata class
    	/// </summary>
    	public   class BrandMetadata
    	{
    		    
    		/// <summary>
    		/// Is Enable
    		/// </summary>        
    	    [Display(Name = "Brand_IsEnable", ResourceType = typeof(ViewRes.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ViewRes.Resource))]
    		public bool  IsEnable { get; set; }
    
    		    
    		/// <summary>
    		/// ID
    		/// </summary>        
    	    [Display(Name = "Brand_ID", ResourceType = typeof(ViewRes.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ViewRes.Resource))]
    		public int  ID { get; set; }
    
    		    
    		/// <summary>
    		/// Neto ID
    		/// </summary>        
    	    [Display(Name = "Brand_NetoID", ResourceType = typeof(ViewRes.Resource))]
    		public Nullable<int>  NetoID { get; set; }
    
    		    
    		/// <summary>
    		/// Name
    		/// </summary>        
    	    [Display(Name = "Brand_Name", ResourceType = typeof(ViewRes.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ViewRes.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ViewRes.Resource))]
    		public string  Name { get; set; }
    
    		    
    		/// <summary>
    		/// Is Export
    		/// </summary>        
    	    [Display(Name = "Brand_IsExport", ResourceType = typeof(ViewRes.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ViewRes.Resource))]
    		public bool  IsExport { get; set; }
    
    		    
    		/// <summary>
    		/// Create By
    		/// </summary>        
    	    [Display(Name = "Brand_CreateBy", ResourceType = typeof(ViewRes.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ViewRes.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ViewRes.Resource))]
    		public string  CreateBy { get; set; }
    
    		    
    		/// <summary>
    		/// Create At
    		/// </summary>        
    	    [Display(Name = "Brand_CreateAt", ResourceType = typeof(ViewRes.Resource))]
            [UIHint("DateTime")]
            [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ViewRes.Resource))]
    		public System.DateTime  CreateAt { get; set; }
    
    		    
    		/// <summary>
    		/// Update By
    		/// </summary>        
    	    [Display(Name = "Brand_UpdateBy", ResourceType = typeof(ViewRes.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ViewRes.Resource))]
    		public string  UpdateBy { get; set; }
    
    		    
    		/// <summary>
    		/// Update At
    		/// </summary>        
    	    [Display(Name = "Brand_UpdateAt", ResourceType = typeof(ViewRes.Resource))]
            [UIHint("DateTime")]
    		public Nullable<System.DateTime>  UpdateAt { get; set; }
    
    		    
    	}
    }
    
}
