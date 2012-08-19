using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using dereddingsarknl.Models;
using StackExchange.Profiling;

namespace dereddingsarknl.Controllers
{
  public class IndexController : BaseController
  {
    private static Random random = new Random();

    public ActionResult Show()
    {
      ViewBag.Title = "Een plek zijn waar God de Vader, Jezus Christus de Zoon en de Heilige Geest centraal staan.";

      using(MiniProfiler.Current.Step("Read calendar file"))
      {
        ViewBag.CalendarItems =
          Calendar.Get(HttpContext)
          .Items
          .Where(i => i.When.Date >= DateTime.Now.Date)
          .OrderBy(i => i.When)
          .Take(5)
          .ToList();
      }

      using(MiniProfiler.Current.Step("Read random 20 foto's from one album"))
      {
        var albumIndex = Index.CreatePhotoAlbumIndex(HttpContext);
        var albums = albumIndex.Items.Select(l => l.First()).ToList();

        var randId = albums[random.Next(albums.Count)];
        var xml = Path.Combine(Settings.GetDataFolder(HttpContext), "fotos", randId + ".xml");

        var album = new PhotoAlbum(XDocument.Load(xml));

        List<Photo> photos = new List<Photo>();
        while(photos.Count < 10)
        {
          photos.Add(album.Photos.Skip(random.Next(album.Photos.Count() - 1)).First());
        }
        ViewBag.Photos = photos;
      }

      ViewBag.CalendarUrl = Calendar.GetCalendarUrl();
      ViewBag.PodcastiTunesUrl = Recording.GetPodcastiTunesUrl();
      return View();
    }

  }
}
