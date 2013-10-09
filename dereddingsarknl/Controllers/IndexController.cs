using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using dereddingsarknl.Models;
using dereddingsarknl.Extensions;
using StackExchange.Profiling;

namespace dereddingsarknl.Controllers
{
  public class IndexController : BaseController
  {
    private static Random random = new Random();

    public ActionResult Show()
    {
      using(MiniProfiler.Current.Step("Read calendar file"))
      {
        ViewBag.CalendarItems =
          Data.GetFile(DataFolders.Calendar, CalendarFiles.Publiek)
          .OpenCalendar(HttpContext)
          .Items
          .Where(i => i.IsPublic && i.When.Date >= DateTime.Now.Date)
          .OrderBy(i => i.When)
          .Take(5)
          .ToList();
      }

      using(MiniProfiler.Current.Step("Read random 20 foto's from one album"))
      {
        var album = GetAlbum("", "Frontpage", "Frontpage");
        var photos = album.Photos.TakeRandom(10);
        ViewBag.Photo = photos.First();
        ViewBag.Photos = photos.Skip(1).ToList();
      }

      ViewBag.CalendarUrl = Calendar.GetCalendarUrl();
      return View();
    }

    public ActionResult Contact()
    {
      return View();
    }

    [HttpPost]
    public ActionResult SendMessage(string afzender, string email, string bericht)
    {
      Mailer.Contact(afzender, email, bericht).Send(new SmtpClient().Wrap());

      Cookies.StoreMessage("Uw bericht is verstuurd.");
      return RedirectToAction("Contact");
    }

  }
}
