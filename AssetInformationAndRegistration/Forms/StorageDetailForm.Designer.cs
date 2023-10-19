using System.Windows.Forms;

namespace AssetInformationAndRegistration.Forms
{
    partial class StorageDetailForm
    {
        /// <summary>
        /// Variável de designer necessária.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpar os recursos que estão sendo usados.
        /// </summary>
        /// <param name="disposing">true se for necessário descartar os recursos gerenciados; caso contrário, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Código gerado pelo Windows Form Designer

        /// <summary>
        /// Método necessário para suporte ao Designer - não modifique 
        /// o conteúdo deste método com o editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StorageDetailForm));
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.storageId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.storageType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.storageSize = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.storageConnection = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.storageModel = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.storageSerialNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.storageSmart = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblFixedTotalSize = new System.Windows.Forms.Label();
            this.lblTotalSize = new System.Windows.Forms.Label();
            this.iconImgStorageSize = new ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgStorageSize)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dataGridView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView1.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
            this.dataGridView1.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.storageId,
            this.storageType,
            this.storageSize,
            this.storageConnection,
            this.storageModel,
            this.storageSerialNumber,
            this.storageSmart});
            this.dataGridView1.EnableHeadersVisualStyles = false;
            resources.ApplyResources(this.dataGridView1, "dataGridView1");
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            // 
            // storageId
            // 
            this.storageId.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.storageId.FillWeight = 103.0457F;
            resources.ApplyResources(this.storageId, "storageId");
            this.storageId.Name = "storageId";
            this.storageId.ReadOnly = true;
            // 
            // storageType
            // 
            this.storageType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.storageType.FillWeight = 99.49239F;
            resources.ApplyResources(this.storageType, "storageType");
            this.storageType.Name = "storageType";
            this.storageType.ReadOnly = true;
            // 
            // storageSize
            // 
            this.storageSize.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.storageSize.FillWeight = 99.49239F;
            resources.ApplyResources(this.storageSize, "storageSize");
            this.storageSize.Name = "storageSize";
            this.storageSize.ReadOnly = true;
            // 
            // storageConnection
            // 
            this.storageConnection.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.storageConnection.FillWeight = 99.49239F;
            resources.ApplyResources(this.storageConnection, "storageConnection");
            this.storageConnection.Name = "storageConnection";
            this.storageConnection.ReadOnly = true;
            // 
            // storageModel
            // 
            this.storageModel.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.storageModel.FillWeight = 99.49239F;
            resources.ApplyResources(this.storageModel, "storageModel");
            this.storageModel.Name = "storageModel";
            this.storageModel.ReadOnly = true;
            // 
            // storageSerialNumber
            // 
            this.storageSerialNumber.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.storageSerialNumber.FillWeight = 99.49239F;
            resources.ApplyResources(this.storageSerialNumber, "storageSerialNumber");
            this.storageSerialNumber.Name = "storageSerialNumber";
            this.storageSerialNumber.ReadOnly = true;
            // 
            // storageSmart
            // 
            this.storageSmart.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.storageSmart.FillWeight = 89.49239F;
            resources.ApplyResources(this.storageSmart, "storageSmart");
            this.storageSmart.Name = "storageSmart";
            this.storageSmart.ReadOnly = true;
            // 
            // lblFixedTotalSize
            // 
            resources.ApplyResources(this.lblFixedTotalSize, "lblFixedTotalSize");
            this.lblFixedTotalSize.Name = "lblFixedTotalSize";
            // 
            // lblTotalSize
            // 
            resources.ApplyResources(this.lblTotalSize, "lblTotalSize");
            this.lblTotalSize.Name = "lblTotalSize";
            // 
            // iconImgStorageSize
            // 
            this.iconImgStorageSize.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            resources.ApplyResources(this.iconImgStorageSize, "iconImgStorageSize");
            this.iconImgStorageSize.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            this.iconImgStorageSize.Name = "iconImgStorageSize";
            this.iconImgStorageSize.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.iconImgStorageSize.TabStop = false;
            // 
            // StorageDetailForm
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.iconImgStorageSize);
            this.Controls.Add(this.lblTotalSize);
            this.Controls.Add(this.lblFixedTotalSize);
            this.Controls.Add(this.dataGridView1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "StorageDetailForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImgStorageSize)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label lblFixedTotalSize;
        private System.Windows.Forms.Label lblTotalSize;
        private ConfigurableQualityPictureBoxDLL.ConfigurableQualityPictureBox iconImgStorageSize;
        private System.Windows.Forms.DataGridViewTextBoxColumn storageId;
        private System.Windows.Forms.DataGridViewTextBoxColumn storageType;
        private System.Windows.Forms.DataGridViewTextBoxColumn storageSize;
        private System.Windows.Forms.DataGridViewTextBoxColumn storageConnection;
        private System.Windows.Forms.DataGridViewTextBoxColumn storageModel;
        private System.Windows.Forms.DataGridViewTextBoxColumn storageSerialNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn storageSmart;
    }
}
