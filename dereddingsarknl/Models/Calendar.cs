using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace dereddingsarknl.Models
{
  //http://www.google.com/calendar/ical/evangeliegemeentedereddingsark%40gmail.com/public/basic.ics
  public class Calendar
  {
    private string _filePath;
    public Calendar(string filePath)
    {
      _filePath = filePath;
    }

    public IEnumerable<CalendarItem> Items
    {
      get
      {
        CalendarItem currentItem = null;
        foreach(var line in File.ReadAllLines(_filePath))
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
              currentItem.What = line.Substring(8).Replace("\\,", ",");
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