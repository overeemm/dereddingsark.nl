using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;

namespace dereddingsarknl.Models
{
  public class Index
  {
    public static Index CreateArticleIndex(HttpContextBase context)
    {
      string filePath = Path.Combine(Settings.GetDataFolder(context), "indexen/artikelen.csv");
      return new Index(filePath);
    }

    public static Index CreateAudioIndex(HttpContextBase context)
    {
      string filePath = Path.Combine(Settings.GetDataFolder(context), "indexen/audio.csv");
      return new Index(filePath);
    }

    public static Index CreatePhotoAlbumIndex(HttpContextBase context)
    {
      string filePath = Path.Combine(Settings.GetDataFolder(context), "indexen/fotos.csv");
      return new Index(filePath);
    }

    private List<List<string>> _contents = new List<List<string>>();

    public Index(string filename)
    {
      ParseCSV(filename);
    }

    private void ParseCSV(string filename)
    {
      foreach (string line in File.ReadAllLines(filename))
      {
        _contents.Add(line.Split(',').Select(vl => vl.Trim().Trim('"')).ToList());
      }
    }

    public IEnumerable<IEnumerable<string>> Items
    {
      get
      {
        return _contents;
      }
    }

    public string CreateLine(IEnumerable<string> item)
    {
      var newLine = new StringBuilder();
      foreach(var value in item)
      {
        if(newLine.Length > 0)
        {
          newLine.Append(",");
        }
        newLine.AppendFormat("\"{0}\"", value);
      }
      return newLine.ToString();
    }
  }
}