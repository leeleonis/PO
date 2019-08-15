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
        private readonly string[] packageEditList = new string[] { "ShippingMethod", "Export", "ExportMethod", "ExportValue", "UploadTracking", "Tracking", "DLExport", "DLExportMethod", "DLExportValue", "DLUploadTracking", "DLTracking", "ShipWarehouse" };

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
                    order = OM.OrderSync(id);
                    if (order.CreateAt.CompareTo(order.UpdateAt.Value) == 0)
                    {
                        OM.ActionLog("Order", "Sync Data");
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

            SetUpdateData(order, updateData, new string[] { "IsRush", "OrderStatus", "Comment" });
            db.SaveChanges();

            return RedirectToAction("Edit", new { order.ID });
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

                if (address.CountryCode != updateAddress.CountryCode) address.CountryName = EnumData.CountryList()[updateAddress.CountryCode];
                SetUpdateData(address, updateAddress, new string[] { "FirstName", "LastName", "AddressLine1", "AddressLine2", "City", "State", "Postcode", "CountryCode", "PhoneNumber" });
                db.SaveChanges();

                var ShipsArray = new string[] { address.AddressLine1, address.AddressLine2, address.City, address.State, address.Postcode, address.CountryName, address.PhoneNumber };
                result.data = string.Join("\r\n", new string[] { string.Format("{0}, {1}", address.FirstName, address.LastName), string.Join("\r\n", ShipsArray.Except(new string[] { "", null })) });
            }
            catch (Exception e)
            {
                result.SetError(e.InnerException?.Message ?? e.Message);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult PackageSave(Packages updatePackage)
        {
            AjaxResult result = new AjaxResult();

            var package = db.Packages.AsNoTracking().First(p => p.ID.Equals(updatePackage.ID));

            try
            {
                if (package == null) throw new Exception("Not found package!");

                SetUpdateData(package, updatePackage, packageEditList);
                foreach (var item in package.Items.Where(i => i.IsEnable))
                {
                    var updateItem = updatePackage.Items.First(i => i.ID.Equals(item.ID));
                    SetUpdateData(item, updateItem, new string[] { "ExportValue", "DLExportValue", "Qty" });

                    if (item.Sku != updateItem.Sku)
                    {
                        item.Sku = updateItem.Sku;
                    }

                    var updateSerial = !string.IsNullOrEmpty(item.SerialEdit) ? item.SerialEdit.Split(',').Select(s => s.Trim()).ToArray() : new string[] { };
                    foreach (var serial in item.Serials)
                    {
                        serial.IsEnable = updateSerial.Contains(serial.SerialNumber);
                        serial.UpdateAt = package.UpdateAt.Value;
                        serial.UpdateBy = package.UpdateBy;
                    }

                    foreach (var newSerial in updateSerial.Except(item.Serials.Select(s => s.SerialNumber).ToArray()))
                    {
                        item.Serials.Add(new OrderSerials()
                        {
                            OrderID = item.OrderID,
                            ItemID = item.ID,
                            Sku = item.Sku,
                            SerialNumber = newSerial,
                            CreateBy = package.UpdateBy
                        });
                    }
                }

                db.SaveChanges();

                var ViewData = new ViewDataDictionary() {
                    { "MethodList", db.ShippingMethods.AsNoTracking().Where(m => m.IsEnable).OrderBy(m => m.Name).Select(m => new SelectListItem() { Text = m.Name, Value = m.ID.ToString() }).ToList() },
                    { "WarehouseList", db.Warehouse.AsNoTracking().Where(w => w.IsEnable && w.IsSellable).OrderBy(w => w.Name).Select(w => new SelectListItem() { Text = w.Name, Value = w.ID.ToString() }).ToList() },
                    { "CurrencyList", db.Currency.AsNoTracking().Select(c => new SelectListItem() { Text = c.Code + " - " + c.Name, Value = c.ID.ToString() }).OrderBy(c => c.Text).ToList() }
                };

                result.data = RenderViewToString(ControllerContext, "_PackageDetail", package, ViewData);
            }
            catch (Exception ex)
            {
                result.SetError(ex.InnerException?.Message ?? ex.Message);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
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
                    if (package.GetOrder.SCID.HasValue)
                    {
                        try
                        {
                            OM.SC_Api = new SellerCloud_WebService.SC_WebService(ApiUserName, ApiPassword);
                            OM.SplitPackage(packageData.Select(p => p.ID).ToArray());
                            OM.OrderSyncPush();
                        }
                        catch (Exception ex)
                        {
                            TempData["ErrorMsg"] = ex.InnerException?.Message ?? ex.Message;
                        }
                    }

                    OM.ActionLog("Package", "Split Package");
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

                    foreach(var oldItem in oldPackage.Items.Where(i => i.IsEnable))
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
                    if (oldPackageList.All(p => p.SCID.HasValue))
                    {
                        try
                        {
                            OM.SC_Api = new SellerCloud_WebService.SC_WebService(ApiUserName, ApiPassword);
                            OM.CombinePackage(oldPackageIDs);
                            OM.OrderSyncPush();
                        }
                        catch (Exception ex)
                        {
                            TempData["ErrorMsg"] = ex.InnerException?.Message ?? ex.Message;
                        }
                    }

                    OM.ActionLog("Package", "Combine Packages");
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

                package.ShippingStatus = (byte)EnumData.OrderShippingStatus.已出貨;

                using (var OM = new OrderManagement(package.OrderID))
                {
                    OM.ActionLog("Package", "Mark Ship");

                    var order = db.Orders.Find(package.OrderID);
                    var fulfillmentStatus = OM.CheckFulfillmentStatus(order);

                    if (!order.FulfillmentStatus.Equals((byte)fulfillmentStatus))
                    {
                        order.FulfillmentStatus = (byte)fulfillmentStatus;
                        OM.ActionLog("Fulfillment", fulfillmentStatus.ToString());

                        if (order.FulfillmentStatus.Equals((byte)EnumData.OrderFulfilledStatus.Fulfilled))
                        {
                            order.OrderStatus = (byte)EnumData.OrderStatus.Completed;
                            OM.ActionLog("Order", "Complete");
                        }
                    }


                    if (order.SCID.HasValue)
                    {
                        OM.SC_Api = new SellerCloud_WebService.SC_WebService(ApiUserName, ApiPassword);
                        OM.MarkShipPackage(package.ID);
                        OM.OrderSyncPush();
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
