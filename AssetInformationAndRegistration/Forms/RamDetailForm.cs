using AssetInformationAndRegistration.Interfaces;
using AssetInformationAndRegistration.Misc;
using ConstantsDLL;
using ConstantsDLL.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace AssetInformationAndRegistration.Forms
{
    /// <summary>
    /// Class for Ram Form
    /// </summary>
    internal partial class RamDetailForm : Form, ITheming
    {
        private static List<List<string>> auxList;

        /// <summary>
        /// Ram form constructor
        /// </summary>
        public RamDetailForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Treats collected Ram data
        /// </summary>
        /// <param name="str">Ram detail matrix</param>
        /// <param name="definitions">Definition object</param>
        /// <param name="isSystemDarkModeEnabled">Theme mode</param>
        public void TreatData(List<List<string>> str, bool isSystemDarkModeEnabled)
        {
            double individualRam;
            string individualRamStr;
            auxList = str;
            dataGridView1.Rows.Clear();
            KeyDown += RamDetailForm_KeyDown;

            //Converts storage raw byte count into a more readable value and adds to the DataGridView
            foreach (List<string> s in auxList)
            {
                if (!s.Contains(Strings.FREE))
                {
                    individualRam = Convert.ToInt64(s[1]);
                    if (individualRam / 1024 / 1024 / 1024 >= 1024)
                        individualRamStr = Math.Round(individualRam / 1024 / 1024 / 1024 / 1024, 0) + " " + Resources.TB;
                    else if (individualRam / 1024 / 1024 / 1024 < 1024 && individualRam / 1024 / 1024 / 1024 >= 1)
                        individualRamStr = Math.Round(individualRam / 1024 / 1024 / 1024, 0) + " " + Resources.GB;
                    else
                        individualRamStr = Math.Round(individualRam / 1024 / 1024, 0) + " " + Resources.MB;
                    s[1] = individualRamStr;

                    if (!s[3].Contains(Strings.UNKNOWN))
                        s[3] = s[3] + " " + Resources.MHZ;
                }

                if (s[2] == "0")
                    s[2] = Strings.UNKNOWN;
                else if (s[2] == Resources.DDR4_SMBIOS)
                    s[2] = Resources.DDR4;
                else if (s[2] == Resources.DDR3_SMBIOS)
                    s[2] = Resources.DDR3;
                else if (s[2] == Strings.FREE)
                {
                    ;
                }
                else
                    s[2] = Resources.DDR2;

                _ = dataGridView1.Rows.Add(s.ToArray());
            }

            //Sorts by slot column
            dataGridView1.Sort(dataGridView1.Columns["ramSlot"], ListSortDirection.Ascending);

            //Paints cell in gray if slot is free
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                foreach(DataGridViewCell cell in row.Cells)
                {
                    if (cell.Value != null && cell.Value.Equals(Strings.FREE))
                    {
                        if (isSystemDarkModeEnabled == false)
                            cell.Style.ForeColor = StringsAndConstants.LIGHT_INACTIVE_CAPTION_COLOR;
                        else
                            cell.Style.ForeColor = StringsAndConstants.DARK_INACTIVE_CAPTION_COLOR;
                    }
                }
            }
        }

        public void LightThemeSpecificControls()
        {

        }

        public void DarkThemeSpecificControls()
        {

        }

        /// <summary>
        /// Closes the form when Escape is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RamDetailForm_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Close();
            }
        }
    }
}