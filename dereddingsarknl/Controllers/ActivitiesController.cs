using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using dereddingsarknl.Models;
using StackExchange.Profiling;

namespace dereddingsarknl.Controllers
{
  public class ActivitiesController : MarkdownController
  {
    public override string BasePath
    {
      get { return "paginas\\activiteiten"; }
    }

    public ActionResult Show(string partName)
    {
      AddMarkdownFileToViewBag(partName);
      ViewBag.Activiteiten = "active";
      return View();
    }

  }
}
