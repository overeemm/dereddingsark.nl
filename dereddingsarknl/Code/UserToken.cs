using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dereddingsarknl
{
  public class UserToken
  {
    public string Guid { get; set; }
    public string Generated { get; set; }
    public string Token { get; set; }
    public string IpAddress { get; set; }

    public DateTime GetExpiration(int nrOfMonths)
    {
      var year = int.Parse(Generated.Substring(0, 4));
      var month = int.Parse(Generated.Substring(4, 2));
      var day = int.Parse(Generated.Substring(6, 2));
      var hour = int.Parse(Generated.Substring(9, 2));
      var minute = int.Parse(Generated.Substring(12, 2));
      var secondes = int.Parse(Generated.Substring(15, 2));
      var generatedDate = new DateTime(year, month, day, hour, minute, secondes, DateTimeKind.Utc);
      return generatedDate.AddMonths(2);
    }
  }

  public class ApiToken
  {
    public string Guid { get; set; }
    public string Token { get; set; }
  }
}