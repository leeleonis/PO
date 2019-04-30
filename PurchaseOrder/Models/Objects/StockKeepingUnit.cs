using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using inventorySKU.NetoDeveloper;
using NetoDeveloper;
using PurchaseOrderSys.SCService;
using SellerCloud_WebService;

namespace PurchaseOrderSys.Models
{
    public class StockKeepingUnit : IDisposable
    {
        protected PurchaseOrderEntities db = new PurchaseOrderEntities();

        protected SKU skuData;
        protected NetoApi netoApi;

        public SC_WebService SC_Api;

        #region IDisposable Support
        private bool disposedValue = false; // 偵測多餘的呼叫

        public StockKeepingUnit() { }

        public StockKeepingUnit(string sku)
        {
            if (!string.IsNullOrEmpty(sku)) SetSkuData(sku);
        }

        public StockKeepingUnit(SKU sku)
        {
            SetSkuData(sku.SkuID);
        }

        public void SetSkuData(string sku)
        {
            skuData = db.SKU.Find(sku);
        }

        /// <summary>
        /// 自動產生新Sku
        /// </summary>
        /// <param name="category">品項</param>
        /// <param name="brand">品牌</param>
        /// <returns></returns>
        public string GetNewSku(int category, int brand)
        {
            int number = 1;
            string newNumber = "";

            try
            {
                if (db.SKU.Any(s => s.SkuID.StartsWith(category.ToString() + brand.ToString())))
                {
                    SKU lastest = db.SKU.AsNoTracking().Where(s => s.SkuID.StartsWith(category.ToString() + brand.ToString())).OrderByDescending(s => s.SkuID).First(s => s.SkuID.Length.Equals(9));
                    string oldNumber = string.Join("", lastest.SkuID.Skip(6).Take(3));
                    number = int.Parse(oldNumber, System.Globalization.NumberStyles.HexNumber) + 1;
                }

                newNumber = string.Format("{0:X}", number).PadLeft(3, '0');
            }
            catch (Exception e)
            {
                string errorMsg = e.InnerException != null && string.IsNullOrEmpty(e.InnerException.Message) ? e.InnerException.Message : e.Message;
                throw new Exception(string.Format("建立新Sku失敗 - {0}", errorMsg));
            }

            return string.Format("{0}{1}{2}", category, brand, newNumber);
        }

        /// <summary>
        /// 建立新Sku
        /// </summary>
        /// <param name="newSku">基本資料</param>
        /// <param name="newLang">語系資料</param>
        /// <returns></returns>
        public SKU CreateSku(SKU newSku, SkuLang newLang)
        {
            if (string.IsNullOrEmpty(newSku.SkuID)) newSku.SkuID = GetNewSku(newSku.Category, newSku.Brand);

            newSku.SkuID = newSku.SkuID.Trim();
            newLang.Sku = newSku.SkuID;
            newLang.CreateAt = newSku.CreateAt;
            newLang.CreateBy = newSku.CreateBy;

            newSku.SkuLang.Add(newLang);
            db.SKU.Add(newSku);

            if (newSku.Type.Equals((byte)EnumData.SkuType.Single) && newSku.Condition.Equals(1))
            {
                foreach (var condition in db.Condition.Where(c => c.IsEnable && !c.ID.Equals(newSku.Condition)).ToList())
                {
                    SKU sku_suffix = SkuInherit(newSku, newSku.SkuID + condition.Suffix, (byte)EnumData.SkuType.Single);
                    sku_suffix.Condition = condition.ID;
                    sku_suffix.ParentShadow = newSku.SkuID;
                    sku_suffix.CreateAt = newSku.CreateAt;
                    sku_suffix.CreateBy = newSku.CreateBy;

                    sku_suffix.SkuLang.Add(new SkuLang()
                    {
                        Sku = sku_suffix.SkuID,
                        LangID = newLang.LangID,
                        Name = newLang.Name,
                        Model = newLang.Model,
                        CreateAt = newLang.CreateAt,
                        CreateBy = newLang.CreateBy
                    });

                    db.SKU.Add(sku_suffix);
                }
            }

            db.SaveChanges();
            skuData = newSku;

            return newSku;
        }
        /// <summary>
        /// 建立Sku的Variation
        /// </summary>
        /// <param name="parentSku">父類品號資料</param>
        /// <returns></returns>
        private SKU CreateSkuVariation(SKU parentSku)
        {
            SKU shadow = SkuInherit(parentSku, parentSku.SkuID + "_var", (byte)EnumData.SkuType.Shadow);
            shadow.ParentShadow = parentSku.SkuID;
            shadow.CreateAt = parentSku.UpdateAt.Value;
            shadow.CreateBy = parentSku.UpdateBy;

            foreach (var lang in parentSku.SkuLang)
            {
                shadow.SkuLang.Add(new SkuLang()
                {
                    Sku = shadow.SkuID,
                    LangID = lang.LangID,
                    Name = lang.Name,
                    Model = lang.Model,
                    CreateAt = shadow.CreateAt,
                    CreateBy = shadow.CreateBy
                });
            }

            db.SKU.Add(shadow);
            db.SaveChanges();

            return shadow;
        }

