using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace PurchaseOrderSys.Models
{

    /// <summary>
    /// RMASerialsLlist class
    /// </summary>
    [MetadataType(typeof(RMASerialsLlistMetadata))]
    public  partial class RMASerialsLlist
    {
    
    	/// <summary>
    	/// RMASerialsLlist Metadata class
    	/// </summary>
    	public   class RMASerialsLlistMetadata
    	{
    		    
    		/// <summary>
    		/// ID
    		/// </summary>        
    	    [Display(Name = "RMASerialsLlist_ID", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public int  ID { get; set; }
    
    		    
    		/// <summary>
    		/// RMASKUID
    		/// </summary>        
    	    [Display(Name = "RMASerialsLlist_RMASKUID", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<int>  RMASKUID { get; set; }
    
    		    
    		/// <summary>
    		/// PID
    		/// </summary>        
    	    [Display(Name = "RMASerialsLlist_PID", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<int>  PID { get; set; }
    
    		    
    		/// <summary>
    		/// Serials No
    		/// </summary>        
    	    [Display(Name = "RMASerialsLlist_SerialsNo", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  SerialsNo { get; set; }
    
    		    
    		/// <summary>
    		/// Serials QTY
    		/// </summary>        
    	    [Display(Name = "RMASerialsLlist_SerialsQTY", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<int>  SerialsQTY { get; set; }
    
    		    
    		/// <summary>
    		/// Create By
    		/// </summary>        
    	    [Display(Name = "RMASerialsLlist_CreateBy", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  CreateBy { get; set; }
    
    		    
    		/// <summary>
    		/// Create At
    		/// </summary>        
    	    [Display(Name = "RMASerialsLlist_CreateAt", ResourceType = typeof(App_GlobalResources.Resource))]
            [UIHint("DateTime")]
            [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public System.DateTime  CreateAt { get; set; }
    
    		    
    		/// <summary>
    		/// Update By
    		/// </summary>        
    	    [Display(Name = "RMASerialsLlist_UpdateBy", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  UpdateBy { get; set; }
    
    		    
    		/// <summary>
    		/// Update At
    		/// </summary>        
    	    [Display(Name = "RMASerialsLlist_UpdateAt", ResourceType = typeof(App_GlobalResources.Resource))]
            [UIHint("DateTime")]
    		public Nullable<System.DateTime>  UpdateAt { get; set; }
    
    		    
    		/// <summary>
    		/// Received By
    		/// </summary>        
    	    [Display(Name = "RMASerialsLlist_ReceivedBy", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  ReceivedBy { get; set; }
    
    		    
    		/// <summary>
    		/// Received At
    		/// </summary>        
    	    [Display(Name = "RMASerialsLlist_ReceivedAt", ResourceType = typeof(App_GlobalResources.Resource))]
            [UIHint("DateTime")]
    		public Nullable<System.DateTime>  ReceivedAt { get; set; }
    
    		    
    		/// <summary>
    		/// Warehouse ID
    		/// </summary>        
    	    [Display(Name = "RMASerialsLlist_WarehouseID", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<int>  WarehouseID { get; set; }
    
    		    
    		/// <summary>
    		/// Serials Type
    		/// </summary>        
    	    [Display(Name = "RMASerialsLlist_SerialsType", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  SerialsType { get; set; }
    
    		    
    		/// <summary>
    		/// Service ID
    		/// </summary>        
    	    [Display(Name = "RMASerialsLlist_ServiceID", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<int>  ServiceID { get; set; }
    
    		    
    	}
    }
    
}
