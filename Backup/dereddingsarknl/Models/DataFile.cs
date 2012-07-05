using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MarkdownSharp;
using System.Text;
using System.Text.RegularExpressions;

namespace dereddingsarknl.Models
{
  public class DataFile
  {
    public string Title { get; private set; }
    public string Content { get; private set; }
    private string FilePath { get; set; }

    public DataFile(string path)
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

      Content = new Markdown().Transform(ReplaceIndexMarkers(content.ToString()));
    }

    private string ReplaceIndexMarkers(string content)
    {
      return new Regex("{{(.*)}}").Replace(content, new MatchEvaluator(match =>
      {
        string parameters = match.Value.Substring(2, match.Value.Length - 4);
        var dictionaries = new Dictionary<string, string>();
        foreach (var parameter in parameters.Split(','))
        {
          var parts = parameter.Split(':');
          dictionaries.Add(parts[0].Trim(), parts[1].Trim());
        }
        return GetIndex(dictionaries);
      }));
    }

    private string GetIndex(Dictionary<string, string> dictionaries)
    {
      Index i = null;
      switch (dictionaries["index"])
      {
        case "contactbladen":
          i = Index.Contactbladen;
          break;
        case "mededelingenbladen":
          i = Index.Mededelingenbladen;
          break;
      }

      if (i != null)
      {
        StringBuilder html = new StringBuilder();
        foreach (var item in i.Items)
        {
          html.AppendFormat(dictionaries["template"], item.ToArray()).AppendLine();
        }
        return html.ToString();
      }
      return "";
    }
  }
}