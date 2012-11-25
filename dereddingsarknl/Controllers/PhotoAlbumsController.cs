using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using dereddingsarknl.Models;
using dereddingsarknl.Extensions;
using StackExchange.Profiling;

namespace dereddingsarknl.Controllers
{
  public class PhotoAlbumsController : BaseController
  {
    public ActionResult Photos(string id, string name)
    {
      if(CurrentUser == null)
        return new HttpUnauthorizedResult("U heeft geen toegang tot deze pagina.");

      using(MiniProfiler.Current.Step("Get photolist"))
      {
        var albumIndex = Data.GetFile(DataFolders.Indexes, IndexFiles.Photos).OpenIndex();
        var album = albumIndex.Items.Select(l => GetAlbum(l.First(), l.Skip(1).First(), l.Skip(2).First())).First(a => a.Id == id);
        return Json(album.Photos.TakeRandom(40), JsonRequestBehavior.AllowGet);
      }
    }

    public ActionResult Show()
    {
      if(CurrentUser == null)
        return new HttpUnauthorizedResult("U heeft geen toegang tot deze pagina.");

      using(MiniProfiler.Current.Step("Read album"))
      {
        var albumIndex = Data.GetFile(DataFolders.Indexes, IndexFiles.Photos).OpenIndex();

        ViewBag.Title = "In blik in onze gemeente";
        ViewBag.Fotos = "active";
        ViewBag.Albums = albumIndex.Items.Select(l => GetAlbum(l.First(), l.Skip(1).First(), l.Skip(2).First())).ToList();
      }

      return View();
    }
  }
}
