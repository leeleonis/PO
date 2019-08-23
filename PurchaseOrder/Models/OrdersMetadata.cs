using System; 
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace PurchaseOrderSys.Models
{
    
    /// <summary>
    /// Orders class
    /// </summary>
    [MetadataType(typeof(OrdersMetadata))]
    public  partial class Orders
    {
    
    	/// <summary>
    	/// Orders Metadata class
    	/// </summary>
    	public   class OrdersMetadata
    	{
    		    
    		/// <summary>
    		/// Is Enable
    		/// </summary>        
    	    [Display(Name = "Orders_IsEnable", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public bool  IsEnable { get; set; }
    
    		    
    		/// <summary>
    		/// Is Rush
    		/// </summary>        
    	    [Display(Name = "Orders_IsRush", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public bool  IsRush { get; set; }
    
    		    
    		/// <summary>
    		/// ID
    		/// </summary>        
    	    [Display(Name = "Orders_ID", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public int  ID { get; set; }
    
    		    
    		/// <summary>
    		/// Order Parent
    		/// </summary>        
    	    [Display(Name = "Orders_OrderParent", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<int>  OrderParent { get; set; }
    
    		    
    		/// <summary>
    		/// Order Source ID
    		/// </summary>        
    	    [Display(Name = "Orders_OrderSourceID", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  OrderSourceID { get; set; }
    
    		    
    		/// <summary>
    		/// SCID
    		/// </summary>        
    	    [Display(Name = "Orders_SCID", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<int>  SCID { get; set; }
    
    		    
    		/// <summary>
    		/// RMAID
    		/// </summary>        
    	    [Display(Name = "Orders_RMAID", ResourceType = typeof(App_GlobalResources.Resource))]
    		public Nullable<int>  RMAID { get; set; }
    
    		    
    		/// <summary>
    		/// Company
    		/// </summary>        
    	    [Display(Name = "Orders_Company", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public int  Company { get; set; }
    
    		    
    		/// <summary>
    		/// Channel
    		/// </summary>        
    	    [Display(Name = "Orders_Channel", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public byte  Channel { get; set; }
    
    		    
    		/// <summary>
    		/// Customer ID
    		/// </summary>        
    	    [Display(Name = "Orders_CustomerID", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  CustomerID { get; set; }
    
    		    
    		/// <summary>
    		/// Customer Email
    		/// </summary>        
    	    [Display(Name = "Orders_CustomerEmail", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(100, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  CustomerEmail { get; set; }
    
    		    
    		/// <summary>
    		/// Order Type
    		/// </summary>        
    	    [Display(Name = "Orders_OrderType", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public byte  OrderType { get; set; }
    
    		    
    		/// <summary>
    		/// Order Status
    		/// </summary>        
    	    [Display(Name = "Orders_OrderStatus", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public byte  OrderStatus { get; set; }
    
    		    
    		/// <summary>
    		/// Order Date
    		/// </summary>        
    	    [Display(Name = "Orders_OrderDate", ResourceType = typeof(App_GlobalResources.Resource))]
            [UIHint("DateTime")]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public System.DateTime  OrderDate { get; set; }
    
    		    
    		/// <summary>
    		/// Payment Status
    		/// </summary>        
    	    [Display(Name = "Orders_PaymentStatus", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public byte  PaymentStatus { get; set; }
    
    		    
    		/// <summary>
    		/// Payment Date
    		/// </summary>        
    	    [Display(Name = "Orders_PaymentDate", ResourceType = typeof(App_GlobalResources.Resource))]
            [UIHint("DateTime")]
    		public Nullable<System.DateTime>  PaymentDate { get; set; }
    
    		    
    		/// <summary>
    		/// Fulfillment Status
    		/// </summary>        
    	    [Display(Name = "Orders_FulfillmentStatus", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public byte  FulfillmentStatus { get; set; }
    
    		    
    		/// <summary>
    		/// Fulfilled Date
    		/// </summary>        
    	    [Display(Name = "Orders_FulfilledDate", ResourceType = typeof(App_GlobalResources.Resource))]
            [UIHint("DateTime")]
    		public Nullable<System.DateTime>  FulfilledDate { get; set; }
    
    		    
    		/// <summary>
    		/// Buyer Note
    		/// </summary>        
    	    [Display(Name = "Orders_BuyerNote", ResourceType = typeof(App_GlobalResources.Resource))]
    		public string  BuyerNote { get; set; }
    
    		    
    		/// <summary>
    		/// Comment
    		/// </summary>        
    	    [Display(Name = "Orders_Comment", ResourceType = typeof(App_GlobalResources.Resource))]
    		public string  Comment { get; set; }
    
    		    
    		/// <summary>
    		/// Update By
    		/// </summary>        
    	    [Display(Name = "Orders_UpdateBy", ResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  UpdateBy { get; set; }
    
    		    
    		/// <summary>
    		/// Update At
    		/// </summary>        
    	    [Display(Name = "Orders_UpdateAt", ResourceType = typeof(App_GlobalResources.Resource))]
            [UIHint("DateTime")]
    		public Nullable<System.DateTime>  UpdateAt { get; set; }
    
    		    
    		/// <summary>
    		/// Create By
    		/// </summary>        
    	    [Display(Name = "Orders_CreateBy", ResourceType = typeof(App_GlobalResources.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public string  CreateBy { get; set; }
    
    		    
    		/// <summary>
    		/// Create At
    		/// </summary>        
    	    [Display(Name = "Orders_CreateAt", ResourceType = typeof(App_GlobalResources.Resource))]
            [UIHint("DateTime")]
            [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(App_GlobalResources.Resource))]
    		public System.DateTime  CreateAt { get; set; }
    
    		    
    	}
    }
    
}
