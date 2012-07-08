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
  public class AgendaController : Controller
  {
    public ActionResult Show()
    {
      ViewBag.Title = "Agenda";

      using(MiniProfiler.Current.Step("Read calendar file"))
      {
        string calendarFile = Path.Combine(Settings.GetDataFolder(HttpContext), "calendar/publiek.ics");
        var calendar = new Calendar(calendarFile);
        var items = calendar.Items
         .Where(i => i.When.Date >= DateTime.Now.Date)
         .GroupBy(i => new DateTime(i.When.Year, i.When.Month, 1, 0, 0, 0))
         .ToDictionary(g => g.Key, g => g.OrderBy(i => i.When).ToList());
        ViewBag.CalendarItems = items;
        ViewBag.CalendarMonths = items.Keys.OrderBy(i => i).ToList();
      }

      return View();
    }

  }
}
