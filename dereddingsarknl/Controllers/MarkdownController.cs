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
  public abstract class MarkdownController : BaseController
  {
    public abstract string BasePath { get; }

    protected void AddMarkdownFileToViewBag(string fileName)
    {
      MarkdownFile file = GetMarkdownFile(fileName);

      ViewBag.PageContent = file.Content;
      ViewBag.PageTitle = file.Title;
      ViewBag.Title = file.Title;
      ViewBag.PagePart = fileName;
    }

    protected MarkdownFile GetMarkdownFile(string fileName)
    {
      using(MiniProfiler.Current.Step("Read markdown file"))
      {
        return MarkdownFile.Create(HttpContext, BasePath, fileName);
      }
    }

  }
}
