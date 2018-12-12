using System; 
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace PurchaseOrderSys.Models
{
    
    /// <summary>
    /// TransferSKU class
    /// </summary>
    [MetadataType(typeof(TransferSKUMetadata))]
    public  partial class TransferSKU
    {
    
    	/// <summary>
    	/// TransferSKU Metadata class
    	/// </summary>
    	public   class TransferSKUMetadata
    	{
    		    
    		/// <summary>
    		/// Is Enable
    		/// </summary>        
    	    [Display(Name = "TransferSKU_IsEnable", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public bool  IsEnable { get; set; }
    
    		    
    		/// <summary>
    		/// ID
    		/// </summary>        
    	    [Display(Name = "TransferSKU_ID", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public int  ID { get; set; }
    
    		    
    		/// <summary>
    		/// Transfer ID
    		/// </summary>        
    	    [Display(Name = "TransferSKU_TransferID", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<int>  TransferID { get; set; }
    
    		    
    		/// <summary>
    		/// Create By
    		/// </summary>        
    	    [Display(Name = "TransferSKU_CreateBy", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  CreateBy { get; set; }
    
    		    
    		/// <summary>
    		/// Create At
    		/// </summary>        
    	    [Display(Name = "TransferSKU_CreateAt", ResourceType = typeof(App_GlobalResources.Resource))]
            [UIHint("DateTime")]
            [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public System.DateTime  CreateAt { get; set; }
    
    		    
    		/// <summary>
    		/// Update By
    		/// </summary>        
    	    [Display(Name = "TransferSKU_UpdateBy", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  UpdateBy { get; set; }
    
    		    
    		/// <summary>
    		/// Update At
    		/// </summary>        
    	    [Display(Name = "TransferSKU_UpdateAt", ResourceType = typeof(App_GlobalResources.Resource))]
            [UIHint("DateTime")]
    		public Nullable<System.DateTime>  UpdateAt { get; set; }
    
    		    
    		/// <summary>
    		/// QTY
    		/// </summary>        
    	    [Display(Name = "TransferSKU_QTY", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<int>  QTY { get; set; }
    
    		    
    		/// <summary>
    		/// Total Receive
    		/// </summary>        
    	    [Display(Name = "TransferSKU_TotalReceive", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<decimal>  TotalReceive { get; set; }
    
    		    
    		/// <summary>
    		/// TWN
    		/// </summary>        
    	    [Display(Name = "TransferSKU_TWN", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<int>  TWN { get; set; }
    
    		    
    		/// <summary>
    		/// Winit
    		/// </summary>        
    	    [Display(Name = "TransferSKU_Winit", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<int>  Winit { get; set; }
    
    		    
    		/// <summary>
    		/// Action
    		/// </summary>        
    	    [Display(Name = "TransferSKU_Action", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<int>  Action { get; set; }
    
    		    
    		/// <summary>
    		/// Sku No
    		/// </summary>        
    	    [Display(Name = "TransferSKU_SkuNo", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  SkuNo { get; set; }
    
    		    
    		/// <summary>
    		/// Name
    		/// </summary>        
    	    [Display(Name = "TransferSKU_Name", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(200, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  Name { get; set; }
    
    		    
    	}
    }
    
}
