using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using Newtonsoft.Json;

namespace PurchaseOrderSys.Models
{
    public class OrderManagement : IDisposable
    {
        protected PurchaseOrderEntities db = new PurchaseOrderEntities();

        protected Orders orderData;

        private bool disposedValue = false; // 偵測多餘的呼叫

        public OrderManagement() { }

        public OrderManagement(int ID)
        {
            SetOrderData(ID);
        }

        public void SetOrderData(int ID)
        {
            orderData = db.Orders.FirstOrDefault(o => o.ID.Equals(ID) || o.SCID.Value.Equals(ID));
        }

        public void OrderSync(int? OrderSCID)
        {
            var result = Request<Orders>("Api/GetOrderData", "Post", new { OrderID = orderData?.SCID ?? OrderSCID });

            try
            {
                if (!result.Status) throw new Exception("Ajax error：" + result.Message);

                var order = result.Data;
                order.Company = db.Company.First(c => c.CompanySCID.Value.Equals(order.Company)).ID;
                order.Channel = EnumData.OrderChannelList().First(c => c.Value.Equals(EnumData.OrderChannelList(true)[order.Channel])).Key;
                order.OrderStatus = EnumData.OrderStatusList().First(s => s.Value.Equals(EnumData.OrderStatusList(true)[order.OrderStatus])).Key;
                order.PaymentStatus = EnumData.OrderPaymentStatusList().First(p => p.Value.Equals(EnumData.OrderPaymentStatusList(true)[order.PaymentStatus])).Key;

                order.OrderType = (byte)CheckOrderType(order);
                order.FulfillmentStatus = (byte)CheckFulfillmentStatus(order);
            }
            catch(Exception e)
            {
                throw new Exception(e.InnerException?.Message ?? e.Message);
            }

        }

        private EnumData.OrderType CheckOrderType(Orders order)
        {
            if (order.Channel.Equals((byte)EnumData.OrderChannel.FBA)) return EnumData.OrderType.FBA;

            if(order.Packages.Any(p => p.IsEnable && p.GetWarehouse.Type.Equals("DropShip"))) return EnumData.OrderType.Dropship;

            if (order.Packages.Any(p => p.IsEnable && p.GetMethod.DirectLine.HasValue)) return EnumData.OrderType.DirectLine;

            return EnumData.OrderType.Normal;
        }

        private EnumData.OrderFulfilledStatus CheckFulfillmentStatus(Orders order)
        {
            if (order.Packages.Where(p => p.IsEnable).All(p => p.ShippingStatus.Equals((byte)EnumData.OrderShippingStatus.已出貨))) return EnumData.OrderFulfilledStatus.Fulfilled;

            if (order.Packages.Where(p => p.IsEnable).Any(p => p.ShippingStatus.Equals((byte)EnumData.OrderShippingStatus.已出貨))) return EnumData.OrderFulfilledStatus.Partial;

            return EnumData.OrderFulfilledStatus.Unfulfilled;
        }

        private Response<T> Request<T>(string url, string method = "post", object data = null) where T : new()
        {
            Response<T> response = new Response<T>();
            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://internal.qd.com.tw/" + url);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://localhost:49920/" + url);
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