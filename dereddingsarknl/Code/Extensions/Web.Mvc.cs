using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using dereddingsarknl.Attributes;

namespace dereddingsarknl.Extensions
{
  public static class WebMvcHelper
  {
    public static bool IsApiAction(this ActionDescriptor actionDescriptor)
    {
      return actionDescriptor.GetCustomAttributes(false).Any(o => o is ApiActionAttribute);
    }

    public static string AbsoluteHttpsAction(this UrlHelper url, string actionName, string controllerName, object routeValues)
    {
      var urlString = url.Action(actionName, controllerName, routeValues);
      return "https://" + Environment.GetAbsoluteDomain(true) + urlString;
    }

    public static string AbsoluteHttpsAction(this UrlHelper url, string actionName, string controllerName)
    {
      var urlString = url.Action(actionName, controllerName);
      return "https://" + Environment.GetAbsoluteDomain(true) + urlString;
    }

    public static string AbsoluteHttpAction(this UrlHelper url, string actionName, string controllerName, object routeValues)
    {
      var urlString = url.Action(actionName, controllerName, routeValues);
      return "http://" + Environment.GetAbsoluteDomain(true) + urlString;
    }

    public static string AbsoluteHttpAction(this UrlHelper url, string actionName, string controllerName)
    {
      var urlString = url.Action(actionName, controllerName);
      return "http://" + Environment.GetAbsoluteDomain(false) + urlString;
    }
  }
}