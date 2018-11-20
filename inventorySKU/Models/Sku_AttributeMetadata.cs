using System; 
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using System; 
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using System; 
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace inventorySKU.Models
{

    /// <summary>
    /// Sku_Attribute class
    /// </summary>
    [MetadataType(typeof(Sku_AttributeMetadata))]
    public partial class Sku_Attribute : IEquatable<Sku_Attribute>
    {

        public bool Equals(Sku_Attribute other)
        {
            if (ReferenceEquals(other, null)) return false;

            if (ReferenceEquals(this, other)) return true;

            return Sku.Equals(other.Sku) && AttrID.Equals(other.AttrID) && LangID.Equals(other.LangID);
        }

        public override int GetHashCode()
        {
            int hashSku = Sku.GetHashCode();
            int hashAttrID = AttrID.GetHashCode();
            int hashLangID = LangID.GetHashCode();

            return hashSku ^ hashAttrID ^ hashLangID;
        }

        /// <summary>
        /// Sku_Attribute Metadata class
        /// </summary>
        public class Sku_AttributeMetadata
        {

            /// <summary>
            /// Sku
            /// </summary>        
            [Display(Name = "Sku_Attribute_Sku", ResourceType = typeof(ViewRes.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ViewRes.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ViewRes.Resource))]
            public string Sku { get; set; }


            /// <summary>
            /// Attr ID
            /// </summary>        
            [Display(Name = "Sku_Attribute_AttrID", ResourceType = typeof(ViewRes.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ViewRes.Resource))]
            public int AttrID { get; set; }


            /// <summary>
            /// Lang ID
            /// </summary>        
            [Display(Name = "Sku_Attribute_LangID", ResourceType = typeof(ViewRes.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ViewRes.Resource))]
            [MaxLength(10, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ViewRes.Resource))]
            public string LangID { get; set; }


            /// <summary>
            /// Value
            /// </summary>        
            [Display(Name = "Sku_Attribute_Value", ResourceType = typeof(ViewRes.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ViewRes.Resource))]
            public string Value { get; set; }


            /// <summary>
            /// Html
            /// </summary>        
            [Display(Name = "Sku_Attribute_Html", ResourceType = typeof(ViewRes.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ViewRes.Resource))]
            public bool Html { get; set; }


            /// <summary>
            /// e Bay
            /// </summary>        
            [Display(Name = "Sku_Attribute_eBay", ResourceType = typeof(ViewRes.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ViewRes.Resource))]
            public bool eBay { get; set; }


            /// <summary>
            /// Create By
            /// </summary>        
            [Display(Name = "Sku_Attribute_CreateBy", ResourceType = typeof(ViewRes.Resource))]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ViewRes.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ViewRes.Resource))]
            public string CreateBy { get; set; }


            /// <summary>
            /// Create At
            /// </summary>        
            [Display(Name = "Sku_Attribute_CreateAt", ResourceType = typeof(ViewRes.Resource))]
            [UIHint("DateTime")]
            [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
            [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ViewRes.Resource))]
            public System.DateTime CreateAt { get; set; }


            /// <summary>
            /// Update By
            /// </summary>        
            [Display(Name = "Sku_Attribute_UpdateBy", ResourceType = typeof(ViewRes.Resource))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ViewRes.Resource))]
            public string UpdateBy { get; set; }


            /// <summary>
            /// Update At
            /// </summary>        
            [Display(Name = "Sku_Attribute_UpdateAt", ResourceType = typeof(ViewRes.Resource))]
            [UIHint("DateTime")]
            public Nullable<System.DateTime> UpdateAt { get; set; }
        }
    }

}
