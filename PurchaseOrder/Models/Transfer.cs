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
    
    public partial class Transfer
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Transfer()
        {
            this.TransferSKU = new HashSet<TransferSKU>();
            this.PurchaseNote = new HashSet<PurchaseNote>();
        }
    
        public bool IsEnable { get; set; }
        public int ID { get; set; }
        public string ExternalTra { get; set; }
        public string Title { get; set; }
        public Nullable<int> FromWID { get; set; }
        public Nullable<int> ToWID { get; set; }
        public Nullable<int> TotalQTY { get; set; }
        public string Status { get; set; }
        public string CreateBy { get; set; }
        public System.DateTime CreateAt { get; set; }
        public string UpdateBy { get; set; }
        public Nullable<System.DateTime> UpdateAt { get; set; }
        public string Interim { get; set; }
        public string Carrier { get; set; }
        public string Tracking { get; set; }
    
        public virtual Warehouse WarehouseFrom { get; set; }
        public virtual Warehouse WarehouseTo { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TransferSKU> TransferSKU { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PurchaseNote> PurchaseNote { get; set; }
    }
}
