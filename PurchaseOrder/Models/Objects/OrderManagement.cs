using System;
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
    public class OrderManagement : Common, IDisposable
    {
        public Orders orderData;

        public SC_WebService SC_Api;

        private readonly DateTime UtcNow = DateTime.UtcNow;

        public OrderManagement() : this(null) { }

        public OrderManagement(int? ID)
        {
            if (ID.HasValue) SetOrderData(ID.Value);
        }

        public void SetOrderData(int ID)
        {
            orderData = dbC.Orders.FirstOrDefault(o => o.ID.Equals(ID) || o.SCID.Value.Equals(ID));
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
                order.RMAID = orderData?.RMAID ?? dbC.RMA.FirstOrDefault(r => r.SCRMA.Equals(order.RMAID.ToString()))?.ID;

                var orderUpdateList = new string[] { "IsRush", "RMAID", "OrderStatus", "PaymentStatus", "PaymentDate", "FulfillmentStatus", "FulfilledDate", "ShippingTime", "Comment" };
                if (orderData != null)
                {
                    SetUpdateData(orderData, order, orderUpdateList);
                }
                else
                {
                    orderData = new Orders() { CreateBy = AdminName, CreateAt = UtcNow };
                    orderUpdateList = orderUpdateList.Concat(new string[] { "OrderParent", "OrderSourceID", "SCID", "CustomerID", "CustomerEmail", "OrderDate", "BuyerNote" }).ToArray();
                    SetUpdateData(orderData, order, orderUpdateList);
                    orderData.Company = dbC.Company.AsNoTracking().First(c => c.CompanySCID.Value.Equals(order.Company)).ID;
                    orderData.Channel = EnumData.OrderChannelList().First(c => c.Value.Equals(EnumData.OrderChannelList(true)[order.Channel])).Key;
                    dbC.Orders.Add(orderData);
                }

                dbC.SaveChanges();

                foreach (var package in order.Packages)
                {
                    package.ShipWarehouse = dbC.WarehouseSummary.AsNoTracking().First(w => w.IsEnable && w.Type.Equals("SCID") && w.Val.Equals(package.ShipWarehouse.ToString())).WarehouseID;
                    package.ReturnWarehouse = dbC.WarehouseSummary.AsNoTracking().First(w => w.IsEnable && w.Type.Equals("SCID") && w.Val.Equals(package.ReturnWarehouse.ToString())).WarehouseID;

                    if (package.ShippingMethod == 0)
                    {
                        package.ShippingMethod = dbC.ShippingMethods.First(m => m.IsEnable).ID;
                    }

                    if (orderData.Packages.Any(p => p.SCID.Value.Equals(package.SCID.Value)))
                    {
                        var packageData = orderData.Packages.First(p => p.SCID.Value.Equals(package.SCID.Value));
                        SetUpdateData(packageData, package, new string[] { "IsEnable", "CarrierBox", "ShippingMethod", "Export", "ExportMethod", "ExportValue", "UploadTracking", "Tracking", "DLExport", "DLExportMethod", "DLExportValue", "DLTracking", "ShipWarehouse", "ReturnWarehouse", "ShippingStatus", "FulfillmentDate" });
                    }
                    else
                    {
                        var ExportCurrency = Enum.GetName(typeof(CurrencyCodeType2), package.ExportCurrency);
                        package.ExportCurrency = dbC.Currency.AsNoTracking().First(c => c.Code.ToLower().Equals(ExportCurrency.ToLower())).ID;
                        var DLExportCurrency = Enum.GetName(typeof(CurrencyCodeType2), package.DLExportCurrency);
                        package.DLExportCurrency = dbC.Currency.AsNoTracking().First(c => c.Code.ToLower().Equals(DLExportCurrency.ToLower())).ID;
                        package.CreateBy = AdminName;
                        package.CreateAt = UtcNow;
                        orderData.Packages.Add(package);
                    }
                }

                dbC.SaveChanges();

                foreach (var item in order.Items)
                {
                    item.PackageID = dbC.Packages.AsNoTracking().First(p => p.SCID.Value.Equals(item.PackageID)).ID;

                    if (item.Sku.Any(s => new char[] { '-', '_' }.Contains(s)))
                    {
                        item.OriginSku = item.Sku;
                        item.Sku = item.Sku.Split('_')[0].Split('-')[0];
                    }

                    if (orderData.Items.Any(i => i.SCID.Value.Equals(item.SCID.Value)))
                    {
                        var itemData = orderData.Items.First(i => i.SCID.Value.Equals(item.SCID.Value));
                        SetUpdateData(itemData, item, new string[] { "IsEnable", "Sku", "OriginSku", "ExportValue", "DLExportValue", "Qty", "eBayItemID", "eBayTransationID", "SalesRecordNumber" });
                    }
                    else
                    {
                        item.CreateBy = AdminName;
                        item.CreateAt = UtcNow;
                        orderData.Items.Add(item);
                    }
                }

                dbC.SaveChanges();

                foreach (var item in orderData.Items.Where(i => !i.SCID.HasValue || !order.Items.Select(ii => ii.SCID.Value).Contains(i.SCID.Value)))
                {
                    item.IsEnable = false;
                }

                foreach(var package in orderData.Packages.Where(p => !p.Items.Any(i => i.IsEnable)))
                {
                    package.IsEnable = false;
                }

                dbC.SaveChanges();

                foreach (var serial in order.Serials)
                {
                    if (!orderData.Serials.Any(s => s.SerialNumber.Equals(serial.SerialNumber)))
                    {
                        serial.ItemID = dbC.Items.AsNoTracking().First(i => i.SCID.Value.Equals(serial.ItemID)).ID;
                        serial.CreateBy = AdminName;
                        serial.CreateAt = UtcNow;
                        orderData.Serials.Add(serial);
                    }
                }

                foreach(var serial in orderData.Serials.Where(s => !order.Serials.Select(ss => ss.SerialNumber).Contains(s.SerialNumber)))
                {
                    orderData.Serials.Remove(serial);
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

                var soldTo = order.Addresses.First();
                if (!orderData.Addresses.Any())
                {
                    soldTo.CreateBy = AdminName;
                    soldTo.CreateAt = UtcNow;
                    orderData.Addresses.Add(soldTo);

                    var shipTo = new OrderAddresses() { Type = (byte)EnumData.OrderAddressType.Shipped };
                    SetUpdateData(shipTo, soldTo, new string[] { "FirstName", "LastName", "AddressLine1", "AddressLine2", "City", "State", "Postcode", "CountryCode", "CountryName", "PhoneNumber", "EmailAddress", "CreateBy", "CreateAt" });
                    orderData.Addresses.Add(shipTo);
                }
                else
                {
                    var soldToData = orderData.Addresses.First(a => a.Type.Equals((byte)EnumData.OrderAddressType.Sold));
                    SetUpdateData(soldToData, soldTo, new string[] { "FirstName", "LastName", "AddressLine1", "AddressLine2", "City", "State", "Postcode", "CountryCode", "CountryName", "PhoneNumber", "EmailAddress" });
                    var shipToData = orderData.Addresses.First(a => a.Type.Equals((byte)EnumData.OrderAddressType.Shipped));
                    SetUpdateData(shipToData, soldTo, new string[] { "FirstName", "LastName", "AddressLine1", "AddressLine2", "City", "State", "Postcode", "CountryCode", "CountryName", "PhoneNumber", "EmailAddress" });
                }

                dbC.SaveChanges();

                orderData = dbC.Orders.AsNoTracking().First(o => o.ID.Equals(orderData.ID));
                orderData.OrderStatus = (byte)CheckOrderStatus(orderData);
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

        public EnumData.OrderStatus CheckOrderStatus(Orders order)
        {
            if (order.OrderStatus.Equals((byte)EnumData.OrderStatus.InProcess))
            {
                if (order.PaymentStatus.Equals((byte)EnumData.OrderPaymentStatus.None) || order.PaymentStatus.Equals((byte)EnumData.OrderPaymentStatus.Partial))
                {
                    return EnumData.OrderStatus.OnHold;
                }

                if (order.PaymentStatus.Equals((byte)EnumData.OrderPaymentStatus.Full) || order.PaymentStatus.Equals((byte)EnumData.OrderPaymentStatus.OverPaid))
                {
                    if (order.FulfillmentStatus.Equals((byte)EnumData.OrderFulfillmentStatus.Full))
                    {
                        return EnumData.OrderStatus.Completed;
                    }
                }
            }

            return (EnumData.OrderStatus)order.OrderStatus;
        }

        public EnumData.OrderFulfillmentStatus CheckFulfillmentStatus(Orders order)
        {
            if (order.Packages.Where(p => p.IsEnable).All(p => p.ShippingStatus.Equals((byte)EnumData.OrderShippingStatus.已出貨))) return EnumData.OrderFulfillmentStatus.Full;

            if (order.Packages.Where(p => p.IsEnable).Any(p => p.ShippingStatus.Equals((byte)EnumData.OrderShippingStatus.已出貨))) return EnumData.OrderFulfillmentStatus.Partial;

            return EnumData.OrderFulfillmentStatus.None;
        }

        public void MarkShip(int PackageID)
        {
            try
            {
                List<dynamic> data = new List<dynamic>();
                foreach (Items item in dbC.Packages.Find(PackageID).Items.Where(i => i.IsEnable))
                {
                    if (item.Serials.Any())
                    {
                        data.AddRange(item.Serials.Select(s => new
                        {
                            OrderID = s.Orders.SCID.Value,
                            SkuNo = s.Sku,
                            SerialsNo = s.SerialNumber,
                            QTY = 1
                        }).ToList());
                    }
                    else
                    {
                        data.Add(new
                        {
                            OrderID = item.GetOrder.SCID.Value,
                            SkuNo = item.Sku,
                            SerialsNo = "",
                            QTY = item.Qty
                        });
                    }
                }

                Response<Dictionary<string, List<string>>> response = Request<Dictionary<string, List<string>>>("Ajax/ShipmentByOrder", "post", data, 1);
                if (!response.Status) throw new Exception(response.Message);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.InnerException?.Message ?? ex.Message);
            }
        }

        /** SC Action **/
        public void SplitPackageToSC(int[] PackageIDs)
        {
            if (SC_Api == null) SC_Api = new SC_WebService(ApiUserName, ApiPassword);

            try
            {
                if (!orderData.SCID.HasValue) throw new Exception("Not found order's SCID!");

                if (!SC_Api.Is_login) throw new Exception("SC is not logged in!");

                var packageList = orderData.Packages.Where(p => PackageIDs.Contains(p.ID)).OrderBy(p => p.ID).ToList(); ;

                Packages packageData = packageList.First(p => p.SCID.HasValue);
                Order SC_order = SC_Api.Get_OrderData(orderData.SCID.Value).Order;
                Package SC_package = SC_order.Packages.FirstOrDefault(p => p.ID.Equals(packageData.SCID));

                if (SC_package == null) throw new Exception(string.Format("Not found SC package-{0}!", packageData.SCID));

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

                    foreach (var newItem in newPackage.Items.Where(i => i.IsEnable))
                    {
                        var SC_item = SC_order.Items.First(i => i.ProductID.Equals(newItem.Sku));
                        SC_item.PackageID = SC_package.ID;
                        SC_item = SC_Api.Add_OrderNewItem(SC_item);
                        newItem.SCID = SC_item.ID;
                    }
                }

                dbC.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("SC Error：{0}", ex.InnerException?.Message ?? ex.Message));
            }
        }

        public void CombinePackageToSC(int[] OldPackageIDs)
        {

            if (SC_Api == null) SC_Api = new SC_WebService(ApiUserName, ApiPassword);

            try
            {
                if (!orderData.SCID.HasValue) throw new Exception("Not found order's SCID!");

                if (!SC_Api.Is_login) throw new Exception("SC is not logged in!");

                var oldPackageList = orderData.Packages.Where(p => OldPackageIDs.Contains(p.ID)).OrderBy(p => p.ID).ToList(); ;

                Packages packageData = orderData.Packages.First(p => p.IsEnable && !p.SCID.HasValue);
                Order SC_order = SC_Api.Get_OrderData(orderData.SCID.Value).Order;

                var newPackage = SC_order.Packages.First(p => p.ID.Equals(oldPackageList.First().SCID.Value));
                newPackage.Qty = packageData.Items.Sum(i => i.Qty);
                newPackage = SC_Api.Add_OrderNewPackage(newPackage);
                packageData.SCID = newPackage.ID;

                foreach (var item in packageData.Items.Where(i => i.IsEnable))
                {
                    var newItem = SC_order.Items.First(i => i.ProductID.Equals(item.Sku));
                    newItem.PackageID = newPackage.ID;
                    newItem.Qty = item.Qty;
                    newItem = SC_Api.Add_OrderNewItem(newItem);
                    item.SCID = newItem.ID;
                }

                dbC.SaveChanges();

                foreach (var SC_package in SC_order.Packages.Where(p => oldPackageList.Select(pp => pp.SCID.Value).ToArray().Contains(p.ID)))
                {
                    if (SC_package == null) throw new Exception(string.Format("Not found SC package-{0}!", packageData.SCID));

                    foreach (var SC_item in SC_order.Items.Where(i => i.PackageID.Equals(SC_package.ID)))
                    {
                        SC_Api.Delete_Item1(SC_item.OrderID, SC_item.ID);
                    }
                    SC_Api.Delete_Package(SC_package.ID);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("SC Error：{0}", ex.InnerException?.Message ?? ex.Message));
            }
        }

        public void UpdateItemSerialToSC(int ItemID, string[] Serials)
        {
            if (SC_Api == null) SC_Api = new SC_WebService(ApiUserName, ApiPassword);

            try
            {
                if (!SC_Api.Is_login) throw new Exception("SC is not logged in!");

                Items itemData = dbC.Items.Find(ItemID);
                if (!itemData.SCID.HasValue) throw new Exception("Not found item's SCID!");

                SC_Api.Update_ItemSerialNumber(itemData.SCID.Value, Serials);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("SC Error：{0}", ex.InnerException?.Message ?? ex.Message));
            }
        }

        public void UpdateAddressToSC(int AddressID)
        {
            if (SC_Api == null) SC_Api = new SC_WebService(ApiUserName, ApiPassword);

            try
            {
                if (!orderData.SCID.HasValue) throw new Exception("Not found order's SCID!");

                if (!SC_Api.Is_login) throw new Exception("SC is not logged in!");

                var addressData = dbC.OrderAddresses.Find(AddressID);
                var SC_address = SC_Api.Get_OrderData(orderData.SCID.Value).Order.ShippingAddress;

                SC_address.FirstName = addressData.FirstName;
                SC_address.LastName = addressData.LastName;
                SC_address.StreetLine1 = addressData.AddressLine1;
                SC_address.StreetLine2 = addressData.AddressLine2;
                SC_address.City = addressData.City;
                SC_address.StateName = addressData.State;
                SC_address.PostalCode = addressData.Postcode;
                SC_address.CountryCode = addressData.CountryCode;
                SC_address.CountryName = addressData.CountryName;
                SC_address.PhoneNumber = addressData.PhoneNumber;
                SC_address.EmailAddress = addressData.EmailAddress;

                SC_Api.Update_OrderAddress(orderData.SCID.Value, SC_address);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("SC Error：{0}", ex.InnerException?.Message ?? ex.Message));
            }
        }

        public void UpdatePaymentToSC(int PaymentID)
        {
            if (SC_Api == null) SC_Api = new SC_WebService(ApiUserName, ApiPassword);

            try
            {
                if (!orderData.SCID.HasValue) throw new Exception("Not found order's SCID!");

                if (!SC_Api.Is_login) throw new Exception("SC is not logged in!");

                var paymentData = dbC.Payments.Find(PaymentID);
                var SC_order = SC_Api.Get_OrderData(orderData.SCID.Value).Order;
                var SC_payment = SC_order.Payments.First(p => p.ID.Equals(paymentData.SCID.Value));

                SC_order.ShippingTotal = paymentData.ShippingCharge;
                SC_order.InsuranceTotal = paymentData.InsuranceCharge;
                SC_order.GrandTotal = paymentData.GrandTotal;
                SC_payment.PaymentStatus = (PaymentStatus)EnumData.OrderPaymentStatusList(true).Select(s => (int)s.Key).ToArray()[paymentData.Status];
                SC_payment.AuditDate = paymentData.Date.HasValue ? new Helpers.TimeZoneConvert(paymentData.Date.Value, EnumData.TimeZone.UTC).ConvertDateTime(EnumData.TimeZone.EST) : DateTime.MinValue;
                SC_payment.PaymentMethod = (PaymentMethod)paymentData.Gateway;
                SC_payment.Amount = paymentData.PaymentTotal;

                SC_Api.Update_Order(SC_order);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("SC Error：{0}", ex.InnerException?.Message ?? ex.Message));
            }
        }

        public void MarkShipPackageToSC(int PackageID)
        {
            if (SC_Api == null) SC_Api = new SC_WebService(ApiUserName, ApiPassword);

            try
            {
                if (!orderData.SCID.HasValue) throw new Exception("Not found order's SCID!");

                if (!SC_Api.Is_login) throw new Exception("SC is not logged in!");

                Packages packageData = orderData.Packages.First(p => p.ID.Equals(PackageID));
                Order SC_order = SC_Api.Get_OrderData(orderData.SCID.Value).Order;
                Package SC_package = SC_order.Packages.FirstOrDefault(p => p.ID.Equals(packageData.SCID.Value));

                if (SC_package == null) throw new Exception(string.Format("Not found SC package-{0}!", packageData.SCID));

                SC_Api.Update_PackageShippingStatus(SC_package, (packageData.UploadTracking ? packageData.Tracking : ""), packageData.GetMethod.LastMile.Name);

                foreach (Items item in packageData.Items.Where(i => i.IsEnable).ToList())
                {
                    if (item.Serials.Any()) SC_Api.Update_ItemSerialNumber(item.ID, item.Serials.Select(s => s.SerialNumber).ToArray());
                }

                if (orderData.Packages.Where(p => p.IsEnable).All(p => p.ShippingStatus.Equals((byte)EnumData.OrderShippingStatus.已出貨)))
                {
                    SC_Api.Update_OrderShippingStatus(SC_order);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("SC Error：{0}", ex.InnerException?.Message ?? ex.Message));
            }
        }

        public void ChangeOrderStatusToSC(byte Status)
        {
            if (SC_Api == null) SC_Api = new SC_WebService(ApiUserName, ApiPassword);

            try
            {
                if (!orderData.SCID.HasValue) throw new Exception("Not found order's SCID!");

                if (!SC_Api.Is_login) throw new Exception("SC is not logged in!");

                var statusList = EnumData.OrderStatusList(true).Select(s => (int)s.Key).ToArray();
                SC_Api.Update_OrderStatus(orderData.SCID.Value, statusList[Status]);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("SC Error：{0}", ex.InnerException?.Message ?? ex.Message));
            }
        }

        public void ChangeOrderSkuToSC(int ItemID, string Sku)
        {
            if (SC_Api == null) SC_Api = new SC_WebService(ApiUserName, ApiPassword);

            try
            {
                if (!orderData.SCID.HasValue) throw new Exception("Not found order's SCID!");

                if (!SC_Api.Is_login) throw new Exception("SC is not logged in!");

                Items itemData = dbC.Items.Find(ItemID);
                if (!itemData.SCID.HasValue) throw new Exception("Not found item's SCID!");

                var SC_order = SC_Api.Get_OrderData(orderData.SCID.Value).Order;
                OrderItem SC_item = SC_order.Items.FirstOrDefault(i => i.ID.Equals(itemData.SCID.Value));

                if (SC_item == null) throw new Exception(string.Format("Not found SC item-{0}!", itemData.SCID));

                SC_item.ProductID = Sku;
                SC_Api.Update_OrderItem(SC_item);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("SC Error：{0}", ex.InnerException?.Message ?? ex.Message));
            }
        }

        public void ActionLog(string Item, string Description)
        {
            dbC.OrderActionLogs.Add(new OrderActionLogs()
            {
                OrderID = orderData.ID,
                Item = Item,
                Description = Description,
                CreateBy = AdminName,
                CreateAt = UtcNow
            });

            dbC.SaveChanges();
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
    }
}