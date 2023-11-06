using System.Windows.Forms;

namespace AssetInformationAndRegistration.Forms
{
    partial class RamDetailForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RamDetailForm));
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.ramSlot = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ramAmount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ramType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ramFrequency = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ramManufacturer = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ramSerialNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ramPartNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
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
            this.ramSlot,
            this.ramAmount,
            this.ramType,
            this.ramFrequency,
            this.ramManufacturer,
            this.ramSerialNumber,
            this.ramPartNumber});
            this.dataGridView1.EnableHeadersVisualStyles = false;
            resources.ApplyResources(this.dataGridView1, "dataGridView1");
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            // 
            // ramSlot
            // 
            this.ramSlot.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.ramSlot, "ramSlot");
            this.ramSlot.Name = "ramSlot";
            this.ramSlot.ReadOnly = true;
            // 
            // ramAmount
            // 
            this.ramAmount.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.ramAmount, "ramAmount");
            this.ramAmount.Name = "ramAmount";
            this.ramAmount.ReadOnly = true;
            // 
            // ramType
            // 
            this.ramType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.ramType, "ramType");
            this.ramType.Name = "ramType";
            this.ramType.ReadOnly = true;
            // 
            // ramFrequency
            // 
            this.ramFrequency.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.ramFrequency, "ramFrequency");
            this.ramFrequency.Name = "ramFrequency";
            this.ramFrequency.ReadOnly = true;
            // 
            // ramManufacturer
            // 
            this.ramManufacturer.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.ramManufacturer, "ramManufacturer");
            this.ramManufacturer.Name = "ramManufacturer";
            this.ramManufacturer.ReadOnly = true;
            // 
            // ramSerialNumber
            // 
            this.ramSerialNumber.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.ramSerialNumber, "ramSerialNumber");
            this.ramSerialNumber.Name = "ramSerialNumber";
            this.ramSerialNumber.ReadOnly = true;
            // 
            // ramPartNumber
            // 
            this.ramPartNumber.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.ramPartNumber, "ramPartNumber");
            this.ramPartNumber.Name = "ramPartNumber";
            this.ramPartNumber.ReadOnly = true;
            // 
            // RamDetailForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dataGridView1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RamDetailForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn ramSlot;
        private System.Windows.Forms.DataGridViewTextBoxColumn ramAmount;
        private System.Windows.Forms.DataGridViewTextBoxColumn ramType;
        private System.Windows.Forms.DataGridViewTextBoxColumn ramFrequency;
        private System.Windows.Forms.DataGridViewTextBoxColumn ramManufacturer;
        private System.Windows.Forms.DataGridViewTextBoxColumn ramSerialNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn ramPartNumber;
    }
}
