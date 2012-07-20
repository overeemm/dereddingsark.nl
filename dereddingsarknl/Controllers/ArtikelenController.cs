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
  public class ArtikelenController : Controller
  {
    public ActionResult Show()
    {
      ViewBag.Title = "Artikelen";

      using(MiniProfiler.Current.Step("Read artikelen index"))
      {
        string filePath = Path.Combine(Settings.GetDataFolder(HttpContext), "indexen/artikelen.csv");
        var artikelenIndex = new Index(filePath);

        var items = artikelenIndex.Items
         .Select(i => new Artikel(i.First(), i.Skip(1).First(), i.Skip(2).First())
         {
           Content = GetFile(i.Skip(1).First()).Content
         })
         .OrderByDescending(o => o.Added)
         .Take(5)
         .ToList();

        ViewBag.ArtikelList = items;
        ViewBag.Artikelen = "active";
      }

      return View();
    }

    public ActionResult Artikel(string alias)
    {
      DataFile file = GetFile(alias);
      ViewBag.PageContent = file.Content;
      ViewBag.PageTitle = file.Title;
      ViewBag.Title = file.Title;
      ViewBag.PagePart = alias;
      ViewBag.Artikelen = "active";

      return View("Artikel");
    }

    private DataFile GetFile(string alias)
    {
      string filePath = Path.Combine(Settings.GetDataFolder(HttpContext),
        string.Format("paginas\\artikelen\\{0}.md", alias));

      if(System.IO.File.Exists(filePath))
      {
        using(MiniProfiler.Current.Step("Read markdown file"))
        {
          DataFile file = new DataFile(filePath);
          return file;
        }
      }

      throw new HttpException(404, "Not found");
    }

  }
}
