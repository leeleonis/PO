using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PurchaseOrderSys.Models
{
    public class Common
    {
        protected static string ApiUserName = "test@qd.com.tw";
        protected static string ApiPassword = "prU$U9R7CHl3O#uXU6AcH6ch";
        protected PurchaseOrderEntities db = new PurchaseOrderEntities();

        protected readonly string AdminName = HttpContext.Current.Session["AdminName"]?.ToString() ?? "System";

        private bool disposedValue = false; // 偵測多餘的呼叫

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
                disposedValue = true;
            }
        }

        // TODO: 僅當上方的 Dispose(bool disposing) 具有會釋放非受控資源的程式碼時，才覆寫完成項。
        // 加入這個程式碼的目的在正確實作可處置的模式。
        public void Dispose()
        {
            // 請勿變更這個程式碼。請將清除程式碼放入上方的 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果上方的完成項已被覆寫，即取消下行的註解狀態。
            // GC.SuppressFinalize(this);
        }
    }

    public class Response<T>
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

        public Response()
        {
            Status = true;
            Message = null;
        }

        public void SetError(string msg)
        {
            Status = false;
            Message = msg;
        }
    }
}