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
            this.groupBoxExistingHardware = new System.Windows.Forms.GroupBox();
            this.treeViewExistingHardware = new System.Windows.Forms.TreeView();
            this.groupBoxNewHardware = new System.Windows.Forms.GroupBox();
            this.treeViewNewHardware = new System.Windows.Forms.TreeView();
            this.groupBoxExistingHardware.SuspendLayout();
            this.groupBoxNewHardware.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxExistingHardware
            // 
            resources.ApplyResources(this.groupBoxExistingHardware, "groupBoxExistingHardware");
            this.groupBoxExistingHardware.Controls.Add(this.treeViewExistingHardware);
            this.groupBoxExistingHardware.ForeColor = System.Drawing.SystemColors.ControlText;
            this.groupBoxExistingHardware.Name = "groupBoxExistingHardware";
            this.groupBoxExistingHardware.TabStop = false;
            // 
            // treeViewExistingHardware
            // 
            resources.ApplyResources(this.treeViewExistingHardware, "treeViewExistingHardware");
            this.treeViewExistingHardware.Name = "treeViewExistingHardware";
            // 
            // groupBoxNewHardware
            // 
            resources.ApplyResources(this.groupBoxNewHardware, "groupBoxNewHardware");
            this.groupBoxNewHardware.Controls.Add(this.treeViewNewHardware);
            this.groupBoxNewHardware.ForeColor = System.Drawing.SystemColors.ControlText;
            this.groupBoxNewHardware.Name = "groupBoxNewHardware";
            this.groupBoxNewHardware.TabStop = false;
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
            this.Controls.Add(this.groupBoxNewHardware);
            this.Controls.Add(this.groupBoxExistingHardware);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "HardwareChangeDetailForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.groupBoxExistingHardware.ResumeLayout(false);
            this.groupBoxNewHardware.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxExistingHardware;
        private System.Windows.Forms.GroupBox groupBoxNewHardware;
        private System.Windows.Forms.TreeView treeViewExistingHardware;
        private System.Windows.Forms.TreeView treeViewNewHardware;
    }
}