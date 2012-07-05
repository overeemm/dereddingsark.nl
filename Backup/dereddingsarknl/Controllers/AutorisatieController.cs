using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using dereddingsarknl.Models;

namespace dereddingsarknl.Controllers
{
  public class AutorisatieController : Controller
  {
    [HttpPost]
    public ActionResult Login(string referrer, string emailadres, string passwd, bool onthouden)
    {
      string hash = Users.Validate(emailadres, passwd);
      if (!string.IsNullOrEmpty(hash))
      {
        WriteCookie(hash, DateTime.Now.AddDays(28));
      }
      else
      {
        WriteCookie("Aanmelden is niet gelukt.");
      }

      if (string.IsNullOrEmpty(referrer))
      {
        return RedirectToAction("Show", "Page");
      }
      else
      {
        return Redirect(referrer);
      }
    }

    private void WriteCookie(string hash, DateTime? dateTime = null)
    {
      HttpCookie cookie = new HttpCookie("a");
      cookie.Value = hash;
      if (dateTime.HasValue)
      {
        cookie.HttpOnly = true;
        cookie.Expires = dateTime.Value;
      }
      this.ControllerContext.HttpContext.Response.Cookies.Add(cookie);
    }
  }
}
