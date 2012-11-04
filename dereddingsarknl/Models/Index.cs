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

    public static Index CreateUserIndex(HttpContextBase context)
    {
      string filePath = Path.Combine(Settings.GetDataFolder(context), "gebruikers\\index.csv");
      if(!System.IO.File.Exists(filePath))
      {
        System.IO.File.CreateText(filePath).Close();
      }
      return new Index(filePath);
    }

    private List<List<string>> _contents = new List<List<string>>();
    private string _filename;

    public Index(string filename)
    {
      _filename = filename;
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

    public void Add(string newLine)
    {
      System.IO.File.AppendAllLines(_filename, new string[] { newLine });
    }

    public bool Contains(Predicate<IEnumerable<string>> predicate)
    {
      var found = this.Items.Where(i => predicate(i)).ToList();
      return found.Count > 0;
    }

    public IEnumerable<string> Find(Predicate<IEnumerable<string>> predicate)
    {
      var found = this.Items.Where(i => predicate(i)).ToList();
      return found.Count > 0 ? found[0] : new string[] { };
    }

    public void Update(Predicate<IEnumerable<string>> predicate, string newLine)
    {
      var lines = new List<string>();
      foreach(var item in this.Items)
      {
        if(predicate(item))
        {
          lines.Add(newLine);
        }
        else
        {
          lines.Add(this.CreateLine(item));
        }
      }

      System.IO.File.WriteAllLines(_filename, lines.ToArray());
    }
  }
}