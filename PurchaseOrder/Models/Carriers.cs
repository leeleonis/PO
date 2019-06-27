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
    
    public partial class Carriers
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Carriers()
        {
            this.IsEnable = true;
            this.FirstMiles = new HashSet<ShippingMethods>();
            this.LastMiles = new HashSet<ShippingMethods>();
        }
    
        public bool IsEnable { get; set; }
        public int ID { get; set; }
        public string Name { get; set; }
        public int MethodType { get; set; }
        public int BoxType { get; set; }
        public string Email { get; set; }
        public Nullable<int> Api { get; set; }
        public string PrinterName { get; set; }
        public string Update_by { get; set; }
        public Nullable<System.DateTime> Update_at { get; set; }
        public string Create_by { get; set; }
        public System.DateTime Create_at { get; set; }
    
        public virtual ApiSetting GetApi { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ShippingMethods> FirstMiles { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ShippingMethods> LastMiles { get; set; }
    }
}
