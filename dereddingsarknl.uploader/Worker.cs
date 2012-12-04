using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using dereddingsarknl.uploader.Config;

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
          FtpWebRequest request = (FtpWebRequest)WebRequest.Create(Config.Config.FTPAddress + category.FTPPath + "/" + targetFileName);
          request.Method = WebRequestMethods.Ftp.UploadFile;
          request.Credentials = new NetworkCredential(Config.Config.FTPUser, Config.Config.FTPPass);
          request.UsePassive = true;
          request.UseBinary = true;
          request.KeepAlive = false;

          StreamReader sourceStream = new StreamReader(tempFile);
          byte[] buffer = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
          sourceStream.Close();
          request.ContentLength = buffer.Length;

          Stream reqStream = request.GetRequestStream();
          reqStream.Write(buffer, 0, buffer.Length);
          reqStream.Close();

          FtpWebResponse response = (FtpWebResponse)request.GetResponse();
          string sResult = response.StatusDescription;

          mp3File = Config.Config.FTPSiteAddress + "/" + category.SitePath + "/" + targetFileName;
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
