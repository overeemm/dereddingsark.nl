using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using DDay.iCal;
using DDay.iCal.Serialization.iCalendar;
using dereddingsarknl.Extensions;

namespace dereddingsarknl.Models
{
  public class Calendar
  {
    private static object fileLock = new object();

    public static string GetCalendarUrl()
    {
      return ConfigurationManager.AppSettings["calendar"];
    }

    public static Calendar Get(HttpContextBase httpContext)
    {
      string calendarFile = Path.Combine(Settings.GetDataFolder(httpContext), "calendar/publiek.ics");

      // If the file does not exists, download it sync (to bad for this request)
      if(!File.Exists(calendarFile))
      {
        DownloadCalendar(calendarFile);
      }
      // If the file is older then 30 minutes, start a backgroundworker to download it
      else if(File.GetLastWriteTimeUtc(calendarFile).AddMinutes(30) < DateTime.UtcNow)
      {
        var worker = new Thread(args => DownloadCalendar(args as string));
        worker.Start(calendarFile);
      }

      return new Calendar(calendarFile);
    }

    private static void DownloadCalendar(string fileLocation)
    {
      try
      {
        using(var client = new WebClient())
        {
          var data = client.DownloadData(new Uri(GetCalendarUrl()));
          lock(fileLock)
          {
            File.WriteAllBytes(fileLocation, data);
          }
        }
      }
      catch (Exception ex)
      {
        Elmah.ErrorLog.GetDefault(HttpContext.Current).Log(new Elmah.Error(ex));
      }
    }

    private string _filePath;
    private iCalendar _calendar;
    private List<CalendarItem> _items;
    private Calendar(string filePath)
    {
      _filePath = filePath;
      lock(fileLock)
      {
        _calendar = iCalendar.LoadFromStream(File.OpenRead(filePath)).FirstOrDefault() as iCalendar;

        _items = _calendar
          .GetOccurrences(new iCalDateTime(DateTime.Now), new iCalDateTime(DateTime.Now.AddYears(1)))
          .Select(o =>
            new CalendarItem()
            {
              What = (o.Source as Event).Summary,
              Where = (o.Source as Event).Location,
              When = new iCalDateTime( 
                o.Period.StartTime.Year, 
                o.Period.StartTime.Month,
                o.Period.StartTime.Day,
                o.Period.StartTime.Hour,
                o.Period.StartTime.Minute,
                o.Period.StartTime.Second,
                "Europe/Amsterdam") // make sure that our timezone is correct
            })
           .ToList();
      }
    }

    public string FilePath
    {
      get { return _filePath; }
    }

    public string GetPubliekIcs()
    {
      iCalendar iCal = new iCalendar();

      foreach(var item in Items.Where(c => c.IsPublic))
      {
        Event evt = iCal.Create<Event>();
        evt.Start = item.When;
        evt.End = item.When.AddHours(1);
        evt.Summary = item.What;
        evt.Location = item.Where;
      }

      return new iCalendarSerializer().SerializeToString(iCal);
    }

    public IEnumerable<CalendarItem> Items
    {
      get
      {
        return _items;
      }
    }

    public string GetIcs()
    {
      iCalendar iCal = new iCalendar();

      foreach(var item in Items)
      {
        Event evt = iCal.Create<Event>();
        evt.Start = item.When;
        evt.End = item.When.AddHours(1);
        evt.Summary = item.What;
        evt.Location = item.Where;
      }

      return new iCalendarSerializer().SerializeToString(iCal);
    }
  }

  public class CalendarItem
  {
    private string _where;
    private string _what;
    public IDateTime When { get; set; }
    
    public string Where
    {
      get
      {
        if(What.IndexOf("dienst baarn", StringComparison.InvariantCultureIgnoreCase) != -1)
        {
          return "De Reddingsark, Adelheidlaan 8, Baarn";
        }
        if(What.IndexOf("dienst bunschoten", StringComparison.InvariantCultureIgnoreCase) != -1 
          || What.IndexOf("jeugddienst", StringComparison.InvariantCultureIgnoreCase) != -1
          || What.IndexOf("gezamelijke dienst", StringComparison.InvariantCultureIgnoreCase) != -1)
        {
          return "Oostwende College, Plecht 1, Bunschoten";
        }
        return _where;
      }
      set
      {
        _where = value;
      }
    }

    public string What
    {
      get
      {
        if(_what.StartsWith("p ", StringComparison.InvariantCultureIgnoreCase))
        {
          return char.ToUpper(_what[2]) + _what.Substring(3);
        }
        return char.ToUpper(_what[0]) + _what.Substring(1);
      }
      set
      {
        _what = value;
      }
    }

    public CalendarItem()
    {
    }

    public CalendarItem(string what, string where, IDateTime when)
    {
      When = when;
      Where = where;
      What = what;
    }

    public static DateTime ParseWhen(string text)
    {
      var year = text.Substring(0, 4);
      var month = text.Substring(4, 2);
      var date = text.Substring(6, 2);
      var hour = text.Substring(9, 2);
      var minutes = text.Substring(11, 2);
      var secondes = text.Substring(13, 2);
      return new DateTime(int.Parse(year), int.Parse(month), int.Parse(date),
        int.Parse(hour), int.Parse(minutes), int.Parse(secondes));
    }

    public string WhatForFrontPage
    {
      get
      {
        if(IsPublic)
        {
          if(What.ToLowerInvariant().StartsWith("dienst ") && What.IndexOf("spreker:") > -1)
          {
            return What.Substring(0, What.IndexOf("spreker:")).Trim();
          }
          if(What.ToLowerInvariant().StartsWith("gezamelijke dienst ") && What.IndexOf(",") > -1)
          {
            return What.Substring(0, What.IndexOf(",")).Trim();
          }
          return What;
        }
        return string.Empty;
      }
    }

    public bool IsPublic
    {
      get
      {
        return What.StartsWith("dienst ", StringComparison.InvariantCultureIgnoreCase) 
          || What.StartsWith("P ", StringComparison.InvariantCultureIgnoreCase) 
          || What.IndexOf("jeugddienst", StringComparison.InvariantCultureIgnoreCase) != -1
          || What.IndexOf("israelstudie", StringComparison.InvariantCultureIgnoreCase) != -1
          || What.IndexOf("gezamelijke dienst", StringComparison.InvariantCultureIgnoreCase) != -1
          || What.IndexOf("kerstmusical", StringComparison.InvariantCultureIgnoreCase) != -1
          || What.IndexOf("kerstdienst", StringComparison.InvariantCultureIgnoreCase) != -1
          || What.IndexOf("praise", StringComparison.InvariantCultureIgnoreCase) != -1
          || What.IndexOf("oudejaarsdienst", StringComparison.InvariantCultureIgnoreCase) != -1;
      }
    }
  }
}