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
    
    public partial class ConditionLang
    {
        public int ConditionID { get; set; }
        public string LangID { get; set; }
        public string Name { get; set; }
        public string CreateBy { get; set; }
        public System.DateTime CreateAt { get; set; }
        public string UpdateBy { get; set; }
        public Nullable<System.DateTime> UpdateAt { get; set; }
    
        public virtual Condition Condition { get; set; }
    }
}
