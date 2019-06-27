using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace PurchaseOrderSys.NewApi
{
    #region 回傳資料

   
    /// <summary>
    /// 回傳資料
    /// </summary>
    public class Received
    {
        public string code { get; set; }
        public dynamic data { get; set; }
        public string msg { get; set; }

    }
    #endregion
    public class Token
    {
        public string action { get; set; }
        public string app_key { get; set; }
    }
    public class getToken : Token
    {
        public getTokendata data { get; set; }

    }
    public class getTokendata
    {
        public string userName { get; set; }
        public string passWord { get; set; }
    }

    public class TokenSign : Token
    {
        public string format { get; set; }
        public string language { get; set; }
        public string platform { get; set; }
        public string sign { get; set; }
        public string sign_method { get; set; }
        public string timestamp { get; set; }
        public string version { get; set; }
    }




    public class QueryToken : TokenSign
    {
        public object data { get; set; }
    }
    public class warehouseData
    {
        public string warehouseCode { get; set; }
        public string warehouseName { get; set; }
        public string warehouseID { get; set; }
        public string warehouseAddress { get; set; }
    }
    public class Winit_APIToken : IDisposable
    {
        private bool disposed = false;

        private string api_url = "http://openapi.winit.com.cn/openapi/service";
        private string action = "getToken";
        private string api_key = "system@qd.com.tw";//"peter0626@hotmail.com";
        private string api_userName = "system@qd.com.tw"; //"peter0626@hotmail.com";
        private string api_password = "rqOHRYOqSA%b22%g6tzb"; //"gubu67qaP5e$ra*t";
        private string api_token = "";
        private string api_version = "2.0";
        private string client_secret = "";

        public Winit_APIToken()
        {
            var token = new getToken();
            token.action = action;
            token.app_key = api_key;
            token.data = new getTokendata { userName = api_userName, passWord = api_password };
            var refjson = req<Received>(api_url, token);
            if (refjson.code.Equals("0"))
            {
                api_token = refjson.data;//取token
            }
        }
        //public Received Received <T>(string action, object data) where T : new()
        //{
        //    var request = _RequestInit<T>(action, JsonConvert.SerializeObject(data));
        //    return null;
        //}

        /// <summary>
        /// 取倉庫資料
        /// </summary>
        /// <returns></returns>
        public List<warehouseData> WarehouseDataList()
        {
            QueryToken request = _RequestInit<QueryToken>("queryWarehouse", JsonConvert.SerializeObject(new { }));
            request.data = new { };

            var result = req<Received>(api_url, request);
            if (result.code.Equals("0"))
            {
               return result.data.ToObject<List<warehouseData>>();
             
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 倉庫列表
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> Warehouse3P()
        {
            var Warehouse3PList = new List<SelectListItem>();

            var WarehouseDataLists = WarehouseDataList();
            if (WarehouseDataLists.Any())
            {
                Warehouse3PList = WarehouseDataLists.Select(x => new SelectListItem { Text = x.warehouseName, Value = x.warehouseID }).ToList();

            }
            return Warehouse3PList;
            // 
        }
        private string _Get_UserSign(string actionValue, string data, string timestamp)
        {
            string sign = "";
            MD5 md5 = MD5.Create();

            string format = "json", platform = "SELLERERP", sign_method = "md5", version = api_version;
            var combine = api_token + "action" + action + "app_key" + api_key + "data" + data + "format" + format + "platform" + platform + "sign_method" + sign_method + "timestamp" + timestamp + "version" + version + api_token;
            byte[] Original = Encoding.ASCII.GetBytes(combine); //將字串來源轉為Byte[] 
            byte[] Change = md5.ComputeHash(Original);
            String a = Convert.ToBase64String(Change);

            for (int i = 0; i < Change.Length; i++)
            {
                sign = sign + Change[i].ToString("X2");
            }

            return sign;
        }
        private string _Get_UserClient_sign(string action, string data, string timestamp)
        {
            string sign = "";
            MD5 md5 = MD5.Create();

            string formatValue = "json", platformValue = "SELLERERP", sign_methodValue = "md5", versionValue = api_version;
            var combine = client_secret + "action" + action + "app_key" + api_key + "data" + data + "format" + formatValue + "platform" + platformValue + "sign_method" + sign_methodValue + "timestamp" + timestamp + "version" + versionValue + client_secret;
            byte[] Original = Encoding.ASCII.GetBytes(combine); //將字串來源轉為Byte[] 
            byte[] Change = md5.ComputeHash(Original);
            String a = Convert.ToBase64String(Change);

            for (int i = 0; i < Change.Length; i++)
            {
                sign = sign + Change[i].ToString("X2");
            }

            return sign;
        }
        private T _RequestInit<T>(string action, string data) where T : TokenSign, new()
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");

            T request = new T()
            {
                action = action,
                app_key = api_key,
                format = "json",
                language = "zh_CN",
                platform = "SELLERERP",
                sign = _Get_UserSign(action, data, timestamp),
                sign_method = "md5",
                timestamp = timestamp,
                version = api_version
            };

            return request;
        }


        /// <summary>
        /// 做HTTP Request
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="targetUrl">API URL</param>
        /// <param name="token">參數</param>
        /// <returns></returns>
        public static T req<T>(string targetUrl, object token)
        {
            string json = JsonConvert.SerializeObject(token);
            byte[] postData = Encoding.UTF8.GetBytes(json);
            string result = "";

            HttpWebRequest request = HttpWebRequest.Create(targetUrl) as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = "application/JSON";
            // request.Timeout = 30000;
            request.ContentLength = postData.Length;
            // 寫入 Post Body Message 資料流
            using (Stream st = request.GetRequestStream())
            {
                st.Write(postData, 0, postData.Length);
            }
            // 取得回應資料
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                {
                    result = sr.ReadToEnd();
                }
            }
            return JsonConvert.DeserializeObject<T>(result);

        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;

            if (disposing)
            {
            }
            disposed = true;
        }
        /// <summary>
        /// 查询商品
        /// </summary>
        /// <returns></returns>
        public List<warehouseData> SKUList()
        {
            QueryToken request = _RequestInit<QueryToken>("winit.mms.item.list", JsonConvert.SerializeObject(new { }));
            request.data = new { pageNo = 1, pageSize = 10, skuCode = "" };

            var result = req<Received>(api_url, request);
            if (result.code.Equals("0"))
            {
                return result.data.ToObject<List<warehouseData>>();

            }
            else
            {
                return null;
            }
        }

    }
}