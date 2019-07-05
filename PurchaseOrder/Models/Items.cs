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
    
    public partial class Items
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Items()
        {
            this.IsEnable = true;
            this.Serials = new HashSet<OrderSerials>();
        }
    
        public bool IsEnable { get; set; }
        public int ID { get; set; }
        public Nullable<int> SCID { get; set; }
        public int OrderID { get; set; }
        public int PackageID { get; set; }
        public string Sku { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal ExportValue { get; set; }
        public decimal DLExportValue { get; set; }
        public int Qty { get; set; }
        public string eBayItemID { get; set; }
        public string SalesRecordNumber { get; set; }
        public Nullable<int> RMAID { get; set; }
        public string Update_by { get; set; }
        public Nullable<System.DateTime> Update_at { get; set; }
        public string Create_by { get; set; }
        public System.DateTime Create_at { get; set; }
    
        public virtual Orders GetOrder { get; set; }
        public virtual Packages GetPackage { get; set; }
        public virtual SKU GetSku { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderSerials> Serials { get; set; }
    }
}
