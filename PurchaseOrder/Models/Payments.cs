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
    
    public partial class Payments
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Payments()
        {
            this.IsEnable = true;
        }
    
        public bool IsEnable { get; set; }
        public int ID { get; set; }
        public Nullable<int> SCID { get; set; }
        public int OrderID { get; set; }
        public byte Status { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public int Gateway { get; set; }
        public decimal TotalValue { get; set; }
        public decimal ShippingCharge { get; set; }
        public decimal InsuranceCharge { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal PaymentTotal { get; set; }
        public decimal Refund { get; set; }
        public decimal Balance { get; set; }
        public string TransactionID { get; set; }
        public string PaymentEmail { get; set; }
        public string UpdateBy { get; set; }
        public Nullable<System.DateTime> UpdateAt { get; set; }
        public string CreateBy { get; set; }
        public System.DateTime CreateAt { get; set; }
    
        public virtual Orders GetOrder { get; set; }
    }
}
