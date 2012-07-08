using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace dereddingsarknl.Models
{
  public class Index
  {
    public static Index Contactbladen;
    public static Index Mededelingenbladen;
    public static object Lock = new object();

    private List<List<string>> _contents = new List<List<string>>();

    public Index(string filename)
    {
      if(Path.GetExtension(filename).EndsWith("csv", StringComparison.InvariantCultureIgnoreCase))
      {
        ParseCSV(filename);
      }
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
  }
}