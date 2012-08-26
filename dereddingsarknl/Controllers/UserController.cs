using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using dereddingsarknl.Models;
using StackExchange.Profiling;

namespace dereddingsarknl.Controllers
{
  public class UserController : BaseController
  {
    [HttpPost]
    public ActionResult StoreNew(string name, string email)
    {
      using(MiniProfiler.Current.Step("Check uniqueness user and write if unique"))
      {
        string filePath = Path.Combine(Settings.GetDataFolder(HttpContext), "gebruikers\\index.csv");
        var users = new Index(filePath);

        if(users.Items.Any(i => i.First().Equals(email.Trim(), StringComparison.InvariantCultureIgnoreCase)))
        {
          return RedirectToAction("Add", new { emailtaken = email });
        }
        else
        {
          string password = GenerateNewPassword();
          string salt = GenerateSalt();
          string passwordHash = HashPassword(password, salt);

          System.IO.File.AppendAllLines(filePath,
            new string[] { dereddingsarknl.Models.User.CreateIndexLine(email, name, passwordHash, salt) });

          string token = Guid.NewGuid().ToString("N");
          StoreResetToken(email, token);
          string reseturl = Url.Action("SetPassword", "User", new { token = token });

          var client = new System.Net.Mail.SmtpClient();
          client.Send(new MailMessage(From_Address, email,
            "Gebruikersgegevens voor dereddingsark.nl",
            string.Format(@"Er is een account aangemaakt voor uw e-maildres ({0})

U kunt uw e-mailadres instellen via de url {1}", email, reseturl)));

          var guid = Guid.NewGuid().ToString("N");
          string tokenFile = Path.Combine(Settings.GetDataFolder(HttpContext), "gebruikers\\tokens", email.Replace("@", "-") + "__" + guid);
          System.IO.File.Create(tokenFile);

          return RedirectToAction("Add");
        }
      }
    }

    [HttpPost]
    public ActionResult StoreUpdate(string password, string password2, string referrer, string token)
    {
      User user;
      if(!string.IsNullOrEmpty(token))
      {
        user = GetUserFromResetPasswordToken(token);
      }
      else
      {
        user = CurrentUser;
      }

      if(password == password2 && user != null)
      {
        string salt = GenerateSalt();
        string passwordHash = HashPassword(password, salt);
        var indexLine = string.Format("\"{0}\", \"{1}\", \"{2}\", \"{3}\"", user.Email, user.Name, passwordHash, salt);

        string filePath = Path.Combine(Settings.GetDataFolder(HttpContext), "gebruikers\\index.csv");
        var users = new Index(filePath);
        var lines = new List<string>();
        foreach(var item in users.Items)
        {
          if(item.First().Equals(user.Email, StringComparison.InvariantCultureIgnoreCase))
          {
            lines.Add(indexLine);
          }
          else
          {
            lines.Add(users.CreateLine(item));
          }
        }

        System.IO.File.WriteAllLines(filePath, lines.ToArray());
      }
      return Redirect(referrer);
    }

    private ActionResult ResetPassword(string email, string referrer)
    {
      var user = GetUserFromEmail(email);
      if(user == null)
      {
        return Redirect(referrer);
      }
      else
      {
        string token = Guid.NewGuid().ToString("N");
        StoreResetToken(email, token);
        string reseturl = Url.Action("SetPassword", "User", new { token = token });

        var client = new System.Net.Mail.SmtpClient();
        client.Send(new MailMessage(From_Address, email,
          "Geef een wachtwoord op voor dereddingsark.nl",
          string.Format(@"Ga naar deze url om een wachtwoord op te geven: {0}", reseturl)));

        StoreMessageInCookie("Er is een e-mail gestuurd met instructies om uw wachtwoord te resetten.");

        return Redirect(referrer);
      }
    }

    public ActionResult SetPassword(string token)
    {
      var user = GetUserFromResetPasswordToken(token);
      if(user == null)
      {
        return RedirectToAction("Show", "Index");
      }
      else
      {
        ViewBag.User = user;
        ViewBag.Token = token;
        return View();
      }
    }

    public ActionResult Logout(string referrer)
    {
      var user = CurrentUser;
      if(user == null)
      {
        return Redirect(referrer);
      }
      else
      {
        RemoveCookieAndToken(user);
        return Redirect(referrer);
      }
      return View();
    }

    public ActionResult Login(string email, string password, string referrer, string inloggen, string reset)
    {
      if(!string.IsNullOrEmpty(reset))
      {
        return ResetPassword(email, referrer);
      }
      else
      {
        if(Request.RequestType == "GET")
        {
          ViewBag.Referrer = Request.UrlReferrer != null ? Request.UrlReferrer.ToString() : Url.Action("Show", "Index");
          return View();
        }
        else
        {
          var user = GetUserFromEmail(email);
          if(user == null)
          {
            StoreMessageInCookie("Het e-mailadres is niet bekend, of het ingevoerde wachtwoord is fout.");
            return Redirect(referrer);
          }
          else
          {
            if(string.IsNullOrEmpty(referrer))
            {
              referrer = Url.Action("Show", "Index");
            }

            if(user.PasswordHash == HashPassword(password, user.Salt))
            {
              StoreCookieAndToken(null, user);
              return Redirect(referrer);
            }
            else
            {
              StoreMessageInCookie("Het e-mailadres is niet bekend, of het ingevoerde wachtwoord is fout.");
              return Redirect(referrer);
            }
          }
        }
      }
    }

    public ActionResult Add(string emailtaken)
    {
      string filePath = Path.Combine(Settings.GetDataFolder(HttpContext), "gebruikers\\index.csv");
      if(!System.IO.File.Exists(filePath))
      {
        System.IO.File.CreateText(filePath).Close();
      }

      if((CurrentUser == null || !CurrentUser.CanAddUser) && GetUserCount() > 0)
        return new HttpUnauthorizedResult("U heeft geen toegang tot deze pagina.");

      ViewBag.EmailTaken = emailtaken;
      return View();
    }
  }
}
