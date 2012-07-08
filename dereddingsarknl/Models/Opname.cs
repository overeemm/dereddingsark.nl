using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dereddingsarknl.Models
{
  public class Opname
  {
    public DateTime Datum { get; set; }
    public string Titel { get; set; }
    public string Url { get; set; }
    public string Categorie { get; set; }
    
    public static DateTime ParseDatum(string text)
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