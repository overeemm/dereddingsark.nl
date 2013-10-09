using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using dereddingsarknl.Models;

namespace dereddingsarknl.Extensions
{
  public static class IOHelper
  {
    public static void AppendCsvValues(this FileInfo file, params object[] values)
    {
      string line = string.Join(", ", values.Select(v => v == null ? "\"\"" : "\""+v.ToString()+"\""));

      if(!file.Exists)
      {
        File.CreateText(file.FullName).Close();
      }
      File.AppendAllLines(file.FullName, new string[] { line });
    }

    public static Calendar OpenCalendar(this FileInfo file, HttpContextBase context)
    {
      return CacheManager.Instance.GetLongCachedFile<Calendar>(file.FullName, () => Calendar.Get(context));
    }

    public static IndexFile OpenIndex(this FileInfo file)
    {
      return CacheManager.Instance.GetShortCachedFile<IndexFile>(file.FullName, () => new IndexFile(file.FullName));
    }

    public static MarkdownFile OpenMarkdown(this FileInfo file)
    {
      return CacheManager.Instance.GetLongCachedFile<MarkdownFile>(file.FullName, () => new MarkdownFile(file.FullName));
    }
  }
}