﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Elmah;
using System.Net;
using System.Net.Mail;
using dereddingsarknl.Models;

namespace dereddingsarknl.Modules
{
  public class CustomErrorMailModule : ErrorMailModule
  {
    protected override void SendMail(MailMessage mail)
    {
      if(mail == null)
        throw new ArgumentNullException("mail");

      var client = new SmtpClient().Init();
      client.Send(mail);
    }
  }
}