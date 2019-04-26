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
    
    public partial class RMA
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public RMA()
        {
            this.RMASKU = new HashSet<RMASKU>();
            this.PurchaseNote = new HashSet<PurchaseNote>();
            this.CreditMemo = new HashSet<CreditMemo>();
        }
    
        public bool IsEnable { get; set; }
        public int ID { get; set; }
        public Nullable<int> OrderID { get; set; }
        public string SourceID { get; set; }
        public Nullable<int> CompanyID { get; set; }
        public string Country { get; set; }
        public string Status { get; set; }
        public string Action { get; set; }
        public string Reason { get; set; }
        public string ReturnTracking { get; set; }
        public string CreateBy { get; set; }
        public System.DateTime CreateAt { get; set; }
        public string UpdateBy { get; set; }
        public Nullable<System.DateTime> UpdateAt { get; set; }
        public string Carrier { get; set; }
        public Nullable<int> Channel { get; set; }
        public string SourceCaseID { get; set; }
        public string SCRMA { get; set; }
        public Nullable<int> WarehouseID { get; set; }
        public Nullable<decimal> FinalShippingFee { get; set; }
        public Nullable<decimal> RestockingFee { get; set; }
        public Nullable<decimal> OtherCosts { get; set; }
        public Nullable<decimal> ReturnShippingCos { get; set; }
    
        public virtual Warehouse Warehouse { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RMASKU> RMASKU { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PurchaseNote> PurchaseNote { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CreditMemo> CreditMemo { get; set; }
    }
}
