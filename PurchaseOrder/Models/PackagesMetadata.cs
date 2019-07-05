using System; 
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace PurchaseOrderSys.Models
{
    
    /// <summary>
    /// Packages class
    /// </summary>
    [MetadataType(typeof(PackagesMetadata))]
    public  partial class Packages
    {
    
    	/// <summary>
    	/// Packages Metadata class
    	/// </summary>
    	public   class PackagesMetadata
    	{
    		    
    		/// <summary>
    		/// Is Enable
    		/// </summary>        
    	    [Display(Name = "Packages_IsEnable", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public bool  IsEnable { get; set; }
    
    		    
    		/// <summary>
    		/// ID
    		/// </summary>        
    	    [Display(Name = "Packages_ID", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public int  ID { get; set; }
    
    		    
    		/// <summary>
    		/// SCID
    		/// </summary>        
    	    [Display(Name = "Packages_SCID", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<int>  SCID { get; set; }
    
    		    
    		/// <summary>
    		/// Order ID
    		/// </summary>        
    	    [Display(Name = "Packages_OrderID", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public int  OrderID { get; set; }
    
    		    
    		/// <summary>
    		/// Carrier Box
    		/// </summary>        
    	    [Display(Name = "Packages_CarrierBox", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  CarrierBox { get; set; }
    
    		    
    		/// <summary>
    		/// Shipping Method
    		/// </summary>        
    	    [Display(Name = "Packages_ShippingMethod", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public int  ShippingMethod { get; set; }
    
    		    
    		/// <summary>
    		/// Export
    		/// </summary>        
    	    [Display(Name = "Packages_Export", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public byte  Export { get; set; }
    
    		    
    		/// <summary>
    		/// Export Method
    		/// </summary>        
    	    [Display(Name = "Packages_ExportMethod", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public byte  ExportMethod { get; set; }
    
    		    
    		/// <summary>
    		/// Export Value
    		/// </summary>        
    	    [Display(Name = "Packages_ExportValue", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public decimal  ExportValue { get; set; }
    
    		    
    		/// <summary>
    		/// Export Currency
    		/// </summary>        
    	    [Display(Name = "Packages_ExportCurrency", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public int  ExportCurrency { get; set; }
    
    		    
    		/// <summary>
    		/// Upload Tracking
    		/// </summary>        
    	    [Display(Name = "Packages_UploadTracking", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public bool  UploadTracking { get; set; }
    
    		    
    		/// <summary>
    		/// Tracking
    		/// </summary>        
    	    [Display(Name = "Packages_Tracking", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  Tracking { get; set; }
    
    		    
    		/// <summary>
    		/// DLExport
    		/// </summary>        
    	    [Display(Name = "Packages_DLExport", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public byte  DLExport { get; set; }
    
    		    
    		/// <summary>
    		/// DLExport Method
    		/// </summary>        
    	    [Display(Name = "Packages_DLExportMethod", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public byte  DLExportMethod { get; set; }
    
    		    
    		/// <summary>
    		/// DLExport Value
    		/// </summary>        
    	    [Display(Name = "Packages_DLExportValue", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public decimal  DLExportValue { get; set; }
    
    		    
    		/// <summary>
    		/// DLExport Currency
    		/// </summary>        
    	    [Display(Name = "Packages_DLExportCurrency", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public int  DLExportCurrency { get; set; }
    
    		    
    		/// <summary>
    		/// DLUpload Tracking
    		/// </summary>        
    	    [Display(Name = "Packages_DLUploadTracking", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public bool  DLUploadTracking { get; set; }
    
    		    
    		/// <summary>
    		/// DLTracking
    		/// </summary>        
    	    [Display(Name = "Packages_DLTracking", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  DLTracking { get; set; }
    
    		    
    		/// <summary>
    		/// Ship Warehouse
    		/// </summary>        
    	    [Display(Name = "Packages_ShipWarehouse", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public int  ShipWarehouse { get; set; }
    
    		    
    		/// <summary>
    		/// Return Warehouse
    		/// </summary>        
    	    [Display(Name = "Packages_ReturnWarehouse", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public int  ReturnWarehouse { get; set; }
    
    		    
    		/// <summary>
    		/// Shipping Status
    		/// </summary>        
    	    [Display(Name = "Packages_ShippingStatus", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public bool  ShippingStatus { get; set; }
    
    		    
    		/// <summary>
    		/// Update Bt
    		/// </summary>        
    	    [Display(Name = "Packages_UpdateBy", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  UpdateBy { get; set; }
    
    		    
    		/// <summary>
    		/// Update At
    		/// </summary>        
    	    [Display(Name = "Packages_UpdateAt", ResourceType = typeof(App_GlobalResources.Resource))]
            [UIHint("DateTime")]
    		public Nullable<System.DateTime>  UpdateAt { get; set; }
    
    		    
    		/// <summary>
    		/// Create Bt
    		/// </summary>        
    	    [Display(Name = "Packages_CreateBt", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  CreateBt { get; set; }
    
    		    
    		/// <summary>
    		/// Create At
    		/// </summary>        
    	    [Display(Name = "Packages_CreateAt", ResourceType = typeof(App_GlobalResources.Resource))]
            [UIHint("DateTime")]
            [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public System.DateTime  CreateAt { get; set; }
    
    		    
    	}
    }
    
}
