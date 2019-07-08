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
    public class clientSign : Token
    {
        public string client_id { get; set; }
        public string client_sign { get; set; }
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
    public class Queryclient : clientSign
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
    #region Winit SKU Data
    public class PageParams
    {
        public int pageNo { get; set; }
        public int pageSize { get; set; }
        public int totalCount { get; set; }
    }

    public class CustomsDeclarationList
    {
        public string countryCode { get; set; }
        public string declareName { get; set; }
        public decimal? importPrice { get; set; }
        public decimal? exportPrice { get; set; }
        public decimal? rebateRate { get; set; }
        public decimal? importRate { get; set; }
        public string firstWayType { get; set; }
        public decimal? vatRate { get; set; }
        public decimal? length { get; set; }
        public decimal? width { get; set; }
        public decimal? height { get; set; }
        public decimal? volume { get; set; }
        public decimal? weight { get; set; }
        public decimal? registerWeight { get; set; }
        public decimal? registerLength { get; set; }
        public decimal? registerWidth { get; set; }
        public decimal? registerHeight { get; set; }
        public decimal? registerVolume { get; set; }
        public object recommendDeclarePrice { get; set; }
        public string isNew { get; set; }
        public string cargoTypeSpec { get; set; }
        public object qty { get; set; }
    }

    public class WinitSKUList
    {
        public object id { get; set; }
        public string code { get; set; }
        public string skuCode { get; set; }
        public string specification { get; set; }
        public string cnName { get; set; }
        public string name { get; set; }
        public decimal? length { get; set; }
        public decimal? width { get; set; }
        public decimal? height { get; set; }
        public decimal? volume { get; set; }
        public decimal? weight { get; set; }
        public decimal? registerWeight { get; set; }
        public decimal? registerLength { get; set; }
        public decimal? registerWidth { get; set; }
        public decimal? registerHeight { get; set; }
        public decimal? registerVolume { get; set; }
        public string supervisorMode { get; set; }
        public string brandName { get; set; }
        public string model { get; set; }
        public string isPlus { get; set; }
        public string isBattery { get; set; }
        public string displayPageUrl { get; set; }
        public string isActive { get; set; }
        public string sourceType { get; set; }
        public List<CustomsDeclarationList> customsDeclarationList { get; set; }
        public object itemThirdVos { get; set; }
    }

    public class WinitSKU
    {
        public PageParams pageParams { get; set; }
        public List<WinitSKUList> list { get; set; }
    }
    #endregion
    #region Wint 標籤資料
    public class SingleItem
    {
        public string productCode { get; set; }
        public string specification { get; set; }
        public int? printQty { get; set; }
    }

    public class PostPrintV2Data
    {
        public List<SingleItem> singleItems { get; set; }
        public string labelType { get; set; }
        public string madeIn { get; set; }
    }
    public class ReturnPrintV2
    {
        public string formatType { get; set; }
        public string itemBarcodeFile { get; set; }
        public List<string> itemBarcodeList { get; set; }
    }
    #endregion
    public class Winit_API : IDisposable
    {
        private bool disposed = false;

        private string api_url = "http://openapi.winit.com.cn/openapi/service";
        //private string action = "getToken";
        private string api_key = "peter0626@hotmail.com";//"peter0626@hotmail.com";
        //private string api_userName = "system@qd.com.tw"; //"peter0626@hotmail.com";
        //private string api_password = "W7oBeN3Vu!rL_rU-ITH-"; //"gubu67qaP5e$ra*t";
        private string api_token = "54F9B02195FCDFE9B2E80341402C9BDD";
        private string api_version = "1.0";
        private string client_id = "NTU5YTUYZWITYZA3ZI00YWVKLTLKNWUTZJZLZDLIMMEXYTU2";
        private string client_secret = "NTJIZWUZNZITNGVINI00NZNHLWFIYZITNWQ4YZEWZWFHM2U5MZG4NDG2ODC5NZAXMZC3NG==";
        private string platformval = "testapi";
        //public Winit_APIToken()
        //{
        //    var token = new getToken();
        //    token.action = action;
        //    token.app_key = api_key;
        //    token.data = new getTokendata { userName = api_userName, passWord = api_password };
        //    var refjson = req<Received>(api_url, token);
        //    if (refjson.code.Equals("0"))
        //    {
        //        api_token = refjson.data;//取token
        //    }
        //}
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

            string format = "json", platform = platformval, sign_method = "md5", version = api_version;
            var combine = api_token + "action" + actionValue + "app_key" + api_key + "data" + data + "format" + format + "platform" + platform + "sign_method" + sign_method + "timestamp" + timestamp + "version" + version + api_token;
            byte[] Original = Encoding.ASCII.GetBytes(combine); //將字串來源轉為Byte[] 
            byte[] Change = md5.ComputeHash(Original);
            String a = Convert.ToBase64String(Change);

            for (int i = 0; i < Change.Length; i++)
            {
                sign = sign + Change[i].ToString("X2");
            }

            return sign;
        }
        private string _Get_UserClient_sign(string actionValue, string data, string timestamp)
        {
            string sign = "";
            MD5 md5 = MD5.Create();

            string formatValue = "json", platformValue = platformval, sign_methodValue = "md5", versionValue = api_version;
            var combine = client_secret + "action" + actionValue + "app_key" + api_key + "data" + data + "format" + formatValue + "platform" + platformValue + "sign_method" + sign_methodValue + "timestamp" + timestamp + "version" + versionValue + client_secret;
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
                platform = platformval,
                sign = _Get_UserSign(action, data, timestamp),
                sign_method = "md5",
                timestamp = timestamp,
                version = api_version
            };

            return request;
        }
        private T _RequestInitNew<T>(string action, object data) where T : Queryclient, new()
        {
            var dataVal = JsonConvert.SerializeObject(data);
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");

            T request = new T()
            {
                action = action,
                app_key = api_key,
                client_id = client_id,
                client_sign = _Get_UserClient_sign(action, dataVal, timestamp),
                data = data,
                format = "json",
                language = "zh_CN",
                platform = platformval,
                sign = _Get_UserSign(action, dataVal, timestamp),
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
        public WinitSKU SKUList(string skuCode)
        {
            var data = new { pageNo = 1, pageSize = 10, skuCode = skuCode };
            Queryclient request = _RequestInitNew<Queryclient>("winit.mms.item.list",data);
            request.data = data;

            var result = req<Received>(api_url, request);
            if (result.code.Equals("0"))
            {
                return result.data.ToObject<WinitSKU>();

            }
            else
            {
                return null;
            }
        }

        internal ReturnPrintV2 GetPrintV2(PostPrintV2Data postPrintV2Data)
        {
            Queryclient request = _RequestInitNew<Queryclient>("winit.singleitem.label.print.v2", postPrintV2Data);
            request.data = postPrintV2Data;

            var result = req<Received>(api_url, request);
            if (result.code.Equals("0"))
            {
                return result.data.ToObject<ReturnPrintV2>();

            }
            else
            {
                return null;
            }
        }
    }
}