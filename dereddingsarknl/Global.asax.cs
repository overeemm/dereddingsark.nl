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
using dereddingsarknl.Models;

namespace dereddingsarknl
{
  public class MvcApplication : HttpApplication
  {
    protected void Application_Start()
    {
      AreaRegistration.RegisterAllAreas();

      GlobalFilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
      RouteConfig.RegisterRoutes(RouteTable.Routes);
      MvcHandler.DisableMvcResponseHeader = true;
      BundleConfig.RegisterBundles(BundleTable.Bundles);
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

    protected void Application_EndRequest()
    {
      MiniProfiler.Stop();
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