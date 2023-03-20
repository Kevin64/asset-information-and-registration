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
            this.lblFixedUser = new System.Windows.Forms.Label();
            this.lblFixedPassword = new System.Windows.Forms.Label();
            this.AuthButton = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.aboutLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusBarText = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripVersionText = new System.Windows.Forms.ToolStripStatusLabel();
            this.topBannerImg = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.lblFixedServerPort = new System.Windows.Forms.Label();
            this.lblFixedServerIP = new System.Windows.Forms.Label();
            this.checkBoxOfflineMode = new System.Windows.Forms.CheckBox();
            this.loadingCircle1 = new MRG.Controls.UI.LoadingCircle();
            this.userIconImg = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.serverPortIconImg = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.serverIPIconImg = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.passwordIconImg = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.comboBoxServerPort = new CustomFlatComboBox();
            this.comboBoxServerIP = new CustomFlatComboBox();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.topBannerImg)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.userIconImg)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.serverPortIconImg)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.serverIPIconImg)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.passwordIconImg)).BeginInit();
            this.SuspendLayout();
            // 
            // textBoxUser
            // 
            resources.ApplyResources(this.textBoxUser, "textBoxUser");
            this.textBoxUser.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxUser.ForeColor = System.Drawing.SystemColors.WindowText;
            this.textBoxUser.Name = "textBoxUser";
            // 
            // textBoxPassword
            // 
            resources.ApplyResources(this.textBoxPassword, "textBoxPassword");
            this.textBoxPassword.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxPassword.ForeColor = System.Drawing.SystemColors.WindowText;
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.UseSystemPasswordChar = true;
            // 
            // lblFixedUser
            // 
            resources.ApplyResources(this.lblFixedUser, "lblFixedUser");
            this.lblFixedUser.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFixedUser.Name = "lblFixedUser";
            // 
            // lblFixedPassword
            // 
            resources.ApplyResources(this.lblFixedPassword, "lblFixedPassword");
            this.lblFixedPassword.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFixedPassword.Name = "lblFixedPassword";
            // 
            // AuthButton
            // 
            resources.ApplyResources(this.AuthButton, "AuthButton");
            this.AuthButton.BackColor = System.Drawing.SystemColors.Control;
            this.AuthButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.AuthButton.Name = "AuthButton";
            this.AuthButton.UseVisualStyleBackColor = true;
            this.AuthButton.Click += new System.EventHandler(this.AuthButton_Click);
            // 
            // statusStrip1
            // 
            resources.ApplyResources(this.statusStrip1, "statusStrip1");
            this.statusStrip1.BackColor = System.Drawing.SystemColors.Control;
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutLabel,
            this.toolStripStatusBarText,
            this.toolStripVersionText});
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            // 
            // aboutLabel
            // 
            resources.ApplyResources(this.aboutLabel, "aboutLabel");
            this.aboutLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.aboutLabel.Name = "aboutLabel";
            this.aboutLabel.Click += new System.EventHandler(this.AboutLabel_Click);
            // 
            // toolStripStatusBarText
            // 
            resources.ApplyResources(this.toolStripStatusBarText, "toolStripStatusBarText");
            this.toolStripStatusBarText.BackColor = System.Drawing.SystemColors.Control;
            this.toolStripStatusBarText.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.toolStripStatusBarText.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
            this.toolStripStatusBarText.ForeColor = System.Drawing.SystemColors.ControlText;
            this.toolStripStatusBarText.Name = "toolStripStatusBarText";
            this.toolStripStatusBarText.Spring = true;
            // 
            // toolStripVersionText
            // 
            resources.ApplyResources(this.toolStripVersionText, "toolStripVersionText");
            this.toolStripVersionText.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.toolStripVersionText.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
            this.toolStripVersionText.ForeColor = System.Drawing.SystemColors.ControlText;
            this.toolStripVersionText.Name = "toolStripVersionText";
            // 
            // topBannerImg
            // 
            resources.ApplyResources(this.topBannerImg, "topBannerImg");
            this.topBannerImg.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.AssumeLinear;
            this.topBannerImg.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            this.topBannerImg.Name = "topBannerImg";
            this.topBannerImg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            this.topBannerImg.TabStop = false;
            // 
            // lblFixedServerPort
            // 
            resources.ApplyResources(this.lblFixedServerPort, "lblFixedServerPort");
            this.lblFixedServerPort.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFixedServerPort.Name = "lblFixedServerPort";
            // 
            // lblFixedServerIP
            // 
            resources.ApplyResources(this.lblFixedServerIP, "lblFixedServerIP");
            this.lblFixedServerIP.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFixedServerIP.Name = "lblFixedServerIP";
            // 
            // checkBoxOfflineMode
            // 
            resources.ApplyResources(this.checkBoxOfflineMode, "checkBoxOfflineMode");
            this.checkBoxOfflineMode.ForeColor = System.Drawing.SystemColors.ControlText;
            this.checkBoxOfflineMode.Name = "checkBoxOfflineMode";
            this.checkBoxOfflineMode.UseVisualStyleBackColor = true;
            this.checkBoxOfflineMode.CheckedChanged += new System.EventHandler(this.CheckBox1_CheckedChanged);
            // 
            // loadingCircle1
            // 
            resources.ApplyResources(this.loadingCircle1, "loadingCircle1");
            this.loadingCircle1.Active = false;
            this.loadingCircle1.BackColor = System.Drawing.SystemColors.Control;
            this.loadingCircle1.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircle1.InnerCircleRadius = 5;
            this.loadingCircle1.Name = "loadingCircle1";
            this.loadingCircle1.NumberSpoke = 12;
            this.loadingCircle1.OuterCircleRadius = 11;
            this.loadingCircle1.RotationSpeed = 1;
            this.loadingCircle1.SpokeThickness = 2;
            this.loadingCircle1.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // userIconImg
            // 
            resources.ApplyResources(this.userIconImg, "userIconImg");
            this.userIconImg.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.userIconImg.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            this.userIconImg.Name = "userIconImg";
            this.userIconImg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.userIconImg.TabStop = false;
            // 
            // serverPortIconImg
            // 
            resources.ApplyResources(this.serverPortIconImg, "serverPortIconImg");
            this.serverPortIconImg.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.serverPortIconImg.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            this.serverPortIconImg.Name = "serverPortIconImg";
            this.serverPortIconImg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.serverPortIconImg.TabStop = false;
            // 
            // serverIPIconImg
            // 
            resources.ApplyResources(this.serverIPIconImg, "serverIPIconImg");
            this.serverIPIconImg.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.serverIPIconImg.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            this.serverIPIconImg.Name = "serverIPIconImg";
            this.serverIPIconImg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.serverIPIconImg.TabStop = false;
            // 
            // passwordIconImg
            // 
            resources.ApplyResources(this.passwordIconImg, "passwordIconImg");
            this.passwordIconImg.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.passwordIconImg.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            this.passwordIconImg.Name = "passwordIconImg";
            this.passwordIconImg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.passwordIconImg.TabStop = false;
            // 
            // comboBoxServerPort
            // 
            resources.ApplyResources(this.comboBoxServerPort, "comboBoxServerPort");
            this.comboBoxServerPort.BackColor = System.Drawing.SystemColors.Window;
            this.comboBoxServerPort.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(122)))), ((int)(((byte)(122)))), ((int)(((byte)(122)))));
            this.comboBoxServerPort.ButtonColor = System.Drawing.SystemColors.Window;
            this.comboBoxServerPort.FormattingEnabled = true;
            this.comboBoxServerPort.Name = "comboBoxServerPort";
            // 
            // comboBoxServerIP
            // 
            resources.ApplyResources(this.comboBoxServerIP, "comboBoxServerIP");
            this.comboBoxServerIP.BackColor = System.Drawing.SystemColors.Window;
            this.comboBoxServerIP.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(122)))), ((int)(((byte)(122)))), ((int)(((byte)(122)))));
            this.comboBoxServerIP.ButtonColor = System.Drawing.SystemColors.Window;
            this.comboBoxServerIP.FormattingEnabled = true;
            this.comboBoxServerIP.Name = "comboBoxServerIP";
            // 
            // LoginForm
            // 
            this.AcceptButton = this.AuthButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.userIconImg);
            this.Controls.Add(this.serverPortIconImg);
            this.Controls.Add(this.serverIPIconImg);
            this.Controls.Add(this.passwordIconImg);
            this.Controls.Add(this.loadingCircle1);
            this.Controls.Add(this.comboBoxServerPort);
            this.Controls.Add(this.comboBoxServerIP);
            this.Controls.Add(this.checkBoxOfflineMode);
            this.Controls.Add(this.lblFixedServerPort);
            this.Controls.Add(this.lblFixedServerIP);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.topBannerImg);
            this.Controls.Add(this.AuthButton);
            this.Controls.Add(this.lblFixedPassword);
            this.Controls.Add(this.lblFixedUser);
            this.Controls.Add(this.textBoxPassword);
            this.Controls.Add(this.textBoxUser);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "LoginForm";
            this.Load += new System.EventHandler(this.Form2_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.topBannerImg)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.userIconImg)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.serverPortIconImg)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.serverIPIconImg)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.passwordIconImg)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxUser;
        private System.Windows.Forms.TextBox textBoxPassword;
        private System.Windows.Forms.Label lblFixedUser;
        private System.Windows.Forms.Label lblFixedPassword;
        private System.Windows.Forms.Button AuthButton;
        private ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox topBannerImg;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripVersionText;
        private System.Windows.Forms.Label lblFixedServerPort;
        private System.Windows.Forms.Label lblFixedServerIP;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusBarText;
        private System.Windows.Forms.CheckBox checkBoxOfflineMode;
        private CustomFlatComboBox comboBoxServerIP;
        private CustomFlatComboBox comboBoxServerPort;
        private MRG.Controls.UI.LoadingCircle loadingCircle1;
        private ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox userIconImg;
        private ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox serverPortIconImg;
        private ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox serverIPIconImg;
        private ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox passwordIconImg;
        private System.Windows.Forms.ToolStripStatusLabel aboutLabel;
    }
}