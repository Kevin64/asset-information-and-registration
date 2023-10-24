using AssetInformationAndRegistration.Interfaces;
using AssetInformationAndRegistration.Misc;
using ConstantsDLL;
using ConstantsDLL.Properties;
using Dark.Net;
using HardwareInfoDLL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace AssetInformationAndRegistration.Forms
{
    /// <summary>
    /// Class for Storage Form
    /// </summary>
    internal partial class StorageDetailForm : Form, ITheming
    {
        private static double totalSize = 0;

        /// <summary>
        /// Storage form constructor
        /// </summary>
        /// <param name="str">Storage detail matrix</param>
        /// <param name="definitions">Definition object</param>
        /// <param name="isSystemDarkModeEnabled">Theme mode</param>
        public StorageDetailForm(List<List<string>> str, Program.Definitions definitions, bool isSystemDarkModeEnabled)
        {
            double individualSize;
            string individualSizeStr, totalSizeStr;
            InitializeComponent();
            KeyDown += StorageDetailForm_KeyDown;

            //Converts storage raw byte count into a more readable value and adds to the DataGridView
            foreach (List<string> s in str)
            {
                if (Convert.ToDouble(s[2].TrimEnd('K', 'M', 'G', 'T', 'B')) > 1000)
                {
                    individualSize = Convert.ToInt64(s[2]);
                    if (individualSize / 1000 / 1000 / 1000 >= 1000)
                        individualSizeStr = Math.Round(individualSize / 1000 / 1000 / 1000 / 1000, 0) + " " + Resources.TB;
                    else if (individualSize / 1000 / 1000 / 1000 < 1000 && individualSize / 1000 / 1000 / 1000 >= 1)
                        individualSizeStr = Math.Round(individualSize / 1000 / 1000 / 1000, 0) + " " + Resources.GB;
                    else
                        individualSizeStr = Math.Round(individualSize / 1000 / 1000, 0) + " " + Resources.MB;
                    s[2] = individualSizeStr;
                    totalSize += individualSize;
                }
                _ = dataGridView1.Rows.Add(s.ToArray());
            }
            //Shows the total storage size
            if (totalSize / 1000 / 1000 / 1000 >= 1000)
                totalSizeStr = Math.Round(totalSize / 1000 / 1000 / 1000 / 1000, 2) + " " + Resources.TB;
            else if (totalSize / 1000 / 1000 / 1000 < 1000 && totalSize / 1000 / 1000 / 1000 >= 1)
                totalSizeStr = Math.Round(totalSize / 1000 / 1000 / 1000, 2) + " " + Resources.GB;
            else
                totalSizeStr = Math.Round(totalSize / 1000 / 1000, 2) + " " + Resources.MB;
            lblTotalSize.Text = totalSizeStr;

            //Sorts by ID column
            dataGridView1.Sort(dataGridView1.Columns["storageId"], ListSortDirection.Ascending);

            //Define theming according to ini file provided info
            (int themeFileSet, bool _) = MiscMethods.GetFileThemeMode(definitions, isSystemDarkModeEnabled);
            switch (themeFileSet)
            {
                case 0:
                    MiscMethods.LightThemeAllControls(this);
                    LightThemeSpecificControls();
                    break;
                case 1:
                    MiscMethods.DarkThemeAllControls(this);
                    DarkThemeSpecificControls();
                    break;
            }

            //Paints cell in red if SMART status equals a 'Pred Fail'
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                DataGridViewCell cell = row.Cells[6];
                if (cell.Value != null && cell.Value.Equals(Resources.PRED_FAIL))
                {
                    cell.Style.BackColor = Color.Red;
                    cell.Style.ForeColor = Color.White;
                }
            }
        }

        public void LightThemeSpecificControls()
        {
            iconImgStorageSize.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Resources.ICON_DISK_SIZE_LIGHT_PATH));
        }

        public void DarkThemeSpecificControls()
        {
            iconImgStorageSize.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Resources.ICON_DISK_SIZE_DARK_PATH));
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
    }
}