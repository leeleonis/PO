
using PurchaseOrderSys.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Resources;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

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
        public static MvcHtmlString DataGridFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, string DataGridName, bool checkbox, string title, string idField, bool additem, bool edititem, bool delitem, bool saveitem, bool showchilds)
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
            DataGridModels.saveitem = saveitem;
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
                        nlist.Add(new DataGridItemsModels { title = itemtitle, align = DataGridVal.Align, field = item.Name, formatter = DataGridVal.Formatter, frozen = DataGridVal.Frozen, sortable = DataGridVal.Sortable ? "true" : "false", width = DataGridVal.Widths, columnsType = DataGridVal.ColumnsType });
                    }
                }
            }
            return nlist;
        }

        public static MvcHtmlString BooleanSwitch(this HtmlHelper helper, string name, bool isChecked, string id = null, bool disabled = false)
        {
            TagBuilder divHtml = new TagBuilder("div");
            divHtml.AddCssClass("onoffswitch");

            TagBuilder inputHtml = new TagBuilder("input");
            inputHtml.AddCssClass("onoffswitch-checkbox");
            inputHtml.Attributes.Add("type", "checkbox");
            inputHtml.Attributes.Add("id", id ?? name);
            inputHtml.Attributes.Add("name", name);
            inputHtml.Attributes.Add("value", "true");
            if (disabled)
            {
                inputHtml.Attributes.Add("disabled", "true");
            }
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

        public static MvcHtmlString SetExtraLink(this HtmlHelper helper, string type = null, object data = null)
        {
            TagBuilder aHtml = new TagBuilder("a");
            aHtml.Attributes.Add("target", "_blank");
            aHtml.InnerHtml = data.ToString();

            switch (type)
            {
                case "PO Page":
                    aHtml.Attributes.Add("href", "http://internal.qd.com.tw:8080/PurchaseOrderPO/EditItem?ID=" + data.ToString());
                    break;
                case "CM Page":
                    aHtml.Attributes.Add("href", "http://internal.qd.com.tw:8080/PurchaseOrderCM/EditItem?ID=" + data.ToString());
                    break;
                case "Order Page":
                    aHtml.Attributes.Add("href", "https://dm.cwa.sellercloud.com/Orders/Orders_details.aspx?id=" + data.ToString());
                    break;
                case "RMAIn Page":
                    aHtml.Attributes.Add("href", "http://internal.qd.com.tw:8080/RMA/Edit/" + data.ToString());
                    break;
                case "SKU Page":
                    aHtml.Attributes.Add("href", "http://internal.qd.com.tw:8080/Sku/Edit/" + data.ToString());
                    break;
                case "TransferIn":
                case "TransferOut":
                    aHtml.Attributes.Add("href", "http://internal.qd.com.tw:8080/Transfer/Edit/" + data.ToString());
                    break;
                default:
                    aHtml.Attributes.Add("href", "javascript:;");
                    break;
            }

            return MvcHtmlString.Create(aHtml.ToString());
        }

        public static MvcHtmlString BCheckBox(this HtmlHelper htmlHelper, string name, object htmlAttributes)
        {
            return BCheckBox(htmlHelper, name, false, htmlAttributes);
        }
        public static MvcHtmlString BCheckBox(this HtmlHelper htmlHelper, string name, bool isChecked, object htmlAttributes)
        {
            string DisplayName = htmlHelper.DisplayName(name).ToHtmlString().Trim();
            string checkBoxWithHidden = htmlHelper.CheckBox(name, isChecked, htmlAttributes).ToHtmlString().Trim();
            string pureCheckBox = checkBoxWithHidden.Substring(0, checkBoxWithHidden.IndexOf("<input", 1));
            TagBuilder labelHtml = new TagBuilder("label");
            labelHtml.AddCssClass("checkbox-inline");
            TagBuilder spanHtml = new TagBuilder("span");
            spanHtml.InnerHtml = DisplayName;
            labelHtml.InnerHtml = pureCheckBox + spanHtml;
            return MvcHtmlString.Create(labelHtml.ToString());
        }

        /// <summary>
        /// 完整的寄信功能
        /// </summary>
        /// <param name="MailFrom">寄信人E-mail Address</param>
        /// <param name="MailTos">收信人E-mail Address</param>
        /// <param name="Ccs">副本E-mail Address</param>
        /// <param name="MailSub">主旨</param>
        /// <param name="MailBody">信件內容</param>
        /// <param name="isBodyHtml">是否採用HTML格式</param>
        /// <param name="filePaths">附檔在WebServer檔案總管路徑</param>
        /// <param name="filePaths2">附檔組</param>
        /// <param name="deleteFileAttachment">是否刪除在WebServer上的附件</param>
        /// <returns>是否成功</returns>
        public static bool Mail_Send(string MailFrom, string[] MailTos, string[] Ccs, string MailSub, string MailBody, bool isBodyHtml, string[] filePaths, List<Tuple<Stream, string>> filePaths2, bool deleteFileAttachment)
        {
            string smtpServer = "smtp.sendgrid.net";
            int smtpPort = 587;
            string mailAccount = "azure_d6e7b8a9c9606ce581770e3a9db11a27@azure.com";
            string mailPwd = "6S2OHhiXob9Q";
            //string mailAccount = "nexeve";
            //string mailPwd = "96yVfFZ9a81GqnDYTUebjncIsZe5Rvhnbv1j";

            try
            {
                //防呆
                if (string.IsNullOrEmpty(MailFrom))
                {//※有些公司的Mail Server會規定寄信人的Domain Name要是該Mail Server的Domain Name
                    MailFrom = "dispatch-qd@hotmail.com";
                }

                //建立MailMessage物件
                MailMessage mms = new MailMessage();
                //指定一位寄信人MailAddress
                mms.From = new MailAddress(MailFrom);
                //信件主旨
                mms.Subject = MailSub;
                //信件內容
                mms.Body = MailBody;
                //信件內容 是否採用Html格式
                mms.IsBodyHtml = isBodyHtml;

                if (MailTos != null)//防呆
                {
                    for (int i = 0; i < MailTos.Length; i++)
                    {
                        //加入信件的收信人(們)address
                        if (!string.IsNullOrEmpty(MailTos[i].Trim()))
                        {
                            mms.To.Add(new MailAddress(MailTos[i].Trim()));
                        }
                    }
                }//End if (MailTos !=null)//防呆

                if (Ccs != null) //防呆
                {
                    for (int i = 0; i < Ccs.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(Ccs[i].Trim()))
                        {
                            //加入信件的副本(們)address
                            mms.CC.Add(new MailAddress(Ccs[i].Trim()));
                        }
                    }
                }//End if (Ccs!=null) //防呆

                if (filePaths != null)//防呆
                {//有夾帶檔案
                    for (int i = 0; i < filePaths.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(filePaths[i].Trim()))
                        {
                            Attachment file = new Attachment(filePaths[i].Trim());
                            //加入信件的夾帶檔案
                            mms.Attachments.Add(file);
                        }
                    }
                }//End if (filePaths!=null)//防呆

                if (filePaths2 != null)
                {
                    foreach (var item in filePaths2)
                    {
                        Attachment file = new Attachment(item.Item1, item.Item2);
                        //加入信件的夾帶檔案
                        mms.Attachments.Add(file);
                    }
                }


                using (SmtpClient client = new SmtpClient(smtpServer, smtpPort))//或公司、客戶的smtp_server
                {
                    if (!string.IsNullOrEmpty(mailAccount) && !string.IsNullOrEmpty(mailPwd))//.config有帳密的話
                    {
                        client.Credentials = new NetworkCredential(mailAccount, mailPwd);//寄信帳密
                    }
                    client.Send(mms);//寄出一封信
                }//end using 

                //釋放每個附件，才不會Lock住
                if (mms.Attachments != null && mms.Attachments.Count > 0)
                {
                    for (int i = 0; i < mms.Attachments.Count; i++)
                    {
                        mms.Attachments[i].Dispose();
                        //mms.Attachments[i] = null;
                    }
                }

                //是否要刪除附檔
                if (deleteFileAttachment && filePaths != null && filePaths.Length > 0)
                {

                    foreach (string filePath in filePaths)
                    {
                        File.Delete(filePath.Trim());
                    }

                }

                return true;//成功
            }
            catch (Exception ex)
            {
                Log("SendMail", null, ex.InnerException != null && !string.IsNullOrEmpty(ex.InnerException.Message) ? ex.InnerException.Message : ex.Message);
                return false;//寄失敗
            }
        }
        public static void Log(string tableName, object targetID, string actionName, HttpSessionStateBase session = null)
        {
            //lock (ActionLog)
            //{
            //    ActionLog.Create(new ActionLog()
            //    {
            //        AdminID = (int)get_session("AdminID", session, -1),
            //        AdminName = (string)get_session("AdminName", session, ""),
            //        TableName = !string.IsNullOrEmpty(tableName) ? tableName : "",
            //        TargetID = targetID != null ? targetID.ToString() : "",
            //        ActionName = !string.IsNullOrEmpty(actionName) ? actionName : "",
            //        CreateDate = DateTime.UtcNow
            //    });

            //    ActionLog.SaveChanges();
            //}
        }
        /// <summary>
        /// 下拉選單改成Label唯讀顯示
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="name"></param>
        /// <param name="selectList"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static MvcHtmlString DropDownListLabel(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> selectList, object htmlAttributes)
        {
            var attrs = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            var readonlyVal = attrs.Where(x => x.Key == "readonly").FirstOrDefault().Value;
            if (readonlyVal.ToString() != "readonly" && readonlyVal.ToString() != "true")
            {
                attrs.Add("readonly", "readonly");
            }
            var Value = htmlHelper.Value(name).ToHtmlString();
            var Text = selectList.Where(x => x.Value == Value).FirstOrDefault()?.Text;
            var labelHtml = htmlHelper.Label(name, Text, attrs);
            return MvcHtmlString.Create(labelHtml.ToHtmlString());
        }
        /// <summary>
        /// 取工作日
        /// </summary>
        /// <param name="AddDay">幾天後</param>
        /// <returns></returns>
        public static DateTime WorkingDay(int AddDay, DateTime? dt = null)
        {
            if (!dt.HasValue)
            {
                dt = DateTime.Today.AddDays(AddDay);
            }
            dt = dt.Value.AddDays(AddDay);
            if (dt.Value.DayOfWeek == DayOfWeek.Saturday)
            {
                dt = dt.Value.AddDays(2);
            }
            else if (dt.Value.DayOfWeek == DayOfWeek.Sunday)
            {
                dt = dt.Value.AddDays(1);
            }
            return dt.Value;
        }
    }
}