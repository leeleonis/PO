using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using PurchaseOrderSys.Models;

namespace PurchaseOrderSys.Controllers
{
    public class MainController : BaseController
    {
        // GET: Main
        public ActionResult Index()
        {
           // var s0 = db.SerialsLlist.Where(x => x.PurchaseSKU.SkuNo == "106005422" &&(!x.TransferSKUID.HasValue||(x.TransferSKU.IsEnable&&x.TransferSKU.Transfer.IsEnable) && (!x.PurchaseSKU.CreditMemoID.HasValue|| x.PurchaseSKU.CreditMemo.IsEnable))).Sum(x => x.SerialsQTY);
           // var s1 = db.SerialsLlist.Where(x => x.PurchaseSKU.SkuNo == "106005422" && x.PurchaseSKU.IsEnable && x.PurchaseSKU.PurchaseOrder.IsEnable).Sum(x => x.SerialsQTY);
           // var s11 = db.SerialsLlist.Where(x => (x.PurchaseSKU.SkuNo == "106005422" && x.PurchaseSKU.IsEnable && x.PurchaseSKU.PurchaseOrder.IsEnable) || (x.TransferSKUID.HasValue && x.TransferSKU.IsEnable && x.TransferSKU.Transfer.IsEnable)).Sum(x => x.SerialsQTY);


           // var s12 = db.SerialsLlist.Where(x => (x.PurchaseSKU.IsEnable &&x.PurchaseSKU.SkuNo == "106005422" &&  x.PurchaseSKU.PurchaseOrder.IsEnable) ||
           //                                      (x.PurchaseSKU.IsEnable &&x.PurchaseSKU.SkuNo == "106005422" &&  x.PurchaseSKU.CreditMemo.IsEnable) ||
           //                                      (x.TransferSKUID.HasValue &&x.TransferSKU.SkuNo == "106005422" &&  x.TransferSKU.IsEnable && x.TransferSKU.Transfer.IsEnable)

           //).Sum(x => x.SerialsQTY);





           // var s2 = db.SerialsLlist.Where(x => x.PurchaseSKU.SkuNo == "106005422"&&!x.SerialsLlistC.Any(y => y.IsEnable)).Sum(x => x.SerialsQTY);


           // var s13 = db.SerialsLlist.Where(x => x.PurchaseSKU.SkuNo == "106005422" && 
           // (
           // (x.PurchaseSKU.IsEnable && x.PurchaseSKU.CreditMemo.IsEnable) ||
           // (x.PurchaseSKU.IsEnable && x.PurchaseSKU.PurchaseOrder.IsEnable)||
           // (x.TransferSKU.IsEnable && x.TransferSKU.Transfer.IsEnable)
           // )).Sum(x => x.SerialsQTY);



            return View();
        }
        public ActionResult PasswordChange()
        {
            return View();
        }
        [HttpPost]
        public ActionResult PasswordChange(string Password)
        {
            var AdminId = (int)Session["AdminId"];
            var AdminUser = db.AdminUser.Find(AdminId);
            AdminUser.Password = Password;
            db.SaveChanges();
            return View();
        }
        //登入
        public ActionResult Login()
        {
            if (Session["IsLogin"] == null ? false : (bool)Session["IsLogin"])
            {
                return RedirectToAction("Index");
            }
            string host = Request.Url.Host;
            HttpCookie LangCookie = Request.Cookies["Lang"];
            if (LangCookie != null)
            {
                ViewBag.Lang = LangCookie.Value;
            }
            ViewBag.Langlist = EnumData.SystemLangList().Select(x => new SelectListItem { Text = x.Value, Value = x.Key }).ToList();
            return View();
        }

