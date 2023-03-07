namespace HardwareInformation
{
    partial class LoginForm
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
            if (disposing && (components != null))
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginForm));
            this.textBoxUser = new System.Windows.Forms.TextBox();
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.AuthButton = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.aboutLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.configurableQualityPictureBox1 = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.checkBoxOfflineMode = new System.Windows.Forms.CheckBox();
            this.loadingCircle1 = new MRG.Controls.UI.LoadingCircle();
            this.configurableQualityPictureBox2 = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.configurableQualityPictureBox5 = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.configurableQualityPictureBox4 = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.configurableQualityPictureBox3 = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.comboBoxServerPort = new CustomFlatComboBox();
            this.comboBoxServerIP = new CustomFlatComboBox();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.configurableQualityPictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.configurableQualityPictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.configurableQualityPictureBox5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.configurableQualityPictureBox4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.configurableQualityPictureBox3)).BeginInit();
            this.SuspendLayout();
            // 
            // textBoxUser
            // 
            this.textBoxUser.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxUser.ForeColor = System.Drawing.SystemColors.WindowText;
            this.textBoxUser.Location = new System.Drawing.Point(91, 143);
            this.textBoxUser.Name = "textBoxUser";
            this.textBoxUser.Size = new System.Drawing.Size(139, 20);
            this.textBoxUser.TabIndex = 0;
            // 
            // textBoxPassword
            // 
            this.textBoxPassword.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxPassword.ForeColor = System.Drawing.SystemColors.WindowText;
            this.textBoxPassword.Location = new System.Drawing.Point(91, 168);
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.Size = new System.Drawing.Size(139, 20);
            this.textBoxPassword.TabIndex = 1;
            this.textBoxPassword.UseSystemPasswordChar = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.Location = new System.Drawing.Point(39, 146);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Usuário:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label2.Location = new System.Drawing.Point(39, 171);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Senha:";
            // 
            // AuthButton
            // 
            this.AuthButton.BackColor = System.Drawing.SystemColors.Control;
            this.AuthButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AuthButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.AuthButton.Location = new System.Drawing.Point(11, 269);
            this.AuthButton.Name = "AuthButton";
            this.AuthButton.Size = new System.Drawing.Size(219, 48);
            this.AuthButton.TabIndex = 5;
            this.AuthButton.Text = "Autenticar";
            this.AuthButton.UseVisualStyleBackColor = true;
            this.AuthButton.Click += new System.EventHandler(this.authButton_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.BackColor = System.Drawing.SystemColors.Control;
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutLabel,
            this.toolStripStatusLabel1,
            this.toolStripStatusLabel2});
            this.statusStrip1.Location = new System.Drawing.Point(0, 330);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.statusStrip1.Size = new System.Drawing.Size(241, 22);
            this.statusStrip1.TabIndex = 6;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // aboutLabel
            // 
            this.aboutLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.aboutLabel.Name = "aboutLabel";
            this.aboutLabel.Size = new System.Drawing.Size(37, 17);
            this.aboutLabel.Text = "Sobre";
            this.aboutLabel.Click += new System.EventHandler(this.aboutLabel_Click);
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.BackColor = System.Drawing.SystemColors.Control;
            this.toolStripStatusLabel1.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.toolStripStatusLabel1.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
            this.toolStripStatusLabel1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(185, 17);
            this.toolStripStatusLabel1.Spring = true;
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.toolStripStatusLabel2.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
            this.toolStripStatusLabel2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(4, 17);
            // 
            // configurableQualityPictureBox1
            // 
            this.configurableQualityPictureBox1.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.AssumeLinear;
            this.configurableQualityPictureBox1.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            this.configurableQualityPictureBox1.Location = new System.Drawing.Point(-5, -5);
            this.configurableQualityPictureBox1.Name = "configurableQualityPictureBox1";
            this.configurableQualityPictureBox1.Size = new System.Drawing.Size(252, 133);
            this.configurableQualityPictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.configurableQualityPictureBox1.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            this.configurableQualityPictureBox1.TabIndex = 5;
            this.configurableQualityPictureBox1.TabStop = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label3.Location = new System.Drawing.Point(39, 224);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Porta:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label4.Location = new System.Drawing.Point(39, 197);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(49, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Servidor:";
            // 
            // checkBoxOfflineMode
            // 
            this.checkBoxOfflineMode.AutoSize = true;
            this.checkBoxOfflineMode.ForeColor = System.Drawing.SystemColors.ControlText;
            this.checkBoxOfflineMode.Location = new System.Drawing.Point(91, 248);
            this.checkBoxOfflineMode.Name = "checkBoxOfflineMode";
            this.checkBoxOfflineMode.Size = new System.Drawing.Size(86, 17);
            this.checkBoxOfflineMode.TabIndex = 4;
            this.checkBoxOfflineMode.Text = "Modo Offline";
            this.checkBoxOfflineMode.UseVisualStyleBackColor = true;
            this.checkBoxOfflineMode.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // loadingCircle1
            // 
            this.loadingCircle1.Active = false;
            this.loadingCircle1.BackColor = System.Drawing.SystemColors.Control;
            this.loadingCircle1.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircle1.InnerCircleRadius = 5;
            this.loadingCircle1.Location = new System.Drawing.Point(13, 271);
            this.loadingCircle1.Name = "loadingCircle1";
            this.loadingCircle1.NumberSpoke = 12;
            this.loadingCircle1.OuterCircleRadius = 11;
            this.loadingCircle1.RotationSpeed = 1;
            this.loadingCircle1.Size = new System.Drawing.Size(215, 44);
            this.loadingCircle1.SpokeThickness = 2;
            this.loadingCircle1.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            this.loadingCircle1.TabIndex = 135;
            this.loadingCircle1.Text = "loadingCircle23";
            this.loadingCircle1.Visible = false;
            // 
            // configurableQualityPictureBox2
            // 
            this.configurableQualityPictureBox2.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.configurableQualityPictureBox2.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            this.configurableQualityPictureBox2.Location = new System.Drawing.Point(11, 139);
            this.configurableQualityPictureBox2.Name = "configurableQualityPictureBox2";
            this.configurableQualityPictureBox2.Size = new System.Drawing.Size(24, 25);
            this.configurableQualityPictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.configurableQualityPictureBox2.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.configurableQualityPictureBox2.TabIndex = 139;
            this.configurableQualityPictureBox2.TabStop = false;
            // 
            // configurableQualityPictureBox5
            // 
            this.configurableQualityPictureBox5.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.configurableQualityPictureBox5.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            this.configurableQualityPictureBox5.Location = new System.Drawing.Point(11, 217);
            this.configurableQualityPictureBox5.Name = "configurableQualityPictureBox5";
            this.configurableQualityPictureBox5.Size = new System.Drawing.Size(24, 25);
            this.configurableQualityPictureBox5.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.configurableQualityPictureBox5.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.configurableQualityPictureBox5.TabIndex = 138;
            this.configurableQualityPictureBox5.TabStop = false;
            // 
            // configurableQualityPictureBox4
            // 
            this.configurableQualityPictureBox4.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.configurableQualityPictureBox4.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            this.configurableQualityPictureBox4.Location = new System.Drawing.Point(11, 191);
            this.configurableQualityPictureBox4.Name = "configurableQualityPictureBox4";
            this.configurableQualityPictureBox4.Size = new System.Drawing.Size(24, 25);
            this.configurableQualityPictureBox4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.configurableQualityPictureBox4.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.configurableQualityPictureBox4.TabIndex = 137;
            this.configurableQualityPictureBox4.TabStop = false;
            // 
            // configurableQualityPictureBox3
            // 
            this.configurableQualityPictureBox3.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.configurableQualityPictureBox3.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            this.configurableQualityPictureBox3.Location = new System.Drawing.Point(11, 165);
            this.configurableQualityPictureBox3.Name = "configurableQualityPictureBox3";
            this.configurableQualityPictureBox3.Size = new System.Drawing.Size(24, 25);
            this.configurableQualityPictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.configurableQualityPictureBox3.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.configurableQualityPictureBox3.TabIndex = 136;
            this.configurableQualityPictureBox3.TabStop = false;
            // 
            // comboBoxServerPort
            // 
            this.comboBoxServerPort.BackColor = System.Drawing.SystemColors.Window;
            this.comboBoxServerPort.BorderColor = System.Drawing.SystemColors.WindowText;
            this.comboBoxServerPort.ButtonColor = System.Drawing.SystemColors.Window;
            this.comboBoxServerPort.FormattingEnabled = true;
            this.comboBoxServerPort.Location = new System.Drawing.Point(91, 221);
            this.comboBoxServerPort.Name = "comboBoxServerPort";
            this.comboBoxServerPort.Size = new System.Drawing.Size(139, 21);
            this.comboBoxServerPort.TabIndex = 3;
            // 
            // comboBoxServerIP
            // 
            this.comboBoxServerIP.BackColor = System.Drawing.SystemColors.Window;
            this.comboBoxServerIP.BorderColor = System.Drawing.SystemColors.WindowText;
            this.comboBoxServerIP.ButtonColor = System.Drawing.SystemColors.Window;
            this.comboBoxServerIP.FormattingEnabled = true;
            this.comboBoxServerIP.Location = new System.Drawing.Point(91, 194);
            this.comboBoxServerIP.Name = "comboBoxServerIP";
            this.comboBoxServerIP.Size = new System.Drawing.Size(139, 21);
            this.comboBoxServerIP.TabIndex = 2;
            // 
            // LoginForm
            // 
            this.AcceptButton = this.AuthButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(241, 352);
            this.Controls.Add(this.configurableQualityPictureBox2);
            this.Controls.Add(this.configurableQualityPictureBox5);
            this.Controls.Add(this.configurableQualityPictureBox4);
            this.Controls.Add(this.configurableQualityPictureBox3);
            this.Controls.Add(this.loadingCircle1);
            this.Controls.Add(this.comboBoxServerPort);
            this.Controls.Add(this.comboBoxServerIP);
            this.Controls.Add(this.checkBoxOfflineMode);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.configurableQualityPictureBox1);
            this.Controls.Add(this.AuthButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxPassword);
            this.Controls.Add(this.textBoxUser);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "LoginForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Login";
            this.Load += new System.EventHandler(this.Form2_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.configurableQualityPictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.configurableQualityPictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.configurableQualityPictureBox5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.configurableQualityPictureBox4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.configurableQualityPictureBox3)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxUser;
        private System.Windows.Forms.TextBox textBoxPassword;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button AuthButton;
        private ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox configurableQualityPictureBox1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.CheckBox checkBoxOfflineMode;
        private CustomFlatComboBox comboBoxServerIP;
        private CustomFlatComboBox comboBoxServerPort;
        private MRG.Controls.UI.LoadingCircle loadingCircle1;
        private ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox configurableQualityPictureBox2;
        private ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox configurableQualityPictureBox5;
        private ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox configurableQualityPictureBox4;
        private ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox configurableQualityPictureBox3;
        private System.Windows.Forms.ToolStripStatusLabel aboutLabel;
    }
}