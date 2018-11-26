
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
//using PurchaseOrderSys.Areas.SKUSystem.Models;

namespace inventorySKU
{
    public class CheckSessionAttribute : ActionFilterAttribute
    {
        public CheckSessionAttribute()
        {
           
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            bool isLogin = (context.HttpContext.Session.Contents["IsLogin"] == null) ? false : (bool)context.HttpContext.Session.Contents["IsLogin"];
            if (!isLogin)
            {
                context.Result = new RedirectToRouteResult(
                        new RouteValueDictionary
                       {
                           { "controller", "Main" },
                           { "action", "Login" },
                           { "id", UrlParameter.Optional }
                       });
            }
            if (isLogin && !(bool)context.HttpContext.Session.Contents["IsManager"])
            {
                string controllerName = HttpContext.Current.Request.RequestContext.RouteData.Values["controller"].ToString().ToLower();
                string actionName = HttpContext.Current.Request.RequestContext.RouteData.Values["action"].ToString().ToLower();
                var Menulist = (List<Menu>)context.HttpContext.Session.Contents["Menu"];
                var MenulistQ = Menulist.Where(x => x.MenuChild.Where(y => y.Controller?.ToLower() == controllerName).Any());
                if (MenulistQ == null || !MenulistQ.Any())
                {
                    context.Result = new RedirectToRouteResult(
                       new RouteValueDictionary
                      {
                           { "controller", "Main" },
                           { "action", "Login" },
                           { "id", UrlParameter.Optional }
                      });
                    context.Controller.TempData["ErrMsg"] = PurchaseOrderSys.App_GlobalResources.Resource.AttributeErr;
                    //context.Result = new ContentResult { Content = "權限不足" };
                }
                else
                {
                    var MenuID = MenulistQ.FirstOrDefault().MenuChild.Where(x=>x.Controller?.ToLower() == controllerName)?.FirstOrDefault().MenuID;
                    var Authlist = ((Dictionary<string, List<string>>)context.HttpContext.Session.Contents["Auth"]).Where(x=>x.Key== MenuID.ToString())?.FirstOrDefault();
                    if (Authlist.HasValue && Authlist.Value.Value.Contains("0"))
                    {
                        context.Controller.ViewBag.Index = Authlist.Value.Value.Contains("0");
                        context.Controller.ViewBag.Create = Authlist.Value.Value.Contains("1");
                        context.Controller.ViewBag.Edit = Authlist.Value.Value.Contains("2");
                        context.Controller.ViewBag.Delete = Authlist.Value.Value.Contains("3");
                        context.Controller.ViewBag.Print = Authlist.Value.Value.Contains("4");
                        if (!(((actionName == "index"||actionName == "getdata") && Authlist.Value.Value.Contains("0")) || (actionName == "create" && Authlist.Value.Value.Contains("1")) || ((actionName == "edit"|| actionName == "update") && Authlist.Value.Value.Contains("2")) || (actionName == "delete" && Authlist.Value.Value.Contains("3")) || (actionName == "print" && Authlist.Value.Value.Contains("4"))))
                        {
                            context.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Main" }, { "action", "Login" }, { "id", UrlParameter.Optional } });
                            context.Controller.TempData["ErrMsg"] = PurchaseOrderSys.App_GlobalResources.Resource.AttributeErr;
                        }
                    }
                    else
                    {
                        context.Result = new RedirectToRouteResult(
                           new RouteValueDictionary
                           {
                           { "controller", "Main" },
                           { "action", "Login" },
                           { "id", UrlParameter.Optional }
                          });
                        context.Controller.TempData["ErrMsg"] = PurchaseOrderSys.App_GlobalResources.Resource.AttributeErr;
                        //context.Result = new ContentResult { Content = "權限不足" };
                    }
                }
            }
            else if(isLogin && (bool)context.HttpContext.Session.Contents["IsManager"])
            {
                context.Controller.ViewBag.Index = true;
                context.Controller.ViewBag.Create = true;
                context.Controller.ViewBag.Edit = true;
                context.Controller.ViewBag.Delete = true;
                context.Controller.ViewBag.Print = true;
            }

            context.HttpContext.Session.Add("IsLogin", isLogin);
            base.OnActionExecuting(context);
        }

        public override void OnResultExecuted(ResultExecutedContext context)
        {
            bool isLogin = (context.HttpContext.Session.Contents["IsLogin"] == null) ? false : (bool)context.HttpContext.Session.Contents["IsLogin"];

            if (isLogin && !(bool)context.HttpContext.Session.Contents["IsManager"])
            {
                /*AdminUsers = new GenericRepository<AdminUsers>(db);
                int userID = (int)context.HttpContext.Session.Contents["AdminId"];

                AdminUsers user = AdminUsers.Get(userID);
                user.LLT = DateTime.Now;
                AdminUsers.Update(user);
                AdminUsers.SaveChanges();*/
            }

            context.HttpContext.Session.Add("IsLogin", isLogin);
            base.OnResultExecuted(context);
        }
    }
}