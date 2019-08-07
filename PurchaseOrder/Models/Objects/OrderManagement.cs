﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using Newtonsoft.Json;
using PurchaseOrderSys.SCService;
using SellerCloud_WebService;

namespace PurchaseOrderSys.Models
{
    public class OrderManagement : IDisposable
    {
        public static string ApiUserName = "test@qd.com.tw";
        public static string ApiPassword = "prU$U9R7CHl3O#uXU6AcH6ch";
        protected PurchaseOrderEntities db = new PurchaseOrderEntities();

        protected Orders orderData;

        public SC_WebService SC_Api;

        private readonly string AdminName = HttpContext.Current.Session["AdminName"]?.ToString() ?? "System";
        private readonly DateTime UtcNow = DateTime.UtcNow;

        private bool disposedValue = false; // 偵測多餘的呼叫

        public OrderManagement() : this(null) { }

        public OrderManagement(int? ID)
        {
            if (ID.HasValue) SetOrderData(ID.Value);
        }

        public void SetOrderData(int ID)
        {
            orderData = db.Orders.FirstOrDefault(o => o.ID.Equals(ID) || o.SCID.Value.Equals(ID));
        }

        public Orders OrderSync(int? OrderSCID)
        {
            var result = Request<Orders>("Api/GetOrderData", "Post", new { OrderID = orderData?.SCID ?? OrderSCID });

            try
            {
                if (!result.Status) throw new Exception(result.Message);

                var order = result.Data;
                order.OrderStatus = EnumData.OrderStatusList().First(s => s.Value.Equals(EnumData.OrderStatusList(true)[order.OrderStatus])).Key;
                order.PaymentStatus = EnumData.OrderPaymentStatusList().First(p => p.Value.Equals(EnumData.OrderPaymentStatusList(true)[order.PaymentStatus])).Key;
                order.FulfilledDate = order.FulfilledDate.HasValue && !order.FulfilledDate.Equals(DateTime.MinValue) ? order.FulfilledDate : null;

                if (orderData != null)
                {
                    SetUpdateData(orderData, order, new string[] { "IsRush", "OrderStatus", "PaymentStatus", "PaymentDate", "FulfillmentStatus", "FulfilledDate", "Comment" });
                }
                else
                {
                    orderData = new Orders() { CreateBy = AdminName, CreateAt = UtcNow };
                    SetUpdateData(orderData, order, new string[] { "IsRush", "OrderParent", "OrderSourceID", "SCID", "CustomerID", "CustomerEmail", "OrderStatus", "OrderDate", "PaymentStatus", "PaymentDate", "FulfillmentStatus", "FulfilledDate", "BuyerNote", "Comment" });
                    orderData.Company = db.Company.AsNoTracking().First(c => c.CompanySCID.Value.Equals(order.Company)).ID;
                    orderData.Channel = EnumData.OrderChannelList().First(c => c.Value.Equals(EnumData.OrderChannelList(true)[order.Channel])).Key;
                    db.Orders.Add(orderData);
                }

                db.SaveChanges();

                foreach (var package in order.Packages)
                {
                    package.ShipWarehouse = db.WarehouseSummary.AsNoTracking().First(w => w.IsEnable && w.Type.Equals("SCID") && w.Val.Equals(package.ShipWarehouse.ToString())).WarehouseID;
                    package.ReturnWarehouse = db.WarehouseSummary.AsNoTracking().First(w => w.IsEnable && w.Type.Equals("SCID") && w.Val.Equals(package.ReturnWarehouse.ToString())).WarehouseID;

                    if (package.ShippingMethod == 0)
                    {
                        package.ShippingMethod = db.ShippingMethods.First(m => m.IsEnable).ID;
                    }

                    if (orderData.Packages.Any(p => p.SCID.Value.Equals(package.SCID.Value)))
                    {
                        var packageData = orderData.Packages.First(p => p.SCID.Value.Equals(package.SCID.Value));
                        SetUpdateData(packageData, package, new string[] { "IsEnable", "CarrierBox", "ShippingMethod", "Export", "ExportMethod", "ExportValue", "UploadTracking", "Tracking", "DLExport", "DLExportMethod", "DLExportValue", "DLUploadTracking", "DLTracking", "ShipWarehouse", "ReturnWarehouse", "ShippingStatus" });
                    }
                    else
                    {
                        var ExportCurrency = Enum.GetName(typeof(CurrencyCodeType2), package.ExportCurrency);
                        package.ExportCurrency = db.Currency.AsNoTracking().First(c => c.Code.ToLower().Equals(ExportCurrency.ToLower())).ID;
                        var DLExportCurrency = Enum.GetName(typeof(CurrencyCodeType2), package.DLExportCurrency);
                        package.DLExportCurrency = db.Currency.AsNoTracking().First(c => c.Code.ToLower().Equals(DLExportCurrency.ToLower())).ID;
                        package.CreateBy = AdminName;
                        package.CreateAt = UtcNow;
                        orderData.Packages.Add(package);
                    }
                }

                db.SaveChanges();

                foreach (var item in order.Items)
                {
                    if (orderData.Items.Any(i => i.SCID.Value.Equals(item.SCID.Value)))
                    {
                        var itemData = orderData.Items.First(i => i.SCID.Value.Equals(item.SCID.Value));
                        SetUpdateData(itemData, item, new string[] { "IsEnable", "ExportValue", "DLExportValue", "Qty", "eBayItemID", "eBayTransationID", "SalesRecordNumber", "RMAID" });
                    }
                    else
                    {
                        item.PackageID = db.Packages.AsNoTracking().First(p => p.SCID.Value.Equals(item.PackageID)).ID;
                        if (item.Sku.Any(s => new char[] { '-', '_' }.Contains(s)))
                        {
                            item.OriginSku = item.Sku;
                            item.Sku = item.Sku.Split('_')[0].Split('-')[0];
                        }
                        item.CreateBy = AdminName;
                        item.CreateAt = UtcNow;
                        orderData.Items.Add(item);
                    }
                }

                db.SaveChanges();

                foreach (var serial in order.Serials)
                {
                    if (!orderData.Serials.Any(s => s.SerialNumber.Equals(serial.SerialNumber)))
                    {
                        serial.ItemID = db.Items.AsNoTracking().First(i => i.SCID.Value.Equals(serial.ItemID)).ID;
                        serial.CreateBy = AdminName;
                        serial.CreateAt = UtcNow;
                        orderData.Serials.Add(serial);
                    }
                }

                foreach (var payment in order.Payments)
                {
                    if (!orderData.Payments.Any(p => p.SCID.Value.Equals(payment.SCID.Value)))
                    {
                        payment.CreateBy = AdminName;
                        payment.CreateAt = UtcNow;
                        orderData.Payments.Add(payment);
                    }
                }

                if (!orderData.Addresses.Any())
                {
                    var soldTo = order.Addresses.First();
                    soldTo.CreateBy = AdminName;
                    soldTo.CreateAt = UtcNow;
                    orderData.Addresses.Add(soldTo);

                    var shipTo = new OrderAddresses() { Type = (byte)EnumData.OrderAddressType.Shipped };
                    SetUpdateData(shipTo, soldTo, new string[] { "FirstName", "LastName", "AddressLine1", "AddressLine2", "City", "State", "Postcode", "CountryCode", "CountryName", "PhoneNumber", "EmailAddress", "CreateBy", "CreateAt" });
                    orderData.Addresses.Add(shipTo);
                }

                db.SaveChanges();

                orderData = db.Orders.AsNoTracking().First(o => o.ID.Equals(orderData.ID));
                orderData.OrderType = (byte)CheckOrderType(orderData);
                orderData.FulfillmentStatus = (byte)CheckFulfillmentStatus(orderData);
            }
            catch (Exception e)
            {
                throw new Exception(e.InnerException?.Message ?? e.Message);
            }

            return orderData;
        }

