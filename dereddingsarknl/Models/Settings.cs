using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace dereddingsarknl.Models
{
  public class Settings
  {
    public static string GetDataFolder(HttpContextBase context)
    {
      var folder = ConfigurationManager.AppSettings["dataFolder"];
      if(string.IsNullOrEmpty(folder) || !System.Diagnostics.Debugger.IsAttached)
      {
        folder = context.Server.MapPath(string.Format("/App_Data"));
      }
      return folder;
    }
  }
}