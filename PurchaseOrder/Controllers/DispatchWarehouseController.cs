
using Newtonsoft.Json;
using PurchaseOrderSys.Api;
using PurchaseOrderSys.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace PurchaseOrderSys.Controllers
{
    public class carrierList
    {
        public string text { get; set; }
        public string value { get; set; }
        public string type { get; set; }
    }
    public class DispatchWarehouseController : BaseController
    {
        // GET: DispatchWarehouse
        public ActionResult Index()
        {
            ViewBag.Warehouse3PList = new Api.Winit_API().Warehouse3P();
            var SCList = new Api.SC_API().SCList();
            ViewBag.SCList = SCList;
            var Warehouse = db.Warehouse.Where(x => x.IsEnable);
            return View(Warehouse);
        }
        public ActionResult Create()
        {
          
            ViewBag.Warehouse3PList = new Api.Winit_API().Warehouse3P();
            ViewBag.SCList = new Api.SC_API().SCList();
            return View();
        }
        [HttpPost]
        public ActionResult Create(Warehouse Warehouse ,string[] Shippingmethods,string SCID,string Warehouse3P)
        {
            Warehouse.IsEnable = true;
            Warehouse.CreateBy = UserBy;
            Warehouse.CreateAt = DateTime.UtcNow;
            db.Warehouse.Add(Warehouse);
            if (Shippingmethods!=null&& Shippingmethods.Any())
            {
                Warehouse.WarehouseSummary.Add(new WarehouseSummary { IsEnable=true, Val = string.Join(",", Shippingmethods), Type = "Shippingmethods" });
            }
            if (!string.IsNullOrWhiteSpace(SCID))
            {
                Warehouse.WarehouseSummary.Add(new WarehouseSummary { IsEnable = true, Val = SCID, Type = "SCID" });
            }
            if (!string.IsNullOrWhiteSpace(Warehouse3P))
            {
                Warehouse.WarehouseSummary.Add(new WarehouseSummary { IsEnable = true, Val = Warehouse3P, Type = "3PWarehouse" });
            }
            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {

                throw;
            }
         
            return RedirectToAction("Index");
        }
        public ActionResult Edit(int id)
        {
            ViewBag.Warehouse3PList = new Api.Winit_API().Warehouse3P();
            var SCList = new Api.SC_API().SCList();
            ViewBag.SCList = SCList;
            var Warehouse = db.Warehouse.Find(id);
            if (Warehouse.WarehouseSummary.Any())
            {
                foreach (var item in Warehouse.WarehouseSummary.Where(x => x.IsEnable))
                {
                    switch (item.Type)
                    {
                        case "Shippingmethods":
                            TempData["Shippingmethods"] = item.Val;
                            break;
                        case "SCID":
                            ViewBag.SCID = item.Val;
                            if (!string.IsNullOrWhiteSpace(item.Val))
                            {
                                var dList = SCList.Where(x => x.ID.ToString() == item.Val);
                                if (dList.Any())
                                {
                                    ViewBag.SCName = dList.FirstOrDefault().Name;
                                    ViewBag.SCType = dList.FirstOrDefault().WarehouseType;
                                    ViewBag.SCSellable = dList.FirstOrDefault().IsSellAble.ToString();
                                    ViewBag.SCDefault = dList.FirstOrDefault().IsDefault.ToString();
                                }
                            }
                          
                            break;
                        case "3PWarehouse":
                            ViewBag.Warehouse3P = item.Val;
                            break;
                    }
                }
            }
            return View(Warehouse);
        }
        [HttpPost]
        public ActionResult Edit(Warehouse Warehouse, string[] Shippingmethods, string SCID, string Warehouse3P)
        {
            var OldWarehouse = db.Warehouse.Find(Warehouse.ID);
            if (!string.IsNullOrWhiteSpace(Warehouse.Name)) OldWarehouse.Name = Warehouse.Name;
            if (!string.IsNullOrWhiteSpace(Warehouse.Type)) OldWarehouse.Type = Warehouse.Type;
            if (!string.IsNullOrWhiteSpace(Warehouse.WinitWarehouse)) OldWarehouse.WinitWarehouse = Warehouse.WinitWarehouse;
            if (!string.IsNullOrWhiteSpace(Warehouse.Fulfillable)) OldWarehouse.Fulfillable = Warehouse.Fulfillable;
            if (!string.IsNullOrWhiteSpace(Warehouse.Location)) OldWarehouse.Location = Warehouse.Location;
            if (!string.IsNullOrWhiteSpace(Warehouse.Countries)) OldWarehouse.Countries = Warehouse.Countries;
            if (!string.IsNullOrWhiteSpace(Warehouse.Marketplace)) OldWarehouse.Marketplace = Warehouse.Marketplace;
            if (!string.IsNullOrWhiteSpace(Warehouse.Company)) OldWarehouse.Company = Warehouse.Company;
            if (Warehouse.DefaultDispatch.HasValue) OldWarehouse.DefaultDispatch = Warehouse.DefaultDispatch;
            if (Warehouse.DefaultRMA.HasValue) OldWarehouse.DefaultRMA = Warehouse.DefaultRMA;
            if (!string.IsNullOrWhiteSpace(Warehouse.Address1)) OldWarehouse.Address1 = Warehouse.Address1;
            if (!string.IsNullOrWhiteSpace(Warehouse.Address2)) OldWarehouse.Address2 = Warehouse.Address2;
            if (!string.IsNullOrWhiteSpace(Warehouse.City)) OldWarehouse.City = Warehouse.City;
            if (!string.IsNullOrWhiteSpace(Warehouse.State)) OldWarehouse.State = Warehouse.State;
            if (!string.IsNullOrWhiteSpace(Warehouse.Postcode)) OldWarehouse.Postcode = Warehouse.Postcode;
            if (!string.IsNullOrWhiteSpace(Warehouse.Country)) OldWarehouse.Country = Warehouse.Country;
            if (!string.IsNullOrWhiteSpace(Warehouse.Phone)) OldWarehouse.Phone = Warehouse.Phone;

            OldWarehouse.UpdateBy = UserBy;
            OldWarehouse.UpdateAt = DateTime.UtcNow;

            if (Shippingmethods != null && Shippingmethods.Any())
            {
                var WarehouseSummary = OldWarehouse.WarehouseSummary.Where(x => x.IsEnable && x.Type == "Shippingmethods");
                if (WarehouseSummary.Any())
                {
                    foreach (var item in WarehouseSummary)
                    {
                        item.Val = string.Join(",", Shippingmethods);
                    }
                }
                else
                {
                    OldWarehouse.WarehouseSummary.Add(new WarehouseSummary { IsEnable = true, Val = string.Join(",", Shippingmethods), Type = "Shippingmethods" });
                }

            }
            else
            {
                var WarehouseSummary = OldWarehouse.WarehouseSummary.Where(x => x.IsEnable && x.Type == "Shippingmethods");
                if (WarehouseSummary.Any())
                {
                    foreach (var item in WarehouseSummary)
                    {
                        item.Val = "";
                    }
                }
            }
            if (!string.IsNullOrWhiteSpace(SCID))
            {
                var WarehouseSummary = OldWarehouse.WarehouseSummary.Where(x => x.IsEnable && x.Type == "SCID");
                if (WarehouseSummary.Any())
                {
                    foreach (var item in WarehouseSummary)
                    {
                        item.Val = SCID;
                    }
                }
                else
                {
                    OldWarehouse.WarehouseSummary.Add(new WarehouseSummary { IsEnable = true, Val = SCID, Type = "SCID" });
                }
            }
            else
            {
                var WarehouseSummary = OldWarehouse.WarehouseSummary.Where(x => x.IsEnable && x.Type == "SCID");
                if (WarehouseSummary.Any())
                {
                    foreach (var item in WarehouseSummary)
                    {
                        item.Val = SCID;
                    }
                }
            }
            if (!string.IsNullOrWhiteSpace(Warehouse3P))
            {
                var WarehouseSummary = OldWarehouse.WarehouseSummary.Where(x => x.IsEnable && x.Type == "3PWarehouse");
                if (WarehouseSummary.Any())
                {
                    foreach (var item in WarehouseSummary)
                    {
                        item.Val = Warehouse3P;
                    }
                }
                else
                {
                    OldWarehouse.WarehouseSummary.Add(new WarehouseSummary { IsEnable = true, Val = Warehouse3P, Type = "3PWarehouse" });
                }
            }
            else
            {
                var WarehouseSummary = OldWarehouse.WarehouseSummary.Where(x => x.IsEnable && x.Type == "3PWarehouse");
                if (WarehouseSummary.Any())
                {
                    foreach (var item in WarehouseSummary)
                    {
                        item.Val = Warehouse3P;
                    }
                }
            }      
            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {

                throw;
            }

            return RedirectToAction("Index");
        }
      
        public ActionResult Delete(int id)
        {
            var Warehouse = db.Warehouse.Find(id);
            Warehouse.IsEnable = false;
            Warehouse.UpdateBy = UserBy;
            Warehouse.UpdateAt = DateTime.UtcNow;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult ShippingMethodData()
        {
            string Shippingmethods = "";
            if (TempData["Shippingmethods"] != null)
            {
                Shippingmethods = TempData["Shippingmethods"].ToString();
            }
            var ShippingArray = Shippingmethods.Split(',');
            int total = 0;
            var ShippingList = new Api.Shipping_API().ShippingList();



            List<dynamic> dataList = new List<dynamic>();
            List<dynamic> dataListOut = new List<dynamic>();
            var results = db.ShippingMethod.Where(m => m.IsEnable).OrderBy(m => m.ID).ToList();
            var carrierList = db.Carriers.Where(c => c.IsEnable && c.CarrierAPI != null && c.CarrierAPI.IsEnable).ToDictionary(c => c.ID, c => Enum.GetName(typeof(EnumData.CarrierType), c.CarrierAPI.Type));
            if (results.Any())
            {

                ////var carrierlist = ((IEnumerable<dynamic>)ShippingList.data.carrier).Cast<carrierList>().ToList();
                //var carrierlist = JsonConvert.DeserializeObject<Api.RootObject>(ShippingList.data.carrier);
                total = results.Count();

                dataList.AddRange(results.OrderBy(m => m.Name).Select(m => new
                {
                    m.ID,
                    CarrierType = carrierList.ContainsKey(m.CarrierID.Value) ? carrierList[m.CarrierID.Value] : "",
                    m.IsDirectLine,
                    m.Name,
                    m.CarrierID,
                    MethodTypeID = m.MethodType,
                    BoxTypeID = m.BoxType,
                    Carrier = ShippingList.data.carrier.Where(x => x.value == m.CarrierID)?.FirstOrDefault().text,
                    MethodType = "",
                    BoxType = "",
                    m.IsExport,
                    m.IsBattery,
                    m.InBox
                }).ToList());

                try
                {
                    dataListOut.AddRange(dataList.Select(m => new
                    {
                        m.ID,
                        m.CarrierType,
                        m.IsDirectLine,
                        m.Name,
                        m.CarrierID,
                        m.MethodTypeID,
                        m.BoxTypeID,
                        m.Carrier,
                        MethodType = MethodTypeFun(m.MethodTypeID, m.CarrierType, ShippingList),
                        BoxType = BoxTypeFun(m.BoxTypeID, m.CarrierType, ShippingList),
                        m.IsExport,
                        m.IsBattery,
                        m.InBox,
                        checkedVal = checkedValFun(ShippingArray, m.ID)
                    }).ToList());
                }
                catch (Exception ex)
                {

                    throw;
                }




                //foreach (var item in dataList)
                //{
                //    item.MethodType = MethodTypeFun(item.MethodTypeID, item.CarrierType, ShippingList);
                //    item.BoxType = BoxTypeFun(item.BoxTypeID, item.CarrierType, ShippingList);
                //}
            }
            int recordsTotal = dataListOut.Count();
            var returnObj =
            new
            {
                recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = dataListOut//分頁後的資料 
            };
            return Json(returnObj, JsonRequestBehavior.AllowGet);
        }

        private string checkedValFun(string[] shippingArray, dynamic ID)
        {
            string val = "";
            val= shippingArray.Any(x => x == ID.ToString()) ? "checked" : "";
            return val;
        }

        private string BoxTypeFun(int? boxType, string CarrierType, RootObject shippingList)
        {
            string val = "";
            switch (CarrierType)
            {
                case "Other":
                    
                    break;
                case "DHL":
                    //val = shippingList.data.methodType.DHL.Where(x => x.value == boxType)?.FirstOrDefault().text;
                    break;
                case "FedEx":
                    val = shippingList.data.boxType.FedEx.Where(x => x.value == boxType)?.FirstOrDefault().text;
                    break;
                case "UPS":
                    //val = shippingList.data.methodType.UPS.Where(x => x.value == boxType)?.FirstOrDefault().text;
                    break;
                case "Winit":
                    //val = shippingList.data.methodType.Winit.Where(x => x.value == boxType)?.FirstOrDefault().text;
                    break;
                case "IDS":
                    //val = shippingList.data.methodType.IDS.Where(x => x.value == boxType)?.FirstOrDefault().text;
                    break;
                case "Sendle":
                    //val = shippingList.data.methodType.Sendle.Where(x => x.value == boxType)?.FirstOrDefault().text;
                    break;
            }
            return val;     
        }

        private string MethodTypeFun(int? methodType,  string CarrierType, RootObject shippingList)
        {
            string val = "";
            switch (CarrierType)
            {
                case "Other":

                    break;
                case "DHL":
                    //val = shippingList.data.methodType.DHL.Where(x => x.value == boxType)?.FirstOrDefault().text;
                    break;
                case "FedEx":
                    val = shippingList.data.methodType.FedEx.Where(x => x.value == methodType)?.FirstOrDefault().text;
                    break;
                case "UPS":
                    //val = shippingList.data.methodType.UPS.Where(x => x.value == boxType)?.FirstOrDefault().text;
                    break;
                case "Winit":
                    val = shippingList.data.methodType.Winit.Where(x => x.value == methodType)?.FirstOrDefault().text;
                    break;
                case "IDS":
                    val = shippingList.data.methodType.IDS.Where(x => x.value == methodType)?.FirstOrDefault().text;
                    break;
                case "Sendle":
                    //val = shippingList.data.methodType.Sendle.Where(x => x.value == boxType)?.FirstOrDefault().text;
                    break;
            }
            return val;
        }
    }
}