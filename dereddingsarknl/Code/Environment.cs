using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dereddingsarknl
{
  public class Environment
  {
    public static string GetAbsoluteDomain(bool https)
    {
      if(System.Diagnostics.Debugger.IsAttached)
      {
        return https ? "localhost:44300" : "localhost:2630";
      }
      else
      {
        return "www.dereddingsark.nl";
      }
    }
  }
}