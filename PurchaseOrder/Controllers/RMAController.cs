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
        public ActionResult Index(RMAVM RMAVM)
        {
            RMAVM.RMAList = db.RMA.Where(x => x.IsEnable);
            return View(RMAVM);
        }
        public ActionResult Create()
        {
            ViewBag.SID = DateTime.Now.ToString("HHmmss");
            return View();
        }
        [HttpPost]
        public ActionResult Create(string SID, List<RMAModelPost> RMAList)
        {
            var OrderItemDataList = (List<OrderItemData>)Session["RSkuNumberList" + SID];
            var CreateAt = DateTime.UtcNow;
            foreach (var OrderItemDataitem in OrderItemDataList)
            {
                var OrderID = OrderItemDataitem.OrderID;
                var SourceID = OrderItemDataitem.OrderSourceOrderId;
                var CompanyID = OrderItemDataitem.CompanyID;
                var Country = OrderItemDataitem.CountryCode;
                var Channel = OrderItemDataitem.OrderSource;
                var WarehouseID = db.WarehouseSummary.AsQueryable().Where(x => x.Type == "SCID" && x.Val.Contains(OrderItemDataitem.WarehouseID.ToString())).FirstOrDefault()?.WarehouseID;
                var newRMA = new RMA
                {
                    IsEnable = true,
                    OrderID = OrderID,
                    SourceID = SourceID,
                    CompanyID = CompanyID,
                    Country = Country,
                    Channel = Channel,
                    WarehouseID = WarehouseID,
                    CreateBy = UserBy,
                    CreateAt = CreateAt
                };
                db.RMA.Add(newRMA);
                var RMAListselect = RMAList.Where(x => !string.IsNullOrWhiteSpace(x.SKU));
                if (RMAListselect.Any())
                {
                    foreach (var RMAListitem in RMAList)
                    {
                        var skulist = OrderItemDataitem.Items.Where(x => x.SKU == RMAListitem.SKU).Select(x => new { x.SKU, x.QTY }).Distinct();
                        if (skulist.Any())
                        {

                            foreach (var Skuitem in skulist)
                            {
                                var ProductName = db.SkuLang.Where(x => x.LangID == "en-US" && x.Sku == Skuitem.SKU).FirstOrDefault()?.Name;
                                var newRMASKU = new RMASKU
                                {
                                    IsEnable = true,
                                    Name = ProductName,
                                    SkuNo = Skuitem.SKU,
                                    QTYOrdered = Skuitem.QTY,
                                    ReturnedQTY = RMAListitem.RMAQTY,
                                    WarehouseID = RMAListitem.Warehouse,
                                    CreateBy = UserBy,
                                    CreateAt = CreateAt
                                };
                                newRMA.RMASKU.Add(newRMASKU);
                            }
                        }
                    }
                    db.SaveChanges();
                }
            }
            return RedirectToAction("Index");
        }
        /// <summary>
        /// 已出貨的訂單
        /// </summary>
        /// <param name="OrderID">訂單號</param>
        /// <param name="SourceID">源頭訂單的ID</param>
        /// <param name="UserID">使用者</param>
        /// <param name="Status">訂單狀態 0：未出貨 1：待出貨 3：已出貨</param>
        /// <returns></returns>
        public List<OrderItemData> GetOrderItemData(int? OrderID, string SourceID, string UserID,int? Status)
        {
            var OrderItemDataList = new List<OrderItemData>();
            using (WebClient wc = new WebClient())
            {

                try
                {
                    wc.Encoding = Encoding.UTF8;
                    var nDictionary = new { OrderID, SourceID, UserID, Status };
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
        public ActionResult RSkuNumberList(int? draw, int? start, int? length, int? OrderID, string SourceID, string UserID, string SID)
        {
            var RMAModelVMList = new List<RMAModelVM>();
            var OrderItemData = GetOrderItemData(OrderID, SourceID, UserID, 3);
            if (OrderItemData != null)
            {
                foreach (var item in OrderItemData)
                {
                    if (item.Items.Count() > 0)
                    {
                        //if (item.Items.Count() == 1)
                        //{
                        //}
                        //else
                        //{
                        //    RMAModelVMList.Add(new RMAModelVM { ck = item.OrderID, Order = item.OrderID, SKU = "Multi " });
                        //}
                        foreach (var SKUitem in item.Items.GroupBy(x => x.SKU))
                        {
                            var SKUNo = SKUitem.Key;
                            var sku = db.SkuLang.Where(x => x.LangID == "en-US" && x.Sku == SKUNo).FirstOrDefault();
                            var ProductName = sku?.Name;
                            var UPC = sku?.GetSku.UPC + sku?.GetSku.EAN;
                            RMAModelVMList.Add(new RMAModelVM { ck = item.OrderID, Order = item.OrderID, QTY = SKUitem.Sum(x => x.QTY), SKU = SKUNo, ProductName = ProductName, UPC = UPC });
                        }
                    }

                }
                Session["RSkuNumberList" + SID] = OrderItemData;
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
        public ActionResult GetSKUList(int OrderID)
        {
            var RMAModelVMList = new List<RMAModelVM>();
            var OrderItemData = GetOrderItemData(OrderID, "", "", 3);
            foreach (var item in OrderItemData)
            {
                foreach (var SKUitemList in item.Items)
                {
                    var SKUNo = SKUitemList.SKU;
                    var ProductName = db.SkuLang.Where(x => x.LangID == "en-US" && x.Sku == SKUNo).FirstOrDefault()?.Name;
                    RMAModelVMList.Add(new RMAModelVM { ck = item.OrderID, Order = item.OrderID, QTY = item.Items.FirstOrDefault().QTY, SKU = SKUNo, ProductName = ProductName });
                }
            }
            var partial = ControlToString("~/Views/Shared/GetRMASKUList.cshtml", RMAModelVMList);
            return Json(new { status = true, partial }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult GetRMASKUList(int ID)
        {
            var RMAModelVMList = new List<RMAModelVM>();
            var RMA = db.RMA.Find(ID);
            foreach (var item in RMA.RMASKU)
            {
                var SKUNo = item.SkuNo;
                var ProductName = db.SkuLang.Where(x => x.LangID == "en-US" && x.Sku == SKUNo).FirstOrDefault()?.Name;
                RMAModelVMList.Add(new RMAModelVM { QTY = item.ReturnedQTY, SKU = SKUNo, ProductName = ProductName });
            }
           
            var partial = ControlToString("~/Views/Shared/GetRMASKUList.cshtml", RMAModelVMList);
            return Json(new { status = true, partial }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Edit(int id)
        {
            var RMA = db.RMA.Find(id);
            return View(RMA);
        }
        [HttpPost]
        public ActionResult Edit(RMA RMA)
        {
            var OldRMA = db.RMA.Find(RMA.ID);
            OldRMA.Status = RMA.Status;
            OldRMA.Action = RMA.Action;
            OldRMA.SourceCaseID = RMA.SourceCaseID;
            OldRMA.UpdateBy = UserBy;
            OldRMA.UpdateAt = DateTime.UtcNow;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult GetSerialList(int ID)
        {
            var RMASKU = db.RMASKU.Find(ID);
            var SerialsLlist = RMASKU.RMASerialsLlist.Where(x => x.SerialsType == "RMAIn").Select(x => new RMASerial { Serial = x.SerialsNo, Warehouse = x.Warehouse.Name, Reason = x.Reason }).ToList();
            var partial = ControlToString("~/Views/Shared/GetRMASerialList.cshtml", SerialsLlist);
            //var partial = Engine.Razor.RunCompile(template, "templateKey", null, new { Name = "World" });
            return Json(new { status = true, partial }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Returnedserials(int ID)
        {
            var companyList = db.Company.AsNoTracking().Where(c => c.IsEnable).ToList();
            ViewBag.Company = companyList.Where(c => !c.ParentID.HasValue).Select(c => new SelectListItem() { Text = c.Name, Value = c.ID.ToString() }).ToList();
            var RMASKU = db.RMASKU.Find(ID);
            return View(RMASKU);
        }
        public ActionResult Saveserials(string serials,string Reason, int RMASKUID, int WarehouseID)
        {
            var RMASKU = db.RMASKU.Find(RMASKUID);
            var SerialsLlistCount = RMASKU.RMASerialsLlist.Where(x => x.SerialsType == "RMAIn").Sum(x => x.SerialsQTY);//計算RMAIn的序號數
            if (SerialsLlistCount >= RMASKU.QTYOrdered)
            {
                return Json(new { status = false, Errmsg = "序號不可大於回收數" }, JsonRequestBehavior.AllowGet);
            }
            var RMASerialsLlist = db.RMASerialsLlist.Where(x => x.SerialsNo == serials && x.RMASKU.SkuNo == RMASKU.SkuNo);//檢查序號是否重複，同SKU序號不能新增,2019/02/05 加入有已出貨或是CM的紀錄, 就能重新在入庫
            if (!RMASerialsLlist.Any())
            {

                var OrderData = db.SerialsLlist.Where(x => x.SerialsType == "Order" && x.SerialsNo == serials && x.PurchaseSKU.SkuNo == RMASKU.SkuNo && x.OrderID == RMASKU.RMA.OrderID);

                if (OrderData.Any())
                {
                    var dt = DateTime.UtcNow;
                    var nSerialsLlistIn = new RMASerialsLlist
                    {
                        WarehouseID = WarehouseID,
                        Reason = Reason,
                        SerialsType = "RMAIn",
                        SerialsNo = serials,
                        SerialsQTY = 1,
                        ReceivedBy = UserBy,
                        ReceivedAt = dt,
                        CreateBy = UserBy,
                        CreateAt = dt
                    };
                    RMASKU.RMASerialsLlist.Add(nSerialsLlistIn);
                    try
                    {
                        db.SaveChanges();
                        return Json(new { status = true }, JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception ex)
                    {
                        return Json(new { status = false, Errmsg = ex.ToString() }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { status = false, Errmsg = "序號沒有出貨資料" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { status = false, Errmsg = "序號已經存在" }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}