using AssetInformationAndRegistration.Misc;

namespace AssetInformationAndRegistration.Forms
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
                components.Dispose();
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
            this.textBoxUsername = new System.Windows.Forms.TextBox();
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.lblFixedUsername = new System.Windows.Forms.Label();
            this.lblFixedPassword = new System.Windows.Forms.Label();
            this.authButton = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.aboutLabelButton = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusBarText = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripVersionText = new System.Windows.Forms.ToolStripStatusLabel();
            this.imgTopBanner = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.lblFixedServerPort = new System.Windows.Forms.Label();
            this.lblFixedServerIP = new System.Windows.Forms.Label();
            this.checkBoxOfflineMode = new System.Windows.Forms.CheckBox();
            this.loadingCircleAuthButton = new MRG.Controls.UI.LoadingCircle();
            this.iconImgUsername = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.iconImgServerPort = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.iconImgServerIP = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.iconImgPassword = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            this.comboBoxServerPort = new AssetInformationAndRegistration.Misc.CustomFlatComboBox();
            this.comboBoxServerIP = new AssetInformationAndRegistration.Misc.CustomFlatComboBox();
            this.textBoxInactiveLoginIntro = new System.Windows.Forms.TextBox();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgTopBanner)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgUsername)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgServerPort)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgServerIP)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgPassword)).BeginInit();
            this.SuspendLayout();
            // 
            // textBoxUsername
            // 
            this.textBoxUsername.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxUsername.ForeColor = System.Drawing.SystemColors.WindowText;
            resources.ApplyResources(this.textBoxUsername, "textBoxUsername");
            this.textBoxUsername.Name = "textBoxUsername";
            // 
            // textBoxPassword
            // 
            this.textBoxPassword.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxPassword.ForeColor = System.Drawing.SystemColors.WindowText;
            resources.ApplyResources(this.textBoxPassword, "textBoxPassword");
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.UseSystemPasswordChar = true;
            // 
            // lblFixedUsername
            // 
            resources.ApplyResources(this.lblFixedUsername, "lblFixedUsername");
            this.lblFixedUsername.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFixedUsername.Name = "lblFixedUsername";
            // 
            // lblFixedPassword
            // 
            resources.ApplyResources(this.lblFixedPassword, "lblFixedPassword");
            this.lblFixedPassword.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFixedPassword.Name = "lblFixedPassword";
            // 
            // authButton
            // 
            this.authButton.BackColor = System.Drawing.SystemColors.Control;
            resources.ApplyResources(this.authButton, "authButton");
            this.authButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.authButton.Name = "authButton";
            this.authButton.UseVisualStyleBackColor = true;
            this.authButton.Click += new System.EventHandler(this.AuthButton_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.BackColor = System.Drawing.SystemColors.Control;
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutLabelButton,
            this.toolStripStatusBarText,
            this.toolStripVersionText});
            resources.ApplyResources(this.statusStrip1, "statusStrip1");
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            // 
            // aboutLabelButton
            // 
            this.aboutLabelButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.aboutLabelButton.Name = "aboutLabelButton";
            resources.ApplyResources(this.aboutLabelButton, "aboutLabelButton");
            this.aboutLabelButton.Click += new System.EventHandler(this.AboutLabelButton_Click);
            // 
            // toolStripStatusBarText
            // 
            this.toolStripStatusBarText.BackColor = System.Drawing.SystemColors.Control;
            this.toolStripStatusBarText.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.toolStripStatusBarText.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
            this.toolStripStatusBarText.ForeColor = System.Drawing.SystemColors.ControlText;
            this.toolStripStatusBarText.Name = "toolStripStatusBarText";
            resources.ApplyResources(this.toolStripStatusBarText, "toolStripStatusBarText");
            this.toolStripStatusBarText.Spring = true;
            // 
            // toolStripVersionText
            // 
            this.toolStripVersionText.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.toolStripVersionText.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
            this.toolStripVersionText.ForeColor = System.Drawing.SystemColors.ControlText;
            this.toolStripVersionText.Name = "toolStripVersionText";
            resources.ApplyResources(this.toolStripVersionText, "toolStripVersionText");
            // 
            // imgTopBanner
            // 
            this.imgTopBanner.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.AssumeLinear;
            this.imgTopBanner.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            resources.ApplyResources(this.imgTopBanner, "imgTopBanner");
            this.imgTopBanner.Name = "imgTopBanner";
            this.imgTopBanner.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            this.imgTopBanner.TabStop = false;
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
            // loadingCircleAuthButton
            // 
            this.loadingCircleAuthButton.Active = false;
            this.loadingCircleAuthButton.BackColor = System.Drawing.Color.Transparent;
            this.loadingCircleAuthButton.Color = System.Drawing.Color.LightSlateGray;
            this.loadingCircleAuthButton.InnerCircleRadius = 5;
            resources.ApplyResources(this.loadingCircleAuthButton, "loadingCircleAuthButton");
            this.loadingCircleAuthButton.Name = "loadingCircleAuthButton";
            this.loadingCircleAuthButton.NumberSpoke = 12;
            this.loadingCircleAuthButton.OuterCircleRadius = 11;
            this.loadingCircleAuthButton.RotationSpeed = 1;
            this.loadingCircleAuthButton.SpokeThickness = 2;
            this.loadingCircleAuthButton.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            // 
            // iconImgUsername
            // 
            this.iconImgUsername.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.iconImgUsername.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            resources.ApplyResources(this.iconImgUsername, "iconImgUsername");
            this.iconImgUsername.Name = "iconImgUsername";
            this.iconImgUsername.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.iconImgUsername.TabStop = false;
            // 
            // iconImgServerPort
            // 
            this.iconImgServerPort.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.iconImgServerPort.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            resources.ApplyResources(this.iconImgServerPort, "iconImgServerPort");
            this.iconImgServerPort.Name = "iconImgServerPort";
            this.iconImgServerPort.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.iconImgServerPort.TabStop = false;
            // 
            // iconImgServerIP
            // 
            this.iconImgServerIP.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.iconImgServerIP.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            resources.ApplyResources(this.iconImgServerIP, "iconImgServerIP");
            this.iconImgServerIP.Name = "iconImgServerIP";
            this.iconImgServerIP.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.iconImgServerIP.TabStop = false;
            // 
            // iconImgPassword
            // 
            this.iconImgPassword.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.iconImgPassword.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            resources.ApplyResources(this.iconImgPassword, "iconImgPassword");
            this.iconImgPassword.Name = "iconImgPassword";
            this.iconImgPassword.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.iconImgPassword.TabStop = false;
            // 
            // comboBoxServerPort
            // 
            this.comboBoxServerPort.BackColor = System.Drawing.SystemColors.Window;
            this.comboBoxServerPort.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(122)))), ((int)(((byte)(122)))), ((int)(((byte)(122)))));
            this.comboBoxServerPort.ButtonColor = System.Drawing.SystemColors.Window;
            resources.ApplyResources(this.comboBoxServerPort, "comboBoxServerPort");
            this.comboBoxServerPort.FormattingEnabled = true;
            this.comboBoxServerPort.Name = "comboBoxServerPort";
            // 
            // comboBoxServerIP
            // 
            this.comboBoxServerIP.BackColor = System.Drawing.SystemColors.Window;
            this.comboBoxServerIP.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(122)))), ((int)(((byte)(122)))), ((int)(((byte)(122)))));
            this.comboBoxServerIP.ButtonColor = System.Drawing.SystemColors.Window;
            resources.ApplyResources(this.comboBoxServerIP, "comboBoxServerIP");
            this.comboBoxServerIP.FormattingEnabled = true;
            this.comboBoxServerIP.Name = "comboBoxServerIP";
            // 
            // textBoxInactiveLoginIntro
            // 
            this.textBoxInactiveLoginIntro.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.textBoxInactiveLoginIntro, "textBoxInactiveLoginIntro");
            this.textBoxInactiveLoginIntro.Name = "textBoxInactiveLoginIntro";
            this.textBoxInactiveLoginIntro.ReadOnly = true;
            // 
            // LoginForm
            // 
            this.AcceptButton = this.authButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.textBoxInactiveLoginIntro);
            this.Controls.Add(this.iconImgUsername);
            this.Controls.Add(this.iconImgServerPort);
            this.Controls.Add(this.iconImgServerIP);
            this.Controls.Add(this.iconImgPassword);
            this.Controls.Add(this.loadingCircleAuthButton);
            this.Controls.Add(this.comboBoxServerPort);
            this.Controls.Add(this.comboBoxServerIP);
            this.Controls.Add(this.checkBoxOfflineMode);
            this.Controls.Add(this.lblFixedServerPort);
            this.Controls.Add(this.lblFixedServerIP);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.imgTopBanner);
            this.Controls.Add(this.authButton);
            this.Controls.Add(this.lblFixedPassword);
            this.Controls.Add(this.lblFixedUsername);
            this.Controls.Add(this.textBoxPassword);
            this.Controls.Add(this.textBoxUsername);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "LoginForm";
            this.Load += new System.EventHandler(this.LoginForm_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgTopBanner)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgUsername)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgServerPort)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgServerIP)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgPassword)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxUsername;
        private System.Windows.Forms.TextBox textBoxPassword;
        private System.Windows.Forms.Label lblFixedUsername;
        private System.Windows.Forms.Label lblFixedPassword;
        private System.Windows.Forms.Button authButton;
        private ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox imgTopBanner;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripVersionText;
        private System.Windows.Forms.Label lblFixedServerPort;
        private System.Windows.Forms.Label lblFixedServerIP;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusBarText;
        private System.Windows.Forms.CheckBox checkBoxOfflineMode;
        private CustomFlatComboBox comboBoxServerIP;
        private CustomFlatComboBox comboBoxServerPort;
        private MRG.Controls.UI.LoadingCircle loadingCircleAuthButton;
        private ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox iconImgUsername;
        private ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox iconImgServerPort;
        private ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox iconImgServerIP;
        private ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox iconImgPassword;
        private System.Windows.Forms.ToolStripStatusLabel aboutLabelButton;
        private System.Windows.Forms.TextBox textBoxInactiveLoginIntro;
    }
}