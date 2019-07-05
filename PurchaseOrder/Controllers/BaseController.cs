using System;
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
            TypeInfoList.FirstOrDefault(info => info.Name.Equals("UpdateBy") || info.Name.Equals("Update_by")).SetValue(originData, Session["AdminName"]);
            TypeInfoList.FirstOrDefault(info => info.Name.Equals("UpdateAt") || info.Name.Equals("Update_at")).SetValue(originData, DateTime.UtcNow);
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
                var chkSerialsLlist = db.SerialsLlist.Where(x => ExcelSerialslist.Contains(x.SerialsNo) && x.PurchaseSKU.IsEnable && x.PurchaseSKU.SkuNo == PurchaseSKU.SkuNo).Select(x => x.SerialsNo).ToList();
                if (chkSerialsLlist.Any())//同SKU不可以重複序號
                {
                    return "序號重複：" + string.Join(",", chkSerialsLlist);
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
        public ActionResult PrepVMList(string SID)
        {
            var PrepVMList = (List<TransferItemVM>)Session["PrepVMList" + SID];
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
        [AllowAnonymous]
        public void PrintMyFiles()
        {
            var b64 = "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IDggMCBSXS9UeXBlL1BhZ2VzL0NvdW50IDIvSVRYVCg0LjIuMCk+PgplbmRvYmoKMTcgMCBvYmoKPDwvTmFtZXNbKEpSX1BBR0VfQU5DSE9SXzBfMSkgMTEgMCBSKEpSX1BBR0VfQU5DSE9SXzBfMikgMTIgMCBSXT4+CmVuZG9iagoxOCAwIG9iago8PC9EZXN0cyAxNyAwIFI+PgplbmRvYmoKMTkgMCBvYmoKPDwvTmFtZXMgMTggMCBSL1R5cGUvQ2F0YWxvZy9QYWdlcyA3IDAgUi9WaWV3ZXJQcmVmZXJlbmNlczw8L1ByaW50U2NhbGluZy9BcHBEZWZhdWx0Pj4+PgplbmRvYmoKMjAgMCBvYmoKPDwvTW9kRGF0ZShEOjIwMTkwNzA0MTYyOTU2KzA4JzAwJykvQ3JlYXRvcihKYXNwZXJSZXBvcnRzIFwoSXRlbUJDXCkpL0NyZWF0aW9uRGF0ZShEOjIwMTkwNzA0MTYyOTU2KzA4JzAwJykvUHJvZHVjZXIoaVRleHQgNC4yLjAgYnkgMVQzWFQpPj4KZW5kb2JqCnhyZWYKMCAyMQowMDAwMDAwMDAwIDY1NTM1IGYgCjAwMDAwMDEzNzEgMDAwMDAgbiAKMDAwMDAwMjQxOSAwMDAwMCBuIAowMDAwMDM4ODA0IDAwMDAwIG4gCjAwMDAwNDAxNjIgMDAwMDAgbiAKMDAwMDAwMDAxNSAwMDAwMCBuIAowMDAwMDAwOTY5IDAwMDAwIG4gCjAwMDAwNDEzNzkgMDAwMDAgbiAKMDAwMDAwMjA2MCAwMDAwMCBuIAowMDAwMDM4OTQxIDAwMDAwIG4gCjAwMDAwMDE2NTcgMDAwMDAgbiAKMDAwMDAwMjM0NyAwMDAwMCBuIAowMDAwMDAyMzgzIDAwMDAwIG4gCjAwMDAwMDI1MDcgMDAwMDAgbiAKMDAwMDAzODA0OSAwMDAwMCBuIAowMDAwMDM4MjM4IDAwMDAwIG4gCjAwMDAwMzg0ODcgMDAwMDAgbiAKMDAwMDA0MTQ0OCAwMDAwMCBuIAowMDAwMDQxNTMxIDAwMDAwIG4gCjAwMDAwNDE1NjUgMDAwMDAgbiAKMDAwMDA0MTY3MCAwMDAwMCBuIAp0cmFpbGVyCjw8L0luZm8gMjAgMCBSL0lEIFs8ZTRmZjgyNmQ4YWRlNGUwMGU2MjE0Y2Y1N2QxNzYwNmM+PDFhYmI3NjA4YTI1YzExZmFlMjI5YzgzYmUyYzhiYmYwPl0vUm9vdCAxOSAwIFIvU2l6ZSAyMT4+CnN0YXJ0eHJlZgo0MTgyNwolJUVPRgo=";
            byte[] pdfbyte = Convert.FromBase64String(b64);
            //Create a ClientPrintJob 
            ClientPrintJob cpj = new ClientPrintJob();
            //set client printer, for multiple files use DefaultPrinter...
            //cpj.ClientPrinter = new DefaultPrinter();

            cpj.ClientPrinter = new UserSelectedPrinter();
            //set files-printers group by using special formatting!!!
            //Invoice.doc PRINT TO Printer1
            cpj.PrintFileGroup.Add(new PrintFile(pdfbyte, "Barcode_PRINT_TO_Printer.pdf"));
            //send it...
            System.Web.HttpContext.Current.Response.ContentType = "application/octet-stream";
            System.Web.HttpContext.Current.Response.BinaryWrite(cpj.GetContent());
            System.Web.HttpContext.Current.Response.End();

        }
    }
}