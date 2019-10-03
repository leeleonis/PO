using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using inventorySKU.NetoDeveloper;
using Newtonsoft.Json;
using OfficeOpenXml;
using PurchaseOrderSys.Models;
using PurchaseOrderSys.PurchaseOrderService;
using PurchaseOrderSys.SCService;
using SellerCloud_WebService;
using PurchaseOrderSys.NewApi;
using System.Threading.Tasks;

namespace PurchaseOrderSys.Controllers
{
    /// <summary>
    /// 每一筆資料Entity
    /// </summary>
    public class MyRecord
    {
        public int sysid { get; set; }
        public string MyTitle { get; set; }
        public int MyMoney { get; set; }

    }
    public class ShipResult
    {
        private bool status;
        private string message;

        public bool Status { get { return status; } }
        public string Message { get { return message; } }

        public ShipResult(bool status = false, string message = "")
        {
            this.status = status;
            this.message = message;
        }
    }
    public class TestController : BaseController
    {
        protected PurchaseOrderEntities db = new PurchaseOrderEntities();

        public TestController()
        {



        }
        /// <summary>
        /// DataSource 資料集合，通常為DB裡的資料
        /// </summary>
        private List<MyRecord> _myRecords = new List<MyRecord>();

        // GET: Test
        public ActionResult Index()
        {
            //var SerialsLlist = db.SerialsLlist.Where(x => x.PurchaseSKUID.HasValue && x.TransferSKUID.HasValue && x.TransferSKU.SkuNo != x.PurchaseSKU.SkuNo).ToList();

            //foreach (var item in SerialsLlist)
            //{
            //    item.TransferSKU.SkuNo = item.PurchaseSKU.SkuNo;
            //    item.TransferSKU.Name = item.PurchaseSKU.Name;
            //    db.Entry(item.TransferSKU).State = System.Data.Entity.EntityState.Modified;
            //    db.SaveChanges();
            //}
            //var transfer = db.Transfer.Find(3279);
            //var fileStream = Base64ToMemoryStream(transfer.WinitTransfer.PackingList);


            //var PostPrintV2Data = new NewApi.PostPrintV2Data
            //{
            //    labelType = "LZ7050",
            //    madeIn = "China",
            //    singleItems = new List<NewApi.SingleItem>
            //    {
            //         new NewApi.SingleItem
            //         {
            //              specification="Samsung Galaxy S10 Plus (SM-G975F/DS) 128GB (Prism Black)",
            //              productCode="106005547-US",
            //              printQty=5,
            //         },
            //    }
            //};
            //NewApi.ReturnPrintV2 PrintV2 = new NewApi.Winit_API().GetPrintV2(PostPrintV2Data);

            //var WinitTransferSKU = db.WinitTransferSKU.Find(20);
            //WinitTransferSKU.ItemBarcodeFile = PrintV2.itemBarcodeFile;
            //WinitTransferSKU.itemBarcodeList = string.Join(";", PrintV2.itemBarcodeList);
            //db.SaveChanges();
            var Winit_API = new NewApi.Winit_API();
            var ItemMt = Winit_API.queryItemMtEntitys("106018123", "US");
            var getWinitProducts = Winit_API.getWinitProducts("106018123-US");
            //var ApiSetting = db.ApiSetting.Find(16);
            //var FedEx_API = new FedExApi.FedEx_API(ApiSetting);
            //var Transfer = db.Transfer.Find(3264);
            //var Tracking = FedEx_API.Tracking(Transfer.Tracking);
            //var QueryOrderTracking = Winit_API.QueryOrderTracking(Transfer.WinitTransfer.WinitOrderNo);
            //var fileStream = new System.IO.MemoryStream(InvoiceExcel(db.Transfer.Find(3264)));
            //var fileStream = Box_PackingListExcel(db.Transfer.Find(3264));

            //   var Tracking = FedEx_API.Tracking("788504512725");
            //var jj=   JsonConvert.SerializeObject(Tracking);
            //ViewBag.WCPScript = Neodynamic.SDK.Web.WebClientPrint.CreateScript(Url.Action("ProcessRequest", "WebClientPrintAPI", null, HttpContext.Request.Url.Scheme), Url.Action("PrintMyFiles", "Test", null, HttpContext.Request.Url.Scheme), HttpContext.Session.SessionID);
            //var WinitProducts = Winit_API.getWinitProducts("OW0103");
            //var winitProductCode = WinitProducts.FirstOrDefault().productCode;
            //var WarehouseList = Winit_API.getWarehouseList(winitProductCode, "INSJ", "DW", null);
            //var warehouseCode = WarehouseList.warehouseList[13].warehouseCode;
            //var list = new List<NewApi.AvailableMerchandise>();
            //foreach (var item in WinitProducts)
            //{
            //    foreach (var Warehouse in WarehouseList.warehouseList)
            //    {

            //        list.AddRange( Winit_API.getAvailableMerchandise("NS", item.productCode, Warehouse.warehouseCode));
            //    }
            //}

            //var Dlist = list.Distinct().ToList();

            //var WinitProducts = Winit_API.getWinitProducts("OW0103");
            //var LogisticsPlanList = new List<NewApi.LogisticsPlan>();
            //foreach (var WinitProductsitem in WinitProducts)
            //{
            //    var WarehouseList = Winit_API.getWarehouseList(WinitProductsitem.productCode, "INSJ", "DW", null);
            //    foreach (var warehouseitem in WarehouseList.warehouseList)
            //    {
            //      var Plan=  Winit_API.getLogisticsPlan(WinitProductsitem.productCode, warehouseitem.warehouseCode, warehouseitem.warehouseCode);
            //        if (Plan.planList.Any())
            //        {
            //            LogisticsPlanList.Add(Plan);
            //        }
            //    }
            //}

            //var  Warehouse3PList = new NewApi.Winit_APIToken().Warehouse3P();
            //var SKUList = new NewApi.Winit_API().SKUList();

            //DropShip();
            //var SerialsLlist = new List<SerialsLlist>();
            //var POSerialsLlist = new List<SerialsLlist>();
            //var DSerialsLlist = new List<SerialsLlist>();
            //foreach (var PurchaseOrderitem in db.PurchaseOrder.Where(x => x.IsEnable))
            //{
            //    foreach (var PurchaseSKUitem in PurchaseOrderitem.PurchaseSKU.Where(x => x.IsEnable && x.SkuNo == "106005422"))
            //    {
            //        SerialsLlist.AddRange(PurchaseSKUitem.SerialsLlist);
            //        POSerialsLlist.AddRange(PurchaseSKUitem.SerialsLlist.Where(x => x.SerialsType == "PO"));
            //    }
            //}
            //DSerialsLlist.AddRange(SerialsLlist.Except(POSerialsLlist));
            //DSerialsLlist.AddRange(POSerialsLlist.Except(SerialsLlist));
            //if (DSerialsLlist.Any())
            //{
            //}
            //return File(fileStream, "application/zip", "Box_PackingList.xlsx");
            // return File(fileStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ExportRanger.xlsx");
            return View();
        }
        public ActionResult NewWinitBarcode(int WinitTransferID)
        {
            var Winit_API = new Winit_API();
            string[] skuList = new string[] { "106005549-US", "106005547-US", "106005548-US", "106006218-US", "106018122-US", "106018123-US", "106003214-US", "106003274-US", "106018155-US", "106018124-AU" };//winit要加上規格才能出貨
            var WinitTransferSKU = db.WinitTransferSKU.AsEnumerable().Where(x => x.WinitTransferID == WinitTransferID && string.IsNullOrWhiteSpace(x.ItemBarcodeFile));
            foreach (var WinitTransferSKUitem in WinitTransferSKU)
            {
                foreach (var TransferSKUitem in WinitTransferSKUitem.WinitTransfer.Transfer.TransferSKU.Where(x => x.IsEnable && WinitTransferSKUitem.SkuNo.Contains(x.SkuNo)))
                {
                    var productCode = WinitTransferSKUitem.SkuNo;
                    var WSKUList = Winit_API.SKUList(productCode);
                    if (WSKUList != null && WSKUList.list != null && WSKUList.list.Any())
                    {
                        WinitTransferSKUitem.winitProductCode = WSKUList.list.FirstOrDefault().code;
                    }
                    //Winit API 取S BARCODE
                    var PostPrintV2Data = new PostPrintV2Data
                    {
                        labelType = WinitTransferSKUitem.WinitTransfer.SBarcodeLabelType,
                        madeIn = "China",
                        singleItems = new List<SingleItem>()
                    };
                    if (skuList.Contains(productCode))
                    {
                        var specification = db.SKU.Where(x => productCode.Contains(x.SkuID)).FirstOrDefault().SkuLang.FirstOrDefault(x => x.LangID == "en-US").Name;
                        PostPrintV2Data.singleItems.Add(new SingleItem
                        {
                            productCode = productCode,
                            specification = specification,
                            printQty = TransferSKUitem.QTY,
                        });
                    }
                    else
                    {
                        PostPrintV2Data.singleItems.Add(new SingleItem
                        {
                            productCode = productCode,
                            printQty = TransferSKUitem.QTY,
                        });
                    }

                    var PrintV2 = Winit_API.GetPrintV2(PostPrintV2Data);

                    WinitTransferSKUitem.ItemBarcodeFile = PrintV2.itemBarcodeFile;
                    WinitTransferSKUitem.itemBarcodeList = string.Join(";", PrintV2.itemBarcodeList);
                }
            }
            db.SaveChanges();
            return Json(new { status = true, reload = true }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult WinitByexcel(HttpPostedFileBase ExcelFile)
        {
            using (var package = new OfficeOpenXml.ExcelPackage(ExcelFile.InputStream))
            {

                OfficeOpenXml.ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                //for (int row = 1; worksheet.Cells[row, 2].Value != null; row++)
                //{
                //    var serial = worksheet.Cells[row, 2].Value?.ToString().Trim();
                //    var SerialsLlist = db.SerialsLlist.Where(x => x.SerialsNo == serial && x.SerialsType == "Order");
                //    db.SerialsLlist.RemoveRange(SerialsLlist);
                //}
                //db.SaveChanges();

                //var SerialsType = new List<string> { "PO", "TransferIn" };
                var dt = DateTime.Now;
                var CreateBy = "WinitExcel";
                //開PO
                var nPurchaseOrder = new Models.PurchaseOrder
                {
                    IsEnable = true,
                    WarehouseID = 1,
                    CompanyID = 163,
                    VendorID = 29,
                    POStatus = "Completed",
                    POType = "PurchaseOrder",
                    PODate = dt,
                    ReceiveStatus = "Completed",
                    PaymentStatus = "Completed",
                    Currency = "TWD",
                    Tax = 5,
                    CreateBy = CreateBy,
                    CreateAt = dt,
                    Memo = "匯入S碼專用"
                };
                db.PurchaseOrder.Add(nPurchaseOrder);
                //開移倉
                var nTransfer = new Transfer
                {
                    IsEnable = true,
                    Title = "匯入S碼",
                    FromWID = 1,
                    ToWID = 27,
                    Status = "Completed",
                    CreateBy = CreateBy,
                    CreateAt = dt,

                };
                db.Transfer.Add(nTransfer);
                var nWinitTransfer = new WinitTransfer
                {
                    IsEnable = true,
                    SBarcodeLabelType = "LZ7050",
                    BoxLabelSize = "10x10",
                    CreateBy = CreateBy,
                    CreateAt = dt,
                };
                var nWinitTransferBox = new WinitTransferBox
                {
                    IsEnable = true,
                    CreateBy = CreateBy,
                    CreateAt = dt,
                };
                nWinitTransfer.WinitTransferBox.Add(nWinitTransferBox);
                nTransfer.WinitTransfer = nWinitTransfer;

                var irow = 0;
                for (int row = 1; worksheet.Cells[row, 1].Value != null; row++)
                {
                    irow = row;
                    var Sbarcode = worksheet.Cells[row, 1].Value?.ToString().Trim();
                    var serial = worksheet.Cells[row, 2].Value?.ToString().Trim();
                    if (!string.IsNullOrWhiteSpace(serial))
                    {
                        var skuno = worksheet.Cells[row, 3].Value?.ToString().Trim();
                        var PurchaseSKU = nPurchaseOrder.PurchaseSKU.Where(x => x.SkuNo == skuno).FirstOrDefault();
                        var SKU = db.SKU.Find(skuno);
                        var SKUNAME = SKU.SkuLang.Where(x => x.LangID == "en-US").FirstOrDefault().Name;
                        //PO SKU
                        if (PurchaseSKU == null)
                        {
                            PurchaseSKU = new PurchaseSKU
                            {
                                IsEnable = true,
                                Name = SKUNAME,
                                SkuNo = SKU.SkuID,
                                QTYOrdered = 1,
                                Price = SKU.Logistic.Price,
                                Discount = 0,
                                VendorSKU = SKU.SkuID,
                                CreateBy = CreateBy,
                                CreateAt = dt
                            };
                            nPurchaseOrder.PurchaseSKU.Add(PurchaseSKU);
                        }
                        //加入序號
                        var SerialsLlist = db.SerialsLlist.Where(x => x.SerialsNo == serial && x.SerialsType == "PO").FirstOrDefault();// && !x.SerialsLlistC.Any(y => y.IsEnable)
                        if (SerialsLlist == null)
                        {
                            SerialsLlist = new SerialsLlist
                            {
                                IsEnable = true,
                                SerialsNo = serial,
                                SerialsQTY = 1,
                                SerialsType = "PO",
                                CreateBy = CreateBy,
                                CreateAt = dt
                            };
                            PurchaseSKU.SerialsLlist.Add(SerialsLlist);
                        }
                        if (!db.WinitTransferBoxItem.Where(x => x.BarCode == Sbarcode).Any())
                        {
                            //加入S碼
                            var nWinitTransferBoxItem = new WinitTransferBoxItem
                            {
                                BarCode = Sbarcode,
                                Name = SKUNAME,
                                SkuNo = SKU.SkuID,
                                SerialsNo = serial,
                                CreateBy = CreateBy,
                                CreateAt = dt
                            };
                            nWinitTransferBox.WinitTransferBoxItem.Add(nWinitTransferBoxItem);
                            SerialsLlist.WinitTransferBoxItem = nWinitTransferBoxItem;
                        }
                    }
                }

                foreach (var item in nPurchaseOrder.PurchaseSKU.ToList())
                {
                    var Qty = item.SerialsLlist.Count();
                    if (Qty > 0)
                    {
                        item.QTYOrdered = Qty;
                    }
                    else
                    {
                        nPurchaseOrder.PurchaseSKU.Remove(item);
                    }
                }
                db.SaveChanges();
            }
            return Json(new { status = true, reload = true }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult winitProductPdf(int id)
        {
            var WinitTransferSKU = db.WinitTransferSKU.Find(id);
            var BarcodeFile = Base64ToMemoryStream(WinitTransferSKU.ItemBarcodeFile);
            return File(BarcodeFile, "application/pdf", WinitTransferSKU.winitProductCode + "_M碼.pdf");
        }
        public ActionResult WinitExcelD(string WinitOrderNo)
        {
            var WinitTransfer = db.WinitTransfer.Where(x => x.IsEnable && x.WinitOrderNo == WinitOrderNo);
            //在記憶體中建立一個Excel物件
            ExcelPackage ep = new ExcelPackage();
            //加入一個Sheet
            ep.Workbook.Worksheets.Add("MySheet");
            //取得剛剛加入的Sheet(實體Sheet就叫MySheet)
            ExcelWorksheet sheet1 = ep.Workbook.Worksheets["MySheet"];//取得Sheet1 
            sheet1.Cells[1, 1].Value = "SKU";//
            sheet1.Cells[1, 2].Value = "M碼";//
            sheet1.Cells[1, 3].Value = "序號";//
            sheet1.Cells[1, 4].Value = "S碼";//
            //迴圈部份自由料理
            int row = 2;
            foreach (var WinitTransferitem in WinitTransfer)
            {
                foreach (var WinitTransferSKUitem in WinitTransferitem.WinitTransferSKU.Where(x => x.IsEnable))
                {
                    sheet1.Cells[row, 1].Value = WinitTransferSKUitem.SkuNo;
                    sheet1.Cells[row, 2].Value = WinitTransferSKUitem.winitProductCode;
                    foreach (var WinitTransferBoxitem in WinitTransferitem.WinitTransferBox.Where(x => x.IsEnable))
                    {
                        foreach (var item in WinitTransferBoxitem.WinitTransferBoxItem.Where(x => x.MerchandiseCode == WinitTransferSKUitem.SkuNo))
                        {
                            sheet1.Cells[row, 3].Value = item.SerialsNo;
                            sheet1.Cells[row, 4].Value = item.BarCode;
                            row++;
                        }
                    }
                }
            }
            //建立檔案串流
            //var OutputStream = new System.IO.MemoryStream();
            //把剛剛的Excel物件真實存進檔案裡
            //ep.SaveAs(OutputStream);
            //關閉串流
            //var fileStream = OutputStream.ToArray();
            //OutputStream.Close();
            return File(ep.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", WinitOrderNo + "WinitExce.xlsx");
        }
        public ActionResult InvoiceExcelD(int id)
        {
            var transfer = db.Transfer.Find(id);
            var fileStream = Base64ToMemoryStream(transfer.WinitTransfer.InvoiceExcel);
            return File(fileStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "InvoiceExcel.xlsx");
        }
        public ActionResult CheckListlD(int id)
        {
            var transfer = db.Transfer.Find(id);
            var fileStream = Base64ToMemoryStream(transfer.WinitTransfer.CheckList);
            return File(fileStream, "application/zip", "CheckList.zip");
        }
        public ActionResult PackingListD(int id)
        {
            var transfer = db.Transfer.Find(id);
            var fileStream = Base64ToMemoryStream(transfer.WinitTransfer.PackingList);
            return File(fileStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "PackingList.xlsx");
        }
        public ActionResult FedexPDFID(int id)
        {
            var WinitTransfer = db.WinitTransfer.Find(id);
            var file = new Ionic.Zip.ZipFile();
            var OutputStream = new MemoryStream();
            foreach (var item in WinitTransfer.WinitTransferBox)
            {
                file.AddEntry("FedexPDF-" + item.ID + ".pdf", Base64ToMemoryStream(item.FedExPdf));
            }
            file.Save(OutputStream);
            return File(OutputStream.ToArray(), "application/zip", "FedexPDF.zip");
        }
        [AllowAnonymous]
        public void PrintMyFiles()
        {
            HttpApplicationStateBase app = HttpContext.Application;
            //Create a ClientPrintJob 
            //set client printer, for multiple files use DefaultPrinter...

            var cpj = new Neodynamic.SDK.Web.ClientPrintJob();
            var pdfbyte = System.IO.File.ReadAllBytes(Server.MapPath(@"~/File/S36BW-419072313360.pdf"));
            var file = new Neodynamic.SDK.Web.PrintFilePDF(pdfbyte, "US.pdf");


            //var file = new Neodynamic.SDK.Web.PrintFile(EXCELSET(), "Invoice.xlsx");
            //cpj.PrintFileGroup.Add(file);
            //cpj.ClientPrinter = new Neodynamic.SDK.Web.DefaultPrinter();//預設印表機
            ////printerName = "";
            //if (string.IsNullOrWhiteSpace(printerName))
            //{
            cpj.ClientPrinter = new Neodynamic.SDK.Web.UserSelectedPrinter();//自己選印表機
            //    //cpj.ClientPrinter = new Neodynamic.SDK.Web.DefaultPrinter();//預設印表機
            //}
            //else
            //{
            //    if (key == "USPDF")
            //    {
            //    }
            //    else
            //    {
            //        cpj.ClientPrinter = new Neodynamic.SDK.Web.InstalledPrinter(printerName);
            //    }
            //}
            //cpj.ClientPrinter = new UserSelectedPrinter();//自己選印表機
            System.Web.HttpContext.Current.Response.ContentType = "application/octet-stream";
            System.Web.HttpContext.Current.Response.BinaryWrite(cpj.GetContent());
            System.Web.HttpContext.Current.Response.End();

        }
        public void SkuOriginCountry()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://internal.qd.com.tw/Test/GetSkuOriginCountry");
            //using (var streamWriter = new System.IO.StreamWriter(request.GetRequestStream()))
            //{
            //    var json = JsonConvert.SerializeObject(data);
            //    streamWriter.Write(json);
            //    streamWriter.Flush();
            //}
            HttpWebResponse httpResponse = (HttpWebResponse)request.GetResponse();
            using (var streamReader = new System.IO.StreamReader(httpResponse.GetResponseStream()))
            {
                var response = JsonConvert.DeserializeObject<dynamic>(streamReader.ReadToEnd());
                foreach (var item in response)
                {
                    var Name = ((Newtonsoft.Json.Linq.JProperty)item).Name;
                    var Value = ((Newtonsoft.Json.Linq.JProperty)item).Value;
                    foreach (var itemValue in Value.ToObject<List<string>>())
                    {
                        var LogisticList = db.Logistic.Where(x => x.Sku == itemValue);
                        foreach (var Logistic in LogisticList)
                        {
                            Logistic.OriginCountry = Name;
                        }
                        db.SaveChanges();
                    }
                }
            }

        }
        public ActionResult TestShipmentByOrder()
        {
            using (WebClient webClient = new WebClient())
            {
                // 指定 WebClient 編碼
                webClient.Encoding = Encoding.UTF8;
                // 指定 WebClient 的 Content-Type header
                webClient.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                // 準備寫入的 data
                var ShipmentOrderlist = new List<ShipmentOrder>();
                ShipmentOrderlist.Add(new ShipmentOrder { OrderID = 123, SkuNo = "100001001", SerialsNo = "A000000001", QTY = 1 });
                // 將 data 轉為 json
                string json = JsonConvert.SerializeObject(ShipmentOrderlist);
                // 執行 post 動作
                var result = webClient.UploadString("http://localhost:59290/Ajax/ShipmentByOrder", json);
                // linqpad 將 post 結果輸出
                return Json(new { status = true, result }, JsonRequestBehavior.AllowGet);
            }



            //using (WebClient wc = new WebClient())
            //{
            //    try
            //    {
            //        wc.Encoding = Encoding.UTF8;

            //        NameValueCollection nc = new NameValueCollection();
            //        nc["OrderID"] = SKU;
            //        nc["SkuNo"] = SCID;
            //        nc["QTY"] = SCID;
            //        byte[] bResult = wc.UploadValues(ApiUrl + "Api/GetSkuInventoryQTY", nc);
            //        string resultXML = Encoding.UTF8.GetString(bResult);

            //    }
            //    catch (WebException ex)
            //    {
            //        //throw new Exception("無法連接遠端伺服器");
            //    }
            //}

        }
        public ActionResult GetData_Full(int draw, int start, int length,//→此三個為DataTables自動傳遞參數
                                                                         //↓以下兩個為表單的查詢條件，請依自己工作需求調整  
         string MyTitle, int? MyMoney)
        {

            int skip = start;//起始資料列索引值(略過幾筆)

            #region jQuery DataTables的排序資料行
            //jQuery DataTable的Column index
            string col_index = Request.QueryString["order[0][column]"];
            //col_index 換算成 資料行名稱
            //排序資料行名稱
            string sortColName = string.IsNullOrEmpty(col_index) ? "sysid" : Request.QueryString[$@"columns[{col_index}][data]"];
            //升冪或降冪
            string asc_desc = string.IsNullOrEmpty(Request.QueryString["order[0][dir]"]) ? "desc" : Request.QueryString["order[0][dir]"];//防呆
            #endregion

            //查詢&排序後的總筆數
            int recordsTotal = 0;
            //要回傳的分頁資料
            List<MyRecord> pagedData = new List<MyRecord>();

            //總資料
            var query = this._myRecords.AsEnumerable();
            //查詢
            if (!string.IsNullOrEmpty(MyTitle))
            {
                query = this._myRecords.Where(m => m.MyTitle.Contains(MyTitle));
            }
            if (MyMoney.HasValue)
            {
                query = this._myRecords.Where(m => m.MyMoney == MyMoney);
            }

            //排序
            //query = query.OrderBy($@"{sortColName} {asc_desc}"); //排序使用到System.Linq.Dynamic

            recordsTotal = query.Count();//查詢後的總筆數

            if (length == -1)
            {//抓全部資料
                pagedData = query.ToList();
            }
            else
            {//分頁 
                pagedData = query.Skip(skip).Take(length)
                            .ToList();
            }


            //回傳Json資料
            var returnObj =
              new
              {
                  draw = draw,
                  recordsTotal = recordsTotal,
                  recordsFiltered = recordsTotal,
                  data = pagedData//分頁後的資料 
              };
            return Json(returnObj, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Neto 品號資料同步，慎改!
        /// </summary>
        public void DoSkuSync()
        {
            NetoApi neto = new NetoApi();
            string LangID = EnumData.DataLangList().First().Key;

            var categoryList = neto.GetCategory().Category.ToList();
            var conditionList = db.Condition.Where(c => c.IsEnable).ToList();

            SKU sku;
            Models.Brand brand;
            SkuType category;
            Dictionary<string, string> VariationList = new Dictionary<string, string>();
            int page = 0, limit = 100, categoryID;
            var skuList = neto.GetItemBySkus(page, limit);
            try
            {
                do
                {
                    Response.Write(string.Format("Sync Sku from {0} to {1}... <br />", page++ * limit + 1, page * limit));
                    foreach (var item in skuList.Item.Where(i => i.Categories[0] != null))
                    {
                        var eBayTitle = new Dictionary<int, string>()
                        {
                            { 2, item.Misc02 }, { 19, item.Misc19 }, { 50, item.Misc50 }, { 24, item.Misc24 }, { 25, item.Misc25 }, { 23, item.Misc23 }, { 21, item.Misc21 },{ 20, item.Misc20 }, { 22, item.Misc22 }
                        };

                        if (!db.SKU.Any(s => s.SkuID.Equals(item.SKU)))
                        {
                            if (!item.SKU.Contains("_var"))
                            {
                                try
                                {

                                    categoryID = int.Parse(item.Categories.First().Category[0].CategoryID);
                                    if (!db.SkuType.Any(t => t.NetoID.Value.Equals(categoryID)))
                                    {
                                        var parentCategoryID = int.Parse(categoryList.First(c => c.CategoryID.Equals(categoryID.ToString())).ParentCategoryID);
                                        if (!parentCategoryID.Equals(0))
                                        {
                                            categoryID = parentCategoryID;
                                        }
                                    }

                                    category = db.SkuType.FirstOrDefault(t => t.NetoID.Value.Equals(categoryID));
                                    if (category == null)
                                    {
                                        category = new SkuType()
                                        {
                                            IsEnable = true,
                                            NetoID = categoryID,
                                            CreateAt = DateTime.UtcNow,
                                            CreateBy = "System Scheduling"
                                        };

                                        category.SkuTypeLang.Add(new SkuTypeLang()
                                        {
                                            Name = categoryList.First(c => c.CategoryID.Equals(categoryID.ToString())).CategoryName,
                                            LangID = LangID,
                                            CreateAt = category.CreateAt,
                                            CreateBy = category.CreateBy
                                        });

                                        db.SkuType.Add(category);
                                        db.SaveChanges();
                                    }

                                    string brandName = string.IsNullOrEmpty(item.Brand) ? "Other" : item.Brand.Trim(' ');
                                    brand = db.Brand.FirstOrDefault(b => b.Name.ToLower().Equals(brandName.ToLower()));
                                    if (brand == null)
                                    {
                                        brand = new Models.Brand()
                                        {
                                            IsEnable = true,
                                            Name = brandName,
                                            CreateAt = DateTime.UtcNow,
                                            CreateBy = "System Scheduling"
                                        };

                                        db.Brand.Add(brand);
                                        db.SaveChanges();
                                    }

                                    using (StockKeepingUnit SKU = new StockKeepingUnit())
                                    {
                                        sku = new SKU()
                                        {
                                            IsEnable = true,
                                            SkuID = item.SKU,
                                            Company = 163,
                                            Category = category.ID,
                                            Brand = brand.ID,
                                            Condition = CheckCondition(conditionList, item.SKU),
                                            UPC = item.UPC,
                                            Type = (byte)EnumData.SkuType.Single,
                                            eBayTitle = JsonConvert.SerializeObject(eBayTitle),
                                            Status = Convert.ToByte(item.IsActive),
                                            CreateAt = DateTime.UtcNow,
                                            CreateBy = "System Scheduling"
                                        };

                                        if (sku.SkuID.Contains('_')) { sku.ParentShadow = sku.SkuID.Split('_')[0]; }

                                        var langData = new SkuLang()
                                        {
                                            Sku = sku.SkuID,
                                            LangID = LangID,
                                            Name = item.Name,
                                            Model = item.ModelNumber,
                                            Description = item.Description,
                                            PackageContent = item.Misc01,
                                            SpecContent = item.Specifications,
                                            FeatureContent = item.Features,
                                            CreateAt = sku.CreateAt,
                                            CreateBy = sku.CreateBy
                                        };

                                        sku = SKU.CreateSku(sku, langData);
                                    }
                                }
                                catch (Exception e)
                                {
                                    Response.Write(string.Format("Error:【{0}】{1} <br />", item.SKU, e.InnerException?.Message ?? e.Message));
                                }
                            }
                            else
                            {
                                VariationList.Add(item.SKU.Split('_')[0], item.ParentSKU);
                            }
                        }
                    }
                    db.SaveChanges();

                    skuList = page < 5 ? neto.GetItemBySkus(page, limit) : new NetoDeveloper.GetItemResponse() { Ack = NetoDeveloper.GetItemResponseAck.Error };
                } while (skuList.Ack == NetoDeveloper.GetItemResponseAck.Success && skuList.Item.Any());

                if (VariationList.Any())
                {
                    var ParentGroup = VariationList.GroupBy(v => v.Value, v => v.Key).ToList();
                    foreach (var SkuGroup in ParentGroup)
                    {
                        var parent = db.SKU.Find(SkuGroup.Key);
                        parent.Type = (byte)EnumData.SkuType.Variation;

                        foreach (var child in db.SKU.Where(s => SkuGroup.Contains(s.SkuID)))
                        {
                            child.ParentSku = SkuGroup.Key;
                        }
                    }
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                string msg = e.Message;
                Response.Write(msg);
            }
        }

        private int CheckCondition(List<Condition> conditionList, string sku)
        {
            foreach (var condition in conditionList.Where(c => !string.IsNullOrEmpty(c.Suffix)))
            {
                if (sku.Contains(condition.Suffix)) return condition.ID;
            }

            return 1;
        }

        public void SkuUpdate(string Sku)
        {
            StockKeepingUnit SKU = new StockKeepingUnit(Sku);
            SKU.UpdateSkuToNeto();
            var netoApi = new NetoApi();
            var skuData = netoApi.GetItemBySku(Sku);
        }

        public void GetOrderData(int OrderID)
        {
            using (var SC_Api = new SC_WebService(ApiUserName, ApiPassword))
            {
                var Order = db.Orders.First(o => o.ID.Equals(OrderID) || o.SCID.Value.Equals(OrderID));
                var OrderItemData1 = SC_Api.Get_OrderData(Order.SCID.Value).Order.Items.ToList();
                var OrderItemData2 = SC_Api.Get_OrderInfo(Order.SCID.Value).Items.ToList(); ;
            }
        }

        private ShipResult DropShip()
        {
            //Orders order = new Orders();
            try
            {
                var ProductID = "TestPOSKU";
                SCWS = new SC_WebService(ApiUserName, ApiPassword);

                var purchaseOrder = SCWS.Get_PurchaseOrder(22901);
                foreach (var item in purchaseOrder.Products)
                {
                    var Serial_All = SCWS.PurchaseItemReceiveSerial_All_New(item.ProductID, purchaseOrder.ID);
                }

                var Company = SCWS.Get_AllCompany();
                foreach (var item in purchaseOrder.Products)
                {
                    SCWS.PurchaseItem_DeleteSerials(item.ProductID, purchaseOrder.ID, item.DefaultWarehouseID, "SerialTest02");//SC上的序號移除
                }
                var CompanyID = Company.FirstOrDefault().ID;
                //int CompanyID = order.CompanyID.Value; // SCWS.Get_CurrentCompanyID();
                POVendor VendorData = SCWS.Get_Vendor_All(CompanyID).FirstOrDefault(v => v.DisplayName.Equals("TWN"));
                var SyncOn = SCWS.SyncOn.ToString("yyyyMMddHHmmss");
                var WarehouseID = 120;
                Purchase newPurchase = SCWS.Create_PurchaseOrder(new Purchase()
                {
                    ID = 0,
                    CompanyID = CompanyID,
                    Priority = PriorityCodeType.Normal,
                    Status = PurchaseOrderService.PurchaseStatus.Ordered,
                    PurchaseTitle = "Test" + SyncOn,
                    VendorID = VendorData != null ? VendorData.ID : 0,
                    VendorInvoiceNumber = "",
                    // Memo = !string.IsNullOrEmpty(package.SupplierComment) ? package.SupplierComment : "",
                    DefaultWarehouseID = WarehouseID,
                    CreatedBy = SCWS.UserID,
                    CreatedOn = SCWS.SyncOn
                });

                PurchaseItem newPurchaseItem = SCWS.Create_PurchaseOrder_Item(new PurchaseItem()
                {
                    PurchaseID = newPurchase.ID,
                    ProductID = ProductID,//SKU
                    ProductName = "TestPOSKUName",
                    UPC = "UPC",
                    QtyOrdered = 1,
                    QtyReceived = 0,
                    QtyReceivedToDate = 0,
                    DefaultWarehouseID = WarehouseID
                });
                purchaseOrder = SCWS.Get_PurchaseOrder(newPurchase.ID);
                var receiveRequest = new PurchaseItemReceiveRequest() { PurchaseID = purchaseOrder.ID };
                var receiveRequestProduct = new List<PurchaseItemReceiveRequestProduct>();
                //List<PurchaseItemReceive> PurchaseItemList = PurchaseItems.GetAll(true).Where(p => p.WarehouseName.Equals(WarehouseData.Name)).ToList();
                //string[] SerialList = PurchaseItemList.Select(p => p.SerialNumber).Distinct().ToArray();
                foreach (var purchaseItem in purchaseOrder.Products)
                {
                    int qty = 1;
                    receiveRequestProduct.Add(new PurchaseItemReceiveRequestProduct()
                    {
                        QtyReceived = qty,
                        WarehouseID = purchaseItem.DefaultWarehouseID,
                        PurchaseItemID = purchaseItem.ID,
                        PurchaseID = purchaseItem.PurchaseID
                    });
                }
                PurchaseItemReceive[] purchaseItemReceive = null;
                var receiveSerial = new List<PurchaseOrderService.PurchaseItemReceiveSerial>();
                if (receiveRequestProduct.Any())
                {
                    receiveRequest.Products = receiveRequestProduct.ToArray();
                    purchaseItemReceive = SCWS.Create_PurchaseOrder_ItemReceive(receiveRequest);

                    receiveSerial = new List<PurchaseOrderService.PurchaseItemReceiveSerial>();
                    receiveSerial.Add(new PurchaseOrderService.PurchaseItemReceiveSerial()
                    {
                        PurchaseID = newPurchase.ID,
                        WarehouseID = WarehouseID,
                        ProductID = ProductID,
                        PurchaseReceiveID = purchaseItemReceive.FirstOrDefault().Id,
                        SerialNumber = "TestSerial01"
                    });
                    //foreach (SerialNumbers SerialNumber in SerialNumberList)
                    //{
                    //    PurchaseOrderService.PurchaseItemReceive itemReceive = purchaseItemReceive.First(i => i.ProductID.Equals(SerialNumber.ProductID));


                    //}

                    SCWS.Update_PurchaseOrder_ItemReceive_Serials(receiveSerial.ToArray());
                }


                //foreach (Items item in package.Items.Where(i => i.IsEnable.Equals(true)).ToList())
                //{
                //    PurchaseItem newPurchaseItem = SCWS.Create_PurchaseOrder_Item(new PurchaseItem()
                //    {
                //        PurchaseID = newPurchase.ID,
                //        ProductID = item.ProductID,//SKU
                //        ProductName = item.Skus.ProductName,
                //        UPC = item.Skus.UPC,
                //        QtyOrdered = item.Qty.Value,
                //        QtyReceived = 0,
                //        QtyReceivedToDate = 0,
                //        DefaultWarehouseID = warehouse.ID
                //    });
                //}

                //package.POId = newPurchase.ID;
                //MyHelp.Log("PurchaseOrder", package.OrderID, string.Format("開啟 Purchase Order【{0}】成功", package.POId));
            }
            catch (Exception e)
            {
                return new ShipResult(false, e.Message);
            }

            return new ShipResult(true);
        }

        public void Task_Test()
        {
            JobProcess job = new JobProcess("Task Test");
            job.Task = Task.Run(() =>
            {
                job.StartWork();

                try
                {
                    job.AddLog("Task Test");
                }
                catch (Exception ex)
                {
                    return ex.InnerException?.Message ?? ex.Message;
                }

                return "";
            });
        }

        public ActionResult Autoreturntoshipper()
        {
            var IsSellable = db.Warehouse.Where(x => x.IsEnable && x.IsSellable).Select(x => x.ID).ToList();
            var RMASerialsLlist = db.RMASerialsLlist.Where(x => x.IsEnable && x.SerialsType == "RMAIn" && x.WarehouseID.HasValue && IsSellable.Contains(x.WarehouseID.Value) && !x.RMASerialsLlistC.Any(y => y.IsEnable)).ToList();
            var SerialsLlist = new List<RMASerialsLlist>();
            foreach (var Serialitem in RMASerialsLlist)
            {
                //if (Serialitem.Reason == "16")
                if (true)
                {
                    var dt = DateTime.UtcNow;
                    //開移倉單
                    var nTransfer = new Transfer
                    {
                        IsEnable = true,
                        Title = Serialitem.RMASKU.RMA.OrderID + "_return to shipper",
                        FromWID = Serialitem.WarehouseID,
                        ToWID = Serialitem.WarehouseID,
                        Status = "Completed",
                        Interim = 2,
                        CreateBy = "RMAAPI",
                        CreateAt = dt
                    };
                    var nTransferSKU = new TransferSKU
                    {
                        IsEnable = true,
                        QTY = 1,
                        SkuNo = Serialitem.RMASKU.SkuNo,
                        Name = Serialitem.RMASKU.Name,
                        CreateBy = "RMAAPI",
                        CreateAt = dt
                    };


                    var nSerialsLlist = new SerialsLlist
                    {
                        IsEnable = true,
                        SerialsNo = Serialitem.SerialsNo,
                        SerialsQTY = 1,
                        SerialsType = "TransferIn",
                        CreateBy = "RMAAPI",
                        CreateAt = dt,
                        ReceivedBy = "RMAAPI",
                        ReceivedAt = dt,
                    };
                    nTransferSKU.SerialsLlist.Add(nSerialsLlist);
                    var nRMASerialsLlist = new RMASerialsLlist
                    {
                        IsEnable = true,
                        RMASKUID = Serialitem.RMASKUID,
                        RMASerialsLlistP = Serialitem,
                        SerialsNo = Serialitem.SerialsNo,
                        SerialsQTY = -1,
                        SerialsType = "TransferOut",
                        CreateBy = "RMAAPI",
                        CreateAt = dt,
                        ReceivedBy = "RMAAPI",
                        ReceivedAt = dt,
                    };
                    nTransferSKU.RMASerialsLlist.Add(nRMASerialsLlist);

                    nTransfer.TransferSKU.Add(nTransferSKU);
                    db.Transfer.Add(nTransfer);
                    db.SaveChanges();
                }
                else
                {
                    SerialsLlist.Add(Serialitem);
                }
            }
            return Json(SerialsLlist.Select(x => new { x.SerialsNo, x.Warehouse.Name, Reason = EnumData.RMAReasonList().Where(y => y.Key.ToString() == x.Reason).FirstOrDefault().Value }).ToList(), JsonRequestBehavior.AllowGet);
        }
        public ActionResult tBOX(int TransferID, int boxid)
        {
            var dt = DateTime.UtcNow;
            var Transfer = db.Transfer.Find(TransferID);
            foreach (var TransferSKUitem in Transfer.TransferSKU.Where(x => x.IsEnable))
            {
                var MerchandiseCode = TransferSKUitem.SkuNo + "-" + Transfer.WarehouseTo.WinitWarehouse;
                foreach (var Serialsitem in TransferSKUitem.SerialsLlist.Where(x => x.IsEnable))
                {
                    if (Serialsitem.WinitTransferBoxItem == null)
                    {
                        var WinitTransferSKU = Transfer.WinitTransfer.WinitTransferSKU.Where(x => x.SkuNo == MerchandiseCode).FirstOrDefault();
                        var WinitTransferSKUID = WinitTransferSKU.ID;
                        var GBoxList = Transfer.WinitTransfer.WinitTransferBox.Where(x => x.WinitTransferBoxItem.Where(y => y.SkuNo == TransferSKUitem.SkuNo).Any()).GroupBy(x => x.WinitTransferBoxItem.GroupBy(y => y.SkuNo)).ToList();
                        var FilePage = GBoxList.Sum(x => x.Sum(y => y.WinitTransferBoxItem.Where(z => z.SkuNo == TransferSKUitem.SkuNo).Count())) + 1;
                        var itemBarcodeList = WinitTransferSKU.itemBarcodeList.Split(';');
                        var BarCode = itemBarcodeList[FilePage - 1];
                        Serialsitem.WinitTransferBoxItem = new WinitTransferBoxItem
                        {
                            BarCode = BarCode,
                            CreateBy = "QDAdmin",
                            CreateAt = dt,
                            Value = Serialsitem.TransferSKU.SKU.Logistic?.Price,
                            SerialsNo = Serialsitem.SerialsNo,
                            Name = Serialsitem.TransferSKU.Name,
                            Weight = Serialsitem.TransferSKU.SKU.Logistic?.ShippingWeight,
                            FilePage = FilePage.ToString(),
                            SkuNo = Serialsitem.TransferSKU.SkuNo,
                            WinitTransferSKUID = WinitTransferSKUID,
                            WinitTransferBoxID = boxid,
                            MerchandiseCode = MerchandiseCode

                        };
                        db.SaveChanges();
                    }
                }
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DelTransferWinit(int id)
        {
            var Transfer = db.Transfer.Find(id);
            if (Transfer.WinitTransfer != null)
            {
                foreach (var item in Transfer.WinitTransfer?.WinitTransferBox?.ToList())
                {
                    db.WinitTransferBoxItem.RemoveRange(item.WinitTransferBoxItem.ToList());
                    db.WinitTransferBox.Remove(item);
                }
                db.WinitTransferSKU.RemoveRange(Transfer.WinitTransfer.WinitTransferSKU.ToList());
                db.WinitTransfer.Remove(Transfer.WinitTransfer);
            }


            foreach (var TransferSKUitem in Transfer.TransferSKU.ToList())
            {
                db.SerialsLlist.RemoveRange(TransferSKUitem.SerialsLlist.ToList());
                db.TransferSKU.Remove(TransferSKUitem);
            }
            db.Transfer.Remove(Transfer);
            //db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SerialsLlistReset()
        {
            var SerialsLlist = db.SerialsLlist.Where(x => x.PID.HasValue && x.SerialsNo != x.SerialsLlistP.SerialsNo);
            foreach (var item in SerialsLlist.ToList())
            {
                item.PID = null;
                db.SaveChanges();
                //if (item.SerialsLlistP.SerialsNo != item.SerialsNo)
                //{
                //    item.SerialsNo = item.SerialsLlistP.SerialsNo;
                //}

            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}