        private EnumData.OrderType CheckOrderType(Orders order)
        {
            if (order.Channel.Equals((byte)EnumData.OrderChannel.FBA)) return EnumData.OrderType.FBA;

            if (order.Packages.Any(p => p.IsEnable && p.GetWarehouse.Type.Equals("DropShip"))) return EnumData.OrderType.Dropship;

            if (order.Packages.Any(p => p.IsEnable && p.GetMethod.DirectLine.HasValue)) return EnumData.OrderType.DirectLine;

            return EnumData.OrderType.Normal;
        }

        private EnumData.OrderFulfilledStatus CheckFulfillmentStatus(Orders order)
        {
            if (order.Packages.Where(p => p.IsEnable).All(p => p.ShippingStatus.Equals((byte)EnumData.OrderShippingStatus.已出貨))) return EnumData.OrderFulfilledStatus.Fulfilled;

            if (order.Packages.Where(p => p.IsEnable).Any(p => p.ShippingStatus.Equals((byte)EnumData.OrderShippingStatus.已出貨))) return EnumData.OrderFulfilledStatus.Partial;

            return EnumData.OrderFulfilledStatus.Unfulfilled;
        }

        public void SplitPackage(int[] PackageIDs)
        {
            if (SC_Api == null) SC_Api = new SC_WebService(ApiUserName, ApiPassword);

            try
            {
                if (!orderData.SCID.HasValue) throw new Exception("Not found order's SCID!");

                var packageList = orderData.Packages.Where(p => PackageIDs.Contains(p.ID)).OrderBy(p => p.ID).ToList(); ;

                Packages packageData = packageList.First(pp => pp.SCID.HasValue);
                Order SC_order = SC_Api.Get_OrderData(orderData.SCID.Value).Order;
                Package SC_package = SC_order.Packages.First(p => p.ID.Equals(packageData.SCID));

                SC_package.Qty = packageData.Items.Where(i => i.IsEnable).Sum(i => i.Qty);
                SC_Api.Update_PackageData(SC_package);

                Items itemData;
                foreach (var SC_item in SC_order.Items.Where(i => i.PackageID.Equals(SC_package.ID)))
                {
                    itemData = packageData.Items.First(i => i.SCID.Value.Equals(SC_item.ID));
                    if (itemData.IsEnable)
                    {
                        if (SC_item.Qty != itemData.Qty)
                        {
                            SC_item.Qty = itemData.Qty;
                            SC_Api.Update_OrderItem(SC_item);
                        }
                    }
                    else
                    {
                        SC_Api.Delete_Item1(SC_item.OrderID, SC_item.ID);
                    }
                }

                foreach (var newPackage in packageList.Where(p => !p.SCID.HasValue))
                {
                    SC_package.Qty = newPackage.Items.Where(i => i.IsEnable).Sum(i => i.Qty);
                    SC_package = SC_Api.Add_OrderNewPackage(SC_package);
                    newPackage.SCID = SC_package.ID;

                    foreach(var newItem in newPackage.Items.Where(i => i.IsEnable))
                    {
                        var SC_item = SC_order.Items.First(i => i.ProductID.Equals(newItem.Sku));
                        SC_item.PackageID = SC_package.ID;
                        SC_item = SC_Api.Add_OrderNewItem(SC_item);
                        newItem.SCID = SC_item.ID;
                    }
                }

                db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("SC Error：{0}", ex.InnerException?.Message ?? ex.Message));
            }
        }

