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
  public class ActiviteitenController : Controller
  {
    public const string pageFolder = "activiteiten";

    public ActionResult Show(string partName)
    {
      string filePath = Path.Combine(Settings.GetDataFolder(HttpContext),
        string.Format("paginas/{0}.md", Path.Combine(pageFolder, partName)));

      if(System.IO.File.Exists(filePath))
      {
        using(MiniProfiler.Current.Step("Read markdown file"))
        {
          DataFile file = new DataFile(filePath);
          ViewBag.PageContent = file.Content;
          ViewBag.PageTitle = file.Title;
          ViewBag.Title = file.Title;
          ViewBag.PagePart = partName;
          ViewBag.Activiteiten = "active";

          return View("Show");
        }
      }

      throw new HttpException(404, "Not found");
    }

  }
}
