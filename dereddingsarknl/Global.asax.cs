using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using StackExchange.Profiling;

namespace dereddingsarknl
{
  public class MvcApplication : System.Web.HttpApplication
  {

    public static void RegisterGlobalFilters(GlobalFilterCollection filters)
    {
      filters.Add(new HandleErrorAttribute());
    }

    protected void Application_EndRequest()
    {
      MiniProfiler.Stop();
    }

    protected void Application_BeginRequest()
    {
      if (Request.IsLocal)
      {
        MiniProfiler.Start();
      }
    }

    public static void RegisterRoutes(RouteCollection routes)
    {
      routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

      routes.MapRoute("Index", "", new { controller = "Index", action = "Show" });

      routes.MapRoute("OverOns", "over-de-gemeente/{partName}", new { controller = "OverOns", action = "Show", partName = "grondslag" });

      routes.MapRoute("Pages", "{fileName}", new { controller = "Page", action = "Show" });
      routes.MapRoute("PagesInFolder", "{folderName}/{fileName}", new { controller = "Page", action = "ShowInFolder", fileName = "welkom" });
    }

    protected void Application_Start()
    {
      AreaRegistration.RegisterAllAreas();

      RegisterGlobalFilters(GlobalFilters.Filters);
      RegisterRoutes(RouteTable.Routes);
    }
  }
}