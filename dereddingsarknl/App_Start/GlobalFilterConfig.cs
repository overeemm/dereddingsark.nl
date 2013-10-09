using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace dereddingsarknl
{
  public class GlobalFilterConfig
  {
    public static void RegisterGlobalFilters(GlobalFilterCollection filters)
    {
      filters.Add(new HandleErrorAttribute
      {
        ExceptionType = typeof(FileNotFoundException),
        View = "Custom404",
        Order = 1
      });
      
      filters.Add(new HandleErrorAttribute
      {
        ExceptionType = typeof(HttpAntiForgeryException),
        View = "Error",
        Order = 2
      });

      filters.Add(new HandleErrorAttribute
      {
        View = "Error",
        Order = 3
      });
    }
  }
}