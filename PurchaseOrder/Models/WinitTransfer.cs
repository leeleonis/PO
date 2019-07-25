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
    
    public partial class WinitTransfer
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public WinitTransfer()
        {
            this.WinitTransferBox = new HashSet<WinitTransferBox>();
            this.WinitTransferSKU = new HashSet<WinitTransferSKU>();
        }
    
        public bool IsEnable { get; set; }
        public int TransferID { get; set; }
        public Nullable<int> CompleteBoxes { get; set; }
        public Nullable<decimal> TotalVal { get; set; }
        public string CreateBy { get; set; }
        public System.DateTime CreateAt { get; set; }
        public string UpdateBy { get; set; }
        public Nullable<System.DateTime> UpdateAt { get; set; }
        public string SBarcodeLabelType { get; set; }
        public string BoxLabelSize { get; set; }
        public string WinitOrderNo { get; set; }
        public string PrintPackageLabe { get; set; }
        public string PackingList { get; set; }
        public string CheckList { get; set; }
        public string InvoiceExcel { get; set; }
        public string WinitStatus { get; set; }
    
        public virtual Transfer Transfer { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WinitTransferBox> WinitTransferBox { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WinitTransferSKU> WinitTransferSKU { get; set; }
    }
}
