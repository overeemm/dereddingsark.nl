using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using dereddingsarknl.Models;
using dereddingsarknl.Extensions;
using StackExchange.Profiling;
using dereddingsarknl.Mailers;

namespace dereddingsarknl.Controllers
{
  public class BaseController : Controller
  {
    protected User CurrentUser { get; private set; }

    protected SiteMailer Mailer { get; private set; }

    protected CookieManager Cookies { get; private set; }

    protected UserManager Users { get; private set; }

    protected DataManager Data { get; private set; }

    protected void ClearCurrentUser()
    {
      CurrentUser = null;
    }

    protected ViewResult PageNotFound()
    {
      Response.StatusCode = 404;
      return View("Custom404");
    }

    protected override void OnActionExecuting(ActionExecutingContext filterContext)
    {
      Data = new DataManager(filterContext.HttpContext);
      Mailer = new SiteMailer();
      Mailer.EmailLogoPath = filterContext.HttpContext.Server.MapPath("~/email-logo.png");
      Cookies = new CookieManager(this);
      Users = new UserManager(Data);

      base.OnActionExecuting(filterContext);

      if(filterContext.HttpContext.Request.HttpMethod.Equals("GET", StringComparison.InvariantCultureIgnoreCase)
        && !filterContext.HttpContext.Request.IsSecureConnection && Cookies.UseHttps())
      {
        string url = "https://" + Environment.GetAbsoluteDomain(true) + filterContext.HttpContext.Request.RawUrl;
        filterContext.Result = new RedirectResult(url);
      }
      else
      {
        UserToken token = Cookies.GetUserToken();
        User user = Users.GetUser(token);
        ViewBag.CurrentUser = CurrentUser = user;

        /* Set user context for API calls */
        if(CurrentUser == null)
        {
          var apiToken = new HeaderManager().GetApiToken(Request.Headers);
          ViewBag.CurrentUser = CurrentUser = Users.GetApiUser(apiToken);
        }

        ViewBag.Message = Cookies.GetMessage();
        Cookies.ClearMessage();

        if(CurrentUser == null || !CurrentUser.EnableProfiler)
        {
          MiniProfiler.Stop(true);
        }
        else
        {
          ViewBag.IncludeProfiler = true;
        }
      }
    }

    protected override void OnActionExecuted(ActionExecutedContext filterContext)
    {
      if(CurrentUser != null && !CurrentUser.IsAPIUser)
      {
        UserToken token = Cookies.GetUserToken();
        if(token == null)
        {
          token = Users.StoreNewToken(CurrentUser, IpAddress);
        }
        Cookies.StoreUserToken(token);
      }

      if(filterContext.Result is HttpUnauthorizedResult
        && !filterContext.HttpContext.Request.IsAjaxRequest()
        && !filterContext.ActionDescriptor.IsApiAction())
      {
        filterContext.Result = RedirectToAction("Login", "User");
      }

      base.OnActionExecuted(filterContext);
    }

    protected string IpAddress
    {
      get { return Request.ServerVariables["REMOTE_ADDR"]; }
    }

    protected PhotoAlbum GetAlbum(string id, string name, string folder)
    {
      var dir = Data.GetPhotoDirectory(folder);

      if(!dir.Exists)
      {
        throw new HttpException(404, "Album " + id + " bestaat niet.");
      }

      return new PhotoAlbum(id, name, dir.FullName);
    }
  }
}
