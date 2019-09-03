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
    
    public partial class RMAOrderTracking
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public RMAOrderTracking()
        {
            this.RMAOrderSerialsLlist = new HashSet<RMAOrderSerialsLlist>();
            this.PurchaseNote = new HashSet<PurchaseNote>();
        }
    
        public bool IsEnable { get; set; }
        public int ID { get; set; }
        public string ReturnTracking { get; set; }
        public string Carrier { get; set; }
        public string ToName { get; set; }
        public string ToAddress1 { get; set; }
        public string ToAddress2 { get; set; }
        public string ToCity { get; set; }
        public string ToState { get; set; }
        public string ToPostcode { get; set; }
        public string ToCountry { get; set; }
        public string FromName { get; set; }
        public string FromAddress1 { get; set; }
        public string FromAddress2 { get; set; }
        public string FromCity { get; set; }
        public string FromState { get; set; }
        public string FromPostcode { get; set; }
        public string FromCountry { get; set; }
        public Nullable<decimal> Weight { get; set; }
        public Nullable<decimal> Length { get; set; }
        public Nullable<decimal> Width { get; set; }
        public Nullable<decimal> Heigth { get; set; }
        public Nullable<bool> Insurance { get; set; }
        public Nullable<int> ETA { get; set; }
        public Nullable<decimal> DeclareValue { get; set; }
        public Nullable<decimal> EstimatedCost { get; set; }
        public string ShippingMethod { get; set; }
        public string CreateBy { get; set; }
        public System.DateTime CreateAt { get; set; }
        public string UpdateBy { get; set; }
        public Nullable<System.DateTime> UpdateAt { get; set; }
        public string Memo { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RMAOrderSerialsLlist> RMAOrderSerialsLlist { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PurchaseNote> PurchaseNote { get; set; }
    }
}