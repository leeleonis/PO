using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using inventorySKU;
using PurchaseOrderSys.Models;

namespace PurchaseOrderSys.Controllers
{
    [CheckSession]
    public class OrderController : BaseController
    {
        private readonly string[] packageEditList = new string[] { "ShippingMethod", "Export", "ExportMethod", "ExportValue", "UploadTracking", "Tracking", "DLExport", "DLExportMethod", "DLExportValue", "DLTracking", "ShipWarehouse" };

        // GET: Order
        public ActionResult Index()
        {
            return View();
        }

        // GET: Order/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Orders order = db.Orders.Find(id);

            if (order == null)
            {
                using (var OM = new OrderManagement(id))
                {
                    try
                    {
                        order = OM.OrderSync(id);
                        if (order.CreateAt.CompareTo(order.UpdateAt.Value) == 0)
                        {
                            OM.ActionLog("Order", "Sync Data");
                        }
                    }
                    catch (Exception ex)
                    {
                        TempData["ErrorMsg"] = ex.InnerException?.Message ?? ex.Message;
                    }
                }
            }

            ViewBag.CompanyList = db.Company.AsNoTracking().Where(c => c.IsEnable).OrderBy(c => c.Name).Select(c => new SelectListItem() { Text = c.Name, Value = c.ID.ToString() }).ToList();
            ViewBag.MethodList = db.ShippingMethods.AsNoTracking().Where(m => m.IsEnable).OrderBy(m => m.Name).Select(m => new SelectListItem() { Text = m.Name, Value = m.ID.ToString() }).ToList();
            ViewBag.WarehouseList = db.Warehouse.AsNoTracking().Where(w => w.IsEnable && w.IsSellable).OrderBy(w => w.Name)
                .Select(w => new SelectListItem() { Text = w.Name, Value = w.ID.ToString() }).ToList();
            ViewBag.CurrencyList = db.Currency.AsNoTracking().Select(c => new SelectListItem() { Text = c.Code + " - " + c.Name, Value = c.ID.ToString() }).OrderBy(c => c.Text).ToList();

            return View(order);
        }

        // POST: Order/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IsRush,ID,OrderStatus,Comment")]Orders updateData)
        {
            Orders order = db.Orders.Find(updateData.ID);
            if (order == null) return HttpNotFound();

            using (var OM = new OrderManagement(updateData.ID))
            {
                bool StatusChange = order.OrderStatus != updateData.OrderStatus;

                if (!order.IsRush.Equals(updateData.IsRush))
                    OM.ActionLog("Change Rush", Enum.GetName(typeof(EnumData.YesNo), updateData.IsRush));

                if (!order.Comment.Equals(updateData.Comment))
                    OM.ActionLog("Change Comment", "");

                SetUpdateData(order, updateData, new string[] { "IsRush", "OrderStatus", "Comment" });
                order.OrderStatus = (byte)OM.CheckOrderStatus(order);
                db.SaveChanges();

                if (StatusChange)
                {
                    OM.ActionLog("Change Status", string.Format("Order to {0}", Enum.GetName(typeof(EnumData.OrderStatus), order.OrderStatus)));

                    if (order.SCID.HasValue)
                    {
                        JobProcess job = new JobProcess(string.Format("更改訂單【{0}】的狀態至SC", order.ID));
                        job.AddWord(() =>
                        {
                            try
                            {
                                job.AddLog("開始在SC上更改訂單狀態");
                                OM.SC_Api = new SellerCloud_WebService.SC_WebService(ApiUserName, ApiPassword);
                                OM.ChangeOrderStatusToSC(order.OrderStatus);
                                OM.OrderSyncPush();
                                job.AddLog("完成訂單狀態更改");
                            }
                            catch (Exception ex)
                            {
                                return ex.InnerException?.Message ?? ex.Message;
                            }

                            return "";
                        });
                    }
                }
            }

            ViewBag.CompanyList = db.Company.AsNoTracking().Where(c => c.IsEnable).OrderBy(c => c.Name).Select(c => new SelectListItem() { Text = c.Name, Value = c.ID.ToString() }).ToList();
            ViewBag.MethodList = db.ShippingMethods.AsNoTracking().Where(m => m.IsEnable).OrderBy(m => m.Name).Select(m => new SelectListItem() { Text = m.Name, Value = m.ID.ToString() }).ToList();
            ViewBag.WarehouseList = db.Warehouse.AsNoTracking().Where(w => w.IsEnable && w.IsSellable).OrderBy(w => w.Name)
                .Select(w => new SelectListItem() { Text = w.Name, Value = w.ID.ToString() }).ToList();
            ViewBag.CurrencyList = db.Currency.AsNoTracking().Select(c => new SelectListItem() { Text = c.Code + " - " + c.Name, Value = c.ID.ToString() }).OrderBy(c => c.Text).ToList();

            return View(order);
        }

