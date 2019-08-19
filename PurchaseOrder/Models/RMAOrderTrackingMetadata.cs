using System; 
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace PurchaseOrderSys.Models
{
    
    /// <summary>
    /// RMAOrderTracking class
    /// </summary>
    [MetadataType(typeof(RMAOrderTrackingMetadata))]
    public  partial class RMAOrderTracking
    {
    
    	/// <summary>
    	/// RMAOrderTracking Metadata class
    	/// </summary>
    	public   class RMAOrderTrackingMetadata
    	{
    		    
    		/// <summary>
    		/// Is Enable
    		/// </summary>        
    	    [Display(Name = "RMAOrderTracking_IsEnable", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public bool  IsEnable { get; set; }
    
    		    
    		/// <summary>
    		/// ID
    		/// </summary>        
    	    [Display(Name = "RMAOrderTracking_ID", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public int  ID { get; set; }
    
    		    
    		/// <summary>
    		/// Return Tracking
    		/// </summary>        
    	    [Display(Name = "RMAOrderTracking_ReturnTracking", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  ReturnTracking { get; set; }
    
    		    
    		/// <summary>
    		/// Carrier
    		/// </summary>        
    	    [Display(Name = "RMAOrderTracking_Carrier", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  Carrier { get; set; }
    
    		    
    		/// <summary>
    		/// To Name
    		/// </summary>        
    	    [Display(Name = "RMAOrderTracking_ToName", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  ToName { get; set; }
    
    		    
    		/// <summary>
    		/// To Address1
    		/// </summary>        
    	    [Display(Name = "RMAOrderTracking_ToAddress1", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  ToAddress1 { get; set; }
    
    		    
    		/// <summary>
    		/// To Address2
    		/// </summary>        
    	    [Display(Name = "RMAOrderTracking_ToAddress2", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  ToAddress2 { get; set; }
    
    		    
    		/// <summary>
    		/// To City
    		/// </summary>        
    	    [Display(Name = "RMAOrderTracking_ToCity", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  ToCity { get; set; }
    
    		    
    		/// <summary>
    		/// To State
    		/// </summary>        
    	    [Display(Name = "RMAOrderTracking_ToState", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  ToState { get; set; }
    
    		    
    		/// <summary>
    		/// To Postcode
    		/// </summary>        
    	    [Display(Name = "RMAOrderTracking_ToPostcode", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  ToPostcode { get; set; }
    
    		    
    		/// <summary>
    		/// To Country
    		/// </summary>        
    	    [Display(Name = "RMAOrderTracking_ToCountry", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  ToCountry { get; set; }
    
    		    
    		/// <summary>
    		/// From Name
    		/// </summary>        
    	    [Display(Name = "RMAOrderTracking_FromName", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  FromName { get; set; }
    
    		    
    		/// <summary>
    		/// From Address1
    		/// </summary>        
    	    [Display(Name = "RMAOrderTracking_FromAddress1", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  FromAddress1 { get; set; }
    
    		    
    		/// <summary>
    		/// From Address2
    		/// </summary>        
    	    [Display(Name = "RMAOrderTracking_FromAddress2", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  FromAddress2 { get; set; }
    
    		    
    		/// <summary>
    		/// From City
    		/// </summary>        
    	    [Display(Name = "RMAOrderTracking_FromCity", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  FromCity { get; set; }
    
    		    
    		/// <summary>
    		/// From State
    		/// </summary>        
    	    [Display(Name = "RMAOrderTracking_FromState", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  FromState { get; set; }
    
    		    
    		/// <summary>
    		/// From Postcode
    		/// </summary>        
    	    [Display(Name = "RMAOrderTracking_FromPostcode", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  FromPostcode { get; set; }
    
    		    
    		/// <summary>
    		/// From Country
    		/// </summary>        
    	    [Display(Name = "RMAOrderTracking_FromCountry", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  FromCountry { get; set; }
    
    		    
    		/// <summary>
    		/// Weight
    		/// </summary>        
    	    [Display(Name = "RMAOrderTracking_Weight", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<decimal>  Weight { get; set; }
    
    		    
    		/// <summary>
    		/// Length
    		/// </summary>        
    	    [Display(Name = "RMAOrderTracking_Length", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<decimal>  Length { get; set; }
    
    		    
    		/// <summary>
    		/// Width
    		/// </summary>        
    	    [Display(Name = "RMAOrderTracking_Width", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<decimal>  Width { get; set; }
    
    		    
    		/// <summary>
    		/// Heigth
    		/// </summary>        
    	    [Display(Name = "RMAOrderTracking_Heigth", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<decimal>  Heigth { get; set; }
    
    		    
    		/// <summary>
    		/// Insurance
    		/// </summary>        
    	    [Display(Name = "RMAOrderTracking_Insurance", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<bool>  Insurance { get; set; }
    
    		    
    		/// <summary>
    		/// ETA
    		/// </summary>        
    	    [Display(Name = "RMAOrderTracking_ETA", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<int>  ETA { get; set; }
    
    		    
    		/// <summary>
    		/// Declare Value
    		/// </summary>        
    	    [Display(Name = "RMAOrderTracking_DeclareValue", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<decimal>  DeclareValue { get; set; }
    
    		    
    		/// <summary>
    		/// Estimated Cost
    		/// </summary>        
    	    [Display(Name = "RMAOrderTracking_EstimatedCost", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<decimal>  EstimatedCost { get; set; }
    
    		    
    		/// <summary>
    		/// Shipping Method
    		/// </summary>        
    	    [Display(Name = "RMAOrderTracking_ShippingMethod", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  ShippingMethod { get; set; }
    
    		    
    		/// <summary>
    		/// Create By
    		/// </summary>        
    	    [Display(Name = "RMAOrderTracking_CreateBy", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  CreateBy { get; set; }
    
    		    
    		/// <summary>
    		/// Create At
    		/// </summary>        
    	    [Display(Name = "RMAOrderTracking_CreateAt", ResourceType = typeof(App_GlobalResources.Resource))]
            [UIHint("DateTime")]
            [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public System.DateTime  CreateAt { get; set; }
    
    		    
    		/// <summary>
    		/// Update By
    		/// </summary>        
    	    [Display(Name = "RMAOrderTracking_UpdateBy", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  UpdateBy { get; set; }
    
    		    
    		/// <summary>
    		/// Update At
    		/// </summary>        
    	    [Display(Name = "RMAOrderTracking_UpdateAt", ResourceType = typeof(App_GlobalResources.Resource))]
            [UIHint("DateTime")]
    		public Nullable<System.DateTime>  UpdateAt { get; set; }
    
    		    
    		/// <summary>
    		/// Memo
    		/// </summary>        
    	    [Display(Name = "RMAOrderTracking_Memo", ResourceType = typeof(App_GlobalResources.Resource))]
    		public string  Memo { get; set; }
    
    		    
    	}
    }
    
}
