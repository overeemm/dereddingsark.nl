using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using dereddingsarknl.Models;
using dereddingsarknl.Extensions;
using StackExchange.Profiling;

namespace dereddingsarknl.Controllers
{
  public class BaseController : Controller
  {
    protected const string Login_Cookie = "ark.login";
    protected const string Message_Cookie = "ark.msg";
    protected const string From_Address = "site@dereddingsark.nl";
    protected const string To_Address = "info@dereddingsark.nl";

    protected User CurrentUser { get; private set; }

    protected ViewResult PageNotFound()
    {
      Response.StatusCode = 404;
      return View("Custom404");
    }

    protected override void OnActionExecuting(ActionExecutingContext filterContext)
    {
      base.OnActionExecuting(filterContext);

      var cookie = filterContext.HttpContext.Request.Cookies[Login_Cookie];
      if(cookie != null)
      {
        try
        {
          var user = ValidateCookieAndToken(cookie);
          ViewBag.CurrentUser = CurrentUser = user;
        }
        catch { }
      }

      var userguid = filterContext.HttpContext.Request.Headers["X-UserGuid"];
      var token = filterContext.HttpContext.Request.Headers["X-Token"];
      if(!string.IsNullOrEmpty(userguid) && !string.IsNullOrEmpty(token))
      {
        try
        {
          var user = ValidateAPIToken(userguid, token);
          ViewBag.CurrentUser = CurrentUser = user;
        }
        catch { }
      }

      var msgcookie = filterContext.HttpContext.Request.Cookies[Message_Cookie];
      if(msgcookie != null)
      {
        try
        {
          ViewBag.Message = msgcookie.Value ?? "";
        }
        catch { }
      }

      RemoveMessageInCookie();

      if(CurrentUser == null || !CurrentUser.EnableProfiler)
      {
        MiniProfiler.Stop(true);
      }
      else
      {
        ViewBag.IncludeProfiler = true;
      }
    }

    protected override void OnActionExecuted(ActionExecutedContext filterContext)
    {
      if(CurrentUser != null)
      {
        StoreCookieAndToken(filterContext.HttpContext.Request.Cookies[Login_Cookie], CurrentUser);
      }

      if(filterContext.Result is HttpUnauthorizedResult && !filterContext.HttpContext.Request.IsAjaxRequest() && !filterContext.ActionDescriptor.IsApiAction())
      {
        filterContext.Result = RedirectToAction("Login", "User");
      }

      base.OnActionExecuted(filterContext);
    }

    protected string GetGuidFromUser(User user)
    {
      string tokenDir = Path.Combine(Settings.GetDataFolder(HttpContext), "gebruikers\\tokens");
      var begin = user.Email.Replace("@", "-") + "__";
      var file = new DirectoryInfo(tokenDir)
                .GetFiles()
                .First(f => f.Name.StartsWith(begin));

      return file.Name.Substring(begin.Length);
    }

    protected User ValidateCookieAndToken(HttpCookie cookie)
    {
      var guid = cookie.Values["guid"];
      var generated = cookie.Values["generated"];
      var token = cookie.Values["token"];
      var ipadress = Request.ServerVariables["REMOTE_ADDR"];

      string tokenDir = Path.Combine(Settings.GetDataFolder(HttpContext), "gebruikers\\tokens");
      var file = new DirectoryInfo(tokenDir)
                .GetFiles()
                .First(f => f.Name.EndsWith(guid));

      var tokenIndex = new Index(file.FullName);
      var item = tokenIndex.Items
                  .FirstOrDefault(i => i.First() == ipadress && i.Skip(1).First() == token && i.Skip(2).First() == generated);

      return item != null ? GetUserFromTokenFile(file.Name) : null;
    }

