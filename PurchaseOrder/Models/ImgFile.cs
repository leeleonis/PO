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
    
    public partial class ImgFile
    {
        public bool IsEnable { get; set; }
        public int ID { get; set; }
        public Nullable<int> PurchaseOrderID { get; set; }
        public Nullable<int> CreditMemoID { get; set; }
        public string Url { get; set; }
        public string CreateBy { get; set; }
        public System.DateTime CreateAt { get; set; }
        public string UpdateBy { get; set; }
        public Nullable<System.DateTime> UpdateAt { get; set; }
        public string ImgType { get; set; }
    
        public virtual CreditMemo CreditMemo { get; set; }
        public virtual PurchaseOrder PurchaseOrder { get; set; }
    }
}