using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
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
//      ViewBag.Title = "Een plek zijn waar God de Vader, Jezus Christus de Zoon en de Heilige Geest centraal staan.";

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
        List<Photo> chooseFrom = album.Photos.Where(p => p.Width > p.Height).ToList();
        while(photos.Count < 10)
        {
          photos.Add(chooseFrom.Skip(random.Next(chooseFrom.Count() - 1)).First());
        }
        ViewBag.Photos = photos;
      }

      ViewBag.CalendarUrl = Calendar.GetCalendarUrl();
      ViewBag.PodcastiTunesUrl = Recording.GetPodcastiTunesUrl();
      return View();
    }

    public ActionResult Contact()
    {
      return View();
    }

    [HttpPost]
    public ActionResult SendMessage(string afzender, string naam, string bericht)
    {
      var client = new System.Net.Mail.SmtpClient();
      client.Send(new MailMessage(From_Address, From_Address,
        "Bericht via dereddingsark.nl van " + naam,
        string.Format(@"Naam: {0} ({1})

{1}", naam, afzender, bericht)));

      StoreMessageInCookie("Uw bericht is verstuurd.");

      return RedirectToAction("Contact");
    }

  }
}
