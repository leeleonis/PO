using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.ModelBinding;

namespace inventorySKU.Models
{
    public class CustomModelMetadataProvider : DataAnnotationsModelMetadataProvider
    {
        protected override ModelMetadata CreateMetadata(
          IEnumerable<Attribute> attributes,
          Type containerType,
          Func<object> modelAccessor,
          Type modelType,
          string propertyName)
        {
            var modelMetadata = base.CreateMetadata(attributes, containerType, modelAccessor, modelType, propertyName);
            attributes.OfType<MetadataAttribute>().ToList().ForEach(x => x.Process(modelMetadata));
            return modelMetadata;
        }
    }
    [AttributeUsage(AttributeTargets.Property)]
    public abstract class MetadataAttribute : Attribute
    {
        public abstract void Process(ModelMetadata modelMetaData);
    }
    public class DataGridAttribute : MetadataAttribute
    {
        public bool Show { get; set; }
        public int width { get; set; }
        public string align { get; set; }
        public bool sortable { get; set; }
        public string type { get; set; }
        public bool frozen { get; set; }
        public Dictionary<string, object> DataGrid { get; set; }
        public DataGridAttribute()
        {
            DataGrid = new Dictionary<string, object>();
            DataGrid.Add("Show", Show);
            DataGrid.Add("width", width);
            DataGrid.Add("align", align);
            DataGrid.Add("sortable", sortable);
            DataGrid.Add("type", type);
        }

        //public DataGridAttribute()
        //{
        //    DataGrid.Add("Show", false);
        //    DataGrid.Add("width", 0);
        //    DataGrid.Add("align", "");
        //    DataGrid.Add("sortable", false);
        //    DataGrid.Add("type", "");
        //}

        public override void Process(ModelMetadata modelMetaData)
        {
            modelMetaData.AdditionalValues.Add("DataGrid", DataGrid);
        }
    }
    //public class DataGridAttribute : Attribute
    //{
    //    public bool Show { get; set; }
    //    public int width { get; set; }
    //    public string align { get; set; }
    //    public bool sortable { get; set; }
    //    public string type { get; set; }
    //    public bool frozen { get; set; }
    //    public DataGridAttribute(bool Show = false, int width = 0, string align = null, bool sortable = false, string type = null)
    //    {
    //        metaData.AdditionalValues.Add();



    //      this.Show = Show;
    //        this.width = width;
    //        this.align = align;
    //        this.sortable = sortable;
    //        this.type = type;
    //    }
    //}
}