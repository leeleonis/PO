using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using inventorySKU.NetoDeveloper;
using Newtonsoft.Json;
using PurchaseOrderSys.Models;

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
    public class TestController : Controller
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
            var SerialsLlist = new List<SerialsLlist>();
            var POSerialsLlist = new List<SerialsLlist>();
            var DSerialsLlist = new List<SerialsLlist>();
            foreach (var PurchaseOrderitem in db.PurchaseOrder.Where(x => x.IsEnable))
            {
                foreach (var PurchaseSKUitem in PurchaseOrderitem.PurchaseSKU.Where(x => x.IsEnable && x.SkuNo == "106005422"))
                {
                    SerialsLlist.AddRange(PurchaseSKUitem.SerialsLlist);
                    POSerialsLlist.AddRange(PurchaseSKUitem.SerialsLlist.Where(x => x.SerialsType == "PO"));
                }
            }
            DSerialsLlist.AddRange(SerialsLlist.Except(POSerialsLlist));
            DSerialsLlist.AddRange(POSerialsLlist.Except(SerialsLlist));
            if (DSerialsLlist.Any())
            {
            }

            return View();
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

        public void DoSkuSync()
        {
            NetoApi neto = new NetoApi();
            string LangID = EnumData.DataLangList().First().Key;

            var categoryList = neto.GetCategory().Category.ToList();
            var conditionList = db.Condition.Where(c => c.IsEnable).ToList();

            SKU sku;
            Brand brand;
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
                                        brand = new Brand()
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
                                            CreateAt = sku.CreateAt,
                                            CreateBy = sku.CreateBy
                                        };

                                        sku = SKU.CreateSku(sku, langData);
                                    }
                                }
                                catch(Exception e)
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

        public void GetProductData(string skuID)
        {
            var SC_Api = new SellerCloud_WebService.SC_WebService("tim@weypro.com", "timfromweypro");
            var sku1 = SC_Api.Get_Product(skuID);

           var netoApi = new NetoApi();
            var sku2 = netoApi.GetItemBySku(skuID);
        }

        public void GetProductBrand()
        {
            var SC_Api = new SellerCloud_WebService.SC_WebService("tim@weypro.com", "timfromweypro");
            var Brand = SC_Api.Get_Manufacturers();
            foreach(var brand in Brand.OrderBy(b => b.ID))
            {
                Response.Write(string.Format("{0} - {1}<br />", brand.ID, brand.ManufacturerName));
            }
        }
    }
}