using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Elmah;
using System.Net;
using System.Net.Mail;
using dereddingsarknl.Extensions;

namespace dereddingsarknl.Modules
{
  public class CustomErrorsModule : IHttpModule
  {
    public CustomErrorsModule()
    {
    }

    public void Dispose()
    {
    }

    public void Init(HttpApplication context)
    {
      context.PreSendRequestHeaders += this.OnPreSendRequestHeaders;
      context.Error += ContextOnError;
    }

    private void ContextOnError(object sender, EventArgs eventArgs)
    {
      var app = sender as System.Web.HttpApplication;
      if(app != null)
      {
        var context = app.Context;
        if(context != null)
        {
          Exception exception = context.Server.GetLastError();
          context.Response.Clear();
          context.Response.Redirect(string.Format("{0}/error?message={1}", Environment.GetAbsoluteDomain(true), exception != null ? exception.Message : "unknown"));
          context.Response.End();
        }
      }
    }

    private void OnPreSendRequestHeaders(object sender, EventArgs e)
    {
      var app = sender as System.Web.HttpApplication;
      if(app != null)
      {
        var context = app.Context;
        if(context != null)
        {
          context.Response.Headers.Remove("Server");
          context.Response.Headers.Remove("X-AspNet-Version");
          context.Response.Headers.Remove("X-AspNetMvc-Version");
          context.Response.Headers.Remove("X-Powered-By");
        }
      }
    }

  }
}