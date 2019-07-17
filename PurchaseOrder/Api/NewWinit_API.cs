﻿using Newtonsoft.Json;
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
    #region 建立訂單
    /// <summary>
    /// 包裹中的商品信息
    /// </summary>
    public class MerchandiseList
    {
        /// <summary>
        /// 商品編碼
        /// </summary>
        public string merchandiseCode { get; set; }
        /// <summary>
        /// 數量
        /// </summary>
        public int quantity { get; set; }
        /// <summary>
        /// 商品規格 
        /// </summary>
        public string specification { get; set; }
    }
    /// <summary>
    /// 箱單信息
    /// </summary>
    public class PackageList
    {
        public List<MerchandiseList> merchandiseList { get; set; }
        /// <summary>
        /// 賣家箱號
        /// </summary>
        public string sellerCaseNo { get; set; }
        /// <summary>
        /// 包裹重量(KG) 
        /// </summary>
        public string sellerWeight { get; set; }
        /// <summary>
        /// 第三方箱號
        /// </summary>
        public string thirdPartyCaseNo { get; set; }
        /// <summary>
        /// 包裹高(CM) 
        /// </summary>
        public string sellerHeight { get; set; }
        /// <summary>
        /// 包裹長(CM) 
        /// </summary>
        public string sellerLength { get; set; }
        /// <summary>
        /// 包裹寬(CM) 
        /// </summary>
        public string sellerWidth { get; set; }
    }
    /// <summary>
    /// 送港信息
    /// </summary>
    public class SendPortInfo
    {
        /// <summary>
        /// 送港類型 WINIT - winit送港；SELF -自送港賣家直發/自驗情況適用
        /// </summary>
        public string sendPortType { get; set; }
        /// <summary>
        /// 提櫃地址編碼
        /// </summary>
        public string pickupCartonAddressCode { get; set; }
        /// <summary>
        /// 送港時間 格式yyyy-MM-dd
        /// </summary>
        public string sendPortDate { get; set; }
        /// <summary>
        /// 櫃號
        /// </summary>
        public string containerNo { get; set; }
        /// <summary>
        /// 鉛封號
        /// </summary>
        public string sealNo { get; set; }
    }
    /// <summary>
    /// 直發預報信息
    /// </summary>
    public class directForecastInfo
    {
        /// <summary>
        /// 預計離港時間 格式yyyy-MM-dd
        /// </summary>
        public string preparedOffPortDate { get; set; }
        /// <summary>
        /// 預計到港時間 格式yyyy-MM-dd
        /// </summary>
        public string preparedArrivePortDate { get; set; }
    }
    public class WinitCreateOrder
    {
        /// <summary>
        /// 訂單類型編碼 SD-標準海外倉入庫（Winit國內驗貨且走Winit頭程） DW-直發海外驗入庫(Winit海外驗貨，頭程僅限賣家直發） DI-直發國內驗入庫（Winit國內驗貨或者賣家自驗，頭程僅限賣家直發） 
        /// </summary>
        public string orderType { get; set; }
        /// <summary>
        /// Winit產品編碼
        /// </summary>
        public string winitProductCode { get; set; }
        /// <summary>
        /// 目的倉編碼
        /// </summary>
        public string destinationWarehouseCode { get; set; }
        /// <summary>
        /// 客戶訂單號
        /// </summary>
        public string sellerOrderNo { get; set; }
        /// <summary>
        /// 驗貨倉編碼
        /// </summary>
        public string inspectionWarehouseCode { get; set; }
        /// <summary>
        /// 進口報關規則編碼 
        /// </summary>
        public string importDeclarationRuleCode { get; set; }
        /// <summary>
        /// 進口商編碼 
        /// </summary>
        public string importerCode { get; set; }
        /// <summary>
        /// 箱單信息
        /// </summary>
        public List<PackageList> packageList { get; set; }
        /// <summary>
        /// 出口報關類型編碼
        /// </summary>
        public string exportDeclarationType { get; set; }
        /// <summary>
        /// 出口商編碼
        /// </summary>
        public string exporterCode { get; set; }
        /// <summary>
        /// 物流計劃編碼
        /// </summary>
        //public string logisticsPlanNo { get; set; }
        /// <summary>
        /// 送港信息
        /// </summary>
        //public SendPortInfo sendPortInfo { get; set; }
        public directForecastInfo directForecastInfo { get; set; }
    }
    public class RefWinitOrderNo
    {
        public string orderNo { get; set; }
    }
    #endregion

    /// <summary>
    /// 查詢頭程服務
    /// </summary>
    public class WinitProducts
    {
        /// <summary>
        /// Winit產品編碼
        /// </summary>
        public string productCode { get; set; }
        /// <summary>
        /// Winit產品名稱
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// Winit產品描述
        /// </summary>
        public string productName { get; set; }
    }
    /// <summary>
    /// 查詢驗貨/目的倉
    /// </summary>
    public class WinitWarehouse
    {
        /// <summary>
        /// 倉庫Code
        /// </summary>
        public string countryCode { get; set; }
        /// <summary>
        /// 倉庫名稱
        /// </summary>
        public string warehouseName { get; set; }
        /// <summary>
        /// 國家編碼
        /// </summary>
        public string warehouseCode { get; set; }
    }

    public class WinitWarehouseList
    {
        public List<WinitWarehouse> warehouseList { get; set; }
    }

    /// <summary>
    /// 查詢進口報關-IOR規則
    /// </summary>
    public class IORList
    {
        /// <summary>
        /// 進口報關規則編碼
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 進口報關規則名稱
        /// </summary>
        public string iorName { get; set; }
        /// <summary>
        /// IOR類型編碼進口報關類型: TIOR-第三方進口商 CIOR-自有進口商
        /// </summary>
        public string iorCode { get; set; }
        /// <summary>
        /// IOR類型名稱
        /// </summary>
        public string name { get; set; }
    }

    public class EorList
    {
        /// <summary>
        /// EOR類型編碼
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// EOR類型名稱 TEOR-第三方出口商 FEOR-金融出口商(需要開通才能使用，詳情可諮詢商務） CEOR-自有出口商
        /// </summary>
        public string name { get; set; }
    }
    #region 查詢物流計劃
    public class PlanList
    {
        /// <summary>
        /// 預計發貨時間
        /// </summary>
        public string planOutboudDate { get; set; }
        /// <summary>
        /// 物流計劃類型：sea-海運,air-空運
        /// </summary>
        public string planType { get; set; }
        /// <summary>
        /// 截止收貨時間
        /// </summary>
        public string pickupPackageDeadline { get; set; }
        /// <summary>
        /// 物流計劃名稱
        /// </summary>
        public string planName { get; set; }
        /// <summary>
        /// 截止下單時間
        /// </summary>
        public string createOrderDeadline { get; set; }
        /// <summary>
        /// 預計上架時間
        /// </summary>
        public string planShelfDate { get; set; }
        /// <summary>
        /// 物流計劃編碼
        /// </summary>
        public string planCode { get; set; }
    }

    public class LogisticsPlan
    {
        public List<PlanList> planList { get; set; }
    }
    #endregion
    /// <summary>
    /// 入庫可選商品
    /// </summary>
    public class AvailableMerchandise
    {
        /// <summary>
        /// 中文名稱
        /// </summary>
        public string merchandiseNameCn { get; set; }
        /// <summary>
        /// M碼
        /// </summary>
        public string winitMerchandiseCode { get; set; }
        /// <summary>
        /// 規格
        /// </summary>
        public string specification { get; set; }
        /// <summary>
        /// 名稱
        /// </summary>
        public string merchandiseName { get; set; }
        /// <summary>
        /// 商品編碼
        /// </summary>
        public string merchandiseCode { get; set; }
    }
    public class VendorInfo
    {
        /// <summary>
        /// 是否Winit供應商：Y-是,N-否
        /// </summary>
        public string isWinit { get; set; }
        /// <summary>
        /// 進口商名稱
        /// </summary>
        public string vendorName { get; set; }
        /// <summary>
        /// 進口商編碼
        /// </summary>
        public string vendorCode { get; set; }
    }
    #region 打印包裹標籤
    public class packageNoList
    {
        public string packageNo { get; set; }
        public string sellerCaseNo { get; set; }
    }
    public class PackageLabe
    {
        public string Label { get; set; }
    }
    #endregion
    #region 查詢入庫單（詳情）
    public class OrderDetailMerchandiseList
    {
        public int? quantity { get; set; }
        public int? actualQuantity { get; set; }
        public string specification { get; set; }
        public string sku { get; set; }
        public string merchandiseCode { get; set; }
    }

    public class OrderDetailPackageList
    {
        public string sellerCaseNo { get; set; }
        public List<OrderDetailMerchandiseList> merchandiseList { get; set; }
        public string thirdPartyCaseNo { get; set; }
        public int? sellerWeight { get; set; }
        public int? sellerLength { get; set; }
        public int? length { get; set; }
        public int? width { get; set; }
        public int? weight { get; set; }
        public int? sellerHeight { get; set; }
        public int? sellerWidth { get; set; }
        public string packageNo { get; set; }
        public int? height { get; set; }
    }

    public class OrderDetailMerchandiseList2
    {
        public int? inspectionQty { get; set; }
        public int? quantity { get; set; }
        public string productBarcode { get; set; }
        public int? actualQuantity { get; set; }
        public string specification { get; set; }
        public string merchandiseCode { get; set; }
        public List<string> skuCode3rdList { get; set; }
    }

    public class OrderDetail
    {
        public string orderType { get; set; }
        public string sellerOrderNo { get; set; }
        public string destinationWarehouseCode { get; set; }
        public string customsDeclarationName { get; set; }
        public string expressNo { get; set; }
        public string winitProductName { get; set; }
        public int? totalItemQty { get; set; }
        public string importDeclarationRuleName { get; set; }
        public int? totalMerchandiseQty { get; set; }
        public string needReservationSendWh { get; set; }
        public string expressVendorName { get; set; }
        public string inspectionWarehouseCode { get; set; }
        public string importerName { get; set; }
        public string importDeclarationType { get; set; }
        public object exporterCode { get; set; }
        public string orderNo { get; set; }
        public string importDeclareValueType { get; set; }
        public int? totalPackageQty { get; set; }
        public string inspectionWarehouseName { get; set; }
        public string inspectionType { get; set; }
        public string planShelfCompletedDate { get; set; }
        public string logisticsPlanName { get; set; }
        public string destinationWarehouseName { get; set; }
        public string expressVendorCode { get; set; }
        public string importDeclarationRuleCode { get; set; }
        public string importDeclareWay { get; set; }
        public string pickupType { get; set; }
        public List<OrderDetailPackageList> packageList { get; set; }
        public List<OrderDetailMerchandiseList2> merchandiseList { get; set; }
        public string importDeclarationName { get; set; }
        public string pickupAddressCode { get; set; }
        public string createdDate { get; set; }
        public int? logisticsPlanNo { get; set; }
        public string pickupAddress { get; set; }
        public string exporterName { get; set; }
        public string winitProductCode { get; set; }
        public string shelveCompletedDate { get; set; }
        public string customsDeclarationType { get; set; }
        public string importerCode { get; set; }
        public string status { get; set; }
    }
    #endregion
    #region WINIT建立商品
    public class ItemThirdVo
    {
        public string skuCodeThird { get; set; }
        public string description { get; set; }
    }

    public class ProductList
    {
        /// <summary>
        /// 客户注册商品时命名的商品编码，提交后不可修改
        /// </summary>
        public string productCode { get; set; }
        /// <summary>
        /// 中文名称
        /// </summary>
        public string chineseName { get; set; }
        /// <summary>
        /// 英文名称
        /// </summary>
        public string englishName { get; set; }
        /// <summary>
        /// 注册重量(克/g)
        /// </summary>
        public decimal registeredWeight { get; set; }
        /// <summary>
        /// 重量体积是否固定，Y|N（目前不可更改，默认为Y）
        /// </summary>
        public string fixedVolumeWeight { get; set; }
        /// <summary>
        /// 注册长度(cm)
        /// </summary>
        public double registeredLength { get; set; }
        /// <summary>
        /// 注册宽度(cm)
        /// </summary>
        public double registeredWidth { get; set; }
        /// <summary>
        /// 注册高度(cm)
        /// </summary>
        public double registeredHeight { get; set; }
        /// <summary>
        /// 是否有品牌，Y|N
        /// </summary>
        public string branded { get; set; }
        /// <summary>
        /// 品牌名称（当branded为Y时必填且可修改）
        /// </summary>
        public string brandedName { get; set; }
        /// <summary>
        /// 当brandedname为Y时为必填
        /// </summary>
        public string model { get; set; }
        /// <summary>
        /// 商品刊登链接
        /// </summary>
        public string displayPageUrl { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string remark { get; set; }
        /// <summary>
        /// 出口国家，国家2字码
        /// </summary>
        public string exportCountry { get; set; }
        /// <summary>
        /// 进口国家，国家2字码
        /// </summary>
        public string inporCountry { get; set; }
        /// <summary>
        /// 进口申报价（USD）
        /// </summary>
        public decimal inportDeclaredvalue { get; set; }
        /// <summary>
        /// 出口申报价（USD）
        /// </summary>
        public decimal exportDeclaredvalue { get; set; }
        /// <summary>
        /// 是否有电池，Y|N必选其一，默认为N
        /// </summary>
        public string battery { get; set; }
    }
    /// <summary>
    /// 產品列表
    /// </summary>
    public class RegisterProduct
    {
        public List<ProductList> productList { get; set; }
    }
    public class ReturnProduct
    {
        public string productCode { get; set; }
    }
    #endregion
    public class Winit_API : IDisposable
    {
        private bool disposed = false;

        private string api_url = "http://openapi.winit.com.cn/openapi/service";
        private string WinitAccountID = "1004244";
        //private string action = "getToken";
        private string api_key = "peter0626@hotmail.com";//"peter0626@hotmail.com";
        //private string api_userName = "system@qd.com.tw"; //"peter0626@hotmail.com";
        //private string api_password = "W7oBeN3Vu!rL_rU-ITH-"; //"gubu67qaP5e$ra*t";
        private string api_token = "54F9B02195FCDFE9B2E80341402C9BDD";
        private string api_version = "1.0";
        private string client_id = "NTU5YTUYZWITYZA3ZI00YWVKLTLKNWUTZJZLZDLIMMEXYTU2";
        private string client_secret = "NTJIZWUZNZITNGVINI00NZNHLWFIYZITNWQ4YZEWZWFHM2U5MZG4NDG2ODC5NZAXMZC3NG==";
        private string platformval = "testapi";
        public Received ResultError;
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
        internal T GetAPI<T>(string action, object data)
        {
            try
            {
                ResultError = null;
                Queryclient request = _RequestInitNew<Queryclient>(action, data);
                request.data = data;
                var json = JsonConvert.SerializeObject(request);
                var result = req<Received>(api_url, request);
                if (result.code.Equals("0"))
                {
                    return result.data.ToObject<T>();
                }
                else
                {
                    ResultError = result;
                    return default(T);
                }
            }
            catch (Exception ex)
            {
                ResultError = new Received { msg = ex.ToString() };
                return default(T);
            }

        }
        /// <summary>
        /// 查询商品
        /// </summary>
        /// <returns></returns>
        public WinitSKU SKUList(string skuCode)
        {
            var data = new { pageNo = 1, pageSize = 10, skuCode };
            return GetAPI<WinitSKU>("winit.mms.item.list", data);
        }

        internal ReturnPrintV2 GetPrintV2(PostPrintV2Data postPrintV2Data)
        {
            return GetAPI<ReturnPrintV2>("winit.singleitem.label.print.v2", postPrintV2Data);
        }

        internal List<WinitProducts> getWinitProducts(string productType)
        {
            var data = new { productType };
            return GetAPI<List<WinitProducts>>("winit.wh.pms.getWinitProducts", data);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="winitProductCode">Winit產品編碼</param>
        /// <param name="warehouseType">驗貨倉：INSJ  目的倉：DEST</param>
        /// <param name="orderType">訂單類型編碼 SD-標準海外倉入庫（Winit國內驗貨且走Winit頭程 DW-直發海外驗入庫(Winit海外驗貨，頭程僅限賣家直發）DI-直發國內驗入庫（Winit國內驗貨或者賣家自驗，頭程僅限賣家直發）</param>
        /// <param name="inspectionWarehouseCode">驗貨倉編碼,當warehouseType=DEST時，必填</param>
        /// <returns></returns>
        internal WinitWarehouseList getWarehouseList(string winitProductCode, string warehouseType, string orderType, string inspectionWarehouseCode)
        {
            var action = "winit.pms.getWarehouseList";
            return GetAPI<WinitWarehouseList>(action, new { winitProductCode, warehouseType, orderType, inspectionWarehouseCode });

        }
        /// <summary>
        /// 查詢進口報關-IOR規則
        /// </summary>
        /// <param name="endWarehouseCode">目的倉庫編碼</param>
        /// <param name="productCode">產品編碼</param>
        /// <returns></returns>
        internal List<IORList> IORList(string endWarehouseCode, string productCode)
        {
            return GetAPI<List<IORList>>("winit.ups.importDeclarationRule.queryList", new { endWarehouseCode, productCode });
        }
        /// <summary>
        /// 查詢出口報關-EOR類型
        /// </summary>
        /// <returns></returns>
        internal List<EorList> EorList()
        {
            return GetAPI<List<EorList>>("winit.ups.eorType.queryEorList", new {});
        }
        /// <summary>
        /// 查詢物流計劃
        /// </summary>
        /// <param name="winitProductCode">WINIT產品編碼</param>
        /// <param name="inspectionWarehouseCode">驗貨倉編碼</param>
        /// <param name="destinationWarehouseCode">目的倉編碼</param>
        /// <returns></returns>
        internal LogisticsPlan getLogisticsPlan(string winitProductCode, string inspectionWarehouseCode, string destinationWarehouseCode)
        {
            return GetAPI<LogisticsPlan>("winit.tms.getLogisticsPlan", new { winitProductCode, inspectionWarehouseCode, destinationWarehouseCode });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="firstLegType">頭程類型： WC-Winit承運,NS-直發, 說明：標準海外倉入庫的頭程類型為WC,國內直發驗入庫與海外直發驗入庫的頭程類型為NS</param>
        /// <param name="winitProductCode">WINIT產品編碼</param>
        /// <param name="destinationWarehouseCode">目的倉編碼</param>
        /// <returns></returns>
        internal List<AvailableMerchandise> getAvailableMerchandise( string firstLegType, string winitProductCode, string destinationWarehouseCode)
        {
            return GetAPI<List<AvailableMerchandise>>("winit.wh.mms.getAvailableMerchandise", new { customerCode = WinitAccountID, firstLegType, winitProductCode, destinationWarehouseCode });
        }
        /// <summary>
        /// 創建入庫單V2
        /// </summary>
        /// <param name="WinitCreateOrder"></param>
        /// <returns></returns>
        internal RefWinitOrderNo WinitOrderCreate(WinitCreateOrder WinitCreateOrder)
        {
            return GetAPI<RefWinitOrderNo>("winit.wh.inbound.order.create", WinitCreateOrder);
        }

        internal List<VendorInfo> getVendorInfo(string countryCode, string vendorType)
        {
            return GetAPI<List<VendorInfo>>("winit.ums.getVendorInfo", new { countryCode, vendorType });
        }

        internal PackageLabe printPackageLabe(string orderNo,string labelSize,List<packageNoList> packageNoList)
        {
            return GetAPI<PackageLabe>("winit.wh.inbound.printPackageLabel", new { orderNo, labelSize, packageNoList });
        }
        internal OrderDetail getOrderDetail(string orderNo)
        {
            return GetAPI<OrderDetail>("winit.wh.inbound.getOrderDetail", new { orderNo, isIncludePackage = "Y" });
        }
        /// <summary>
        /// 注册/编辑商品
        /// </summary>
        /// <param name="RegisterProduct"></param>
        /// <returns></returns>
        internal List<ReturnProduct> registerProduct(RegisterProduct RegisterProduct)
        {
            return GetAPI<List<ReturnProduct>>("registerProduct", RegisterProduct);
        }
    }
}