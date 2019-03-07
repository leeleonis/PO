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
        public List<OrderItemData> GetOrderItemData(int? OrderID, string SourceID, string UserID)
        {

            var OrderItemDataList = new List<OrderItemData>();
            using (WebClient wc = new WebClient())
            {

                try
                {
                    wc.Encoding = Encoding.UTF8;
                    var nDictionary = new { OrderID, SourceID, UserID };
                    var dataString = JsonConvert.SerializeObject(nDictionary);
                    wc.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                    string resultXML = wc.UploadString(ApiUrl + "Api/GetOrderItemData", "Post", dataString);
                    OrderItemDataList = JsonConvert.DeserializeObject<List<OrderItemData>>(resultXML);
                }
                catch (WebException ex)
                {

                }
            }
            return OrderItemDataList;
        }
        public ActionResult RSkuNumberList(int? draw, int? start, int? length, int? OrderID, string SourceID, string UserID)
        {
            var RMAModelVMList = new List<RMAModelVM>();
            var OrderItemData = GetOrderItemData(OrderID, SourceID, UserID);
            if (OrderItemData != null)
            {
                foreach (var item in OrderItemData)
                {
                    if (item.Items.Count() > 0)
                    {
                        if (item.Items.Count() == 1)
                        {
                            var SKUNo = item.Items.FirstOrDefault()?.SKU;
                            var ProductName = db.SkuLang.Where(x => x.LangID == "en-US" && x.Sku == SKUNo).FirstOrDefault()?.Name;
                            RMAModelVMList.Add(new RMAModelVM { ck = item.OrderID, Order = item.OrderID, QTY = item.Items.FirstOrDefault().QTY, SKU = SKUNo, ProductName = ProductName });
                        }
                        else
                        {
                            RMAModelVMList.Add(new RMAModelVM { ck = item.OrderID, Order = item.OrderID, SKU = "Multi " });
                        }
                    }

                }

            }
            int recordsTotal = RMAModelVMList.Count();
            var returnObj = new
            {
                draw = draw,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = RMAModelVMList//分頁後的資料 
            };
            return Json(returnObj, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult GetSerialList(int OrderID)
        {
            var RMAModelVMList = new List<RMAModelVM>();
            var OrderItemData = GetOrderItemData(OrderID, "", "");
            foreach (var item in OrderItemData)
            {
                foreach (var SKUitem in item.Items)
                {
                    var SKUNo = item.Items.FirstOrDefault()?.SKU;
                    var ProductName = db.SkuLang.Where(x => x.LangID == "en-US" && x.Sku == SKUNo).FirstOrDefault()?.Name;
                    RMAModelVMList.Add(new RMAModelVM { ck = item.OrderID, Order = item.OrderID, QTY = item.Items.FirstOrDefault().QTY, SKU = SKUNo, ProductName = ProductName });
                }
            }
            var partial = ControlToString("~/Views/Shared/GetRMASKUList.cshtml", RMAModelVMList);
            return Json(new { status = true, partial }, JsonRequestBehavior.AllowGet);
        }
    }
}