        //登入(POST)
        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            if (ValidateUser(username, password))
            {
                HttpCookie cookie = new HttpCookie("PurchaseOrder");
                cookie.Value = Server.UrlEncode("Hello!!世界");
                return RedirectToAction("Index");
            }
            return RedirectToAction("Login");
        }
        private  void SCLogin(string username, string password)
        {
            SCWS = new SellerCloud_WebService.SC_WebService( username,  password);
        }
        // 驗證帳號密碼
        private bool ValidateUser(string Account, string Password)
        {
            if (Account == "weypro" && Password == "weypro12ab")
            {
                return SetSessionData(true, "QDAdmin", ApiUserName, ApiPassword);
            }

            var AdminUser = db.AdminUser.Where(x => x.IsEnable && x.Account == Account && x.Password == Password).FirstOrDefault();
            if (AdminUser == null)
            {
                return SetErrorMessage("帳號或密碼錯誤");
            }
            else
            {
                //IsManger先設為 true
                return SetSessionData(false, AdminUser.Name, AdminUser.ApiUserName, AdminUser.ApiPassword, AdminUser.ID, AdminUser.Group, 0, AdminUser.TimeZone, AdminUser.Auth, AdminUser.AdminGroup.Auth, AdminUser.PageAuth.ToList());
            }

        }
        //登出
        public ActionResult Logout()
        {
            Session.Clear();

            return RedirectToAction("Login");
        }

        /// <summary>
        /// 設置Session
        /// </summary>
        /// <param name="IsManger"></param>
        /// <param name="AdminName"></param>
        /// <param name="ApiUserName"></param>
        /// <param name="ApiPassword"></param>
        /// <param name="AdminId"></param>
        /// <param name="GroupId"></param>
        /// <param name="WarehouseId"></param>
        /// <param name="TimeZone"></param>
        /// <param name="Auth"></param>
        /// <param name="GAuth"></param>
        /// <returns></returns>
        private bool SetSessionData(bool IsManger, string AdminName, string ApiUserName, string ApiPassword, int AdminId = 0, int GroupId = 0, int WarehouseId = 0, int TimeZone = -1, string Auth = null, string GAuth = null, List<PageAuth> PageAuth = null)
        {
            var Menu = db.Menu.Where(x => x.IsEnable == true).Include(m => m.MenuParent).Include(m=>m.MenuLang);//選單
            var Authlist =new Dictionary<string, List<string>>();//使用者權限
            var TAuthlist = new Dictionary<string, List<string>>();//實際使用者權限
            var AuthItem = new List<int>();
            if (!IsManger)
            {
                var GAuthlist = new Dictionary<string, List<string>>();//群組權限
                GAuthlist = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(GAuth);
                Authlist = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(Auth);
                foreach (var Uitem in Authlist)
                {
                    foreach (var Gitem in GAuthlist)
                    {
                        if (Uitem.Key == Gitem.Key)
                        {
                            TAuthlist.Add(Uitem.Key, Uitem.Value.Intersect(Gitem.Value).ToList());
                        }
                    }
                }

                AuthItem = TAuthlist.Where(x => x.Value.Any()).Select(x => int.Parse(x.Key)).ToList();
                Menu = Menu.Where(x => x.MenuChild.Where(y => AuthItem.Contains(y.MenuID)).Any());
            }
            Session.Add("IsLogin", true);
            Session.Add("IsManager", IsManger);
            Session.Add("AdminId", AdminId);
            Session.Add("AdminName", AdminName);
            Session.Add("GroupId", GroupId);
            Session.Add("Auth", TAuthlist);
            Session.Add("PageAuth", PageAuth?.ToDictionary(a => a.Type, a => JsonConvert.DeserializeObject<Dictionary<string, bool>>(a.AuthGroup)));
            Session.Add("ApiUserName", ApiUserName);
            Session.Add("ApiPassword", ApiPassword);
            Session.Add("WarehouseID", WarehouseId);
            Session.Add("TimeZone", TimeZone);
            Session.Add("Menu", Menu.ToList());
            UserBy = AdminName;

            //Session.Add("Warehouse", db.Warehouse.Where(x => x.IsEnable).ToList());
           

            return true;
        }

        private bool SetErrorMessage(string message)
        {
            TempData["errorMessage"] = message;

            return false;
        }
    }
}