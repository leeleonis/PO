using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PurchaseOrderSys
{
    public class BrandFilter
    {
        private string nameField;

        public Nullable<int> ID { get; set; }
        public Nullable<int> NetoID { get; set; }
        public string Name { get { return this.nameField; } set { nameField = !string.IsNullOrEmpty(value) ? value.Trim() : value; } }
        public Nullable<bool> IsExport { get; set; }
    }

    public class CompanyFilter
    {
        private string nameField;
        private string shandowSuffixField;
        private string eBayAccountIDField;
        private string amazonAccountIDField;

        public Nullable<int> ID { get; set; }
        public string Name { get { return this.nameField; } set { nameField = !string.IsNullOrEmpty(value) ? value.Trim() : value; } }
        public string ShandowSuffix { get { return this.shandowSuffixField; } set { shandowSuffixField = !string.IsNullOrEmpty(value) ? value.Trim() : value; } }
        public Nullable<int> ParentID { get; set; }
        public Nullable<int> RelateID { get; set; }
        public string eBayAccountID { get { return this.eBayAccountIDField; } set { eBayAccountIDField = !string.IsNullOrEmpty(value) ? value.Trim() : value; } }
        public string AmazonAccountID { get { return this.amazonAccountIDField; } set { amazonAccountIDField = !string.IsNullOrEmpty(value) ? value.Trim() : value; } }
    }

    public class SkuAttributeTypeFilter
    {
        private string nameField;
        public Nullable<int> ID { get; set; }
        public string Name { get { return this.nameField; } set { nameField = !string.IsNullOrEmpty(value) ? value.Trim() : value; } }
    }

    public class SkuAttributeFilter : SkuAttributeTypeFilter
    {
        public string LangID { get; set; }
        public Nullable<int> Type { get; set; }
    }

    public class SkuTypeFilter
    {
        private string nameField;
        public Nullable<int> ID { get; set; }
        public string LangID { get; set; }
        public Nullable<int> NetoID { get; set; }
        public string Name { get { return this.nameField; } set { nameField = !string.IsNullOrEmpty(value) ? value.Trim() : value; } }
    }

    public class SkuFilter
    {
        private string idField;
        private string parentSkuField;
        private string nameField;
        private string upcField;
        private string eanField;
        public string ID { get { return this.idField; } set { idField = !string.IsNullOrEmpty(value) ? value.Trim() : value; } }
        public string LangID { get; set; }
        public string ParentSku { get { return this.parentSkuField; } set { parentSkuField = !string.IsNullOrEmpty(value) ? value.Trim() : value; } }
        public string Name { get { return this.nameField; } set { nameField = !string.IsNullOrEmpty(value) ? value.Trim() : value; } }
        public Nullable<int> Condition { get; set; }
        public Nullable<int> Category { get; set; }
        public string UPC { get { return this.nameField; } set { nameField = !string.IsNullOrEmpty(value) ? value.Trim() : value; } }
        public string EAN { get { return this.nameField; } set { nameField = !string.IsNullOrEmpty(value) ? value.Trim() : value; } }
        public Nullable<int> Replenishable { get; set; }
        public Nullable<int> Status { get; set; }
    }
}