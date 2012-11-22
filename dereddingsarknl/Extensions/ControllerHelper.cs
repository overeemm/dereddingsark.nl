using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using dereddingsarknl.Attributes;

namespace dereddingsarknl.Extensions
{
  public static class ControllerHelper
  {
    public static bool IsApiAction(this ActionDescriptor actionDescriptor)
    {
      return actionDescriptor.GetCustomAttributes(false).Any(o => o is ApiActionAttribute);
    }
  }
}