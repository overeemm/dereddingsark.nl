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
      MiniProfiler.Start();
      if(Request.IsLocal)
      {

      }
    }

    public static void RegisterRoutes(RouteCollection routes)
    {
      routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

      routes.MapRoute("Index", "", new { controller = "Index", action = "Show" });

      routes.MapRoute("OverOns", "over-de-gemeente/{partName}", new { controller = "OverOns", action = "Show", partName = "grondslag" });
      routes.MapRoute("Activiteiten", "activiteiten/{partName}", new { controller = "Activiteiten", action = "Show", partName = "samenkomsten" });
      routes.MapRoute("Fotos", "fotos", new { controller = "Foto", action = "Show" });
      routes.MapRoute("FotosJSON", "fotos/{name}/{id}", new { controller = "Foto", action = "Photos" });
      routes.MapRoute("Agenda", "agenda", new { controller = "Agenda", action = "Show" });
      routes.MapRoute("Audio", "audio", new { controller = "Audio", action = "Show" });
      routes.MapRoute("Artikelen", "artikelen", new { controller = "Artikelen", action = "Show" });
      routes.MapRoute("Artikel", "artikelen/{alias}", new { controller = "Artikelen", action = "Artikel" });
    }

    protected void Application_Start()
    {
      AreaRegistration.RegisterAllAreas();

      RegisterGlobalFilters(GlobalFilters.Filters);
      RegisterRoutes(RouteTable.Routes);
    }
  }
}