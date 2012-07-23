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
  public class RecordingsController : Controller
  {
    public ActionResult Show()
    {
      ViewBag.Title = "Opnames";

      using(MiniProfiler.Current.Step("Read album"))
      {
        var items = 
          Index.CreateAudioIndex(HttpContext)
          .Items
          .Select(i => Recording.CreateFromIndexLine(i))
          .GroupBy(i => new DateTime(i.Date.Year, i.Date.Month, 1, 0, 0, 0))
          .ToDictionary(g => g.Key, g => g.OrderByDescending(i => i.Date).ToList());

        ViewBag.RecordingList = items;
        ViewBag.RecordingMonths = items.Keys.OrderByDescending(i => i).ToList();
        ViewBag.Recordings = "active";
      }

      return View();
    }

    public ActionResult Podcast()
    {
      using(MiniProfiler.Current.Step("Read album"))
      {
        var items =
          Index.CreateAudioIndex(HttpContext)
          .Items
          .Select(i => Recording.CreateFromIndexLine(i))
          .OrderByDescending(r => r.Date)
          .ToList();

        ViewBag.RecordingList = items;
      }

      Response.ContentType = "text/xml";
      return View();
    }

  }
}
