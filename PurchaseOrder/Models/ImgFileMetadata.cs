using System; 
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace PurchaseOrderSys.Models
{
    
    /// <summary>
    /// ImgFile class
    /// </summary>
    [MetadataType(typeof(ImgFileMetadata))]
    public  partial class ImgFile
    {
    
    	/// <summary>
    	/// ImgFile Metadata class
    	/// </summary>
    	public   class ImgFileMetadata
    	{
    		    
    		/// <summary>
    		/// Is Enable
    		/// </summary>        
    	    [Display(Name = "ImgFile_IsEnable", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public bool  IsEnable { get; set; }
    
    		    
    		/// <summary>
    		/// ID
    		/// </summary>        
    	    [Display(Name = "ImgFile_ID", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public int  ID { get; set; }
    
    		    
    		/// <summary>
    		/// Purchase Order ID
    		/// </summary>        
    	    [Display(Name = "ImgFile_PurchaseOrderID", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<int>  PurchaseOrderID { get; set; }
    
    		    
    		/// <summary>
    		/// Credit Memo ID
    		/// </summary>        
    	    [Display(Name = "ImgFile_CreditMemoID", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<int>  CreditMemoID { get; set; }
    
    		    
    		/// <summary>
    		/// Url
    		/// </summary>        
    	    [Display(Name = "ImgFile_Url", ResourceType = typeof(App_GlobalResources.Resource))]
    		public string  Url { get; set; }
    
    		    
    		/// <summary>
    		/// Create By
    		/// </summary>        
    	    [Display(Name = "ImgFile_CreateBy", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  CreateBy { get; set; }
    
    		    
    		/// <summary>
    		/// Create At
    		/// </summary>        
    	    [Display(Name = "ImgFile_CreateAt", ResourceType = typeof(App_GlobalResources.Resource))]
            [UIHint("DateTime")]
            [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public System.DateTime  CreateAt { get; set; }
    
    		    
    		/// <summary>
    		/// Update By
    		/// </summary>        
    	    [Display(Name = "ImgFile_UpdateBy", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  UpdateBy { get; set; }
    
    		    
    		/// <summary>
    		/// Update At
    		/// </summary>        
    	    [Display(Name = "ImgFile_UpdateAt", ResourceType = typeof(App_GlobalResources.Resource))]
            [UIHint("DateTime")]
    		public Nullable<System.DateTime>  UpdateAt { get; set; }
    
    		    
    		/// <summary>
    		/// Img Type
    		/// </summary>        
    	    [Display(Name = "ImgFile_ImgType", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  ImgType { get; set; }
    
    		    
    		/// <summary>
    		/// Purchase SKUID
    		/// </summary>        
    	    [Display(Name = "ImgFile_PurchaseSKUID", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<int>  PurchaseSKUID { get; set; }
    
    		    
    	}
    }
    
}
