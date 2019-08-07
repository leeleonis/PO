using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace PurchaseOrderSys.Models
{

    /// <summary>
    /// Warehouse class
    /// </summary>
    [MetadataType(typeof(WarehouseMetadata))]
    [Serializable]
    public  partial class Warehouse
    {
    
    	/// <summary>
    	/// Warehouse Metadata class
    	/// </summary>
    	public   class WarehouseMetadata
    	{
    		    
    		/// <summary>
    		/// Is Enable
    		/// </summary>        
    	    [Display(Name = "Warehouse_IsEnable", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public bool  IsEnable { get; set; }
    
    		    
    		/// <summary>
    		/// Is Default
    		/// </summary>        
    	    [Display(Name = "Warehouse_IsDefault", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public bool  IsDefault { get; set; }
    
    		    
    		/// <summary>
    		/// Is Sellable
    		/// </summary>        
    	    [Display(Name = "Warehouse_IsSellable", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public bool  IsSellable { get; set; }
    
    		    
    		/// <summary>
    		/// ID
    		/// </summary>        
    	    [Display(Name = "Warehouse_ID", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public int  ID { get; set; }
    
    		    
    		/// <summary>
    		/// Name
    		/// </summary>        
    	    [Display(Name = "Warehouse_Name", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  Name { get; set; }
    
    		    
    		/// <summary>
    		/// Type
    		/// </summary>        
    	    [Display(Name = "Warehouse_Type", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(10, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  Type { get; set; }
    
    		    
    		/// <summary>
    		/// Winit Warehouse
    		/// </summary>        
    	    [Display(Name = "Warehouse_WinitWarehouse", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  WinitWarehouse { get; set; }
    
    		    
    		/// <summary>
    		/// Create By
    		/// </summary>        
    	    [Display(Name = "Warehouse_CreateBy", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  CreateBy { get; set; }
    
    		    
    		/// <summary>
    		/// Create At
    		/// </summary>        
    	    [Display(Name = "Warehouse_CreateAt", ResourceType = typeof(App_GlobalResources.Resource))]
            [UIHint("DateTime")]
            [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public System.DateTime  CreateAt { get; set; }
    
    		    
    		/// <summary>
    		/// Update By
    		/// </summary>        
    	    [Display(Name = "Warehouse_UpdateBy", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  UpdateBy { get; set; }
    
    		    
    		/// <summary>
    		/// Update At
    		/// </summary>        
    	    [Display(Name = "Warehouse_UpdateAt", ResourceType = typeof(App_GlobalResources.Resource))]
            [UIHint("DateTime")]
    		public Nullable<System.DateTime>  UpdateAt { get; set; }
    
    		    
    		/// <summary>
    		/// Fulfillable
    		/// </summary>        
    	    [Display(Name = "Warehouse_Fulfillable", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(1, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  Fulfillable { get; set; }
    
    		    
    		/// <summary>
    		/// Location
    		/// </summary>        
    	    [Display(Name = "Warehouse_Location", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(10, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  Location { get; set; }
    
    		    
    		/// <summary>
    		/// Countries
    		/// </summary>        
    	    [Display(Name = "Warehouse_Countries", ResourceType = typeof(App_GlobalResources.Resource))]
    		public string  Countries { get; set; }
    
    		    
    		/// <summary>
    		/// Marketplace
    		/// </summary>        
    	    [Display(Name = "Warehouse_Marketplace", ResourceType = typeof(App_GlobalResources.Resource))]
    		public string  Marketplace { get; set; }
    
    		    
    		/// <summary>
    		/// Company
    		/// </summary>        
    	    [Display(Name = "Warehouse_Company", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  Company { get; set; }
    
    		    
    		/// <summary>
    		/// Default Dispatch
    		/// </summary>        
    	    [Display(Name = "Warehouse_DefaultDispatch", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<bool>  DefaultDispatch { get; set; }
    
    		    
    		/// <summary>
    		/// Default RMA
    		/// </summary>        
    	    [Display(Name = "Warehouse_DefaultRMA", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<bool>  DefaultRMA { get; set; }
    
    		    
    		/// <summary>
    		/// Address1
    		/// </summary>        
    	    [Display(Name = "Warehouse_Address1", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  Address1 { get; set; }
    
    		    
    		/// <summary>
    		/// Address2
    		/// </summary>        
    	    [Display(Name = "Warehouse_Address2", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  Address2 { get; set; }
    
    		    
    		/// <summary>
    		/// City
    		/// </summary>        
    	    [Display(Name = "Warehouse_City", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  City { get; set; }
    
    		    
    		/// <summary>
    		/// State
    		/// </summary>        
    	    [Display(Name = "Warehouse_State", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  State { get; set; }
    
    		    
    		/// <summary>
    		/// Postcode
    		/// </summary>        
    	    [Display(Name = "Warehouse_Postcode", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  Postcode { get; set; }
    
    		    
    		/// <summary>
    		/// Country
    		/// </summary>        
    	    [Display(Name = "Warehouse_Country", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  Country { get; set; }
    
    		    
    		/// <summary>
    		/// Phone
    		/// </summary>        
    	    [Display(Name = "Warehouse_Phone", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  Phone { get; set; }
    
    		    
    	}
    }
    
}