        public List<SKU> GetVariationSku()
        {
            try
            {
                if (skuData == null) throw new Exception("No Sku data!");

                return db.SKU.Where(s => s.IsEnable && s.ParentSku.Equals(skuData.SkuID)).OrderBy(s => s.SkuID).ToList();
            }
            catch (Exception e)
            {
                string errorMsg = e.InnerException != null && string.IsNullOrEmpty(e.InnerException.Message) ? e.InnerException.Message : e.Message;
                throw new Exception(string.Format("取得Variation Sku失敗 - {0}", errorMsg));
            }
        }

        public List<Sku_Attribute> CompareVariationSku(List<SKU> SkuList = null)
        {
            List<Sku_Attribute> diverseList = new List<Sku_Attribute>();

            try
            {
                if (skuData == null) throw new Exception("No Sku data!");

                if (SkuList == null) SkuList = GetVariationSku();

                if (SkuList.Any())
                {
                    int[] diverseAttribute = skuData.Sku_Attribute.Where(a => a.IsDiverse).Select(a => a.AttrID).Distinct().ToArray();

                    if (!diverseAttribute.Any()) diverseAttribute = DiverseAttribute(SkuList.SelectMany(s => s.Sku_Attribute).ToList());

                    if (diverseAttribute.Any())
                    {
                        diverseList = SkuList.SelectMany(s => s.Sku_Attribute).Where(a => diverseAttribute.Contains(a.AttrID)).ToList();
                    }
                }
            }
            catch (Exception e)
            {
                string errorMsg = e.InnerException != null && string.IsNullOrEmpty(e.InnerException.Message) ? e.InnerException.Message : e.Message;
                throw new Exception(string.Format("取得Variation Sku失敗 - {0}", errorMsg));
            }

            return diverseList;
        }

        public int[] DiverseAttribute(List<Sku_Attribute> AttrList)
        {
            List<int> attrIDs = new List<int>();

            foreach (var group in AttrList.GroupBy(a => a.AttrID))
            {
                string value = group.First().Value;
                if (!group.All(a => !string.IsNullOrEmpty(a.Value) && a.Value.Equals(value)))
                {
                    attrIDs.Add(group.Key);
                }
            }

            return attrIDs.ToArray(); ;
        }

        /// <summary>
        /// 繼承Parent Sku的資料
        /// </summary>
        /// <param name="parentSku">父類品號資料</param>
        /// <param name="sku">品號</param>
        /// <param name="type">品號類型</param>
        /// <returns></returns>
        public SKU SkuInherit(SKU parentSku, string sku, byte type)
        {
            return new SKU()
            {
                IsEnable = true,
                SkuID = sku,
                Company = parentSku.Company,
                Type = type,
                Category = parentSku.Category,
                Brand = parentSku.Brand,
                Condition = parentSku.Condition,
                EAN = parentSku.EAN,
                UPC = parentSku.UPC,
                Status = (byte)EnumData.SkuStatus.Inactive
            };
        }

