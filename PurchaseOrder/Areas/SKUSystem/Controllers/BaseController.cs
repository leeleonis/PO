using PurchaseOrderSys.Areas.SKUSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace inventorySKU.Controllers
{
    public class BaseController : Controller
    {
        protected InventorySKUEntities db = new InventorySKUEntities();

        protected string LangCode;

        public BaseController()
        {
            if (string.IsNullOrEmpty(LangCode))
            {
                LangCode = System.Web.HttpContext.Current.Request.Cookies["Lang"]?.Value;
            }
        }

        protected string AuthToString(Dictionary<string, List<string>> auth)
        {
            var authW = new Dictionary<string, List<string>>();
            foreach (var authitem in auth)
            {
                if (authitem.Value.Where(x => x != "false").Any())
                {
                    authW.Add(authitem.Key, authitem.Value.Where(x => x != "false").ToList());
                }
            }
           return  new JavaScriptSerializer().Serialize(authW);
        }

        /// <summary>
        /// 設定要更新的欄位
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="OldData">舊資料</param>
        /// <param name="NewData">新資料</param>
        /// <param name="EditList">要更新欄位</param>
        protected void SetEditDatas<T>(T OldData, T NewData, string[] EditList)
        {
            var OldDatatp = OldData.GetType().GetProperties();
            foreach (var NameItem in EditList)
            {
                OldDatatp.FirstOrDefault(x => x.Name == NameItem).SetValue(OldData, OldDatatp.FirstOrDefault(x => x.Name == NameItem).GetValue(NewData, null));
            }
            OldDatatp.FirstOrDefault(x => x.Name == "UpdateBy").SetValue(OldData, Session["AdminName"]);
            OldDatatp.FirstOrDefault(x => x.Name == "UpdateAt").SetValue(OldData, DateTime.UtcNow);
            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                var s = ex.ToString();

            }

        }

        protected void SetUpdateData<T>(T originData, T updateData, string[] updateColumn)
        {
            var TypeInfoList = originData.GetType().GetProperties();
            foreach(string column in updateColumn)
            {
                var newData = TypeInfoList.FirstOrDefault(info => info.Name.Equals(column)).GetValue(updateData, null);
                TypeInfoList.FirstOrDefault(info => info.Name.Equals(column)).SetValue(originData, newData);
            }
            TypeInfoList.FirstOrDefault(info => info.Name.Equals("UpdateBy")).SetValue(originData, Session["AdminName"]);
            TypeInfoList.FirstOrDefault(info => info.Name.Equals("UpdateAt")).SetValue(originData, DateTime.UtcNow);
        }
    }
}