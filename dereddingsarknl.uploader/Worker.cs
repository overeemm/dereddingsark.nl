using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using dereddingsarknl.uploader.Config;
using Limilabs.FTP.Client;

namespace dereddingsarknl.uploader
{
  public class Worker
  {
    public void Work(string sourceFile, string targetFileName, string friendlyName, DateTime datetime, Category category)
    {
      string tempFile = "";
      string mp3File = "";

      using(Mp3Transformer transformer = new Mp3Transformer(sourceFile))
      {
        try
        {
          tempFile = transformer.Encode();
        }
        catch(Exception exc)
        {
          throw new InvalidOperationException("Het maken van de MP3 is niet goed gegaan", exc);
        }

        try
        {
          using(Ftp ftp = new Ftp())
          {
            ftp.Connect(Config.Config.FTPAddress);
            ftp.Login(Config.Config.FTPUser, Config.Config.FTPPass);

            ftp.ChangeFolder(category.FTPPath);
            ftp.Upload(targetFileName, tempFile);

            mp3File = Config.Config.FTPSiteAddress + "/" + category.SitePath + "/" + targetFileName;

            ftp.Close();
          }
        }
        catch(Exception exc)
        {
          throw new InvalidOperationException("Uploaden (FTP) is niet goed gegaan", exc);
        }
      }

      try
      {
        string postData = String.Format("url={0}&name={1}&datetime={2}&categorie={3}", mp3File, friendlyName, datetime.ToString("yyyy-MM-dd HH:mm:ss"), category.Name);

        HttpWebRequest request = WebRequest.Create(Config.Config.Site) as HttpWebRequest;
        request.Method = "POST";
        request.Headers.Add("X-UserGuid", Config.Config.SiteUserGuid);
        request.Headers.Add("X-Token", Config.Config.SiteToken);
        request.ContentLength = postData.Length;
        request.ContentType = "application/x-www-form-urlencoded";

        ASCIIEncoding encoding = new ASCIIEncoding();
        byte[] postDataBytes = encoding.GetBytes(postData);
        request.ContentLength = postDataBytes.Length;
        Stream stream = request.GetRequestStream();
        stream.Write(postDataBytes, 0, postDataBytes.Length);

        HttpWebResponse webResp = (HttpWebResponse)request.GetResponse();
        if(webResp.StatusCode != HttpStatusCode.OK)
        {
          throw new InvalidOperationException("Uploaden is niet goed gegaan (" + webResp.StatusCode + ")");
        }
      }
      catch(InvalidOperationException)
      {
        throw;
      }
      catch(Exception exc)
      {
        throw new InvalidOperationException("Uploaden is niet goed gegaan", exc);
      }
    }

    public Category Category { get; set; }

    public DateTime Date { get; set; }

    public string FriendlyName { get; set; }

    public string TargetFileName { get; set; }

    public string SourceFile { get; set; }

    internal void Work()
    {
      Work(SourceFile, TargetFileName, FriendlyName, Date, Category);
    }
  }
}
