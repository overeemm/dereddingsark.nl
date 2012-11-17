﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using dereddingsarknl.Models;
using StackExchange.Profiling;

namespace dereddingsarknl.Controllers
{
  public class AboutController : MarkdownController
  {
    public override string BasePath
    {
      get { return "paginas\\over-de-gemeente"; }
    }

    public ActionResult Show(string partName)
    {
      try
      {
        AddMarkdownFileToViewBag(partName);
      }
      catch(FileNotFoundException)
      {
        return PageNotFound();
      }
      ViewBag.OverOns = "active";
      return View();
    }

  }
}
