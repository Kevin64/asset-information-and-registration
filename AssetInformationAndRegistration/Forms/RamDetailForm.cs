using AssetInformationAndRegistration.Interfaces;
using AssetInformationAndRegistration.Misc;
using ConstantsDLL;
using ConstantsDLL.Properties;
using Dark.Net;
using HardwareInfoDLL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
        /// <param name="str">Ram detail matrix</param>
        /// <param name="definitions">Definition object</param>
        /// <param name="isSystemDarkModeEnabled">Theme mode</param>
        public RamDetailForm(List<List<string>> str, Program.Definitions definitions, bool isSystemDarkModeEnabled)
        {
            double individualRam;
            string individualRamStr;
            auxList = str;
            InitializeComponent();
            KeyDown += RamDetailForm_KeyDown;

            //Converts storage raw byte count into a more readable value and adds to the DataGridView
            foreach (List<string> s in auxList)
            {
                if (Convert.ToDouble(s[1].TrimEnd('K', 'M', 'G', 'T', 'B')) > 1024)
                {
                    individualRam = Convert.ToInt64(s[1]);
                    if (individualRam / 1024 / 1024 / 1024 >= 1024)
                        individualRamStr = Math.Round(individualRam / 1024 / 1024 / 1024 / 1024, 0) + " " + Resources.TB;
                    else if (individualRam / 1024 / 1024 / 1024 < 1024 && individualRam / 1024 / 1024 / 1024 >= 1)
                        individualRamStr = Math.Round(individualRam / 1024 / 1024 / 1024, 0) + " " + Resources.GB;
                    else
                        individualRamStr = Math.Round(individualRam / 1024 / 1024, 0) + " " + Resources.MB;
                    s[1] = individualRamStr;

                    if (s[2] == Resources.DDR4_SMBIOS)
                        s[2] = Resources.DDR4;
                    else if (s[2] == Resources.DDR3_SMBIOS)
                        s[2] = Resources.DDR3;
                    else
                        s[2] = Resources.DDR2;

                    s[3] = s[3] + " " + Resources.MHZ;
                }

                _ = dataGridView1.Rows.Add(s.ToArray());
            }

            //Sorts by slot column
            dataGridView1.Sort(dataGridView1.Columns["ramSlot"], ListSortDirection.Ascending);

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