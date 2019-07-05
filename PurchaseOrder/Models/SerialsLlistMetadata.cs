using System; 
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace PurchaseOrderSys.Models
{
    
    /// <summary>
    /// SerialsLlist class
    /// </summary>
    [MetadataType(typeof(SerialsLlistMetadata))]
    public  partial class SerialsLlist
    {
    
    	/// <summary>
    	/// SerialsLlist Metadata class
    	/// </summary>
    	public   class SerialsLlistMetadata
    	{
    		    
    		/// <summary>
    		/// ID
    		/// </summary>        
    	    [Display(Name = "SerialsLlist_ID", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public int  ID { get; set; }
    
    		    
    		/// <summary>
    		/// Purchase SKUID
    		/// </summary>        
    	    [Display(Name = "SerialsLlist_PurchaseSKUID", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<int>  PurchaseSKUID { get; set; }
    
    		    
    		/// <summary>
    		/// PID
    		/// </summary>        
    	    [Display(Name = "SerialsLlist_PID", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<int>  PID { get; set; }
    
    		    
    		/// <summary>
    		/// Serials No
    		/// </summary>        
    	    [Display(Name = "SerialsLlist_SerialsNo", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  SerialsNo { get; set; }
    
    		    
    		/// <summary>
    		/// Serials QTY
    		/// </summary>        
    	    [Display(Name = "SerialsLlist_SerialsQTY", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<int>  SerialsQTY { get; set; }
    
    		    
    		/// <summary>
    		/// Create By
    		/// </summary>        
    	    [Display(Name = "SerialsLlist_CreateBy", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  CreateBy { get; set; }
    
    		    
    		/// <summary>
    		/// Create At
    		/// </summary>        
    	    [Display(Name = "SerialsLlist_CreateAt", ResourceType = typeof(App_GlobalResources.Resource))]
            [UIHint("DateTime")]
            [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public System.DateTime  CreateAt { get; set; }
    
    		    
    		/// <summary>
    		/// Update By
    		/// </summary>        
    	    [Display(Name = "SerialsLlist_UpdateBy", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  UpdateBy { get; set; }
    
    		    
    		/// <summary>
    		/// Update At
    		/// </summary>        
    	    [Display(Name = "SerialsLlist_UpdateAt", ResourceType = typeof(App_GlobalResources.Resource))]
            [UIHint("DateTime")]
    		public Nullable<System.DateTime>  UpdateAt { get; set; }
    
    		    
    		/// <summary>
    		/// Received By
    		/// </summary>        
    	    [Display(Name = "SerialsLlist_ReceivedBy", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  ReceivedBy { get; set; }
    
    		    
    		/// <summary>
    		/// Received At
    		/// </summary>        
    	    [Display(Name = "SerialsLlist_ReceivedAt", ResourceType = typeof(App_GlobalResources.Resource))]
            [UIHint("DateTime")]
    		public Nullable<System.DateTime>  ReceivedAt { get; set; }
    
    		    
    		/// <summary>
    		/// Transfer SKUID
    		/// </summary>        
    	    [Display(Name = "SerialsLlist_TransferSKUID", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<int>  TransferSKUID { get; set; }
    
    		    
    		/// <summary>
    		/// Order ID
    		/// </summary>        
    	    [Display(Name = "SerialsLlist_OrderID", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<int>  OrderID { get; set; }
    
    		    
    		/// <summary>
    		/// RMASKUID
    		/// </summary>        
    	    [Display(Name = "SerialsLlist_RMASKUID", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<int>  RMASKUID { get; set; }
    
    		    
    		/// <summary>
    		/// Serials Type
    		/// </summary>        
    	    [Display(Name = "SerialsLlist_SerialsType", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  SerialsType { get; set; }
    
    		    
    		/// <summary>
    		/// Memo
    		/// </summary>        
    	    [Display(Name = "SerialsLlist_Memo", ResourceType = typeof(App_GlobalResources.Resource))]
    		public string  Memo { get; set; }
    
    		    
    		/// <summary>
    		/// Is Enable
    		/// </summary>        
    	    [Display(Name = "SerialsLlist_IsEnable", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public bool  IsEnable { get; set; }
    
    		    
    	}
    }
    
}
