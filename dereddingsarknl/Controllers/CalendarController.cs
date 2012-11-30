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
  public class CalendarController : BaseController
  {
    public ActionResult Show()
    {
      ViewBag.Title = "Agenda";

      using(MiniProfiler.Current.Step("Read calendar file"))
      {
        var items =
           Calendar.Get(HttpContext)
          .Items
          .Where(i => i.When.Date >= DateTime.Now.Date)
          .GroupBy(i => new DateTime(i.When.Year, i.When.Month, 1, 0, 0, 0))
          .ToDictionary(g => g.Key, g => g.OrderBy(i => i.When).ToList());

        ViewBag.CalendarItems = items;
        var months = items.Keys.OrderBy(i => i).ToList();
        ViewBag.CalendarMonths = months;
        ViewBag.FirstMonth = months.FirstOrDefault();
      }

      return View();
    }

    public ActionResult Download()
    {
      var stream = System.IO.File.OpenRead(Calendar.Get(HttpContext).FilePath);
      return File(stream, "text/calendar");
    }

  }
}
