using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dereddingsarknl.Models
{
  public class User
  {
    private User() { }

    public string Email { get; set; }
    public string Name { get; set; }
    public string PasswordHash { get; set; }
    public string Salt { get; set; }

    public string Extras { get; private set; }
    public bool IsAPIUser { get; private set; }
    public bool UserManager { get; private set; }
    public bool AudioManager { get; private set; }
    public bool EnableProfiler { get; private set; }
    public bool Mailer { get; private set; }

    public bool Baarn { get; private set; }
    public bool Bunschoten { get; private set; }

    public static string CreateIndexLine(string email, string name, string passwordHash, string salt, string extras)
    {
      return string.Format("\"{0}\", \"{1}\", \"{2}\", \"{3}\", \"{4}\"", email.Trim(), name.Trim(), passwordHash, salt, extras);
    }

    public static User Create(IEnumerable<string> indexLine)
    {
      var extras = indexLine.Skip(4).FirstOrDefault() ?? "";

      return new User()
      {
        Email = indexLine.First(),
        Name = indexLine.Skip(1).First(),
        PasswordHash = indexLine.Skip(2).First(),
        Salt = indexLine.Skip(3).First(),
        Extras = extras,
        IsAPIUser = extras.Contains("api"),
        UserManager = extras.Contains("users"),
        AudioManager = extras.Contains("audio"),
        EnableProfiler = extras.Contains("profiler"),
        Mailer = extras.Contains("mail"),
        Baarn = extras.Contains("baarn"),
        Bunschoten = extras.Contains("bunschoten")
      };
    }
  }
}