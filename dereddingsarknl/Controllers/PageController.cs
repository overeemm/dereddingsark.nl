using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using StackExchange.Profiling;
using MarkdownSharp;
using dereddingsarknl.Models;

namespace dereddingsarknl.Controllers
{
  public class PageController : Controller
  {
    [HttpGet]
    public ActionResult ShowInFolder(string fileName = "welkom", string folderName = "")
    {
      string filePath = Server.MapPath(string.Format("/App_Data/paginas/{0}.md", Path.Combine(folderName, fileName)));

      if (System.IO.File.Exists(filePath))
      {
        using (MiniProfiler.Current.Step("Read markdown file"))
        {
          DataFile file = new DataFile(filePath);
          ViewBag.PageContent = file.Content;
          ViewBag.PageTitle = file.Title;
          ViewBag.PageKey = fileName;
          ViewBag.ShowMaps = fileName.Equals("welkom", StringComparison.InvariantCultureIgnoreCase);

          return View("Show");
        }
      }

      throw new HttpException(404, "Not found");
    }

    [HttpGet]
    public ActionResult Show(string fileName = "welkom")
    {
      return ShowInFolder(fileName: fileName, folderName: "");
    }

  }
}
