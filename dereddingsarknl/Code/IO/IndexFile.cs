using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.IO;
using System.Text;

namespace dereddingsarknl.Models
{
  public class IndexFile
  {
    private List<List<string>> _contents = new List<List<string>>();
    private string _filename;

    public IndexFile(string filename)
    {
      _filename = filename;
      ParseCSV(filename);
    }

    private void ParseCSV(string filename)
    {
      _contents = new List<List<string>>();
      foreach(string line in File.ReadAllLines(filename))
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
      ParseCSV(_filename);
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

    public void Remove(Predicate<IEnumerable<string>> predicate)
    {
      var lines = new List<string>();
      foreach(var item in this.Items)
      {
        if(!predicate(item))
        {
          lines.Add(this.CreateLine(item));
        }
      }

      System.IO.File.WriteAllLines(_filename, lines.ToArray());
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