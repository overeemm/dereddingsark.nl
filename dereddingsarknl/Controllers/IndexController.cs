﻿using System;
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
          Calendar.Get(HttpContext)
          .Items
          .Where(i => i.When.Date >= DateTime.Now.Date)
          .OrderBy(i => i.When)
          .Take(5)
          .ToList();
      }

      using(MiniProfiler.Current.Step("Read random 20 foto's from one album"))
      {
        var album = GetAlbum("", "Frontpage", "Frontpage");
        ViewBag.Photos = album.Photos.TakeRandom(20).ToList();
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
    public ActionResult SendMessage(string afzender, string email, string bericht)
    {
      Mailer.Contact(afzender, email, bericht).Send(new SmtpClient().Wrap());

      Cookies.StoreMessage("Uw bericht is verstuurd.");
      return RedirectToAction("Contact");
    }

  }
}
