using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace dereddingsarknl
{
  public class Environment
  {
    public static string GetAbsoluteDomain(bool https)
    {
      if(Developmode)
      {
        return "http://www.dereddingsarktest.nl";
      }
      else
      {
        return (https ? "https://" : "http://") + "www.dereddingsark.nl";
      }
    }

    public static bool SupportsHttps { get { return !Developmode; } }

    public static bool Developmode { get { return HttpContext.Current.IsDebuggingEnabled; } }
  }
}