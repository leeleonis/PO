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


namespace PurchaseOrderSys.Models
{
    
    /// <summary>
    /// Menu class
    /// </summary>
    [MetadataType(typeof(MenuMetadata))]
    public  partial class Menu
    {
    
    	/// <summary>
    	/// Menu Metadata class
    	/// </summary>
    	public   class MenuMetadata
    	{
    		    
    		/// <summary>
    		/// Is Enable
    		/// </summary>        
    	    [Display(Name = "Menu_IsEnable", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public bool  IsEnable { get; set; }
    
    		    
    		/// <summary>
    		/// Menu ID
    		/// </summary>        
    	    [Display(Name = "Menu_MenuID", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public int  MenuID { get; set; }
    
    		    
    		/// <summary>
    		/// Name
    		/// </summary>        
    	    [Display(Name = "Menu_Name", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  Name { get; set; }
    
    		    
    		/// <summary>
    		/// Prev ID
    		/// </summary>        
    	    [Display(Name = "Menu_PrevID", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<int>  PrevID { get; set; }
    
    		    
    		/// <summary>
    		/// Controller
    		/// </summary>        
    	    [Display(Name = "Menu_Controller", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  Controller { get; set; }
    
    		    
    		/// <summary>
    		/// Action
    		/// </summary>        
    	    [Display(Name = "Menu_Action", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  Action { get; set; }
    
    		    
    		/// <summary>
    		/// Auth
    		/// </summary>        
    	    [Display(Name = "Menu_Auth", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  Auth { get; set; }
    
    		    
    		/// <summary>
    		/// Order
    		/// </summary>        
    	    [Display(Name = "Menu_Order", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<int>  Order { get; set; }
    
    		    
    	}
    }
    
}
