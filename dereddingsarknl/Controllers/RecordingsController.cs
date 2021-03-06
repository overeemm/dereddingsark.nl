﻿using System;
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
  public class RecordingsController : BaseController
  {
    public ActionResult OldPermaLinks(string alias)
    {
      using(MiniProfiler.Current.Step("Read album"))
      {
        var item = Data.GetFile(DataFolders.Indexes, IndexFiles.Recordings).OpenIndex()
          .Items
          .Select(i => Recording.CreateFromIndexLine(i))
          .Where(r => r.OldAlias == alias)
          .FirstOrDefault();

        if(item == null)
        {
          return PageNotFound();
        }

        return RedirectToActionPermanent("Single", "Recordings", new { alias = item.Alias });
      }
    }

    public ActionResult Single(string alias)
    {
      using(MiniProfiler.Current.Step("Read recording index"))
      {
        var item = Data.GetFile(DataFolders.Indexes, IndexFiles.Recordings).OpenIndex()
          .Items
          .Select(i => Recording.CreateFromIndexLine(i))
          .Where(r => r.Alias == alias)
          .FirstOrDefault();

        if(item == null)
        {
          return PageNotFound();
        }

        ViewBag.Title = item.Title;
        ViewBag.Recording = item;
        ViewBag.Recordings = "active";
      }

      return View();
    }

    [HttpPost]
    [ApiAction]
    public ActionResult Add(string name, string url, string datetime, string categorie)
    {
      if(CurrentUser == null || !CurrentUser.AudioManager)
        return new HttpUnauthorizedResult("U heeft geen toegang tot deze pagina.");

      var recording = new Recording()
      {
        Url = url,
        Title = name,
        OldAlias = "",
        DateString = datetime,
        Category = categorie,
        Alias = Path.GetFileNameWithoutExtension(new Uri(url).LocalPath)
      };

      var index = Data.GetFile(DataFolders.Indexes, IndexFiles.Recordings).OpenIndex();
      index.Add(recording.CreateIndexLine());

      return new EmptyResult();
    }

    public ActionResult Show()
    {
      ViewBag.Title = "Opnames";
      ViewBag.MetaDescription = "Opnames van samenkomsten en studies van de evangeliegemeente De Reddingsark";

      using(MiniProfiler.Current.Step("Read recording index"))
      {
        var items = Data.GetFile(DataFolders.Indexes, IndexFiles.Recordings).OpenIndex()
          .Items
          .Select(i => Recording.CreateFromIndexLine(i))
          .GroupBy(i => new DateTime(i.Date.Year, i.Date.Month, 1, 0, 0, 0))
          .ToDictionary(g => g.Key, g => g.OrderByDescending(i => i.Date).ToList());

        ViewBag.RecordingList = items;
        ViewBag.RecordingMonths = items.Keys.OrderByDescending(i => i).ToList();
        ViewBag.Recordings = "active";
      }

      return View();
    }

    public ActionResult Podcast()
    {
      using(MiniProfiler.Current.Step("Read album"))
      {
        var items = Data.GetFile(DataFolders.Indexes, IndexFiles.Recordings).OpenIndex()
          .Items
          .Select(i => Recording.CreateFromIndexLine(i))
          .OrderByDescending(r => r.Date)
          .Take(50)
          .ToList();

        ViewBag.RecordingList = items;
        ViewBag.PubDate = items.Select(r => r.Date).Max();
      }

      Response.ContentType = "text/xml";
      return View();
    }

  }
}