        public UpdateItemResponseItem UpdateSkuToNeto()
        {
            if (netoApi == null) netoApi = new NetoApi();
            string LangID = EnumData.DataLangList().First().Key;

            if (!netoApi.GetItemBySku(skuData.SkuID).Item.Any())
                CreateSkuToNeto();

            var skuLang = skuData.SkuLang.First(l => l.LangID.Equals(LangID));
            var eBayTitle = !string.IsNullOrEmpty(skuData.eBayTitle) ? Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, string>>(skuData.eBayTitle) : new Dictionary<int, string> { };

            var update = new UpdateItemItem()
            {
                SKU = skuData.SkuID,
                Brand = skuData.GetBrand.Name,
                Name = skuLang.Name,
                Active = Convert.ToBoolean(skuData.Status),
                ActiveSpecified = true,
                Description = skuLang.Description,
                Specifications = skuLang.SpecContent,
                ModelNumber = skuLang.Model,
                UPC = skuData.UPC,
                Type = skuData.SkuType.SkuTypeLang.FirstOrDefault(l => l.LangID.Equals(LangID))?.Name ?? "",
                Misc01 = skuLang.PackageContent,

                Misc02 = eBayTitle.ContainsKey(2) ? eBayTitle[2] : "",
                Misc19 = eBayTitle.ContainsKey(19) ? eBayTitle[19] : "",
                Misc50 = eBayTitle.ContainsKey(50) ? eBayTitle[50] : "",
                Misc24 = eBayTitle.ContainsKey(24) ? eBayTitle[24] : "",
                Misc25 = eBayTitle.ContainsKey(25) ? eBayTitle[25] : "",
                Misc23 = eBayTitle.ContainsKey(23) ? eBayTitle[23] : "",
                Misc21 = eBayTitle.ContainsKey(21) ? eBayTitle[21] : "",
                Misc20 = eBayTitle.ContainsKey(20) ? eBayTitle[20] : "",
                Misc22 = eBayTitle.ContainsKey(22) ? eBayTitle[22] : "",

                PriceGroups = skuData.PriceGroup.Where(p => p.GetMarket.NetoGroup.HasValue)
                    .Select(p => new UpdateItemItemPriceGroup() { Group = p.GetMarket.GetNetoGroup.Name, Price = p.Price, PriceSpecified = true, MaximumQuantity = p.Max.ToString(), MinimumQuantity = p.Min.ToString() }).ToArray(),
                Categories = new UpdateItemItemCategory[] { new UpdateItemItemCategory() { CategoryID = skuData.SkuType.NetoID.Value.ToString() } },
                ItemSpecifics = skuData.Sku_Attribute.Where(a => skuData.Type.Equals((byte)EnumData.SkuType.Variation) ? !a.IsDiverse : true)
                    .Where(a => a.eBay).GroupBy(a => a.AttrID).Where(g => !g.Any(a => a.LangID.Equals(LangID) && !a.eBay)).Select(g => g.First(a => a.LangID.Equals(LangID)))
                    .Select(g => new UpdateItemItemItemSpecific() { Name = g.SkuAttribute.SkuAttributeLang.First(l => l.LangID.Equals(LangID)).Name, Value = g.Value }).ToArray()
            };

            var result = netoApi.UpdateItem(update);
            if (result.Ack.Equals(UpdateItemResponseAck.Success))
            {
                return result.Item[0];
            }

            if (result.Messages.Error.Any())
            {
                throw new Exception("Error:" + string.Join(",", result.Messages.Error.Select(e => e.Message + "-" + e.Description).ToArray()));
            }
            else
            {
                throw new Exception("Waring:" + string.Join(",", result.Messages.Warning.Select(e => e.Message).ToArray()));
            }
        }

