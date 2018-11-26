using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace inventorySKU.Models
{
    public class StockKeepingUnit : IDisposable
    {
        protected InventorySKUEntities db = new InventorySKUEntities();

        protected Sku skuData;

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
            skuData = db.Sku.Find(sku);
        }

        public string GetNewSku(int category, int brand)
        {
            int number = 1;
            string newNumber = "";

            if (db.Sku.Any(s => s.ID.StartsWith(category.ToString() + brand.ToString())))
            {
                Sku lastest = db.Sku.AsNoTracking().Where(s => s.ID.StartsWith(category.ToString() + brand.ToString())).OrderByDescending(s => s.ID).First(s => s.ID.Length.Equals(9));
                string oldNumber = string.Join("", lastest.ID.Skip(6).Take(3));
                number = int.Parse(oldNumber, System.Globalization.NumberStyles.HexNumber) + 1;
            }

            newNumber = string.Format("{0:X}", number).PadLeft(3, '0');

            return string.Format("{0}{1}{2}", category, brand, newNumber);
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