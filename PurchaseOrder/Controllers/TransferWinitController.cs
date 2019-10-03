using inventorySKU;
using Ionic.Zip;
using iTextSharp.text;
using iTextSharp.text.pdf;
using OfficeOpenXml;
using PurchaseOrderSys.Models;
using PurchaseOrderSys.NewApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PurchaseOrderSys.Controllers
{
    [CheckSession]
    public class TransferWinitController : BaseController
    {
        public string[] skuList = new string[] { "106005549-US", "106005547-US", "106005548-US", "106006218-US", "106018122-US", "106018123-US", "106003214-US", "106003274-US", "106018155-US", "106018124-AU" };//winit要加上規格才能出貨
        // GET: TransferWinit
        public ActionResult Index(TransferSearchVM TransferSearchVM)
        {
            var Transferlist = db.Transfer.Where(x => x.IsEnable && x.TransferType == "Winit");
            if (!string.IsNullOrWhiteSpace(TransferSearchVM.Status))
            {
                Transferlist = Transferlist.Where(x => x.Status == TransferSearchVM.Status);
            }
            if (!string.IsNullOrWhiteSpace(TransferSearchVM.SKU))
            {
                Transferlist = Transferlist.Where(x => x.TransferSKU.Where(y => y.IsEnable && y.SkuNo == TransferSearchVM.SKU).Any());
            }
            if (!string.IsNullOrWhiteSpace(TransferSearchVM.Serial))
            {
                Transferlist = Transferlist.Where(x => x.TransferSKU.Where(y => y.IsEnable && y.SerialsLlist.Where(z => z.SerialsNo == TransferSearchVM.Serial).Any()).Any());
            }
            if (TransferSearchVM.From.HasValue)
            {
                Transferlist = Transferlist.Where(x => x.FromWID == TransferSearchVM.From);
            }
            if (TransferSearchVM.Interim.HasValue)
            {
                Transferlist = Transferlist.Where(x => x.Interim == TransferSearchVM.Interim);
            }
            if (TransferSearchVM.To.HasValue)
            {
                Transferlist = Transferlist.Where(x => x.ToWID == TransferSearchVM.To);
            }
            if (TransferSearchVM.Transfer.HasValue)
            {
                Transferlist = Transferlist.Where(x => x.ID == TransferSearchVM.Transfer);
            }
            if (!string.IsNullOrWhiteSpace(TransferSearchVM.ExternalTransfer))
            {
                Transferlist = Transferlist.Where(x => x.ExternalTra == TransferSearchVM.ExternalTransfer);
            }
            if (!string.IsNullOrWhiteSpace(TransferSearchVM.Title))
            {
                Transferlist = Transferlist.Where(x => x.Title == TransferSearchVM.Title);
            }
            if (!string.IsNullOrWhiteSpace(TransferSearchVM.Carrier))
            {
                Transferlist = Transferlist.Where(x => x.Carrier == TransferSearchVM.Carrier);
            }
            if (!string.IsNullOrWhiteSpace(TransferSearchVM.Tracking))
            {
                Transferlist = Transferlist.Where(x => x.Tracking == TransferSearchVM.Tracking);
            }
            TransferSearchVM.Transferlist = Transferlist.Take(2000);

            return View(TransferSearchVM);
        }
        public ActionResult Create()
        {
            var SID = DateTime.Now.ToString("HHmmssfff");
            ViewBag.SID = SID;
            Session["TSkuNumberList" + SID] = null;
            return View();
        }
        [HttpPost]
        public ActionResult Create(Transfer Transfer, string SID, string BoxLabelSize, string SBarcodeLabelType)
        {
            var CreateBy = UserBy;
            var CreateAt = DateTime.UtcNow;
            Transfer.TransferType = "Winit";
            Transfer.Status = "Pending";
            Transfer.CreateBy = CreateBy;
            Transfer.CreateAt = CreateAt;
            Transfer.WinitTransfer = new WinitTransfer
            {
                IsEnable = true,
                BoxLabelSize = BoxLabelSize,
                SBarcodeLabelType = SBarcodeLabelType,
                CreateBy = CreateBy,
                CreateAt = CreateAt
            };
            var odataList = (List<TranSKUVM>)Session["TSkuNumberList" + SID];
            if (odataList != null)
            {
                foreach (var item in odataList.Where(x => x.Model == "E"))
                {
                    Transfer.TransferSKU.Add(new TransferSKU { IsEnable = true, SkuNo = item.SKU, Name = item.ProductName, QTY = item.QTY, CreateBy = CreateBy, CreateAt = CreateAt });
                    var Logistic = db.Logistic.Find(item.SKU);
                    if (Logistic != null)
                    {
                        Logistic.Price = item.Price ?? 0;
                    }
                    else
                    {
                        db.Logistic.Add(new Logistic { Sku = item.SKU, Price = item.Price ?? 0, CreateBy = CreateBy, CreateAt = CreateAt, BoxID = 1 });
                    }
                }
            }
            db.Transfer.Add(Transfer);
            db.SaveChanges();
            Session["TSkuNumberList" + SID] = null;
            return RedirectToAction("Edit", new { Transfer.ID });
        }
        public ActionResult Edit(int ID)
        {
            var NoPrepSerials = new List<string>();
            var SID = ID;
            var Transfer = db.Transfer.Find(ID);
            if (Transfer.Status == "Requested")
            {
                var Winit_API = new Winit_API();
                foreach (var WinitTransferSKU in Transfer.WinitTransfer.WinitTransferSKU)
                {
                    if (string.IsNullOrWhiteSpace(WinitTransferSKU.winitProductCode))//沒有M碼補上M碼
                    {
                        var WSKUList = Winit_API.SKUList(WinitTransferSKU.SkuNo);
                        if (WSKUList != null && WSKUList.list != null && WSKUList.list.Any())
                        {
                            if (skuList.Contains(WinitTransferSKU.SkuNo))
                            {
                                WinitTransferSKU.winitProductCode = WSKUList.list.Where(x => !string.IsNullOrWhiteSpace(x.specification)).FirstOrDefault().code;
                            }
                            else
                            {
                                WinitTransferSKU.winitProductCode = WSKUList.list.FirstOrDefault().code;
                            }

                        }
                        db.SaveChanges();
                    }
                    if (WinitTransferSKU.winitstatus!= "Yes")
                    {
                        //檢查審核狀態
                        var ItemMt = Winit_API.queryItemMtEntitys(WinitTransferSKU.SkuNo, WinitTransferSKU.WinitTransfer.Transfer.WarehouseTo.WinitWarehouse);
                        if (!ItemMt.list.Any())
                        {
                            WinitTransferSKU.winitstatus = "No";
                        }
                        else 
                        {
                            foreach (var item in ItemMt.list)
                            {
                                switch (item.status)
                                {
                                    case "PD":
                                        WinitTransferSKU.winitstatus = "Yes";
                                        break;
                                    case "DR":
                                        WinitTransferSKU.winitstatus = "Pending";
                                        break;
                                    case "CO":
                                        WinitTransferSKU.winitstatus = "Pending";
                                        break;
                                    case "WSA":
                                        WinitTransferSKU.winitstatus = "Pending";
                                        break;
                                    case "BK":
                                        WinitTransferSKU.winitstatus = "No";
                                        break;
                                }
                            }
                        }
                        db.SaveChanges();
                    }
                }
            }
            var TranSKUVMList = new List<TranSKUVM>();
            foreach (var item in Transfer.TransferSKU.Where(x => x.IsEnable))
            {
                TranSKUVMList.Add(new TranSKUVM
                {
                    ID = item.ID,
                    ck = item.ID,
                    sk = item.SkuNo,
                    SKUR =  Transfer.WinitTransfer.WinitTransferSKU.Where(x => x.IsEnable && x.SkuNo.Contains(item.SkuNo)).FirstOrDefault()?.winitstatus??GetWinitstatus(item.SkuNo + "-" + Transfer.WarehouseTo.WinitWarehouse, Transfer.WarehouseTo.WinitWarehouse),
                    Mcode = Transfer.WinitTransfer.WinitTransferSKU.Where(x => x.IsEnable && x.SkuNo.Contains(item.SkuNo)).FirstOrDefault()?.winitProductCode,
                    SKU = item.SkuNo,
                    ProductName = item.SKU.SkuLang.Where(x => x.LangID == LangID).FirstOrDefault()?.Name,
                    ProductMsg = GetNameSize(item.SKU),
                    QTY = item.QTY,
                    TotalReceive = item.SerialsLlist.Where(x => x.SerialsType == "TransferIn").Sum(x => x.SerialsQTY),
                    Serial = GetSerialMulti(item),
                    TWN = item.Transfer.WarehouseFrom?.Name,
                    Winit = item.Transfer.WarehouseTo?.Name,
                    Model = "L",
                    Prep = PrepSerialChk(item),
                    Price = item.SKU.Logistic?.Price ?? 0
                });
            }
            foreach (var item in TranSKUVMList.Where(x => x.Prep != 0))
            {
                NoPrepSerials.Add(item.SKU + " " + item.ProductName + "X" + item.Prep);
            }
            ViewBag.NoPrepSerials = NoPrepSerials;
            Session["TSkuNumberList" + SID] = TranSKUVMList;
            ViewBag.WCPScript = Neodynamic.SDK.Web.WebClientPrint.CreateScript(Url.Action("ProcessRequest", "WebClientPrintAPI", null, HttpContext.Request.Url.Scheme), Url.Action("PrintMyFiles", "Ajax", null, HttpContext.Request.Url.Scheme), HttpContext.Session.SessionID);
            return View(Transfer);
        }

        private string GetWinitstatus(string skuCode, string importCountryCodee)
        {
            var winitstatus = "";
            var Winit_API = new Winit_API();
            var ItemMt = Winit_API.queryItemMtEntitys(skuCode, importCountryCodee);
            if (!ItemMt.list.Any())
            {
                winitstatus = "No";
            }
            else
            {
                foreach (var item in ItemMt.list)
                {
                    switch (item.status)
                    {
                        case "PD":
                            winitstatus = "Yes";
                            break;
                        case "DR":
                            winitstatus = "Pending";
                            break;
                        case "CO":
                            winitstatus = "Pending";
                            break;
                        case "WSA":
                            winitstatus = "Pending";
                            break;
                        case "BK":
                            winitstatus = "No";
                            break;
                    }
                }
            }
            return winitstatus;
        }

        [HttpPost]
        public ActionResult Edit(Transfer Transfer, string SID, string BoxLabelSize, string SBarcodeLabelType, bool? saveexit, bool? Requestedval)
        {
            var Winit_API = new Winit_API();
            var dt = DateTime.UtcNow;
            var oTransfer = db.Transfer.Find(Transfer.ID);
            if (!string.IsNullOrWhiteSpace(Transfer.ExternalTra)) oTransfer.ExternalTra = Transfer.ExternalTra;
            if (!string.IsNullOrWhiteSpace(Transfer.Title)) oTransfer.Title = Transfer.Title;
            if (Transfer.FromWID.HasValue) oTransfer.FromWID = Transfer.FromWID;
            if (Transfer.ToWID.HasValue) oTransfer.ToWID = Transfer.ToWID;
            if (!string.IsNullOrWhiteSpace(Transfer.Status)) oTransfer.Status = Transfer.Status;
            if (Transfer.Interim.HasValue) oTransfer.Interim = Transfer.Interim;
            if (!string.IsNullOrWhiteSpace(Transfer.Carrier)) oTransfer.Carrier = Transfer.Carrier;
            if (!string.IsNullOrWhiteSpace(Transfer.Tracking)) oTransfer.Tracking = Transfer.Tracking;


            oTransfer.UpdateBy = UserBy;
            oTransfer.UpdateAt = dt;

            oTransfer.WinitTransfer.BoxLabelSize = BoxLabelSize;
            oTransfer.WinitTransfer.SBarcodeLabelType = SBarcodeLabelType;
            oTransfer.WinitTransfer.CreateBy = UserBy;
            oTransfer.WinitTransfer.CreateAt = dt;


            var odataList = (List<TranSKUVM>)Session["TSkuNumberList" + SID];

            foreach (var item in odataList.Where(x => x.Model == "E"))
            {
                var TransferSKUList = oTransfer.TransferSKU.Where(x => x.IsEnable && x.ID == item.ID);
                if (TransferSKUList.Any())
                {
                    foreach (var SKUitem in TransferSKUList)
                    {
                        if (SKUitem.QTY != item.QTY)
                        {
                            SKUitem.QTY = item.QTY;
                            SKUitem.UpdateBy = UserBy;
                            SKUitem.UpdateAt = dt;
                        }
                    }
                }
                else
                {
                    oTransfer.TransferSKU.Add(new TransferSKU { IsEnable = true, SkuNo = item.SKU, Name = item.ProductName, QTY = item.QTY, CreateBy = UserBy, CreateAt = dt });
                }
                var Logistic = db.Logistic.Find(item.SKU);
                if (Logistic != null)
                {
                    Logistic.Price = item.Price ?? 0;
                }
                else
                {
                    db.Logistic.Add(new Logistic { Sku = item.SKU, Price = item.Price ?? 0, BoxID = 1, CreateBy = UserBy, CreateAt = dt });
                }

            }
            foreach (var item in odataList.Where(x => x.Model == "D"))
            {
                var TransferSKUList = oTransfer.TransferSKU.Where(x => x.SkuNo == item.SKU);
                if (TransferSKUList.Any())
                {
                    foreach (var SKUitem in TransferSKUList)
                    {
                        SKUitem.IsEnable = false;
                        SKUitem.UpdateBy = UserBy;
                        SKUitem.UpdateAt = dt;
                    }
                }
            }
            if (Requestedval.HasValue && Requestedval.Value)
            {
                if (oTransfer.Status == "Pending")
                {
                    var NoWSKUList = new List<string>();
                    var Errmsg = "";
                    foreach (var SKUitem in oTransfer.TransferSKU)
                    {
                        var WSKUList = Winit_API.SKUList(SKUitem.SkuNo + "-" + oTransfer.WarehouseTo.WinitWarehouse);
                        if (!WSKUList.list.Any() && !skuList.Contains(SKUitem.SkuNo + "-" + oTransfer.WarehouseTo.WinitWarehouse))//如果沒有資料就新增
                        {

                            //檢查審核狀態
                            var ItemMt = Winit_API.queryItemMtEntitys(SKUitem.SkuNo + "-" + oTransfer.WarehouseTo.WinitWarehouse, oTransfer.WarehouseTo.WinitWarehouse);
                            if (!ItemMt.list.Any())
                            {
                                NoWSKUList.Add(SKUitem.SkuNo);
                            }
                            else if (!ItemMt.list.Where(x => x.status == "PD").Any())
                            {
                                Errmsg += "Winit SKU狀態\\n\\r";
                                foreach (var item in ItemMt.list.Where(x => x.status != "PD"))
                                {
                                    switch (item.status)
                                    {
                                        case "DR":
                                            Errmsg += SKUitem.SkuNo + "-" + oTransfer.WarehouseTo.WinitWarehouse + "待維護\\n\\r";
                                            break;
                                        case "PD":
                                            Errmsg += SKUitem.SkuNo + "-" + oTransfer.WarehouseTo.WinitWarehouse + "已發布\\n\\r";
                                            break;
                                        case "CO":
                                            Errmsg += SKUitem.SkuNo + "-" + oTransfer.WarehouseTo.WinitWarehouse + "維護完成\\n\\r";
                                            break;
                                        case "BK":
                                            Errmsg += SKUitem.SkuNo + "-" + oTransfer.WarehouseTo.WinitWarehouse + "退回\\n\\r";
                                            break;
                                        case "WSA":
                                            Errmsg += SKUitem.SkuNo + "-" + oTransfer.WarehouseTo.WinitWarehouse + "驗證中\\n\\r";
                                            break;
                                    }
                                }
                            }
                        }
                    }
                    if (NoWSKUList.Any())
                    {
                        Errmsg += "Winit SKU註冊中\\n\\r";
                        Errmsg += WinitRegisterProduct(NoWSKUList, oTransfer.WarehouseTo.WinitWarehouse);
                    }
                    if (!string.IsNullOrWhiteSpace(Errmsg))
                    {
                        TempData["ErrMsg"] = Errmsg;
                        return RedirectToAction("Edit", new { Transfer.ID });
                    }
                    oTransfer.Status = "Requested";
                    var WinitWarehouse = oTransfer.WarehouseTo.WinitWarehouse;
                    foreach (var item in oTransfer.TransferSKU.Where(x => x.IsEnable))
                    {
                        var productCode = item.SkuNo + "-" + WinitWarehouse;
                        //Winit API 取S BARCODE
                        var PostPrintV2Data = new PostPrintV2Data
                        {
                            labelType = oTransfer.WinitTransfer.SBarcodeLabelType,
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
                                printQty = item.QTY,
                            });
                        }
                        else
                        {
                            PostPrintV2Data.singleItems.Add(new SingleItem
                            {
                                productCode = productCode,
                                printQty = item.QTY,
                            });
                        }

                        var PrintV2 = Winit_API.GetPrintV2(PostPrintV2Data);

                        if (PrintV2 == null)
                        {
                            TempData["ErrMsg"] = Winit_API.ResultError.msg;
                            return RedirectToAction("Edit", new { Transfer.ID });

                        }
                        oTransfer.WinitTransfer.WinitTransferSKU.Add(new WinitTransferSKU
                        {
                            IsEnable = true,
                            SkuNo = productCode,
                            ItemBarcodeFile = PrintV2.itemBarcodeFile,
                            itemBarcodeList = string.Join(";", PrintV2.itemBarcodeList),
                            CreateBy = UserBy,
                            CreateAt = dt
                        });
                    }
                }
            }
            db.SaveChanges();
            if (saveexit.HasValue && saveexit.Value)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Edit", new { Transfer.ID });
            }
        }
        /// <summary>
        /// Winit註冊商品
        /// </summary>
        /// <param name="SkuList"></param>
        /// <param name="winitWarehouse">倉庫代碼</param>
        /// <returns></returns>
        private string WinitRegisterProduct(List<string> SkuList, string winitWarehouse)
        {
            var Code = CurrencyCode(winitWarehouse);
            var EXRate = db.Currency.Where(x => x.Code == Code).FirstOrDefault()?.EXRate ?? 1;
            var errmsg = "";
            var nRegisterProduct = new RegisterProduct { productList = new List<ProductList>() };
            foreach (var item in SkuList)
            {
                var POSKU = db.SKU.Find(item);
                var chineseName = POSKU.SkuLang.Where(x => x.LangID == "zh-TW").FirstOrDefault()?.Name;
                string englishName = POSKU.SkuLang.Where(x => x.LangID == "en-US").FirstOrDefault()?.Name;
                string SKUmodel = POSKU.SkuLang.Where(x => x.LangID == "en-US").FirstOrDefault()?.Model;
                if (string.IsNullOrWhiteSpace(chineseName))
                {
                    chineseName = englishName;
                }

                var Declaredvalue = Math.Round(POSKU.Logistic.Price / 1.05m / EXRate, 2);
                string battery = POSKU.Battery ? "Y" : "N";
                string brandedName = POSKU.GetBrand.Name;
                string displayPageUrl = POSKU.DisplayPageUrl;
                var ShippingWeight = POSKU.Logistic.ShippingWeight;
                var ShippingLength = POSKU.Logistic.ShippingLength;
                var ShippingWidth = POSKU.Logistic.ShippingWidth;
                var ShippingHeight = POSKU.Logistic.ShippingHeight;
                decimal inportDeclaredvalue = Declaredvalue;
                decimal exportDeclaredvalue = Declaredvalue;
                var OriginCountry = POSKU.Logistic.OriginCountry;
                if (ShippingWeight == 0) errmsg += item + ": ShippingWeight不可為0;" + Environment.NewLine;
                if (ShippingLength == 0) errmsg += item + ": ShippingLength不可為0;" + Environment.NewLine;
                if (ShippingWidth == 0) errmsg += item + ": ShippingWidth不可為0;" + Environment.NewLine;
                if (ShippingHeight == 0) errmsg += item + ": ShippingHeight不可為0;" + Environment.NewLine;
                if (inportDeclaredvalue == 0) errmsg += item + ": inportDeclaredvalue不可為0;" + Environment.NewLine;
                if (exportDeclaredvalue == 0) errmsg += item + ": exportDeclaredvalue不可為0;" + Environment.NewLine;
                if(string.IsNullOrWhiteSpace(displayPageUrl)) errmsg += item + ": 產品官方網址必填;" + Environment.NewLine;
                if (string.IsNullOrWhiteSpace(OriginCountry)) errmsg += item + ": 產品產地必填;" + Environment.NewLine;
                if (string.IsNullOrWhiteSpace(SKUmodel)) errmsg += item + ": 產品型號必填;" + Environment.NewLine;
                nRegisterProduct.productList.Add(new ProductList
                {
                    productCode = item + "-" + winitWarehouse,
                    chineseName = chineseName,
                    englishName = englishName,
                    registeredWeight = ShippingWeight,
                    fixedVolumeWeight = "Y",
                    registeredLength = ShippingLength / 10,
                    registeredWidth = ShippingWidth / 10,
                    registeredHeight = ShippingHeight / 10,
                    branded = "Y",
                    brandedName = brandedName,
                    model = SKUmodel,
                    displayPageUrl = displayPageUrl,
                    exportCountry = "CN",
                    inporCountry = winitWarehouse,
                    inportDeclaredvalue = inportDeclaredvalue,
                    exportDeclaredvalue = exportDeclaredvalue,
                    battery = battery,
                    //remark = "特批客戶"
                });
            }
            if (string.IsNullOrWhiteSpace(errmsg))
            {
                var Winit_API = new Winit_API();
                var rProduct = Winit_API.registerProduct(nRegisterProduct);
                if (Winit_API.ResultError != null)
                {
                    errmsg = Winit_API.ResultError.msg;
                }
            }
            return errmsg;
        }


        public ActionResult Requested(int ID)
        {
            var Transfer = db.Transfer.Find(ID);
            if (Transfer.Status == "Pending")
            {
                Transfer.Status = "Requested";
            }
            else
            {
                if (Transfer.TransferSKU.Where(x => x.SerialsLlist.Any()).Any())
                {
                    return Json(new { status = false, Errmsg = "已輸入序號，無法修改" }, JsonRequestBehavior.AllowGet);
                }
                Transfer.Status = "Pending";
                db.WinitTransferSKU.RemoveRange(Transfer.WinitTransfer.WinitTransferSKU.ToList());
            }

            Transfer.UpdateBy = UserBy;
            Transfer.UpdateAt = DateTime.UtcNow;
            db.SaveChanges();

            return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Prep(int ID)
        {
            //var WinitPrepTable = new WinitPrepTable { ID = ID, BoxID = "testBOXID" };
            var oTransfer = db.Transfer.Find(ID);
            var PrepVMList = new List<TransferItemVM>();
            foreach (var item in oTransfer.TransferSKU.Where(x => x.IsEnable))
            {
                PrepVMList.Add(new TransferItemVM
                {
                    ID = item.ID,
                    WarehouseID = oTransfer.FromWID,
                    SKU = item.SkuNo,
                    Name = item.Name,
                    QTY = item.QTY,
                    SerialsLlist = item.SerialsLlist.Where(x => x.SerialsType == "TransferOut").ToList(), //一般序號移倉
                    RMASerialsLlist = item.RMASerialsLlist.Where(x => x.SerialsType == "TransferOut").ToList() //RMA序號移倉
                });
            }
            Session["WinitPrepVMList" + ID] = PrepVMList;
            if (!oTransfer.WinitTransfer.WinitTransferBox.Any())
            {
                oTransfer.WinitTransfer.WinitTransferBox.Add(new WinitTransferBox());
            }
            Session["WinitTransferBox" + ID] = oTransfer.WinitTransfer.WinitTransferBox.ToList();
            ViewBag.WCPScript = Neodynamic.SDK.Web.WebClientPrint.CreateScript(Url.Action("ProcessRequest", "WebClientPrintAPI", null, HttpContext.Request.Url.Scheme), Url.Action("PrintMyFiles", "Ajax", null, HttpContext.Request.Url.Scheme), HttpContext.Session.SessionID);
            return View(oTransfer);
        }

        [HttpPost]
        public ActionResult Prep(int ID, List<PostList> Prep, bool? saveexit)
        {
            SavePrep(ID, Prep);
            if (saveexit.HasValue && saveexit.Value)
            {
                return RedirectToAction("Edit", new { ID });
            }
            else
            {
                return RedirectToAction("Prep", new { ID });
            }
        }

        private string SavePrep(int ID, List<PostList> Prep)
        {
            var Msg = "";
            if (Prep != null)
            {
                Prep = Prep.Where(x => !string.IsNullOrWhiteSpace(x.val)).Distinct().ToList();
            }
            var PrepVMList = (List<TransferItemVM>)Session["WinitPrepVMList" + ID];
            var WinitTransferBoxList = (List<WinitTransferBox>)Session["WinitTransferBox" + ID];
            var CreateBy = UserBy;
            var CreateAt = DateTime.UtcNow;
            var oTransfer = db.Transfer.Find(ID);
            foreach (var item in oTransfer.TransferSKU.Where(x => x.IsEnable))
            {
                //一般
                foreach (var PrepVM in PrepVMList.Where(x => x.SKU == item.SkuNo))
                {
                    var nSerialsLlist = PrepVM.SerialsLlist.Select(x => new SerialsLlist
                    {
                        IsEnable = true,
                        TransferSKUID = item.ID,
                        PID = x.ID,
                        CreateAt = CreateAt,
                        CreateBy = CreateBy,
                        UpdateAt = CreateAt,
                        UpdateBy = CreateBy,
                        //OrderID = x.OrderID,
                        PurchaseSKUID = x.PurchaseSKUID,
                        //RMAID = x.RMAID,
                        SerialsNo = x.SerialsNo,
                        SerialsQTY = -1,
                        SerialsType = "TransferOut"//等待移倉
                    }).ToList();
                    //db.SerialsLlist.AddRange(nSerialsLlist);
                    foreach (var Serial in nSerialsLlist)
                    {
                        if (!item.SerialsLlist.Where(x => x.SerialsNo == Serial.SerialsNo).Any())
                        {
                            item.SerialsLlist.Add(Serial);
                        }
                    }
                }


                //無序號
                if (!item.SKU.SerialTracking)//不用序號
                {
                    var TransferOutCount = item.SerialsLlist.Where(x => x.SerialsType == "TransferOut").Count();
                    if (Prep != null)
                    {
                        foreach (var Prepitem in Prep.Where(x => x.ID == item.ID))
                        {
                            var PrepCount = 0;
                            if (int.TryParse(Prepitem.val, out PrepCount))
                            {
                                if (PrepCount > item.QTY)
                                {
                                    PrepCount = item.QTY.Value;
                                }
                                if (PrepCount > TransferOutCount)
                                {
                                    var SerialsType = new List<string> { "PO", "TransferIn" };
                                    var TransferOutlist = db.SerialsLlist.Where(x => !x.SerialsLlistC.Any(y => y.IsEnable) && x.PurchaseSKU.IsEnable && x.PurchaseSKU.PurchaseOrder.IsEnable && x.PurchaseSKU.PurchaseOrder.WarehouseID == oTransfer.FromWID && x.PurchaseSKU.SkuNo == item.SkuNo && SerialsType.Contains(x.SerialsType)).Take(PrepCount - TransferOutCount).ToList();
                                    var nSerialsLlist = TransferOutlist.Select(x => new SerialsLlist
                                    {
                                        IsEnable = true,
                                        TransferSKUID = item.ID,
                                        PID = x.ID,
                                        CreateAt = CreateAt,
                                        CreateBy = CreateBy,
                                        UpdateAt = CreateAt,
                                        UpdateBy = CreateBy,
                                        OrderID = x.OrderID,
                                        PurchaseSKUID = x.PurchaseSKUID,
                                        SerialsNo = x.SerialsNo,
                                        SerialsQTY = -1,
                                        SerialsType = "TransferOut"//等待移倉
                                    }).ToList();
                                    foreach (var Serial in nSerialsLlist)
                                    {
                                        if (!item.SerialsLlist.Where(x => x.SerialsNo == Serial.SerialsNo).Any())
                                        {
                                            item.SerialsLlist.Add(Serial);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            try
            {
                db.SaveChanges();
                foreach (var Boxitem in WinitTransferBoxList)//Winit單
                {
                    if (Boxitem.ID == 0)
                    {
                        Boxitem.IsEnable = true;
                        Boxitem.CreateBy = CreateBy;
                        Boxitem.CreateAt = CreateAt;
                        foreach (var item in Boxitem.WinitTransferBoxItem)
                        {
                            var Serial = db.SerialsLlist.Where(x => x.IsEnable && x.SerialsNo == item.SerialsNo && x.SerialsType == "TransferOut" && x.TransferSKU.TransferID == ID && x.TransferSKU.IsEnable && x.TransferSKU.Transfer.IsEnable).FirstOrDefault();
                            item.SerialsLlistID = Serial.ID;
                            item.MerchandiseCode = item.SkuNo + "-" + oTransfer.WarehouseTo.WinitWarehouse;
                            item.Value = Serial.TransferSKU.SKU.Logistic?.Price;
                            item.CreateBy = CreateBy;
                            item.CreateAt = CreateAt;
                        }
                        oTransfer.WinitTransfer.WinitTransferBox.Add(Boxitem);
                    }
                    else
                    {
                        var WinitTransferBox = db.WinitTransferBox.Find(Boxitem.ID);
                        if (WinitTransferBox.Length != Boxitem.Length) WinitTransferBox.Length = Boxitem.Length;
                        if (WinitTransferBox.Width != Boxitem.Width) WinitTransferBox.Width = Boxitem.Width;
                        if (WinitTransferBox.Heigth != Boxitem.Heigth) WinitTransferBox.Heigth = Boxitem.Heigth;
                        if (WinitTransferBox.Weight != Boxitem.Weight) WinitTransferBox.Weight = Boxitem.Weight;

                        foreach (var item in Boxitem.WinitTransferBoxItem)
                        {
                            if (!WinitTransferBox.WinitTransferBoxItem.Where(x => x.SerialsNo == item.SerialsNo).Any())
                            {
                                var Serial = db.SerialsLlist.Where(x => x.IsEnable && x.SerialsNo == item.SerialsNo && x.SerialsType == "TransferOut" && x.TransferSKU.TransferID == ID && x.TransferSKU.IsEnable && x.TransferSKU.Transfer.IsEnable).FirstOrDefault();
                                item.SerialsLlistID = Serial.ID;
                                item.MerchandiseCode = item.SkuNo + "-" + oTransfer.WarehouseTo.WinitWarehouse;
                                item.Value = Serial.TransferSKU.SKU.Logistic?.Price;
                                item.CreateBy = CreateBy;
                                item.CreateAt = CreateAt;
                                WinitTransferBox.WinitTransferBoxItem.Add(item);
                            }
                        }
                    }
                }
                db.SaveChanges();
            }
            catch (Exception ex)
            {

                Msg = ex.ToString();
            }
            return Msg;
        }

        private decimal? GetSkuPrice(SerialsLlist serial)
        {
            var Price = 0m;
            if (serial.PurchaseSKUID.HasValue)
            {
                Price = serial.PurchaseSKU.SKU.Logistic?.Price ?? 0;
            }
            else if (serial.TransferSKUID.HasValue)
            {
                Price = serial.TransferSKU.SKU.Logistic?.Price ?? 0;
            }
            return Price;
        }

        public ActionResult Receive(int ID)
        {
            var oTransfer = db.Transfer.Find(ID);
            var ReceiveVMList = new List<TransferItemVM>();
            foreach (var item in oTransfer.TransferSKU.Where(x => x.IsEnable))
            {
                ReceiveVMList.Add(new TransferItemVM
                {
                    ID = item.ID,
                    SKU = item.SkuNo,
                    Name = item.Name,
                    QTY = item.QTY,
                    SerialsLlist = item.SerialsLlist.Where(x => x.SerialsType == "TransferIn").ToList(),//一般
                    RMASerialsLlist = item.RMASerialsLlist.Where(x => x.SerialsType == "TransferIn").ToList()//RMA
                });
            }
            Session["ReceiveVMList" + ID] = ReceiveVMList;
            if (!oTransfer.WinitTransfer.WinitTransferBox.Any())
            {
                oTransfer.WinitTransfer.WinitTransferBox.Add(new WinitTransferBox());
            }
            Session["WinitTransferBox" + ID] = oTransfer.WinitTransfer.WinitTransferBox.ToList();
            ViewData["Mod"] = "Receive";
            ViewData["SID"] = ID;
            return View(oTransfer);
        }
        [HttpPost]
        public ActionResult Receive(int ID, List<PostList> Prep, bool? saveexit)
        {
            Prep = Prep.Where(x => !string.IsNullOrWhiteSpace(x.val)).Distinct().ToList();
            var CreateBy = UserBy;
            var CreateAt = DateTime.UtcNow;
            var ReceiveVMList = (List<TransferItemVM>)Session["ReceiveVMList" + ID];
            var oTransfer = db.Transfer.Find(ID);
            foreach (var item in oTransfer.TransferSKU.Where(x => x.IsEnable))
            {
                foreach (var ReceiveVM in ReceiveVMList.Where(x => x.SKU == item.SkuNo))
                {
                    if (oTransfer.Status != "Received")
                    {
                        oTransfer.Status = "Received";
                        oTransfer.UpdateAt = CreateAt;
                        oTransfer.UpdateBy = CreateBy;
                    }
                    //一般
                    var nSerialsLlist = ReceiveVM.SerialsLlist.Select(x => new SerialsLlist
                    {
                        IsEnable = true,
                        TransferSKUID = item.ID,
                        PID = x.ID,
                        CreateAt = CreateAt,
                        CreateBy = CreateBy,
                        ReceivedAt = CreateAt,
                        ReceivedBy = CreateBy,
                        //OrderID = x.OrderID,
                        PurchaseSKUID = x.PurchaseSKUID,
                        //RMAID = x.RMAID,
                        SerialsNo = x.SerialsNo,
                        SerialsQTY = 1,
                        SerialsType = "TransferIn"
                    }).ToList();
                    //db.SerialsLlist.AddRange(nSerialsLlist);
                    foreach (var Serial in nSerialsLlist)
                    {
                        if (!item.SerialsLlist.Where(x => x.SerialsNo == Serial.SerialsNo && x.SerialsType == "TransferIn").Any())
                        {
                            item.SerialsLlist.Add(Serial);
                        }
                    }

                    //RMA
                    var nRMASerialsLlist = ReceiveVM.RMASerialsLlist.Select(x => new SerialsLlist
                    {
                        IsEnable = true,
                        TransferSKUID = item.ID,
                        PID = x.ID,
                        CreateAt = CreateAt,
                        CreateBy = CreateBy,
                        ReceivedAt = CreateAt,
                        ReceivedBy = CreateBy,
                        //OrderID = x.OrderID,
                        //PurchaseSKUID = x.PurchaseSKUID,
                        //RMAID = x.RMAID,
                        SerialsNo = x.SerialsNo,
                        SerialsQTY = 1,
                        SerialsType = "TransferIn"
                    }).ToList();
                    //db.SerialsLlist.AddRange(nSerialsLlist);
                    foreach (var Serial in nRMASerialsLlist)
                    {
                        if (!item.SerialsLlist.Where(x => x.SerialsNo == Serial.SerialsNo && x.SerialsType == "TransferIn").Any())
                        {
                            item.SerialsLlist.Add(Serial);
                        }
                    }
                }
                //無序號
                if (!item.SKU.SerialTracking)//不用序號
                {
                    var TransferOutCount = item.SerialsLlist.Where(x => x.SerialsType == "TransferIn").Count();
                    foreach (var Prepitem in Prep.Where(x => x.ID == item.ID))
                    {
                        var PrepCount = 0;
                        if (int.TryParse(Prepitem.val, out PrepCount))
                        {
                            if (PrepCount > item.QTY)
                            {
                                PrepCount = item.QTY.Value;
                            }
                            if (PrepCount > TransferOutCount)
                            {
                                var TransferOutlist = db.SerialsLlist.Where(x => !x.SerialsLlistC.Any(y => y.IsEnable) && x.PurchaseSKU.IsEnable && x.PurchaseSKU.PurchaseOrder.IsEnable && x.PurchaseSKU.PurchaseOrder.WarehouseID == oTransfer.FromWID && x.PurchaseSKU.SkuNo == item.SkuNo && x.SerialsType == "TransferOut").Take(PrepCount - TransferOutCount).ToList();
                                foreach (var SerialOut in TransferOutlist)
                                {
                                    SerialOut.SerialsLlistC.Add(new SerialsLlist
                                    {
                                        IsEnable = true,
                                        TransferSKUID = item.ID,
                                        PID = SerialOut.ID,
                                        CreateAt = CreateAt,
                                        CreateBy = CreateBy,
                                        ReceivedAt = CreateAt,
                                        ReceivedBy = CreateBy,
                                        //OrderID = x.OrderID,
                                        //PurchaseSKUID = x.PurchaseSKUID,
                                        //RMAID = x.RMAID,
                                        SerialsNo = SerialOut.SerialsNo,
                                        SerialsQTY = 1,
                                        SerialsType = "TransferIn"
                                    });
                                }
                            }
                        }
                    }
                }
            }
            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {

                var s = ex.ToString();
            }
            if (saveexit.HasValue && saveexit.Value)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Receive", new { ID });
            }
        }
        public ActionResult PrepSaveserials(string serials, string SID, int boxitemset)
        {
            var SKU = db.SKU.Find(serials);
            if (SKU != null && !SKU.SerialTracking)
            {
                serials = db.SerialsLlist.Where(y => y.IsEnable && y.SerialsQTY > 0 && (y.SerialsType == "PO" || y.SerialsType == "TransferIn" || y.SerialsType == "DropshpOrderIn") && !y.SerialsLlistC.Any(z => z.IsEnable) && ((y.PurchaseSKUID.HasValue && y.PurchaseSKU.SkuNo == serials && !y.PurchaseSKU.SKU.SerialTracking) || (y.TransferSKUID.HasValue && y.TransferSKU.SkuNo == serials && !y.TransferSKU.SKU.SerialTracking))).FirstOrDefault()?.SerialsNo;
            }
            var PrepVMList = (List<TransferItemVM>)Session["WinitPrepVMList" + SID];
            var WinitTransferBoxList = (List<WinitTransferBox>)Session["WinitTransferBox" + SID];
            var WarehouseID = PrepVMList.FirstOrDefault().WarehouseID;
            var SerialsLlist = db.SerialsLlist.Where(x => x.SerialsNo == serials && !x.SerialsLlistC.Any(y => y.IsEnable) && x.SerialsQTY > 0);//找到序號
            SerialsLlist = SerialsLlist.Where(x => (x.TransferSKUID.HasValue && x.TransferSKU.Transfer.ToWID == WarehouseID) || (x.PurchaseSKUID.HasValue && x.PurchaseSKU.PurchaseOrder.WarehouseID == WarehouseID));
            var RMASerialsLlist = db.RMASerialsLlist.Where(x => x.SerialsNo == serials && !x.RMASerialsLlistC.Any() && x.SerialsQTY > 0 && x.WarehouseID == WarehouseID);//找到RMA序號
            if (SerialsLlist.Any())
            {
                if (SerialsLlist.Where(x => x.SerialsQTY > 0 && !x.SerialsLlistC.Any(y => y.IsEnable)).Any())
                {
                    var Serial = SerialsLlist.FirstOrDefault();
                    var SkuNo = "";
                    if (Serial.PurchaseSKUID.HasValue)
                    {
                        SkuNo = Serial.PurchaseSKU.SkuNo;
                    }
                    else if (Serial.TransferSKUID.HasValue)
                    {
                        SkuNo = Serial.TransferSKU.SkuNo;
                    }

                    var PrepVM = PrepVMList.Where(x => x.SKU == SkuNo && x.QTY > x.SerialsLlist.Count()).FirstOrDefault();
                    if (PrepVM != null)
                    {
                        if (PrepVM.QTY > PrepVM.SerialsLlist.Count() + PrepVM.RMASerialsLlist.Count())
                        {
                            if (!PrepVMList.Where(x => x.SerialsLlist.Where(y => y.SerialsNo == serials).Any()).Any() && !PrepVMList.Where(x => x.RMASerialsLlist.Where(y => y.SerialsNo == serials).Any()).Any())
                            {
                                var print = "";
                                var WinitTransferSKU = db.Transfer.Find(int.Parse(SID)).WinitTransfer.WinitTransferSKU.Where(x => x.SkuNo.Contains(SkuNo)).FirstOrDefault();
                                var itemBarcodeList = WinitTransferSKU.itemBarcodeList.Split(';');
                                //var IndexNo = WinitTransferSKU.IndexNo;
                                var FilePage = 0;
                                PrepVM.SerialsLlist.Add(Serial);
                                var WinitTransferBox = WinitTransferBoxList.Skip(boxitemset).Take(1).FirstOrDefault();
                                //var broke = false;
                                var GBoxList = WinitTransferBoxList.Where(x => x.WinitTransferBoxItem.Where(y => y.SkuNo == SkuNo).Any()).GroupBy(x => x.WinitTransferBoxItem.GroupBy(y => y.SkuNo)).ToList();
                                FilePage = GBoxList.Sum(x => x.Sum(y => y.WinitTransferBoxItem.Where(z => z.SkuNo == SkuNo).Count())) + 1;
                                //foreach (var itemBarcode in itemBarcodeList)
                                //{


                                //    //foreach (var WinitTransferBoxitem in GBoxList)
                                //    //{
                                //    //    WinitTransferBoxitem.
                                //    //    var GBoxItem = WinitTransferBoxitem.WinitTransferBoxItem.Where(x => x.SkuNo == SkuNo);

                                //    //    if (!WinitTransferBoxItem.Where(x => x.BarCode == itemBarcode).Any())
                                //    //    {
                                //    //        FilePage = WinitTransferBoxItem.Count() + 1;
                                //    //        BarCode = itemBarcode;
                                //    //        broke = true;
                                //    //        break;
                                //    //    }
                                //    //}
                                //    //if (broke) break;
                                //}
                                var Name = "";
                                var Weight = 0m;
                                if (Serial.PurchaseSKUID.HasValue)
                                {
                                    Name = Serial.PurchaseSKU.Name;
                                    Weight = Serial.PurchaseSKU.SKU.Logistic?.ShippingWeight ?? 0;
                                }
                                else if (Serial.TransferSKUID.HasValue)
                                {
                                    Name = Serial.TransferSKU.Name;
                                    Weight = Serial.TransferSKU.SKU.Logistic?.ShippingWeight ?? 0;
                                }
                                //檢查是否有用過
                                for (int i = 1; i <= FilePage; i++)
                                {
                                    if (!WinitTransferBox.WinitTransferBoxItem.Where(x=>x.BarCode == itemBarcodeList[i - 1]).Any())
                                    {
                                        FilePage = i;
                                        break;
                                    }
                                }

                                WinitTransferBox.WinitTransferBoxItem.Add(new WinitTransferBoxItem
                                {
                                    SkuNo = SkuNo,
                                    SerialsLlistID = Serial.ID,
                                    SerialsNo = Serial.SerialsNo,
                                    Name = Name,
                                    BarCode = itemBarcodeList[FilePage - 1],
                                    FilePage = FilePage.ToString(),
                                    WinitTransferSKUID = WinitTransferSKU.ID,
                                    Weight = Weight
                                });
                                db.SaveChanges();
                                print = "key=serial&id=" + WinitTransferSKU.ID + "&Page=" + FilePage;
                                Session["WinitPrepVMList" + SID] = PrepVMList;
                                Session["WinitTransferBox" + SID] = WinitTransferBoxList;

                                var Msg = SavePrep(int.Parse(SID), null);
                                if (string.IsNullOrWhiteSpace(Msg))
                                {
                                    return Json(new { status = true, print }, JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                    return Json(new { status = false }, JsonRequestBehavior.AllowGet);
                                }
                            }
                            else
                            {
                                return Json(new { status = false, Errmsg = "序號已在清單" }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            return Json(new { status = false, Errmsg = "移倉數量超過" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        return Json(new { status = false, Errmsg = "序號無對應的SKU" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { status = false, Errmsg = "序號不在倉庫，此序號不能移倉" }, JsonRequestBehavior.AllowGet);
                }
            }
            else if (RMASerialsLlist.Any())
            {
                if (RMASerialsLlist.Where(x => x.SerialsQTY > 0 && !x.RMASerialsLlistC.Any()).Any())
                {
                    var RMASerial = RMASerialsLlist.FirstOrDefault();
                    var RMAPrepVM = PrepVMList.Where(x => x.SKU == RMASerial.RMASKU.SkuNo || x.SKU == RMASerial.NewSkuNo).FirstOrDefault();
                    try
                    {
                        if (RMAPrepVM != null)
                        {
                            if (RMAPrepVM.QTY > RMAPrepVM.SerialsLlist.Count() + RMAPrepVM.RMASerialsLlist.Count())
                            {
                                if (!PrepVMList.Where(x => x.SerialsLlist.Where(y => y.SerialsNo == serials).Any()).Any() && !PrepVMList.Where(x => x.RMASerialsLlist.Where(y => y.SerialsNo == serials).Any()).Any())
                                {
                                    RMAPrepVM.RMASerialsLlist.Add(RMASerial);
                                    Session["WinitPrepVMList" + SID] = PrepVMList;
                                    return Json(new { status = true }, JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                    return Json(new { status = false, Errmsg = "序號已在清單" }, JsonRequestBehavior.AllowGet);
                                }
                            }
                            else
                            {
                                return Json(new { status = false, Errmsg = "移倉數量超過" }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            return Json(new { status = false, Errmsg = "序號無對應的SKU" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    catch (Exception ex)
                    {

                        throw;
                    }
                }
                else
                {
                    return Json(new { status = false, Errmsg = "序號不在倉庫，此序號不能移倉" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { status = false, Errmsg = "序號不存在或是已在清單" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult AddBox(int ID)
        {
            var WinitTransferBoxList = (List<WinitTransferBox>)Session["WinitTransferBox" + ID];
            WinitTransferBoxList.Add(new WinitTransferBox());
            Session["WinitTransferBox" + ID] = WinitTransferBoxList;
            return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult PrintBox(int ID)
        {

            var Winit_API = new Winit_API();
            var WinitTransfer = db.WinitTransfer.Find(ID);
            if (string.IsNullOrWhiteSpace(WinitTransfer.WinitOrderNo))//沒有訂單號，產生訂單
            {
                //檢查Box規格
                if (WinitTransfer.WinitTransferBox.Where(x => !x.Heigth.HasValue || !x.Length.HasValue || !x.Width.HasValue).Any())
                {
                    var Errmsg = "必須輸入Box的規格";
                    return Json(new { status = false, Errmsg }, JsonRequestBehavior.AllowGet);
                }
                var warehouseCode = "";
                var Warehouse3P = WinitTransfer.Transfer.WarehouseTo.WarehouseSummary.Where(x => x.Type == "3PWarehouse").FirstOrDefault();
                if (Warehouse3P == null || string.IsNullOrWhiteSpace(Warehouse3P.Val))
                {
                    return Json(new { status = false, Msg = "3PWarehouse無資料" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    warehouseCode = Warehouse3P.Val;
                }
                var importDeclarationRuleCode = "CIOR";
                var exportDeclarationType = "CEOR";
                //var WinitProducts = Winit_API.getWinitProducts("OW0103");
                var winitProductCode = "OW01030329";  //var winitProductCode = WinitProducts[0].productCode;
                //var WarehouseList = Winit_API.getWarehouseList(winitProductCode, "INSJ", "DW", null);
                //var warehouseCode = WarehouseList.warehouseList[0].warehouseCode;
                //var IORList = Winit_API.IORList(warehouseCode, winitProductCode);
                //var EorList = Winit_API.EorList();
                //var LogisticsPlan = Winit_API.getLogisticsPlan(winitProductCode, warehouseCode, warehouseCode);
                //var AvailableMerchandise = Winit_API.getAvailableMerchandise("WC", winitProductCode, warehouseCode);
                var VendorInfoIOR = Winit_API.getVendorInfo("AU", "IOR");//進口
                var VendorInfoEOR = Winit_API.getVendorInfo("AU", "EOR");//出口
                var packageList = new List<PackageList>();
                var destinationWarehouseCode = warehouseCode;
                string exporterCode = VendorInfoEOR.FirstOrDefault().vendorCode;
                string importerCode = VendorInfoIOR.FirstOrDefault().vendorCode;
                //string logisticsPlanNo = "";
                var preparedOffPortDate = Helpers.MyHelps.WorkingDay(1);
                var preparedArrivePortDate = Helpers.MyHelps.WorkingDay(2, preparedOffPortDate);
                // 產生WINIT訂單
                var nWinitCreateOrder = new WinitCreateOrder
                {
                    orderType = "DW",
                    winitProductCode = winitProductCode,
                    destinationWarehouseCode = destinationWarehouseCode,
                    sellerOrderNo = "TransferWinit:" + ID,
                    inspectionWarehouseCode = destinationWarehouseCode,
                    importDeclarationRuleCode = importDeclarationRuleCode,
                    importerCode = importerCode,
                    exportDeclarationType = exportDeclarationType,
                    exporterCode = exporterCode,
                    packageList = packageList,
                    //logisticsPlanNo = logisticsPlanNo,
                    directForecastInfo = new directForecastInfo
                    {
                        preparedOffPortDate = preparedOffPortDate.ToString("yyyy-MM-dd"),
                        preparedArrivePortDate = preparedArrivePortDate.ToString("yyyy-MM-dd")
                    }

                };
         
                foreach (var Box in WinitTransfer.WinitTransferBox)
                {
                    var merchandiseList = new List<MerchandiseList>();
                    var NpackageList = new PackageList
                    {
                        sellerCaseNo = ID + "-" + Box.ID,
                        sellerHeight = Box.Heigth.ToString(),
                        sellerLength = Box.Length.ToString(),
                        sellerWidth = Box.Width.ToString(),
                        sellerWeight = Box.Weight.ToString(),
                        merchandiseList = merchandiseList,
                        thirdPartyCaseNo = ""
                    };
                    foreach (var item in Box.WinitTransferBoxItem.GroupBy(x => x.MerchandiseCode))
                    {
                        var specification = "";
                        if (skuList.Contains(item.Key))
                        {
                            specification = db.SKU.Where(x => item.Key.Contains(x.SkuID)).FirstOrDefault().SkuLang.FirstOrDefault(x => x.LangID == "en-US").Name;
                        }
                        //if (AvailableMerchandise.Where(x => x.merchandiseCode == item.Key).Any())
                        {
                            var NmerchandiseList = new MerchandiseList
                            {
                                merchandiseCode = item.Key,
                                quantity = item.Count(),
                                specification= specification
                            };
                            merchandiseList.Add(NmerchandiseList);
                        }
                    }
                    packageList.Add(NpackageList);
                }
                var WinitOrderCreate = Winit_API.WinitOrderCreate(nWinitCreateOrder);
                if (WinitOrderCreate == null)
                {
                    var Errmsg = Winit_API.ResultError.msg;
                    return Json(new { status = false, Errmsg }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    WinitTransfer.WinitOrderNo = WinitOrderCreate.orderNo;
                    db.SaveChanges();
                }

            }
            if (string.IsNullOrWhiteSpace(WinitTransfer.PrintPackageLabe))
            {
                var OrderDetail = Winit_API.getOrderDetail(WinitTransfer.WinitOrderNo);
                foreach (var packageList in OrderDetail.packageList)
                {
                    var CaseNo = packageList.sellerCaseNo.Split(':');
                    var BoxID = 0;
                    var sBoxID = "";
                    if (CaseNo.Count() == 2)
                    {
                        sBoxID = CaseNo[1];
                    }
                    else if (CaseNo.Count() == 1)
                    {
                        sBoxID = CaseNo[0];
                    }
                    if (int.TryParse(sBoxID, out BoxID))
                    {
                        foreach (var Boxitem in WinitTransfer.WinitTransferBox.Where(x => x.ID == BoxID))
                        {
                            Boxitem.PackageNo = packageList.packageNo;
                        }
                    }
                }
                var packageNoList = OrderDetail.packageList.Select(x => new packageNoList { packageNo = x.packageNo, sellerCaseNo = "" }).ToList();
                var printPackageLabe = Winit_API.printPackageLabe(WinitTransfer.WinitOrderNo, WinitTransfer.BoxLabelSize, packageNoList);
                WinitTransfer.PrintPackageLabe = printPackageLabe.Label;
                db.SaveChanges();
            }


            var print = "key=box&id=" + WinitTransfer.TransferID;
            return Json(new { status = true, print }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult PrintAWB(int ID)
        {
            //產生Fedex單
            var WinitTransfer = db.WinitTransfer.Find(ID);
            if (string.IsNullOrWhiteSpace(WinitTransfer.Transfer.Tracking))//空的就產提單
            {
                var ShippingMethods = db.ShippingMethods.Find(int.Parse(WinitTransfer.Transfer.Carrier));
                if (ShippingMethods.Name.Contains("FedEx"))
                {


                    var winitWarehouse = WinitTransfer.Transfer.WarehouseTo.WinitWarehouse;
                    var Currency = CurrencyCode(winitWarehouse);
                    var EXRate = db.Currency.Where(x => x.Code == Currency).FirstOrDefault()?.EXRate ?? 1;
                    var nFedExData = new FedExApi.FedExData
                    {
                        TransferID = WinitTransfer.TransferID,
                        Currency = Currency,
                        BoxType = ShippingMethods.LastMile.BoxType,
                        MethodType = ShippingMethods.LastMile.MethodType,
                        WinitTransferBoxList = WinitTransfer.WinitTransferBox.Where(x => x.IsEnable && x.WinitTransferBoxItem.Any()).ToList(),
                        WinitWarehouse = WinitTransfer.Transfer.WarehouseTo,
                        EXRate = EXRate,
                    };
                    var ApiSetting = db.ApiSetting.Find(16);
                    var FedExTrackingDataList = new FedExApi.FedEx_API(ApiSetting).CreateBox(nFedExData);
                    if (FedExTrackingDataList.Any())
                    {
                        var index = 0;
                        WinitTransfer.Transfer.Tracking = FedExTrackingDataList.FirstOrDefault().Tracking;
                        foreach (var Boxitem in WinitTransfer.WinitTransferBox)
                        {
                            Boxitem.Tracking = FedExTrackingDataList[index].Tracking;
                            Boxitem.FedExPdf = ZplToPdf( FedExTrackingDataList[index].FedExZpl);
                            index++;
                        }
                        db.SaveChanges();
                    }
                    else
                    {
                        return Json(new { status = false, Errmsg = "FedEx提單產生錯誤" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { status = false, Errmsg = "非FedEx，無去產生提單" }, JsonRequestBehavior.AllowGet);
                }
            }
            var print = "key=FedEx&id=" + WinitTransfer.TransferID;
            return Json(new { status = true, print }, JsonRequestBehavior.AllowGet);
        }

        private string ZplToPdf(byte[] fedExZpl)
        {
            System.Net.HttpWebRequest webRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create("http://api.labelary.com/v1/printers/8dpmm/labels/4x6/");
            webRequest.Method = "POST";
            webRequest.Accept = "application/pdf";
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.ContentLength = fedExZpl.Length;
            using (Stream requestStream = webRequest.GetRequestStream())
            {
                requestStream.Write(fedExZpl, 0, fedExZpl.Length);

                System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)webRequest.GetResponse();
                using (StreamReader StreamReader = new StreamReader(response.GetResponseStream()))
                {
                    var ms = new MemoryStream();
                    StreamReader.BaseStream.CopyTo(ms);
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        public ActionResult BoxValChange(int ID, string name, string val)
        {
            var index = 0;
            var boxNo = 0;
            var newval = 0m;
            var keylist = name.Split('_');
            var WinitTransferBoxList = (List<WinitTransferBox>)Session["WinitTransferBox" + ID];
            int.TryParse(keylist[1], out boxNo);
            decimal.TryParse(val, out newval);
            foreach (var item in WinitTransferBoxList)
            {
                if (index == boxNo)
                {
                    switch (keylist[0])
                    {
                        case "Length":
                            item.Length = newval;
                            break;
                        case "Width":
                            item.Width = newval;
                            break;
                        case "Heigth":
                            item.Heigth = newval;
                            break;
                        default:
                            break;
                    }
                }
                index++;
            }
            Session["WinitTransferBox" + ID] = WinitTransferBoxList;
            return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult BoxChange(int ID)
        {
            ViewData["Mod"] = "";
            ViewData["SID"] = ID;
            var WinitTransferBoxList = (List<WinitTransferBox>)Session["WinitTransferBox" + ID];
            var html = RenderPartialViewToString("Boxitem", WinitTransferBoxList);
            var set = WinitTransferBoxList.Count() - 1;
            return Json(new { status = true, html, set }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DelSerialsNo(string serial, int ID,int serialID)
        {
            var WinitTransferBoxList = (List<WinitTransferBox>)Session["WinitTransferBox" + ID];
            var PrepVMList = (List<TransferItemVM>)Session["WinitPrepVMList" + ID];
            foreach (var Boxitem in WinitTransferBoxList)
            {
                foreach (var item in Boxitem.WinitTransferBoxItem.ToList())
                {
                    if (item.SerialsLlistID == serialID)
                    {
                        Boxitem.WinitTransferBoxItem.Remove(item);
                    }
                }
            }
            foreach (var Prepitem in PrepVMList)
            {
                foreach (var item in Prepitem.SerialsLlist.ToList())
                {
                    if (item.ID == serialID)
                    {
                        Prepitem.SerialsLlist.Remove(item);
                    }
                }
            }
            Session["WinitTransferBox" + ID] = WinitTransferBoxList;
            Session["WinitPrepVMList" + ID] = PrepVMList;

            var oSerialsLlist = db.SerialsLlist.Find(serialID);
            db.WinitTransferBoxItem.Remove(oSerialsLlist.WinitTransferBoxItem);
            db.SerialsLlist.Remove(oSerialsLlist);
            db.SaveChanges();
            return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Ship(int ID)
        {
            var Transfer = db.Transfer.Find(ID);
            if (!string.IsNullOrWhiteSpace(Transfer.WinitTransfer.WinitOrderNo))
            {
                if (!string.IsNullOrWhiteSpace(Transfer.Tracking))
                {
                    var status = true;
                    status = SendMailToFedEx(Transfer);
                    //status = SendMailToWinit(Transfer);
                    if (status)
                    {
                        Transfer.Status = "Shipped";
                        Transfer.UpdateBy = UserBy;
                        Transfer.UpdateAt = DateTime.UtcNow;
                        db.SaveChanges();
                    }
                    else
                    {
                        TempData["ErrMsg"] = "Mail寄送失敗";
                    }
                }
                else
                {
                    TempData["ErrMsg"] = "無FedEx單號,不能Ship";
                }
            }
            else
            {
                TempData["ErrMsg"] = "無Winit入庫單號,不能Ship";
            }
            return RedirectToAction("Edit", new { ID });
            //return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        }

        private bool SendMailToWinit(Transfer transfer)
        {
            throw new NotImplementedException();
        }

        private bool SendMailToFedEx(Transfer transfer)
        {
            string MailFrom = "dispatch-qd@hotmail.com";
            string MailSub = string.Format("至優網 (Zhi You Wan LTD) 正式出口報關資料 (Tracking #" + transfer.Tracking + ")");
            string MailBody = "";
            string[] MailTos = new string[] { "edd@fedex.com" };
            //string[] MailTos = new string[] { "leoli.qd@hotmail.com" };

            List<string> FedExFile1 = new List<string>();
            List<Tuple<Stream, string>> FedExFile2 = new List<Tuple<Stream, string>>();
            string[] Ccs = new string[] { "peter@qd.com.tw", "kelly@qd.com.tw", "demi@qd.com.tw" };
            //string[] Ccs = new string[] {  };//
            if (string.IsNullOrWhiteSpace(transfer.WinitTransfer.InvoiceExcel))
            {
                var Stream = InvoiceExcel(transfer);
                FedExFile2.Add(new Tuple<Stream, string>(Stream, "Invoice.xlsx"));//發票檔
                transfer.WinitTransfer.InvoiceExcel= Convert.ToBase64String(Stream.ToArray());
            }
            else
            {
                FedExFile2.Add(new Tuple<Stream, string>(Base64ToMemoryStream(transfer.WinitTransfer.InvoiceExcel), "Invoice.xlsx"));//發票檔       
            }
            if (string.IsNullOrWhiteSpace(transfer.WinitTransfer.CheckList))
            {
                var Stream = CheckListExcel(transfer);
                transfer.WinitTransfer.CheckList = Convert.ToBase64String(Stream.ToArray());//不知道為什麼直接使用不行，必須轉成Base64才能正常
                //FedExFile2.Add(new Tuple<Stream, string>(Stream, "CheckList.zip"));
                FedExFile2.Add(new Tuple<Stream, string>(Base64ToMemoryStream(transfer.WinitTransfer.CheckList), "CheckList.zip"));
            }
            else
            {
                FedExFile2.Add(new Tuple<Stream, string>(Base64ToMemoryStream(transfer.WinitTransfer.CheckList), "CheckList.zip"));
            }
            if (string.IsNullOrWhiteSpace(transfer.WinitTransfer.PackingList))
            {
                var Stream = Box_PackingListExcel(transfer);
                FedExFile2.Add(new Tuple<Stream, string>(Stream, "Box_PackingList.xlsx"));
                transfer.WinitTransfer.PackingList= Convert.ToBase64String(Stream.ToArray());
            }
            else
            {
                FedExFile2.Add(new Tuple<Stream, string>(Base64ToMemoryStream(transfer.WinitTransfer.PackingList), "Box_PackingList.xlsx"));
            }
            FedExFile2.Add(new Tuple<Stream, string>(new MemoryStream(System.IO.File.ReadAllBytes(Server.MapPath(@"~/File/Fedex_Recognizances.pdf"))), "Fedex_Recognizances.pdf"));
            //FedExFile1.Add(Path.Combine(filePath, "PackageList.xlsx"));
            db.SaveChanges();
            return  Helpers.MyHelps.Mail_Send(MailFrom, MailTos, Ccs, MailSub, MailBody, true, FedExFile1.ToArray(), FedExFile2, false);
        }
        public ActionResult RemoveData(string[] IDList, string SID)
        {
            var Errmsg = "";
            if (IDList != null && IDList.Any())
            {
                var odataList = (List<TranSKUVM>)Session["TSkuNumberList" + SID];
                foreach (var item in IDList)
                {
                    foreach (var odataListitem in odataList.Where(x => x.ck.ToString() == item || x.ID.ToString() == item || x.SKU == item))
                    {
                        if (odataListitem.ID.HasValue)
                        {
                            var TransferSKU = db.TransferSKU.Find(odataListitem.ID);
                            if (TransferSKU.SerialsLlist.Any())
                            {
                                Errmsg += "【" + TransferSKU.SkuNo + "】已有序號不能刪除；";
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
                Session["TSkuNumberList" + SID] = odataList;
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
        public ActionResult DownloadBox(int ID)
        {
            var file = new ZipFile();
            var OutputStream = new MemoryStream();
            var WinitTransfer = db.WinitTransfer.Find(ID);
            var pdfbyte = Convert.FromBase64String(WinitTransfer.PrintPackageLabe);
            file.AddEntry("Box-" + WinitTransfer.WinitOrderNo + ".pdf", pdfbyte);
            file.Save(OutputStream);
            return File(OutputStream.ToArray(), "application/zip", "Box.zip");
        }
        public ActionResult DownloadAWB(int ID)
        {
            var file = new ZipFile();
            var OutputStream = new MemoryStream();
            var WinitTransfer = db.WinitTransfer.Find(ID);
            foreach (var item in WinitTransfer.WinitTransferBox)
            {
                var pdfbyte = Convert.FromBase64String(item.FedExPdf);
                using (PdfReader reader = new PdfReader(pdfbyte))
                {
                    var os = new MemoryStream();
                    Document document = new Document();
                    PdfCopy copy = new PdfCopy(document, os);
                    document.Open();
                    if (reader.NumberOfPages > 1)
                    {
                        copy.AddPage(copy.GetImportedPage(reader, 2));
                    }
                    for (int pagenumber = 1; pagenumber <= reader.NumberOfPages; pagenumber++)
                    {
                        copy.AddPage(copy.GetImportedPage(reader, pagenumber));
                    }
                    document.Close();
                    file.AddEntry("AWB-" + item.Tracking + ".pdf", os.ToArray());
                }
            }
            file.Save(OutputStream);
            return File(OutputStream.ToArray(), "application/zip", "AWB.zip");
        }
        public ActionResult DownloadSCode(int? SerialsLlistID,int? boxid,int? TransferID)
        {
            var file = new ZipFile();
            var OutputStream = new MemoryStream();
            if (boxid.HasValue)
            {
                var WinitTransferBox = db.WinitTransferBox.Find(boxid);
                foreach (var item in WinitTransferBox.WinitTransferBoxItem)
                {
                    var nWinitTransferSKUID = item.WinitTransferSKUID;
                    var nFilePage = int.Parse(item.FilePage);
                    file.AddEntry("SCode-" + item.BarCode + ".pdf", SplistSCodePdf(nWinitTransferSKUID, nFilePage));
                }
            }
            else if (SerialsLlistID.HasValue)
            {
                var WinitTransferBoxItem = db.WinitTransferBoxItem.Find(SerialsLlistID);
                file.AddEntry("SCode-" + WinitTransferBoxItem.BarCode + ".pdf", SplistSCodePdf(WinitTransferBoxItem.WinitTransferSKUID, int.Parse(WinitTransferBoxItem.FilePage)));
            }
            else if (TransferID.HasValue)
            {
                var WinitTransfer = db.WinitTransfer.Find(TransferID);
                foreach (var WinitTransferBox in WinitTransfer.WinitTransferBox.Where(x => x.IsEnable))
                {
                    foreach (var item in WinitTransferBox.WinitTransferBoxItem)
                    {
                        var nWinitTransferSKUID = item.WinitTransferSKUID;
                        var nFilePage = int.Parse(item.FilePage);
                        file.AddEntry("SCode-" + item.BarCode + ".pdf", SplistSCodePdf(nWinitTransferSKUID, nFilePage));
                    }
                }
            }
            file.Save(OutputStream);
            return File(OutputStream.ToArray(), "application/zip", "SCode.zip");
        }
        public ActionResult GetBoxSerialList(int boxid, string skuno)
        {
            var WinitTransferBox = db.WinitTransferBox.Find(boxid);
            var SerialsLlist = WinitTransferBox.WinitTransferBoxItem.Where(x => x.SkuNo == skuno).Select(x => new BoxSerialList { SerialsNo = x.SerialsLlist.SerialsNo, BarCode = x.BarCode, Status = GetType(x.SerialsLlist) }).ToList();
            var partial = ControlToString("~/Views/TransferWinit/GetBoxSerialList.cshtml", SerialsLlist);
            return Json(new { status = true, partial }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DownloadInvoice(int ID)
        {
            var file = new ZipFile();
            var OutputStream = new MemoryStream();
            var transfer = db.Transfer.Find(ID);
            var fileStream = Base64ToMemoryStream(transfer.WinitTransfer.InvoiceExcel);
            file.AddEntry("InvoiceExcel-" + transfer.Tracking + ".xlsx", fileStream.ToArray());
            file.Save(OutputStream);
            return File(OutputStream.ToArray(), "application/zip", "InvoiceExcel.zip");
        }
        public ActionResult DownloadExcelOut(int ID)
        {
            var WinitTransfer = db.WinitTransfer.Find(ID);
            var OutputStream = new MemoryStream();
            //在記憶體中建立一個Excel物件
            ExcelPackage ep = new ExcelPackage();
            //加入一個Sheet
            ep.Workbook.Worksheets.Add("MySheet");
            //取得剛剛加入的Sheet(實體Sheet就叫MySheet)
            ExcelWorksheet sheet1 = ep.Workbook.Worksheets["MySheet"];//取得Sheet1 
            sheet1.Cells[1, 1].Value = "Box";//
            sheet1.Cells[1, 2].Value = "Winit M Code";//
            sheet1.Cells[1, 3].Value = "SKU";//
            sheet1.Cells[1, 4].Value = "Serial";//
            sheet1.Cells[1, 5].Value = "S Code";//
            sheet1.Cells[1, 6].Value = "Stock Status";//
            sheet1.Cells[1, 7].Value = "Price";//
            //迴圈部份自由料理
            int row = 1;
            var box = 0;
            foreach (var WinitTransferBoxitem in WinitTransfer.WinitTransferBox)
            {
                box++;
                foreach (var WinitTransferBoxItem in WinitTransferBoxitem.WinitTransferBoxItem)
                {
                    row++;
                    sheet1.Cells[row, 1].Value = box;
                    sheet1.Cells[row, 2].Value = WinitTransferBoxItem.WinitTransferBox.WinitTransfer.WinitTransferSKU.Where(x => x.IsEnable && x.SkuNo.Contains(WinitTransferBoxItem.SkuNo)).FirstOrDefault()?.winitProductCode;
                    sheet1.Cells[row, 3].Value = WinitTransferBoxItem.SkuNo;
                    sheet1.Cells[row, 4].Value = WinitTransferBoxItem.SerialsNo;
                    sheet1.Cells[row, 5].Value = WinitTransferBoxItem.BarCode;
                    sheet1.Cells[row, 6].Value = WinitTransferBoxItem.SerialsLlist.SerialsLlistC.Where(x => x.IsEnable && x.SerialsType == "TransferIn").Any() ? "已入庫" : "已出庫";
                    sheet1.Cells[row, 7].Value = WinitTransferBoxItem.SerialsLlist.TransferSKU.SKU.Logistic.Price;
                }
            }
            return File(ep.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "WinitBoxExcel" + WinitTransfer.Transfer.Tracking + ".xlsx");
        }
        private string GetType(SerialsLlist SerialsLlist)
        {
            var returnval = "";
            if (SerialsLlist.SerialsLlistC.Where(x => x.SerialsType == "TransferIn").Any())
            {
                returnval = "已入庫";
            }
            else if (SerialsLlist.TransferSKU.Transfer.Status == "Requested")
            {
                returnval = "待出庫";
            }
            else if (SerialsLlist.TransferSKU.Transfer.Status == "Shipped")
            {
                returnval = "已出庫";
            }
            return returnval;
        }
        private byte[] SplistSCodePdf(int? winitTransferSKUID, int? filePage)
        {
            var WinitTransferSKU = db.WinitTransferSKU.Find(winitTransferSKUID);
            var fileStream = Base64ToMemoryStream(WinitTransferSKU.ItemBarcodeFile);
            var os = new MemoryStream();
            using (PdfReader reader = new PdfReader(fileStream))
            {
                Document document = new Document();
                PdfCopy copy = new PdfCopy(document, os);
                document.Open();
                copy.AddPage(copy.GetImportedPage(reader, filePage.Value));
                document.Close();
            }
            return os.ToArray();
        }
    }
}