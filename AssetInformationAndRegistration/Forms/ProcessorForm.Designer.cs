using System.Windows.Forms;

namespace AssetInformationAndRegistration.Forms
{
    partial class ProcessorDetailForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProcessorDetailForm));
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.processorId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.processorName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.processorFrequency = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.processorNumberOfCores = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.processorNumberOfThreads = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.processorCache = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
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
            this.processorId,
            this.processorName,
            this.processorFrequency,
            this.processorNumberOfCores,
            this.processorNumberOfThreads,
            this.processorCache});
            this.dataGridView1.EnableHeadersVisualStyles = false;
            resources.ApplyResources(this.dataGridView1, "dataGridView1");
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            // 
            // processorId
            // 
            this.processorId.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.processorId.FillWeight = 18.99606F;
            resources.ApplyResources(this.processorId, "processorId");
            this.processorId.Name = "processorId";
            this.processorId.ReadOnly = true;
            // 
            // processorName
            // 
            this.processorName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.processorName, "processorName");
            this.processorName.Name = "processorName";
            this.processorName.ReadOnly = true;
            // 
            // processorFrequency
            // 
            this.processorFrequency.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.processorFrequency, "processorFrequency");
            this.processorFrequency.Name = "processorFrequency";
            this.processorFrequency.ReadOnly = true;
            // 
            // processorNumberOfCores
            // 
            this.processorNumberOfCores.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.processorNumberOfCores, "processorNumberOfCores");
            this.processorNumberOfCores.Name = "processorNumberOfCores";
            this.processorNumberOfCores.ReadOnly = true;
            // 
            // processorNumberOfThreads
            // 
            this.processorNumberOfThreads.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.processorNumberOfThreads, "processorNumberOfThreads");
            this.processorNumberOfThreads.Name = "processorNumberOfThreads";
            this.processorNumberOfThreads.ReadOnly = true;
            // 
            // processorCache
            // 
            this.processorCache.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.processorCache, "processorCache");
            this.processorCache.Name = "processorCache";
            this.processorCache.ReadOnly = true;
            // 
            // ProcessorDetailForm
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.dataGridView1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProcessorDetailForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn processorId;
        private System.Windows.Forms.DataGridViewTextBoxColumn processorName;
        private System.Windows.Forms.DataGridViewTextBoxColumn processorFrequency;
        private System.Windows.Forms.DataGridViewTextBoxColumn processorNumberOfCores;
        private System.Windows.Forms.DataGridViewTextBoxColumn processorNumberOfThreads;
        private System.Windows.Forms.DataGridViewTextBoxColumn processorCache;
    }
}
