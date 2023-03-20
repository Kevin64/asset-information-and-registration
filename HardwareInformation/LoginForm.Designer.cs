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
            // lblFixedUser
            // 
            this.lblFixedUser.AutoSize = true;
            this.lblFixedUser.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFixedUser.Location = new System.Drawing.Point(39, 146);
            this.lblFixedUser.Name = "lblFixedUser";
            this.lblFixedUser.Size = new System.Drawing.Size(46, 13);
            this.lblFixedUser.TabIndex = 2;
            this.lblFixedUser.Text = "Usuário:";
            // 
            // lblFixedPassword
            // 
            this.lblFixedPassword.AutoSize = true;
            this.lblFixedPassword.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFixedPassword.Location = new System.Drawing.Point(39, 171);
            this.lblFixedPassword.Name = "lblFixedPassword";
            this.lblFixedPassword.Size = new System.Drawing.Size(41, 13);
            this.lblFixedPassword.TabIndex = 3;
            this.lblFixedPassword.Text = "Senha:";
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
            this.AuthButton.Click += new System.EventHandler(this.AuthButton_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.BackColor = System.Drawing.SystemColors.Control;
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutLabel,
            this.toolStripStatusBarText,
            this.toolStripVersionText});
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
            this.aboutLabel.Click += new System.EventHandler(this.AboutLabel_Click);
            // 
            // toolStripStatusBarText
            // 
            this.toolStripStatusBarText.BackColor = System.Drawing.SystemColors.Control;
            this.toolStripStatusBarText.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.toolStripStatusBarText.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
            this.toolStripStatusBarText.ForeColor = System.Drawing.SystemColors.ControlText;
            this.toolStripStatusBarText.Name = "toolStripStatusBarText";
            this.toolStripStatusBarText.Size = new System.Drawing.Size(185, 17);
            this.toolStripStatusBarText.Spring = true;
            // 
            // toolStripVersionText
            // 
            this.toolStripVersionText.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.toolStripVersionText.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
            this.toolStripVersionText.ForeColor = System.Drawing.SystemColors.ControlText;
            this.toolStripVersionText.Name = "toolStripVersionText";
            this.toolStripVersionText.Size = new System.Drawing.Size(4, 17);
            // 
            // topBannerImg
            // 
            this.topBannerImg.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.AssumeLinear;
            this.topBannerImg.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            this.topBannerImg.Location = new System.Drawing.Point(-5, -1);
            this.topBannerImg.Name = "topBannerImg";
            this.topBannerImg.Size = new System.Drawing.Size(252, 126);
            this.topBannerImg.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.topBannerImg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            this.topBannerImg.TabIndex = 5;
            this.topBannerImg.TabStop = false;
            // 
            // lblFixedServerPort
            // 
            this.lblFixedServerPort.AutoSize = true;
            this.lblFixedServerPort.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFixedServerPort.Location = new System.Drawing.Point(39, 224);
            this.lblFixedServerPort.Name = "lblFixedServerPort";
            this.lblFixedServerPort.Size = new System.Drawing.Size(35, 13);
            this.lblFixedServerPort.TabIndex = 10;
            this.lblFixedServerPort.Text = "Porta:";
            // 
            // lblFixedServerIP
            // 
            this.lblFixedServerIP.AutoSize = true;
            this.lblFixedServerIP.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFixedServerIP.Location = new System.Drawing.Point(39, 197);
            this.lblFixedServerIP.Name = "lblFixedServerIP";
            this.lblFixedServerIP.Size = new System.Drawing.Size(49, 13);
            this.lblFixedServerIP.TabIndex = 9;
            this.lblFixedServerIP.Text = "Servidor:";
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
            this.checkBoxOfflineMode.CheckedChanged += new System.EventHandler(this.CheckBox1_CheckedChanged);
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
            // userIconImg
            // 
            this.userIconImg.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.userIconImg.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            this.userIconImg.Location = new System.Drawing.Point(11, 139);
            this.userIconImg.Name = "userIconImg";
            this.userIconImg.Size = new System.Drawing.Size(24, 25);
            this.userIconImg.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.userIconImg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.userIconImg.TabIndex = 139;
            this.userIconImg.TabStop = false;
            // 
            // serverPortIconImg
            // 
            this.serverPortIconImg.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.serverPortIconImg.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            this.serverPortIconImg.Location = new System.Drawing.Point(11, 217);
            this.serverPortIconImg.Name = "serverPortIconImg";
            this.serverPortIconImg.Size = new System.Drawing.Size(24, 25);
            this.serverPortIconImg.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.serverPortIconImg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.serverPortIconImg.TabIndex = 138;
            this.serverPortIconImg.TabStop = false;
            // 
            // serverIPIconImg
            // 
            this.serverIPIconImg.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.serverIPIconImg.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            this.serverIPIconImg.Location = new System.Drawing.Point(11, 191);
            this.serverIPIconImg.Name = "serverIPIconImg";
            this.serverIPIconImg.Size = new System.Drawing.Size(24, 25);
            this.serverIPIconImg.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.serverIPIconImg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.serverIPIconImg.TabIndex = 137;
            this.serverIPIconImg.TabStop = false;
            // 
            // passwordIconImg
            // 
            this.passwordIconImg.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.passwordIconImg.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            this.passwordIconImg.Location = new System.Drawing.Point(11, 165);
            this.passwordIconImg.Name = "passwordIconImg";
            this.passwordIconImg.Size = new System.Drawing.Size(24, 25);
            this.passwordIconImg.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.passwordIconImg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.passwordIconImg.TabIndex = 136;
            this.passwordIconImg.TabStop = false;
            // 
            // comboBoxServerPort
            // 
            this.comboBoxServerPort.BackColor = System.Drawing.SystemColors.Window;
            this.comboBoxServerPort.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(122)))), ((int)(((byte)(122)))), ((int)(((byte)(122)))));
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
            this.comboBoxServerIP.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(122)))), ((int)(((byte)(122)))), ((int)(((byte)(122)))));
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
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "LoginForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Login";
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