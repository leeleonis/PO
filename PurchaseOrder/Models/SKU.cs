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
    
    public partial class SKU
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SKU()
        {
            this.GetKit = new HashSet<KitSku>();
            this.Sku_Attribute = new HashSet<Sku_Attribute>();
            this.Sku_PackageContent = new HashSet<Sku_PackageContent>();
            this.SkuPicture = new HashSet<SkuPicture>();
            this.SkuLang = new HashSet<SkuLang>();
            this.PriceGroup = new HashSet<PriceGroup>();
            this.PurchaseSKU = new HashSet<PurchaseSKU>();
            this.TransferSKU = new HashSet<TransferSKU>();
            this.RMASKU = new HashSet<RMASKU>();
            this.RMASerialsLlist = new HashSet<RMASerialsLlist>();
            this.OrderItems = new HashSet<Items>();
            this.OrderSerials = new HashSet<OrderSerials>();
        }
    
        public bool IsEnable { get; set; }
        public string SkuID { get; set; }
        public int Company { get; set; }
        public byte Type { get; set; }
        public string ParentSku { get; set; }
        public string ParentShadow { get; set; }
        public int Condition { get; set; }
        public int Category { get; set; }
        public int Brand { get; set; }
        public string EAN { get; set; }
        public string UPC { get; set; }
        public bool Replenishable { get; set; }
        public bool SerialTracking { get; set; }
        public bool Battery { get; set; }
        public byte Status { get; set; }
        public string eBayTitle { get; set; }
        public string CreateBy { get; set; }
        public System.DateTime CreateAt { get; set; }
        public string UpdateBy { get; set; }
        public Nullable<System.DateTime> UpdateAt { get; set; }
    
        public virtual Brand GetBrand { get; set; }
        public virtual Condition GetCondition { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<KitSku> GetKit { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Sku_Attribute> Sku_Attribute { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Sku_PackageContent> Sku_PackageContent { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SkuPicture> SkuPicture { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SkuLang> SkuLang { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PriceGroup> PriceGroup { get; set; }
        public virtual Company GetCompany { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PurchaseSKU> PurchaseSKU { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TransferSKU> TransferSKU { get; set; }
        public virtual Logistic Logistic { get; set; }
        public virtual SkuType SkuType { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RMASKU> RMASKU { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RMASerialsLlist> RMASerialsLlist { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Items> OrderItems { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderSerials> OrderSerials { get; set; }
    }
}
