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
    
    public partial class RMASerialsLlist
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public RMASerialsLlist()
        {
            this.RMASerialsLlistC = new HashSet<RMASerialsLlist>();
        }
    
        public int ID { get; set; }
        public Nullable<int> RMASKUID { get; set; }
        public Nullable<int> PID { get; set; }
        public string SerialsNo { get; set; }
        public Nullable<int> SerialsQTY { get; set; }
        public string CreateBy { get; set; }
        public System.DateTime CreateAt { get; set; }
        public string UpdateBy { get; set; }
        public Nullable<System.DateTime> UpdateAt { get; set; }
        public string ReceivedBy { get; set; }
        public Nullable<System.DateTime> ReceivedAt { get; set; }
        public Nullable<int> WarehouseID { get; set; }
        public string SerialsType { get; set; }
        public Nullable<int> ServiceID { get; set; }
        public string Reason { get; set; }
        public Nullable<int> TransferSKUID { get; set; }
        public string NewSkuNo { get; set; }
        public string NewSKUCreateBy { get; set; }
        public Nullable<System.DateTime> NewSKUCreateAt { get; set; }
        public bool IsEnable { get; set; }
    
        public virtual RMASKU RMASKU { get; set; }
        public virtual Warehouse Warehouse { get; set; }
        public virtual TransferSKU TransferSKU { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RMASerialsLlist> RMASerialsLlistC { get; set; }
        public virtual RMASerialsLlist RMASerialsLlistP { get; set; }
        public virtual SKU SKU { get; set; }
    }
}
