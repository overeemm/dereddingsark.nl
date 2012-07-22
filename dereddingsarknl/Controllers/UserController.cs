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
        string filePath = Path.Combine(Settings.GetDataFolder(HttpContext), "gebruikers/index.csv");
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
            new string[] { 
              string.Format("\"{0}\", \"{1}\", \"{2}\", \"{3}\"", email.Trim(), name.Trim(), passwordHash, salt) 
            });

          var client = new System.Net.Mail.SmtpClient();
          client.Send(new MailMessage("website@dereddingsark.nl", email,
            "Gebruikersgegevens voor dereddingsark.nl",
            string.Format(@"Hierbij uw gegevens voor dereddingsark.nl

e-mailadres: {0}
wachtwoord: {1}", email, password)));

          var guid = Guid.NewGuid().ToString("N");
          string tokenFile = Path.Combine(Settings.GetDataFolder(HttpContext), "gebruikers/tokens", email.Replace("@", "-") + "__" + guid);
          System.IO.File.Create(tokenFile);

          return RedirectToAction("Add");
        }
      }
    }

    public ActionResult Login(string email, string password, string referrer)
    {
      var user = GetUser(email);
      if(user == null)
      {
        return Redirect(referrer);
      }
      else
      {
        if(user.PasswordHash == HashPassword(password, user.Salt))
        {
          StoreCookieAndToken(null, user);
          return Redirect(referrer);
        }
        else
        {
          return Redirect(referrer);
        }
      }
      return View();
    }

    public ActionResult Add(string emailtaken)
    {
      ViewBag.EmailTaken = emailtaken;
      return View();
    }
  }
}
