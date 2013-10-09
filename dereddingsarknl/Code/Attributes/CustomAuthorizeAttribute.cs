using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using dereddingsarknl.Models;

namespace dereddingsarknl.Attributes
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
  public class CustomAuthorizeAttribute : FilterAttribute, IAuthorizationFilter
  {
    public string Role { get; set; }

    public virtual void OnAuthorization(AuthorizationContext filterContext)
    {
      if(filterContext == null)
      {
        throw new ArgumentNullException("filterContext");
      }

      var cookies = new CookieManager(filterContext);
      var users = new UserManager(new DataManager(filterContext.HttpContext));

      // only check when we actually have users
      if(users.GetUserCount() > 0)
      {
        User user = users.GetUser(cookies.GetUserToken());

        if(user == null)
          filterContext.Result = new HttpUnauthorizedResult("U heeft geen toegang.");
        else
        {
          if(!string.IsNullOrEmpty(Role))
          {
            var prop = user.GetType().GetProperty(Role);
            if(prop != null && !((bool)prop.GetValue(user)))
            {
              filterContext.Result = new HttpUnauthorizedResult("U heeft geen toegang.");
            }
          }
        }
      }
    }
  }
}