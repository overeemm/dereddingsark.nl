using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dereddingsarknl.Models
{
  public class Article
  {
    public Article(string title, string alias, string added)
    {
      Alias = alias;
      Title = title;
      Added = ParseDate(added);
    }

    public static Article CreateFromIndexLine(IEnumerable<string> i)
    {
      return new Article(i.First(), i.Skip(1).First(), i.Skip(2).First());
    }

    private static DateTime ParseDate(string text)
    {
      var year = text.Substring(0, 4);
      var month = text.Substring(5, 2);
      var date = text.Substring(8, 2);
      var hour = text.Substring(11, 2);
      var minutes = text.Substring(14, 2);
      var secondes = text.Substring(17, 2);
      if(year == "0000")
      {
        return new DateTime(2010, 1, 1, 12, 00, 00);
      }
      else
      {
        return new DateTime(int.Parse(year), int.Parse(month), int.Parse(date),
          int.Parse(hour), int.Parse(minutes), int.Parse(secondes));
      }
    }

    public string Alias { get; set; }
    public string Title { get; set; }

    public string Content { get; set; }

    public DateTime Added { get; set; }
  }
}