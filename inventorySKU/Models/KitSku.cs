//------------------------------------------------------------------------------
// <auto-generated>
//     這個程式碼是由範本產生。
//
//     對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//     如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace inventorySKU.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class KitSku
    {
        public string Sku { get; set; }
        public string ParentKit { get; set; }
        public int Qty { get; set; }
        public string CreateBy { get; set; }
        public System.DateTime CreateAt { get; set; }
        public string UpdateBy { get; set; }
        public Nullable<System.DateTime> UpdateAt { get; set; }
    
        public virtual Sku GetSku { get; set; }
        public virtual Sku GetParent { get; set; }
    }
}
