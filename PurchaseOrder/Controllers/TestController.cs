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
        {//在建構子裡新增資料
            for (int i = 0; i < 300; i++)
            {
                this._myRecords.Add(new MyRecord()
                {
                    sysid = i + 1,
                    MyTitle = "MyTitle" + i,
                    MyMoney = i * 1000
                });
            }//end for 
        }
        /// <summary>
        /// DataSource 資料集合，通常為DB裡的資料
        /// </summary>
        private List<MyRecord> _myRecords = new List<MyRecord>();

        // GET: Test
        public ActionResult Index()
        {
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

            var categroyList = neto.GetCategory().Category.ToList();
            var conditionList = db.Condition.Where(c => c.IsEnable).ToList();

            SKU sku;
            Brand brand;
            List<string> MissCategory = new List<string>();
            int page = 0, limit = 100, categoryID;
            var skuList = neto.GetItemBySkus(page++, limit);
            try
            {
                do
                {
                    Debug.Print(string.Format("Sync Sku from {0} to {1}...", page * limit + 1, (page + 1) * limit));
                    foreach (var item in skuList.Item.Where(i => i.Categories[0] != null))
                    {
                        var eBayTitle = new Dictionary<int, string>()
                        {
                            { 2, item.Misc02 }, { 19, item.Misc19 }, { 50, item.Misc50 }, { 24, item.Misc24 }, { 25, item.Misc25 }, { 23, item.Misc23 }, { 21, item.Misc21 },{ 20, item.Misc20 }, { 22, item.Misc22 }
                        };

                        if (!db.SKU.Any(s => s.SkuID.Equals(item.SKU)))
                        {
                            categoryID = int.Parse(item.Categories.First().Category[0].CategoryID);
                            if (!db.SkuType.Any(t => t.NetoID.Value.Equals(categoryID)))
                            {
                                categoryID = int.Parse(categroyList.First(c => c.CategoryID.Equals(categoryID.ToString())).ParentCategoryID);
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
                                    CreateBy = Session["AdminName"].ToString()
                                };

                                db.Entry(brand).State = System.Data.Entity.EntityState.Added;
                                db.SaveChanges();
                            }

                            sku = new SKU()
                            {
                                IsEnable = true,
                                SkuID = item.SKU,
                                Company = 163,
                                ParentSku = item.ParentSKU,
                                Category = db.SkuType.FirstOrDefault(t => t.NetoID.Value.Equals(categoryID))?.ID ?? 0,
                                Brand = brand.ID,
                                Condition = CheckCondition(conditionList, item.SKU),
                                UPC = item.UPC,
                                Type = (byte)EnumData.SkuType.Single,
                                eBayTitle = Newtonsoft.Json.JsonConvert.SerializeObject(eBayTitle),
                                Status = Convert.ToByte(item.IsActive),
                                CreateAt = DateTime.UtcNow,
                                CreateBy = Session["AdminName"].ToString()
                            };

                            sku.SkuLang.Add(new SkuLang()
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
                            });

                            db.SKU.Add(sku);
                        }
                        else
                        {
                            sku = db.SKU.Find(item.SKU);
                            sku.Company = 163;
                            sku.Condition = CheckCondition(conditionList, sku.SkuID);
                            sku.Status = Convert.ToByte(item.IsActive);
                            sku.eBayTitle = Newtonsoft.Json.JsonConvert.SerializeObject(eBayTitle);
                            sku.UpdateAt = DateTime.UtcNow;
                            sku.UpdateBy = Session["AdminName"].ToString();

                            var skuLang = sku.SkuLang.First(l => l.LangID.Equals(LangID));
                            skuLang.Description = item.Description;
                            skuLang.PackageContent = item.Misc01;
                            skuLang.SpecContent = item.Specifications;
                            skuLang.UpdateAt = sku.UpdateAt.Value;
                            skuLang.UpdateBy = sku.UpdateBy;
                        }
                    }

                    db.SaveChanges();

                    if (skuList.Item.Any(i => i.Categories[0] == null)) MissCategory.AddRange(skuList.Item.Where(i => i.Categories[0] == null).Select(i => i.SKU).ToArray());

                    skuList = neto.GetItemBySkus(page++, limit);
                } while (skuList.Ack == NetoDeveloper.GetItemResponseAck.Success && skuList.Item.Any());
            }
            catch (Exception e)
            {
                string msg = e.Message;
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

        public void SkuUpdate(string skuID)
        {
            NetoApi neto = new NetoApi();
            string LangID = EnumData.DataLangList().First().Key;

            var sku = db.SKU.Find(skuID);
            var skuLang = sku.SkuLang.First(l => l.LangID.Equals(sku.SkuLang.Any(ll => ll.LangID.Equals(LangID)) ? LangID : sku.SkuLang.First().LangID));
            var skuType = sku.SkuType;
            var eBayTitle = !string.IsNullOrEmpty(sku.eBayTitle) ? Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, string>>(sku.eBayTitle) : new Dictionary<int, string> { };

            var netoSku = neto.GetItemBySku(skuID).Item.First();
            var categories = (NetoDeveloper.GetItemResponseItemCategory[])netoSku.Categories.First().Category;
            var update = new NetoDeveloper.UpdateItemItem()
            {
                Active = Convert.ToBoolean(sku.Status),
                ActiveSpecified = true,
                Approved = netoSku.Approved,
                ApprovedSpecified = netoSku.ApprovedSpecified,
                InventoryID = netoSku.InventoryID,
                SKU = sku.SkuID,
                Brand = sku.GetBrand.Name,
                Categories = categories.Select(c => new NetoDeveloper.UpdateItemItemCategory() { CategoryID = c.CategoryID, Priority = c.Priority }).ToArray(),
                CostPrice = 0,
                DefaultPrice = 0,
                PromotionPrice = 0,
                Name = skuLang.Name,
                Model = skuLang.Name,
                ModelNumber = skuLang.Model,
                Description = skuLang.Description,
                Specifications = skuLang.SpecContent,
                ParentSKU = sku.ParentSku,
                UPC = sku.UPC,
                Type = skuType.SkuTypeLang.First(l => l.LangID.Equals(skuType.SkuTypeLang.Any(ll => ll.LangID.Equals(LangID)) ? LangID : sku.SkuLang.First().LangID)).Name,

                Misc01 = skuLang.PackageContent,
                Misc02 = eBayTitle.ContainsKey(2) ? eBayTitle[2] : "",
                Misc19 = eBayTitle.ContainsKey(19) ? eBayTitle[19] : "",
                Misc50 = eBayTitle.ContainsKey(50) ? eBayTitle[50] : "",
                Misc24 = eBayTitle.ContainsKey(24) ? eBayTitle[24] : "",
                Misc25 = eBayTitle.ContainsKey(25) ? eBayTitle[25] : "",
                Misc23 = eBayTitle.ContainsKey(23) ? eBayTitle[23] : "",
                Misc21 = eBayTitle.ContainsKey(21) ? eBayTitle[21] : "",
                Misc20 = eBayTitle.ContainsKey(20) ? eBayTitle[20] : "",
                Misc22 = eBayTitle.ContainsKey(22) ? eBayTitle[22] : ""
            };
        }

        public void GetCustomer()
        {
            NetoApi neto = new NetoApi();
            var customerList = neto.GetCustomer();
        }
    }
}