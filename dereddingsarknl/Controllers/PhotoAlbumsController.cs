using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using dereddingsarknl.Models;
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
        var album = GetAlbum(id, name);
        return Json(album.Photos.Take(40), JsonRequestBehavior.AllowGet);
      }
    }

    public ActionResult Show()
    {
      if(CurrentUser == null)
        return new HttpUnauthorizedResult("U heeft geen toegang tot deze pagina.");

      using(MiniProfiler.Current.Step("Read album"))
      {
        var albumIndex = Index.CreatePhotoAlbumIndex(HttpContext);

        ViewBag.Title = "In blik in onze gemeente";
        ViewBag.Fotos = "active";
        ViewBag.Albums = albumIndex.Items.Select(l => GetAlbum(l.First(), l.Skip(1).First())).ToList();
      }

      return View();
    }
  }
}
