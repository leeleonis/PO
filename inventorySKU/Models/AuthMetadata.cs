using System.ComponentModel.DataAnnotations;
namespace inventorySKU.Models
{

    /// <summary>
    /// Auth class
    /// </summary>
    [MetadataType(typeof(AuthMetadata))]
    public  partial class Auth
    {
    
    	/// <summary>
    	/// Auth Metadata class
    	/// </summary>
    	public   class AuthMetadata
    	{
    		    
    		/// <summary>
    		/// ID
    		/// </summary>        
    	    [Display(Name = "Auth_ID", ResourceType = typeof(ViewRes.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ViewRes.Resource))]
    		public int  ID { get; set; }
    
    		    
    		/// <summary>
    		/// Name
    		/// </summary>        
    	    [Display(Name = "Auth_Name", ResourceType = typeof(ViewRes.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ViewRes.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ViewRes.Resource))]
    		public string  Name { get; set; }
    
    		    
    		/// <summary>
    		/// Value
    		/// </summary>        
    	    [Display(Name = "Auth_Value", ResourceType = typeof(ViewRes.Resource))]
            [MaxLength(1, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ViewRes.Resource))]
    		public string  Value { get; set; }
    
    		    
    	}
    }
    
}