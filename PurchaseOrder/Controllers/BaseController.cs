using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.UI;
using PurchaseOrderSys.Models;

namespace PurchaseOrderSys.Controllers
{
    public class BaseController : Controller
    {
        //protected string ApiUrl = "http://internal.qd.com.tw/";
        protected string ApiUrl = "http://localhost:49920/";
        protected string FileUploads = "~/FileUploads";
        protected string UserBy = "test";
        protected PurchaseOrderEntities db = new PurchaseOrderEntities();
        /// <summary>
        /// 設定要更新的欄位
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="OldData">舊資料</param>
        /// <param name="NewData">新資料</param>
        /// <param name="EditList">要更新欄位</param>
        protected void SetEditDatas<T>(T OldData, T NewData, string[] EditList)
        {
            var OldDatatp = OldData.GetType().GetProperties();
            foreach (var NameItem in EditList)
            {
                OldDatatp.FirstOrDefault(x => x.Name == NameItem).SetValue(OldData, OldDatatp.FirstOrDefault(x => x.Name == NameItem).GetValue(NewData, null));
            }
            OldDatatp.FirstOrDefault(x => x.Name == "UpdateBy").SetValue(OldData, Session["AdminName"]);
            OldDatatp.FirstOrDefault(x => x.Name == "UpdateAt").SetValue(OldData, DateTime.UtcNow);
            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                var s = ex.ToString();

            }

        }
        protected string GetPOTypeName(string POType)
        {
            return EnumData.POTypeDDL().Where(x => x.Key == POType)?.FirstOrDefault().Value;
        }
        protected string GetVendorName(int? VendorID)
        {
            if (VendorID.HasValue)
            {

                return db.VendorLIst.Find(VendorID).Name;
            }
            else
            {
                return "";
            }

        }
        protected string GetPOStatusName(string POStatus)
        {
            return EnumData.POStatusDDL().Where(x => x.Key == POStatus)?.FirstOrDefault().Value;
        }
        protected int RandomVal(int minValue, int maxValue)
        {
            Random crandom = new Random(Guid.NewGuid().GetHashCode());
            return crandom.Next(minValue, maxValue);
        }
        protected string ControlToString(string controlPath, object model)
        {
            //CshtmlView control = new CshtmlView(controlPath);
            RazorView control = new RazorView(this.ControllerContext, controlPath, null, false, null);

            this.ViewData.Model = model;

            HtmlTextWriter writer = new HtmlTextWriter(new System.IO.StringWriter());
            control.Render(new ViewContext(this.ControllerContext, control, this.ViewData, this.TempData, writer), writer);

            string value = ((StringWriter)writer.InnerWriter).ToString();

            return value;
        }
        protected string AuthToString(Dictionary<string, List<string>> auth)
        {
            var authW = new Dictionary<string, List<string>>();
            foreach (var authitem in auth)
            {
                if (authitem.Value.Where(x => x != "false").Any())
                {
                    authW.Add(authitem.Key, authitem.Value.Where(x => x != "false").ToList());
                }
            }
            return new JavaScriptSerializer().Serialize(authW);
        }
        protected void SetUpdateData<T>(T originData, T updateData, string[] updateColumn)
        {
            var TypeInfoList = originData.GetType().GetProperties();
            foreach (string column in updateColumn)
            {
                var newData = TypeInfoList.FirstOrDefault(info => info.Name.Equals(column)).GetValue(updateData, null);
                TypeInfoList.FirstOrDefault(info => info.Name.Equals(column)).SetValue(originData, newData);
            }
            TypeInfoList.FirstOrDefault(info => info.Name.Equals("UpdateBy")).SetValue(originData, Session["AdminName"]);
            TypeInfoList.FirstOrDefault(info => info.Name.Equals("UpdateAt")).SetValue(originData, DateTime.UtcNow);
        }
        protected IQueryable<T1> QueryData<T1, T2>(IQueryable<T1> BasicData, T2 QureyData)
        {
            var TypeInfoList = QureyData.GetType().GetProperties();
            foreach (var item in TypeInfoList)
            {
                var Name = item.Name;
                var Val = TypeInfoList.FirstOrDefault(info => info.Name.Equals(Name)).GetValue(QureyData, null);
                if (Val != null)
                {
                    BasicData = BasicData.Where(x => x.GetType().GetProperty(Name).GetValue(BasicData, null) == Val);
                }
            }
            return BasicData;
        }
        public string RenderViewToString(ControllerContext controllerContext, string viewName, object model, ViewDataDictionary viewData = null, TempDataDictionary tempData = null)
        {
            if (viewData == null) viewData = new ViewDataDictionary();

            if (tempData == null) tempData = new TempDataDictionary();

            // assing model to the viewdata
            viewData.Model = model;

            using (var sw = new System.IO.StringWriter())
            {
                // try to find the specified view
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(controllerContext, viewName);
                // create the associated context
                ViewContext viewContext = new ViewContext(controllerContext, viewResult.View, viewData, tempData, sw);
                // write the render view with the given context to the stringwriter
                viewResult.View.Render(viewContext, sw);

                viewResult.ViewEngine.ReleaseView(controllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }
        /// <summary>
        /// 存圖檔
        /// </summary>
        /// <param name="file">圖檔</param>
        /// <returns>回傳路徑</returns>
        public string SaveImg(HttpPostedFileBase file)
        {
            try
            {         
                var Dirpath = Server.MapPath(FileUploads);
                if (!Directory.Exists(Dirpath))
                    Directory.CreateDirectory(Dirpath);
                string FileExtension = Path.GetExtension(file.FileName);
                var fileName = Guid.NewGuid().ToString("N") + FileExtension;
                var path = Path.Combine(Dirpath, fileName);
                file.SaveAs(path);
                return FileUploads.Replace("~", "") + "/" + fileName;
            }
            catch (Exception)
            {
                return "Error";
            }

        }
        /// <summary>
        /// 倉庫等待出貨的庫總量
        /// </summary>
        /// <param name="SKU">SKU</param>
        /// <param name="SCID">SCID</param>
        /// <returns></returns>
        public int GetAwaitingCount(string SKU, string SCID)
        {
            var count = 0;
            if (string.IsNullOrWhiteSpace(SCID))
            {
                return count;
            }
            using (WebClient wc = new WebClient())
            {
                try
                {
                    wc.Encoding = Encoding.UTF8;

                    NameValueCollection nc = new NameValueCollection();
                    nc["Sku"] = SKU;
                    nc["WarehouseID"] = SCID;
                    byte[] bResult = wc.UploadValues(ApiUrl+ "Api/GetSkuInventoryQTY", nc);
                    string resultXML = Encoding.UTF8.GetString(bResult);
                    int.TryParse(resultXML, out count);
                    if (count>0)
                    {
                        count *= -1;
                    }
                }
                catch (WebException ex)
                {
                    //throw new Exception("無法連接遠端伺服器");
                }
            }
            return count;
        }
    }
}