﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.3053
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by xsd, Version=2.0.50727.3038.
// 
namespace DaxnetBlog.Common
{
    using System.Xml;
    using System.Xml.Serialization;


    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class rss
    {

        private RssChannel channelField;

        private System.Xml.XmlElement[] anyField;

        private decimal versionField;

        private System.Xml.XmlAttribute[] anyAttrField;

        public rss()
        {
            this.versionField = ((decimal)(2.0m));
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public RssChannel channel
        {
            get
            {
                return this.channelField;
            }
            set
            {
                this.channelField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAnyElementAttribute()]
        public System.Xml.XmlElement[] Any
        {
            get
            {
                return this.anyField;
            }
            set
            {
                this.anyField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal version
        {
            get
            {
                return this.versionField;
            }
            set
            {
                this.versionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAnyAttributeAttribute()]
        public System.Xml.XmlAttribute[] AnyAttr
        {
            get
            {
                return this.anyAttrField;
            }
            set
            {
                this.anyAttrField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class RssChannel
    {

        private object[] itemsField;

        private ItemsChoiceType[] itemsElementNameField;

        private RssItem[] itemField;

        private System.Xml.XmlElement[] anyField;

        private System.Xml.XmlAttribute[] anyAttrField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAnyElementAttribute(Order = 0)]
        [System.Xml.Serialization.XmlElementAttribute("category", typeof(Category), Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 0)]
        [System.Xml.Serialization.XmlElementAttribute("cloud", typeof(Cloud), Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 0)]
        [System.Xml.Serialization.XmlElementAttribute("copyright", typeof(string), Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 0)]
        [System.Xml.Serialization.XmlElementAttribute("description", typeof(string), Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 0)]
        [System.Xml.Serialization.XmlElementAttribute("docs", typeof(string), Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "anyURI", Order = 0)]
        [System.Xml.Serialization.XmlElementAttribute("generator", typeof(string), Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 0)]
        [System.Xml.Serialization.XmlElementAttribute("image", typeof(Image), Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 0)]
        [System.Xml.Serialization.XmlElementAttribute("language", typeof(string), Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "language", Order = 0)]
        [System.Xml.Serialization.XmlElementAttribute("lastBuildDate", typeof(string), Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 0)]
        [System.Xml.Serialization.XmlElementAttribute("link", typeof(string), Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "anyURI", Order = 0)]
        [System.Xml.Serialization.XmlElementAttribute("managingEditor", typeof(string), Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 0)]
        [System.Xml.Serialization.XmlElementAttribute("pubDate", typeof(string), Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 0)]
        [System.Xml.Serialization.XmlElementAttribute("rating", typeof(string), Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 0)]
        [System.Xml.Serialization.XmlElementAttribute("skipDays", typeof(SkipDaysList), Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 0)]
        [System.Xml.Serialization.XmlElementAttribute("skipHours", typeof(SkipHoursList), Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 0)]
        [System.Xml.Serialization.XmlElementAttribute("textInput", typeof(TextInput), Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 0)]
        [System.Xml.Serialization.XmlElementAttribute("title", typeof(string), Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 0)]
        [System.Xml.Serialization.XmlElementAttribute("ttl", typeof(string), Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "nonNegativeInteger", Order = 0)]
        [System.Xml.Serialization.XmlElementAttribute("webMaster", typeof(string), Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 0)]
        [System.Xml.Serialization.XmlChoiceIdentifierAttribute("ItemsElementName")]
        public object[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ItemsElementName", Order = 1)]
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public ItemsChoiceType[] ItemsElementName
        {
            get
            {
                return this.itemsElementNameField;
            }
            set
            {
                this.itemsElementNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("item", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 2)]
        public RssItem[] item
        {
            get
            {
                return this.itemField;
            }
            set
            {
                this.itemField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAnyElementAttribute(Order = 3)]
        public System.Xml.XmlElement[] Any
        {
            get
            {
                return this.anyField;
            }
            set
            {
                this.anyField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAnyAttributeAttribute()]
        public System.Xml.XmlAttribute[] AnyAttr
        {
            get
            {
                return this.anyAttrField;
            }
            set
            {
                this.anyAttrField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class Category
    {

        private string domainField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string domain
        {
            get
            {
                return this.domainField;
            }
            set
            {
                this.domainField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class Source
    {

        private string urlField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "anyURI")]
        public string url
        {
            get
            {
                return this.urlField;
            }
            set
            {
                this.urlField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class RssItemGuid
    {

        private bool isPermaLinkField;

        private string valueField;

        public RssItemGuid()
        {
            this.isPermaLinkField = true;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool isPermaLink
        {
            get
            {
                return this.isPermaLinkField;
            }
            set
            {
                this.isPermaLinkField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class Enclosure
    {

        private string urlField;

        private string lengthField;

        private string typeField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "anyURI")]
        public string url
        {
            get
            {
                return this.urlField;
            }
            set
            {
                this.urlField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "nonNegativeInteger")]
        public string length
        {
            get
            {
                return this.lengthField;
            }
            set
            {
                this.lengthField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class RssItem
    {

        private object[] itemsField;

        private ItemsChoiceType1[] itemsElementNameField;

        private System.Xml.XmlAttribute[] anyAttrField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAnyElementAttribute()]
        [System.Xml.Serialization.XmlElementAttribute("author", typeof(string), Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlElementAttribute("category", typeof(Category), Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlElementAttribute("comments", typeof(string), Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "anyURI")]
        [System.Xml.Serialization.XmlElementAttribute("description", typeof(string), Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlElementAttribute("enclosure", typeof(Enclosure), Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlElementAttribute("guid", typeof(RssItemGuid), Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlElementAttribute("link", typeof(string), Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "anyURI")]
        [System.Xml.Serialization.XmlElementAttribute("pubDate", typeof(string), Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlElementAttribute("source", typeof(Source), Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlElementAttribute("title", typeof(string), Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlChoiceIdentifierAttribute("ItemsElementName")]
        public object[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ItemsElementName")]
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public ItemsChoiceType1[] ItemsElementName
        {
            get
            {
                return this.itemsElementNameField;
            }
            set
            {
                this.itemsElementNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAnyAttributeAttribute()]
        public System.Xml.XmlAttribute[] AnyAttr
        {
            get
            {
                return this.anyAttrField;
            }
            set
            {
                this.anyAttrField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    
    [System.Xml.Serialization.XmlTypeAttribute(IncludeInSchema = false)]
    public enum ItemsChoiceType1
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("##any:")]
        Item,

        /// <remarks/>
        author,

        /// <remarks/>
        category,

        /// <remarks/>
        comments,

        /// <remarks/>
        description,

        /// <remarks/>
        enclosure,

        /// <remarks/>
        guid,

        /// <remarks/>
        link,

        /// <remarks/>
        pubDate,

        /// <remarks/>
        source,

        /// <remarks/>
        title,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class SkipDaysList
    {

        private SkipDay[] dayField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("day", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public SkipDay[] day
        {
            get
            {
                return this.dayField;
            }
            set
            {
                this.dayField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    
    public enum SkipDay
    {

        /// <remarks/>
        Monday,

        /// <remarks/>
        Tuesday,

        /// <remarks/>
        Wednesday,

        /// <remarks/>
        Thursday,

        /// <remarks/>
        Friday,

        /// <remarks/>
        Saturday,

        /// <remarks/>
        Sunday,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class SkipHoursList
    {

        private string[] hourField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("hour", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "nonNegativeInteger")]
        public string[] hour
        {
            get
            {
                return this.hourField;
            }
            set
            {
                this.hourField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class TextInput
    {

        private string titleField;

        private string descriptionField;

        private string nameField;

        private string linkField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string title
        {
            get
            {
                return this.titleField;
            }
            set
            {
                this.titleField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string description
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

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "anyURI")]
        public string link
        {
            get
            {
                return this.linkField;
            }
            set
            {
                this.linkField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class Image
    {

        private string urlField;

        private string titleField;

        private string linkField;

        private string widthField;

        private string heightField;

        private string descriptionField;

        public Image()
        {
            this.widthField = "88";
            this.heightField = "31";
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "anyURI")]
        public string url
        {
            get
            {
                return this.urlField;
            }
            set
            {
                this.urlField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string title
        {
            get
            {
                return this.titleField;
            }
            set
            {
                this.titleField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "anyURI")]
        public string link
        {
            get
            {
                return this.linkField;
            }
            set
            {
                this.linkField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "positiveInteger")]
        [System.ComponentModel.DefaultValueAttribute("88")]
        public string width
        {
            get
            {
                return this.widthField;
            }
            set
            {
                this.widthField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "positiveInteger")]
        [System.ComponentModel.DefaultValueAttribute("31")]
        public string height
        {
            get
            {
                return this.heightField;
            }
            set
            {
                this.heightField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string description
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
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class Cloud
    {

        private string domainField;

        private string portField;

        private string pathField;

        private string registerProcedureField;

        private CloudProtocol protocolField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string domain
        {
            get
            {
                return this.domainField;
            }
            set
            {
                this.domainField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "positiveInteger")]
        public string port
        {
            get
            {
                return this.portField;
            }
            set
            {
                this.portField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string path
        {
            get
            {
                return this.pathField;
            }
            set
            {
                this.pathField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string registerProcedure
        {
            get
            {
                return this.registerProcedureField;
            }
            set
            {
                this.registerProcedureField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public CloudProtocol protocol
        {
            get
            {
                return this.protocolField;
            }
            set
            {
                this.protocolField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    
    public enum CloudProtocol
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("xml-rpc")]
        xmlrpc,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("http-post")]
        httppost,

        /// <remarks/>
        soap,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    
    [System.Xml.Serialization.XmlTypeAttribute(IncludeInSchema = false)]
    public enum ItemsChoiceType
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("##any:")]
        Item,

        /// <remarks/>
        category,

        /// <remarks/>
        cloud,

        /// <remarks/>
        copyright,

        /// <remarks/>
        description,

        /// <remarks/>
        docs,

        /// <remarks/>
        generator,

        /// <remarks/>
        image,

        /// <remarks/>
        language,

        /// <remarks/>
        lastBuildDate,

        /// <remarks/>
        link,

        /// <remarks/>
        managingEditor,

        /// <remarks/>
        pubDate,

        /// <remarks/>
        rating,

        /// <remarks/>
        skipDays,

        /// <remarks/>
        skipHours,

        /// <remarks/>
        textInput,

        /// <remarks/>
        title,

        /// <remarks/>
        ttl,

        /// <remarks/>
        webMaster,
    }
}