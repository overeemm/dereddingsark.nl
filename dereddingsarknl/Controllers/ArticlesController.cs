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
  public class ArticlesController : MarkdownController
  {
    public ActionResult Show()
    {
      ViewBag.Title = "Artikelen";

      using(MiniProfiler.Current.Step("Read artikelen index"))
      {
        var items =
          Index.CreateArticleIndex(HttpContext)
          .Items
          .Select(i => Article.CreateFromIndexLine(i))
          .OrderByDescending(o => o.Added)
          .Take(5)
          .ToList();

        items.ForEach(a => a.Content = GetMarkdownFile(a.Alias).Content);

        ViewBag.NewestArtikel = items.First();
        ViewBag.ArtikelList = items.Skip(1);
        ViewBag.Artikelen = "active";
      }

      return View();
    }

    public ActionResult Archief(string sortering)
    {
      ViewBag.Title = "Artikelen archief";
      throw new ArgumentException("test");
      using(MiniProfiler.Current.Step("Read artikelen index"))
      {
        var items = 
          Index.CreateArticleIndex(HttpContext)
          .Items
          .Select(i => Article.CreateFromIndexLine(i));

        if(sortering == "datum")
        {
          items = items.OrderByDescending(o => o.Added).ToList();
        }
        else
        {
          items = items.OrderBy(o => o.Title).ToList();
        }

        ViewBag.ArtikelList = items;
        ViewBag.Artikelen = "active";
      }

      return View();
    }

    public ActionResult Artikel(string alias)
    {
      AddMarkdownFileToViewBag(alias);
      ViewBag.Artikelen = "active";
      return View("Artikel");
    }

    public override string BasePath
    {
      get { return "paginas\\artikelen"; }
    }

  }
}
