using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using StackExchange.Profiling;

namespace dereddingsarknl.Controllers
{
  public class ErrorController : Controller
  {
    public ViewResult Unknown()
    {
      Response.StatusCode = (int)HttpStatusCode.InternalServerError;

      MiniProfiler.Stop(true);

      return View("Unknown");
    }

    public ViewResult NotFound(string path)
    {
      Response.StatusCode = (int)HttpStatusCode.NotFound;
      
      MiniProfiler.Stop(true);

      return View("NotFound");
    }
  }
}
