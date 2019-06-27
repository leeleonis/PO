﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class PurchaseOrderEntities : DbContext
    {
        public PurchaseOrderEntities()
            : base("name=PurchaseOrderEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<AdminGroup> AdminGroup { get; set; }
        public virtual DbSet<AdminUser> AdminUser { get; set; }
        public virtual DbSet<Auth> Auth { get; set; }
        public virtual DbSet<Brand> Brand { get; set; }
        public virtual DbSet<CMCreditNote> CMCreditNote { get; set; }
        public virtual DbSet<CMReplacement> CMReplacement { get; set; }
        public virtual DbSet<Condition> Condition { get; set; }
        public virtual DbSet<ConditionLang> ConditionLang { get; set; }
        public virtual DbSet<CreditMemo> CreditMemo { get; set; }
        public virtual DbSet<DirectLine> DirectLine { get; set; }
        public virtual DbSet<KitSku> KitSku { get; set; }
        public virtual DbSet<Menu> Menu { get; set; }
        public virtual DbSet<MenuLang> MenuLang { get; set; }
        public virtual DbSet<PackageContent> PackageContent { get; set; }
        public virtual DbSet<PackageContentLang> PackageContentLang { get; set; }
        public virtual DbSet<PurchaseNote> PurchaseNote { get; set; }
        public virtual DbSet<PurchaseOrder> PurchaseOrder { get; set; }
        public virtual DbSet<PurchaseSKU> PurchaseSKU { get; set; }
        public virtual DbSet<SKU> SKU { get; set; }
        public virtual DbSet<Sku_Attribute> Sku_Attribute { get; set; }
        public virtual DbSet<Sku_PackageContent> Sku_PackageContent { get; set; }
        public virtual DbSet<SkuAttribute> SkuAttribute { get; set; }
        public virtual DbSet<SkuAttributeLang> SkuAttributeLang { get; set; }
        public virtual DbSet<SkuAttributeType> SkuAttributeType { get; set; }
        public virtual DbSet<SkuPicture> SkuPicture { get; set; }
        public virtual DbSet<SkuTypeLang> SkuTypeLang { get; set; }
        public virtual DbSet<TransferSKU> TransferSKU { get; set; }
        public virtual DbSet<VendorLIst> VendorLIst { get; set; }
        public virtual DbSet<Warehouse> Warehouse { get; set; }
        public virtual DbSet<WarehouseSummary> WarehouseSummary { get; set; }
        public virtual DbSet<WarehouseUser> WarehouseUser { get; set; }
        public virtual DbSet<Carriers> Carriers { get; set; }
        public virtual DbSet<SkuLang> SkuLang { get; set; }
        public virtual DbSet<ImgFile> ImgFile { get; set; }
        public virtual DbSet<Currency> Currency { get; set; }
        public virtual DbSet<NetoGroup> NetoGroup { get; set; }
        public virtual DbSet<PriceGroup> PriceGroup { get; set; }
        public virtual DbSet<Marketplace> Marketplace { get; set; }
        public virtual DbSet<Company> Company { get; set; }
        public virtual DbSet<SerialsLlist> SerialsLlist { get; set; }
        public virtual DbSet<OrderLog> OrderLog { get; set; }
        public virtual DbSet<Transfer> Transfer { get; set; }
        public virtual DbSet<BoxType> BoxType { get; set; }
        public virtual DbSet<Logistic> Logistic { get; set; }
        public virtual DbSet<RMASKU> RMASKU { get; set; }
        public virtual DbSet<SkuType> SkuType { get; set; }
        public virtual DbSet<RMA> RMA { get; set; }
        public virtual DbSet<RMASerialsLlist> RMASerialsLlist { get; set; }
        public virtual DbSet<RMAOrderSerialsLlist> RMAOrderSerialsLlist { get; set; }
        public virtual DbSet<WinitTransfer> WinitTransfer { get; set; }
        public virtual DbSet<WinitTransferBox> WinitTransferBox { get; set; }
        public virtual DbSet<WinitTransferBoxItem> WinitTransferBoxItem { get; set; }
        public virtual DbSet<Items> Items { get; set; }
        public virtual DbSet<OrderActionLogs> OrderActionLogs { get; set; }
        public virtual DbSet<Orders> Orders { get; set; }
        public virtual DbSet<Packages> Packages { get; set; }
        public virtual DbSet<Payments> Payments { get; set; }
        public virtual DbSet<ShippingMethods> ShippingMethods { get; set; }
        public virtual DbSet<ApiSetting> ApiSetting { get; set; }
        public virtual DbSet<OrderAddresses> OrderAddresses { get; set; }
        public virtual DbSet<OrderSerials> OrderSerials { get; set; }
    }
}
