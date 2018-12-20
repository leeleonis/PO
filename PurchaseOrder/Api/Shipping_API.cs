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

    public class Shipping_API
    {
        private string api_url = "http://internal.qd.com.tw/ajax/shippingMethodData";
        public  string ShippingList()
        {
            //var optionType = new List<string>();// { "shippingMethod", "carrier", "methodType" , "boxType" };
            //string json = JsonConvert.SerializeObject(optionType);
            //byte[] postData = Encoding.UTF8.GetBytes(json);
            string result = "";
            HttpWebRequest request = HttpWebRequest.Create(api_url) as HttpWebRequest;
            request.Method = "GET";
            request.ContentType = "application/JSON";
            request.Timeout = 30000;
            //request.ContentLength = postData.Length;
            // 寫入 Post Body Message 資料流
            //using (Stream st = request.GetRequestStream())
            //{
            //    st.Write(postData, 0, postData.Length);
            //}
            // 取得回應資料
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                {
                    result = sr.ReadToEnd();
                }
            }
            return "";
        }
    }
}