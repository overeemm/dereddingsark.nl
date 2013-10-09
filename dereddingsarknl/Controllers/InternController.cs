using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using dereddingsarknl.Attributes;
using dereddingsarknl.Models;
using dereddingsarknl.Extensions;
using StackExchange.Profiling;

namespace dereddingsarknl.Controllers
{
  public class InternController : BaseController
  {
    [CustomAuthorize]
    public ActionResult Memoriseren()
    {
      ViewBag.Title = "Memoriseren";

      return View();
    }

    [CustomAuthorize]
    public ActionResult Bunschoten(string datum)
    {
      if(CurrentUser == null)
        return new HttpUnauthorizedResult();

      return PdfDownload(DataFolders.InternBunschoten, f => f.Name.StartsWith(datum));
    }

    [CustomAuthorize]
    public ActionResult Baarn(string datum)
    {
      return PdfDownload(DataFolders.InternBaarn, f => f.Name.StartsWith(datum));
    }

    [CustomAuthorize]
    public ActionResult Contactblad(string nummer)
    {
      return PdfDownload(DataFolders.InternContactblad, f => f.Name.StartsWith(nummer));
    }

    private ActionResult PdfDownload(DataFolders folder, Func<FileInfo, bool> predicate)
    {
      var pdffile = Data.GetDirectory(folder).GetFiles("*.pdf").FirstOrDefault(predicate);
      if(pdffile == null)
      {
        return PageNotFound();
      }
      else
      {
        return File(pdffile.FullName, "application/pdf");
      }
    }

    [CustomAuthorize]
    public ActionResult Show()
    {
      ViewBag.Title = "Intern";

      using(MiniProfiler.Current.Step("Read photoalbums"))
      {
        var albumIndex = Data.GetFile(DataFolders.Indexes, IndexFiles.Photos).OpenIndex();
        ViewBag.Albums = albumIndex.Items.Select(l => GetAlbum(l.First(), l.Skip(1).First(), l.Skip(2).First())).ToList();
      }

      using(MiniProfiler.Current.Step("contactbladen"))
      {
        ViewBag.Contactbladen = Data.GetDirectory(DataFolders.InternContactblad)
          .GetFiles("*.pdf")
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
        ViewBag.Baarn = Data.GetDirectory(DataFolders.InternBaarn)
          .GetFiles("*.pdf")
          .Select(f => {
            var date = f.Name.Substring(0, 8);
            return new DateTime(int.Parse(date.Substring(0, 4)), int.Parse(date.Substring(4, 2)), int.Parse(date.Substring(6, 2)));
          })
          .OrderByDescending(d => d).Take(6)
          .ToList();
      }

      using(MiniProfiler.Current.Step("bunschoten"))
      {
        ViewBag.Bunschoten = Data.GetDirectory(DataFolders.InternBunschoten)
          .GetFiles("*.pdf")
          .Select(f => {
            var date = f.Name.Substring(0, 8);
            return new DateTime(int.Parse(date.Substring(0, 4)), int.Parse(date.Substring(4, 2)), int.Parse(date.Substring(6, 2)));
          })
          .OrderByDescending(d => d).Take(6)
          .ToList();
      }

      ViewBag.Intern = "active";

      return View();
    }

  }
}