        // GET: Order/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Orders order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Order/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Orders order = db.Orders.Find(id);
            db.Orders.Remove(order);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult AddressSave(OrderAddresses updateAddress)
        {
            AjaxResult result = new AjaxResult();

            var address = db.OrderAddresses.Find(updateAddress.ID);

            try
            {
                if (address == null) throw new Exception("Not found address!");

                CheckAddressData(address, updateAddress);

                if (address.GetOrder.SCID.HasValue)
                {
                    JobProcess job = new JobProcess(string.Format("更改訂單【{0}】的地址至SC", address.OrderID));
                    job.AddWord(() =>
                    {
                        try
                        {
                            using (var OM = new OrderManagement(address.OrderID))
                            {
                                job.AddLog("開始在SC上更改訂單地址");
                                OM.SC_Api = new SellerCloud_WebService.SC_WebService(ApiUserName, ApiPassword);
                                OM.UpdateAddressToSC(address.ID);
                                OM.OrderSyncPush();
                                job.AddLog("完成訂單地址更改");
                            }
                        }
                        catch (Exception ex)
                        {
                            return ex.InnerException?.Message ?? ex.Message;
                        }

                        return "";
                    });
                }

                var ShipsArray = new string[] { address.AddressLine1, address.AddressLine2, address.City, address.State, address.Postcode, address.CountryName, address.PhoneNumber };
                result.data = string.Join("\r\n", new string[] { string.Format("{0}, {1}", address.FirstName, address.LastName), string.Join("\r\n", ShipsArray.Except(new string[] { "", null })) });
            }
            catch (Exception e)
            {
                result.SetError(e.InnerException?.Message ?? e.Message);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private void CheckAddressData(OrderAddresses address, OrderAddresses updateAddress)
        {
            using (var OM = new OrderManagement(address.OrderID))
            {
                if (!address.FirstName.Equals(updateAddress.FirstName))
                    OM.ActionLog("Change Ship to", "First Name to " + updateAddress.FirstName);

                if (!address.LastName.Equals(updateAddress.LastName))
                    OM.ActionLog("Change Ship to", "Last Name to " + updateAddress.LastName);

                if (!address.AddressLine1.Equals(updateAddress.AddressLine1))
                    OM.ActionLog("Change Ship to", "Address1 to " + updateAddress.AddressLine1);

                if ((string.IsNullOrEmpty(address.AddressLine2) && !string.IsNullOrEmpty(updateAddress.AddressLine2)) || address.AddressLine2 != updateAddress.AddressLine2)
                    OM.ActionLog("Change Ship to", "Address2 to " + (updateAddress.AddressLine2 ?? "Empty"));

                if (!address.City.Equals(updateAddress.City))
                    OM.ActionLog("Change Ship to", "City to " + updateAddress.City);

                if (!address.State.Equals(updateAddress.State))
                    OM.ActionLog("Change Ship to", "State to " + updateAddress.State);

                if (!address.Postcode.Equals(updateAddress.Postcode))
                    OM.ActionLog("Change Ship to", "Postcode to " + updateAddress.Postcode);

                if (!address.CountryCode.Equals(updateAddress.CountryCode))
                {
                    address.CountryName = EnumData.CountryList()[updateAddress.CountryCode];
                    OM.ActionLog("Change Ship to", "Country to " + address.CountryName);
                }

                if (!address.PhoneNumber.Equals(updateAddress.PhoneNumber))
                    OM.ActionLog("Change Address", "PhoneNumber to " + updateAddress.PhoneNumber);

                SetUpdateData(address, updateAddress, new string[] { "FirstName", "LastName", "AddressLine1", "AddressLine2", "City", "State", "Postcode", "CountryCode", "PhoneNumber" });
                db.SaveChanges();
            }
        }

        [HttpPost]
        public ActionResult PaymentSave(Payments updatePayment)
        {
            AjaxResult result = new AjaxResult();

            var payment = db.Payments.Find(updatePayment.ID);

            try
            {
                if (payment == null) throw new Exception("Not found address!");

                updatePayment.GrandTotal = payment.TotalValue + updatePayment.ShippingCharge + updatePayment.InsuranceCharge;
                updatePayment.Balance = updatePayment.PaymentTotal - updatePayment.Refund - updatePayment.GrandTotal;
                CheckPaymentData(payment, updatePayment);

                if (payment.SCID.HasValue)
                {
                    JobProcess job = new JobProcess(string.Format("更改訂單【{0}】的帳單至SC", payment.OrderID));
                    job.AddWord(() =>
                    {
                        try
                        {
                            using (var OM = new OrderManagement(payment.OrderID))
                            {
                                job.AddLog("開始在SC上更改訂單帳單");
                                OM.SC_Api = new SellerCloud_WebService.SC_WebService(ApiUserName, ApiPassword);
                                OM.UpdatePaymentToSC(payment.ID);
                                OM.OrderSyncPush();
                                job.AddLog("完成訂單帳單更改");
                            }
                        }
                        catch (Exception ex)
                        {
                            return ex.InnerException?.Message ?? ex.Message;
                        }

                        return "";
                    });
                }
            }
            catch (Exception e)
            {
                result.SetError(e.InnerException?.Message ?? e.Message);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private void CheckPaymentData(Payments payment, Payments updatePayment)
        {
            using (var OM = new OrderManagement(payment.OrderID))
            {
                if (!payment.Status.Equals(updatePayment.Status))
                    OM.ActionLog("Change Payment", "Status to " + Enum.GetName(typeof(EnumData.PaymentStatus), updatePayment.Status));

                if ((!payment.Date.HasValue && updatePayment.Date.HasValue) || (payment.Date.Value.CompareTo(updatePayment.Date ?? DateTime.MinValue) != 0))
                    OM.ActionLog("Change Payment", "Date to " + (updatePayment.Date.HasValue ? updatePayment.Date.ToString() : "Empty"));

                if (!payment.Gateway.Equals(updatePayment.Gateway))
                    OM.ActionLog("Change Payment", "Gateway to " + Enum.GetName(typeof(SCService.PaymentMethod), updatePayment.Status));
                
                if (!payment.ShippingCharge.Equals(updatePayment.ShippingCharge))
                    OM.ActionLog("Change Payment", "Shipping to " + updatePayment.ShippingCharge);

                if (!payment.InsuranceCharge.Equals(updatePayment.InsuranceCharge))
                    OM.ActionLog("Change Payment", "Insurance to" + updatePayment.InsuranceCharge);

                if (!payment.GrandTotal.Equals(updatePayment.GrandTotal))
                    OM.ActionLog("Change Payment", "Grand Total to" + updatePayment.GrandTotal);

                if (!payment.PaymentTotal.Equals(updatePayment.PaymentTotal))
                    OM.ActionLog("Change Payment", "Payment Total to " + updatePayment.PaymentTotal);

                if (!payment.Refund.Equals(updatePayment.Refund))
                    OM.ActionLog("Change Payment", "Refund to " + updatePayment.Refund);

                if (!payment.Balance.Equals(updatePayment.Balance))
                    OM.ActionLog("Change Payment", "Balance to " + updatePayment.Balance);

                SetUpdateData(payment, updatePayment, new string[] { "Status", "Date", "Gateway", "ShippingCharge", "InsuranceCharge", "GrandTotal", "PaymentTotal", "Refund", "Balance" });
                db.SaveChanges();
            }
        }

        [HttpPost]
        public ActionResult PackageSave(Packages updatePackage)
        {
            AjaxResult result = new AjaxResult();

            var package = db.Packages.FirstOrDefault(p => p.ID.Equals(updatePackage.ID));

            try
            {
                if (package == null) throw new Exception("Not found package!");

                using (var OM = new OrderManagement(package.OrderID))
                {
                    OM.SC_Api = new SellerCloud_WebService.SC_WebService(ApiUserName, ApiPassword);

                    CheckPackageData(package, updatePackage);

                    bool OrderSync = false;
                    foreach (var item in package.Items.Where(i => i.IsEnable))
                    {
                        var updateItem = updatePackage.Items.First(i => i.ID.Equals(item.ID));

                        if (item.Sku != updateItem.Sku)
                        {
                            OM.ActionLog("Change Sku", string.Format("{0} to {1}", item.Sku, updateItem.Sku));
                            item.Sku = updateItem.Sku;

                            if (item.SCID.HasValue)
                            {
                                OM.ChangeOrderSkuToSC(item.ID, item.Sku);
                                OrderSync = true;
                            }
                        }

                        if (!item.ExportValue.Equals(updateItem.ExportValue))
                            OM.ActionLog("Change Export Value", string.Format("{0} to {1}", updateItem.Sku, updateItem.ExportValue));

                        if (!item.DLExportValue.Equals(updateItem.DLExportValue))
                            OM.ActionLog("Change DL Export Value", string.Format("{0} to {1}", updateItem.Sku, updateItem.DLExportValue));

                        if (!item.DLExportValue.Equals(updateItem.DLExportValue))
                            OM.ActionLog("Change QTY", string.Format("{0} to {1}", updateItem.Sku, updateItem.Qty));

                        SetUpdateData(item, updateItem, new string[] { "ExportValue", "DLExportValue", "Qty" });

                        var updateSerial = !string.IsNullOrEmpty(updateItem.SerialEdit) ? updateItem.SerialEdit.Split(',').Select(s => s.Trim()).ToArray() : new string[] { };
                        foreach (var serial in item.Serials)
                        {
                            serial.IsEnable = updateSerial.Contains(serial.SerialNumber);
                            serial.UpdateBy = package.UpdateBy;
                            serial.UpdateAt = package.UpdateAt.Value;

                            if (!serial.IsEnable)
                                OM.ActionLog("Remove Serial", serial.SerialNumber);
                        }

                        foreach (var newSerial in updateSerial.Except(item.Serials.Select(s => s.SerialNumber).ToArray()))
                        {
                            item.Serials.Add(new OrderSerials()
                            {
                                OrderID = item.OrderID,
                                ItemID = item.ID,
                                Sku = item.Sku,
                                SerialNumber = newSerial,
                                CreateBy = package.UpdateBy,
                                CreateAt = package.UpdateAt.Value
                            });

                            if (item.SCID.HasValue)
                            {
                                OM.UpdateItemSerialToSC(item.ID, new string[] { newSerial });
                                OrderSync = true;
                            }

                            OM.ActionLog("Add Serial", newSerial);
                        }
                    }

                    db.SaveChanges();

                    if (OrderSync) OM.OrderSyncPush();
                }

                result.data = PackageContent(package.ID);
            }
            catch (Exception ex)
            {
                result.SetError(ex.InnerException?.Message ?? ex.Message);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public string PackageContent(int PackageID)
        {
            var package = db.Packages.Find(PackageID);

            var ViewData = new ViewDataDictionary() {
                { "index", CheckPackageIndex(package) },
                { "total", db.Packages.Count(p => p.IsEnable && p.OrderID.Equals(package.OrderID)) },
                { "MethodList", db.ShippingMethods.AsNoTracking().Where(m => m.IsEnable).OrderBy(m => m.Name).Select(m => new SelectListItem() { Text = m.Name, Value = m.ID.ToString() }).ToList() },
                { "WarehouseList", db.Warehouse.AsNoTracking().Where(w => w.IsEnable && w.IsSellable).OrderBy(w => w.Name).Select(w => new SelectListItem() { Text = w.Name, Value = w.ID.ToString() }).ToList() },
                { "CurrencyList", db.Currency.AsNoTracking().Select(c => new SelectListItem() { Text = c.Code + " - " + c.Name, Value = c.ID.ToString() }).OrderBy(c => c.Text).ToList() }
            };

            return RenderViewToString(ControllerContext, "_PackageDetail", package, ViewData);
        }

        private void CheckPackageData(Packages package, Packages updatePackage)
        {
            using (var OM = new OrderManagement(package.OrderID))
            {
                if (!package.ShipWarehouse.Equals(updatePackage.ShipWarehouse))
                    OM.ActionLog("Change Warehouse", string.Format("Ship to {0}", db.Warehouse.Find(updatePackage.ShipWarehouse).Name));

                if (!package.ShippingMethod.Equals(updatePackage.ShippingMethod))
                    OM.ActionLog("Change Shipping Setting", string.Format("Method to {0}", db.ShippingMethods.Find(updatePackage.ShippingMethod).Name));

                if ((string.IsNullOrEmpty(package.Tracking) && !string.IsNullOrEmpty(updatePackage.Tracking)) || package.Tracking != updatePackage.Tracking)
                    OM.ActionLog("Change Shipping Setting", string.Format("Tracking to {0}", updatePackage.Tracking ?? "Empty"));

                if (!package.UploadTracking.Equals(updatePackage.UploadTracking))
                    OM.ActionLog("Change Shipping Setting", string.Format("Upload Tracking to {0}", Enum.GetName(typeof(EnumData.YesNo), updatePackage.UploadTracking)));

                if (!package.Export.Equals(updatePackage.Export))
                    OM.ActionLog("Change Shipping Setting", string.Format("Export to {0}", Enum.GetName(typeof(EnumData.Export), updatePackage.Export)));

                if (!package.ExportMethod.Equals(updatePackage.ExportMethod))
                    OM.ActionLog("Change Shipping Setting", string.Format("Export Method to {0}", Enum.GetName(typeof(EnumData.ExportMethod), updatePackage.ExportMethod)));

                if (!package.ExportValue.Equals(updatePackage.ExportValue))
                    OM.ActionLog("Change Shipping Setting", string.Format("Export Value to {0}", updatePackage.ExportValue.ToString()));

                if ((string.IsNullOrEmpty(package.DLTracking) && !string.IsNullOrEmpty(updatePackage.DLTracking)) || package.DLTracking != updatePackage.DLTracking)
                    OM.ActionLog("Change DL Shipping Setting", string.Format("Tracking to {0}", updatePackage.DLTracking ?? "Empty"));

                if (!package.DLExport.Equals(updatePackage.DLExport))
                    OM.ActionLog("Change DL Shipping Setting", string.Format("Export to {0}", Enum.GetName(typeof(EnumData.Export), updatePackage.DLExport)));

                if (!package.DLExportMethod.Equals(updatePackage.DLExportMethod))
                    OM.ActionLog("Change DL Shipping Setting", string.Format("Export Method to {0}", Enum.GetName(typeof(EnumData.ExportMethod), updatePackage.DLExportMethod)));

                if (!package.DLExportValue.Equals(updatePackage.DLExportValue))
                    OM.ActionLog("Change DL Shipping Setting", string.Format("Export Value to {0}", updatePackage.DLExportValue.ToString()));

                SetUpdateData(package, updatePackage, packageEditList);
                db.SaveChanges();
            }
        }

        private int CheckPackageIndex(Packages package)
        {
            var index = 1;
            var packageList = db.Packages.Where(p => p.IsEnable && p.OrderID.Equals(package.OrderID)).ToList();
            foreach (Packages p in packageList)
            {
                if (p.ID.Equals(package.ID)) return index;

                index++;
            }

            return 0;
        }

        public string SplitTable(int PackageID, int Qty)
        {
            var html = "";
            var package = db.Packages.Find(PackageID);

            try
            {
                if (package == null) throw new Exception("Not found package!");

                var itemList = package.Items.Where(i => i.IsEnable).ToList();
                if (itemList.Sum(i => i.Qty) < Qty || Qty < 2) throw new Exception("Split amount error!");

                int index = 0;
                bool isEdit = itemList.Sum(i => i.Qty) != Qty;
                if (!isEdit)
                {
                    foreach (var item in itemList)
                    {
                        for (int i = 0; i < item.Qty; i++)
                        {
                            html += RenderViewToString(ControllerContext, "_SplitQtyTable", package, new ViewDataDictionary()
                            {
                                { "isEdit", isEdit },
                                { "ID", index == 0 ? PackageID : 0 },
                                { "title", index == 0 ?string.Format("#{0}", PackageID): (index+1).ToString() },
                                { "items", itemList.Select(x => Tuple.Create(x.Sku, item.Sku.Equals(x.Sku) ? 1 : 0, x.Qty)).ToList() },
                                { "index", index++ }
                            });
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < Qty; i++)
                    {
                        html += RenderViewToString(ControllerContext, "_SplitQtyTable", package, new ViewDataDictionary()
                        {
                            { "isEdit", isEdit },
                            { "ID", index == 0 ? PackageID : 0 },
                            { "title", index == 0 ?string.Format("#{0}", PackageID): (index+1).ToString() },
                            { "items", itemList.Select(x => Tuple.Create(x.Sku, index == 0 ? x.Qty : 0, x.Qty)).ToList() },
                            { "index", index++ }
                        });
                    }
                }

                html += "<hr />" + RenderViewToString(ControllerContext, "_SplitQtyTable", package);
            }
            catch (Exception ex)
            {
                html = ex.InnerException?.Message ?? ex.Message;
            }

            return html;
        }

        [HttpPost]
        public ActionResult SplitPackage(int PackageID, Packages[] packageData)
        {
            AjaxResult result = new AjaxResult();

            try
            {
                var package = db.Packages.Find(PackageID);

                if (package == null) throw new Exception("Not found package!");

                if (!packageData.Any()) throw new Exception("Not found data!");

                var itemList = package.Items.Where(i => i.IsEnable).ToList();
                foreach (var update in packageData)
                {
                    if (update.ID != 0)
                    {
                        foreach (var item in itemList)
                        {
                            item.Qty = update.Items.First(i => i.Sku.Equals(item.Sku)).Qty;
                            item.IsEnable = item.Qty != 0;
                        }

                        package.ExportValue = itemList.Where(i => i.IsEnable).Sum(i => i.ExportValue * i.Qty);
                        package.DLExportValue = itemList.Where(i => i.IsEnable).Sum(i => i.DLExportValue * i.Qty);
                    }
                    else
                    {
                        var newPackage = new Packages()
                        {
                            OrderID = package.OrderID,
                            ExportCurrency = package.ExportCurrency,
                            DLExportCurrency = package.DLExportCurrency,
                            ReturnWarehouse = package.ReturnWarehouse,
                            CreateBy = Session["AdminName"].ToString(),
                            CreateAt = DateTime.UtcNow
                        };
                        SetUpdateData(newPackage, package, packageEditList);

                        foreach (var newItem in update.Items.Where(i => !i.Qty.Equals(0)))
                        {
                            SetUpdateData(newItem, itemList.First(i => i.Sku.Equals(newItem.Sku)), new string[] { "OrderID", "Sku", "OriginSku", "UnitPrice", "ExportValue", "DLExportValue", "eBayItemID", "eBayTransationID", "SalesRecordNumber" });
                            newItem.CreateBy = Session["AdminName"].ToString();
                            newItem.CreateAt = DateTime.UtcNow;
                            newPackage.Items.Add(newItem);
                        }

                        newPackage.ExportValue = newPackage.Items.Sum(i => i.ExportValue * i.Qty);
                        newPackage.DLExportValue = newPackage.Items.Sum(i => i.DLExportValue * i.Qty);
                        db.Packages.Add(newPackage);
                    }
                }
                db.SaveChanges();

                using (var OM = new OrderManagement(package.OrderID))
                {
                    OM.ActionLog("Change Shipping Actions", "Split Package");

                    JobProcess job = new JobProcess(string.Format("分批訂單【{0}】的包裹", package.OrderID));
                    job.AddWord(() =>
                    {
                        try
                        {
                            job.AddLog("開始在SC上進訂單分批");
                            OM.SC_Api = new SellerCloud_WebService.SC_WebService(ApiUserName, ApiPassword);
                            OM.SplitPackageToSC(packageData.Select(p => p.ID).ToArray());
                            OM.OrderSyncPush();
                            job.AddLog("完成包裹分批");
                        }
                        catch (Exception ex)
                        {
                            return ex.InnerException?.Message ?? ex.Message;
                        }

                        return "";
                    });
                }
            }
            catch (Exception ex)
            {
                result.SetError(ex.InnerException?.Message ?? ex.Message);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CombineTable(int[] PackageIDs)
        {
            AjaxResult result = new AjaxResult();

            try
            {
                if (PackageIDs == null || !PackageIDs.Any()) throw new Exception("Not get any package ID");

                var packageList = db.Packages.Where(p => PackageIDs.Contains(p.ID)).ToList();

                if (!packageList.Any() || packageList.Count() != 2) throw new Exception("The number of package is not 2! ");

                result.data = RenderViewToString(ControllerContext, "_CombineTable", null, new ViewDataDictionary() { { "packageList", packageList } });
            }
            catch (Exception ex)
            {
                result.SetError(ex.InnerException?.Message ?? ex.Message);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult CombinePackage(int[] oldPackageIDs, Items[] itemData)
        {
            AjaxResult result = new AjaxResult();

            try
            {
                if (oldPackageIDs == null || !oldPackageIDs.Any()) throw new Exception("Not get any odl package ID");

                if (itemData == null || !itemData.Any()) throw new Exception("Not get any combine sku");

                var oldPackageList = db.Packages.Where(p => oldPackageIDs.Contains(p.ID)).ToList();
                foreach (var oldPackage in oldPackageList)
                {
                    oldPackage.IsEnable = false;
                    oldPackage.UpdateBy = Session["AdminName"].ToString();
                    oldPackage.UpdateAt = DateTime.UtcNow;

                    foreach (var oldItem in oldPackage.Items.Where(i => i.IsEnable))
                    {
                        oldItem.IsEnable = false;
                        oldItem.UpdateBy = Session["AdminName"].ToString();
                        oldItem.UpdateAt = DateTime.UtcNow;
                    }
                }

                var package = oldPackageList.First();
                var newPackage = new Packages()
                {
                    OrderID = package.OrderID,
                    ExportCurrency = package.ExportCurrency,
                    DLExportCurrency = package.DLExportCurrency,
                    ReturnWarehouse = package.ReturnWarehouse,
                    CreateBy = Session["AdminName"].ToString(),
                    CreateAt = DateTime.UtcNow
                };
                SetUpdateData(newPackage, package, packageEditList);

                foreach (var newItem in itemData)
                {
                    newItem.CreateBy = Session["AdminName"].ToString();
                    newItem.CreateAt = DateTime.UtcNow;
                    newPackage.Items.Add(newItem);
                }

                newPackage.ExportValue = newPackage.Items.Sum(i => i.ExportValue * i.Qty);
                newPackage.DLExportValue = newPackage.Items.Sum(i => i.DLExportValue * i.Qty);
                db.Packages.Add(newPackage);
                db.SaveChanges();

                using (var OM = new OrderManagement(newPackage.OrderID))
                {
                    OM.ActionLog("Change Shipping Actions", "Combine Packages");

                    if (oldPackageList.All(p => p.SCID.HasValue))
                    {
                        JobProcess job = new JobProcess(string.Format("整合訂單【{0}】的包裹", package.OrderID));
                        job.AddWord(() =>
                        {
                            try
                            {
                                job.AddLog("開始在SC上進行包裹整合");
                                OM.SC_Api = new SellerCloud_WebService.SC_WebService(ApiUserName, ApiPassword);
                                OM.CombinePackageToSC(oldPackageIDs);
                                OM.OrderSyncPush();
                                job.AddLog("完成包裹整合");
                            }
                            catch (Exception ex)
                            {
                                return ex.InnerException?.Message ?? ex.Message;
                            }

                            return "";
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                result.SetError(ex.InnerException?.Message ?? ex.Message);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult PackageMarkShip(int PackageID)
        {
            AjaxResult result = new AjaxResult();

            try
            {
                var package = db.Packages.Find(PackageID);

                if (package == null) throw new Exception("Not found package!");

                using (var OM = new OrderManagement(package.OrderID))
                {
                    OM.ActionLog("Package", "Mark Ship");
                    OM.MarkShip(package.ID);

                    package.ShippingStatus = (byte)EnumData.OrderShippingStatus.已出貨;

                    var order = db.Orders.Find(package.OrderID);
                    var fulfillmentStatus = OM.CheckFulfillmentStatus(order);

                    if (!order.FulfillmentStatus.Equals((byte)fulfillmentStatus))
                    {
                        order.FulfillmentStatus = (byte)fulfillmentStatus;
                        OM.ActionLog("Change Status", string.Format("Fulfillment to", fulfillmentStatus.ToString()));

                        if (order.FulfillmentStatus.Equals((byte)EnumData.OrderFulfillmentStatus.Full))
                        {
                            order.OrderStatus = (byte)EnumData.OrderStatus.Completed;
                            OM.ActionLog("Change Status", "Order to Complete");
                        }
                    }

                    if (order.SCID.HasValue)
                    {
                        JobProcess job = new JobProcess(string.Format("Mark Ship訂單【{0}】", package.OrderID));
                        job.AddWord(() =>
                        {
                            try
                            {
                                job.AddLog("開始在SC上進行包裹Mrak Ship");
                                OM.SC_Api = new SellerCloud_WebService.SC_WebService(ApiUserName, ApiPassword);
                                OM.MarkShipPackageToSC(package.ID);
                                OM.OrderSyncPush();
                                job.AddLog("完成包裹Mrak Ship");
                            }
                            catch (Exception ex)
                            {
                                return ex.InnerException?.Message ?? ex.Message;
                            }

                            return "";
                        });
                    }
                }

                db.SaveChanges();
            }
            catch (Exception ex)
            {
                result.SetError(ex.InnerException?.Message ?? ex.Message);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
