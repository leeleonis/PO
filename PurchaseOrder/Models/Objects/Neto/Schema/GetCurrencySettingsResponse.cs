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
    public partial class GetCurrencySettingsResponse
    {

        private GetCurrencySettingsResponseCurrencySettings[] currencySettingsField;

        private string currentTimeField;

        private GetCurrencySettingsResponseAck ackField;

        private GetCurrencySettingsResponseMessages messagesField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("CurrencySettings", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public GetCurrencySettingsResponseCurrencySettings[] CurrencySettings
        {
            get
            {
                return this.currencySettingsField;
            }
            set
            {
                this.currencySettingsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
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
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public GetCurrencySettingsResponseAck Ack
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
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public GetCurrencySettingsResponseMessages Messages
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
    public partial class GetCurrencySettingsResponseCurrencySettings
    {

        private string dEFAULTCOUNTRYField;

        private string dEFAULTCURRENCYField;

        private string gST_AMTField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string DEFAULTCOUNTRY
        {
            get
            {
                return this.dEFAULTCOUNTRYField;
            }
            set
            {
                this.dEFAULTCOUNTRYField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string DEFAULTCURRENCY
        {
            get
            {
                return this.dEFAULTCURRENCYField;
            }
            set
            {
                this.dEFAULTCURRENCYField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string GST_AMT
        {
            get
            {
                return this.gST_AMTField;
            }
            set
            {
                this.gST_AMTField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "NetoAPI")]
    public enum GetCurrencySettingsResponseAck
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
    public partial class GetCurrencySettingsResponseMessages
    {

        private GetCurrencySettingsResponseMessagesError[] errorField;

        private GetCurrencySettingsResponseMessagesWarning[] warningField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Error", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public GetCurrencySettingsResponseMessagesError[] Error
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
        [System.Xml.Serialization.XmlElementAttribute("Warning", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public GetCurrencySettingsResponseMessagesWarning[] Warning
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
    public partial class GetCurrencySettingsResponseMessagesError
    {

        private string messageField;

        private string severityCodeField;

        private string descriptionField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
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
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
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
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
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
    public partial class GetCurrencySettingsResponseMessagesWarning
    {

        private string messageField;

        private string severityCodeField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
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
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
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
