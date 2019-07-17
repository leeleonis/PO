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
        // GET: Order
        public ActionResult Index()
        {
            return View();
        }

        // GET: Order/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            //Orders order = db.Orders.FirstOrDefault(o => o.ID.Equals(id.Value) || o.SCID.Value.Equals(id.Value));
            Orders order;

            using (var OM = new OrderManagement(id))
            {
                try
                {
                    order = OM.OrderSync(id);
                    if (order.CreateAt.CompareTo(order.UpdateAt.Value) == 0)
                    {
                        order.ActionLogs.Add(new OrderActionLogs()
                        {
                            OrderID = order.ID,
                            Item = "Order",
                            Description = "Sync Data",
                            CreateBy = Session["AdminName"].ToString(),
                            CreateAt = DateTime.UtcNow
                        });

                        db.SaveChanges();
                    }
                }
                catch (Exception e)
                {
                    throw new Exception(e.InnerException?.Message ?? e.Message);
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
        public ActionResult Edit([Bind(Include = "IsRush,ID,OrderStatus,Comment")]Orders order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
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

                SetUpdateData(package, updatePackage, new string[] { "ShippingMethod", "Export", "ExportMethod", "ExportValue", "UploadTracking", "Tracking", "DLExport", "DLExportMethod", "DLExportValue", "DLUploadTracking", "DLTracking", "ShipWarehouse" });
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
            catch (Exception e)
            {
                result.SetError(e.InnerException?.Message ?? e.Message);
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
