using System; 
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using System; 
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using System; 
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace PurchaseOrderSys.Models
{
    
    /// <summary>
    /// WarehouseSummary class
    /// </summary>
    [MetadataType(typeof(WarehouseSummaryMetadata))]
    public  partial class WarehouseSummary
    {
    
    	/// <summary>
    	/// WarehouseSummary Metadata class
    	/// </summary>
    	public   class WarehouseSummaryMetadata
    	{
    		    
    		/// <summary>
    		/// Is Enable
    		/// </summary>        
    	    [Display(Name = "WarehouseSummary_IsEnable", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public bool  IsEnable { get; set; }
    
    		    
    		/// <summary>
    		/// ID
    		/// </summary>        
    	    [Display(Name = "WarehouseSummary_ID", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public int  ID { get; set; }
    
    		    
    		/// <summary>
    		/// Warehouse ID
    		/// </summary>        
    	    [Display(Name = "WarehouseSummary_WarehouseID", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public int  WarehouseID { get; set; }
    
    		    
    		/// <summary>
    		/// Type
    		/// </summary>        
    	    [Display(Name = "WarehouseSummary_Type", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  Type { get; set; }
    
    		    
    		/// <summary>
    		/// Val
    		/// </summary>        
    	    [Display(Name = "WarehouseSummary_Val", ResourceType = typeof(App_GlobalResources.Resource))]
    		public string  Val { get; set; }
    
    		    
    		/// <summary>
    		/// Url
    		/// </summary>        
    	    [Display(Name = "WarehouseSummary_Url", ResourceType = typeof(App_GlobalResources.Resource))]
    		public string  Url { get; set; }
    
    		    
    	}
    }
    
}