        public void UpdateSkuToSC()
        {
            if (SC_Api == null) SC_Api = new SC_WebService("tim@weypro.com", "timfromweypro");
            string LangID = EnumData.DataLangList().First().Key;

            try
            {
                if (SC_Api.Get_ProductFullInfo(skuData.SkuID) == null)
                    CreateSkuToSC();

                var skuLang = skuData.SkuLang.First(l => l.LangID.Equals(LangID));
                ProductFullInfo newSku = new ProductFullInfo()
                {
                    ID = skuData.SkuID,
                    CompanyID = skuData.Company,
                    ProductName = skuLang.Name,
                    ProductTypeID = skuData.SkuType.SCID.Value,
                    ManufacturerID = skuData.GetBrand.SCID.Value,
                    UPC = skuData.UPC
                };

                bool result = SC_Api.Update_ProductFullInfo(newSku);
                if (skuData.Type.Equals((byte)EnumData.SkuType.Single) && result)
                {
                    foreach (var company in db.Company.Where(c => c.ParentID.Value.Equals(skuData.Company)))
                    {
                        result = result && SC_Api.Update_ProductFullInfo(new ProductFullInfo()
                        {
                            ID = skuData.SkuID + company.ShandowSuffix,
                            CompanyID = company.ID,
                            ProductName = skuLang.Name,
                            ProductTypeID = skuData.SkuType.SCID.Value,
                            ManufacturerID = skuData.GetBrand.SCID.Value,
                            UPC = skuData.UPC
                        });
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("SC Error:" + e.InnerException?.Message ?? e.Message);
            }
        }

        public void UpdateVariationToNeto()
        {
            if (netoApi == null) netoApi = new NetoApi();

            var parentSku = skuData;
            foreach (var variationSku in db.SKU.Where(s => s.IsEnable && s.ParentSku.Equals(parentSku.SkuID)))
            {
                var response = netoApi.GetItemBySku(variationSku.SkuID);
                skuData = variationSku;
                //if (!response.Item.Any())
                //{
                //    CreateSkuToNeto();
                //}
                //UpdateSkuToNeto();
            }
            skuData = parentSku;
        }

        public AddItemResponseItem CreateSkuToNeto()
        {
            if (netoApi == null) netoApi = new NetoApi();
            string LangID = EnumData.DataLangList().First().Key;

            if (!skuData.SkuType.NetoID.HasValue)
                throw new Exception("Neto Error: SKU的Category尚未設定NetoID!");

            var skuLang = skuData.SkuLang.First(l => l.LangID.Equals(LangID));

            AddItemItem newItem = new AddItemItem()
            {
                SKU = skuData.SkuID,
                Name = skuLang.Name,
                Brand = skuData.GetBrand.Name,
                ModelNumber = skuLang.Model,
                UPC = skuData.UPC,
                UPC1 = skuData.EAN,
                Type = skuData.SkuType.SkuTypeLang.FirstOrDefault(l => l.LangID.Equals(LangID))?.Name ?? "",
                Categories = new AddItemItemCategory[] { new AddItemItemCategory() { CategoryID = skuData.SkuType.NetoID.Value.ToString() } }
            };

            var result = netoApi.AddItem(newItem);
            if (result.Ack.Equals(AddItemResponseAck.Success))
            {
                if (skuData.Type.Equals((byte)EnumData.SkuType.Single) && skuData.Condition.Equals(1))
                {
                    foreach (var condition in db.Condition.Where(c => c.IsEnable && !c.ID.Equals(skuData.Condition)).ToList())
                    {
                        SetSkuData(skuData.SkuID + condition.Suffix);
                        CreateSkuToNeto();
                    }
                }
                return result.Item[0];
            }

            if (result.Messages.Error.Any())
            {
                throw new Exception("Neto Error:" + string.Join(",", result.Messages.Error.Select(e => e.Message + "-" + e.Description).ToArray()));
            }
            else
            {
                throw new Exception("Neto Waring:" + string.Join(",", result.Messages.Warning.Select(e => e.Message).ToArray()));
            }
        }

        public void CreateSkuToSC()
        {
            if (SC_Api == null) SC_Api = new SC_WebService("tim@weypro.com", "timfromweypro");
            string LangID = EnumData.DataLangList().First().Key;

            try
            {
                if (!skuData.SkuType.SCID.HasValue)
                    throw new Exception("SKU的Category尚未設定SCID!");

                if (!skuData.GetBrand.SCID.HasValue)
                    throw new Exception("SKU的Brand尚未設定SCID!");

                var skuLang = skuData.SkuLang.First(l => l.LangID.Equals(LangID));
                ProductFullInfo newSku = new ProductFullInfo()
                {
                    ID = skuData.SkuID,
                    CompanyID = skuData.Company,
                    ProductName = skuLang.Name,
                    ProductTypeID = skuData.SkuType.SCID.Value,
                    ManufacturerID = skuData.GetBrand.SCID.Value,
                    UPC = skuData.UPC
                };

                bool result = SC_Api.Create_ProductFullInfo(newSku);
                if (skuData.Type.Equals((byte)EnumData.SkuType.Single) && result)
                {
                    foreach (var company in db.Company.Where(c => c.ParentID.Value.Equals(skuData.Company)))
                    {
                        result = result && SC_Api.Create_ProductFullInfo(new ProductFullInfo()
                        {
                            ID = newSku.ID + company.ShandowSuffix,
                            CompanyID = company.ID,
                            ProductName = skuLang.Name,
                            ProductTypeID = newSku.ProductTypeID,
                            ManufacturerID = newSku.ManufacturerID,
                            UPC = skuData.UPC
                        });
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("SC Error:" + e.InnerException?.Message ?? e.Message);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 處置受控狀態 (受控物件)。
                }

                // TODO: 釋放非受控資源 (非受控物件) 並覆寫下方的完成項。
                // TODO: 將大型欄位設為 null。

                db = null;
                skuData = null;
                netoApi = null;
                SC_Api = null;

                disposedValue = true;
            }
        }

        // TODO: 僅當上方的 Dispose(bool disposing) 具有會釋放非受控資源的程式碼時，才覆寫完成項。
        // ~StockKeepingUnit() {
        //   // 請勿變更這個程式碼。請將清除程式碼放入上方的 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 加入這個程式碼的目的在正確實作可處置的模式。
        public void Dispose()
        {
            // 請勿變更這個程式碼。請將清除程式碼放入上方的 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果上方的完成項已被覆寫，即取消下行的註解狀態。
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}