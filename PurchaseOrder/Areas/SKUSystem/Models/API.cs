//------------------------------------------------------------------------------
// <auto-generated>
//     這個程式碼是由範本產生。
//
//     對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//     如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace PurchaseOrderSys.Areas.SKUSystem.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class API
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public API()
        {
            this.ShippingMethod = new HashSet<ShippingMethod>();
        }
    
        public bool IsEnable { get; set; }
        public bool IsTest { get; set; }
        public int ID { get; set; }
        public string Name { get; set; }
        public byte Type { get; set; }
        public string ApiKey { get; set; }
        public string ApiAccount { get; set; }
        public string ApiPassword { get; set; }
        public string ApiMeter { get; set; }
        public string ApiHub { get; set; }
        public string CreateBy { get; set; }
        public System.DateTime CreateAt { get; set; }
        public string UpdateBy { get; set; }
        public Nullable<System.DateTime> UpdateAt { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ShippingMethod> ShippingMethod { get; set; }
    }
}
