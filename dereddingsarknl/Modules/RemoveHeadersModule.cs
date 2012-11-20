using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dereddingsarknl.Modules
{
  public class RemoveHeadersModule : IHttpModule
  {
    public RemoveHeadersModule()
    {
    }

    public void Dispose()
    {
    }

    public void Init(HttpApplication context)
    {
      context.PreSendRequestHeaders += this.OnPreSendRequestHeaders;
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