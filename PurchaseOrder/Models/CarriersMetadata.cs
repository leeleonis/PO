using System; 
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace PurchaseOrderSys.Models
{
    
    /// <summary>
    /// Carriers class
    /// </summary>
    [MetadataType(typeof(CarriersMetadata))]
    public  partial class Carriers
    {
    
    	/// <summary>
    	/// Carriers Metadata class
    	/// </summary>
    	public   class CarriersMetadata
    	{
    		    
    		/// <summary>
    		/// Is Enable
    		/// </summary>        
    	    [Display(Name = "Carriers_IsEnable", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public bool  IsEnable { get; set; }
    
    		    
    		/// <summary>
    		/// ID
    		/// </summary>        
    	    [Display(Name = "Carriers_ID", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public int  ID { get; set; }
    
    		    
    		/// <summary>
    		/// Name
    		/// </summary>        
    	    [Display(Name = "Carriers_Name", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  Name { get; set; }
    
    		    
    		/// <summary>
    		/// Email
    		/// </summary>        
    	    [Display(Name = "Carriers_Email", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(150, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  Email { get; set; }
    
    		    
    		/// <summary>
    		/// Api
    		/// </summary>        
    	    [Display(Name = "Carriers_Api", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<int>  Api { get; set; }
    
    		    
    	}
    }
    
}
