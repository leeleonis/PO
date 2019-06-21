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
    /// ShippingMethods class
    /// </summary>
    [MetadataType(typeof(ShippingMethodsMetadata))]
    public  partial class ShippingMethods
    {
    
    	/// <summary>
    	/// ShippingMethods Metadata class
    	/// </summary>
    	public   class ShippingMethodsMetadata
    	{
    		    
    		/// <summary>
    		/// Is Enable
    		/// </summary>        
    	    [Display(Name = "ShippingMethods_IsEnable", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public bool  IsEnable { get; set; }
    
    		    
    		/// <summary>
    		/// Is Export
    		/// </summary>        
    	    [Display(Name = "ShippingMethods_IsExport", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public bool  IsExport { get; set; }
    
    		    
    		/// <summary>
    		/// Is Battery
    		/// </summary>        
    	    [Display(Name = "ShippingMethods_IsBattery", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public bool  IsBattery { get; set; }
    
    		    
    		/// <summary>
    		/// ID
    		/// </summary>        
    	    [Display(Name = "ShippingMethods_ID", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public int  ID { get; set; }
    
    		    
    		/// <summary>
    		/// Name
    		/// </summary>        
    	    [Display(Name = "ShippingMethods_Name", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  Name { get; set; }
    
    		    
    		/// <summary>
    		/// Carrier ID
    		/// </summary>        
    	    [Display(Name = "ShippingMethods_CarrierID", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public int  CarrierID { get; set; }
    
    		    
    		/// <summary>
    		/// Method Type
    		/// </summary>        
    	    [Display(Name = "ShippingMethods_MethodType", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public int  MethodType { get; set; }
    
    		    
    		/// <summary>
    		/// Box Type
    		/// </summary>        
    	    [Display(Name = "ShippingMethods_BoxType", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public int  BoxType { get; set; }
    
    		    
    		/// <summary>
    		/// In Box
    		/// </summary>        
    	    [Display(Name = "ShippingMethods_InBox", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public bool  InBox { get; set; }
    
    		    
    		/// <summary>
    		/// Contact Name
    		/// </summary>        
    	    [Display(Name = "ShippingMethods_ContactName", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  ContactName { get; set; }
    
    		    
    		/// <summary>
    		/// Conpany Name
    		/// </summary>        
    	    [Display(Name = "ShippingMethods_ConpanyName", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  ConpanyName { get; set; }
    
    		    
    		/// <summary>
    		/// Phone Number
    		/// </summary>        
    	    [Display(Name = "ShippingMethods_PhoneNumber", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  PhoneNumber { get; set; }
    
    		    
    		/// <summary>
    		/// Country Name
    		/// </summary>        
    	    [Display(Name = "ShippingMethods_CountryName", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  CountryName { get; set; }
    
    		    
    		/// <summary>
    		/// Country Code
    		/// </summary>        
    	    [Display(Name = "ShippingMethods_CountryCode", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(3, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  CountryCode { get; set; }
    
    		    
    		/// <summary>
    		/// City
    		/// </summary>        
    	    [Display(Name = "ShippingMethods_City", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  City { get; set; }
    
    		    
    		/// <summary>
    		/// Street Line1
    		/// </summary>        
    	    [Display(Name = "ShippingMethods_StreetLine1", ResourceType = typeof(App_GlobalResources.Resource))]
    		public string  StreetLine1 { get; set; }
    
    		    
    		/// <summary>
    		/// Street Line2
    		/// </summary>        
    	    [Display(Name = "ShippingMethods_StreetLine2", ResourceType = typeof(App_GlobalResources.Resource))]
    		public string  StreetLine2 { get; set; }
    
    		    
    		/// <summary>
    		/// State Name
    		/// </summary>        
    	    [Display(Name = "ShippingMethods_StateName", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  StateName { get; set; }
    
    		    
    		/// <summary>
    		/// Postal Code
    		/// </summary>        
    	    [Display(Name = "ShippingMethods_PostalCode", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  PostalCode { get; set; }
    
    		    
    		/// <summary>
    		/// Update_by
    		/// </summary>        
    	    [Display(Name = "ShippingMethods_Update_by", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  Update_by { get; set; }
    
    		    
    		/// <summary>
    		/// Update_at
    		/// </summary>        
    	    [Display(Name = "ShippingMethods_Update_at", ResourceType = typeof(App_GlobalResources.Resource))]
            [UIHint("DateTime")]
    		public Nullable<System.DateTime>  Update_at { get; set; }
    
    		    
    		/// <summary>
    		/// Create_by
    		/// </summary>        
    	    [Display(Name = "ShippingMethods_Create_by", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  Create_by { get; set; }
    
    		    
    		/// <summary>
    		/// Create_at
    		/// </summary>        
    	    [Display(Name = "ShippingMethods_Create_at", ResourceType = typeof(App_GlobalResources.Resource))]
            [UIHint("DateTime")]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public System.DateTime  Create_at { get; set; }
    
    		    
    	}
    }
    
}
