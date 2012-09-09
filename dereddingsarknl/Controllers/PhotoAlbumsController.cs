using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using dereddingsarknl.Models;
using Google.GData.Photos;
using StackExchange.Profiling;

namespace dereddingsarknl.Controllers
{
  public class PhotoAlbumsController : BaseController
  {
    public ActionResult Photos(string id, string name)
    {
      using(MiniProfiler.Current.Step("Get photolist"))
      {
        var album = GetAlbum(id, name);
        return Json(album.Photos.Take(40), JsonRequestBehavior.AllowGet);
      }
    }

    private PhotoAlbum GetAlbum(string id, string name)
    {
      var xml = Path.Combine(Settings.GetDataFolder(HttpContext), "fotos", id + ".xml");

      if(!System.IO.File.Exists(xml))
      {
        throw new HttpException(404, "Album " + id + " bestaat niet.");
      }

      return new PhotoAlbum(XDocument.Load(xml))
      {
        Id = id,
        Name = name
      };
    }

    public ActionResult Show()
    {
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
