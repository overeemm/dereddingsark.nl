using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace dereddingsarknl.Models
{
  /// <summary>
  /// Download xml files through URL like "https://picasaweb.google.com/data/feed/api/user/evangeliegemeentedereddingsark@gmail.com/albumid/{0}?kind=photo"
  /// </summary>
  public class PhotoAlbum
  {
    private XDocument _content;
    XNamespace _atom = "http://www.w3.org/2005/Atom";
    XNamespace _gphoto = "http://schemas.google.com/photos/2007";

    public PhotoAlbum(XDocument content)
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
          .Select(e => new Photo(
            e.Element(_atom + "content").Attribute("src").Value,
            int.Parse(e.Element(_gphoto + "width").Value),
            int.Parse(e.Element(_gphoto + "height").Value)
          ));
      }
    }
  }

  public class Photo
  {
    public Photo(string url, int width, int height)
    {
      Url = url;
      Width = width;
      Height = height;
    }

    public int Width { get; private set; }
    public int Height { get; private set; }

    public string Url { get; private set; }
  }
}