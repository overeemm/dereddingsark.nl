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
  public class FotoController : Controller
  {
    public ActionResult Photos(string id, string name)
    {
      using(MiniProfiler.Current.Step("Get photolist"))
      {
        var album = GetAlbum(id, name);
        return Json(album.Photos, JsonRequestBehavior.AllowGet);
      }
    }

    private FotoAlbum GetAlbum(string id, string name)
    {
      var folder = Path.Combine(Settings.GetDataFolder(HttpContext), "fotos");
      Directory.CreateDirectory(folder);
      var xml = Path.Combine(folder, id + ".xml");
      if(!System.IO.File.Exists(xml))
      {
        throw new HttpException(404, "Album " + id + " bestaat niet.");
        //using(WebClient wb = new WebClient())
        //{
        //  // hiervoor moet je geauthentiseerd zijn.
        //  var uri = string.Format("https://picasaweb.google.com/data/feed/api/user/evangeliegemeentedereddingsark@gmail.com/albumid/{0}?kind=photo", id);
        //  wb.DownloadFile(uri, xml);
        //}
      }

      var content = XDocument.Load(xml);
      return new FotoAlbum(content)
      {
        Id = id,
        Name = name
      };
    }

    public ActionResult Show()
    {
      using(MiniProfiler.Current.Step("Read album"))
      {
        string filePath = Path.Combine(Settings.GetDataFolder(HttpContext), "indexen/fotos.csv");
        var albumIndex = new Index(filePath);

        ViewBag.Title = "In blik in onze gemeente";
        ViewBag.Fotos = "active";
        ViewBag.Albums = albumIndex.Items.Select(l => GetAlbum(l.First(), l.Skip(1).First())).ToList();
      }

      return View();
    }
  }
}
