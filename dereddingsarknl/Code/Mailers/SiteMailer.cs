using System.Collections.Generic;
using System.Net.Mail;
using dereddingsarknl.Controllers;
using dereddingsarknl.Models;
using Mvc.Mailer;

namespace dereddingsarknl.Mailers
{
  public class SiteMailer : MailerBase
  {
    public const string Email_AddressTo = "info@dereddingsark.nl";
    public const string Email_AddressFrom = "site@dereddingsark.nl";
    public const string Email_AddressReplyTo = "info@dereddingsark.nl";
    public string Email_Name
    {
      get
      {
        return System.Diagnostics.Debugger.IsAttached
          ? "DEBUG de reddingsark" : "Evangeliegemeente De Reddingsark";
      }
    }

    public SiteMailer()
    {
      MasterName = "_Layout";
    }

    public string EmailLogoPath { get; set; }

    public virtual MvcMailMessage Contact(string fromname, string fromaddress, string message)
    {
      ViewBag.FromName = fromname;
      ViewBag.FromAddress = fromaddress;
      ViewBag.Message = message;

      var resources = new Dictionary<string, string>();
      resources["emaillogo"] = EmailLogoPath;

      return Populate(x =>
      {
        x.Subject = "Bericht van " + fromname + " (via de site)";
        x.ViewName = "Contact";
        x.To.Add(Email_AddressTo);
        x.From = new MailAddress(Email_AddressFrom, Email_Name);
        x.ReplyTo = new MailAddress(fromaddress, fromname);
        x.LinkedResources = resources;
      });
    }

    public virtual MvcMailMessage PasswordReset(User user, string reseturl)
    {
      ViewBag.Naam = user.Name;
      ViewBag.ResetUrl = reseturl;

      var resources = new Dictionary<string, string>();
      resources["emaillogo"] = EmailLogoPath;

      return Populate(x =>
      {
        x.Subject = "Uw wachtwoord voor dereddingsark.nl";
        x.ViewName = "PasswordReset";
        x.To.Add(new MailAddress(user.Email, user.Name));
        x.From = new MailAddress(Email_AddressFrom, Email_Name);
        x.ReplyTo = new MailAddress(Email_AddressReplyTo, Email_Name);
        x.LinkedResources = resources;
      });
    }

    public virtual MvcMailMessage WelcomeNew(string name, string email, string reseturl)
    {
      ViewBag.Naam = name;
      ViewBag.ResetUrl = reseturl;
      ViewBag.Email = email;

      var resources = new Dictionary<string, string>();
      resources["emaillogo"] = EmailLogoPath;

      return Populate(x =>
      {
        x.Subject = "Uw account voor dereddingsark.nl";
        x.ViewName = "WelcomeNew";
        x.To.Add(new MailAddress(email, name));
        x.From = new MailAddress(Email_AddressFrom, Email_Name);
        x.ReplyTo = new MailAddress(Email_AddressReplyTo, Email_Name);
        x.LinkedResources = resources;
      });
    }
  }
}