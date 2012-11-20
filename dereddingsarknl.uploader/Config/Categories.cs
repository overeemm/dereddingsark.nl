using System;
using System.Collections.Generic;
using System.Text;

namespace dereddingsarknl.uploader.Config
{
  public class Categories : List<Category>
  {
    private Categories()
    {
    }

    public static Categories Init()
    {
      return new Categories()
      {
        new Category("samenkomst baarn", "public_html/preken/samenkomsten", "preken/samenkomsten"),
        new Category("samenkomst bunschoten", "public_html/preken/samenkomsten", "preken/samenkomsten"),
        new Category("jeugddienst", "public_html/preken/jeugddiensten", "preken/jeugddiensten"),
        new Category("maranatha avond", "public_html/preken/maranatha", "preken/maranatha"),
        new Category("bijbelstudies", "public_html/bijbelstudies", "bijbelstudies")
      };
    }
  }
}
