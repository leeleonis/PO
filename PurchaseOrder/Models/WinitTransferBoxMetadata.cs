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
    /// WinitTransferBox class
    /// </summary>
    [MetadataType(typeof(WinitTransferBoxMetadata))]
    public  partial class WinitTransferBox
    {
    
    	/// <summary>
    	/// WinitTransferBox Metadata class
    	/// </summary>
    	public   class WinitTransferBoxMetadata
    	{
    		    
    		/// <summary>
    		/// Is Enable
    		/// </summary>        
    	    [Display(Name = "WinitTransferBox_IsEnable", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public bool  IsEnable { get; set; }
    
    		/// <summary>
    		/// Winit Transfer ID
    		/// </summary>        
    	    [Display(Name = "WinitTransferBox_WinitTransferID", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<int>  WinitTransferID { get; set; }
    
    		    
    		/// <summary>
    		/// Length
    		/// </summary>        
    	    [Display(Name = "WinitTransferBox_Length", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<decimal>  Length { get; set; }
    
    		    
    		/// <summary>
    		/// Width
    		/// </summary>        
    	    [Display(Name = "WinitTransferBox_Width", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<decimal>  Width { get; set; }
    
    		    
    		/// <summary>
    		/// Heigth
    		/// </summary>        
    	    [Display(Name = "WinitTransferBox_Heigth", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<decimal>  Heigth { get; set; }
    
    		    
    		/// <summary>
    		/// Weight
    		/// </summary>        
    	    [Display(Name = "WinitTransferBox_Weight", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<decimal>  Weight { get; set; }
    
    		    
    		/// <summary>
    		/// Bar Code
    		/// </summary>        
    	    [Display(Name = "WinitTransferBox_BarCode", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  BarCode { get; set; }
    
    		    
    		/// <summary>
    		/// Create By
    		/// </summary>        
    	    [Display(Name = "WinitTransferBox_CreateBy", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  CreateBy { get; set; }
    
    		    
    		/// <summary>
    		/// Create At
    		/// </summary>        
    	    [Display(Name = "WinitTransferBox_CreateAt", ResourceType = typeof(App_GlobalResources.Resource))]
            [UIHint("DateTime")]
            [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public System.DateTime  CreateAt { get; set; }
    
    		    
    		/// <summary>
    		/// Update By
    		/// </summary>        
    	    [Display(Name = "WinitTransferBox_UpdateBy", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  UpdateBy { get; set; }
    
    		    
    		/// <summary>
    		/// Update At
    		/// </summary>        
    	    [Display(Name = "WinitTransferBox_UpdateAt", ResourceType = typeof(App_GlobalResources.Resource))]
            [UIHint("DateTime")]
    		public Nullable<System.DateTime>  UpdateAt { get; set; }
    
    		    
    	}
    }
    
}
