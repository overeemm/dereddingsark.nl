﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using dereddingsarknl.Models;
using dereddingsarknl.Extensions;
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
           Data.GetFile(DataFolders.Calendar, CalendarFiles.Publiek)
          .OpenCalendar(HttpContext)
          .Items
          .Where(i => i.IsPublic && i.When.Date >= DateTime.Now.Date)
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
      var content =
         Data.GetFile(DataFolders.Calendar, CalendarFiles.Publiek)
        .OpenCalendar(HttpContext)
        .GetPubliekIcs();

      return Content(content, "text/calendar");
    }

  }
}
