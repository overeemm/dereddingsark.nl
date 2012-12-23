using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dereddingsarknl
{
  public enum ContentFolders
  {
    Photos
  }

  public enum PageFolders
  {
    About,
    Activity,
    Article
  }

  public enum CalendarFiles
  {
    Publiek
  }

  public enum DataFolders
  {
    UsersToken,
    Root,
    Indexes,
    Users,
    Calendar,
    UsersResetPasswords,
    InternBaarn,
    InternBunschoten,
    InternContactblad
  }

  public enum IndexFiles
  {
    Articles,
    Recordings,
    Photos,
    Users
  }
}