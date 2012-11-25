using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dereddingsarknl.Extensions
{
  public static class CollectionsHelper
  {
    public static IEnumerable<T> TakeRandom<T>(this IEnumerable<T> collection, int take)
    {
      var rand = new Random();
      var count = collection.Count();
      for(int i = 0; i < take && i < count; i++)
      {
        var randindex = rand.Next(count);
        yield return collection.Skip(randindex).Take(1).First();
      }
    }
  }
}