        public void ActionLog(string Item, string Description)
        {
            orderData.ActionLogs.Add(new OrderActionLogs()
            {
                OrderID = orderData.ID,
                Item = Item,
                Description = Description,
                CreateBy = AdminName,
                CreateAt = UtcNow
            });

            db.SaveChanges();
        }

        public void OrderSyncPush()
        {
            Request<object>("OrderSync/GetOrder", "post", new { orderIDs = new string[] { orderData.SCID.ToString() } });
        }

        private void SetUpdateData<T>(T originData, T updateData, string[] updateColumn)
        {
            var TypeInfoList = originData.GetType().GetProperties();
            foreach (string column in updateColumn)
            {
                var newData = TypeInfoList.FirstOrDefault(info => info.Name.Equals(column)).GetValue(updateData, null);
                TypeInfoList.FirstOrDefault(info => info.Name.Equals(column)).SetValue(originData, newData);
            }
            TypeInfoList.FirstOrDefault(info => info.Name.Equals("UpdateBy")).SetValue(originData, AdminName);
            TypeInfoList.FirstOrDefault(info => info.Name.Equals("UpdateAt")).SetValue(originData, UtcNow);
        }

        private Response<T> Request<T>(string url, string method = "post", object data = null) where T : new()
        {
            Response<T> response = new Response<T>();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://internal.qd.com.tw/" + url);
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

                db = null;
                orderData = null;

                disposedValue = true;
            }
        }

        // TODO: 僅當上方的 Dispose(bool disposing) 具有會釋放非受控資源的程式碼時，才覆寫完成項。
        // ~StockKeepingUnit() {
        //   // 請勿變更這個程式碼。請將清除程式碼放入上方的 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

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