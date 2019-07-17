using System; 
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace PurchaseOrderSys.Models
{
    
    /// <summary>
    /// OrderAddresses class
    /// </summary>
    [MetadataType(typeof(OrderAddressesMetadata))]
    public  partial class OrderAddresses
    {
    
    	/// <summary>
    	/// OrderAddresses Metadata class
    	/// </summary>
    	public   class OrderAddressesMetadata
    	{
    		    
    		/// <summary>
    		/// Is Enable
    		/// </summary>        
    	    [Display(Name = "OrderAddresses_IsEnable", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public bool  IsEnable { get; set; }
    
    		    
    		/// <summary>
    		/// ID
    		/// </summary>        
    	    [Display(Name = "OrderAddresses_ID", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public int  ID { get; set; }
    
    		    
    		/// <summary>
    		/// SCID
    		/// </summary>        
    	    [Display(Name = "OrderAddresses_SCID", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<int>  SCID { get; set; }
    
    		    
    		/// <summary>
    		/// Order ID
    		/// </summary>        
    	    [Display(Name = "OrderAddresses_OrderID", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public int  OrderID { get; set; }
    
    		    
    		/// <summary>
    		/// Type
    		/// </summary>        
    	    [Display(Name = "OrderAddresses_Type", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public byte  Type { get; set; }
    
    		    
    		/// <summary>
    		/// First Name
    		/// </summary>        
    	    [Display(Name = "OrderAddresses_FirstName", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  FirstName { get; set; }
    
    		    
    		/// <summary>
    		/// Last Name
    		/// </summary>        
    	    [Display(Name = "OrderAddresses_LastName", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  LastName { get; set; }
    
    		    
    		/// <summary>
    		/// Address Line1
    		/// </summary>        
    	    [Display(Name = "OrderAddresses_AddressLine1", ResourceType = typeof(App_GlobalResources.Resource))]
    		public string  AddressLine1 { get; set; }
    
    		    
    		/// <summary>
    		/// Address Line2
    		/// </summary>        
    	    [Display(Name = "OrderAddresses_AddressLine2", ResourceType = typeof(App_GlobalResources.Resource))]
    		public string  AddressLine2 { get; set; }
    
    		    
    		/// <summary>
    		/// City
    		/// </summary>        
    	    [Display(Name = "OrderAddresses_City", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  City { get; set; }
    
    		    
    		/// <summary>
    		/// State
    		/// </summary>        
    	    [Display(Name = "OrderAddresses_State", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  State { get; set; }
    
    		    
    		/// <summary>
    		/// Postcode
    		/// </summary>        
    	    [Display(Name = "OrderAddresses_Postcode", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  Postcode { get; set; }
    
    		    
    		/// <summary>
    		/// Country Code
    		/// </summary>        
    	    [Display(Name = "OrderAddresses_CountryCode", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  CountryCode { get; set; }
    
    		    
    		/// <summary>
    		/// Country Name
    		/// </summary>        
    	    [Display(Name = "OrderAddresses_CountryName", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  CountryName { get; set; }
    
    		    
    		/// <summary>
    		/// Phone Number
    		/// </summary>        
    	    [Display(Name = "OrderAddresses_PhoneNumber", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  PhoneNumber { get; set; }
    
    		    
    		/// <summary>
    		/// Email Address
    		/// </summary>        
    	    [Display(Name = "OrderAddresses_EmailAddress", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(100, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  EmailAddress { get; set; }
    
    		    
    		/// <summary>
    		/// Update By
    		/// </summary>        
    	    [Display(Name = "OrderAddresses_UpdateBy", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  UpdateBy { get; set; }
    
    		    
    		/// <summary>
    		/// Update At
    		/// </summary>        
    	    [Display(Name = "OrderAddresses_UpdateAt", ResourceType = typeof(App_GlobalResources.Resource))]
            [UIHint("DateTime")]
    		public Nullable<System.DateTime>  UpdateAt { get; set; }
    
    		    
    		/// <summary>
    		/// Create By
    		/// </summary>        
    	    [Display(Name = "OrderAddresses_CreateBy", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  CreateBy { get; set; }
    
    		    
    		/// <summary>
    		/// Create At
    		/// </summary>        
    	    [Display(Name = "OrderAddresses_CreateAt", ResourceType = typeof(App_GlobalResources.Resource))]
            [UIHint("DateTime")]
            [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public System.DateTime  CreateAt { get; set; }
    
    		    
    	}
    }
    
}
