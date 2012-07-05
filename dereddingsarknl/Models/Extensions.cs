using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Web.Mvc;

namespace dereddingsarknl.Models
{
  public static class Extensions
  {
    public static IHtmlString BuildMenu1(UrlHelper urlHelper, string currentPage)
    {
      return BuildMenu(urlHelper, currentPage, Menus.Menu1);
    }

    public static IHtmlString BuildMenu3(UrlHelper urlHelper, string currentPage)
    {
      return BuildMenu(urlHelper, currentPage, Menus.Menu3);
    }

    public static IHtmlString BuildMenu2(UrlHelper urlHelper, string currentPage)
    {
      return BuildMenu(urlHelper, currentPage, Menus.Menu2);
    }

    private static IHtmlString BuildMenu(UrlHelper urlHelper, string currentPage, Menu menu)
    {
      StringBuilder menuHtml = new StringBuilder();
      menuHtml.Append("<ul class=\"menu\">");
      BuildMenuItems(urlHelper, "", currentPage, menuHtml, menu.Items);
      menuHtml.Append("</ul>");

      return MvcHtmlString.Create(menuHtml.ToString());
    }

    private static void BuildMenuItems(UrlHelper urlHelper, string parent, string currentPage, StringBuilder menuHtml, MenuItem[] items)
    {
      foreach (MenuItem item in items)
      {
        bool isActive = currentPage == item.Key;
        menuHtml.AppendFormat(@"<li class=""parent"">", isActive ? "active" : "");
        menuHtml.AppendFormat(@"<a href=""{0}""><span>{1}</span></a>", urlHelper.Page(parent, item.Key), item.Name);

        if ((isActive || item.Items.Any(i => i.Key == currentPage)) && items.Length > 0)
        {
          menuHtml.Append("<ul>");
          BuildMenuItems(urlHelper, item.Key, currentPage, menuHtml, item.Items);
          menuHtml.Append("</ul>");
        }

        menuHtml.Append("</li>");
      }
    }

    public static string Page(this UrlHelper urlHelper, string fileName)
    {
      return urlHelper.Action("Show", "Page", new { fileName = fileName });
    }

    public static string Page(this UrlHelper urlHelper, string folderName, string fileName)
    {
      if (string.IsNullOrEmpty(folderName))
      {
        return urlHelper.Page(fileName);
      }
      return urlHelper.Action("ShowInFolder", "Page", new { folderName = folderName, fileName = fileName });
    }
  }
}