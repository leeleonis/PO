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
    
    public partial class OrderAddresses
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public OrderAddresses()
        {
            this.IsEnable = true;
        }
    
        public bool IsEnable { get; set; }
        public int ID { get; set; }
        public Nullable<int> SCID { get; set; }
        public int OrderID { get; set; }
        public bool Type { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Postcode { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public string Update_by { get; set; }
        public Nullable<System.DateTime> Update_at { get; set; }
        public string Create_by { get; set; }
        public System.DateTime Create_at { get; set; }
    
        public virtual Orders GetOrder { get; set; }
    }
}
