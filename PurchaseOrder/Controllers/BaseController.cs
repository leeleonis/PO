﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.UI;
using Neodynamic.SDK.Web;
using Newtonsoft.Json;
using PurchaseOrderSys.Models;

namespace PurchaseOrderSys.Controllers
{
    public class BaseController : Controller
    {
        protected string ApiUrl = "http://internal.qd.com.tw/";
        //protected string ApiUrl = "http://localhost:49920/";
        protected string FileUploads = "~/Uploads";
        public static string UserBy = "test";
        public static string LangID = "en-US";
        public static string ApiUserName = "test@qd.com.tw";
        public static string ApiPassword = "prU$U9R7CHl3O#uXU6AcH6ch";
        protected PurchaseOrderEntities db = new PurchaseOrderEntities();
        public static SellerCloud_WebService.SC_WebService SCWS;
      
        protected string RenderPartialViewToString()
        {
            return RenderPartialViewToString(null, null);
        }

        protected string RenderPartialViewToString(string viewName)
        {
            return RenderPartialViewToString(viewName, null);
        }

        protected string RenderPartialViewToString(object model)
        {
            return RenderPartialViewToString(null, model);
        }

        protected string RenderPartialViewToString(string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = ControllerContext.RouteData.GetRequiredString("action");

            ViewData.Model = model;

            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                ViewContext viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }
        public ActionResult CMRemoveData(string[] IDList, string SID)
        {
            var Errmsg = "";
            if (IDList != null && IDList.Any())
            {
                var odataList = (List<CMSKUVM>)Session["CMSkuNumberList" + SID];
                foreach (var item in IDList)
                {
                    foreach (var odataListitem in odataList.Where(x => x.ID.ToString() == item || x.SKU == item))
                    {
                        if (odataListitem.ID.HasValue)
                        {
                            var PurchaseSKU = db.PurchaseSKU.Find(odataListitem.ID);
                            if (PurchaseSKU.SerialsLlist.Any())
                            {
                                Errmsg += "【" + PurchaseSKU.SkuNo + "】已有序號不能刪除；";
                            }
                            else
                            {
                                odataListitem.Model = "D";
                            }
                        }
                        else
                        {
                            odataListitem.Model = "D";
                        }
                    }
                }
                Session["CMSkuNumberList" + SID] = odataList;
            }
            else
            {
                Errmsg = "沒有選取SKU";
            }
            if (string.IsNullOrWhiteSpace(Errmsg))
            {
                return Json(new { status = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = false, Errmsg }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult EditCMSKUData(string[] IDList, string SID)
        {
            var Errmsg = "";
            if (IDList != null && IDList.Any())
            {
                var odataList = (List<CMSKUVM>)Session["CMSkuNumberList" + SID];
                foreach (var item in IDList)
                {
                    foreach (var odataListitem in odataList.Where(x => x.ID.ToString() == item || x.SKU == item))
                    {
                        odataListitem.Model = "E";
                    }
                }
            }
            else
            {
                Errmsg = "沒有選取SKU";
            }
            if (string.IsNullOrWhiteSpace(Errmsg))
            {
                return Json(new { status = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = false, Errmsg }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult CMCreatNoteImg(int? ID, HttpPostedFileBase Img, string SID)
        {
            try
            {
                if (Img == null)
                {
                    return Json(new { status = false, Errmsg = "沒有圖檔" }, JsonRequestBehavior.AllowGet);
                }
                var NoteType = Img.ContentType;
                var CMPurchaseNote = new List<PurchaseNote>();
                if (ID.HasValue && ID != 0)
                {
                    var Note = SaveImg(Img);
                    var CreditMemo = db.CreditMemo.Find(ID);
                    CreditMemo.PurchaseNote.Add(new PurchaseNote { IsEnable = true, Note = Note, NoteType = "Url", CreateAt = DateTime.UtcNow, CreateBy = UserBy });
                    db.SaveChanges();
                    CMPurchaseNote = CreditMemo.PurchaseNote.ToList();
                }
                else
                {
                    MemoryStream target = new MemoryStream();

                    Img.InputStream.CopyTo(target);
                    byte[] data = target.ToArray();
                    string Note = Convert.ToBase64String(data, 0, data.Length);
                    CMPurchaseNote = (List<PurchaseNote>)Session["CMPurchaseNote" + SID];
                    if (CMPurchaseNote == null)
                    {
                        CMPurchaseNote = new List<PurchaseNote>();
                    }

                    CMPurchaseNote.Add(new PurchaseNote { IsEnable = true, Note = Note, NoteType = NoteType, CreateAt = DateTime.UtcNow, CreateBy = UserBy });
                    Session["CMPurchaseNote"] = CMPurchaseNote;
                }
                return Json(new { status = true, datalist = CMPurchaseNote.OrderByDescending(x => x.CreateAt).Select(x => new { CreateAt = x.CreateAt.ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss"), x.CreateBy, x.Note, x.NoteType }).ToList() }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, Errmsg = ex.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult CMCreatNote(int? ID, string Note, string SID)
        {
            try
            {
                var CMPurchaseNote = new List<PurchaseNote>();
                if (ID.HasValue && ID != 0)
                {
                    var CreditMemo = db.CreditMemo.Find(ID);
                    CreditMemo.PurchaseNote.Add(new PurchaseNote { IsEnable = true, Note = Note, NoteType = "txt", CreateAt = DateTime.UtcNow, CreateBy = UserBy });
                    db.SaveChanges();
                    CMPurchaseNote = CreditMemo.PurchaseNote.ToList();
                }
                else
                {
                    CMPurchaseNote = (List<PurchaseNote>)Session["CMPurchaseNote" + SID];
                    if (CMPurchaseNote == null)
                    {
                        CMPurchaseNote = new List<PurchaseNote>();
                    }

                    CMPurchaseNote.Add(new PurchaseNote { IsEnable = true, Note = Note, NoteType = "txt", CreateAt = DateTime.UtcNow, CreateBy = UserBy });
                    Session["CMPurchaseNote" + SID] = CMPurchaseNote;
                }
                return Json(new { status = true, datalist = CMPurchaseNote.OrderByDescending(x => x.CreateAt).Select(x => new { CreateAt = x.CreateAt.ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss"), x.CreateBy, x.Note, x.NoteType }).ToList() }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, Errmsg = ex.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }

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
        /// 取得MIME Content Type對應的檔案副檔名
        /// </summary>
        /// <param name="mime"></param>
        /// <returns></returns>
        private static IEnumerable<string> GetMIMESupportedExt(string mime)
        {
            var linq = from item in Microsoft.Win32.Registry.ClassesRoot.GetSubKeyNames()
                       let key = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(item)
                       let value = key.GetValue("Content Type")
                       where value != null && value.ToString().Equals(mime, StringComparison.CurrentCultureIgnoreCase)
                       select item;
            return linq;
        }


        /// <summary>
        /// 存圖檔
        /// </summary>
        /// <param name="base64"></param>
        /// <param name="filetype"></param>
        /// <returns></returns>
        public string SaveImg(string base64, string filetype)
        {
            try
            {
                var FileExtension = GetMIMESupportedExt(filetype).FirstOrDefault() ?? "";
                var fileBytes = Convert.FromBase64String(base64);
                var Dirpath = Server.MapPath(FileUploads);
                if (!Directory.Exists(Dirpath)) Directory.CreateDirectory(Dirpath);
                var fileName = Guid.NewGuid().ToString("N") + FileExtension;
                var path = Path.Combine(Dirpath, fileName);
                FileStream fs = new FileStream(path, FileMode.Create);
                fs.Write(fileBytes, 0, fileBytes.Length);
                fs.Close();
                return FileUploads.Replace("~", "") + "/" + fileName;
            }
            catch (Exception)
            {
                return "Error";
            }
        }
        /// <summary>
        /// 存圖檔
        /// </summary>
        /// <param name="file">圖檔</param>
        /// <param name="UpfileName">資料夾名稱</param> 
        /// <returns>回傳路徑</returns>
        public string SaveImg(HttpPostedFileBase file, string UpfileName = "")
        {
            try
            {
                if (string.IsNullOrWhiteSpace(UpfileName))
                {
                    UpfileName = FileUploads;
                }
                var Dirpath = Server.MapPath(UpfileName);
                if (!Directory.Exists(Dirpath)) Directory.CreateDirectory(Dirpath);
                string FileExtension = Path.GetExtension(file.FileName);
                var fileName = Guid.NewGuid().ToString("N") + FileExtension;
                var path = Path.Combine(Dirpath, fileName);
                file.SaveAs(path);
                return UpfileName.Replace("~", "") + "/" + fileName;
            }
            catch (Exception)
            {
                return "Error";
            }
        }
        /// <summary>
        /// 刪除圖檔
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public bool DeleteImg(string fileName)
        {
            try
            {
                var Dirpath = Server.MapPath(fileName);
                System.IO.File.Delete(Dirpath);
                return true;
            }
            catch (Exception ex)
            {
                return false;
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
                var chkSerialsLlist = db.SerialsLlist.Where(x => ExcelSerialslist.Contains(x.SerialsNo) && x.PurchaseSKU.IsEnable && x.PurchaseSKU.SkuNo == PurchaseSKU.SkuNo).OrderByDescending(x=>x.CreateAt).ToList();//.Select(x => x.SerialsNo)
                if (chkSerialsLlist.Any())//同SKU不可以重複序號
                {
                    if (chkSerialsLlist.FirstOrDefault().SerialsType != "CM")//最後一筆資料不是CM才顯示
                    {
                        return "序號重複：" + string.Join(",", chkSerialsLlist.Select(x => x.SerialsNo).Distinct());
                    }
                }
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
                        if (POType == "DropshpOrder")//直發一入一出
                        {
                            //var SerialsLlist = db.SerialsLlist.Where(x => x.SerialsNo == serials && x.PurchaseSKU.PurchaseOrderID == PurchaseSKU.PurchaseOrderID);//檢查序號是否重複，同訂單同序號不能新增
                            var SerialsLlist = db.SerialsLlist.Where(x => x.SerialsNo == serials && x.PurchaseSKU.SkuNo == PurchaseSKU.SkuNo && !x.SerialsLlistC.Any() && (x.SerialsType == "PO" || x.SerialsType == "TransferIn"));//檢查序號是否重複，同SKU序號不能新增,2019/02/05 加入有已出貨或是CM的紀錄, 就能重新在入庫
                            if (!SerialsLlist.Any())
                            {
                                var dt = DateTime.UtcNow;
                                var nSerialsLlistIn = new SerialsLlist
                                {
                                    IsEnable = true,
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
                                    IsEnable = true,
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
                            //var SerialsLlist = db.SerialsLlist.Where(x => x.SerialsNo == serials && x.PurchaseSKU.PurchaseOrderID == PurchaseSKU.PurchaseOrderID);//檢查序號是否重複，同訂單同序號不能新增
                            var SerialsLlist = db.SerialsLlist.Where(x => x.SerialsNo == serials && x.PurchaseSKU.SkuNo == PurchaseSKU.SkuNo && !x.SerialsLlistC.Any() && (x.SerialsType == "PO" || x.SerialsType == "TransferIn"));//檢查序號是否重複，同SKU序號不能新增,2019/02/05 加入有已出貨或是CM的紀錄, 就能重新在入庫
                            if (!SerialsLlist.Any())
                            {
                                var dt = DateTime.UtcNow;
                                var nSerialsLlist = new SerialsLlist
                                {
                                    IsEnable = true,
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
                    return "";
                }
                else
                {
                    return PurchaseSKU.SkuNo + "：序號數量超過採購數";
                }
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        /// <summary>
        /// 序號用EXCEL檔寫入SC
        /// </summary>
        /// <param name="PurchaseOrder"></param>
        public void CreatSCPObyExcel(PurchaseOrder PurchaseOrder)
        {
            CreatAndEditPOSKUbySC(PurchaseOrder);
            foreach (var SKUitem in PurchaseOrder.PurchaseSKU.Where(x=>x.IsEnable))
            {
                CreatAndEditPOSKUbySC(PurchaseOrder);
                foreach (var Serialitem in SKUitem.SerialsLlist.Where(x=>x.SerialsType=="PO"||x.SerialsType== "DropshpOrderIn"))
                {
                    AddSerialToSC(SKUitem, Serialitem.SerialsNo);
                }
                if (PurchaseOrder.POType == "DropshpOrder")
                {
                    foreach (var Serialitem in SKUitem.SerialsLlist.Where(x => x.SerialsType == "DropshpOrderOut"))
                    {
                        DelSerialToSC(SKUitem, Serialitem.SerialsNo);
                    }
                }
            }
        }
        /// <summary>
        /// SC批次寫入序號
        /// </summary>
        /// <param name="purchaseSKU"></param>
        /// <param name="excelSerialslist"></param>
        public void AddSerialListToSC(PurchaseSKU purchaseSKU, List<string> excelSerialslist)
        {
            foreach (var Serialitem in excelSerialslist)
            {
                AddSerialToSC(purchaseSKU, Serialitem);
            }
        }

        /// <summary>
        /// 已出貨的訂單
        /// </summary>
        /// <param name="OrderID">訂單號</param>
        /// <param name="SourceID">源頭訂單的ID</param>
        /// <param name="UserID">使用者</param>
        /// <param name="Status">訂單狀態 0：未出貨 1：待出貨 3：已出貨</param>
        /// <returns></returns>
        public List<OrderItemData> GetOrderItemData(int? OrderID, string SourceID, string UserID, int? Status)
        {
            var OrderItemDataList = new List<OrderItemData>();
            using (WebClient wc = new WebClient())
            {
                try
                {
                    wc.Encoding = Encoding.UTF8;
                    var nDictionary = new { OrderID, SourceID, UserID, Status };
                    var dataString = JsonConvert.SerializeObject(nDictionary);
                    wc.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                    string resultXML = wc.UploadString(ApiUrl + "Api/GetOrderItemData", "Post", dataString);
                    OrderItemDataList = JsonConvert.DeserializeObject<List<OrderItemData>>(resultXML);
                }
                catch (WebException ex)
                {

                }
            }
            return OrderItemDataList;
        }
        public string GetCountryCode(string countryCode)
        {
            if (countryCode.Length > 2)
            {
                var countryList = CultureInfo.GetCultures(CultureTypes.SpecificCultures).Where(x => x.LCID != 4096).Select(x => new Country(x.LCID)).GroupBy(c => c.ID).Select(c => c.First()).OrderBy(x => x.Name);
                if (countryList.Any(c => c.Name == countryCode))
                {
                    return countryList.First(c => c.Name == countryCode).TwoCode;
                }
            }
            return countryCode;
        }
        /// <summary>
        /// 檢查倉庫是否有SKU
        /// </summary>
        /// <param name="search"></param>
        /// <param name="FromWID"></param>
        /// <returns></returns>
        public List<string> SearchSkuByWarehouse(string search, int FromWID)
        {
            var SKUList = new List<string>();
            //一般入庫
            var PurchaseSKUList = db.PurchaseSKU.Where(x => x.IsEnable && x.PurchaseOrder.IsEnable && x.PurchaseOrder.WarehouseID == FromWID).Select(x => x.SkuNo).ToList();
            SKUList.AddRange(PurchaseSKUList);
            //移倉入庫
            var TransferSKUList = db.TransferSKU.Where(x => x.IsEnable && x.Transfer.IsEnable && x.Transfer.ToWID == FromWID).Select(x => x.SkuNo).ToList();//只找移入的SKU
            SKUList.AddRange(TransferSKUList);
            //RMA入庫
            var NoNewRMAferSKUList = db.RMASerialsLlist.AsEnumerable().Where(x => x.RMASKU.IsEnable && x.RMASKU.RMA.IsEnable && x.WarehouseID == FromWID && string.IsNullOrWhiteSpace(x.NewSkuNo)).Select(x => x.RMASKU.SkuNo).ToList();//沒有新的SKU
            SKUList.AddRange(NoNewRMAferSKUList);
            var NewRMAferSKUList = db.RMASerialsLlist.AsEnumerable().Where(x => x.RMASKU.IsEnable && x.RMASKU.RMA.IsEnable && x.WarehouseID == FromWID && !string.IsNullOrWhiteSpace(x.NewSkuNo)).Select(x => x.NewSkuNo).ToList();//沒有新的SKU
            SKUList.AddRange(NewRMAferSKUList);
            SKUList = SKUList.Distinct().ToList();
            return SKUList;
        }
        public IEnumerable<WarehouseVM> GetWarehouseVMList(Warehouse Warehouse, string Product, int? FulfillableMin, int? FulfillableMax)
        {
            var SCID = Warehouse.WarehouseSummary.Where(x => x.Type == "SCID").FirstOrDefault()?.Val;
            var Awaitinglist = GetAwaitingCount("", SCID);
            var AllSKUList = db.SKU.Where(x => x.IsEnable && x.Status == 1).Select(x => new { x.SkuID, x.SkuLang.FirstOrDefault().Name }).ToList();
            var WarehouseVM = new List<WarehouseVM>();
            var GroupWarehouseVM = new List<WarehouseVM>();
            if (Warehouse.Type == "Interim")
            {
                var NoRmaTransferSKU = db.TransferSKU.Where(x => x.IsEnable && x.Transfer.Interim == Warehouse.ID && x.Transfer.Status == "Shipped" && !x.RMASerialsLlist.Any());//無RMA移倉中
                var RmaTransferSKU = db.TransferSKU.Where(x => x.IsEnable && x.Transfer.Interim == Warehouse.ID && x.Transfer.Status == "Shipped" && x.RMASerialsLlist.Any());//有RMA移倉中
                if (!string.IsNullOrWhiteSpace(Product))
                {
                    NoRmaTransferSKU = NoRmaTransferSKU.Where(x => x.SkuNo == Product);
                    RmaTransferSKU = RmaTransferSKU.Where(x => x.RMASerialsLlist.Where(y => y.NewSkuNo == Product).Any() || x.SkuNo == Product);
                }
                WarehouseVM.AddRange(NoRmaTransferSKU.Select(x => new WarehouseVM
                {
                    ID = x.ID,
                    Name = x.Name,
                    SKU = x.SkuNo,
                    POQTY = x.SerialsLlist.Where(y => (y.SerialsType == "TransferOut" && !y.SerialsLlistC.Any())).Sum(y => y.SerialsQTY) * -1 ?? 0//借用
                }).ToList());
                WarehouseVM.AddRange(RmaTransferSKU.Select(x => new WarehouseVM
                {
                    ID = x.ID,
                    Name = x.Name,
                    SKU = x.SkuNo,
                    POQTY = x.RMASerialsLlist.Where(y => (y.SerialsType == "TransferOut" && !y.RMASerialsLlistC.Any())).Sum(y => y.SerialsQTY) * -1 ?? 0//借用
                }).ToList());
                GroupWarehouseVM = WarehouseVM.GroupBy(x => new { x.SKU }).Select(x => new WarehouseVM //SKU數量總計
                {
                    ID = x.FirstOrDefault().ID,
                    Name = x.FirstOrDefault().Name,
                    SKU = x.Key.SKU,
                    POQTY = x.Sum(p => p.POQTY),
                    CMQTY = x.Sum(p => p.CMQTY),
                    OrderQTY = x.Sum(p => p.OrderQTY),
                    TransferInQTY = x.Sum(p => p.TransferInQTY),
                    TransferOutQTY = x.Sum(p => p.TransferOutQTY),
                    Velocity = x.Sum(p => p.Velocity),
                    TransferAwaiting = x.Sum(p => p.TransferAwaiting),
                    //DaysOfSupply = x.Sum(p => p.DaysOfSupply),
                    //Aggregate = x.Sum(p => p.Aggregate),
                    //Awaiting = x.Sum(p => p.Awaiting),
                    //Fulfillable = x.Sum(p => p.Fulfillable),
                    //Unfulfillable = x.Sum(p => p.Unfulfillable),
                }).ToList();
            }
            else
            {
                var NoList = new List<int>();
                var PurchaseSKU = db.PurchaseSKU.Where(x => x.IsEnable && x.PurchaseOrder.IsEnable).Include(x => x.SerialsLlist).ToList();//PO單
                var TransferToSKU = db.TransferSKU.Where(x => x.IsEnable && x.Transfer.IsEnable).Include(x => x.SerialsLlist).ToList();//移倉單入庫
                var TransferFromSKU = db.TransferSKU.Where(x => x.IsEnable && x.Transfer.IsEnable).Include(x => x.SerialsLlist).ToList();//移倉單出庫
                var RMASerialsLlist = db.RMASerialsLlist.Where(x => x.RMASKU.IsEnable && x.RMASKU.RMA.IsEnable).ToList();//RMA單
                if (Warehouse.ID != 0)
                {
                    PurchaseSKU= PurchaseSKU.Where(x=> x.PurchaseOrder.WarehousePO.ID == Warehouse.ID).ToList();
                    TransferToSKU= TransferToSKU.Where(x => x.Transfer.ToWID == Warehouse.ID).ToList();
                    TransferFromSKU = TransferFromSKU.Where(x => x.Transfer.FromWID == Warehouse.ID).ToList();
                    RMASerialsLlist = RMASerialsLlist.Where(x => x.WarehouseID == Warehouse.ID).ToList();
                }
                if (!string.IsNullOrWhiteSpace(Product))
                {
                    PurchaseSKU = PurchaseSKU.Where(x => x.SkuNo == Product).ToList();
                    TransferToSKU = TransferToSKU.Where(x => x.SkuNo == Product).ToList();
                    TransferFromSKU = TransferFromSKU.Where(x => x.SkuNo == Product).ToList();
                    RMASerialsLlist = RMASerialsLlist.Where(x => (!string.IsNullOrWhiteSpace(x.NewSkuNo) && x.NewSkuNo == Product) || (string.IsNullOrWhiteSpace(x.NewSkuNo) && x.RMASKU.SkuNo == Product)).ToList();
                }
                WarehouseVM.AddRange(PurchaseSKU.Select(x => new WarehouseVM
                {
                    ID = x.ID,
                    Name = x.Name,
                    SKU = x.SkuNo,
                    BackOrdered = x.QTYOrdered ?? 0,
                    POQTY = GetPOQty(x),
                    CMQTY = GetCMQty(x),
                    OrderQTY = GetOrderQTY(x),
                    //TransferInQTY = GetTransferInQTY(x, Warehouse,"", ref NoList),
                    TransferOutQTY = GetTransferOutQTY(x, Warehouse, "", ref NoList),
                    TransferAwaiting = GetTransferAwaiting(x, Warehouse, ref NoList),//等待移倉的數量
                    TransferOutCloseQTY = GetTransferOutCloseQTY(x, Warehouse, ref NoList),//移倉已入庫
                    UnfulfillableRMA = GetUnfulfillableRMA(x, Warehouse, ref NoList),//RMA數量
                    Velocity = GetVelocity(x),
                    DaysOfSupply = 0,
                    Aggregate = 0,//可上架的庫存總數
                    Awaiting = 0,//等待出貨的庫總量
                    Fulfillable = 0,
                    Unfulfillable = 0
                }).ToList());


                WarehouseVM.AddRange(TransferToSKU.Select(x => new WarehouseVM
                {
                    ID = x.ID,
                    Name = x.Name,
                    SKU = x.SkuNo,
                    TransferInQTY = GetTransferInQTY(x, Warehouse, "To", ref NoList),//接入庫當PO
                    TransferOutQTY = GetTransferOutQTY(x, Warehouse, "To", ref NoList) * -1,
                    TransferOutCloseQTY = GetTransferOutCloseQTY(x, Warehouse, ref NoList),//移倉已入庫
                    TransferAwaiting = GetTransferAwaiting(x, Warehouse, ref NoList),//等待移倉的數量
                }).ToList());
                WarehouseVM.AddRange(TransferFromSKU.Select(x => new WarehouseVM
                {
                    ID = x.ID,
                    Name = x.Name,
                    SKU = x.SkuNo,
                    //TransferInQTY = GetTransferInQTY(x, Warehouse, "From", ref NoList),
                    TransferOutQTY = GetTransferOutQTY(x, Warehouse, "From", ref NoList),
                    TransferOutCloseQTY = GetTransferOutCloseQTY(x, Warehouse, ref NoList),
                    TransferAwaiting = GetTransferAwaiting(x, Warehouse, ref NoList),//等待移倉的數量
                }).ToList());
                var NoNewRMASKU = RMASerialsLlist.Where(x => string.IsNullOrWhiteSpace(x.NewSkuNo)).Select(x => x.RMASKU).ToList();//沒有新SKU
                WarehouseVM.AddRange(NoNewRMASKU.Select(x => new WarehouseVM
                {
                    ID = x.ID,
                    Name = x.Name,
                    SKU = x.SkuNo,
                    UnfulfillableRMA = GetRMAInQty(x, x.SkuNo, false, null, Warehouse.ID),
                    TransferInQTY = GetTransferInQTY(x, Warehouse, "", ref NoList),//接入庫當PO
                    TransferOutQTY = GetTransferOutQTY(x, Warehouse, "", ref NoList) * -1,
                    TransferOutCloseQTY = GetTransferOutCloseQTY(x, Warehouse, ref NoList),//移倉已入庫
                    TransferAwaiting = GetTransferAwaiting(x, Warehouse, ref NoList),//等待移倉的數量
                }).ToList());
                var NewRMASKU = RMASerialsLlist.Where(x => !string.IsNullOrWhiteSpace(x.NewSkuNo)).Select(x => x.RMASKU).ToList();//有新SKU
                WarehouseVM.AddRange(NewRMASKU.Select(x => new WarehouseVM
                {
                    ID = x.ID,
                    Name = x.Name,
                    SKU = x.RMASerialsLlist.FirstOrDefault().NewSkuNo,
                    UnfulfillableRMA = GetRMAInQty(x, x.RMASerialsLlist.FirstOrDefault().NewSkuNo, true, null, Warehouse.ID),
                    TransferInQTY = GetTransferInQTY(x, Warehouse, "", ref NoList),//接入庫當PO
                    TransferOutQTY = GetTransferOutQTY(x, Warehouse, "", ref NoList) * -1,
                    TransferOutCloseQTY = GetTransferOutCloseQTY(x, Warehouse, ref NoList),//移倉已入庫
                    TransferAwaiting = GetTransferAwaiting(x, Warehouse, ref NoList),//等待移倉的數量
                }).ToList());

                GroupWarehouseVM = WarehouseVM.GroupBy(x => new { x.SKU }).Select(x => new WarehouseVM //SKU數量總計
                {
                    ID = x.FirstOrDefault().ID,
                    Name = x.FirstOrDefault().Name,
                    SKU = x.Key.SKU,
                    BackOrdered = x.Sum(p => p.BackOrdered),
                    POQTY = x.Sum(p => p.POQTY),
                    CMQTY = x.Sum(p => p.CMQTY),
                    OrderQTY = x.Sum(p => p.OrderQTY),
                    TransferInQTY = x.Sum(p => p.TransferInQTY),
                    TransferOutQTY = x.Sum(p => p.TransferOutQTY),
                    Velocity = x.Sum(p => p.Velocity),
                    TransferAwaiting = x.Sum(p => p.TransferAwaiting),
                    Fulfillable = x.Sum(p => p.Fulfillable),
                    Unfulfillable = x.Sum(p => p.TransferOutCloseQTY),
                    DaysOfSupply = x.Sum(p => p.DaysOfSupply),
                    Aggregate = x.Sum(p => p.Aggregate),
                    Awaiting = x.Sum(p => p.Awaiting),
                    UnfulfillableRMA = x.Sum(p => p.UnfulfillableRMA),
                    //Fulfillable = x.Sum(p => p.Fulfillable),
                }).ToList();
            }
            if (string.IsNullOrWhiteSpace(Product))
            {
                //把所有的SKU放進去
                foreach (var item in AllSKUList)
                {
                    if (!GroupWarehouseVM.Where(x => x.SKU == item.SkuID).Any())
                    {
                        GroupWarehouseVM.Add(new WarehouseVM
                        {
                            ID = 0,
                            SKU = item.SkuID,
                            Name = item.Name,
                            POQTY = 0,
                            CMQTY = 0,
                            OrderQTY = 0,
                            TransferInQTY = 0,
                            TransferOutQTY = 0,
                            Velocity = 0,
                            DaysOfSupply = 0,
                            Aggregate = 0,//可上架的庫存總數
                            Awaiting = 0,//等待出貨的庫總量
                            Fulfillable = 0,
                            Unfulfillable = 0
                        });
                    }
                }
            }

            foreach (var item in GroupWarehouseVM)
            {
                item.Unfulfillable = Math.Abs(item.Unfulfillable);
                item.Fulfillable = item.POQTY + item.TransferInQTY - item.TransferAwaiting + item.UnfulfillableRMA;
                item.Awaiting = (Awaitinglist.Where(x => x.SKU == item.SKU && x.SCID == SCID).FirstOrDefault()?.QTY ?? 0) - item.TransferAwaiting;
                item.Aggregate = item.Fulfillable - item.Awaiting - item.UnfulfillableRMA;//Aggregate = Fulfillable - Awaiting dispatch
                item.DaysOfSupply = item.Aggregate != 0 && item.Velocity != 0 ? item.Aggregate / item.Velocity / 30 : 0;  //Days of supply 算法: Aggregate / Velocity (30 days) / 30
                                                                                                                          //item.Fulfillable = item.Awaiting + item.Aggregate; //Fulfillable = Awaiting dispatch + Aggregate 2018/12/28 拿掉公式
            }
            if (FulfillableMin.HasValue)
            {
                GroupWarehouseVM = GroupWarehouseVM.Where(x => x.Aggregate >= FulfillableMin).ToList();
            }
            if (FulfillableMax.HasValue)
            {
                GroupWarehouseVM = GroupWarehouseVM.Where(x => x.Aggregate <= FulfillableMax).ToList();
            }
            return GroupWarehouseVM.OrderByDescending(x => x.Fulfillable).ThenByDescending(x => x.Awaiting).ToList();
        }

        /// <summary>
        /// 取30天內訂單出貨數量
        /// </summary>
        /// <param name="PurchaseSKU"></param>
        /// <returns></returns>
        public int GetVelocity(PurchaseSKU PurchaseSKU)
        {
            var edt = DateTime.UtcNow;
            var sdt = edt.AddDays(-30);
            var count = 0;
            count = PurchaseSKU.SerialsLlist.Where(z => z.SerialsType == "Order" && z.CreateAt >= sdt && z.CreateAt <= sdt).Sum(z => z.SerialsQTY).Value;
            return count;
        }

        /// <summary>
        /// 取RMA數量
        /// </summary>
        /// <param name="SKU"></param>
        /// <param name="WarehouseType">倉庫</param>
        /// <returns></returns>
        public int GetUnfulfillableRMA(dynamic SKU, Warehouse Warehouse, ref List<int> WNoList)
        {
            if (WNoList == null)
            {
                WNoList = new List<int>();
            }
            var NoList = WNoList;
            var count = 0;
            //只找移出倉的ID
            if (SKU.GetType().BaseType.Name == typeof(PurchaseSKU).Name)
            {
                var PurchaseSKU = (PurchaseSKU)SKU;
                var SerialsLlist = PurchaseSKU.SerialsLlist.Where(y => !NoList.Contains(y.ID) && y.SerialsType == "RMA" && !y.SerialsLlistC.Any());
                count = SerialsLlist.Sum(y => y.SerialsQTY).Value;
                WNoList.AddRange(SerialsLlist.Select(x => x.ID));
            }
            else if (SKU.GetType().BaseType.Name == typeof(TransferSKU).Name)
            {
                var TransferSKU = (TransferSKU)SKU;
                var SerialsLlist = TransferSKU.SerialsLlist.Where(y => !NoList.Contains(y.ID) && y.SerialsType == "RMA" && !y.SerialsLlistC.Any());
                count = SerialsLlist.Sum(y => y.SerialsQTY).Value;
                WNoList.AddRange(SerialsLlist.Select(x => x.ID));
            }
            return count;
        }
        /// <summary>
        /// 取移庫入庫序號數量
        /// </summary>
        /// <param name="SKU"></param>
        /// <param name="WarehouseType">倉庫</param>
        /// <returns></returns>
        public int GetTransferOutQTY(dynamic SKU, Warehouse Warehouse, string WarehouseKey, ref List<int> WNoList)
        {
            return GetTransferAwaitingOut(SKU, "Shipped", Warehouse, WarehouseKey, ref WNoList);
        }

        /// <summary>
        /// 待移倉的數量
        /// </summary>
        /// <param name="SKU"></param>
        /// <param name="Warehouse">倉庫</param>
        /// <returns></returns>
        public int GetTransferAwaiting(dynamic SKU, Warehouse Warehouse, ref List<int> WNoList)
        {
            return GetTransferAwaitingOut(SKU, "Requested", Warehouse, "", ref WNoList);
        }
        /// <summary>
        /// 還在運送中的數量
        /// </summary>
        /// <param name="x"></param>
        /// <param name="warehouse"></param>
        /// <returns></returns>
        public int GetTransferOutCloseQTY(dynamic SKU, Warehouse Warehouse, ref List<int> WNoList)
        {
            if (WNoList == null)
            {
                WNoList = new List<int>();
            }
            var NoList = WNoList;
            var count = 0;
            //只找移出倉的ID
            if (SKU.GetType().BaseType.Name == typeof(PurchaseSKU).Name)
            {
                var PurchaseSKU = (PurchaseSKU)SKU;
                var SerialsLlist = PurchaseSKU.SerialsLlist.Where(y => !NoList.Contains(y.ID) && y.SerialsType == "TransferOut" && y.TransferSKU.IsEnable && y.TransferSKU.Transfer.IsEnable && y.TransferSKU.Transfer.FromWID == Warehouse.ID && y.TransferSKU.Transfer.Status == "Shipped" && !y.SerialsLlistC.Any());
                count = SerialsLlist.Sum(y => y.SerialsQTY).Value;
                WNoList.AddRange(SerialsLlist.Select(x => x.ID));
            }
            else if (SKU.GetType().BaseType.Name == typeof(TransferSKU).Name)
            {
                var TransferSKU = (TransferSKU)SKU;
                var SerialsLlist = TransferSKU.SerialsLlist.Where(y => !NoList.Contains(y.ID) && y.SerialsType == "TransferOut" && y.TransferSKU.IsEnable && y.TransferSKU.Transfer.IsEnable && y.TransferSKU.Transfer.FromWID == Warehouse.ID && y.TransferSKU.Transfer.Status == "Shipped" && !y.SerialsLlistC.Any());
                count = SerialsLlist.Sum(y => y.SerialsQTY).Value;
                WNoList.AddRange(SerialsLlist.Select(x => x.ID));
            }
            return count;
        }


        public int GetTransferAwaitingOut(dynamic SKU, string Status, Warehouse Warehouse, string WarehouseKey, ref List<int> WNoList)
        {
            if (WNoList == null)
            {
                WNoList = new List<int>();
            }
            var NoList = WNoList;
            var count = 0;
            //只找移出倉的ID
            if (SKU.GetType().BaseType.Name == typeof(PurchaseSKU).Name)
            {
                var PurchaseSKU = (PurchaseSKU)SKU;
                var SerialsLlist = PurchaseSKU.SerialsLlist.Where(y => !NoList.Contains(y.ID));
                SerialsLlist = SerialsLlist.Where(y => y.SerialsType == "TransferOut");
                SerialsLlist = SerialsLlist.Where(y => y.TransferSKU.IsEnable);
                SerialsLlist = SerialsLlist.Where(y => y.TransferSKU.Transfer.IsEnable);
                if (WarehouseKey == "To")
                {
                    SerialsLlist = SerialsLlist.Where(y => y.TransferSKU.Transfer.ToWID == Warehouse.ID);
                }
                else
                {
                    SerialsLlist = SerialsLlist.Where(y => y.TransferSKU.Transfer.FromWID == Warehouse.ID);
                }
                SerialsLlist = SerialsLlist.Where(y => y.TransferSKU.Transfer.Status == Status);
                SerialsLlist = SerialsLlist.Where(y => !y.SerialsLlistC.Any());
                count = SerialsLlist.Sum(y => y.SerialsQTY).Value;
                WNoList.AddRange(SerialsLlist.Select(x => x.ID));
            }
            else if (SKU.GetType().BaseType.Name == typeof(TransferSKU).Name)
            {
                var TransferSKU = (TransferSKU)SKU;
                //var SerialsLlist = TransferSKU.SerialsLlist;

                var SerialsLlist = TransferSKU.SerialsLlist.Where(y => !NoList.Contains(y.ID));
                SerialsLlist = SerialsLlist.Where(y => y.SerialsType == "TransferOut");
                SerialsLlist = SerialsLlist.Where(y => y.TransferSKU.IsEnable);
                SerialsLlist = SerialsLlist.Where(y => y.TransferSKU.Transfer.IsEnable).ToList();
                if (WarehouseKey == "To")
                {
                    SerialsLlist = SerialsLlist.Where(y => y.TransferSKU.Transfer.ToWID == Warehouse.ID);
                }
                else
                {
                    SerialsLlist = SerialsLlist.Where(y => y.TransferSKU.Transfer.FromWID == Warehouse.ID);
                }
                SerialsLlist = SerialsLlist.Where(y => y.TransferSKU.Transfer.Status == Status).ToList();
                SerialsLlist = SerialsLlist.Where(y => !y.SerialsLlistC.Any()).ToList();
                count += SerialsLlist.Sum(y => y.SerialsQTY).Value;


                var RMASerialsLlist = TransferSKU.RMASerialsLlist.Where(y => !NoList.Contains(y.ID));
                RMASerialsLlist = RMASerialsLlist.Where(y => y.SerialsType == "TransferOut");
                RMASerialsLlist = RMASerialsLlist.Where(y => y.TransferSKU.IsEnable);
                RMASerialsLlist = RMASerialsLlist.Where(y => y.TransferSKU.Transfer.IsEnable).ToList();
                if (WarehouseKey == "To")
                {
                    RMASerialsLlist = RMASerialsLlist.Where(y => y.TransferSKU.Transfer.ToWID == Warehouse.ID);
                }
                else
                {
                    RMASerialsLlist = RMASerialsLlist.Where(y => y.TransferSKU.Transfer.FromWID == Warehouse.ID);
                }
                RMASerialsLlist = RMASerialsLlist.Where(y => y.TransferSKU.Transfer.Status == Status).ToList();
                RMASerialsLlist = RMASerialsLlist.Where(y => !y.RMASerialsLlistC.Any()).ToList();
                count += RMASerialsLlist.Sum(y => y.SerialsQTY).Value;

                WNoList.AddRange(RMASerialsLlist.Select(x => x.ID));
                //count = TransferSKU.SerialsLlist.Where(y => !NoList.Contains(y.ID) && y.SerialsType == "TransferOut" && y.TransferSKU.IsEnable && y.TransferSKU.Transfer.IsEnable && y.TransferSKU.Transfer.FromWID == Warehouse.ID && y.TransferSKU.Transfer.Status == Status).Sum(y => y.SerialsQTY).Value;
            }
            return count;
        }
        /// <summary>
        /// 取移庫出庫序號數量
        /// </summary>
        /// <param name="SKU"></param>
        /// <param name="Warehouse"></param>
        /// <returns></returns>
        public int GetTransferInQTY(dynamic SKU, Warehouse Warehouse, string WarehouseKey, ref List<int> WNoList)
        {
            if (WNoList == null)
            {
                WNoList = new List<int>();
            }
            var NoList = WNoList;
            var count = 0;
            //只找移入倉的ID
            if (SKU.GetType().BaseType.Name == typeof(PurchaseSKU).Name)
            {
                var PurchaseSKU = (PurchaseSKU)SKU;
                //var SerialsLlist = PurchaseSKU.SerialsLlist;
                var SerialsLlist = PurchaseSKU.SerialsLlist.Where(y => !NoList.Contains(y.ID) && !y.SerialsLlistC.Any()); ;
                SerialsLlist = SerialsLlist.Where(y => y.SerialsType == "TransferIn");
                SerialsLlist = SerialsLlist.Where(y => y.TransferSKU.IsEnable);
                SerialsLlist = SerialsLlist.Where(y => y.TransferSKU.Transfer.IsEnable);
                if (WarehouseKey == "From")
                {
                    SerialsLlist = SerialsLlist.Where(y => y.TransferSKU.Transfer.FromWID == Warehouse.ID);
                }
                else
                {
                    SerialsLlist = SerialsLlist.Where(y => y.TransferSKU.Transfer.ToWID == Warehouse.ID);
                }
                WNoList.AddRange(SerialsLlist.Select(x => x.ID));
                //count = PurchaseSKU.SerialsLlist.Where(y => !NoList.Contains(y.ID) && y.SerialsType == "TransferIn" && y.TransferSKU.IsEnable && y.TransferSKU.Transfer.IsEnable && y.TransferSKU.Transfer.ToWID == Warehouse.ID).Sum(y => y.SerialsQTY).Value;
            }
            else if (SKU.GetType().BaseType.Name == typeof(TransferSKU).Name)
            {
                var TransferSKU = (TransferSKU)SKU;
                if (TransferSKU.Transfer.Status != "Shipped")
                {
                    var SerialsLlist = TransferSKU.SerialsLlist.Where(y => !NoList.Contains(y.ID));
                    SerialsLlist = SerialsLlist.Where(y => !y.SerialsLlistC.Any());
                    SerialsLlist = SerialsLlist.Where(y => y.SerialsType == "TransferIn");
                    SerialsLlist = SerialsLlist.Where(y => y.TransferSKU.IsEnable);
                    SerialsLlist = SerialsLlist.Where(y => y.TransferSKU.Transfer.IsEnable);
                    if (WarehouseKey == "From")
                    {
                        SerialsLlist = SerialsLlist.Where(y => y.TransferSKU.Transfer.FromWID == Warehouse.ID);
                    }
                    else
                    {
                        SerialsLlist = SerialsLlist.Where(y => y.TransferSKU.Transfer.ToWID == Warehouse.ID);
                    }
                    count = SerialsLlist.Sum(y => y.SerialsQTY).Value;
                    WNoList.AddRange(SerialsLlist.Select(x => x.ID));
                }
            }
            return count;
        }
        /// <summary>
        /// 取PO採購數量
        /// </summary>
        /// <param name="PurchaseSKU"></param>
        /// <returns></returns>
        public int GetOrderQTY(dynamic SKU, List<int> NoList = null)
        {
            if (NoList == null)
            {
                NoList = new List<int>();
            }
            var count = 0;
            //只找移入倉的ID
            if (SKU.GetType().BaseType.Name == typeof(PurchaseSKU).Name)
            {
                var PurchaseSKU = (PurchaseSKU)SKU;
                var SerialsLlist = PurchaseSKU.SerialsLlist.Where(z => !NoList.Contains(z.ID) && z.SerialsType == "Order");
                count = SerialsLlist.Sum(y => y.SerialsQTY).Value;
                //WNoList.AddRange(SerialsLlist.Select(x => x.ID));
            }
            else if (SKU.GetType().BaseType.Name == typeof(TransferSKU).Name)
            {
                var TransferSKU = (TransferSKU)SKU;
                var SerialsLlist = TransferSKU.SerialsLlist.Where(z => !NoList.Contains(z.ID) && z.SerialsType == "Order");
                count = SerialsLlist.Sum(y => y.SerialsQTY).Value;
                //WNoList.AddRange(SerialsLlist.Select(x => x.ID));
            }

            return count;


        }
        /// <summary>
        /// 取PO序號數量
        /// </summary>
        /// <param name="PurchaseSKU"></param>
        /// <returns></returns>
        public int GetPOQty(dynamic SKU, List<int> NoList = null, int? WarehouseID = null)
        {
            if (NoList == null)
            {
                NoList = new List<int>();
            }
            var count = 0;
            if (SKU.GetType().BaseType.Name == typeof(PurchaseSKU).Name)
            {
                var PurchaseSKU = (PurchaseSKU)SKU;
                var SerialsLlist = PurchaseSKU.SerialsLlist.Where(y => !NoList.Contains(y.ID) && y.SerialsType == "PO" && !y.SerialsLlistC.Any());//有輸入直接讀輸入，沒輸入計算序號數
                count = SerialsLlist.Sum(y => y.SerialsQTY) ?? 0;
                //if (PurchaseSKU.QTYFulfilled.HasValue && PurchaseSKU.QTYFulfilled > 0)
                //{
                //    count = PurchaseSKU.QTYFulfilled.Value;
                //}
                //else
                //{
                //}
            }
            else if (SKU.GetType().BaseType.Name == typeof(TransferSKU).Name)
            {
                var TransferSKU = (TransferSKU)SKU;
                var SerialsLlist = TransferSKU.SerialsLlist.Where(y => !NoList.Contains(y.ID) && y.SerialsType == "TransferIn" && !y.SerialsLlistC.Any());//有輸入直接讀輸入，沒輸入計算序號數
                count = SerialsLlist.Sum(y => y.SerialsQTY) ?? 0;
                //WNoList.AddRange(SerialsLlist.Select(x => x.ID));
                //if (TransferSKU.QTYFulfilled.HasValue && TransferSKU.QTYFulfilled > 0)
                //{
                //    count = TransferSKU.QTYFulfilled.Value;
                //}
                //else
                //{
                //}
            }
            return count;
        }
        /// <summary>
        /// 計算RMAIN的庫存
        /// </summary>
        /// <param name="SKU"></param>
        /// <param name="SKUNO"></param>
        /// <param name="NoList"></param>
        /// <param name="WarehouseID"></param>
        /// <returns></returns>
        public int GetRMAInQty(dynamic SKU, string SKUNO, bool IsNew, List<int> NoList = null, int? WarehouseID = null)
        {
            if (NoList == null)
            {
                NoList = new List<int>();
            }
            var count = 0;
            if (SKU.GetType().BaseType.Name == typeof(RMASKU).Name)
            {
                var RMASKU = (RMASKU)SKU;
                var SerialsLlist = RMASKU.RMASerialsLlist.Where(y => y.WarehouseID == WarehouseID && !NoList.Contains(y.ID));//有輸入直接讀輸入，沒輸入計算序號數
                if (!SerialsLlist.Where(x => x.SerialsType == "TransferOut" && (x.TransferSKU.Transfer.Status == "Shipped" || x.TransferSKU.Transfer.Status == "Received" || x.TransferSKU.Transfer.Status == "Completed")).Any())
                {
                    SerialsLlist = SerialsLlist.Where(x => x.SerialsType != "TransferOut");
                }
                if (IsNew)
                {
                    count = SerialsLlist.Where(x => x.NewSkuNo == SKUNO).Sum(y => y.SerialsQTY) ?? 0;
                }
                else
                {
                    count = SerialsLlist.Where(x => string.IsNullOrWhiteSpace(x.NewSkuNo)).Sum(y => y.SerialsQTY) ?? 0;
                }

                //WNoList.AddRange(SerialsLlist.Select(x => x.ID));
                //if (TransferSKU.QTYFulfilled.HasValue && TransferSKU.QTYFulfilled > 0)
                //{
                //    count = TransferSKU.QTYFulfilled.Value;
                //}
                //else
                //{
                //}
            }
            return count;
        }

        /// <summary>
        /// 取CM序號數量
        /// </summary>
        /// <param name="PurchaseSKU"></param>
        /// <returns></returns>
        public int GetCMQty(dynamic SKU, List<int> NoList = null)
        {
            if (NoList == null)
            {
                NoList = new List<int>();
            }
            var count = 0;
            if (SKU.GetType().BaseType.Name == typeof(PurchaseSKU).Name)
            {
                var PurchaseSKU = (PurchaseSKU)SKU;
                foreach (var Serialsitem in PurchaseSKU.SerialsLlist.Where(x => !NoList.Contains(x.ID) && x.SerialsLlistC.Any(y => y.SerialsType == "CM")))
                {
                    var SerialsLlist = Serialsitem.SerialsLlistC.Where(x => !NoList.Contains(x.ID) && x.SerialsType == "CM");
                    count += SerialsLlist.Sum(y => y.SerialsQTY).Value;
                    //WNoList.AddRange(SerialsLlist.Select(x => x.ID));
                }
            }
            else if (SKU.GetType().BaseType.Name == typeof(TransferSKU).Name)
            {
                var TransferSKU = (TransferSKU)SKU;
                foreach (var Serialsitem in TransferSKU.SerialsLlist.Where(x => !NoList.Contains(x.ID) && x.SerialsLlistC.Any(y => y.SerialsType == "CM")))
                {
                    var SerialsLlist = Serialsitem.SerialsLlistC.Where(x => !NoList.Contains(x.ID) && x.SerialsType == "CM");
                    count += SerialsLlist.Sum(y => y.SerialsQTY).Value;
                    //WNoList.AddRange(SerialsLlist.Select(x => x.ID));
                }
            }
            return count;
        }
        /// <summary>
        /// 建立PO單內的SKU,如果沒資料就新增，有數量就修改
        /// </summary>
        /// <param name="nPurchaseOrder"></param>
        public void CreatAndEditPOSKUbySC(PurchaseOrder nPurchaseOrder)
        {
            if (nPurchaseOrder.SCPurchaseID.HasValue)
            {
                var SCPurchase = SCWS.Get_PurchaseOrder(nPurchaseOrder.SCPurchaseID.Value);
                foreach (var skuitem in nPurchaseOrder.PurchaseSKU.Where(x=>x.IsEnable))
                {
                    var Products = SCPurchase.Products.Where(x => x.ProductID == skuitem.SkuNo);
                    if (!Products.Any())
                    {
                        var newPurchaseItem = SCWS.Create_PurchaseOrder_Item(new PurchaseOrderService.PurchaseItem()
                        {
                            PurchaseID = SCPurchase.ID,
                            ProductID = skuitem.SkuNo,//SKU
                            ProductName = skuitem.Name,
                            UPC = skuitem.UPCEAN,
                            QtyOrdered = skuitem.QTYOrdered ?? 0,
                            QtyReceived = 0,
                            QtyReceivedToDate = 0,
                            DefaultWarehouseID = SCPurchase.DefaultWarehouseID
                        });
                    }
                    else
                    {
                        var Product = Products.FirstOrDefault();
                        if (Product.QtyOrdered != skuitem.QTYOrdered)
                        {
                            Product.QtyOrdered = skuitem.QTYOrdered ?? 0;
                            var UpProduct = SCWS.PurchaseOrderItems_Update(Product);
                        }
                        if (Product.DefaultWarehouseID != SCPurchase.DefaultWarehouseID)
                        {
                            Product.DefaultWarehouseID = SCPurchase.DefaultWarehouseID;
                            var UpProduct = SCWS.PurchaseOrderItems_Update(Product);
                        }
                    }
                }
            }
            else
            {

            }
        }
        /// <summary>
        /// 建立SC的PO單
        /// </summary>
        /// <param name="nPurchaseOrder"></param>
        /// <returns></returns>
        public PurchaseOrderService.Purchase CreatPObySC(PurchaseOrder nPurchaseOrder)
        {
            var WarehouseID = 0;
            var Warehouse = db.Warehouse.Find(nPurchaseOrder.WarehouseID);
            var SCID = Warehouse.WarehouseSummary.Where(x => x.Type == "SCID").FirstOrDefault().Val;
            var VendorID = db.VendorLIst.Find(nPurchaseOrder.VendorID).SCID;
            var Company = db.Company.Find(nPurchaseOrder.CompanyID);
            int.TryParse(SCID, out WarehouseID);
            var PurchaseTitle = "PO:" + nPurchaseOrder.ID;
            if (!string.IsNullOrWhiteSpace(nPurchaseOrder.Description))
            {
                PurchaseTitle += ";Description" + nPurchaseOrder.Description;
            }
            var newPurchase = SCWS.Create_PurchaseOrder(new PurchaseOrderService.Purchase()//建立PO表頭
            {
                ID = 0,
                CompanyID = Company.CompanySCID.Value,
                Priority = PurchaseOrderService.PriorityCodeType.Normal,
                Status = PurchaseOrderService.PurchaseStatus.Ordered,
                PurchaseTitle = PurchaseTitle,
                VendorID = VendorID ?? 0,
                VendorInvoiceNumber = nPurchaseOrder.VendorLIst.VendorNo,
                //Memo = nPurchaseOrder.Memo,
                DefaultWarehouseID = WarehouseID,
                CreatedBy = SCWS.UserID,
                CreatedOn = SCWS.SyncOn
            });
            return newPurchase;
        }
        /// <summary>
        /// 修改SC上的訂單資料
        /// </summary>
        /// <param name="nPurchaseOrder"></param>
        /// <returns></returns>
        public bool UpdatePObySC(PurchaseOrder nPurchaseOrder)
        {
            var WarehouseID = 0;
            var Warehouse = db.Warehouse.Find(nPurchaseOrder.WarehouseID);
            var SCID = Warehouse.WarehouseSummary.Where(x => x.Type == "SCID").FirstOrDefault().Val;
            var VendorID = db.VendorLIst.Find(nPurchaseOrder.VendorID).SCID;
            var Company = db.Company.Find(nPurchaseOrder.CompanyID);
            int.TryParse(SCID, out WarehouseID);
            var PurchaseTitle = "PO:" + nPurchaseOrder.ID;
            if (!string.IsNullOrWhiteSpace(nPurchaseOrder.Description))
            {
                PurchaseTitle += ";Description" + nPurchaseOrder.Description;
            }
            var purchaseOrder = SCWS.Get_PurchaseOrder(nPurchaseOrder.SCPurchaseID.Value);
            purchaseOrder.CompanyID = Company.CompanySCID.Value;
            purchaseOrder.PurchaseTitle = PurchaseTitle;
            purchaseOrder.VendorID = VendorID ?? 0;
            purchaseOrder.VendorInvoiceNumber = nPurchaseOrder.VendorLIst.VendorNo;
            purchaseOrder.DefaultWarehouseID = WarehouseID;
            return  SCWS.UpdatePurchaseOrder(purchaseOrder);
        }
        /// <summary>
        /// 序號入庫SC
        /// </summary>
        /// <param name="PurchaseSKU"></param>
        /// <param name="SerialsNo"></param>
        public void AddSerialToSC(PurchaseSKU PurchaseSKU, string SerialsNo)
        {
            if (PurchaseSKU.PurchaseOrder.SCPurchaseID.HasValue)
            {
                var purchaseOrder = SCWS.Get_PurchaseOrder(PurchaseSKU.PurchaseOrder.SCPurchaseID.Value);
                var receiveRequest = new PurchaseOrderService.PurchaseItemReceiveRequest() { PurchaseID = purchaseOrder.ID };
                var receiveRequestProduct = new List<PurchaseOrderService.PurchaseItemReceiveRequestProduct>();
                foreach (var purchaseItem in purchaseOrder.Products.Where(x => x.ProductID == PurchaseSKU.SkuNo))
                {
                    var Serial_All = SCWS.PurchaseItemReceiveSerial_All_New(purchaseItem.ProductID, purchaseOrder.ID);
                    if (!Serial_All.SerialsList.Where(x => x.SerialNumber == SerialsNo).Any())//沒有序號才能新增
                    {
                        receiveRequestProduct.Add(new PurchaseOrderService.PurchaseItemReceiveRequestProduct()
                        {
                            QtyReceived = 1,
                            WarehouseID = purchaseItem.DefaultWarehouseID,
                            PurchaseItemID = purchaseItem.ID,
                            PurchaseID = purchaseItem.PurchaseID
                        });
                        PurchaseOrderService.PurchaseItemReceive[] purchaseItemReceive = null;
                        var receiveSerial = new List<PurchaseOrderService.PurchaseItemReceiveSerial>();
                        if (receiveRequestProduct.Any())
                        {
                            receiveRequest.Products = receiveRequestProduct.ToArray();
                            purchaseItemReceive = SCWS.Create_PurchaseOrder_ItemReceive(receiveRequest);
                            receiveSerial = new List<PurchaseOrderService.PurchaseItemReceiveSerial>();
                            receiveSerial.Add(new PurchaseOrderService.PurchaseItemReceiveSerial()
                            {
                                PurchaseID = purchaseOrder.ID,
                                WarehouseID = purchaseOrder.DefaultWarehouseID,
                                ProductID = purchaseItem.ProductID,
                                PurchaseReceiveID = purchaseItemReceive.FirstOrDefault().Id,
                                SerialNumber = SerialsNo
                            });
                            SCWS.Update_PurchaseOrder_ItemReceive_Serials(receiveSerial.ToArray());

                        }
                    }
                }
            }
        }
        /// <summary>
        /// 刪除sc上的序號
        /// </summary>
        /// <param name="PurchaseSKU"></param>
        /// <param name="SerialsNo"></param>
        public void DelSerialToSC(PurchaseSKU PurchaseSKU, string SerialsNo)
        {
            var purchaseOrder = SCWS.Get_PurchaseOrder(PurchaseSKU.PurchaseOrder.SCPurchaseID.Value);
            foreach (var purchaseItem in purchaseOrder.Products.Where(x => x.ProductID == PurchaseSKU.SkuNo))
            {
                var Serial_All = SCWS.PurchaseItemReceiveSerial_All_New(purchaseItem.ProductID, purchaseOrder.ID);
                if (Serial_All.SerialsList.Where(x => x.SerialNumber == SerialsNo).Any())//有序號才能刪除
                {
                    SCWS.PurchaseItem_DeleteSerials(purchaseItem.ProductID, purchaseOrder.ID, purchaseItem.DefaultWarehouseID, SerialsNo);//SC上的序號移除
                }
            }
        }
        public string GetSerialMulti(TransferSKU item)
        {
            var SerialsLlist = item.SerialsLlist.Where(x => x.SerialsType == "TransferOut");
            var RMASerialsLlist = item.RMASerialsLlist.Where(x => x.SerialsType == "TransferOut");
            if (SerialsLlist.Any() && RMASerialsLlist.Any())
            {
                return "Multi";
            }
            else if (SerialsLlist.Any())
            {
                if (SerialsLlist.Count() > 1)
                {
                    return "Multi";
                }
                else
                {
                    return SerialsLlist.FirstOrDefault().SerialsNo;
                }
            }
            else if (RMASerialsLlist.Any())
            {
                if (RMASerialsLlist.Count() > 1)
                {
                    return "Multi";
                }
                else
                {
                    return RMASerialsLlist.FirstOrDefault().SerialsNo;
                }
            }
            else
            {
                return "";
            }
        }
        public int? PrepSerialChk(TransferSKU item)
        {
            var tQTY = item.QTY;
            var SerialsLlist = item.SerialsLlist.Where(x => x.SerialsType == "TransferOut").Sum(x => x.SerialsQTY);
            var RMASerialsLlist = item.RMASerialsLlist.Where(x => x.SerialsType == "TransferOut").Sum(x => x.SerialsQTY);
            return SerialsLlist + RMASerialsLlist + tQTY;
        }
        public ActionResult PrepVMList(string SID, string Key)
        {
            var PrepVMList = (List<TransferItemVM>)Session[Key + "PrepVMList" + SID];
            var PrepTableList = new List<PrepTable>();
            if (PrepVMList != null)
            {
                foreach (var item in PrepVMList)
                {
                    if (item.SerialsLlist.Any() || item.RMASerialsLlist.Any())
                    {
                        foreach (var SerialsLlistitem in item.SerialsLlist)//一般
                        {
                            PrepTableList.Add(new PrepTable { ID = item.ID, SKU = item.SKU, Name = item.Name, Serial = SerialsLlistitem.SerialsNo, QTY = (item.SerialsLlist.Count() + item.RMASerialsLlist.Count()) + "/" + item.QTY, SerialTracking = GetSerialTracking(item.SKU), Full = (item.SerialsLlist.Count() + item.RMASerialsLlist.Count()) == item.QTY, MaxQTY = item.QTY });
                        }

                        foreach (var RMASerialsLlistitem in item.RMASerialsLlist)//RMA
                        {
                            PrepTableList.Add(new PrepTable { ID = item.ID, SKU = item.SKU, Name = item.Name, Serial = RMASerialsLlistitem.SerialsNo, QTY = (item.SerialsLlist.Count() + item.RMASerialsLlist.Count()) + "/" + item.QTY, SerialTracking = GetSerialTracking(item.SKU), Full = (item.SerialsLlist.Count() + item.RMASerialsLlist.Count()) == item.QTY, MaxQTY = item.QTY });
                        }
                    }
                    else
                    {
                        PrepTableList.Add(new PrepTable { ID = item.ID, SKU = item.SKU, Name = item.Name, QTY = (item.SerialsLlist.Count() + item.RMASerialsLlist.Count()) + "/" + item.QTY, SerialTracking = GetSerialTracking(item.SKU), Full = (item.SerialsLlist.Count() + item.RMASerialsLlist.Count()) == item.QTY, MaxQTY = item.QTY });
                    }
                }
            }
            int recordsTotal = PrepTableList.Count();
            var returnObj =
           new
           {
               recordsFiltered = recordsTotal,
               data = PrepTableList//分頁後的資料 
           };
            return Json(returnObj, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ReceiveVMList(string SID)
        {
            var ReceiveVMList = (List<TransferItemVM>)Session["ReceiveVMList" + SID];
            var PrepTableList = new List<PrepTable>();
            if (ReceiveVMList != null)
            {
                foreach (var item in ReceiveVMList)
                {
                    if (item.SerialsLlist.Any() || item.RMASerialsLlist.Any())
                    {
                        foreach (var SerialsLlistitem in item.SerialsLlist)
                        {
                            PrepTableList.Add(new PrepTable { ID = item.ID, SKU = item.SKU, Name = item.Name, Serial = SerialsLlistitem.SerialsNo, QTY = item.SerialsLlist.Count() + "/" + item.QTY, SerialTracking = GetSerialTracking(item.SKU), count = (item.QTY - item.SerialsLlist.Count()), Full = (item.SerialsLlist.Count() + item.RMASerialsLlist.Count()) == item.QTY, MaxQTY = item.QTY });
                        }
                        foreach (var RMASerialsLlistitem in item.RMASerialsLlist)
                        {
                            PrepTableList.Add(new PrepTable { ID = item.ID, SKU = item.SKU, Name = item.Name, Serial = RMASerialsLlistitem.SerialsNo, QTY = item.RMASerialsLlist.Count() + "/" + item.QTY, SerialTracking = GetSerialTracking(item.SKU), count = (item.QTY - item.RMASerialsLlist.Count()), Full = (item.SerialsLlist.Count() + item.RMASerialsLlist.Count()) == item.QTY, MaxQTY = item.QTY });
                        }
                    }
                    else
                    {
                        PrepTableList.Add(new PrepTable { ID = item.ID, SKU = item.SKU, Name = item.Name, QTY = item.SerialsLlist.Count() + "/" + item.QTY, SerialTracking = GetSerialTracking(item.SKU), count = (item.QTY - item.SerialsLlist.Count()), Full = (item.SerialsLlist.Count() + item.RMASerialsLlist.Count()) == item.QTY, MaxQTY = item.QTY });
                    }
                }
            }
            int recordsTotal = PrepTableList.Count();
            var returnObj =
           new
           {
               recordsFiltered = recordsTotal,
               data = PrepTableList//分頁後的資料 
           };
            return Json(returnObj, JsonRequestBehavior.AllowGet);
        }
        private bool GetSerialTracking(string SKU)
        {
            return db.SKU.Find(SKU).SerialTracking;
        }
        public string GetNameSize(SkuLang x)
        {
            string name = "";
            name = x.Name + "<br/>";
            var Size = "";
            var iSize = "";
            var inch = 0.0393700787;
            var lbs = 0.00220462262;
            if (x.GetSku.Logistic != null)
            {

                name += "Shipping Weight:" + x.GetSku.Logistic.ShippingWeight + "g/" + (x.GetSku.Logistic.ShippingWeight * lbs).ToString("f2") + "lbs<br/>"; ;
                Size += x.GetSku.Logistic.ShippingLength + "x";
                iSize += (x.GetSku.Logistic.ShippingLength * inch).ToString("f2");
                Size += x.GetSku.Logistic.ShippingWidth + "x";
                iSize += (x.GetSku.Logistic.ShippingWidth * inch).ToString("f2");
                Size += x.GetSku.Logistic.ShippingHeight;
                iSize += (x.GetSku.Logistic.ShippingHeight * inch).ToString("f2");
                name += "Shipping Dimension:" + Size + "mm/" + iSize + "inch";
            }
            return name;
        }
    }
}