using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.Entity;

namespace inventorySKU.Controllers
{
    public class TestController : BaseController
    {
        public void CreateMenuLang()
        {
            string[] langList = new string[] { "zh-TW", "en-US" };

            var menuList = db.Menu.Where(m => m.IsEnable).AsQueryable();
            foreach(var menu in menuList)
            {
                foreach(var langCode in langList)
                {
                    if(!menu.MenuLang.AsQueryable().Any(m => m.LangID.Equals(langCode)))
                    {
                        var lang = new Models.MenuLang()
                        {
                            MenuID = menu.MenuID,
                            LangID = langCode,
                            Name = langCode.Equals("zh-TW") ? menu.Name : "",
                            CreateAt = DateTime.UtcNow,
                            CreateBy = "Test"
                        };
                        db.MenuLang.Add(lang);
                    }
                }
            }
            db.SaveChanges();
        }

        public void Test()
        {
            db.SkuType.Include(t => t.SkuTypeLang.Where(l => l.LangID.Equals(1)));
        }
    }
}