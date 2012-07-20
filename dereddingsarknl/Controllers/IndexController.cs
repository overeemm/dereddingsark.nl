using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using dereddingsarknl.Models;
using StackExchange.Profiling;

namespace dereddingsarknl.Controllers
{
  public class IndexController : Controller
  {
    public ActionResult Show()
    {
      ViewBag.Title = "Een plek zijn waar God de Vader, Jezus Christus de Zoon en de Heilige Geest centraal staan.";

      using(MiniProfiler.Current.Step("Read calendar file"))
      {
        var calendar = new Calendar(Calendar.GetCachedFile(HttpContext));
        ViewBag.CalendarItems = calendar.Items
          .Where(i => i.When.Date >= DateTime.Now.Date)
          .OrderBy(i => i.When)
          .Take(5).ToList();
      }
      return View();
    }

  }
}
