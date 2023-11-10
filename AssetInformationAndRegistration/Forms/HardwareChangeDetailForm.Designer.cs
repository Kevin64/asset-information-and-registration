namespace AssetInformationAndRegistration.Forms
{
    partial class HardwareChangeDetailForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HardwareChangeDetailForm));
            this.lblFixedExistingHardwareHashId = new System.Windows.Forms.Label();
            this.lblFixedNewHardwareHashId = new System.Windows.Forms.Label();
            this.textBoxExistingHardwareHashId = new System.Windows.Forms.TextBox();
            this.textBoxNewHardwareHashId = new System.Windows.Forms.TextBox();
            this.lblFixedExistingHardware = new System.Windows.Forms.Label();
            this.lblFixedNewHardware = new System.Windows.Forms.Label();
            this.treeViewExistingHardware = new AssetInformationAndRegistration.SyncTreeView();
            this.treeViewNewHardware = new AssetInformationAndRegistration.SyncTreeView();
            this.SuspendLayout();
            // 
            // lblFixedExistingHardwareHashId
            // 
            resources.ApplyResources(this.lblFixedExistingHardwareHashId, "lblFixedExistingHardwareHashId");
            this.lblFixedExistingHardwareHashId.Name = "lblFixedExistingHardwareHashId";
            // 
            // lblFixedNewHardwareHashId
            // 
            resources.ApplyResources(this.lblFixedNewHardwareHashId, "lblFixedNewHardwareHashId");
            this.lblFixedNewHardwareHashId.Name = "lblFixedNewHardwareHashId";
            // 
            // textBoxExistingHardwareHashId
            // 
            this.textBoxExistingHardwareHashId.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.textBoxExistingHardwareHashId, "textBoxExistingHardwareHashId");
            this.textBoxExistingHardwareHashId.Name = "textBoxExistingHardwareHashId";
            this.textBoxExistingHardwareHashId.ReadOnly = true;
            // 
            // textBoxNewHardwareHashId
            // 
            this.textBoxNewHardwareHashId.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.textBoxNewHardwareHashId, "textBoxNewHardwareHashId");
            this.textBoxNewHardwareHashId.Name = "textBoxNewHardwareHashId";
            this.textBoxNewHardwareHashId.ReadOnly = true;
            // 
            // lblFixedExistingHardware
            // 
            resources.ApplyResources(this.lblFixedExistingHardware, "lblFixedExistingHardware");
            this.lblFixedExistingHardware.Name = "lblFixedExistingHardware";
            // 
            // lblFixedNewHardware
            // 
            resources.ApplyResources(this.lblFixedNewHardware, "lblFixedNewHardware");
            this.lblFixedNewHardware.Name = "lblFixedNewHardware";
            // 
            // treeViewExistingHardware
            // 
            resources.ApplyResources(this.treeViewExistingHardware, "treeViewExistingHardware");
            this.treeViewExistingHardware.Name = "treeViewExistingHardware";
            // 
            // treeViewNewHardware
            // 
            resources.ApplyResources(this.treeViewNewHardware, "treeViewNewHardware");
            this.treeViewNewHardware.Name = "treeViewNewHardware";
            // 
            // HardwareChangeDetailForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.treeViewExistingHardware);
            this.Controls.Add(this.treeViewNewHardware);
            this.Controls.Add(this.lblFixedNewHardware);
            this.Controls.Add(this.lblFixedExistingHardware);
            this.Controls.Add(this.textBoxNewHardwareHashId);
            this.Controls.Add(this.textBoxExistingHardwareHashId);
            this.Controls.Add(this.lblFixedNewHardwareHashId);
            this.Controls.Add(this.lblFixedExistingHardwareHashId);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "HardwareChangeDetailForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lblFixedExistingHardwareHashId;
        private System.Windows.Forms.Label lblFixedNewHardwareHashId;
        private System.Windows.Forms.TextBox textBoxExistingHardwareHashId;
        private System.Windows.Forms.TextBox textBoxNewHardwareHashId;
        private System.Windows.Forms.Label lblFixedExistingHardware;
        private System.Windows.Forms.Label lblFixedNewHardware;
        private SyncTreeView treeViewNewHardware;
        private SyncTreeView treeViewExistingHardware;
    }
}