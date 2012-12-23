using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using dereddingsarknl.Models;

namespace dereddingsarknl
{
  public class DataManager
  {
    private HttpContextBase _httpContext;

    public DataManager(HttpContextBase httpContext)
    {
      _httpContext = httpContext;
    }

    private string GetDataFolder()
    {
      var folder = ConfigurationManager.AppSettings["dataFolder"];

      if(string.IsNullOrEmpty(folder) || !System.Diagnostics.Debugger.IsAttached)
      {
        folder = _httpContext.Server.MapPath(string.Format("/App_Data"));
      }
      return folder;
    }

    public string GetContentFolder()
    {
      var folder = ConfigurationManager.AppSettings["dataFolder"];
      if(string.IsNullOrEmpty(folder) || !System.Diagnostics.Debugger.IsAttached)
      {
        folder = _httpContext.Server.MapPath(string.Format("/Content"));
      }
      return folder;
    }

    public FileInfo GetFileBasedOnContent(DataFolders folder, Predicate<string> predicate)
    {
      foreach(var file in GetDirectory(folder).GetFiles())
      {
        foreach(var line in File.ReadAllLines(file.FullName))
        {
          if(predicate(line))
          {
            return file;
          }
        }
      }
      return null;
    }

    public DirectoryInfo GetDirectory(ContentFolders folder)
    {
      switch(folder)
      {
        case ContentFolders.Photos:
          return new DirectoryInfo(Path.Combine(GetContentFolder(), "fotos"));
      }

      return null;
    }

    public DirectoryInfo GetDirectory(PageFolders folder)
    {
      switch(folder)
      {
        case PageFolders.About:
          return new DirectoryInfo(Path.Combine(GetDataFolder(), "paginas", "over-de-gemeente"));
        case PageFolders.Activity:
          return new DirectoryInfo(Path.Combine(GetDataFolder(), "paginas", "activiteiten"));
        case PageFolders.Article:
          return new DirectoryInfo(Path.Combine(GetDataFolder(), "paginas", "artikelen"));
      }

      return null;
    }

    public DirectoryInfo GetDirectory(DataFolders folder)
    {
      switch(folder)
      {
        case DataFolders.Users:
          return new DirectoryInfo(Path.Combine(GetDataFolder(), "gebruikers"));
        case DataFolders.UsersToken:
          return new DirectoryInfo(Path.Combine(GetDataFolder(), "gebruikers", "tokens"));
        case DataFolders.UsersResetPasswords:
          return new DirectoryInfo(Path.Combine(GetDataFolder(), "gebruikers", "resetpassword"));
        case DataFolders.Indexes:
          return new DirectoryInfo(Path.Combine(GetDataFolder(), "indexen"));
        case DataFolders.InternBaarn:
          return new DirectoryInfo(Path.Combine(GetDataFolder(), "intern", "baarn"));
        case DataFolders.InternBunschoten:
          return new DirectoryInfo(Path.Combine(GetDataFolder(), "intern", "bunschoten"));
        case DataFolders.InternContactblad:
          return new DirectoryInfo(Path.Combine(GetDataFolder(), "intern", "contactblad"));
        case DataFolders.Calendar:
          return new DirectoryInfo(Path.Combine(GetDataFolder(), "calendar"));
      }

      return null;
    }

    public FileInfo GetFile(DataFolders folder, string name)
    {
      return new FileInfo(Path.Combine(GetDirectory(folder).FullName, name));
    }

    public FileInfo GetFile(PageFolders folder, string name)
    {
      return new FileInfo(Path.Combine(GetDirectory(folder).FullName, name));
    }

    public FileInfo GetFile(DataFolders folder, CalendarFiles file)
    {
      return new FileInfo(Path.Combine(GetDirectory(folder).FullName, GetFileName(file)));
    }

    public FileInfo GetFile(DataFolders folder, IndexFiles file)
    {
      return new FileInfo(Path.Combine(GetDirectory(folder).FullName, GetFileName(file)));
    }

    public FileInfo GetFile(DataFolders folder, Predicate<FileInfo> predicate)
    {
      return this.GetDirectory(DataFolders.UsersToken)
          .GetFiles()
          .First(f => predicate(f));
    }

    private string GetFileName(CalendarFiles file)
    {
      switch(file)
      {
        case CalendarFiles.Publiek:
          return "publiek.ics";
      }
      return "";
    }

    private string GetFileName(IndexFiles file)
    {
      switch(file)
      {
        case IndexFiles.Articles:
          return "artikelen.csv";
        case IndexFiles.Photos:
          return "fotos.csv";
        case IndexFiles.Recordings:
          return "audio.csv";
        case IndexFiles.Users:
          return "index.csv";
      }
      return "";
    }

    public FileInfo GetNewFile(DataFolders folder, string name)
    {
      var fileName = Path.Combine(GetDirectory(folder).FullName, name);
      if(File.Exists(fileName))
      {
        File.Delete(fileName);
      }
      File.CreateText(fileName).Close();

      return new FileInfo(fileName);
    }

    public DirectoryInfo GetPhotoDirectory(string folder)
    {
      var path = Path.Combine(GetDirectory(ContentFolders.Photos).FullName, folder);
      return new DirectoryInfo(path);
    }
  }
}