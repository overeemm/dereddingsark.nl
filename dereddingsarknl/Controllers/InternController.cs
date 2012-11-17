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
  public class InternController : BaseController
  {
    public ActionResult Bunschoten(string datum)
    {
      if(CurrentUser == null)
        return new HttpUnauthorizedResult();

      return PdfDownload("bunschoten", f => f.Name.StartsWith(datum));
    }

    public ActionResult Baarn(string datum)
    {
      if(CurrentUser == null)
        return new HttpUnauthorizedResult();

      return PdfDownload("baarn", f => f.Name.StartsWith(datum));
    }

    public ActionResult Contactblad(string nummer)
    {
      if(CurrentUser == null)
        return new HttpUnauthorizedResult();

      return PdfDownload("contactblad", f => f.Name.StartsWith(nummer));
    }

    private ActionResult PdfDownload(string folder, Func<FileInfo, bool> predicate)
    {
      string directory = Path.Combine(Settings.GetDataFolder(HttpContext), "intern", folder);
      var pdffile = new DirectoryInfo(directory).GetFiles("*.pdf").FirstOrDefault(predicate);
      if(pdffile == null)
      {
        return PageNotFound();
      }
      else
      {
        return File(pdffile.FullName, "application/pdf");
      }
    }

    public ActionResult Show()
    {
      if(CurrentUser == null)
        return new HttpUnauthorizedResult("U heeft geen toegang tot deze pagina.");

      ViewBag.Title = "Intern";

      using(MiniProfiler.Current.Step("Read photoalbums"))
      {
        var albumIndex = Index.CreatePhotoAlbumIndex(HttpContext);
        ViewBag.Albums = albumIndex.Items.Select(l => GetAlbum(l.First(), l.Skip(1).First())).ToList();
      }

      using(MiniProfiler.Current.Step("contactbladen"))
      {
        string contactbladen = Path.Combine(Settings.GetDataFolder(HttpContext), "intern", "contactblad");
        ViewBag.Contactbladen = new DirectoryInfo(contactbladen).
          GetFiles("*.pdf")
          .Select(f =>
          {
            var number = f.Name.Substring(0, 3);
            var monthstrs = f.Name.Substring(4, f.Name.Length - 8).Split('-');
            var months = monthstrs.Select(s => new DateTime(int.Parse(s.Substring(0, 4)), int.Parse(s.Substring(4, 2)), 1));
            return new Tuple<string, DateTime[]>(number, months.ToArray());
          })
          .OrderByDescending(t => t.Item1).Take(4)
          .ToList();
      }

      using(MiniProfiler.Current.Step("baarn"))
      {
        string bladen = Path.Combine(Settings.GetDataFolder(HttpContext), "intern", "baarn");
        ViewBag.Baarn = new DirectoryInfo(bladen).
          GetFiles("*.pdf")
          .Select(f => {
            var date = f.Name.Substring(0, 8);
            return new DateTime(int.Parse(date.Substring(0, 4)), int.Parse(date.Substring(4, 2)), int.Parse(date.Substring(6, 2)));
          })
          .OrderByDescending(d => d).Take(4)
          .ToList();
      }

      using(MiniProfiler.Current.Step("bunschoten"))
      {
        string bladen = Path.Combine(Settings.GetDataFolder(HttpContext), "intern", "bunschoten");
        ViewBag.Bunschoten = new DirectoryInfo(bladen).
          GetFiles("*.pdf")
          .Select(f => {
            var date = f.Name.Substring(0, 8);
            return new DateTime(int.Parse(date.Substring(0, 4)), int.Parse(date.Substring(4, 2)), int.Parse(date.Substring(6, 2)));
          })
          .OrderByDescending(d => d).Take(4)
          .ToList();
      }

      ViewBag.Intern = "active";

      return View();
    }

  }
}
