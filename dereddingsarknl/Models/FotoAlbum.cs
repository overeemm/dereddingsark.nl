using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace dereddingsarknl.Models
{
  public class FotoAlbum
  {
    private XDocument _content;
    XNamespace _atom = "http://www.w3.org/2005/Atom";

    public FotoAlbum(XDocument content)
    {
      _content = content;
    }
    public string Name { get; set; }

    public string Id { get; set; }

    public string Thumbnail
    {
      get
      {
        return _content.Element(_atom + "feed").Element(_atom + "icon").Value;
      }
    }

    public string Url
    {
      get
      {
        return _content.Element("feed").Elements("link").FirstOrDefault(e => e.Attribute("rel").Value == "alternate" && e.Attribute("type").Value == "text/html").Value;
      }
    }

    public IEnumerable<Photo> Photos
    {
      get
      {
        return _content.Element(_atom + "feed").Elements(_atom + "entry")
          .Select(e => new Photo(e.Element(_atom + "content").Attribute("src").Value));
      }
    }
  }

  public class Photo
  {
    public Photo(string url)
    {
      Url = url;
    }

    public string Url { get; private set; }
  }
}