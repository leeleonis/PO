using Newtonsoft.Json;
using PurchaseOrderSys.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace PurchaseOrderSys.Controllers
{
    public class RMAController : BaseController
    {
        // GET: RMA
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(RMA filter)
        {
            return RedirectToAction("Index");
        }
        /// <summary>
        /// 已出貨的訂單
        /// </summary>
        /// <param name="OrderID">訂單號</param>
        /// <param name="SourceID">源頭訂單的ID</param>
        /// <param name="UserID">使用者</param>
        /// <returns></returns>
        public List<AwaitingDispatchVM> GetOrderItemData(int? OrderID, string SourceID, string UserID)
        {

            var AwaitingDispatchList = new List<AwaitingDispatchVM>();
            using (WebClient wc = new WebClient())
            {

                try
                {
                    wc.Encoding = Encoding.UTF8;
                    var nDictionary = new  { OrderID, SourceID , UserID };
                    var dataString = JsonConvert.SerializeObject(nDictionary);
                    wc.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                    string resultXML = wc.UploadString(ApiUrl + "Api/GetOrderItemData", "Get", dataString);
                    AwaitingDispatchList = JsonConvert.DeserializeObject<List<AwaitingDispatchVM>>(resultXML);
                }
                catch (WebException ex)
                {

                }
            }
            return AwaitingDispatchList;
        }


        public ActionResult RSkuNumberList(int? OrderID, string SourceID, string UserID)
        {
            var OrderItemData = GetOrderItemData(OrderID, SourceID, UserID);
            if (OrderItemData != null)
            {
                return Json(new { status = true, OrderItemData }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = false, Msg = "查無資料" }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}