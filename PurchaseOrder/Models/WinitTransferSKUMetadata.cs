using System; 
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace PurchaseOrderSys.Models
{
    
    /// <summary>
    /// WinitTransferSKU class
    /// </summary>
    [MetadataType(typeof(WinitTransferSKUMetadata))]
    [Serializable]
    public  partial class WinitTransferSKU
    {
    
    	/// <summary>
    	/// WinitTransferSKU Metadata class
    	/// </summary>
    	public   class WinitTransferSKUMetadata
    	{
    		    
    		/// <summary>
    		/// Is Enable
    		/// </summary>        
    	    [Display(Name = "WinitTransferSKU_IsEnable", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public bool  IsEnable { get; set; }
    
    		    
    		/// <summary>
    		/// ID
    		/// </summary>        
    	    [Display(Name = "WinitTransferSKU_ID", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public int  ID { get; set; }
    
    		    
    		/// <summary>
    		/// Winit Transfer ID
    		/// </summary>        
    	    [Display(Name = "WinitTransferSKU_WinitTransferID", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<int>  WinitTransferID { get; set; }
    
    		    
    		/// <summary>
    		/// Sku No
    		/// </summary>        
    	    [Display(Name = "WinitTransferSKU_SkuNo", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  SkuNo { get; set; }
    
    		    
    		/// <summary>
    		/// item Barcode List
    		/// </summary>        
    	    [Display(Name = "WinitTransferSKU_itemBarcodeList", ResourceType = typeof(App_GlobalResources.Resource))]
    		public string  itemBarcodeList { get; set; }
    
    		    
    		/// <summary>
    		/// Create By
    		/// </summary>        
    	    [Display(Name = "WinitTransferSKU_CreateBy", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  CreateBy { get; set; }
    
    		    
    		/// <summary>
    		/// Create At
    		/// </summary>        
    	    [Display(Name = "WinitTransferSKU_CreateAt", ResourceType = typeof(App_GlobalResources.Resource))]
            [UIHint("DateTime")]
            [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public System.DateTime  CreateAt { get; set; }
    
    		    
    		/// <summary>
    		/// Update By
    		/// </summary>        
    	    [Display(Name = "WinitTransferSKU_UpdateBy", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  UpdateBy { get; set; }
    
    		    
    		/// <summary>
    		/// Update At
    		/// </summary>        
    	    [Display(Name = "WinitTransferSKU_UpdateAt", ResourceType = typeof(App_GlobalResources.Resource))]
            [UIHint("DateTime")]
    		public Nullable<System.DateTime>  UpdateAt { get; set; }
    
    		    
    		/// <summary>
    		/// Item Barcode File
    		/// </summary>        
    	    [Display(Name = "WinitTransferSKU_ItemBarcodeFile", ResourceType = typeof(App_GlobalResources.Resource))]
    		public string  ItemBarcodeFile { get; set; }
    
    		    
    	}
    }
    
}
