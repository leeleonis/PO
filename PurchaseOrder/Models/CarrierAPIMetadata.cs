using System;
using System.ComponentModel.DataAnnotations;


namespace PurchaseOrderSys.Models
{

    /// <summary>
    /// CarrierAPI class
    /// </summary>
    [MetadataType(typeof(CarrierAPIMetadata))]
    public  partial class CarrierAPI
    {
    
    	/// <summary>
    	/// CarrierAPI Metadata class
    	/// </summary>
    	public   class CarrierAPIMetadata
    	{
    		    
    		/// <summary>
    		/// Is Enable
    		/// </summary>        
    	    [Display(Name = "CarrierAPI_IsEnable", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public bool  IsEnable { get; set; }
    
    		    
    		/// <summary>
    		/// Is Test
    		/// </summary>        
    	    [Display(Name = "CarrierAPI_IsTest", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public bool  IsTest { get; set; }
    
    		    
    		/// <summary>
    		/// Id
    		/// </summary>        
    	    [Display(Name = "CarrierAPI_Id", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public int  Id { get; set; }
    
    		    
    		/// <summary>
    		/// Name
    		/// </summary>        
    	    [Display(Name = "CarrierAPI_Name", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  Name { get; set; }
    
    		    
    		/// <summary>
    		/// Type
    		/// </summary>        
    	    [Display(Name = "CarrierAPI_Type", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<byte>  Type { get; set; }
    
    		    
    		/// <summary>
    		/// Account ID
    		/// </summary>        
    	    [Display(Name = "CarrierAPI_AccountID", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  AccountID { get; set; }
    
    		    
    		/// <summary>
    		/// Api Key
    		/// </summary>        
    	    [Display(Name = "CarrierAPI_ApiKey", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  ApiKey { get; set; }
    
    		    
    		/// <summary>
    		/// Api Password
    		/// </summary>        
    	    [Display(Name = "CarrierAPI_ApiPassword", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  ApiPassword { get; set; }
    
    		    
    		/// <summary>
    		/// Api Account
    		/// </summary>        
    	    [Display(Name = "CarrierAPI_ApiAccount", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  ApiAccount { get; set; }
    
    		    
    		/// <summary>
    		/// Api Meter
    		/// </summary>        
    	    [Display(Name = "CarrierAPI_ApiMeter", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  ApiMeter { get; set; }
    
    		    
    		/// <summary>
    		/// Api Hub
    		/// </summary>        
    	    [Display(Name = "CarrierAPI_ApiHub", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  ApiHub { get; set; }
    
    		    
    	}
    }
    
}
