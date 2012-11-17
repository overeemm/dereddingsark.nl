using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Net;
using System.Xml.Linq;

namespace dereddingsarknl.Models
{
  public static class SmtpClientExtensions
  {
    private static string Host { get; set; }
    private static NetworkCredential Credentials { get; set; }
    private static int Port { get; set; }

    public static void InitSettings(HttpContext context)
    {
      if(string.IsNullOrEmpty(Host))
      {
        var settingsfiles = context.Server.MapPath("/Bin/mailsettings.xml");
        var settings = XDocument.Load(settingsfiles);

        Host = settings.Element("mail").Attribute("smtp").Value;
        Credentials = new NetworkCredential(settings.Element("mail").Attribute("username").Value, settings.Element("mail").Attribute("password").Value);
        Port = int.Parse(settings.Element("mail").Attribute("port").Value);
      }
    }

    public static SmtpClient Init(this SmtpClient client)
    {
      client.EnableSsl = true;
      client.Host = Host;
      client.DeliveryMethod = SmtpDeliveryMethod.Network;
      client.Credentials = Credentials;
      client.Port = Port;
      return client;
    }
  }
}