using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dereddingsarknl.Models
{
  public class User
  {
    public string Email { get; set; }
    public string Name { get; set; }
    public string PasswordHash { get; set; }
    public string Salt { get; set; }
  }
}