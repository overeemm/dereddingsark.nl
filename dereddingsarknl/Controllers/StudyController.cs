using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using dereddingsarknl.Models;
using dereddingsarknl.Extensions;
using StackExchange.Profiling;

namespace dereddingsarknl.Controllers
{
  public class StudyController : BaseController
  {
    public ActionResult Show()
    {
      ViewBag.Title = "Studies";
      ViewBag.Studies = "active";
      return View();
    }
  }
}
