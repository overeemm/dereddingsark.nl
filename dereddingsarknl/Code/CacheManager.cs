using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;

namespace dereddingsarknl
{
  public class CacheManager
  {
    private HttpContextBase _context;
    private Dictionary<string, object> _locks = new Dictionary<string, object>();
    private static object _lockLock = new object();

    private static CacheManager _instance;
    public static CacheManager Instance
    {
      get
      {
        if(_instance == null)
        {
          lock(_lockLock)
          {
            if(_instance == null)
            {
              _instance = new CacheManager();
            }
          }
        }
        return _instance;
      }
    }

    private CacheManager()
    {
    }

    private object GetLock(string key)
    {
      object lockObj;
      if(!_locks.ContainsKey(key))
      {
        lock(_lockLock)
        {
          if(!_locks.ContainsKey(key))
          {
            lockObj = new object();
            _locks.Add(key, lockObj);
          }
          else
          {
            lockObj = _locks[key];
          }
        }
      }
      else
      {
        lockObj = _locks[key];
      }
      return lockObj;
    }

    public T GetCachedFile<T>(string filePath, Func<T> constructor) where T : class
    {
      T obj = HttpRuntime.Cache[filePath] as T;
      if(obj == null)
      {
        lock(GetLock(filePath))
        {
          if(obj == null)
          {
            obj = constructor();
            HttpRuntime.Cache.Add(filePath, obj,
              new CacheDependency(filePath), Cache.NoAbsoluteExpiration,
              Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
          }
        }
      }
      return obj;
    }
  }
}