﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class InventorySKUEntities : DbContext
    {
        public InventorySKUEntities()
            : base("name=InventorySKUEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<AdminGroup> AdminGroup { get; set; }
        public virtual DbSet<AdminUser> AdminUser { get; set; }
        public virtual DbSet<API> API { get; set; }
        public virtual DbSet<Auth> Auth { get; set; }
        public virtual DbSet<Brand> Brand { get; set; }
        public virtual DbSet<Carrier> Carrier { get; set; }
        public virtual DbSet<Company> Company { get; set; }
        public virtual DbSet<DirectLine> DirectLine { get; set; }
        public virtual DbSet<Menu> Menu { get; set; }
        public virtual DbSet<MenuLang> MenuLang { get; set; }
        public virtual DbSet<ShippingMethod> ShippingMethod { get; set; }
        public virtual DbSet<Sku_Attribute> Sku_Attribute { get; set; }
        public virtual DbSet<SkuAttribute> SkuAttribute { get; set; }
        public virtual DbSet<SkuAttributeLang> SkuAttributeLang { get; set; }
        public virtual DbSet<SkuAttributeType> SkuAttributeType { get; set; }
        public virtual DbSet<SkuLang> SkuLang { get; set; }
        public virtual DbSet<SkuType> SkuType { get; set; }
        public virtual DbSet<SkuTypeLang> SkuTypeLang { get; set; }
        public virtual DbSet<Warehouse> Warehouse { get; set; }
        public virtual DbSet<Sku> Sku { get; set; }
        public virtual DbSet<ConditionLang> ConditionLang { get; set; }
        public virtual DbSet<Condition> Condition { get; set; }
        public virtual DbSet<KitSku> KitSku { get; set; }
        public virtual DbSet<SkuPicture> SkuPicture { get; set; }
    }
}
