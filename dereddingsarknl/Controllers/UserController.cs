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
  public class UserController : Controller
  {
    private User GetUser(string email)
    {
      string filePath = Path.Combine(Settings.GetDataFolder(HttpContext), "gebruikers/index.csv");
      var users = new Index(filePath);
      var indexLine = (users.Items.FirstOrDefault(i => i.First().Equals(email.Trim(), StringComparison.InvariantCultureIgnoreCase)));
      if(indexLine == null)
      {
        return null;
      }
      else
      {
        return new User()
        {
          Email = indexLine.First(),
          Name = indexLine.Skip(1).First(),
          PasswordHash = indexLine.Skip(2).First(),
          Salt = indexLine.Skip(3).First()
        };
      }
    }

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

          return RedirectToAction("Add");
        }
      }
    }

    private string GenerateSalt()
    {
      var buf = new byte[16];
      (new RNGCryptoServiceProvider()).GetBytes(buf);
      return Convert.ToBase64String(buf);
    }

    private string HashPassword(string password, string salt)
    {
      var crypto = new Rfc2898DeriveBytes(password, System.Text.Encoding.Default.GetBytes(salt), 10000);
      var hash = crypto.GetBytes(32);
      return Convert.ToBase64String(hash);
    }

    private string GenerateNewPassword()
    {
      var passwordLength = 10;
      string allowedLetterChars = "abcdefghijkmnpqrstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ";
      string allowedNumberChars = "23456789!@$?_-";
      char[] chars = new char[passwordLength];
      Random rd = new Random();

      bool useLetter = true;
      for(int i = 0; i < passwordLength; i++)
      {
        if(useLetter)
        {
          chars[i] = allowedLetterChars[rd.Next(0, allowedLetterChars.Length)];
          useLetter = false;
        }
        else
        {
          chars[i] = allowedNumberChars[rd.Next(0, allowedNumberChars.Length)];
          useLetter = true;
        }
      }

      return new string(chars);
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
