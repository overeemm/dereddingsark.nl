using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using dereddingsarknl.Controllers;
using dereddingsarknl.Models;

namespace dereddingsarknl
{
  public class CookieManager
  {
    protected const string Https_Cookie = "ark.secure";
    protected const string Login_Cookie = "ark.login";
    protected const string Message_Cookie = "ark.msg";

    private HttpResponseBase _response;
    private HttpRequestBase _request;

    public CookieManager(BaseController controller)
    {
      _response = controller.Response;
      _request = controller.Request;
    }

    public CookieManager(AuthorizationContext filterContext)
    {
      _response = filterContext.HttpContext.Response;
      _request = filterContext.HttpContext.Request;
    }

    public string GetMessage()
    {
      var msgcookie = _request.Cookies[Message_Cookie];
      if(msgcookie != null)
      {
        try
        {
          return msgcookie.Value ?? "";
        }
        catch { }
      }

      return "";
    }

    public void StoreMessage(string msg)
    {
      var responsecookie = new HttpCookie(Message_Cookie);
      responsecookie.Value = msg;
      _response.Cookies.Add(responsecookie);
    }

    public void ClearMessage()
    {
      var responsecookie = new HttpCookie(Message_Cookie);
      responsecookie.Value = "";
      responsecookie.Expires = DateTime.Now.AddDays(-1);
      _response.Cookies.Add(responsecookie);
    }

    public UserToken GetUserToken()
    {
      var cookie = _request.Cookies[Login_Cookie];
      if(cookie != null)
      {
        var guid = cookie.Values["guid"];
        var generated = cookie.Values["generated"];
        var token = cookie.Values["token"];
        var ipadress = _request.ServerVariables["REMOTE_ADDR"];

        return new UserToken
        {
          Guid = guid,
          Generated = generated,
          Token = token,
          IpAddress = ipadress
        };
      }
      else
      {
        return null;
      }
    }

    internal void StoreUserToken(UserToken token)
    {
      var responsecookie = new HttpCookie(Login_Cookie);
      responsecookie.Values.Add("guid", token.Guid);
      responsecookie.Values.Add("generated", token.Generated);
      responsecookie.Values.Add("token", token.Token);
      responsecookie.HttpOnly = true;
      responsecookie.Secure = Environment.SupportsHttps;
      responsecookie.Expires = token.GetExpiration(2);
      _response.Cookies.Add(responsecookie);

      var httpsResponsecookie = new HttpCookie(Https_Cookie);
      httpsResponsecookie.Value = "1";
      httpsResponsecookie.HttpOnly = true;
      httpsResponsecookie.Expires = token.GetExpiration(2);
      _response.Cookies.Add(httpsResponsecookie);
    }

    public UserToken ClearUserToken(User user)
    {
      var cookie = _request.Cookies[Login_Cookie];

      var responsecookie = new HttpCookie(Login_Cookie);
      responsecookie.HttpOnly = true;
      responsecookie.Secure = true;
      responsecookie.Expires = DateTime.Now.AddDays(-1);
      _response.Cookies.Add(responsecookie);

      var httpsResponsecookie = new HttpCookie(Https_Cookie);
      httpsResponsecookie.Value = "0";
      httpsResponsecookie.HttpOnly = true;
      httpsResponsecookie.Expires = DateTime.Now.AddDays(-1);
      _response.Cookies.Add(httpsResponsecookie);

      if(cookie != null)
      {
        var token = cookie.Values["token"];
        var guid = cookie.Values["guid"];
        return new UserToken() { Token = token, Guid = guid };
      }

      return null;
    }

    public bool UseHttps()
    {
      var cookie = _request.Cookies[Https_Cookie];
      return cookie != null && cookie.Value == "1" && Environment.SupportsHttps;
    }
  }
}