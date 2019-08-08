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
                            html += RenderViewToString(ControllerContext, "_SkuQtyTable", package, new ViewDataDictionary()
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
                        html += RenderViewToString(ControllerContext, "_SkuQtyTable", package, new ViewDataDictionary()
                        {
                            { "isEdit", isEdit },
                            { "ID", index == 0 ? PackageID : 0 },
                            { "title", index == 0 ?string.Format("#{0}", PackageID): (index+1).ToString() },
                            { "items", itemList.Select(x => Tuple.Create(x.Sku, index == 0 ? x.Qty : 0, x.Qty)).ToList() },
                            { "index", index++ }
                        });
                    }
                }

                html += "<hr />" + RenderViewToString(ControllerContext, "_SkuQtyTable", package);
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
                    }
                    else
                    {
                        SetUpdateData(update, package, packageEditList);
                        update.OrderID = package.OrderID;
                        update.ExportCurrency = package.ExportCurrency;
                        update.DLExportCurrency = package.DLExportCurrency;
                        update.ReturnWarehouse = package.ReturnWarehouse;
                        update.CreateBy = Session["AdminName"].ToString();
                        update.CreateAt = DateTime.UtcNow;

                        foreach (var newItem in update.Items)
                        {
                            if (newItem.Qty != 0)
                            {
                                SetUpdateData(newItem, itemList.First(i => i.Sku.Equals(newItem.Sku)), new string[] { "OrderID", "Sku", "OriginSku", "UnitPrice", "ExportValue", "DLExportValue" });
                                newItem.CreateBy = Session["AdminName"].ToString();
                                newItem.CreateAt = DateTime.UtcNow;
                            }
                        }

                        db.Packages.Add(update);
                    }
                    db.SaveChanges();
                }

                if (package.GetOrder.SCID.HasValue)
                {
                    try
                    {
                        using (var OM = new OrderManagement(package.OrderID))
                        {
                            OM.SC_Api = new SellerCloud_WebService.SC_WebService(ApiUserName, ApiPassword);
                            OM.SplitPackage(packageData.Select(p => p.ID).ToArray());
                            OM.OrderSyncPush();
                        }
                    }
                    catch (Exception ex)
                    {
                        TempData["ErrorMsg"] = ex.InnerException?.Message ?? ex.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                result.SetError(ex.InnerException?.Message ?? ex.Message);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

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
                    var order = db.Orders.Find(package.OrderID);
                    order.FulfillmentStatus = (byte)OM.CheckFulfillmentStatus(order);
                    if (order.FulfillmentStatus.Equals((byte)EnumData.OrderFulfilledStatus.Fulfilled))
                    {
                        order.OrderStatus = (byte)EnumData.OrderStatus.Completed;
                    }

                    if (order.SCID.HasValue)
                    {
                        OM.SC_Api = new SellerCloud_WebService.SC_WebService(ApiUserName, ApiPassword);
                        OM.MarkShipPackage(package.ID);
                        OM.OrderSyncPush();
                    }
                }
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
