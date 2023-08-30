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
            {
                components.Dispose();
            }
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
            this.storageType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.storageSize = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.storageModel = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.storageSerialNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            resources.ApplyResources(this.dataGridView1, "dataGridView1");
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dataGridView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView1.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.storageType,
            this.storageSize,
            this.storageModel,
            this.storageSerialNumber});
            this.dataGridView1.EnableHeadersVisualStyles = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            // 
            // storageType
            // 
            resources.ApplyResources(this.storageType, "storageType");
            this.storageType.Name = "storageType";
            this.storageType.ReadOnly = true;
            // 
            // storageSize
            // 
            resources.ApplyResources(this.storageSize, "storageSize");
            this.storageSize.Name = "storageSize";
            this.storageSize.ReadOnly = true;
            // 
            // storageModel
            // 
            resources.ApplyResources(this.storageModel, "storageModel");
            this.storageModel.Name = "storageModel";
            this.storageModel.ReadOnly = true;
            // 
            // storageSerialNumber
            // 
            resources.ApplyResources(this.storageSerialNumber, "storageSerialNumber");
            this.storageSerialNumber.Name = "storageSerialNumber";
            this.storageSerialNumber.ReadOnly = true;
            // 
            // StorageDetailForm
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.dataGridView1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "StorageDetailForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn storageType;
        private System.Windows.Forms.DataGridViewTextBoxColumn storageSize;
        private System.Windows.Forms.DataGridViewTextBoxColumn storageModel;
        private System.Windows.Forms.DataGridViewTextBoxColumn storageSerialNumber;
    }
}
