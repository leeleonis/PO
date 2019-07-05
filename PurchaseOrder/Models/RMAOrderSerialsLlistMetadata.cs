using System; 
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace PurchaseOrderSys.Models
{
    
    /// <summary>
    /// RMAOrderSerialsLlist class
    /// </summary>
    [MetadataType(typeof(RMAOrderSerialsLlistMetadata))]
    public  partial class RMAOrderSerialsLlist
    {
    
    	/// <summary>
    	/// RMAOrderSerialsLlist Metadata class
    	/// </summary>
    	public   class RMAOrderSerialsLlistMetadata
    	{
    		    
    		/// <summary>
    		/// Is Enable
    		/// </summary>        
    	    [Display(Name = "RMAOrderSerialsLlist_IsEnable", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public bool  IsEnable { get; set; }
    
    		    
    		/// <summary>
    		/// ID
    		/// </summary>        
    	    [Display(Name = "RMAOrderSerialsLlist_ID", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public int  ID { get; set; }
    
    		    
    		/// <summary>
    		/// RMASKUID
    		/// </summary>        
    	    [Display(Name = "RMAOrderSerialsLlist_RMASKUID", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<int>  RMASKUID { get; set; }
    
    		    
    		/// <summary>
    		/// Serials No
    		/// </summary>        
    	    [Display(Name = "RMAOrderSerialsLlist_SerialsNo", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  SerialsNo { get; set; }
    
    		    
    		/// <summary>
    		/// Serials QTY
    		/// </summary>        
    	    [Display(Name = "RMAOrderSerialsLlist_SerialsQTY", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<int>  SerialsQTY { get; set; }
    
    		    
    		/// <summary>
    		/// Create By
    		/// </summary>        
    	    [Display(Name = "RMAOrderSerialsLlist_CreateBy", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  CreateBy { get; set; }
    
    		    
    		/// <summary>
    		/// Create At
    		/// </summary>        
    	    [Display(Name = "RMAOrderSerialsLlist_CreateAt", ResourceType = typeof(App_GlobalResources.Resource))]
            [UIHint("DateTime")]
            [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public System.DateTime  CreateAt { get; set; }
    
    		    
    	}
    }
    
}
