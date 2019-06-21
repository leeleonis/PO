using System; 
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PurchaseOrderSys.Models
{
    
    /// <summary>
    /// RMA class
    /// </summary>
    [MetadataType(typeof(RMAMetadata))]
    public  partial class RMA
    {
    
    	/// <summary>
    	/// RMA Metadata class
    	/// </summary>
    	public   class RMAMetadata
    	{
    		    
    		/// <summary>
    		/// Is Enable
    		/// </summary>        
    	    [Display(Name = "RMA_IsEnable", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public bool  IsEnable { get; set; }
    
    		    
    		/// <summary>
    		/// ID
    		/// </summary>        
    	    [Display(Name = "RMA_ID", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public int  ID { get; set; }
    
    		    
    		/// <summary>
    		/// Order ID
    		/// </summary>        
    	    [Display(Name = "RMA_OrderID", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<int>  OrderID { get; set; }
    
    		    
    		/// <summary>
    		/// Source ID
    		/// </summary>        
    	    [Display(Name = "RMA_SourceID", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  SourceID { get; set; }
    
    		    
    		/// <summary>
    		/// Company ID
    		/// </summary>        
    	    [Display(Name = "RMA_CompanyID", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<int>  CompanyID { get; set; }
    
    		    
    		/// <summary>
    		/// Country
    		/// </summary>        
    	    [Display(Name = "RMA_Country", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  Country { get; set; }
    
    		    
    		/// <summary>
    		/// Status
    		/// </summary>        
    	    [Display(Name = "RMA_Status", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  Status { get; set; }
    
    		    
    		/// <summary>
    		/// Action
    		/// </summary>        
    	    [Display(Name = "RMA_Action", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  Action { get; set; }
    
    		    
    		/// <summary>
    		/// Reason
    		/// </summary>        
    	    [Display(Name = "RMA_Reason", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  Reason { get; set; }
    
    		    
    		/// <summary>
    		/// Return Tracking
    		/// </summary>        
    	    [Display(Name = "RMA_ReturnTracking", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  ReturnTracking { get; set; }
    
    		    
    		/// <summary>
    		/// Create By
    		/// </summary>        
    	    [Display(Name = "RMA_CreateBy", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  CreateBy { get; set; }
    
    		    
    		/// <summary>
    		/// Create At
    		/// </summary>        
    	    [Display(Name = "RMA_CreateAt", ResourceType = typeof(App_GlobalResources.Resource))]
            [UIHint("FDate")]
            [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public System.DateTime  CreateAt { get; set; }
    
    		    
    		/// <summary>
    		/// Update By
    		/// </summary>        
    	    [Display(Name = "RMA_UpdateBy", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  UpdateBy { get; set; }
    
    		    
    		/// <summary>
    		/// Update At
    		/// </summary>        
    	    [Display(Name = "RMA_UpdateAt", ResourceType = typeof(App_GlobalResources.Resource))]
            [UIHint("FDate")]
    		public Nullable<System.DateTime>  UpdateAt { get; set; }
    
    		    
    		/// <summary>
    		/// Carrier
    		/// </summary>        
    	    [Display(Name = "RMA_Carrier", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  Carrier { get; set; }
    
    		    
    		/// <summary>
    		/// Channel
    		/// </summary>        
    	    [Display(Name = "RMA_Channel", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<int>  Channel { get; set; }
    
    		    
    		/// <summary>
    		/// Source Case ID
    		/// </summary>        
    	    [Display(Name = "RMA_SourceCaseID", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  SourceCaseID { get; set; }
    
    		    
    		/// <summary>
    		/// SCRMA
    		/// </summary>        
    	    [Display(Name = "RMA_SCRMA", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  SCRMA { get; set; }
    
    		    
    		/// <summary>
    		/// Warehouse ID
    		/// </summary>        
    	    [Display(Name = "RMA_WarehouseID", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<int>  WarehouseID { get; set; }
    
    		    
    		/// <summary>
    		/// Final Shipping Fee
    		/// </summary>        
    	    [Display(Name = "RMA_FinalShippingFee", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<decimal>  FinalShippingFee { get; set; }
    
    		    
    		/// <summary>
    		/// Restocking Fee
    		/// </summary>        
    	    [Display(Name = "RMA_RestockingFee", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<decimal>  RestockingFee { get; set; }
    
    		    
    		/// <summary>
    		/// Other Costs
    		/// </summary>        
    	    [Display(Name = "RMA_OtherCosts", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<decimal>  OtherCosts { get; set; }
    
    		    
    		/// <summary>
    		/// Return Shipping Cos
    		/// </summary>        
    	    [Display(Name = "RMA_ReturnShippingCos", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<decimal>  ReturnShippingCos { get; set; }
    
    		    
    		/// <summary>
    		/// SCUser ID
    		/// </summary>        
    	    [Display(Name = "RMA_SCUserID", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  SCUserID { get; set; }
    
    		    
    	}
    }
    
}
