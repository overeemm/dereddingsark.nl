using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dereddingsarknl.Models
{
  public class Menus
  {
    public static Menu Menu1 = new Menu(
      new MenuItem("", "welkom"),
      new MenuItem("audio", "audio",
          new MenuItem("bijbelstudies", "bijbelstudies"),
          new MenuItem("jeugddiensten", "jeugddiensten"),
          new MenuItem("maranatha-avonden", "maranatha avonden"),
          new MenuItem("samenkomsten baarn", "samenkomsten baarn"),
          new MenuItem("samenkomsten bunschoten", "samenkomsten bunschoten")
        ),
      new MenuItem("artikelen", "artikelen"),
      new MenuItem("contact", "contact"),
      new MenuItem("fotos", "foto's")
    );

    public static Menu Menu2 = new Menu(
      new MenuItem("over-de-gemeente", "over de gemeente",
          new MenuItem("grondslag", "grondslag"),
          new MenuItem("visie", "visie"),
          new MenuItem("broederraad", "broederraad"),
          new MenuItem("lidmaatschap", "lidmaatschap"),
          new MenuItem("doop", "doop"),
          new MenuItem("avondmaal", "avondmaal")
        ),
      new MenuItem("samenkomsten", "samenkomsten",
          new MenuItem("jeugddiensten", "jeugddiensten"),
          new MenuItem("praisediensten", "praisediensten")
        ),
      new MenuItem("gemeente-opbouw", "gemeente-opbouw"),
      new MenuItem("jeugd", "jeugd"),
      new MenuItem("pastoraat", "pastoraat"),
      new MenuItem("evangelisatie", "evangelisatie"),
      new MenuItem("zending", "zending")
    );

    public static Menu Menu3 = new Menu(
      new MenuItem("contactbladen", "contactbladen"),
      new MenuItem("mededelingenbladen", "mededelingenbladen")
    );
  }

  public class Menu 
  {
    public Menu(params MenuItem[] items)
    {
      Items = items;
    }

    public MenuItem[] Items{get; private set;}
  }

  public class MenuItem
  {

    public MenuItem(string key, string name, params MenuItem[] items)
    {
      Key = key;
      Name = name;
      Items = items ?? new MenuItem[0];
    }

    public string Key {get; private set;}
    public string Name {get; private set;}
    public MenuItem[] Items {get; private set;}
  }
}