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
using Newtonsoft.Json;
using PurchaseOrderSys.Models;

namespace PurchaseOrderSys.Controllers
{
    public class BaseController : Controller
    {
        protected string ApiUrl = "http://internal.qd.com.tw/";
        //protected string ApiUrl = "http://localhost:49920/";
        protected string FileUploads = "~/FileUploads";
        public static string UserBy = "test";
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
        public List<AwaitingDispatchVM> GetAwaitingCount(string SKU, string SCID)
        {
            var SKUs = new string[] { SKU };
            var SCIDs = new string[] { SCID };
            return GetAwaitingCount(SKUs, SCIDs);
        }
        /// <summary>
        /// 倉庫等待出貨的庫總量
        /// </summary>
        /// <param name="SKUs">SKU</param>
        /// <param name="SCIDs">SCID</param>
        /// <returns></returns>
        public List<AwaitingDispatchVM> GetAwaitingCount(string[] SKUs, string[] SCIDs)
        {

            var AwaitingDispatchList = new List<AwaitingDispatchVM>();
            using (WebClient wc = new WebClient())
            {

                try
                {
                    SKUs = SKUs.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
                    SCIDs = SCIDs.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
                    wc.Encoding = Encoding.UTF8;
                    var nDictionary = new GetSkuInventoryQTYVM { WarehouseIDs = SCIDs, Skus = SKUs };
                    var dataString = JsonConvert.SerializeObject(nDictionary);
                    wc.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                    string resultXML = wc.UploadString(ApiUrl + "Api/GetSkuInventoryQTY", "POST", dataString);
                    AwaitingDispatchList = JsonConvert.DeserializeObject<List<AwaitingDispatchVM>>(resultXML);
                }
                catch (WebException ex)
                {

                }
            }
            return AwaitingDispatchList;
        }
        /// <summary>
        /// 比對資料，並寫回資料庫
        /// </summary>
        /// <param name="PurchaseSKU"></param>
        /// <param name="ExcelSerialslist">序號</param>
        /// <param name="POType">POType</param>
        /// <returns></returns>
        public string SerialsAddCheck(PurchaseSKU PurchaseSKU, List<string> ExcelSerialslist, string POType)
        {
            try
            {


                if (PurchaseSKU.PurchaseOrder != null)
                {
                    POType = PurchaseSKU.PurchaseOrder.POType;
                }
                var QTYOrdered = PurchaseSKU.QTYOrdered;//採購數
                var SerialsQTYList = PurchaseSKU.SerialsLlist.Where(x => x.SerialsType == "PO");//所有序號
                var SerialsQTY = SerialsQTYList.Sum(x => x.SerialsQTY);//入庫數
                if (QTYOrdered >= ExcelSerialslist.Count() + SerialsQTY)
                {
                    foreach (var serials in ExcelSerialslist)
                    {
                        if (SerialsQTYList.Where(x => x.SerialsNo == serials).Any())
                        {
                            return "序號重複";
                        }
                        else
                        {
                            if (POType == "DropshpOrder")//直發一入一出
                            {
                                var SerialsLlist = db.SerialsLlist.Where(x => x.SerialsNo == serials && x.PurchaseSKU.PurchaseOrderID == PurchaseSKU.PurchaseOrderID);//檢查序號是否重複，同訂單同序號不能新增
                                if (!SerialsLlist.Any())
                                {
                                    var dt = DateTime.UtcNow;
                                    var nSerialsLlistIn = new SerialsLlist
                                    {
                                        SerialsType = "DropshpOrderIn",
                                        SerialsNo = serials,
                                        SerialsQTY = 1,
                                        ReceivedBy = UserBy,
                                        ReceivedAt = dt,
                                        CreateBy = UserBy,
                                        CreateAt = dt
                                    };
                                    PurchaseSKU.SerialsLlist.Add(nSerialsLlistIn);
                                    var nSerialsLlistOut = new SerialsLlist
                                    {
                                        SerialsType = "DropshpOrderOut",
                                        SerialsNo = serials,
                                        SerialsQTY = -1,
                                        ReceivedBy = UserBy,
                                        ReceivedAt = dt,
                                        CreateBy = UserBy,
                                        CreateAt = dt
                                    };
                                    nSerialsLlistIn.SerialsLlistC.Add(nSerialsLlistOut);
                                }
                            }
                            else if (POType == "PurchaseOrder")
                            {
                                var SerialsLlist = db.SerialsLlist.Where(x => x.SerialsNo == serials && x.PurchaseSKU.PurchaseOrderID == PurchaseSKU.PurchaseOrderID);//檢查序號是否重複，同訂單同序號不能新增
                                if (!SerialsLlist.Any())
                                {
                                    var dt = DateTime.UtcNow;
                                    var nSerialsLlist = new SerialsLlist
                                    {
                                        SerialsType = "PO",
                                        SerialsNo = serials,
                                        SerialsQTY = 1,
                                        ReceivedBy = UserBy,
                                        ReceivedAt = dt,
                                        CreateBy = UserBy,
                                        CreateAt = dt
                                    };
                                    PurchaseSKU.SerialsLlist.Add(nSerialsLlist);
                                }
                            }
                        }
                    }
                    return "";
                }
                else
                {
                    return "序號不可大於採購數";
                }
            }
            catch (Exception ex)
            {
                return ex.ToString(); 
            }
        }
    }
}