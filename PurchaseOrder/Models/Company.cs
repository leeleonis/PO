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
    
    public partial class Company
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Company()
        {
            this.PurchaseOrder = new HashSet<PurchaseOrder>();
            this.SKU = new HashSet<SKU>();
            this.Marketplace = new HashSet<Marketplace>();
            this.Orders = new HashSet<Orders>();
        }
    
        public bool IsEnable { get; set; }
        public int ID { get; set; }
        public string Name { get; set; }
        public string ShadowSuffix { get; set; }
        public Nullable<int> ParentID { get; set; }
        public Nullable<int> RelateID { get; set; }
        public string eBayAccountID { get; set; }
        public string AmazonAccountID { get; set; }
        public string CreateBy { get; set; }
        public System.DateTime CreateAt { get; set; }
        public string UpdateBy { get; set; }
        public Nullable<System.DateTime> UpdateAt { get; set; }
        public string CompanyNo { get; set; }
        public int CurrencyID { get; set; }
        public Nullable<int> CompanySCID { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PurchaseOrder> PurchaseOrder { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SKU> SKU { get; set; }
        public virtual Currency Currency { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Marketplace> Marketplace { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Orders> Orders { get; set; }
    }
}
