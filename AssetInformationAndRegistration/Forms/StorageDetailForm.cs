using AssetInformationAndRegistration.Interfaces;
using AssetInformationAndRegistration.Misc;
using ConstantsDLL;
using ConstantsDLL.Properties;
using LogGeneratorDLL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace AssetInformationAndRegistration.Forms
{
    /// <summary>
    /// Class for Storage Form
    /// </summary>
    internal partial class StorageDetailForm : Form, ITheming
    {
        private readonly LogGenerator log;
        /// <summary>
        /// Storage form constructor
        /// </summary>
        public StorageDetailForm(LogGenerator log)
        {
            InitializeComponent();
            FormClosing += StorageDetailForm_Closing;
            this.log = log;
        }

        /// <summary>
        /// Treats collected Storage data
        /// </summary>
        /// <param name="str">Storage detail matrix</param>
        public void TreatData(List<List<string>> str)
        {
            double totalSize = 0;
            double individualSize;
            string individualSizeStr, totalSizeStr;
            dataGridView1.Rows.Clear();
            KeyDown += StorageDetailForm_KeyDown;

            //Converts storage raw byte count into a more readable value and adds to the DataGridView
            foreach (List<string> s in str)
            {
                individualSizeStr = MiscMethods.FriendlySizeDecimal(Convert.ToInt64(s[2]), false);
                individualSize = Convert.ToInt64(individualSizeStr.Substring(0, individualSizeStr.Length - 3));
                totalSize += Convert.ToInt64(s[2]);
                s[2] = individualSizeStr;

                if (s[1] == (Convert.ToInt32(HardwareInfoDLL.HardwareInfo.StorageTypes.SSD)).ToString())
                    s[1] = HardwareInfoDLL.HardwareInfo.StorageTypes.SSD.ToString();
                else
                    s[1] = HardwareInfoDLL.HardwareInfo.StorageTypes.HDD.ToString();

                if (s[3] == (Convert.ToInt32(HardwareInfoDLL.HardwareInfo.StorageConnectionTypes.SATA)).ToString())
                    s[3] = StringsAndConstants.LIST_STORAGE_CONNECTION_TYPES[Convert.ToInt32(HardwareInfoDLL.HardwareInfo.StorageConnectionTypes.SATA)];
                else if (s[3] == (Convert.ToInt32(HardwareInfoDLL.HardwareInfo.StorageConnectionTypes.PCI_E).ToString()))
                    s[3] = StringsAndConstants.LIST_STORAGE_CONNECTION_TYPES[Convert.ToInt32(HardwareInfoDLL.HardwareInfo.StorageConnectionTypes.PCI_E)];
                else
                    s[3] = StringsAndConstants.LIST_STORAGE_CONNECTION_TYPES[Convert.ToInt32(HardwareInfoDLL.HardwareInfo.StorageConnectionTypes.IDE)];

                if (s[6] == GenericResources.OK_CODE)
                    s[6] = GenericResources.OK_NAME;
                else if (s[6] == GenericResources.PRED_FAIL_CODE)
                    s[6] = GenericResources.PRED_FAIL_NAME;
                else
                    s[6] = GenericResources.NOT_AVAILABLE_NAME;

                _ = dataGridView1.Rows.Add(s.ToArray());
            }

            //Shows the total storage size
            totalSizeStr = MiscMethods.FriendlySizeDecimal(Convert.ToInt64(totalSize), true);
            lblTotalSize.Text = totalSizeStr;

            //Sorts by ID column
            dataGridView1.Sort(dataGridView1.Columns["storageId"], ListSortDirection.Ascending);

            //Paints cell in red if SMART status equals a 'Pred Fail'
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                DataGridViewCell cell = row.Cells[6];
                if (cell.Value != null && cell.Value.Equals(GenericResources.PRED_FAIL_NAME))
                {
                    cell.Style.BackColor = Color.Red;
                    cell.Style.ForeColor = Color.White;
                }
            }
        }

        public void LightThemeSpecificControls()
        {
            iconImgStorageSize.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.ICON_DISK_SIZE_LIGHT_PATH));
        }

        public void DarkThemeSpecificControls()
        {
            iconImgStorageSize.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GenericResources.ICON_DISK_SIZE_DARK_PATH));
        }

        /// <summary>
        /// Closes the form when Escape is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StorageDetailForm_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Close();
            }
        }

        /// <summary> 
        /// Handles the closing of the current form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StorageDetailForm_Closing(object sender, FormClosingEventArgs e)
        {
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_CLOSING_STORAGE_FORM, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
        }
    }
}