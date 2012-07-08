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
  public class AudioController : Controller
  {
    public ActionResult Show()
    {
      ViewBag.Title = "Opnames";

      using(MiniProfiler.Current.Step("Read album"))
      {
        string filePath = Path.Combine(Settings.GetDataFolder(HttpContext), "indexen/audio.csv");
        var audioIndex = new Index(filePath);

        var items = audioIndex.Items
         .Select(i => new Opname()
         {
           Datum = Opname.ParseDatum(i.Skip(3).First()),
           Titel = i.Skip(1).First(),
           Url = i.Skip(2).First(),
           Categorie = i.First()
         })
         .OrderByDescending(o => o.Datum)
         .ToList();
        ViewBag.OpnameList = items;
        ViewBag.Opnames = "active";
      }

      return View();
    }

  }
}
