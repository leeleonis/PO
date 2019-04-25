using System; 
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using System;
using System.ComponentModel.DataAnnotations;


namespace PurchaseOrderSys.Models
{

    /// <summary>
    /// ShippingMethod class
    /// </summary>
    [MetadataType(typeof(ShippingMethodMetadata))]
    public  partial class ShippingMethod
    {
    
    	/// <summary>
    	/// ShippingMethod Metadata class
    	/// </summary>
    	public   class ShippingMethodMetadata
    	{
    		    
    		/// <summary>
    		/// Is Enable
    		/// </summary>        
    	    [Display(Name = "ShippingMethod_IsEnable", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public bool  IsEnable { get; set; }
    
    		    
    		/// <summary>
    		/// Is Direct Line
    		/// </summary>        
    	    [Display(Name = "ShippingMethod_IsDirectLine", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public bool  IsDirectLine { get; set; }
    
    		    
    		/// <summary>
    		/// ID
    		/// </summary>        
    	    [Display(Name = "ShippingMethod_ID", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public int  ID { get; set; }
    
    		    
    		/// <summary>
    		/// Name
    		/// </summary>        
    	    [Display(Name = "ShippingMethod_Name", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  Name { get; set; }
    
    		    
    		/// <summary>
    		/// Carrier ID
    		/// </summary>        
    	    [Display(Name = "ShippingMethod_CarrierID", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<int>  CarrierID { get; set; }
    
    		    
    		/// <summary>
    		/// Method Type
    		/// </summary>        
    	    [Display(Name = "ShippingMethod_MethodType", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<int>  MethodType { get; set; }
    
    		    
    		/// <summary>
    		/// In Box
    		/// </summary>        
    	    [Display(Name = "ShippingMethod_InBox", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public bool  InBox { get; set; }
    
    		    
    		/// <summary>
    		/// Box Type
    		/// </summary>        
    	    [Display(Name = "ShippingMethod_BoxType", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<int>  BoxType { get; set; }
    
    		    
    		/// <summary>
    		/// Direct Line
    		/// </summary>        
    	    [Display(Name = "ShippingMethod_DirectLine", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public int  DirectLine { get; set; }
    
    		    
    		/// <summary>
    		/// Printer Name
    		/// </summary>        
    	    [Display(Name = "ShippingMethod_PrinterName", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  PrinterName { get; set; }
    
    		    
    		/// <summary>
    		/// Is Export
    		/// </summary>        
    	    [Display(Name = "ShippingMethod_IsExport", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<bool>  IsExport { get; set; }
    
    		    
    		/// <summary>
    		/// Is Battery
    		/// </summary>        
    	    [Display(Name = "ShippingMethod_IsBattery", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<bool>  IsBattery { get; set; }
    
    		    
    		/// <summary>
    		/// Country Data
    		/// </summary>        
    	    [Display(Name = "ShippingMethod_CountryData", ResourceType = typeof(App_GlobalResources.Resource))]
    		public string  CountryData { get; set; }
    
    		    
    		/// <summary>
    		/// Sync On
    		/// </summary>        
    	    [Display(Name = "ShippingMethod_SyncOn", ResourceType = typeof(App_GlobalResources.Resource))]
            [UIHint("DateTime")]
    		public Nullable<System.DateTime>  SyncOn { get; set; }
    
    		    
    		/// <summary>
    		/// Contact Name
    		/// </summary>        
    	    [Display(Name = "ShippingMethod_ContactName", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  ContactName { get; set; }
    
    		    
    		/// <summary>
    		/// Conpany Name
    		/// </summary>        
    	    [Display(Name = "ShippingMethod_ConpanyName", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  ConpanyName { get; set; }
    
    		    
    		/// <summary>
    		/// Phone Number
    		/// </summary>        
    	    [Display(Name = "ShippingMethod_PhoneNumber", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  PhoneNumber { get; set; }
    
    		    
    		/// <summary>
    		/// Country Name
    		/// </summary>        
    	    [Display(Name = "ShippingMethod_CountryName", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  CountryName { get; set; }
    
    		    
    		/// <summary>
    		/// Country Code
    		/// </summary>        
    	    [Display(Name = "ShippingMethod_CountryCode", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(3, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  CountryCode { get; set; }
    
    		    
    		/// <summary>
    		/// City
    		/// </summary>        
    	    [Display(Name = "ShippingMethod_City", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  City { get; set; }
    
    		    
    		/// <summary>
    		/// Street Line1
    		/// </summary>        
    	    [Display(Name = "ShippingMethod_StreetLine1", ResourceType = typeof(App_GlobalResources.Resource))]
    		public string  StreetLine1 { get; set; }
    
    		    
    		/// <summary>
    		/// Street Line2
    		/// </summary>        
    	    [Display(Name = "ShippingMethod_StreetLine2", ResourceType = typeof(App_GlobalResources.Resource))]
    		public string  StreetLine2 { get; set; }
    
    		    
    		/// <summary>
    		/// State Name
    		/// </summary>        
    	    [Display(Name = "ShippingMethod_StateName", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  StateName { get; set; }
    
    		    
    		/// <summary>
    		/// Postal Code
    		/// </summary>        
    	    [Display(Name = "ShippingMethod_PostalCode", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  PostalCode { get; set; }
    
    		    
    	}
    }
    
}
