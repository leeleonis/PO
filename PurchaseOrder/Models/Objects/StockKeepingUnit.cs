using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace PurchaseOrderSys.Models
{
    public class StockKeepingUnit : IDisposable
    {
        protected PurchaseOrderEntities db = new PurchaseOrderEntities();

        protected SKU skuData;

        #region IDisposable Support
        private bool disposedValue = false; // 偵測多餘的呼叫

        public StockKeepingUnit()
        {
        }

        public StockKeepingUnit(string sku)
        {
            if (!string.IsNullOrEmpty(sku)) SetSkuData(sku);
        }

        public void SetSkuData(string sku)
        {
            skuData = db.SKU.Find(sku);
        }

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

        public SKU CreateSku(SKU newSku, SkuLang newLang)
        {
            if (string.IsNullOrEmpty(newSku.SkuID)) newSku.SkuID = GetNewSku(newSku.Category, newSku.Brand);

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

            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {

            }

            return newSku;
        }

        public SKU CreateSkuShadow(SKU parentSku)
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

        public SKU SkuInherit(SKU parentSku, string sku, byte type)
        {
            return new SKU()
            {
                IsEnable = true,
                SkuID = sku,
                Type = type,
                ParentShadow = parentSku.SkuID,
                Category = parentSku.Category,
                Brand = parentSku.Brand,
                Condition = parentSku.Condition,
                EAN = parentSku.EAN,
                UPC = parentSku.UPC,
                Status = (byte)EnumData.SkuStatus.Inactive
            };
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