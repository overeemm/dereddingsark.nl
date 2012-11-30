using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

namespace dereddingsarknl
{
  public class HeaderManager
  {

    public ApiToken GetApiToken(NameValueCollection headers)
    {
      var userguid = headers["X-UserGuid"];
      var token = headers["X-Token"];
      if(!string.IsNullOrEmpty(userguid) && !string.IsNullOrEmpty(token))
      {
        return new ApiToken() { Guid = userguid.Trim(), Token = token.Trim() };
      }

      return null;
    }
  }
}