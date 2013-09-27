using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Elmah;
using System.Net;
using System.Net.Mail;
using dereddingsarknl.Extensions;

namespace dereddingsarknl.Modules
{
  public class CustomErrorMailModule : ErrorMailModule
  {
    protected override void SendMail(MailMessage mail)
    {
      if(mail == null)
        throw new ArgumentNullException("mail");

      if(mail.Body.IndexOf("Are your start/end tags properly balanced?", StringComparison.InvariantCultureIgnoreCase) > -1)
      {
        // stupid workaround for rare production bug
        return;
      }

      if(HttpContext.Current != null && HttpContext.Current.Request.Url.AbsoluteUri.IndexOf("podcast", StringComparison.InvariantCultureIgnoreCase) > -1)
      {
        // stupid workaround for rare production bug
        return;
      }

      if(!System.Diagnostics.Debugger.IsAttached)
      {
        var client = new SmtpClient().Init();
        client.Send(mail);
      }
    }
  }
}