using PurchaseOrderSys.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PurchaseOrderSys.Controllers
{
    public class TransferController : BaseController
    {
        // GET: Transfer
        public ActionResult Index()
        {
            var Transferlist = db.Transfer.Where(x => x.IsEnable);
            return View(Transferlist);
        }
        public ActionResult Create()
        {
            var Warehouselist = db.Warehouse.Where(x => x.IsEnable).Select(x => new SelectListItem { Text = x.Name, Value = x.ID.ToString() }).ToList();
            ViewBag.Warehouselist = Warehouselist;
            Session["TSkuNumberList"] = null;
            return View();
        }
        [HttpPost]
        public ActionResult Create(Transfer Transfer)
        {
            var CreateBy = UserBy;
            var CreateAt = DateTime.UtcNow;
            Transfer.CreateBy = CreateBy;
            Transfer.CreateAt = CreateAt;
            var odataList = (List<TranSKUVM>)Session["TSkuNumberList"];
            foreach (var item in odataList)
            {
                Transfer.TransferSKU.Add(new TransferSKU { SkuNo = item.SKU, QTY = item.QTY, CreateBy = CreateBy, CreateAt = CreateAt });
            }
            db.Transfer.Add(Transfer);
            db.SaveChanges();
            Session["TSkuNumberList"] = null;
            return RedirectToAction("Index");
        }
        public ActionResult Edit(int ID)
        {
            var Warehouselist = db.Warehouse.Where(x => x.IsEnable).Select(x => new SelectListItem { Text = x.Name, Value = x.ID.ToString() }).ToList();
            ViewBag.Warehouselist = Warehouselist;
            var Transfer = db.Transfer.Find(ID);
            var TranSKUVMList = new List<TranSKUVM>();
            foreach (var item in Transfer.TransferSKU)
            {
                TranSKUVMList.Add(new TranSKUVM
                {
                    ck = item.SkuNo,
                    sk = item.SkuNo,
                    SKU = item.SkuNo,
                    ProductName = item.Name,
                    QTY = item.QTY
                });
            }

            Session["TSkuNumberList"] = TranSKUVMList;
            return View(Transfer);
        }
        [HttpPost]
        public ActionResult Edit(Transfer Transfer)
        {
            var dt = DateTime.UtcNow;
            var oTransfer = db.Transfer.Find(Transfer.ID);
            oTransfer.ExternalTra = Transfer.ExternalTra;
            oTransfer.Title = Transfer.Title;
            oTransfer.FromWID = Transfer.FromWID;
            oTransfer.ToWID = Transfer.ToWID;
            oTransfer.Status = Transfer.Status;
            oTransfer.Interim = Transfer.Interim;
            oTransfer.Carrier = Transfer.Carrier;
            oTransfer.Tracking = Transfer.Tracking;
            oTransfer.UpdateBy = UserBy;
            oTransfer.UpdateAt = dt;

            var odataList = (List<TranSKUVM>)Session["TSkuNumberList"];
            foreach (var item in odataList)
            {
                var TransferSKUList = oTransfer.TransferSKU.Where(x => x.SkuNo == item.SKU);
                if (TransferSKUList.Any())
                {
                    foreach (var SKUitem in TransferSKUList)
                    {
                        if (SKUitem.QTY != item.QTY)
                        {
                            SKUitem.QTY = item.QTY;
                            SKUitem.UpdateBy = UserBy;
                            SKUitem.UpdateAt = dt;
                        }
                    }
                }
                else
                {
                    oTransfer.TransferSKU.Add(new TransferSKU { SkuNo = item.SKU, Name = item.ProductName, QTY = item.QTY, CreateBy = UserBy, CreateAt = dt });
                }

            }
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Delete(int ID)
        {
            return View();
        }
        public ActionResult Prep(int ID)
        {
            var oTransfer = db.Transfer.Find(ID);
            var PrepVMList = new List<PrepVM>();
            foreach (var item in oTransfer.TransferSKU)
            {
                PrepVMList.Add(new PrepVM { SKU = item.SkuNo, Name = item.Name, QTY = item.QTY, SerialsLlist = item.SerialsLlist.Select(x => x.SerialsNo).ToList() });
            }
            Session["PrepVMList"] = PrepVMList;
            return View(oTransfer);
        }
        public ActionResult Saveserials(string serials)
        {
            var PrepVMList = (List<PrepVM>)Session["PrepVMList"];
            
            var SerialsLlist = db.SerialsLlist.Where(x => x.SerialsNo == serials);//找到序號
            if (SerialsLlist.Any())
            {
                if (SerialsLlist.Sum(c => c.SerialsQTY) == 1)
                {
                    if (PrepVMList[0].QTY > PrepVMList[0].SerialsLlist.Count())
                    {
                        PrepVMList[0].SerialsLlist.Add(serials);
                        Session["PrepVMList"] = PrepVMList;
                        return Json(new { status = true }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { status = false, Errmsg = "移倉數量超過" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { status = false, Errmsg = "序號不在倉庫，此序號不能移倉" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { status = false, Errmsg = "序號不存在，此序號不能移倉" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult PrepVMList()
        {
            var PrepVMList = (List<PrepVM>)Session["PrepVMList"];
            var PrepTableList = new List<PrepTable>();
            if (PrepVMList != null)
            {
                foreach (var item in PrepVMList)
                {
                    if (item.SerialsLlist.Any())
                    {
                        foreach (var SerialsLlistitem in item.SerialsLlist)
                        {
                            PrepTableList.Add(new PrepTable { SKU = item.SKU, Name = item.Name, Serial = SerialsLlistitem, QTY = item.QTY + "/" + item.SerialsLlist.Count() });
                        }
                    }
                    else
                    {
                        PrepTableList.Add(new PrepTable { SKU = item.SKU, Name = item.Name, QTY = item.QTY + "/" + item.SerialsLlist.Count() });
                    }
                }
            }
            int recordsTotal = PrepTableList.Count();
            var returnObj =
           new
           {
               recordsFiltered = recordsTotal,
               data = PrepTableList//分頁後的資料 
           };
            return Json(returnObj, JsonRequestBehavior.AllowGet);
        }
    }
}