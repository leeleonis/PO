using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using inventorySKU.NetoDeveloper;
using PurchaseOrderSys.Models;

namespace PurchaseOrderSys.Controllers
{
    /// <summary>
    /// 每一筆資料Entity
    /// </summary>
    public class MyRecord
    {
        public int sysid { get; set; }
        public string MyTitle { get; set; }
        public int MyMoney { get; set; }

    }

    public class TestController : Controller
    {
        protected PurchaseOrderEntities db = new PurchaseOrderEntities();

        public TestController()
        {//在建構子裡新增資料
            for (int i = 0; i < 300; i++)
            {
                this._myRecords.Add(new MyRecord()
                {
                    sysid = i + 1,
                    MyTitle = "MyTitle" + i,
                    MyMoney = i * 1000
                });
            }//end for 
        }
        /// <summary>
        /// DataSource 資料集合，通常為DB裡的資料
        /// </summary>
        private List<MyRecord> _myRecords = new List<MyRecord>();

        // GET: Test
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetData_Full(int draw, int start, int length,//→此三個為DataTables自動傳遞參數
                                                                         //↓以下兩個為表單的查詢條件，請依自己工作需求調整  
         string MyTitle, int? MyMoney)
        {

            int skip = start;//起始資料列索引值(略過幾筆)

            #region jQuery DataTables的排序資料行
            //jQuery DataTable的Column index
            string col_index = Request.QueryString["order[0][column]"];
            //col_index 換算成 資料行名稱
            //排序資料行名稱
            string sortColName = string.IsNullOrEmpty(col_index) ? "sysid" : Request.QueryString[$@"columns[{col_index}][data]"];
            //升冪或降冪
            string asc_desc = string.IsNullOrEmpty(Request.QueryString["order[0][dir]"]) ? "desc" : Request.QueryString["order[0][dir]"];//防呆
            #endregion

            //查詢&排序後的總筆數
            int recordsTotal = 0;
            //要回傳的分頁資料
            List<MyRecord> pagedData = new List<MyRecord>();

            //總資料
            var query = this._myRecords.AsEnumerable();
            //查詢
            if (!string.IsNullOrEmpty(MyTitle))
            {
                query = this._myRecords.Where(m => m.MyTitle.Contains(MyTitle));
            }
            if (MyMoney.HasValue)
            {
                query = this._myRecords.Where(m => m.MyMoney == MyMoney);
            }

            //排序
            //query = query.OrderBy($@"{sortColName} {asc_desc}"); //排序使用到System.Linq.Dynamic

            recordsTotal = query.Count();//查詢後的總筆數

            if (length == -1)
            {//抓全部資料
                pagedData = query.ToList();
            }
            else
            {//分頁 
                pagedData = query.Skip(skip).Take(length)
                            .ToList();
            }


            //回傳Json資料
            var returnObj =
              new
              {
                  draw = draw,
                  recordsTotal = recordsTotal,
                  recordsFiltered = recordsTotal,
                  data = pagedData//分頁後的資料 
              };
            return Json(returnObj, JsonRequestBehavior.AllowGet);
        }

        public void SkuSync()
        {
            NetoApi neto = new NetoApi();
            var categoryList = neto.GetCategory().Category.Where(c => c.ParentCategoryID.Equals("0")).ToList();
            var aa = categoryList.Where(c => !c.ID.Equals(c.CategoryID)).ToList();
            foreach(var category in categoryList)
            {
                if(!db.SkuType.Any(t => t.NetoID.ToString().Equals(category.ID))){
                    SkuType type = new SkuType()
                    {
                        IsEnable = true,
                        NetoID = int.Parse(category.ID),
                        CreateAt = DateTime.UtcNow,
                        CreateBy = Session["AdminName"].ToString()
                    };
                }
            }
        }
    }
}