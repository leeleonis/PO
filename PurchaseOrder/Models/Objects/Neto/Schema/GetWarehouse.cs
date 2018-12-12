﻿//------------------------------------------------------------------------------
// <auto-generated>
//     這段程式碼是由工具產生的。
//     執行階段版本:4.0.30319.42000
//
//     對這個檔案所做的變更可能會造成錯誤的行為，而且如果重新產生程式碼，
//     變更將會遺失。
// </auto-generated>
//------------------------------------------------------------------------------

// 
// 此原始程式碼由 xsd 版本=4.6.1055.0 自動產生。
// 
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NetoDeveloper
{

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "NetoAPI")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "NetoAPI", IsNullable = false)]
    public partial class GetWarehouse
    {

        private GetWarehouseFilter filterField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public GetWarehouseFilter Filter
        {
            get
            {
                return this.filterField;
            }
            set
            {
                this.filterField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "NetoAPI")]
    public partial class GetWarehouseFilter
    {

        private string[] warehouseIDField;

        private string[] warehouseReferenceField;

        private string[] warehouseNameField;

        private string pageField;

        private string limitField;

        private GetWarehouseFilterOutputSelector[] outputSelectorField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("WarehouseID", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "integer")]
        public string[] WarehouseID
        {
            get
            {
                return this.warehouseIDField;
            }
            set
            {
                this.warehouseIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("WarehouseReference", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string[] WarehouseReference
        {
            get
            {
                return this.warehouseReferenceField;
            }
            set
            {
                this.warehouseReferenceField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("WarehouseName", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string[] WarehouseName
        {
            get
            {
                return this.warehouseNameField;
            }
            set
            {
                this.warehouseNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "integer")]
        public string Page
        {
            get
            {
                return this.pageField;
            }
            set
            {
                this.pageField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "integer")]
        public string Limit
        {
            get
            {
                return this.limitField;
            }
            set
            {
                this.limitField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("OutputSelector", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public GetWarehouseFilterOutputSelector[] OutputSelector
        {
            get
            {
                return this.outputSelectorField;
            }
            set
            {
                this.outputSelectorField = value;
            }
        }
    }

    /// <remarks/>
    [JsonConverter(typeof(StringEnumConverter))]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "NetoAPI")]
    public enum GetWarehouseFilterOutputSelector
    {

        /// <remarks/>
        ID,

        /// <remarks/>
        WarehouseID,

        /// <remarks/>
        IsPrimary,

        /// <remarks/>
        IsActive,

        /// <remarks/>
        ShowQuantity,

        /// <remarks/>
        WarehouseReference,

        /// <remarks/>
        WarehouseName,

        /// <remarks/>
        WarehouseStreet1,

        /// <remarks/>
        WarehouseStreet2,

        /// <remarks/>
        WarehouseCity,

        /// <remarks/>
        WarehouseState,

        /// <remarks/>
        WarehousePostcode,

        /// <remarks/>
        WarehouseCountry,

        /// <remarks/>
        WarehouseContact,

        /// <remarks/>
        WarehousePhone,

        /// <remarks/>
        WarehouseNotes,
    }
}