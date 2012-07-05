using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using dereddingsarknl.Models;

namespace dereddingsarknl.Modules
{
  public class ActivateIndexHttpModule : IHttpModule
  {
    public void Dispose()
    {
    }

    public void Init(HttpApplication context)
    {
      context.BeginRequest += new EventHandler(context_BeginRequest);
    }

    void context_BeginRequest(object sender, EventArgs e)
    {
      //if (Index.Contactbladen == null)
      //{
      //  lock (Index.Lock)
      //  {
      //    if (Index.Contactbladen == null)
      //    {
      //      Index.Contactbladen = new Index((sender as HttpApplication).Server.MapPath("~/App_Data/indexen/contactbladen.csv"));
      //    }
      //  }
      //}
      //if (Index.Mededelingenbladen == null)
      //{
      //  lock (Index.Lock)
      //  {
      //    if (Index.Mededelingenbladen == null)
      //    {
      //      Index.Mededelingenbladen = new Index((sender as HttpApplication).Server.MapPath("~/App_Data/indexen/mededelingenbladen.csv"));
      //    }
      //  }
      //}
    }
  }
}