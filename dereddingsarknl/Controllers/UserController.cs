using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using dereddingsarknl.Models;
using dereddingsarknl.Extensions;
using StackExchange.Profiling;
using MarkdownSharp;
using System.Text;

namespace dereddingsarknl.Controllers
{
  public class UserController : BaseController
  {
    [HttpPost]
    public ActionResult StoreNewBulk(string userlist, string sendmail)
    {
      if((CurrentUser == null || !CurrentUser.UserManager) && Users.GetUserCount() > 0)
        return new HttpUnauthorizedResult("U heeft geen toegang tot deze pagina.");

      List<string> failed = new List<string>();
      foreach(var userNameEmail in userlist.Split(new string[] { System.Environment.NewLine }, StringSplitOptions.None))
      {
        var email = userNameEmail.Split(';')[0].Trim();
        var name = userNameEmail.Split(';')[1].Trim();
        var extras = userNameEmail.Split(';')[2].Trim();

        var isTaken = false;
        using(MiniProfiler.Current.Step("Check uniqueness user " + email))
        {
          isTaken = Users.IsEmailAlreadyTaken(email);
        }
        if(isTaken)
        {
          failed.Add(userNameEmail);
        }
        else
        {
          using(MiniProfiler.Current.Step("Write user " + email))
          {
            Users.Add(name, email, extras);
          }
          var token = "";
          using(MiniProfiler.Current.Step("Write usertoken " + email))
          {
            token = Users.StoreResetToken(email, true);
          }
          using(MiniProfiler.Current.Step("Send mail " + email))
          {
            string reseturl = Url.AbsoluteHttpsAction("SetPassword", "User", new { token = token, reason = "nieuw" });
            Mailer.WelcomeNew(name, email, reseturl).Send(new SmtpClient().Wrap());
          }
        }
      }

      Mailer.NewUsersBulk(failed).Send(new SmtpClient().Wrap());

      return RedirectToAction("Show");
    }

    [HttpPost]
    public ActionResult StoreNew(string name, string email, string extras)
    {
      if((CurrentUser == null || !CurrentUser.UserManager) && Users.GetUserCount() > 0)
        return new HttpUnauthorizedResult("U heeft geen toegang tot deze pagina.");

      var isTaken = false;
      using(MiniProfiler.Current.Step("Check uniqueness user"))
      {
        isTaken = Users.IsEmailAlreadyTaken(email);
      }
      if(isTaken)
      {
        return RedirectToAction("Show", new { emailtaken = email });
      }
      else
      {
        using(MiniProfiler.Current.Step("Write user"))
        {
          Users.Add(name, email, extras);
        }
        var token = "";
        using(MiniProfiler.Current.Step("Write usertoken"))
        {
          token = Users.StoreResetToken(email, true);
        }
        using(MiniProfiler.Current.Step("Send mail"))
        {
          string reseturl = Url.AbsoluteHttpsAction("SetPassword", "User", new { token = token, reason = "nieuw" });
          Mailer.WelcomeNew(name, email, reseturl).Send(new SmtpClient().Wrap());
        }
        return RedirectToAction("Show");
      }
    }

    [HttpPost]
    public ActionResult StoreUpdate(string password, string password2, string referrer, string token, string reason)
    {
      User user;
      if(!string.IsNullOrEmpty(token))
      {
        user = Users.GetUserFromResetPasswordToken(token);
      }
      else
      {
        user = CurrentUser;
      }

      if(password == password2 && user != null)
      {
        Users.Update(user, password);
      }
      else if(password != password2)
      {
        Cookies.StoreMessage("De wachtwoorden komen niet overeen.");
        return RedirectToAction("SetPassword", new { token = token, reason = reason });
      }

      return Redirect(referrer);
    }

    private ActionResult ResetPassword(string email, string referrer)
    {
      var user = Users.GetUserFromEmail(email);
      if(user == null)
      {
        return Redirect(referrer);
      }
      else
      {
        var token = Users.StoreResetToken(email);
        string reseturl = Url.AbsoluteHttpsAction("SetPassword", "User", new { token = token, reason = "change" });

        Mailer.PasswordReset(user, reseturl).Send(new SmtpClient().Wrap());

        Cookies.StoreMessage("Er is een e-mail gestuurd met instructies om uw wachtwoord te resetten.");

        return Redirect(referrer);
      }
    }

    public ActionResult MailList(string to)
    {
      if(CurrentUser == null || !CurrentUser.Mailer)
        return new HttpUnauthorizedResult("U heeft geen toegang tot deze pagina.");

      var users = Users.GetUsers().ToList();
      if(to == "baarn")
      {
        users = users.Where(u => u.Baarn).ToList();
      }
      else if(to == "bunschoten")
      {
        users = users.Where(u => u.Bunschoten).ToList();
      }

      var count = users.Count();

      var sb = new StringBuilder();
      for(int i = 0; i < (count / 20); i++)
      {
        sb.AppendLine(string.Join(", ", users.Skip(i * 20).Take(20).Select(u => u.Email)));
        sb.AppendLine();
      }

      return Content(sb.ToString());
    }