    protected User ValidateAPIToken(string guid, string token)
    {
      string tokenDir = Path.Combine(Settings.GetDataFolder(HttpContext), "gebruikers\\tokens");
      var file = new DirectoryInfo(tokenDir)
                .GetFiles()
                .First(f => f.Name.EndsWith(guid));

      var tokenIndex = new Index(file.FullName);
      var item = tokenIndex.Items
                  .FirstOrDefault(i => i.First() == "API" && i.Skip(1).First() == token);

      return item != null ? GetUserFromTokenFile(file.Name) : null;
    }

    protected void RemoveCookieAndToken(User user)
    {
      var cookie = Request.Cookies[Login_Cookie];
      if(cookie != null)
      {
        CurrentUser = null;
        var token = cookie.Values["token"];
        var guid = cookie.Values["guid"];
        string tokenFile = Path.Combine(Settings.GetDataFolder(HttpContext), "gebruikers\\tokens", user.Email.Replace("@", "-") + "__" + guid);

        var lines = new List<string>();
        var tokenIndex = new Index(tokenFile);
        foreach(var item in tokenIndex.Items)
        {
          if(item.Skip(1).First() != token)
          {
            lines.Add(tokenIndex.CreateLine(item));
          }
        }
        System.IO.File.WriteAllLines(tokenFile, lines.ToArray());
      }
    }

    protected void StoreMessageInCookie(string msg)
    {
      var responsecookie = new HttpCookie(Message_Cookie);
      responsecookie.Value = msg;
      Response.Cookies.Add(responsecookie);
    }

    protected void RemoveMessageInCookie()
    {
      var responsecookie = new HttpCookie(Message_Cookie);
      responsecookie.Value = "";
      responsecookie.Expires = DateTime.Now.AddDays(-1);
      Response.Cookies.Add(responsecookie);
    }

    /// <summary>
    /// Genereer voor de gegeven gebruiker een speciaal, eeuwig durend token.
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    protected Tuple<string, string> GenerateAndStoreAPIToken(User user)
    {
      var token = GenerateSalt();
      var generated = DateTime.UtcNow.ToString("yyyyMMddTHH:mm:ss");
      var guid = GetGuidFromUser(user);

      var tokenFileName = user.Email.Replace("@", "-") + "__" + guid;
        var tokenFile = Path.Combine(Settings.GetDataFolder(HttpContext), "gebruikers\\tokens", tokenFileName);
        System.IO.File.CreateText(tokenFile).Close();
        System.IO.File.AppendAllLines(tokenFile,
              new string[] { 
              string.Format("\"{0}\", \"{1}\", \"{2}\"", "API", token, generated) 
            });

        return new Tuple<string, string>(guid, token);
    }

    protected void StoreCookieAndToken(HttpCookie cookie, User user)
    {
      var newtoken = cookie == null;
      var ipadress = Request.ServerVariables["REMOTE_ADDR"];
      var token = newtoken ? GenerateSalt() : cookie.Values["token"];
      var generated = newtoken ? DateTime.UtcNow.ToString("yyyyMMddTHH:mm:ss") : cookie.Values["generated"];

      var guid = GetGuidFromUser(user);

      var responsecookie = new HttpCookie(Login_Cookie);
      responsecookie.Values.Add("guid", guid);
      responsecookie.Values.Add("generated", generated);
      responsecookie.Values.Add("token", token);
      responsecookie.HttpOnly = true;
      responsecookie.Expires = DateTime.Now.AddDays(14);
      // the refresh of a token is max 2 months after the first generated date
      if(!newtoken)
      {
        var year = int.Parse(generated.Substring(0, 4));
        var month = int.Parse(generated.Substring(4, 2));
        var day = int.Parse(generated.Substring(6, 2));
        var hour = int.Parse(generated.Substring(9, 2));
        var minute = int.Parse(generated.Substring(12, 2));
        var secondes = int.Parse(generated.Substring(15, 2));
        var generatedDate = new DateTime(year, month, day, hour, minute, secondes, DateTimeKind.Utc);
        if(generatedDate.AddMonths(2) < responsecookie.Expires)
        {
          responsecookie.Expires = generatedDate.AddMonths(2);
        }
      }

      Response.Cookies.Add(responsecookie);

      if(newtoken)
      {
        var tokenFileName = user.Email.Replace("@", "-") + "__" + guid;
        var tokenFile = Path.Combine(Settings.GetDataFolder(HttpContext), "gebruikers\\tokens", tokenFileName);
        System.IO.File.CreateText(tokenFile).Close();
        System.IO.File.AppendAllLines(tokenFile,
              new string[] { 
              string.Format("\"{0}\", \"{1}\", \"{2}\"", ipadress, token, generated) 
            });
      }
    }

