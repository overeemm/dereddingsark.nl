using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MarkdownSharp;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace dereddingsarknl.Models
{
  public class MarkdownFile
  {
    public string Title { get; private set; }
    public string Content { get; private set; }
    private string FilePath { get; set; }

    public MarkdownFile(string path)
    {
      FilePath = path;
      Parse();
    }

    private void Parse()
    {
      string[] fileContent = System.IO.File.ReadAllLines(FilePath);

      bool header = true;
      var content = new StringBuilder();
      foreach (var line in fileContent)
      {
        if (!header)
        {
          content.AppendLine(line);
        }
        else if (line.StartsWith("-----"))
        {
          header = false;
        }
        else if (line.StartsWith("title="))
        {
          Title = line.Substring(6).Trim();
        }
      }

      Content = new Markdown().Transform(content.ToString());
    }

    public static MarkdownFile Create(HttpContextBase context, params string[] path)
    {
      string filePath = Path.Combine(Settings.GetDataFolder(context),
                                     Path.Combine(path)) + ".md";

      if(File.Exists(filePath))
      {
        return new MarkdownFile(filePath);
      }

      throw new FileNotFoundException();
    }
  }
}