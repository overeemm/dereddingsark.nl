using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

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
      // If the file is older then 10 minutes, start a backgroundworker to download it
      else if(File.GetLastWriteTimeUtc(calendarFile).AddMinutes(10) < DateTime.UtcNow)
      {
        var worker = new BackgroundWorker();
        worker.DoWork += (_, args) => DownloadCalendar(args.Argument as string);
        worker.RunWorkerAsync(calendarFile);
      }

      return new Calendar(calendarFile);
    }

    private static void DownloadCalendar(string fileLocation)
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

    private string _filePath;
    
    private Calendar(string filePath)
    {
      _filePath = filePath;
    }

    public IEnumerable<CalendarItem> Items
    {
      get
      {
        CalendarItem currentItem = null;
        var lines = new string[0];
        lock(fileLock)
        {
          lines = File.ReadAllLines(_filePath);
        }
        foreach(var line in lines)
        {
          if(line == "BEGIN:VEVENT")
          {
            currentItem = new CalendarItem();
          }
          else if(line == "END:VEVENT")
          {
            yield return currentItem;
            currentItem = null;
          }
          else if(currentItem != null)
          {
            if(line.StartsWith("LOCATION"))
            {
              currentItem.Where = line.Substring(9).Replace("\\,", ",");
            }
            else if(line.StartsWith("SUMMARY"))
            {
              currentItem.What = line.Substring(8).Replace("\\,", ",").Replace("samenkomst", "Samenkomst");
            }
            else if(line.StartsWith("DTSTART;TZID=Europe/Amsterdam"))
            {
              currentItem.When = CalendarItem.ParseWhen(line.Substring(30));
            }
          }
        }
      }
    }
  }

  public class CalendarItem
  {
    public DateTime When { get; set; }
    public string Where { get; set; }
    public string What { get; set; }

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
  }
}