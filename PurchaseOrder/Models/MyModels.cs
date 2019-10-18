using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;

namespace PurchaseOrderSys.Models
{

    public static class EnumerableExtender
    {
        public static IEnumerable<TSource> Distinct<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                var elementValue = keySelector(element);
                if (seenKeys.Add(elementValue))
                {
                    yield return element;
                }
            }
        }
    }
    public class PasswordChange
    {
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required")]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
    }
    public class DataGridModels
    {
        /// <summary>
        /// DataGrid的名稱，可ID,class
        /// </summary>
        public string DataGridName { get; set; }
        /// <summary>
        /// 是否顯示checkbox
        /// </summary>
        public bool checkbox { get; set; }
        /// <summary>
        /// 表格名稱
        /// </summary>
        public string title { get; set; }
        public string idField { get; set; }
        /// <summary>
        /// 顯示新增子項目
        /// </summary>
        public bool additem { get; set; }
        /// <summary>
        /// 是否顯修改按鈕
        /// </summary>
        public bool edititem { get; set; }
        /// <summary>
        /// 是否顯示刪除按鈕
        /// </summary>
        public bool delitem { get; set; }
        /// <summary>
        /// 是否顯示儲存按鈕
        /// </summary>
        public bool saveitem { get; set; }
        /// <summary>
        /// 顯示子表
        /// </summary>
        public bool showchilds { get; set; }
        public List<DataGridItemsModels> DataGridItems { get; set; }

    }
    public class DataGridItemsModels
    {
        public string field { get; set; }
        public string title { get; set; }
        public bool frozen { get; set; }
        public float width { get; set; }
        public string align { get; set; }
        public string sortable { get; set; }
        public bool formatter { get; set; }
        public string columnsType { get; set; }
    }
    public class SelectItem
    {
        public string id { get; set; }
        public string text { get; set; }
        public bool disabled { get; set; }
    }
    public class Country
    {
        private RegionInfo info;

        public string ID { get { return info.TwoLetterISORegionName; } }
        public string Name { get { return info.EnglishName; } }
        public string ChtName { get { return info.DisplayName; } }
        public string TwoCode { get { return info.TwoLetterISORegionName; } }
        public string ThreeCode { get { return info.ThreeLetterISORegionName; } }

        public Country(int LCID)
        {
            info = new RegionInfo(LCID);
        }

        public Country(string Name)
        {
            info = new RegionInfo(Name);
        }

        public string OriginName
        {
            get
            {
                switch (ID)
                {
                    case "CN":
                        return "China";
                    case "TW":
                        return "Taiwan";
                    case "US":
                        return "USA";
                    default:
                        return Name;
                }
            }
        }
    }

    public partial class PurchaseOrderEntities : DbContext
    {
        /// <summary>
        /// 倉庫等待出貨的庫總量
        /// </summary>
        /// <param name="SKU">SKU</param>
        /// <param name="SCID">SCID</param>
        /// <returns></returns>
        public List<AwaitingDispatchVM> GetAwaitingCount(string SKU, string SCID)
        {
            var SKUs = new string[] { SKU };
            var SCIDs = new string[] { SCID };
            return GetAwaitingCount(SKUs, SCIDs);
        }
        /// <summary>
        /// 倉庫等待出貨的庫總量
        /// </summary>
        /// <param name="SKUs">SKU</param>
        /// <param name="SCIDs">SCID</param>
        /// <returns></returns>
        public List<AwaitingDispatchVM> GetAwaitingCount(string[] SKUs, string[] SCIDs)
        {

            var AwaitingDispatchList = new List<AwaitingDispatchVM>();
            using (WebClient wc = new WebClient())
            {
                try
                {
                    SKUs = SKUs.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
                    SCIDs = SCIDs.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
                    if (SCIDs.Any())
                    {
                        string ApiUrl = "http://internal.qd.com.tw/";
                        wc.Encoding = Encoding.UTF8;
                        var nDictionary = new GetSkuInventoryQTYVM { WarehouseIDs = SCIDs, Skus = SKUs };
                        var dataString = JsonConvert.SerializeObject(nDictionary);
                        wc.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                        string resultXML = wc.UploadString(ApiUrl + "Api/GetSkuInventoryQTY", "POST", dataString);
                        AwaitingDispatchList = JsonConvert.DeserializeObject<List<AwaitingDispatchVM>>(resultXML);
                    }
                }
                catch (WebException ex)
                {

                }
            }
            return AwaitingDispatchList;
        }
        //public override int SaveChanges()
        //{
        //    var modifiedEntities = ChangeTracker.Entries().Where(x => x.State == EntityState.Added || x.State == EntityState.Modified).ToList();
        //    base.SaveChanges();
        //    try
        //    {
        //        if (modifiedEntities.Count < 10)
        //        {
        //            foreach (var change in modifiedEntities)
        //            {
        //                var entityName = change.Entity.GetType().FullName;
        //                var bentityName = change.Entity.GetType().BaseType.FullName;
        //                if (entityName == "PurchaseOrderSys.Models.SerialsLlist" || bentityName == "PurchaseOrderSys.Models.SerialsLlist")
        //                {
        //                    var edt = DateTime.UtcNow;
        //                    var sdt = edt.AddDays(-30);
        //                    var ESerialsLlist = ((SerialsLlist)change.Entity);
        //                    var SkuNo = ESerialsLlist.PurchaseSKU?.SkuNo ?? ESerialsLlist.TransferSKU.SkuNo;
        //                    //var WarehouseList = new List<int?> { ESerialsLlist.PurchaseSKU?.PurchaseOrder.WarehouseID, ESerialsLlist.TransferSKU?.Transfer.ToWID, ESerialsLlist.TransferSKU?.Transfer.FromWID };
        //                    //WarehouseList = WarehouseList.Where(x => x.HasValue).ToList();
        //                    var dbWarehouse = Warehouse.AsNoTracking().Where(x => x.IsEnable).Include(x => x.WarehouseSummary).ToList();//&& WarehouseList.Contains(x.ID)
        //                    var dbAllSKUList = SKU.AsNoTracking().Where(x => x.IsEnable && x.SkuID == SkuNo).Select(x => x.SkuID).ToList();// && x.Status == 1
        //                    var dbPOSerialsLlist = SerialsLlist.AsNoTracking().Where(x => x.IsEnable && !x.SerialsLlistC.Any(y => y.IsEnable) && x.SerialsType == "PO" && x.PurchaseSKU.IsEnable && x.PurchaseSKU.PurchaseOrder.IsEnable).Include(x => x.PurchaseSKU).Include(x => x.PurchaseSKU.PurchaseOrder).ToList();
        //                    var dbInSerialsLlist = SerialsLlist.AsNoTracking().Where(x => x.IsEnable && !x.SerialsLlistC.Any(y => y.IsEnable) && x.SerialsType == "TransferIn" && x.TransferSKU.IsEnable && x.TransferSKU.Transfer.IsEnable).Include(x => x.TransferSKU).Include(x => x.TransferSKU.Transfer).ToList();
        //                    var dbOutSerialsLlist = SerialsLlist.AsNoTracking().Where(x => x.IsEnable && !x.SerialsLlistC.Any(y => y.IsEnable) && x.SerialsType == "TransferOut" && x.TransferSKU.IsEnable && x.TransferSKU.Transfer.IsEnable).Include(x => x.TransferSKU).Include(x => x.TransferSKU.Transfer).ToList();
        //                    var dbRMAINSerialsLlist = RMASerialsLlist.AsNoTracking().Where(x => x.IsEnable && !x.RMASerialsLlistC.Any(y => y.IsEnable) && x.SerialsType == "RMAIn" && x.RMASKU.IsEnable && x.RMASKU.RMA.IsEnable).Include(x => x.RMASKU).Include(x => x.RMASKU.RMA).ToList();
        //                    var dbRMAOUTSerialsLlist = RMASerialsLlist.AsNoTracking().Where(x => x.IsEnable && !x.RMASerialsLlistC.Any(y => y.IsEnable) && x.SerialsType == "TransferOut" && x.RMASKU.IsEnable && x.RMASKU.RMA.IsEnable && x.TransferSKU.IsEnable && x.TransferSKU.Transfer.IsEnable && !x.TransferSKU.SerialsLlist.Where(y => y.IsEnable && y.SerialsType == "TransferIn").Any()).Include(x => x.RMASKU).Include(x => x.RMASKU.RMA).Include(x => x.TransferSKU).Include(x => x.TransferSKU.Transfer).ToList();
        //                    var dbOrderSerialsLlist = SerialsLlist.AsNoTracking().Where(x => x.IsEnable && x.SerialsType == "Order" && x.CreateAt >= sdt && x.CreateAt <= edt).Include(x => x.PurchaseSKU).Include(x => x.PurchaseSKU.PurchaseOrder).Include(x => x.TransferSKU).Include(x => x.TransferSKU.Transfer).ToList();
        //                    var threadlist = new List<Thread>();
        //                    foreach (var Warehouseitem in dbWarehouse)
        //                    {
        //                        var SCID = Warehouseitem.WarehouseSummary.Where(x => x.Type == "SCID").FirstOrDefault()?.Val;
        //                        var dbAwaitinglist = GetAwaitingCount("", SCID);
        //                        MyThread myThread = new MyThread();
        //                        myThread.Warehouseitem = Warehouseitem;
        //                        myThread.AllSKUList = dbAllSKUList;
        //                        myThread.Awaitinglist = dbAwaitinglist;
        //                        myThread.POSerialsLlist = dbPOSerialsLlist;
        //                        myThread.InSerialsLlist = dbInSerialsLlist;
        //                        myThread.OutSerialsLlist = dbOutSerialsLlist;
        //                        myThread.RMAINSerialsLlist = dbRMAINSerialsLlist;
        //                        myThread.RMAOUTSerialsLlist = dbRMAOUTSerialsLlist;
        //                        myThread.OrderSerialsLlist = dbOrderSerialsLlist;
        //                        Thread thread = new Thread(myThread.ThreadMain);
        //                        threadlist.Add(thread);
        //                        thread.Start();
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception)
        //    {


        //    }

        //    return base.SaveChanges();
        //}
    }
    public class MyThread
    {
        public Models.Warehouse Warehouseitem { set; get; }
        public List<string> AllSKUList { set; get; }
        public List<AwaitingDispatchVM> Awaitinglist { set; get; }
        public List<SerialsLlist> POSerialsLlist { get; set; }
        public List<SerialsLlist> InSerialsLlist { get; internal set; }
        public List<SerialsLlist> OutSerialsLlist { get; internal set; }
        public List<RMASerialsLlist> RMAINSerialsLlist { get; internal set; }
        public List<RMASerialsLlist> RMAOUTSerialsLlist { get; internal set; }
        public List<SerialsLlist> OrderSerialsLlist { get; internal set; }


        public void ThreadMain()
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Reset();
            sw.Start();
            int threadId = Thread.CurrentThread.ManagedThreadId;
            Console.WriteLine("報行ID：" + threadId + " ;開始時間：" + sw.ElapsedMilliseconds);
            using (PurchaseOrderEntities dbW = new PurchaseOrderEntities())
            {
                dbW.Configuration.AutoDetectChangesEnabled = false;
                dbW.Configuration.LazyLoadingEnabled = false;
                dbW.Configuration.ProxyCreationEnabled = false;


                var inventory = new List<inventory>();
                foreach (var SKUitem in AllSKUList)
                {
                    //PO
                    var POQTY = POSerialsLlist.Where(x => x.PurchaseSKU.PurchaseOrder.WarehouseID == Warehouseitem.ID && x.PurchaseSKU.SkuNo == SKUitem).Sum(x => x.SerialsQTY) ?? 0;
                    //移倉入庫
                    var TinQTY = InSerialsLlist.Where(x => x.TransferSKU.Transfer.ToWID == Warehouseitem.ID && x.TransferSKU.SkuNo == SKUitem).Sum(x => x.SerialsQTY) ?? 0;
                    //移倉已Shipped
                    var FOutQTY = Math.Abs(OutSerialsLlist.Where(x => x.TransferSKU.Transfer.FromWID == Warehouseitem.ID && x.TransferSKU.Transfer.TransferType != "Winit" && (x.TransferSKU.Transfer.Status == "Shipped" || x.TransferSKU.Transfer.Status == "Received") && x.TransferSKU.SkuNo == SKUitem).Sum(x => x.SerialsQTY) ?? 0);
                    var WFOutQTY = Math.Abs(OutSerialsLlist.Where(x => x.TransferSKU.Transfer.FromWID == Warehouseitem.ID && x.TransferSKU.Transfer.TransferType == "Winit" && (x.TransferSKU.Transfer.Status == "Shipped" || x.TransferSKU.Transfer.Status == "Received") && x.TransferSKU.SkuNo == SKUitem).Sum(x => x.SerialsQTY) ?? 0);
                    var TOutQTY = Math.Abs(OutSerialsLlist.Where(x => x.TransferSKU.Transfer.ToWID == Warehouseitem.ID && x.TransferSKU.Transfer.TransferType != "Winit" && (x.TransferSKU.Transfer.Status == "Shipped" || x.TransferSKU.Transfer.Status == "Received") && x.TransferSKU.SkuNo == SKUitem).Sum(x => x.SerialsQTY) ?? 0);
                    var WTOutQTY = Math.Abs(OutSerialsLlist.Where(x => x.TransferSKU.Transfer.ToWID == Warehouseitem.ID && x.TransferSKU.Transfer.TransferType == "Winit" && (x.TransferSKU.Transfer.Status == "Shipped" || x.TransferSKU.Transfer.Status == "Received") && x.TransferSKU.SkuNo == SKUitem).Sum(x => x.SerialsQTY) ?? 0);

                    //移倉未Shipped
                    var UnFOutQTY = Math.Abs(OutSerialsLlist.Where(x => x.TransferSKU.Transfer.FromWID == Warehouseitem.ID && x.TransferSKU.Transfer.TransferType != "Winit" && x.TransferSKU.Transfer.Status == "Requested" && x.TransferSKU.SkuNo == SKUitem).Sum(x => x.SerialsQTY) ?? 0);
                    var UnWFOutQTY = Math.Abs(OutSerialsLlist.Where(x => x.TransferSKU.Transfer.FromWID == Warehouseitem.ID && x.TransferSKU.Transfer.TransferType == "Winit" && x.TransferSKU.Transfer.Status == "Requested" && x.TransferSKU.SkuNo == SKUitem).Sum(x => x.SerialsQTY) ?? 0);
                    var UnTOutQTY = Math.Abs(OutSerialsLlist.Where(x => x.TransferSKU.Transfer.ToWID == Warehouseitem.ID && x.TransferSKU.Transfer.TransferType != "Winit" && x.TransferSKU.Transfer.Status == "Requested" && x.TransferSKU.SkuNo == SKUitem).Sum(x => x.SerialsQTY) ?? 0);
                    var UnWTOutQTY = Math.Abs(OutSerialsLlist.Where(x => x.TransferSKU.Transfer.ToWID == Warehouseitem.ID && x.TransferSKU.Transfer.TransferType == "Winit" && x.TransferSKU.Transfer.Status == "Requested" && x.TransferSKU.SkuNo == SKUitem).Sum(x => x.SerialsQTY) ?? 0);

                    //待出貨
                    var Awaiting = Awaitinglist.Where(x => x.SKU == SKUitem).Sum(x => x.QTY);
                    //RMA入庫
                    var RMAINQTY = RMAINSerialsLlist.Where(x => x.WarehouseID == Warehouseitem.ID && x.RMASKU.SkuNo == SKUitem).Sum(x => x.SerialsQTY) ?? 0;
                    //RMA移倉已Shipped
                    var FRMAOUTQTY = Math.Abs(RMAOUTSerialsLlist.Where(x => x.TransferSKU.Transfer.FromWID == Warehouseitem.ID && (x.TransferSKU.Transfer.Status == "Shipped" || x.TransferSKU.Transfer.Status == "Received") && x.RMASKU.SkuNo == SKUitem).Sum(x => x.SerialsQTY) ?? 0);
                    var TRMAOUTQTY = Math.Abs(RMAOUTSerialsLlist.Where(x => x.TransferSKU.Transfer.ToWID == Warehouseitem.ID && (x.TransferSKU.Transfer.Status == "Shipped" || x.TransferSKU.Transfer.Status == "Received") && x.RMASKU.SkuNo == SKUitem).Sum(x => x.SerialsQTY) ?? 0);
                    //RMA移倉未Shipped
                    var UnFRMAOUTQTY = Math.Abs(RMAOUTSerialsLlist.Where(x => x.TransferSKU.Transfer.FromWID == Warehouseitem.ID && x.TransferSKU.Transfer.Status == "Requested" && x.RMASKU.SkuNo == SKUitem).Sum(x => x.SerialsQTY) ?? 0);
                    var UnTRMAOUTQTY = Math.Abs(RMAOUTSerialsLlist.Where(x => x.TransferSKU.Transfer.ToWID == Warehouseitem.ID && x.TransferSKU.Transfer.Status == "Requested" && x.RMASKU.SkuNo == SKUitem).Sum(x => x.SerialsQTY) ?? 0);
                    //30天出貨數量
                    var Velocity = Math.Abs(OrderSerialsLlist.Where(x => (x.TransferSKUID.HasValue && x.TransferSKU.SkuNo == SKUitem) || (x.PurchaseSKUID.HasValue && x.PurchaseSKU.SkuNo == SKUitem)).Sum(x => x.SerialsQTY) ?? 0);


                    inventory.Add(new inventory
                    {
                        WarehouseID = Warehouseitem.ID,
                        SkuID = SKUitem,
                        Aggregate = POQTY + TinQTY + RMAINQTY + UnFOutQTY + UnWFOutQTY + UnTOutQTY + UnWTOutQTY + UnFRMAOUTQTY + UnTRMAOUTQTY - Awaiting,
                        Awaiting = Awaiting,
                        Fulfillable = POQTY + TinQTY + RMAINQTY + UnFOutQTY + UnWFOutQTY + UnTOutQTY + UnWTOutQTY + UnFRMAOUTQTY + UnTRMAOUTQTY,
                        TransferOutQTY = FOutQTY + FRMAOUTQTY,
                        TransferInQTY = TOutQTY + TRMAOUTQTY,
                        WTransferOutQTY = WFOutQTY,
                        WTransferInQTY = WTOutQTY,
                        TotalVelocity = Velocity,
                        CreateAt = DateTime.UtcNow
                        //Aggregate = WarehouseVM.Sum(x => x.Aggregate),
                        //Awaiting = WarehouseVM.Sum(x => x.Awaiting),
                        //Fulfillable = WarehouseVM.Sum(x => x.Fulfillable),
                        //TransferOutQTY = WarehouseVM.Sum(x => x.TransferOutQTY)
                    });
                }
                dbW.BulkDelete(dbW.inventory.Where(x => x.WarehouseID == Warehouseitem.ID));
                //dbW.inventory.RemoveRange(dbW.inventory.Where(x=>x.WarehouseID== Warehouseitem.ID).ToList());
                dbW.SaveChanges();
                dbW.BulkInsert(inventory);
                //dbW.inventory.AddRange(inventory);
                dbW.SaveChanges();
                Console.WriteLine("報行ID：" + threadId + " ;結束時間：" + sw.ElapsedMilliseconds);
            }
        }

        public void Func_Test()
        {
            var job = new JobProcess("Task Test");
            job.StatusLog(EnumData.TaskStatus.執行中);
            job.AddLog("Task Test");
            Thread.Sleep(5000);
            job.FinishWork();
        }
    }
}