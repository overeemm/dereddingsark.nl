using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Taskbar;

namespace dereddingsarknl.uploader
{
  public partial class Form1 : Form
  {
    public Form1()
    {
      InitializeComponent();

      cmbCategory.DataSource = Config.Categories.Init();
      button2.Focus();

      var lastSunday = DateTime.Now;
      while(lastSunday.DayOfWeek != DayOfWeek.Sunday)
      {
        lastSunday = lastSunday.Subtract(new TimeSpan(1, 0, 0, 0));
      }
      lastSunday = new DateTime(lastSunday.Year, lastSunday.Month, lastSunday.Day, 9, 45, 00);
      dateTimePicker1.Value = lastSunday;
    }

    private void button1_Click(object sender, EventArgs e)
    {
      if(string.IsNullOrEmpty(tbxFile.Text) || !File.Exists(tbxFile.Text))
      {
        MessageBox.Show(this, "Selecteer een bestand", "Geen bestand", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }

      if(string.IsNullOrEmpty(tbxSpreker.Text) || string.IsNullOrEmpty(tbxSpreker.Text.Trim()))
      {
        MessageBox.Show(this, "Vul een spreker in", "Geen spreker", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }

      string name = tbxSpreker.Text.Trim();
      if(!string.IsNullOrEmpty(tbxTitel.Text) && !string.IsNullOrEmpty(tbxTitel.Text.Trim()))
      {
        name = name + " - " + tbxTitel.Text.Trim();
      }

      string date = dateTimePicker1.Value.ToString("yyyyMMdd");
      string filename = CreateSafeName(date, name) + ".mp3";

      LockUI(true);

      progressBar1.Style = ProgressBarStyle.Marquee;
      progressBar1.UseWaitCursor = true;
      if(TaskbarManager.IsPlatformSupported)
      {
        TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Indeterminate);
      }

      BackgroundWorker worker = new BackgroundWorker();
      worker.DoWork += worker_DoWork;
      worker.RunWorkerCompleted += worker_RunWorkerCompleted;
      worker.RunWorkerAsync(new Worker()
      {
        SourceFile = tbxFile.Text,
        TargetFileName = filename,
        FriendlyName = name,
        Date = dateTimePicker1.Value,
        Category = cmbCategory.SelectedItem as Config.Category
      });
    }

    private void LockUI(bool locked)
    {
      tbxFile.Enabled = !locked;
      tbxSpreker.Enabled = !locked;
      tbxTitel.Enabled = !locked;
      dateTimePicker1.Enabled = !locked;
      button1.Enabled = !locked;
      button2.Enabled = !locked;
      cmbCategory.Enabled = !locked;
    }

    void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      if(e.Result is Exception)
      {
        MessageBox.Show(this, (e.Result as Exception).Message, "Fout", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
      else
      {
        MessageBox.Show(this, "De opname is geplaatst", "Geplaats", MessageBoxButtons.OK, MessageBoxIcon.Information);
        tbxSpreker.Text = "";
        tbxFile.Text = "";
        tbxTitel.Text = "";
      }

      LockUI(false);

      progressBar1.Style = ProgressBarStyle.Blocks;
      progressBar1.UseWaitCursor = false;
      progressBar1.Value = 0;
      if(TaskbarManager.IsPlatformSupported)
      {
        TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.NoProgress);
      }
    }

    void worker_DoWork(object sender, DoWorkEventArgs e)
    {
      try
      {
        (e.Argument as Worker).Work();
        e.Result = true;
      }
      catch(Exception ex)
      {
        e.Result = ex;
      }
    }

    private string CreateSafeName(string date, string name)
    {
      string filename = date + "-" + name;
      filename = filename.Replace(" ", "_");
      foreach(char c in Path.GetInvalidFileNameChars())
      {
        filename = filename.Replace(c, '_');
      }
      foreach(char c in Path.GetInvalidPathChars())
      {
        filename = filename.Replace(c, '_');
      }
      return filename.ToLowerInvariant();
    }

    private void button2_Click(object sender, EventArgs e)
    {
      openFileDialog1.Filter = "MP3 bestanden (*.mp3)|";
      openFileDialog1.FileName = "";
      openFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
      var result = openFileDialog1.ShowDialog(this);
      if(result == System.Windows.Forms.DialogResult.OK)
      {
        tbxFile.Text = openFileDialog1.FileName;
      }
    }
  }
}
