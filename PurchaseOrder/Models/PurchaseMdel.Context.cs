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
    
        public virtual DbSet<PurchaseOrder> PurchaseOrder { get; set; }
        public virtual DbSet<Company> Company { get; set; }
        public virtual DbSet<SerialsLlist> SerialsLlist { get; set; }
        public virtual DbSet<SKU> SKU { get; set; }
        public virtual DbSet<SkuLang> SkuLang { get; set; }
        public virtual DbSet<PurchaseNote> PurchaseNote { get; set; }
        public virtual DbSet<CMCreditNote> CMCreditNote { get; set; }
        public virtual DbSet<CMReplacement> CMReplacement { get; set; }
        public virtual DbSet<PurchaseSKU> PurchaseSKU { get; set; }
        public virtual DbSet<VendorLIst> VendorLIst { get; set; }
        public virtual DbSet<Warehouse> Warehouse { get; set; }
        public virtual DbSet<AdminUser> AdminUser { get; set; }
        public virtual DbSet<WarehouseUser> WarehouseUser { get; set; }
        public virtual DbSet<Transfer> Transfer { get; set; }
        public virtual DbSet<TransferSKU> TransferSKU { get; set; }
    }
}
