namespace AssetInformationAndRegistration
{
    partial class BusyForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BusyForm));
            this.lblFixedLoading = new System.Windows.Forms.Label();
            this.loadingCircleLoading = new MRG.Controls.UI.LoadingCircle();
            this.SuspendLayout();
            // 
            // lblFixedLoading
            // 
            resources.ApplyResources(this.lblFixedLoading, "lblFixedLoading");
            this.lblFixedLoading.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFixedLoading.Name = "lblFixedLoading";
            this.lblFixedLoading.UseWaitCursor = true;
            // 
            // loadingCircleLoading
            // 
            resources.ApplyResources(this.loadingCircleLoading, "loadingCircleLoading");
            this.loadingCircleLoading.Active = false;
            this.loadingCircleLoading.Color = System.Drawing.Color.DarkGray;
            this.loadingCircleLoading.InnerCircleRadius = 5;
            this.loadingCircleLoading.Name = "loadingCircleLoading";
            this.loadingCircleLoading.NumberSpoke = 12;
            this.loadingCircleLoading.OuterCircleRadius = 11;
            this.loadingCircleLoading.RotationSpeed = 100;
            this.loadingCircleLoading.SpokeThickness = 2;
            this.loadingCircleLoading.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            this.loadingCircleLoading.UseWaitCursor = true;
            // 
            // BusyForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ControlBox = false;
            this.Controls.Add(this.loadingCircleLoading);
            this.Controls.Add(this.lblFixedLoading);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BusyForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.UseWaitCursor = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblFixedLoading;
        private MRG.Controls.UI.LoadingCircle loadingCircleLoading;
    }
}