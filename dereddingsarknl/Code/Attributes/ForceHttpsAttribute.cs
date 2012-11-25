using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace dereddingsarknl.Attributes
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
  public class ForceHttpsAttribute : FilterAttribute, IAuthorizationFilter
  {
    public virtual void OnAuthorization(AuthorizationContext filterContext)
    {
      if(filterContext == null)
      {
        throw new ArgumentNullException("filterContext");
      }

      if(!filterContext.HttpContext.Request.IsSecureConnection)
      {
        HandleNonHttpsRequest(filterContext);
      }
    }

    protected virtual void HandleNonHttpsRequest(AuthorizationContext filterContext)
    {
      if(!String.Equals(filterContext.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
      {
        throw new InvalidOperationException("");
      }

      // redirect to HTTPS version of page
      string url = "https://" + Environment.GetAbsoluteDomain(true) + filterContext.HttpContext.Request.RawUrl;
      filterContext.Result = new RedirectResult(url);
    }
  }
}