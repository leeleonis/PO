using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PurchaseOrderSys.Areas.SKUSystem.Models;

namespace PurchaseOrderSys.Areas.SKUSystem.Models
{
    public class StockKeepingUnit : IDisposable
    {
        protected InventorySKUEntities db = new InventorySKUEntities();

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
            skuData =db.SKU.Find(sku);
        }

        public string GetNewSku(int category, int brand)
        {
            int number = 1;
            string newNumber = "";

            try
            {
                if (db.SKU.Any(s => s.ID.StartsWith(category.ToString() + brand.ToString())))
                {
                    Sku lastest =db.SKU.AsNoTracking().Where(s => s.ID.StartsWith(category.ToString() + brand.ToString())).OrderByDescending(s => s.ID).First(s => s.ID.Length.Equals(9));
                    string oldNumber = string.Join("", lastest.ID.Skip(6).Take(3));
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

        public Sku CreateSku(Sku newSku, SkuLang newLang)
        {
            if (string.IsNullOrEmpty(newSku.ID)) newSku.ID = GetNewSku(newSku.Category, newSku.Brand);

            newLang.Sku = newSku.ID;
            newLang.CreateAt = newSku.CreateAt;
            newLang.CreateBy = newSku.CreateBy;

           db.SKU.Add(newSku);
            db.SkuLang.Add(newLang);

            if (newSku.Type.Equals((byte)EnumData.SkuType.Single) && newSku.Condition.Equals(1))
            {
                foreach (var condition in db.Condition.Where(c => c.IsEnable && !c.ID.Equals(newSku.Condition)).ToList())
                {
                    Sku sku_suffix = new Sku()
                    {
                        IsEnable = true,
                        ID = newSku.ID + condition.Suffix,
                        Type = (byte)EnumData.SkuType.Single,
                        Condition = condition.ID,
                        Category = newSku.Category,
                        Brand = newSku.Brand,
                        Status = (byte)EnumData.SkuStatus.Inactive,
                        CreateAt = newSku.CreateAt,
                        CreateBy = newSku.CreateBy
                    };

                   db.SKU.Add(sku_suffix);
                    db.SkuLang.Add(new SkuLang()
                    {
                        Sku = sku_suffix.ID,
                        LangID = newLang.LangID,
                        Name = newLang.Name,
                        CreateAt = newLang.CreateAt,
                        CreateBy = newLang.CreateBy
                    });
                }
            }

            db.SaveChanges();

            return newSku;
        }

        public List<SKU> GetVariationSku()
        {
            try
            {
                if (skuData == null) throw new Exception("No Sku data!");

                return db.SKU.Where(s => s.IsEnable && s.ParentSku.Equals(skuData.ID)).OrderBy(s => s.ID).ToList();
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