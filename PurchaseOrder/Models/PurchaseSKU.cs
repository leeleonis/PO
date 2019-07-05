//------------------------------------------------------------------------------
// <auto-generated>
//     這個程式碼是由範本產生。
//
//     對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//     如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace PurchaseOrderSys.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class PurchaseSKU
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PurchaseSKU()
        {
            this.ImgFile = new HashSet<ImgFile>();
            this.SerialsLlist = new HashSet<SerialsLlist>();
        }
    
        public bool IsEnable { get; set; }
        public int ID { get; set; }
        public string Name { get; set; }
        public Nullable<int> PurchaseOrderID { get; set; }
        public Nullable<int> CreditMemoID { get; set; }
        public string SkuNo { get; set; }
        public Nullable<int> QTYOrdered { get; set; }
        public Nullable<int> QTYFulfilled { get; set; }
        public Nullable<int> QTYReturned { get; set; }
        public Nullable<int> QTYReceived { get; set; }
        public Nullable<decimal> Price { get; set; }
        public Nullable<decimal> Discount { get; set; }
        public Nullable<decimal> DiscountedPrice { get; set; }
        public Nullable<decimal> ShippingCost { get; set; }
        public Nullable<decimal> Other { get; set; }
        public Nullable<decimal> TotalRefunded { get; set; }
        public string VendorSKU { get; set; }
        public Nullable<decimal> Credit { get; set; }
        public Nullable<int> CreditQTY { get; set; }
        public string UPCEAN { get; set; }
        public Nullable<System.DateTime> ReceivedDate { get; set; }
        public string CreateBy { get; set; }
        public System.DateTime CreateAt { get; set; }
        public string UpdateBy { get; set; }
        public Nullable<System.DateTime> UpdateAt { get; set; }
        public string ReceivedBy { get; set; }
        public Nullable<System.DateTime> ReceivedAt { get; set; }
    
        public virtual CreditMemo CreditMemo { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ImgFile> ImgFile { get; set; }
        public virtual PurchaseOrder PurchaseOrder { get; set; }
        public virtual SKU SKU { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SerialsLlist> SerialsLlist { get; set; }
    }
}
