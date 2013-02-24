using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading;

namespace dereddingsarknl.uploader
{
  public class Mp3Transformer : IDisposable
  {
    private string _tempfile;
    private string _source;

    public Mp3Transformer(string source)
    {
      _source = source;
      _tempfile = Path.GetTempFileName();
      FileInfo fileInfo = new FileInfo(_tempfile);
      fileInfo.Attributes = FileAttributes.Temporary;
    }

    private static string LameEncoder
    {
      get
      {
        var lameexe = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "lame.exe");
        if(!File.Exists(lameexe))
        {
          lameexe = System.IO.Path.Combine(new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).Directory.FullName, "lame.exe");
        }
        return lameexe;
      }
    }

    public string Encode()
    {
      if(!string.IsNullOrEmpty(_source) && !string.IsNullOrEmpty(_tempfile))
      {
        System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo();
        psi.FileName = LameEncoder;
        psi.Arguments = string.Format("-V9 -b 32 -h \"{0}\" \"{1}\"", _source, _tempfile);
        psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
        Process p = System.Diagnostics.Process.Start(psi);
        while(p.HasExited == false)
        {
          Thread.Sleep(100);
          Application.DoEvents();
        }
        p.Dispose();

        return _tempfile;
      }
      else
      {
        throw new ArgumentException("Kan MP3 file niet converteren.");
      }
    }

    public void Dispose()
    {
      try
      {
        if(!string.IsNullOrEmpty(_tempfile) && File.Exists(_tempfile))
        {
          File.Delete(_tempfile);
        }
      }
      catch { }
    }

  }
}
