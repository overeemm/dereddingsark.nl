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
        return "Evangeliegemeente De Reddingsark";
      }
    }

    public SiteMailer()
    {
      MasterName = "_Layout";
    }

    public string EmailLogoPath { get; set; }

    public MvcMailMessage Contact(string fromname, string fromaddress, string message)
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

    public MvcMailMessage PasswordReset(User user, string reseturl)
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
        x.From = new MailAddress(Email_AddressTo, Email_Name);
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

    public MvcMailMessage GroupMail(IEnumerable<User> users, string subject, string htmlmessage, string message)
    {
      ViewBag.HtmlMessage = htmlmessage;
      ViewBag.Message = message;

      var resources = new Dictionary<string, string>();
      resources["emaillogo"] = EmailLogoPath;

      return Populate(x =>
      {
        x.Subject = subject;
        x.ViewName = "GroupMail";
        foreach(User u in users)
        {
          x.Bcc.Add(new MailAddress(u.Email, u.Name));
        }
        x.To.Add(new MailAddress(Email_AddressFrom, Email_Name));
        x.From = new MailAddress(Email_AddressFrom, Email_Name);
        x.ReplyTo = new MailAddress(Email_AddressReplyTo, Email_Name);
        x.LinkedResources = resources;
      });
    }

    public MvcMailMessage NewUsersBulk(List<string> failed)
    {
      ViewBag.FailedEmails = string.Join(", ", failed);

      var resources = new Dictionary<string, string>();
      resources["emaillogo"] = EmailLogoPath;

      return Populate(x =>
      {
        x.Subject = "Resultaat bulk users";
        x.ViewName = "NewUsersBulk";
        x.To.Add(new MailAddress(Email_AddressFrom, Email_Name));
        x.From = new MailAddress(Email_AddressFrom, Email_Name);
        x.ReplyTo = new MailAddress(Email_AddressReplyTo, Email_Name);
        x.LinkedResources = resources;
      });
    }
  }
}