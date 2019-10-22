using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using Newtonsoft.Json;

namespace PurchaseOrderSys.Models
{
    public class Common
    {
        protected PurchaseOrderEntities dbC = new PurchaseOrderEntities();

        protected static string ApiUserName = "test@qd.com.tw";
        protected static string ApiPassword = "prU$U9R7CHl3O#uXU6AcH6ch";

        protected readonly string AdminName = HttpContext.Current?.Session["AdminName"]?.ToString() ?? "System";

        private readonly string[] RequestUrl = new string[] { "http://internal.qd.com.tw/", "http://internal.qd.com.tw:8080/" };

        private bool disposedValue = false; // 偵測多餘的呼叫
        public Common()
        {
            //dbC.Configuration.AutoDetectChangesEnabled = false;
            //dbC.Configuration.LazyLoadingEnabled = false;
            //dbC.Configuration.ProxyCreationEnabled = false;
        }

        protected Response<T> Request<T>(string url, string method = "post", object data = null, int urlIndex = 0) where T : new()
        {
            Response<T> response = new Response<T>();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(RequestUrl[urlIndex] + url);
            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://localhost:49920/" + url);
            request.ContentType = "application/json";
            request.Method = method;
            request.ProtocolVersion = HttpVersion.Version10;

            if (data != null)
            {
                using (StreamWriter streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    var json = JsonConvert.SerializeObject(data);
                    streamWriter.Write(json);
                    streamWriter.Flush();
                }
            }

            HttpWebResponse httpResponse = (HttpWebResponse)request.GetResponse();
            using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                response = JsonConvert.DeserializeObject<Response<T>>(streamReader.ReadToEnd());
            }

            return response;
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
                dbC = null;
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