    protected void StoreResetToken(string email, string token)
    {
      var tokenFileName = email.Replace("@", "-");
      var tokenFile = Path.Combine(Settings.GetDataFolder(HttpContext), "gebruikers\\resetpassword", tokenFileName);
      if(System.IO.File.Exists(tokenFile))
      {
        System.IO.File.Delete(tokenFile);
      }
      System.IO.File.CreateText(tokenFile).Close();
      System.IO.File.AppendAllLines(tokenFile,
            new string[] { 
              string.Format("\"{0}\", \"{1}\"", token, DateTime.Now.Ticks) 
            });
    }

    protected User GetUserFromResetPasswordToken(string token)
    {
      var resetPasswordDir = Path.Combine(Settings.GetDataFolder(HttpContext), "gebruikers\\resetpassword");
      foreach(var file in new DirectoryInfo(resetPasswordDir).GetFiles())
      {
        var content = System.IO.File.ReadAllLines(file.FullName).First();
        if(content.StartsWith("\"" + token))
        {
          return GetUserFromEmail(file.Name);
        }
      }
      return null;
    }

    protected string GenerateSalt()
    {
      var buf = new byte[16];
      new RNGCryptoServiceProvider().GetBytes(buf);
      return Convert.ToBase64String(buf);
    }

    protected string HashPassword(string password, string salt)
    {
      var crypto = new Rfc2898DeriveBytes(password, System.Text.Encoding.Default.GetBytes(salt), 10000);
      var hash = crypto.GetBytes(32);
      return Convert.ToBase64String(hash);
    }

    protected string GenerateNewPassword()
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

    private User GetUserFromTokenFile(string tokenFileName)
    {
      var users = Index.CreateUserIndex(HttpContext);
      var indexLine = users
        .Items
        .FirstOrDefault(i =>
          tokenFileName.StartsWith(i.First().Replace("@", "-"), StringComparison.InvariantCultureIgnoreCase));
      if(indexLine == null)
      {
        return null;
      }
      else
      {
        return dereddingsarknl.Models.User.Create(indexLine);
      }
    }

    protected int GetUserCount()
    {
      var users = Index.CreateUserIndex(HttpContext);
      return users.Items.Count();
    }

    protected User GetUserFromEmail(string email)
    {
      var normalizedEmail = email.Replace("@", "-");

      var users = Index.CreateUserIndex(HttpContext);
      var indexLine = (users.Items.FirstOrDefault(i => 
        i.First().Replace("@", "-")
         .Equals(normalizedEmail.Trim(), StringComparison.InvariantCultureIgnoreCase)));
      if(indexLine == null)
      {
        return null;
      }
      else
      {
        return dereddingsarknl.Models.User.Create(indexLine);
      }
    }

    protected PhotoAlbum GetAlbum(string id, string name, string folder)
    {
      //var xml = Path.Combine(Settings.GetDataFolder(HttpContext), "fotos", id + ".xml");
      var folderPath = Path.Combine(Settings.GetContentFolder(HttpContext), "fotos", folder);
      
      if(!System.IO.Directory.Exists(folderPath))
      {
        throw new HttpException(404, "Album " + id + " bestaat niet.");
      }

      return new PhotoAlbum(id, name, folderPath);
    }
  }
}
