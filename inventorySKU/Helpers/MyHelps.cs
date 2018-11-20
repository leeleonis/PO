using inventorySKU.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace inventorySKU.Helpers
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
        public static MvcHtmlString DataGridFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
        {

            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, html.ViewData);
            foreach (var item in metadata.Properties)
            {
                var attr = html.ViewData.ModelMetadata.AdditionalValues.SingleOrDefault(x => x.Key == "DataGrid").Value;
            }
            //string lookup = metadata.PropertyName;

            //IEnumerable<lookup> list = Get(lookup);

            var info = expression.GetType();
            var tclassAttribute = (DataGridAttribute)Attribute.GetCustomAttribute(info, typeof(DataGridAttribute));
            foreach (var method in info.GetProperties())
            {
                var methodinfo = Type.GetType(method.Name);
                var classAttribute = (DataGridAttribute)Attribute.GetCustomAttribute(methodinfo, typeof(DataGridAttribute));
            }
            return MvcHtmlString.Create("");
        }

        public static MvcHtmlString BooleanSwitch(this HtmlHelper helper, string name, bool isChecked, string id = null)
        {
            TagBuilder divHtml = new TagBuilder("div");
            divHtml.AddCssClass("onoffswitch");

            TagBuilder inputHtml = new TagBuilder("input");
            inputHtml.AddCssClass("onoffswitch-checkbox");
            inputHtml.Attributes.Add("type", "checkbox");
            inputHtml.Attributes.Add("id", id ?? name);
            inputHtml.Attributes.Add("name", name);
            inputHtml.Attributes.Add("value", "true");
            if (isChecked) inputHtml.Attributes.Add("checked", "checked");

            TagBuilder labelHtml = new TagBuilder("label");
            labelHtml.AddCssClass("onoffswitch-label");
            labelHtml.Attributes.Add("for", id ?? name);
            labelHtml.InnerHtml = "<span class=\"onoffswitch-inner\" data-swchon-text=\"Yes\" data-swchoff-text=\"No\"></span><span class=\"onoffswitch-switch\"></span>";

            TagBuilder hiddenHtml = new TagBuilder("input");
            hiddenHtml.Attributes.Add("type", "hidden");
            hiddenHtml.Attributes.Add("name", name);
            hiddenHtml.Attributes.Add("value", "false");

            divHtml.InnerHtml = inputHtml.ToString() + labelHtml.ToString() + hiddenHtml.ToString();

            return MvcHtmlString.Create(divHtml.ToString());
        }
    }
}