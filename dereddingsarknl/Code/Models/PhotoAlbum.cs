using System;
using System.Collections.Generic;
using System.IO;
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

    private DirectoryInfo _folder;
    private List<Photo> _photos;
    private Photo _thumb;

    public PhotoAlbum(string id, string name, XDocument content)
    {
      Id = id;
      Name = name;
      _content = content;
      _photos = _content.Element(_atom + "feed").Elements(_atom + "entry")
            .Select(e => new Photo(
              e.Element(_atom + "content").Attribute("src").Value
            )).ToList();
      SetThumb();
    }

    public PhotoAlbum(string id, string name, string folder)
    {
      Id = id;
      Name = name;
      _folder = new DirectoryInfo(folder);
      _photos = _folder.GetFiles("*.jpg").Select(f => new Photo("/Content/Fotos/" + _folder.Name + "/" + f.Name)).ToList();
      SetThumb();
    }

    private void SetThumb()
    {
      var rand = new Random();
      _thumb = _photos[rand.Next(_photos.Count)];
    }

    public int Count { get { return _photos.Count; } }

    public string Name { get; private set; }

    public string Id { get; private set; }

    public string Thumbnail
    {
      get
      {
        return _thumb.Url;
      }
    }

    public IEnumerable<Photo> Photos
    {
      get
      {
        return _photos;
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