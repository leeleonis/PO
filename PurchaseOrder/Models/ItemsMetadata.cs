using System; 
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace PurchaseOrderSys.Models
{
    
    /// <summary>
    /// Items class
    /// </summary>
    [MetadataType(typeof(ItemsMetadata))]
    public  partial class Items
    {
        public string SerialEdit { get; set; }

        /// <summary>
        /// Items Metadata class
        /// </summary>
        public   class ItemsMetadata
    	{
    		    
    		/// <summary>
    		/// Is Enable
    		/// </summary>        
    	    [Display(Name = "Items_IsEnable", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public bool  IsEnable { get; set; }
    
    		    
    		/// <summary>
    		/// ID
    		/// </summary>        
    	    [Display(Name = "Items_ID", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public int  ID { get; set; }
    
    		    
    		/// <summary>
    		/// SCID
    		/// </summary>        
    	    [Display(Name = "Items_SCID", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<int>  SCID { get; set; }
    
    		    
    		/// <summary>
    		/// Order ID
    		/// </summary>        
    	    [Display(Name = "Items_OrderID", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public int  OrderID { get; set; }
    
    		    
    		/// <summary>
    		/// Package ID
    		/// </summary>        
    	    [Display(Name = "Items_PackageID", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public int  PackageID { get; set; }
    
    		    
    		/// <summary>
    		/// Sku
    		/// </summary>        
    	    [Display(Name = "Items_Sku", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  Sku { get; set; }
    
    		    
    		/// <summary>
    		/// Unit Price
    		/// </summary>        
    	    [Display(Name = "Items_UnitPrice", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public decimal  UnitPrice { get; set; }
    
    		    
    		/// <summary>
    		/// Export Value
    		/// </summary>        
    	    [Display(Name = "Items_ExportValue", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public decimal  ExportValue { get; set; }
    
    		    
    		/// <summary>
    		/// DLExport Value
    		/// </summary>        
    	    [Display(Name = "Items_DLExportValue", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public decimal  DLExportValue { get; set; }
    
    		    
    		/// <summary>
    		/// Qty
    		/// </summary>        
    	    [Display(Name = "Items_Qty", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public int  Qty { get; set; }
    
    		    
    		/// <summary>
    		/// e Bay Item ID
    		/// </summary>        
    	    [Display(Name = "Items_eBayItemID", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  eBayItemID { get; set; }
    
    		    
    		/// <summary>
    		/// Sales Record Number
    		/// </summary>        
    	    [Display(Name = "Items_SalesRecordNumber", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  SalesRecordNumber { get; set; }
    
    		    
    		/// <summary>
    		/// RMAID
    		/// </summary>        
    	    [Display(Name = "Items_RMAID", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<int>  RMAID { get; set; }
    
    		    
    		/// <summary>
    		/// Update_by
    		/// </summary>        
    	    [Display(Name = "Items_Update_by", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  Update_by { get; set; }
    
    		    
    		/// <summary>
    		/// Update_at
    		/// </summary>        
    	    [Display(Name = "Items_Update_at", ResourceType = typeof(App_GlobalResources.Resource))]
            [UIHint("DateTime")]
    		public Nullable<System.DateTime>  Update_at { get; set; }
    
    		    
    		/// <summary>
    		/// Create_by
    		/// </summary>        
    	    [Display(Name = "Items_Create_by", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  Create_by { get; set; }
    
    		    
    		/// <summary>
    		/// Create_at
    		/// </summary>        
    	    [Display(Name = "Items_Create_at", ResourceType = typeof(App_GlobalResources.Resource))]
            [UIHint("DateTime")]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public System.DateTime  Create_at { get; set; }
        }
    }
    
}
