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
    }

    public static void RegisterRoutes(RouteCollection routes)
    {
      routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

      routes.MapRoute("Index", "", new { controller = "Index", action = "Show" });

      routes.MapRoute("About", "over-de-gemeente/{partName}", new { controller = "About", action = "Show", partName = "grondslag" });
      routes.MapRoute("Activities", "activiteiten/{partName}", new { controller = "Activities", action = "Show", partName = "samenkomsten" });
      routes.MapRoute("PhotoAlbums", "fotos", new { controller = "PhotoAlbums", action = "Show" });
      routes.MapRoute("PhotoAlbums JSON", "fotos/{name}/{id}", new { controller = "PhotoAlbums", action = "Photos" });
      routes.MapRoute("Calendar", "agenda", new { controller = "Calendar", action = "Show" });
      routes.MapRoute("Recordings", "audio", new { controller = "Recordings", action = "Show" });
      routes.MapRoute("Articles", "artikelen", new { controller = "Articles", action = "Show" });
      routes.MapRoute("Articles archive", "artikelen/archief", new { controller = "Articles", action = "Archief" });
      routes.MapRoute("Article", "artikelen/{alias}", new { controller = "Articles", action = "Artikel" });
      routes.MapRoute("Intern", "intern", new { controller = "Intern", action = "Show" });

      routes.MapRoute("Podcast", "podcast", new { controller = "Recordings", action = "Podcast" });

      routes.MapRoute("DlContactblad", "intern/contactblad/{nummer}", new { controller = "Intern", action = "Contactblad" });
      routes.MapRoute("DlBaarn", "intern/baarn/{datum}", new { controller = "Intern", action = "Baarn" });
      routes.MapRoute("DlBunschoten", "intern/bunschoten/{datum}", new { controller = "Intern", action = "Bunschoten" });

      routes.MapRoute("UserAdd", "user/add", new { controller = "User", action = "Add" });
      routes.MapRoute("UserStoreNew", "user/new", new { controller = "User", action = "StoreNew" });
      routes.MapRoute("UserStoreUpdate", "user/update", new { controller = "User", action = "StoreUpdate" });
      routes.MapRoute("UserLogin", "user/login", new { controller = "User", action = "Login" });
      routes.MapRoute("UserLogout", "user/logout", new { controller = "User", action = "Logout" });
    }

    protected void Application_Start()
    {
      AreaRegistration.RegisterAllAreas();

      RegisterGlobalFilters(GlobalFilters.Filters);
      RegisterRoutes(RouteTable.Routes);
    }
  }
}