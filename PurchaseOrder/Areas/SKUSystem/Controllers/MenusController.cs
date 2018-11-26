using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using inventorySKU.Models;

namespace inventorySKU.Controllers
{
    [CheckSession]
    public class MenusController : BaseController
    {
      
        // GET: Menus
        public ActionResult Index()
        {
            var menu = db.Menu.Include(m => m.MenuParent);
            ViewBag.MenuSet = true;
            return View(menu.ToList());
        }

        [HttpPost]
        public ActionResult Index(Dictionary<string, List<string>> auth)
        {
            if (auth.Any())
            {
                foreach (var item in auth)
                {
                    var AuthList = item.Value.Where(x => x != "false").Select(x => x);
                    var Menu = db.Menu.Find(int.Parse(item.Key));
                    Menu.Auth = string.Join(";", AuthList);
                }
                db.SaveChanges();
            }
            var menu = db.Menu.Include(m => m.MenuParent);
            ViewBag.MenuSet = true;
            return View(menu.ToList());
        }
    }
}
