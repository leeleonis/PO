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
    
    public partial class WarehouseSummary
    {
        public bool IsEnable { get; set; }
        public int ID { get; set; }
        public int WarehouseID { get; set; }
        public string Type { get; set; }
        public string Val { get; set; }
        public string Url { get; set; }
    
        public virtual Warehouse Warehouse { get; set; }
    }
}
