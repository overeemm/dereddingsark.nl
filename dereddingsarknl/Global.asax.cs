using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using StackExchange.Profiling;
using dereddingsarknl.Extensions;
using System.Web.Optimization;

namespace dereddingsarknl
{
  public class MvcApplication : System.Web.HttpApplication
  {
    public static void RegisterGlobalFilters(GlobalFilterCollection filters)
    {
      //filters.Add(new HandleErrorAttribute());
      filters.Add(new HandleErrorAttribute
      {
        ExceptionType = typeof(FileNotFoundException),
        View = "Custom404", // Custom404.cshtml is a view in the Shared folder.
        Order = 2
      });
    }

    protected void Application_EndRequest()
    {
      MiniProfiler.Stop();
    }

    protected void Application_BeginRequest()
    {
      if(!Request.Url.Host.StartsWith("www", StringComparison.InvariantCultureIgnoreCase) && !Request.Url.Host.StartsWith("localhost", StringComparison.InvariantCultureIgnoreCase))
      {
        Response.Clear();
        Response.AddHeader("Location", String.Format("{0}://www.{1}{2}", Request.Url.Scheme, Request.Url.Host, Request.Url.PathAndQuery));
        Response.StatusCode = 301;
        Response.End();
      }

      MiniProfiler.Start();
      new System.Net.Mail.SmtpClient().Set(HttpContext.Current);
    }

    public static void RegisterRoutes(RouteCollection routes)
    {
      routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

      routes.MapRoute("Index", "", new { controller = "Index", action = "Show" });
      routes.MapRoute("Contact", "contact", new { controller = "Index", action = "Contact" });
      routes.MapRoute("SendMessage", "sendmessage", new { controller = "Index", action = "SendMessage" });

      routes.MapRoute("About", "over-de-gemeente/{partName}", new { controller = "About", action = "Show", partName = "grondslag" });
      routes.MapRoute("Activities", "activiteiten/{partName}", new { controller = "Activities", action = "Show", partName = "samenkomsten" });
      routes.MapRoute("PhotoAlbums", "fotos", new { controller = "PhotoAlbums", action = "Show" });
      routes.MapRoute("PhotoAlbums JSON", "fotos/{name}/{id}", new { controller = "PhotoAlbums", action = "Photos" });

      routes.MapRoute("CalendarICS", "agenda.ics", new { controller = "Calendar", action = "Download" });
      routes.MapRoute("Calendar", "agenda", new { controller = "Calendar", action = "Show" });

      routes.MapRoute("InternCalendarICS", "76C49EEA6C5C400086161325BB262201.ics", new { controller = "Calendar", action = "DownloadIntern" });
      routes.MapRoute("Intern Calendar", "intern/agenda", new { controller = "Calendar", action = "ShowIntern" });

      routes.MapRoute("RecordingAdd", "audio/add", new { controller = "Recordings", action = "Add" });
      routes.MapRoute("RecordingsOldStyle", "audio/{cat}/{alias}", new { controller = "Recordings", action = "OldPermaLinks" });
      routes.MapRoute("RecordingPermaLink", "audio/{alias}", new { controller = "Recordings", action = "Single" });
      routes.MapRoute("Recordings", "audio", new { controller = "Recordings", action = "Show" });

      routes.MapRoute("Articles", "artikelen", new { controller = "Articles", action = "Show" });
      routes.MapRoute("Articles archive", "artikelen/archief", new { controller = "Articles", action = "Archief" });
      routes.MapRoute("Articles rss feed", "artikelen/feed", new { controller = "Articles", action = "Feed" });
      routes.MapRoute("ArticlesOldStyle", "artikelen/{cat}/{alias}", new { controller = "Articles", action = "OldPermaLinks" });
      routes.MapRoute("Article", "artikelen/{alias}", new { controller = "Articles", action = "Artikel" });

      routes.MapRoute("Intern", "intern", new { controller = "Intern", action = "Show" });
      routes.MapRoute("Intern Memoriseren", "intern/memoriseren", new { controller = "Intern", action = "Memoriseren" });

      routes.MapRoute("Podcast", "podcast", new { controller = "Recordings", action = "Podcast" });

      routes.MapRoute("DlContactblad", "intern/contactblad/{nummer}", new { controller = "Intern", action = "Contactblad" });
      routes.MapRoute("DlBaarn", "intern/baarn/{datum}", new { controller = "Intern", action = "Baarn" });
      routes.MapRoute("DlBunschoten", "intern/bunschoten/{datum}", new { controller = "Intern", action = "Bunschoten" });

      routes.MapRoute("UserDiff", "user/diff", new { controller = "User", action = "Diff" });
      routes.MapRoute("UserShow", "user/show", new { controller = "User", action = "Show" });
      routes.MapRoute("UserStoreNewBulk", "user/newbulk", new { controller = "User", action = "StoreNewBulk" });
      routes.MapRoute("UserStoreNew", "user/new", new { controller = "User", action = "StoreNew" });
      routes.MapRoute("UserStoreUpdate", "user/update", new { controller = "User", action = "StoreUpdate" });
      routes.MapRoute("UserLogin", "user/login", new { controller = "User", action = "Login" });
      routes.MapRoute("UserLogout", "user/logout", new { controller = "User", action = "Logout" });
      routes.MapRoute("UserResetPassword", "user/resetpassword", new { controller = "User", action = "ResetPassword" });
      routes.MapRoute("UserPassword", "user/setpassword", new { controller = "User", action = "SetPassword" });
      routes.MapRoute("UserCreateAPIToken", "user/apitoken", new { controller = "User", action = "CreateAPIToken" });
      routes.MapRoute("UserMail", "user/mail", new { controller = "User", action = "Mail" });
      routes.MapRoute("UserMailList", "user/maillist", new { controller = "User", action = "MailList" });

      routes.MapRoute("404PageNotFound", "{*url}", new { controller = "Error", action = "NotFound" });
    }

    protected void Application_Start()
    {
      AreaRegistration.RegisterAllAreas();

      RegisterGlobalFilters(GlobalFilters.Filters);
      RegisterRoutes(RouteTable.Routes);
      
      MvcHandler.DisableMvcResponseHeader = true;

      BundleConfig.RegisterBundles(BundleTable.Bundles);
    }

    protected void Application_Error(object sender, EventArgs e)
    {
      Exception exception = Server.GetLastError();
      Response.Clear();
      HttpException httpException = exception as HttpException;
      if(httpException != null)
      {
        string action;
        switch(httpException.GetHttpCode())
        {
          case 404:
            // page not found
            action = "HttpError404";
            break;
          case 500:
            // server error
            action = "Internal500";
            break;
          default:
            action = "General";
            break;
        }
        // clear error on server
        Server.ClearError();
        Response.Redirect(String.Format("~/Error.html?message={1}", action, exception.Message));
      }
    }
  }
}