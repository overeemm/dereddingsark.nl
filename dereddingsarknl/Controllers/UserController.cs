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

namespace dereddingsarknl.Controllers
{
  public class UserController : BaseController
  {
    [HttpPost]
    public ActionResult StoreNewBulk(string userlist, string sendmail, string sync)
    {
      //if((CurrentUser == null || !CurrentUser.UserManager) && GetUserCount() > 0)
      //  return new HttpUnauthorizedResult("U heeft geen toegang tot deze pagina.");

      //using(MiniProfiler.Current.Step("Store new users"))
      //{
      //  var users = Index.CreateUserIndex(HttpContext);
      //  List<string> failed = new List<string>();
      //  foreach(var userNameEmail in userlist.Split(new string[] { Environment.NewLine }, StringSplitOptions.None))
      //  {
      //    try
      //    {
      //      var name = userNameEmail.Split(' ')[0].Trim();
      //      var email = userNameEmail.Split(' ')[1].Trim();
      //    }
      //    catch
      //    {
      //      failed.Add(userNameEmail);
      //    }
      //  }

      return RedirectToAction("Show");
      //}
    }

    [HttpPost]
    public ActionResult StoreNew(string name, string email, string extras)
    {
      if((CurrentUser == null || !CurrentUser.UserManager) && Users.GetUserCount() > 0)
        return new HttpUnauthorizedResult("U heeft geen toegang tot deze pagina.");

      using(MiniProfiler.Current.Step("Check uniqueness user and write if unique"))
      {
        if(Users.IsEmailAlreadyTaken(email))
        {
          return RedirectToAction("Show", new { emailtaken = email });
        }
        else
        {
          Users.Add(name, email, extras);
          var token = Users.StoreResetToken(email, true);

          string reseturl = Url.AbsoluteHttpsAction("SetPassword", "User", new { token = token });
          Mailer.WelcomeNew(name, email, reseturl).Send(new SmtpClient().Wrap());

          return RedirectToAction("Show");
        }
      }
    }

    [HttpPost]
    public ActionResult StoreUpdate(string password, string password2, string referrer, string token)
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
        return RedirectToAction("SetPassword", new { token = token });
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
        string reseturl = Url.AbsoluteHttpsAction("SetPassword", "User", new { token = token });

        Mailer.PasswordReset(user, reseturl).Send(new SmtpClient().Wrap());

        Cookies.StoreMessage("Er is een e-mail gestuurd met instructies om uw wachtwoord te resetten.");

        return Redirect(referrer);
      }
    }

    public ActionResult SetPassword(string token)
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
