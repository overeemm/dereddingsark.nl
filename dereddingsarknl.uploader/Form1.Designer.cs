namespace dereddingsarknl.uploader
{
  partial class Form1
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if(disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
      this.label1 = new System.Windows.Forms.Label();
      this.cmbCategory = new System.Windows.Forms.ComboBox();
      this.label2 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.tbxSpreker = new System.Windows.Forms.TextBox();
      this.tbxTitel = new System.Windows.Forms.TextBox();
      this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
      this.label4 = new System.Windows.Forms.Label();
      this.progressBar1 = new System.Windows.Forms.ProgressBar();
      this.button1 = new System.Windows.Forms.Button();
      this.tbxFile = new System.Windows.Forms.TextBox();
      this.button2 = new System.Windows.Forms.Button();
      this.label5 = new System.Windows.Forms.Label();
      this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
      this.SuspendLayout();
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(29, 44);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(51, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "categorie";
      // 
      // cmbCategory
      // 
      this.cmbCategory.FormattingEnabled = true;
      this.cmbCategory.Location = new System.Drawing.Point(91, 41);
      this.cmbCategory.Name = "cmbCategory";
      this.cmbCategory.Size = new System.Drawing.Size(227, 21);
      this.cmbCategory.TabIndex = 1;
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(38, 71);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(42, 13);
      this.label2.TabIndex = 2;
      this.label2.Text = "spreker";
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(57, 98);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(23, 13);
      this.label3.TabIndex = 3;
      this.label3.Text = "titel";
      // 
      // tbxSpreker
      // 
      this.tbxSpreker.Location = new System.Drawing.Point(91, 68);
      this.tbxSpreker.Name = "tbxSpreker";
      this.tbxSpreker.Size = new System.Drawing.Size(227, 20);
      this.tbxSpreker.TabIndex = 4;
      // 
      // tbxTitel
      // 
      this.tbxTitel.Location = new System.Drawing.Point(91, 95);
      this.tbxTitel.Name = "tbxTitel";
      this.tbxTitel.Size = new System.Drawing.Size(227, 20);
      this.tbxTitel.TabIndex = 5;
      // 
      // dateTimePicker1
      // 
      this.dateTimePicker1.CustomFormat = "dd-MM-yyyy HH:mm:ss";
      this.dateTimePicker1.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
      this.dateTimePicker1.Location = new System.Drawing.Point(91, 122);
      this.dateTimePicker1.Name = "dateTimePicker1";
      this.dateTimePicker1.Size = new System.Drawing.Size(227, 20);
      this.dateTimePicker1.TabIndex = 6;
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(13, 126);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(67, 13);
      this.label4.TabIndex = 7;
      this.label4.Text = "datum en tijd";
      // 
      // progressBar1
      // 
      this.progressBar1.Location = new System.Drawing.Point(12, 178);
      this.progressBar1.Name = "progressBar1";
      this.progressBar1.Size = new System.Drawing.Size(306, 23);
      this.progressBar1.TabIndex = 8;
      // 
      // button1
      // 
      this.button1.Location = new System.Drawing.Point(242, 149);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(75, 23);
      this.button1.TabIndex = 9;
      this.button1.Text = "uploaden";
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new System.EventHandler(this.button1_Click);
      // 
      // tbxFile
      // 
      this.tbxFile.Location = new System.Drawing.Point(91, 15);
      this.tbxFile.Name = "tbxFile";
      this.tbxFile.Size = new System.Drawing.Size(196, 20);
      this.tbxFile.TabIndex = 10;
      // 
      // button2
      // 
      this.button2.Location = new System.Drawing.Point(293, 12);
      this.button2.Name = "button2";
      this.button2.Size = new System.Drawing.Size(24, 23);
      this.button2.TabIndex = 11;
      this.button2.Text = "...";
      this.button2.UseVisualStyleBackColor = true;
      this.button2.Click += new System.EventHandler(this.button2_Click);
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(35, 18);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(45, 13);
      this.label5.TabIndex = 12;
      this.label5.Text = "bestand";
      // 
      // openFileDialog1
      // 
      this.openFileDialog1.FileName = "openFileDialog1";
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(330, 213);
      this.Controls.Add(this.label5);
      this.Controls.Add(this.button2);
      this.Controls.Add(this.tbxFile);
      this.Controls.Add(this.button1);
      this.Controls.Add(this.progressBar1);
      this.Controls.Add(this.label4);
      this.Controls.Add(this.dateTimePicker1);
      this.Controls.Add(this.tbxTitel);
      this.Controls.Add(this.tbxSpreker);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.cmbCategory);
      this.Controls.Add(this.label1);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Name = "Form1";
      this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
      this.Text = "dereddingsark.nl audio uploader";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.ComboBox cmbCategory;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.TextBox tbxSpreker;
    private System.Windows.Forms.TextBox tbxTitel;
    private System.Windows.Forms.DateTimePicker dateTimePicker1;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.ProgressBar progressBar1;
    private System.Windows.Forms.Button button1;
    private System.Windows.Forms.TextBox tbxFile;
    private System.Windows.Forms.Button button2;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.OpenFileDialog openFileDialog1;
  }
}

