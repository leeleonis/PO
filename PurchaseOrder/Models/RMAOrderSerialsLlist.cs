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
    
    public partial class RMAOrderSerialsLlist
    {
        public bool IsEnable { get; set; }
        public int ID { get; set; }
        public Nullable<int> RMASKUID { get; set; }
        public string SerialsNo { get; set; }
        public Nullable<int> SerialsQTY { get; set; }
        public string CreateBy { get; set; }
        public System.DateTime CreateAt { get; set; }
        public string ReturnTracking { get; set; }
        public string Carrier { get; set; }
    
        public virtual RMASKU RMASKU { get; set; }
    }
}
