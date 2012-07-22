using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using dereddingsarknl.Models;

namespace dereddingsarknl.Controllers
{
  public class BaseController : Controller
  {
    protected override void OnActionExecuting(ActionExecutingContext filterContext)
    {
      var cookie = filterContext.HttpContext.Request.Cookies["login"];
      if(cookie != null)
      {
        var email = cookie.Values["email"];
        var token = cookie.Values["token"];
        var generated = cookie.Values["generated"];

        //if(IsTokenValid(email, token, generated))
        //{

        //}
      }
    }

    protected string GetUserGuid(User user)
    {
      string tokenDir = Path.Combine(Settings.GetDataFolder(HttpContext), "gebruikers/tokens");
      var begin = user.Email.Replace("@", "-") + "__";
      var file = new DirectoryInfo(tokenDir)
                .GetFiles()
                .First(f => f.Name.StartsWith(begin));

      return file.Name.Substring(begin.Length);
    }

    protected bool ValidateCookieAndToken(HttpRequestBase request, User user)
    {
      var guid = GetUserGuid(user);
      var ipadress = Request.ServerVariables["REMOTE_ADDR"];

      var tokenFileName = user.Email.Replace("@", "-") + "__" + guid;
      var tokenFile = Path.Combine(Settings.GetDataFolder(HttpContext), "gebruikers/tokens", tokenFileName);

      var cookie = request.Cookies["login"];
      var generated = cookie.Values["generated"];
      var token = cookie.Values["token"];
      
      var tokenIndex = new Index(tokenFile);
      var item = tokenIndex.Items
                  .FirstOrDefault(i => i.First() == ipadress && i.Skip(1).First() == token && i.Skip(2).First() == generated);

      return item != null;
    }

    protected void StoreCookieAndToken(HttpRequestBase request, User user)
    {
      var cookie = request.Cookies["login"];
      var newtoken = cookie == null;
      var ipadress = Request.ServerVariables["REMOTE_ADDR"];
      var token = newtoken ? GenerateSalt() : cookie.Values["token"];
      var generated = newtoken ? DateTime.UtcNow.ToString("yyyyMMddTHH:mm:ss") : cookie.Values["generated"];

      var guid = GetUserGuid(user);

      var responsecookie = new HttpCookie("login");
      responsecookie.Values.Add("guid", guid);
      responsecookie.Values.Add("generated", generated);
      responsecookie.Values.Add("token", token);

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

      if(newtoken != null)
      {
        var tokenFileName = user.Email.Replace("@", "-") + "__" + guid;
        var tokenFile = Path.Combine(Settings.GetDataFolder(HttpContext), "gebruikers/tokens", tokenFileName);
        System.IO.File.AppendAllLines(tokenFile,
              new string[] { 
              string.Format("\"{0}\", \"{1}\", \"{2}\"", ipadress, token, generated) 
            });
      }
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

    protected User GetUser(string email)
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
  }
}