    public ActionResult Diff(HttpPostedFileBase file)
    {
      if(CurrentUser == null || !CurrentUser.UserManager)
        return new HttpUnauthorizedResult("U heeft geen toegang tot deze pagina.");

      if(file != null && file.ContentLength > 0)
      {
        var b = new StreamReader(file.InputStream);
        var lines = b.ReadToEnd().Split('\n').Where(l => !string.IsNullOrEmpty(l.Trim())).Select(l => l.Trim());
        var newAddresses = new List<string>();
        foreach(var line in lines)
        {
          var users = Users.GetUsers().ToList();
          var user = users.SingleOrDefault(u => u.Email.Equals(line, StringComparison.InvariantCultureIgnoreCase));
          if(user == null)
          {
            newAddresses.Add(line);
          }
        }

        return Content(string.Join("; ", newAddresses));
      }

      return RedirectToAction("Show", "Index");
    }

    public ActionResult Mail(string to, string subject, string body, bool? test)
    {
      if(CurrentUser == null || !CurrentUser.Mailer)
        return new HttpUnauthorizedResult("U heeft geen toegang tot deze pagina.");

      if(Request.RequestType == "GET")
      {
        return View();
      }
      else
      {
        var users = Users.GetUsers().ToList();
        if(to == "baarn")
        {
          users = users.Where(u => u.Baarn).ToList();
        }
        else if(to == "bunschoten")
        {
          users = users.Where(u => u.Bunschoten).ToList();
        }
        if(test != null && test.Value)
        {
          users = new User[] { CurrentUser }.ToList();
        }

        string htmlmessage = new Markdown().Transform(body)
          .Replace("<p>", "<p style=\"color: #555; font-size: 15px; margin-top: 20px; padding: 10px;\">")
          .Replace("<a ", "<a style=\"color: #555; font-size: 15px;\" ");

        for(var i = 0; i < users.Count; i = i + 20)
        {
          var batch = users.Skip(i).Take(20);
          try
          {
            Mailer.GroupMail(batch, subject, htmlmessage, body).Send(new SmtpClient().Wrap());
          }
          catch(Exception e)
          {
            Elmah.ErrorLog.GetDefault(System.Web.HttpContext.Current)
              .Log(new Elmah.Error(
                new InvalidOperationException("Fout bij mailen aan " +
                      string.Join(", ", batch.Select(u => u.Email)), e)
               ));
          }
        }

        return RedirectToAction("Mail");
      }
    }

    public ActionResult SetPassword(string token, string reason)
    {
      User user = null;
      if(CurrentUser == null && !string.IsNullOrEmpty(token))
      {
        user = Users.GetUserFromResetPasswordToken(token);
      }
      else
      {
        user = CurrentUser;
      }
      if(user == null)
      {
        return RedirectToAction("Show", "Index");
      }
      else
      {
        ViewBag.NotExpandLogin = true;
        ViewBag.User = user;
        ViewBag.Token = token;
        ViewBag.Reason = reason;
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
        var token = Cookies.ClearUserToken(user);
        Users.RemoveToken(user, token);
        ClearCurrentUser();

        return Redirect(referrer);
      }
      return View();
    }

    public ActionResult CreateAPIToken(string email, string password)
    {
      if(CurrentUser == null || !CurrentUser.UserManager)
        return new HttpUnauthorizedResult("U heeft geen toegang tot deze pagina.");

      if(Request.RequestType == "GET")
      {
        return View();
      }
      else
      {
        ViewBag.Email = email;
        var user = Users.GetUserFromEmail(email);
        if(user == null)
        {
          ViewBag.Message = "Gebruiker is onbekend.";
          return View();
        }
        else
        {
          if(Users.CheckPassword(user, password))
          {
            ViewBag.Message = "API token gemaakt.";
            ViewBag.Token = Users.StoreNewApiToken(user).Token;
            return View();
          }
          else
          {
            ViewBag.Message = "Wachtwoord is fout";
            return View();
          }
        }
      }
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
          var user = Users.GetUserFromEmail(email);
          if(user == null)
          {
            Cookies.StoreMessage("Het e-mailadres is niet bekend of het ingevoerde wachtwoord is fout.");
            return Redirect(referrer);
          }
          else
          {
            if(string.IsNullOrEmpty(referrer))
            {
              referrer = Url.Action("Show", "Index");
            }

            if(Users.CheckPassword(user, password))
            {
              var token = Users.StoreNewToken(user, IpAddress);
              Cookies.StoreUserToken(token);
              return Redirect(referrer);
            }
            else
            {
              Cookies.StoreMessage("Het e-mailadres is niet bekend of het ingevoerde wachtwoord is fout.");
              return Redirect(referrer);
            }
          }
        }
      }
    }

    public ActionResult Show(string emailtaken)
    {
      if((CurrentUser == null || !CurrentUser.UserManager) && Users.GetUserCount() > 0)
        return new HttpUnauthorizedResult("U heeft geen toegang tot deze pagina.");

      ViewBag.EmailTaken = emailtaken;

      ViewBag.UserList = Users.GetUsers().OrderBy(u => u.Name).ToList();

      return View();
    }
  }
}
