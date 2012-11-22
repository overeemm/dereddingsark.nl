using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace dereddingsarknl.Extensions
{
  public static class ViewHelper
  {
    public static string AbsoluteHttpsAction(this UrlHelper url, string actionName, string controllerName)
    {
      var urlString = url.Action(actionName, controllerName);
      return "https://www.dereddingsark.nl" + urlString;
    }

    public static string AbsoluteHttpAction(this UrlHelper url, string actionName, string controllerName)
    {
      var urlString = url.Action(actionName, controllerName);
      return "http://www.dereddingsark.nl" + urlString;
    }
  }
}