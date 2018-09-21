
using PurchaseOrderSys.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Resources;

namespace PurchaseOrderSys.Helpers
{
    public static class MyHelps
    {
        /// <summary>
        /// 依語系顯示資料內容
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="LangList">語系資料</param>
        /// <param name="columnName">欄位名稱</param>
        /// <returns></returns>
        public static MvcHtmlString TableLangName(this HtmlHelper htmlHelper, dynamic LangList, string columnName = "Name")
        {
            var LangVal = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            var Name = "";

            if (LangList != null)
            {
                foreach (dynamic item in LangList)
                {
                    var LangID = item.GetType().GetProperty("LangID").GetValue(item);
                    if (LangID == LangVal)
                    {
                        // 判斷是否有此欄位
                        var itemColumn = item.GetType().GetProperty(columnName);
                        if (itemColumn != null)
                        {
                            Name = itemColumn.GetValue(item);
                        }
                    }
                }
            }

            return MvcHtmlString.Create(Name);
        }
        /// <summary>
        /// 依權限自動產生 編輯和刪除按鈕
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="controllerName"></param>
        /// <param name="routeValues"></param>
        /// <returns></returns>
        public static MvcHtmlString TableRowCtrl(this HtmlHelper helper, string controllerName, object routeValues)
        {
            var shtml = "";
            shtml += TableEdit(helper, "Edit", controllerName, routeValues) + " ";
            shtml += TableDelete(helper, "Delete", controllerName, routeValues);
            return MvcHtmlString.Create(shtml);
        }
        /// <summary>
        /// 編輯按鈕
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="actionName"></param>
        /// <param name="controllerName"></param>
        /// <param name="routeValues"></param>
        /// <returns></returns>
        public static MvcHtmlString TableEdit(this HtmlHelper helper, string actionName, string controllerName, object routeValues)
        {
            if (helper.ViewBag.Edit != null && helper.ViewBag.Edit)
            {
                var urlHelper = new UrlHelper(helper.ViewContext.RequestContext);
                var url = urlHelper.Action(actionName, controllerName, routeValues);
                TagBuilder aHtml = new TagBuilder("a");
                aHtml.Attributes.Add(new KeyValuePair<string, string>("href", url));
                aHtml.AddCssClass("btn btn-primary");
                TagBuilder aSpan = new TagBuilder("span");
                aSpan.AddCssClass("glyphicon glyphicon-pencil");
                aSpan.InnerHtml = "Edit";
                aHtml.InnerHtml = aSpan.ToString();
                return MvcHtmlString.Create(aHtml.ToString());
            }
            else
            {
                return MvcHtmlString.Create("");
            }
        }
        /// <summary>
        /// 刪除按鈕
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="actionName"></param>
        /// <param name="controllerName"></param>
        /// <param name="routeValues"></param>
        /// <returns></returns>
        public static MvcHtmlString TableDelete(this HtmlHelper helper, string actionName, string controllerName, object routeValues)
        {
            if (helper.ViewBag.Delete != null && helper.ViewBag.Delete)
            {
                var urlHelper = new UrlHelper(helper.ViewContext.RequestContext);
                var url = urlHelper.Action(actionName, controllerName, routeValues);
                TagBuilder aHtml = new TagBuilder("a");
                aHtml.Attributes.Add(new KeyValuePair<string, string>("href", url));
                aHtml.AddCssClass("btn btn-danger");
                TagBuilder aSpan = new TagBuilder("span");
                aSpan.AddCssClass("glyphicon glyphicon-trash");
                aSpan.InnerHtml = "Delete";
                aHtml.InnerHtml = aSpan.ToString();
                return MvcHtmlString.Create(aHtml.ToString());
            }
            else
            {
                return MvcHtmlString.Create("");
            }
        }
        /// <summary>
        /// 依MODEL 產生 DataGrid
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="html"></param>
        /// <param name="expression"></param>
        /// <param name="DataGridName"></param>
        /// <param name="checkbox">是否顯示checkbox</param>
        /// <param name="title"></param>
        /// <param name="idField"></param>
        /// <param name="additem">是否顯示新增子項目</param>
        /// <param name="edititem">是否顯修改按鈕</param>
        /// <param name="delitem">是否顯示刪除按鈕</param>
        /// <returns></returns>
        public static MvcHtmlString DataGridFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression,string DataGridName,  bool checkbox, string title, string idField ,bool additem, bool edititem, bool delitem, bool showchilds)
        {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, html.ViewData);
            var DataGridModels = new DataGridModels();
            DataGridModels.DataGridName = DataGridName;
            DataGridModels.checkbox = checkbox;
            DataGridModels.title = title;
            DataGridModels.idField = idField;
            DataGridModels.additem = additem;
            DataGridModels.edititem = edititem;
            DataGridModels.delitem = delitem;
            DataGridModels.showchilds = showchilds;
            DataGridModels.DataGridItems = new List<DataGridItemsModels>();
            try
            {
                if (metadata.ModelType.GenericTypeArguments.Any())
                {
                    foreach (var Typeitem in metadata.ModelType.GenericTypeArguments)
                    {
                        DataGridModels.DataGridItems.AddRange(DataGridItemslist(Typeitem.FullName));
                    }
                }
                else
                {
                    DataGridModels.DataGridItems.AddRange(DataGridItemslist(metadata.ModelType.FullName));
                }
            }
            catch
            {

            }

            return html.DisplayFor(x => DataGridModels, "DataGrid");
        }
        /// <summary>
        /// Model Class 的名稱
        /// </summary>
        /// <param name="ClassName"></param>
        /// <returns></returns>
        private static IEnumerable<DataGridItemsModels> DataGridItemslist(string ClassName)
        {
            var nlist = new List<DataGridItemsModels>();
            var type = Type.GetType(ClassName);
            var metadataType = type.GetCustomAttributes(typeof(MetadataTypeAttribute), true).OfType<MetadataTypeAttribute>().FirstOrDefault();
            var metaData = (metadataType != null) ? ModelMetadataProviders.Current.GetMetadataForType(null, metadataType.MetadataClassType) : ModelMetadataProviders.Current.GetMetadataForType(null, type);
            foreach (var item in Type.GetType(metaData.ModelType.FullName).GetProperties())//model
            {
                var Attributeslist = item.GetCustomAttributes();
                var DisplayAttributelist = Attributeslist.Where(a => a.GetType() == typeof(DisplayAttribute));
                var DataGridAttributelist = Attributeslist.Where(a => a.GetType() == typeof(DataGridAttribute));
                if (DisplayAttributelist.Any() && DataGridAttributelist.Any())
                {

                    var DisplayAttributeVal = (DisplayAttribute)DisplayAttributelist.FirstOrDefault();
                    var resourceManager = new ResourceManager(DisplayAttributeVal.ResourceType.FullName, Assembly.GetExecutingAssembly());
                    var itemtitle = resourceManager.GetString(DisplayAttributeVal.Name);
                    if (DataGridAttributelist.Any(a => ((DataGridAttribute)a) != null))
                    {
                        var DataGridVal = (DataGridAttribute)DataGridAttributelist.FirstOrDefault();
                        nlist.Add(new DataGridItemsModels { title = itemtitle, align = DataGridVal.Align, field = item.Name, formatter = DataGridVal.Formatter, frozen = DataGridVal.Frozen, sortable = DataGridVal.Sortable ? "true" : "false", width = DataGridVal.Widths,columnsType= DataGridVal.ColumnsType });
                    }
                }
            }
            return nlist;
        }
    }
}