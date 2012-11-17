﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace dereddingsarknl.Models
{
  public class Recording
  {
    public static string GetPodcastiTunesUrl()
    {
      return ConfigurationManager.AppSettings["podcastiTunes"];
    }

    public DateTime Date { get; private set; }
    public string Title { get; private set; }
    public string Url { get; private set; }
    public string Category { get; private set; }
    public string OldAlias { get; private set; }
    public string Alias { get; private set; }

    public static Recording CreateFromIndexLine(IEnumerable<string> i)
    {
      var alias = i.Count() > 4 ? i.Skip(4).First() : "";
      var oldalias = i.Count() > 5 ? i.Skip(5).First() : "";

      return new Recording()
      {
        Date = Recording.ParseDate(i.Skip(3).First()),
        Title = i.Skip(1).First(),
        Url = i.Skip(2).First(),
        Alias = alias,
        OldAlias = oldalias,
        Category = i.First()
      };
    }

    private static DateTime ParseDate(string text)
    {
      var year = text.Substring(0, 4);
      var month = text.Substring(5, 2);
      var date = text.Substring(8, 2);
      var hour = text.Substring(11, 2);
      var minutes = text.Substring(14, 2);
      var secondes = text.Substring(17, 2);
      return new DateTime(int.Parse(year), int.Parse(month), int.Parse(date),
        int.Parse(hour), int.Parse(minutes), int.Parse(secondes));
    }
  }
}