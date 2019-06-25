using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace PurchaseOrderSys.Models
{

    /// <summary>
    /// Logistic class
    /// </summary>
    [MetadataType(typeof(LogisticMetadata))]
    public  partial class Logistic
    {
    
    	/// <summary>
    	/// Logistic Metadata class
    	/// </summary>
    	public   class LogisticMetadata
    	{
    		    
    		/// <summary>
    		/// Sku
    		/// </summary>        
    	    [Display(Name = "Logistic_Sku", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  Sku { get; set; }
    
    		    
    		/// <summary>
    		/// Box ID
    		/// </summary>        
    	    [Display(Name = "Logistic_BoxID", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public int  BoxID { get; set; }
    
    		    
    		/// <summary>
    		/// Case Width
    		/// </summary>        
    	    [Display(Name = "Logistic_CaseWidth", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public double  CaseWidth { get; set; }
    
    		    
    		/// <summary>
    		/// Case Length
    		/// </summary>        
    	    [Display(Name = "Logistic_CaseLength", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public double  CaseLength { get; set; }
    
    		    
    		/// <summary>
    		/// Case Height
    		/// </summary>        
    	    [Display(Name = "Logistic_CaseHeight", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public double  CaseHeight { get; set; }
    
    		    
    		/// <summary>
    		/// Case Weight
    		/// </summary>        
    	    [Display(Name = "Logistic_CaseWeight", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public double  CaseWeight { get; set; }
    
    		    
    		/// <summary>
    		/// Shipping Width
    		/// </summary>        
    	    [Display(Name = "Logistic_ShippingWidth", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public double  ShippingWidth { get; set; }
    
    		    
    		/// <summary>
    		/// Shipping Length
    		/// </summary>        
    	    [Display(Name = "Logistic_ShippingLength", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public double  ShippingLength { get; set; }
    
    		    
    		/// <summary>
    		/// Shipping Height
    		/// </summary>        
    	    [Display(Name = "Logistic_ShippingHeight", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public double  ShippingHeight { get; set; }
    
    		    
    		/// <summary>
    		/// Shipping Weight
    		/// </summary>        
    	    [Display(Name = "Logistic_ShippingWeight", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public double  ShippingWeight { get; set; }
    
    		    
    		/// <summary>
    		/// Origin Country
    		/// </summary>        
    	    [Display(Name = "Logistic_OriginCountry", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  OriginCountry { get; set; }
    
    		    
    		/// <summary>
    		/// Image Path
    		/// </summary>        
    	    [Display(Name = "Logistic_ImagePath", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(100, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  ImagePath { get; set; }
    
    		    
    		/// <summary>
    		/// Create By
    		/// </summary>        
    	    [Display(Name = "Logistic_CreateBy", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  CreateBy { get; set; }
    
    		    
    		/// <summary>
    		/// Create At
    		/// </summary>        
    	    [Display(Name = "Logistic_CreateAt", ResourceType = typeof(App_GlobalResources.Resource))]
            [UIHint("DateTime")]
            [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public System.DateTime  CreateAt { get; set; }
    
    		    
    		/// <summary>
    		/// Update By
    		/// </summary>        
    	    [Display(Name = "Logistic_UpdateBy", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  UpdateBy { get; set; }
    
    		    
    		/// <summary>
    		/// Update At
    		/// </summary>        
    	    [Display(Name = "Logistic_UpdateAt", ResourceType = typeof(App_GlobalResources.Resource))]
            [UIHint("DateTime")]
    		public Nullable<System.DateTime>  UpdateAt { get; set; }
    
    		    
    	}
    }
    
}
