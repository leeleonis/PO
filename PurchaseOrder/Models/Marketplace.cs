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
    
    public partial class Marketplace
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Marketplace()
        {
            this.PriceGroup = new HashSet<PriceGroup>();
        }
    
        public bool IsEnable { get; set; }
        public int ID { get; set; }
        public string FullName { get; set; }
        public string GlobalID { get; set; }
        public string CountryCode { get; set; }
        public Nullable<int> CompanyID { get; set; }
        public Nullable<int> CurrencyID { get; set; }
        public Nullable<int> NetoGroup { get; set; }
        public Nullable<bool> Status { get; set; }
        public string CreateBy { get; set; }
        public System.DateTime CreateAt { get; set; }
        public string UpdateBy { get; set; }
        public Nullable<System.DateTime> UpdateAt { get; set; }
    
        public virtual Currency Currency { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PriceGroup> PriceGroup { get; set; }
        public virtual NetoGroup GetNetoGroup { get; set; }
        public virtual Company Company { get; set; }
    }
}
