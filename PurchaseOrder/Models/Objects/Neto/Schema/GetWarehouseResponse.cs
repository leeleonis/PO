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
namespace NetoDeveloper
{

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "NetoAPI")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "NetoAPI", IsNullable = false)]
    public partial class GetWarehouseResponse
    {

        private GetWarehouseResponseWarehouse[] warehouseField;

        private string currentTimeField;

        private GetWarehouseResponseAck ackField;

        private GetWarehouseResponseMessages[] messagesField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Warehouse")]
        public GetWarehouseResponseWarehouse[] Warehouse
        {
            get
            {
                return this.warehouseField;
            }
            set
            {
                this.warehouseField = value;
            }
        }

        /// <remarks/>
        public string CurrentTime
        {
            get
            {
                return this.currentTimeField;
            }
            set
            {
                this.currentTimeField = value;
            }
        }

        /// <remarks/>
        public GetWarehouseResponseAck Ack
        {
            get
            {
                return this.ackField;
            }
            set
            {
                this.ackField = value;
            }
        }

        /// <remarks/>
        public GetWarehouseResponseMessages[] Messages
        {
            get
            {
                return this.messagesField;
            }
            set
            {
                this.messagesField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "NetoAPI")]
    public partial class GetWarehouseResponseWarehouse
    {

        private string idField;

        private string warehouseNotesField;

        private string warehousePhoneField;

        private string warehouseContactField;

        private string warehouseCountryField;

        private string warehousePostcodeField;

        private string warehouseStateField;

        private string warehouseCityField;

        private string warehouseStreet2Field;

        private string warehouseStreet1Field;

        private string warehouseNameField;

        private string warehouseReferenceField;

        private string showQuantityField;

        private bool isActiveField;

        private bool isActiveFieldSpecified;

        private bool isPrimaryField;

        private bool isPrimaryFieldSpecified;

        private string warehouseIDField;

        /// <remarks/>
        public string ID
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        public string WarehouseNotes
        {
            get
            {
                return this.warehouseNotesField;
            }
            set
            {
                this.warehouseNotesField = value;
            }
        }

        /// <remarks/>
        public string WarehousePhone
        {
            get
            {
                return this.warehousePhoneField;
            }
            set
            {
                this.warehousePhoneField = value;
            }
        }

        /// <remarks/>
        public string WarehouseContact
        {
            get
            {
                return this.warehouseContactField;
            }
            set
            {
                this.warehouseContactField = value;
            }
        }

        /// <remarks/>
        public string WarehouseCountry
        {
            get
            {
                return this.warehouseCountryField;
            }
            set
            {
                this.warehouseCountryField = value;
            }
        }

        /// <remarks/>
        public string WarehousePostcode
        {
            get
            {
                return this.warehousePostcodeField;
            }
            set
            {
                this.warehousePostcodeField = value;
            }
        }

        /// <remarks/>
        public string WarehouseState
        {
            get
            {
                return this.warehouseStateField;
            }
            set
            {
                this.warehouseStateField = value;
            }
        }

        /// <remarks/>
        public string WarehouseCity
        {
            get
            {
                return this.warehouseCityField;
            }
            set
            {
                this.warehouseCityField = value;
            }
        }

        /// <remarks/>
        public string WarehouseStreet2
        {
            get
            {
                return this.warehouseStreet2Field;
            }
            set
            {
                this.warehouseStreet2Field = value;
            }
        }

        /// <remarks/>
        public string WarehouseStreet1
        {
            get
            {
                return this.warehouseStreet1Field;
            }
            set
            {
                this.warehouseStreet1Field = value;
            }
        }

        /// <remarks/>
        public string WarehouseName
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
        public string WarehouseReference
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
        public string ShowQuantity
        {
            get
            {
                return this.showQuantityField;
            }
            set
            {
                this.showQuantityField = value;
            }
        }

        /// <remarks/>
        public bool IsActive
        {
            get
            {
                return this.isActiveField;
            }
            set
            {
                this.isActiveField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool IsActiveSpecified
        {
            get
            {
                return this.isActiveFieldSpecified;
            }
            set
            {
                this.isActiveFieldSpecified = value;
            }
        }

        /// <remarks/>
        public bool IsPrimary
        {
            get
            {
                return this.isPrimaryField;
            }
            set
            {
                this.isPrimaryField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool IsPrimarySpecified
        {
            get
            {
                return this.isPrimaryFieldSpecified;
            }
            set
            {
                this.isPrimaryFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "integer")]
        public string WarehouseID
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
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "NetoAPI")]
    public enum GetWarehouseResponseAck
    {

        /// <remarks/>
        Error,

        /// <remarks/>
        Warning,

        /// <remarks/>
        Success,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "NetoAPI")]
    public partial class GetWarehouseResponseMessages
    {

        private GetWarehouseResponseMessagesError errorField;

        private GetWarehouseResponseMessagesWarning warningField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Error")]
        public GetWarehouseResponseMessagesError Error
        {
            get
            {
                return this.errorField;
            }
            set
            {
                this.errorField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Warning")]
        public GetWarehouseResponseMessagesWarning Warning
        {
            get
            {
                return this.warningField;
            }
            set
            {
                this.warningField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "NetoAPI")]
    public partial class GetWarehouseResponseMessagesError
    {

        private string messageField;

        private string severityCodeField;

        private string descriptionField;

        /// <remarks/>
        public string Message
        {
            get
            {
                return this.messageField;
            }
            set
            {
                this.messageField = value;
            }
        }

        /// <remarks/>
        public string SeverityCode
        {
            get
            {
                return this.severityCodeField;
            }
            set
            {
                this.severityCodeField = value;
            }
        }

        /// <remarks/>
        public string Description
        {
            get
            {
                return this.descriptionField;
            }
            set
            {
                this.descriptionField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "NetoAPI")]
    public partial class GetWarehouseResponseMessagesWarning
    {

        private string messageField;

        private string severityCodeField;

        /// <remarks/>
        public string Message
        {
            get
            {
                return this.messageField;
            }
            set
            {
                this.messageField = value;
            }
        }

        /// <remarks/>
        public string SeverityCode
        {
            get
            {
                return this.severityCodeField;
            }
            set
            {
                this.severityCodeField = value;
            }
        }
    }
}
