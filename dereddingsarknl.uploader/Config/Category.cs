using System;
using System.Collections.Generic;
using System.Text;

namespace dereddingsarknl.uploader.Config
{
  public class Category
  {
    public Category(string name, string ftppath, string sitepath)
    {
      Name = name;
      FTPPath = ftppath;
      SitePath = sitepath;
    }

    public string Name { get; set; }
    public string FTPPath { get; set; }
    public string SitePath { get; set; }

    public override string ToString()
    {
      return Name;
    }
  }
}
