using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace inventorySKU.Models
{

    /// <summary>
    /// Condition class
    /// </summary>
    [MetadataType(typeof(ConditionMetadata))]
    public  partial class Condition
    {
    
    	/// <summary>
    	/// Condition Metadata class
    	/// </summary>
    	public   class ConditionMetadata
    	{
    		    
    		/// <summary>
    		/// Is Enable
    		/// </summary>        
    	    [Display(Name = "Condition_IsEnable", ResourceType = typeof(ViewRes.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ViewRes.Resource))]
    		public bool  IsEnable { get; set; }
    
    		    
    		/// <summary>
    		/// ID
    		/// </summary>        
    	    [Display(Name = "Condition_ID", ResourceType = typeof(ViewRes.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ViewRes.Resource))]
    		public int  ID { get; set; }
    
    		    
    		/// <summary>
    		/// Amazon
    		/// </summary>        
    	    [Display(Name = "Condition_Amazon", ResourceType = typeof(ViewRes.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ViewRes.Resource))]
    		public string  Amazon { get; set; }
    
    		    
    		/// <summary>
    		/// e Bay
    		/// </summary>        
    	    [Display(Name = "Condition_eBay", ResourceType = typeof(ViewRes.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ViewRes.Resource))]
    		public string  eBay { get; set; }
    
    		    
    		/// <summary>
    		/// Buy_com
    		/// </summary>        
    	    [Display(Name = "Condition_Buy_com", ResourceType = typeof(ViewRes.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ViewRes.Resource))]
    		public string  Buy_com { get; set; }
    
    		    
    		/// <summary>
    		/// New Egg_com
    		/// </summary>        
    	    [Display(Name = "Condition_NewEgg_com", ResourceType = typeof(ViewRes.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ViewRes.Resource))]
    		public string  NewEgg_com { get; set; }
    
    		    
    		/// <summary>
    		/// Sears
    		/// </summary>        
    	    [Display(Name = "Condition_Sears", ResourceType = typeof(ViewRes.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ViewRes.Resource))]
    		public string  Sears { get; set; }
    
    		    
    		/// <summary>
    		/// Suffix
    		/// </summary>        
    	    [Display(Name = "Condition_Suffix", ResourceType = typeof(ViewRes.Resource))]
            [MaxLength(10, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ViewRes.Resource))]
    		public string  Suffix { get; set; }
    
    		    
    		/// <summary>
    		/// Create By
    		/// </summary>        
    	    [Display(Name = "Condition_CreateBy", ResourceType = typeof(ViewRes.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ViewRes.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ViewRes.Resource))]
    		public string  CreateBy { get; set; }
    
    		    
    		/// <summary>
    		/// Create At
    		/// </summary>        
    	    [Display(Name = "Condition_CreateAt", ResourceType = typeof(ViewRes.Resource))]
            [UIHint("DateTime")]
            [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ViewRes.Resource))]
    		public System.DateTime  CreateAt { get; set; }
    
    		    
    		/// <summary>
    		/// Update By
    		/// </summary>        
    	    [Display(Name = "Condition_UpdateBy", ResourceType = typeof(ViewRes.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ViewRes.Resource))]
    		public string  UpdateBy { get; set; }
    
    		    
    		/// <summary>
    		/// Update At
    		/// </summary>        
    	    [Display(Name = "Condition_UpdateAt", ResourceType = typeof(ViewRes.Resource))]
            [UIHint("DateTime")]
    		public Nullable<System.DateTime>  UpdateAt { get; set; }
    
    		    
    	}
    }
    
}
