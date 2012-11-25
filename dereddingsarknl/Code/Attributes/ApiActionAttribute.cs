using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dereddingsarknl.Attributes
{
  [AttributeUsage(AttributeTargets.Method)]
  public class ApiActionAttribute : Attribute
  {
  }
}