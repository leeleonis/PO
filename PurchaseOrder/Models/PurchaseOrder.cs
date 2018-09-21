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
    
    public partial class PurchaseOrder
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PurchaseOrder()
        {
            this.PurchaseSKU = new HashSet<PurchaseSKU>();
            this.PurchaseNote = new HashSet<PurchaseNote>();
            this.CMCreditNote = new HashSet<CMCreditNote>();
            this.CMReplacement = new HashSet<CMReplacement>();
        }
    
        public bool IsEnable { get; set; }
        public int ID { get; set; }
        public Nullable<int> CompanyID { get; set; }
        public Nullable<int> VendorID { get; set; }
        public string POType { get; set; }
        public Nullable<System.DateTime> PODate { get; set; }
        public string ReceiveStatus { get; set; }
        public Nullable<System.DateTime> ReceivedDate { get; set; }
        public Nullable<System.DateTime> ShippedDate { get; set; }
        public string Carrier { get; set; }
        public string Tracking { get; set; }
        public string PaymentStatus { get; set; }
        public Nullable<System.DateTime> PaymentDate { get; set; }
        public Nullable<decimal> PaidAmount { get; set; }
        public string CreateBy { get; set; }
        public System.DateTime CreateAt { get; set; }
        public string UpdateBy { get; set; }
        public Nullable<System.DateTime> UpdateAt { get; set; }
        public string ReceivedBy { get; set; }
        public Nullable<System.DateTime> ReceivedAt { get; set; }
        public string POStatus { get; set; }
        public Nullable<System.DateTime> InvoiceDate { get; set; }
        public string InvoiceNo { get; set; }
        public string PaymentProof { get; set; }
        public string Warehouse { get; set; }
        public string Currency { get; set; }
        public Nullable<decimal> Tax { get; set; }
    
        public virtual Company Company { get; set; }
        public virtual VendorLIst VendorLIst { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PurchaseSKU> PurchaseSKU { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PurchaseNote> PurchaseNote { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CMCreditNote> CMCreditNote { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CMReplacement> CMReplacement { get; set; }
    }
}
