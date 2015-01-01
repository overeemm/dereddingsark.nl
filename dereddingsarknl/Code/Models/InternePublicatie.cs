using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dereddingsarknl.Code.Models
{
  public class InternePublicatie
  {
    public string Name { get; set; }
    public DateTime Date { get; set; }
    public DateTime? Date2 { get; set; }
    public string Link { get; set; }
  }
}