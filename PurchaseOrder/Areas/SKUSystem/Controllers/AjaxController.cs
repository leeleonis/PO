using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace inventorySKU.Controllers
{
    public class AjaxController : BaseController
    {
        [HttpPost]
        public ActionResult Language(string Lang)
        {
            if (!string.IsNullOrEmpty(Lang))
            {
                HttpCookie LangCookie = Request.Cookies["Lang"];
                if (LangCookie != null)
                {
                    LangCookie.Value = Lang.Trim();
                }
                else
                {
                    LangCookie = new HttpCookie("Lang");
                    LangCookie.Value = Lang.Trim();
                    LangCookie.Expires.AddDays(30);
                }
                Response.Cookies.Add(LangCookie);
                //System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(Lang);
                //System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(Lang);
            }
            return Json(new { status = true, Lang }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetSelectOption(string[] optionType)
        {
            AjaxResult result = new AjaxResult();

            try
            {
                if (!optionType.Any()) throw new Exception("沒有給項目!");

                var LangID = EnumData.DataLangList().First().Key;
                Dictionary<string, object> optionList = new Dictionary<string, object>();

                foreach (string type in optionType)
                {
                    switch (type)
                    {
                        case "ApprevedForExport":
                        case "Replenishable":
                            optionList.Add(type, Enum.GetValues(typeof(EnumData.YesNo)).Cast<EnumData.YesNo>().Select(e => new { text = e.ToString(), value = Convert.ToBoolean((int)e).ToString() }));
                            break;
                        case "CompanyOption":
                            var companyList = new List<object>() { new { text = "Not Choose", value = "" } };
                            companyList.AddRange(db.Company.AsNoTracking().Where(c => c.IsEnable).Select(c => new { text = c.Name, value = c.ID.ToString() }));
                            optionList.Add(type, companyList);
                            break;
                        case "SkuAttributeType":
                            optionList.Add(type, db.SkuAttributeType.AsNoTracking().Where(t => t.IsEnable).Select(t => new { text = t.Name, value = t.ID.ToString() }));
                            break;
                        case "SkuCondition":
                            optionList.Add(type, db.ConditionLang.AsNoTracking().Where(l => l.LangID.Equals(LangID)).Select(l => new { text = l.Name, value = l.ConditionID.ToString() }));
                            break;
                        case "SkuType":
                            optionList.Add(type, db.SkuTypeLang.AsNoTracking().Where(l => l.LangID.Equals(LangID)).Select(l => new { text = l.Name, value = l.TypeID.ToString() }));
                            break;
                        case "SkuStatus":
                            optionList.Add(type, Enum.GetValues(typeof(EnumData.SkuStatus)).Cast<EnumData.SkuStatus>().Select(e => new { text = e.ToString(), value = Convert.ToBoolean((int)e).ToString() }));
                            break;
                    }
                }

                result.data = optionList;
            }
            catch (Exception e)
            {
                result.SetError(e.InnerException != null && !string.IsNullOrEmpty(e.InnerException.Message) ? e.InnerException.Message : e.Message);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MenuListAuth(int id, string mod, int? Gid)
        {
            ViewBag.mod = mod;
            var Menu = db.Menu.Where(x => x.IsEnable).Include(m => m.MenuParent).AsQueryable();//選單
            if (Gid.HasValue)
            {
                var UserAuth = db.AdminUser.Find(id).Auth;//使用者
                var AdminGroup = db.AdminGroup.Find(Gid);//群組
                if (mod == "UE" || mod == "UI")
                {
                    var AuthItem = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(AdminGroup.Auth).Where(x => x.Value.Any()).Select(x => int.Parse(x.Key));
                    Menu = Menu.Where(x => x.MenuChild.Where(y => AuthItem.Contains(y.MenuID)).Any());
                }
                ViewBag.menu = Menu;
                if (!string.IsNullOrWhiteSpace(UserAuth))
                {
                    ViewBag.UserAuth = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(UserAuth);
                }
                return View(AdminGroup);
            }
            else
            {
                var AdminGroup = db.AdminGroup.Find(id);//群組
                if (mod == "UE")
                {
                    var AuthItem = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(AdminGroup.Auth).Where(x => x.Value.Any()).Select(x => int.Parse(x.Key));
                    Menu = Menu.Where(x => x.MenuChild.Where(y => AuthItem.Contains(y.MenuID)).Any());
                }
                ViewBag.menu = Menu;
                return View(AdminGroup);
            }

        }
    }

    internal class AjaxResult
    {
        public bool status { get; set; }
        public string message { get; set; }
        public object data { get; set; }

        public AjaxResult()
        {
            status = true;
        }

        public void SetError(string msg)
        {
            status = false;
            message = msg;
        }
    }
}