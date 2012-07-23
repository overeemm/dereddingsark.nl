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
  public class IndexController : BaseController
  {
    public ActionResult Show()
    {
      ViewBag.Title = "Een plek zijn waar God de Vader, Jezus Christus de Zoon en de Heilige Geest centraal staan.";

      using(MiniProfiler.Current.Step("Read calendar file"))
      {
        ViewBag.CalendarItems = 
          Calendar.Get(HttpContext)
          .Items
          .Where(i => i.When.Date >= DateTime.Now.Date)
          .OrderBy(i => i.When)
          .Take(5)
          .ToList();
      }

      ViewBag.CalendarUrl = Calendar.GetCalendarUrl();
      ViewBag.PodcastiTunesUrl = Recording.GetPodcastiTunesUrl();
      return View();
    }

  }
}
