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
    
    public partial class Packages
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Packages()
        {
            this.IsEnable = true;
            this.Items = new HashSet<Items>();
        }
    
        public bool IsEnable { get; set; }
        public int ID { get; set; }
        public Nullable<int> SCID { get; set; }
        public int OrderID { get; set; }
        public string CarrierBox { get; set; }
        public int ShippingMethod { get; set; }
        public byte Export { get; set; }
        public byte ExportMethod { get; set; }
        public decimal ExportValue { get; set; }
        public int ExportCurrency { get; set; }
        public bool UploadTracking { get; set; }
        public string Tracking { get; set; }
        public byte DLExport { get; set; }
        public byte DLExportMethod { get; set; }
        public decimal DLExportValue { get; set; }
        public int DLExportCurrency { get; set; }
        public bool DLUploadTracking { get; set; }
        public string DLTracking { get; set; }
        public int ShipWarehouse { get; set; }
        public int ReturnWarehouse { get; set; }
        public bool ShippingStatus { get; set; }
        public string Update_by { get; set; }
        public Nullable<System.DateTime> Update_at { get; set; }
        public string Create_by { get; set; }
        public System.DateTime Create_at { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Items> Items { get; set; }
        public virtual Orders GetOrder { get; set; }
        public virtual ShippingMethods GetMethod { get; set; }
        public virtual Currency GetCurrency { get; set; }
        public virtual Currency GetDLCurrency { get; set; }
        public virtual Warehouse GetWarehouse { get; set; }
        public virtual Warehouse GetReturnWarehouse { get; set; }
    }
}
