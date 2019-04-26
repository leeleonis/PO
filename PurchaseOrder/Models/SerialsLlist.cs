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
    
    public partial class SerialsLlist
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SerialsLlist()
        {
            this.SerialsLlistC = new HashSet<SerialsLlist>();
        }
    
        public int ID { get; set; }
        public Nullable<int> PurchaseSKUID { get; set; }
        public Nullable<int> PID { get; set; }
        public string SerialsNo { get; set; }
        public Nullable<int> SerialsQTY { get; set; }
        public string CreateBy { get; set; }
        public System.DateTime CreateAt { get; set; }
        public string UpdateBy { get; set; }
        public Nullable<System.DateTime> UpdateAt { get; set; }
        public string ReceivedBy { get; set; }
        public Nullable<System.DateTime> ReceivedAt { get; set; }
        public Nullable<int> TransferSKUID { get; set; }
        public Nullable<int> OrderID { get; set; }
        public string SerialsType { get; set; }
        public string Memo { get; set; }
        public bool IsEnable { get; set; }
        public Nullable<int> RMASKUID { get; set; }
    
        public virtual Orders Orders { get; set; }
        public virtual PurchaseSKU PurchaseSKU { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SerialsLlist> SerialsLlistC { get; set; }
        public virtual SerialsLlist SerialsLlistP { get; set; }
        public virtual TransferSKU TransferSKU { get; set; }
    }
}
