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
        public Nullable<byte> Property { get; set; }
    }

    public class ConditionFilter
    {
        private string nameField;
        private string amazonField;
        private string eBayField;
        private string buy_comField;
        private string newEgg_comField;
        private string searsField;
        private string suffixField;

        public Nullable<int> ID { get; set; }
        public string LangID { get; set; }
        public string Name { get { return nameField; } set { nameField = !string.IsNullOrEmpty(value) ? value.Trim() : value; } }
        public string Amazon { get { return amazonField; } set { amazonField = !string.IsNullOrEmpty(value) ? value.Trim() : value; } }
        public string eBay { get { return eBayField; } set { eBayField = !string.IsNullOrEmpty(value) ? value.Trim() : value; } }
        public string Buy_com { get { return buy_comField; } set { buy_comField = !string.IsNullOrEmpty(value) ? value.Trim() : value; } }
        public string NewEgg_com { get { return newEgg_comField; } set { newEgg_comField = !string.IsNullOrEmpty(value) ? value.Trim() : value; } }
        public string Sears { get { return searsField; } set { searsField = !string.IsNullOrEmpty(value) ? value.Trim() : value; } }
        public string Suffix { get { return suffixField; } set { suffixField = !string.IsNullOrEmpty(value) ? value.Trim() : value; } }
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
        public string ParentSku { get { return parentSkuField; } set { parentSkuField = !string.IsNullOrEmpty(value) ? value.Trim() : value; } }
        public string Name { get { return nameField; } set { nameField = !string.IsNullOrEmpty(value) ? value.Trim() : value; } }
        public Nullable<byte> Type { get; set; }
        public Nullable<int> Condition { get; set; }
        public Nullable<int> Category { get; set; }
        public Nullable<int> Brand { get; set; }
        public string UPC { get { return upcField; } set { upcField = !string.IsNullOrEmpty(value) ? value.Trim() : value; } }
        public string EAN { get { return eanField; } set { eanField = !string.IsNullOrEmpty(value) ? value.Trim() : value; } }
        public Nullable<bool> Replenishable { get; set; }
        public Nullable<byte> Status { get; set; }
    }
}