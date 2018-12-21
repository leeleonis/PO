using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace PurchaseOrderSys.Api
{
    public class ShippingMethod
    {
        public string text { get; set; }
        public int value { get; set; }
    }

    public class Carrier
    {
        public string text { get; set; }
        public int value { get; set; }
        public string type { get; set; }
    }

    public class FedEx
    {
        public string text { get; set; }
        public int value { get; set; }
    }

    public class Winit
    {
        public string text { get; set; }
        public int value { get; set; }
    }

    public class ID
    {
        public string text { get; set; }
        public int value { get; set; }
    }

    public class MethodType
    {
        public List<FedEx> FedEx { get; set; }
        public object DHL { get; set; }
        public List<Winit> Winit { get; set; }
        public List<ID> IDS { get; set; }
    }

    public class ValueData
    {
        public string text { get; set; }
        public int value { get; set; }
    }

    public class BoxType
    {
        public List<ValueData> FedEx { get; set; }
    }

    public class Data
    {
        public List<ShippingMethod> shippingMethod { get; set; }
        public List<Carrier> carrier { get; set; }
        public MethodType methodType { get; set; }
        public BoxType boxType { get; set; }
    }

    public class RootObject
    {
        public bool status { get; set; }
        public object message { get; set; }
        public Data data { get; set; }
    }

    public class Shipping_API
    {
        private string api_url = "http://internal.qd.com.tw/shipping/getSelectOption";
        public RootObject ShippingList()
        {
            var optionType = new List<string> { "shippingMethod", "carrier", "methodType", "boxType" };
            string json = JsonConvert.SerializeObject(optionType);
            byte[] postData = Encoding.UTF8.GetBytes(json);
            string result = "";
            HttpWebRequest request = HttpWebRequest.Create(api_url) as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = "application/JSON";
            request.Timeout = 30000;
            request.ContentLength = postData.Length;
            //寫入 Post Body Message 資料流
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
            return JsonConvert.DeserializeObject<RootObject>(result);
        }
    }
}