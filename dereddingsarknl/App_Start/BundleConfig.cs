using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace dereddingsarknl
{
  public class BundleConfig
  {
    public static void RegisterBundles(BundleCollection bundles)
    {
      bundles.Add(
        new StyleBundle("~/Content/bundle_dereddingsark")
        {
          Orderer = new AsIsBundleOrderer()
        }
        .Include("~/Content/foundation.css")
        .Include("~/Content/app.css")
      );

      bundles.Add(
        new ScriptBundle("~/Scripts/bundle_modernizr")
        {
          Orderer = new AsIsBundleOrderer()
        }
        .Include("~/Scripts/modernizr.foundation.js")
      );

      bundles.Add(
        new ScriptBundle("~/Scripts/bundle_dereddingsark")
        {
          Orderer = new AsIsBundleOrderer()
        }
        .Include("~/Scripts/jquery-1.9.0.js")
        .Include("~/Scripts/jquery.orbit-1.4.0.js")
        .Include("~/Scripts/jquery.placeholder.js")
        .Include("~/Scripts/jquery.tooltips.js")
        .Include("~/Scripts/app.js")
      );
    }
  }

  public class AsIsBundleOrderer : IBundleOrderer
  {
    public IEnumerable<BundleFile> OrderFiles(BundleContext context, IEnumerable<BundleFile> files)
    {
      if(context == null)
        throw new ArgumentNullException("context");

      if(files == null)
        throw new ArgumentNullException("files");

      return files;
    }
  }
}