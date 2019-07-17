using PurchaseOrderSys.Models;
using PurchaseOrderSys.NewApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PurchaseOrderSys.Controllers
{
    public class TransferWinitController : BaseController
    {
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
            TransferSearchVM.Transferlist = Transferlist;

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
        public ActionResult Create(Transfer Transfer, string SID,string BoxLabelSize,string SBarcodeLabelType)
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
                        db.Logistic.Add(new Logistic { Sku = item.SKU, Price = item.Price ?? 0, CreateBy = CreateBy, CreateAt = CreateAt });
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
            var TranSKUVMList = new List<TranSKUVM>();
            foreach (var item in Transfer.TransferSKU.Where(x => x.IsEnable))
            {
                TranSKUVMList.Add(new TranSKUVM
                {
                    ID = item.ID,
                    ck = item.ID,
                    sk = item.SkuNo,
                    SKU = item.SkuNo,
                    ProductName = item.SKU.SkuLang.Where(x => x.LangID == LangID).FirstOrDefault()?.Name,
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
            return View(Transfer);
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
                    db.Logistic.Add(new Logistic {Sku= item.SKU,Price = item.Price ?? 0 , CreateBy = UserBy, CreateAt = dt });
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
                    foreach (var SKUitem in oTransfer.TransferSKU)
                    {

                        var WSKUList = Winit_API.SKUList(SKUitem.SkuNo + "-" + oTransfer.WarehouseTo.WinitWarehouse);
                        if (!WSKUList.list.Any())//如果沒有資料就新增
                        {
                            NoWSKUList.Add(SKUitem.SkuNo);
                        }
                    }
                    if (NoWSKUList.Any())
                    {
                        var Errmsh = WinitRegisterProduct(NoWSKUList, oTransfer.WarehouseTo.WinitWarehouse);
                        if (!string.IsNullOrWhiteSpace(Errmsh))
                        {
                            TempData["ErrMsg"] = Errmsh;
                            return RedirectToAction("Edit", new { Transfer.ID });
                        }
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
                        PostPrintV2Data.singleItems.Add(new SingleItem
                        {
                            productCode = productCode,
                            printQty = item.QTY,
                        });
                        var PrintV2 =Winit_API.GetPrintV2(PostPrintV2Data);

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
                string displayPageUrl = "internal.qd.com.tw:8080";
                var ShippingWeight = POSKU.Logistic.ShippingWeight;
                var ShippingLength = POSKU.Logistic.ShippingLength;
                var ShippingWidth = POSKU.Logistic.ShippingWidth;
                var ShippingHeight = POSKU.Logistic.ShippingHeight;
                decimal inportDeclaredvalue = Declaredvalue;
                decimal exportDeclaredvalue = Declaredvalue;

                if (ShippingWeight == 0) errmsg += item + ":ShippingWeight不可為0;" + Environment.NewLine;
                if (ShippingLength == 0) errmsg += item + ":ShippingLength不可為0;" + Environment.NewLine;
                if (ShippingWidth == 0) errmsg += item + ":ShippingWidth不可為0;" + Environment.NewLine;
                if (ShippingHeight == 0) errmsg += item + ":ShippingHeight不可為0;" + Environment.NewLine;
                if (inportDeclaredvalue == 0) errmsg += item + ":inportDeclaredvalue不可為0;" + Environment.NewLine;
                if (exportDeclaredvalue == 0) errmsg += item + ":exportDeclaredvalue不可為0;" + Environment.NewLine;

                nRegisterProduct.productList.Add(new ProductList
                {
                    productCode = item + "-" + winitWarehouse,
                    chineseName = chineseName,
                    englishName = englishName,
                    registeredWeight = ShippingWeight,
                    fixedVolumeWeight = "Y",
                    registeredLength = ShippingLength,
                    registeredWidth = ShippingWidth,
                    registeredHeight = ShippingHeight,
                    branded = "Y",
                    brandedName = brandedName,
                    model = SKUmodel,
                    displayPageUrl = displayPageUrl,
                    exportCountry = "TW",
                    inporCountry = winitWarehouse,
                    inportDeclaredvalue = inportDeclaredvalue,
                    exportDeclaredvalue = exportDeclaredvalue,
                    battery = battery
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Country">國家代碼(2碼)</param>
        /// <returns></returns>
        private string CurrencyCode(string Country)
        {
            var Code = "";
            switch (Country)
            {
                case "AU":
                    Code = "AUD";
                    break;
                case "US":
                    Code = "USD";
                    break;
                case "HK":
                    Code = "HKD";
                    break;
                case "JP":
                    Code = "JPY";
                    break;
                case "TW":
                    Code = "TWD";
                    break;
                default:
                    break;
            }
            return Code;
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
                oTransfer.WinitTransfer.WinitTransferBox.Add(new WinitTransferBox ());
            }
            Session["WinitTransferBox" + ID] = oTransfer.WinitTransfer.WinitTransferBox.ToList();
            ViewBag.WCPScript = Neodynamic.SDK.Web.WebClientPrint.CreateScript(Url.Action("ProcessRequest", "WebClientPrintAPI", null, HttpContext.Request.Url.Scheme), Url.Action("PrintMyFiles", "TransferWinit", null, HttpContext.Request.Url.Scheme), HttpContext.Session.SessionID);
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

        private void SavePrep(int ID, List<PostList> Prep)
        {
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
                                var TransferOutlist = db.SerialsLlist.Where(x => !x.SerialsLlistC.Any() && x.PurchaseSKU.IsEnable && x.PurchaseSKU.PurchaseOrder.IsEnable && x.PurchaseSKU.PurchaseOrder.WarehouseID == oTransfer.FromWID && x.PurchaseSKU.SkuNo == item.SkuNo && SerialsType.Contains(x.SerialsType)).Take(PrepCount - TransferOutCount).ToList();
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

                var s = ex.ToString();
            }
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
                                var TransferOutlist = db.SerialsLlist.Where(x => !x.SerialsLlistC.Any() && x.PurchaseSKU.IsEnable && x.PurchaseSKU.PurchaseOrder.IsEnable && x.PurchaseSKU.PurchaseOrder.WarehouseID == oTransfer.FromWID && x.PurchaseSKU.SkuNo == item.SkuNo && x.SerialsType == "TransferOut").Take(PrepCount - TransferOutCount).ToList();
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
        public ActionResult PrepSaveserials(string serials, string SID,int boxitemset)
        {
            var PrepVMList = (List<TransferItemVM>)Session["WinitPrepVMList" + SID];
            var WinitTransferBoxList = (List<WinitTransferBox>)Session["WinitTransferBox" + SID];
            var WarehouseID = PrepVMList.FirstOrDefault().WarehouseID;
            var SerialsLlist = db.SerialsLlist.Where(x => x.SerialsNo == serials && !x.SerialsLlistC.Any() && x.SerialsQTY > 0);//找到序號
            SerialsLlist = SerialsLlist.Where(x => (x.TransferSKUID.HasValue && x.TransferSKU.Transfer.ToWID == WarehouseID) || (x.PurchaseSKUID.HasValue && x.PurchaseSKU.PurchaseOrder.WarehouseID == WarehouseID));
            var RMASerialsLlist = db.RMASerialsLlist.Where(x => x.SerialsNo == serials && !x.RMASerialsLlistC.Any() && x.SerialsQTY > 0 && x.WarehouseID == WarehouseID);//找到RMA序號
            if (SerialsLlist.Any())
            {
                if (SerialsLlist.Where(x => x.SerialsQTY > 0 && !x.SerialsLlistC.Any()).Any())
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

                                WinitTransferBox.WinitTransferBoxItem.Add(new WinitTransferBoxItem
                                {
                                    SkuNo = SkuNo,
                                    SerialsLlistID = Serial.ID,
                                    SerialsNo = Serial.SerialsNo,
                                    Name = Serial.PurchaseSKU.Name,
                                    BarCode = itemBarcodeList[FilePage - 1],
                                    FilePage = FilePage.ToString(),
                                    WinitTransferSKUID = WinitTransferSKU.ID,
                                    Weight = Serial.PurchaseSKU.SKU.Logistic?.ShippingWeight ?? 0
                                });
                                print = "key=serial&id=" + WinitTransferSKU.ID + "&Page=" + FilePage;
                                Session["WinitPrepVMList" + SID] = PrepVMList;
                                Session["WinitTransferBox" + SID] = WinitTransferBoxList;

                                return Json(new { status = true, print }, JsonRequestBehavior.AllowGet);
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
                var warehouseCode = "";
                var Warehouse3P = WinitTransfer.Transfer.WarehouseTo.WarehouseSummary.Where(x => x.Type == "3PWarehouse").FirstOrDefault();
                if (Warehouse3P == null|| string.IsNullOrWhiteSpace( Warehouse3P.Val))
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
                var IORList = Winit_API.IORList(warehouseCode, winitProductCode);
                var EorList = Winit_API.EorList();
                var LogisticsPlan = Winit_API.getLogisticsPlan(winitProductCode, warehouseCode, warehouseCode);
                var AvailableMerchandise = Winit_API.getAvailableMerchandise("WC", winitProductCode, warehouseCode);
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
                        //if (AvailableMerchandise.Where(x => x.merchandiseCode == item.Key).Any())
                        {
                            var NmerchandiseList = new MerchandiseList
                            {
                                merchandiseCode = item.Key,
                                quantity = item.Count(),
                                specification = ""
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
            //產生Fedex單
            var ShippingMethods = db.ShippingMethods.Find(int.Parse(WinitTransfer.Transfer.Carrier));
            var winitWarehouse = WinitTransfer.Transfer.WarehouseTo.WinitWarehouse;
            var Currency = CurrencyCode(winitWarehouse);
            var EXRate = db.Currency.Where(x => x.Code == Currency).FirstOrDefault()?.EXRate ?? 1;
            var nFedExData = new FedExApi.FedExData
            {
                TransferID = WinitTransfer.TransferID,
                Currency = Currency,
                BoxType = ShippingMethods.LastMile.BoxType,
                MethodType = ShippingMethods.LastMile.MethodType,
                WinitTransferBoxList = WinitTransfer.WinitTransferBox.ToList(),
                WinitWarehouse = WinitTransfer.Transfer.WarehouseTo,
                EXRate = EXRate,
            };
            var CreateBox = new FedExApi.FedEx_API().CreateBox(nFedExData);
            var print = "key=box&id=" + WinitTransfer.TransferID;
            return Json(new { status = true, print }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult BoxValChange(int ID,string name,string val)
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
        public ActionResult DelSerialsNo(string serial, int ID)
        {
            var WinitTransferBoxList = (List<WinitTransferBox>)Session["WinitTransferBox" + ID];
            var PrepVMList = (List<TransferItemVM>)Session["WinitPrepVMList" + ID];
            foreach (var Boxitem in WinitTransferBoxList)
            {
                foreach (var item in Boxitem.WinitTransferBoxItem.ToList())
                {
                    if (item.SerialsNo.Trim() == serial.Trim())
                    {
                        Boxitem.WinitTransferBoxItem.Remove(item);
                    }
                }
            }
            foreach (var Prepitem in PrepVMList)
            {
                foreach (var item in Prepitem.SerialsLlist.ToList())
                {
                    if (item.SerialsNo.Trim() == serial.Trim())
                    {
                        Prepitem.SerialsLlist.Remove(item);
                    }
                }
            }
            Session["WinitTransferBox" + ID] = WinitTransferBoxList;
            Session["WinitPrepVMList" + ID] = PrepVMList;
            return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Ship(int ID)
        {
            var Transfer = db.Transfer.Find(ID);
            if (!string.IsNullOrWhiteSpace(Transfer.WinitTransfer.WinitOrderNo))
            {
                Transfer.Status = "Shipped";
                Transfer.UpdateBy = UserBy;
                Transfer.UpdateAt = DateTime.UtcNow;
                db.SaveChanges();
            }
            else
            {
                TempData["ErrMsg"] = "無Winit入庫單號,不能Ship";
            }
            return RedirectToAction("Edit", new { ID });
            //return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        }
        [AllowAnonymous]
        public void PrintMyFiles(int id,string Page,string key)
        {
            byte[] pdfbyte = new byte[0];
            //var b64 = "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KFNpbmdsZUl0ZW1cKSkvQ3JlYXRpb25EYXRlKEQ6MjAxOTA3MDgxNzE5MDgrMDgnMDAnKS9Qcm9kdWNlcihpVGV4dCA0LjIuMCBieSAxVDNYVCk+PgplbmRvYmoKeHJlZgowIDUzCjAwMDAwMDAwMDAgNjU1MzUgZiAKMDAwMDAwMTM2OSAwMDAwMCBuIAowMDAwMDA4MTQxIDAwMDAwIG4gCjAwMDAwNDQ1MjYgMDAwMDAgbiAKMDAwMDA1NTczMiAwMDAwMCBuIAowMDAwMDAwMDE1IDAwMDAwIG4gCjAwMDAwMDA5NjkgMDAwMDAgbiAKMDAwMDA1Njk2NyAwMDAwMCBuIAowMDAwMDAyMDQ1IDAwMDAwIG4gCjAwMDAwNDQ2NjMgMDAwMDAgbiAKMDAwMDAwMTY0NSAwMDAwMCBuIAowMDAwMDAyNzI0IDAwMDAwIG4gCjAwMDAwNDU4OTUgMDAwMDAgbiAKMDAwMDAwMjMyMiAwMDAwMCBuIAowMDAwMDAzNDA1IDAwMDAwIG4gCjAwMDAwNDgzNDkgMDAwMDAgbiAKMDAwMDAwMzAwMyAwMDAwMCBuIAowMDAwMDA0MDg2IDAwMDAwIG4gCjAwMDAwNTIwMzggMDAwMDAgbiAKMDAwMDAwMzY4NCAwMDAwMCBuIAowMDAwMDA0NzY5IDAwMDAwIG4gCjAwMDAwNDk1ODUgMDAwMDAgbiAKMDAwMDAwNDM2NSAwMDAwMCBuIAowMDAwMDA1NDUxIDAwMDAwIG4gCjAwMDAwNTMyNzIgMDAwMDAgbiAKMDAwMDAwNTA0OCAwMDAwMCBuIAowMDAwMDA2MTMzIDAwMDAwIG4gCjAwMDAwNTQ1MDQgMDAwMDAgbiAKMDAwMDAwNTczMCAwMDAwMCBuIAowMDAwMDA2ODEzIDAwMDAwIG4gCjAwMDAwNDcxMjAgMDAwMDAgbiAKMDAwMDAwNjQxMiAwMDAwMCBuIAowMDAwMDA3NDkzIDAwMDAwIG4gCjAwMDAwNTA4MTEgMDAwMDAgbiAKMDAwMDAwNzA5MiAwMDAwMCBuIAowMDAwMDA3NzczIDAwMDAwIG4gCjAwMDAwMDc4MDkgMDAwMDAgbiAKMDAwMDAwNzg0NiAwMDAwMCBuIAowMDAwMDA3ODgyIDAwMDAwIG4gCjAwMDAwMDc5MTkgMDAwMDAgbiAKMDAwMDAwNzk1NiAwMDAwMCBuIAowMDAwMDA3OTkzIDAwMDAwIG4gCjAwMDAwMDgwMzAgMDAwMDAgbiAKMDAwMDAwODA2NyAwMDAwMCBuIAowMDAwMDA4MTA0IDAwMDAwIG4gCjAwMDAwMDgyMjkgMDAwMDAgbiAKMDAwMDA0Mzc3MSAwMDAwMCBuIAowMDAwMDQzOTYwIDAwMDAwIG4gCjAwMDAwNDQyMDkgMDAwMDAgbiAKMDAwMDA1NzA5MyAwMDAwMCBuIAowMDAwMDU3MzkzIDAwMDAwIG4gCjAwMDAwNTc0MjcgMDAwMDAgbiAKMDAwMDA1NzUzMiAwMDAwMCBuIAp0cmFpbGVyCjw8L0luZm8gNTIgMCBSL0lEIFs8NGFmMTNlNDYwNTIwOGVkODhiODc0MmNhYTA0N2EzOGU+PDM1ZmU3ZWIwMzY5MDc1ODQ1MTAzNGVhOTBjZDNhYjBjPl0vUm9vdCA1MSAwIFIvU2l6ZSA1Mz4+CnN0YXJ0eHJlZgo1NzY5MwolJUVPRgo=";
            if (key == "serial")
            {

                var WinitTransferSKU = db.WinitTransferSKU.Find(id);
                pdfbyte = Convert.FromBase64String(WinitTransferSKU.ItemBarcodeFile);
            }
            else if (key == "box")
            {
                var WinitTransfer = db.WinitTransfer.Find(id);
                pdfbyte = Convert.FromBase64String(WinitTransfer.PrintPackageLabe);
            }
            var file = new Neodynamic.SDK.Web.PrintFilePDF(pdfbyte, "Barcode.pdf");
            if (key == "serial")
                file.PagesRange = Page;//要印的頁數

            //Create a ClientPrintJob 
            var cpj = new Neodynamic.SDK.Web.ClientPrintJob();
            //set client printer, for multiple files use DefaultPrinter...
            cpj.ClientPrinter = new Neodynamic.SDK.Web.DefaultPrinter();//預設印表機

            //cpj.ClientPrinter = new UserSelectedPrinter();//自己選印表機
            //set files-printers group by using special formatting!!!
            //Invoice.doc PRINT TO Printer1
            cpj.PrintFileGroup.Add(file);
            //send it...
            System.Web.HttpContext.Current.Response.ContentType = "application/octet-stream";
            System.Web.HttpContext.Current.Response.BinaryWrite(cpj.GetContent());
            System.Web.HttpContext.Current.Response.End();

        }
    }
}