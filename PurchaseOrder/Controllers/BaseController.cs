using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using PurchaseOrderSys.Models;

namespace PurchaseOrderSys.Controllers
{
    public class BaseController : Controller
    {
        protected string UserBy = "test";
        protected PurchaseOrderEntities db = new PurchaseOrderEntities();
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
        protected string GetPOTypeName(string POType)
        {
          return  EnumData.POTypeDDL().Where(x => x.Key == POType)?.FirstOrDefault().Value;
        }
        protected string GetVendorName(int? VendorID)
        {
            if (VendorID.HasValue)
            {

                return db.VendorLIst.Find(VendorID).Name;
            }
            else
            {
                return "";
            }
           
        }
        protected string GetPOStatusName(string POStatus)
        {
            return EnumData.POStatusDDL().Where(x => x.Key == POStatus)?.FirstOrDefault().Value;
        }
        protected int RandomVal(int minValue, int maxValue)
        {
            Random crandom = new Random(Guid.NewGuid().GetHashCode());
            return crandom.Next(minValue, maxValue);
        }
        protected string ControlToString(string controlPath, object model)
        {
            //CshtmlView control = new CshtmlView(controlPath);
            RazorView control = new RazorView(this.ControllerContext, controlPath, null, false, null);

            this.ViewData.Model = model;

            HtmlTextWriter writer = new HtmlTextWriter(new System.IO.StringWriter());
            control.Render(new ViewContext(this.ControllerContext, control, this.ViewData, this.TempData, writer), writer);

            string value = ((StringWriter)writer.InnerWriter).ToString();

            return value;
        }
    }
}