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
    /// Class for Processor Form
    /// </summary>
    internal partial class ProcessorDetailForm : Form, ITheming
    {
        /// <summary>
        /// Processor form constructor
        /// </summary>
        /// <param name="str">Processor detail matrix</param>
        /// <param name="definitions">Definition object</param>
        /// <param name="isSystemDarkModeEnabled">Theme mode</param>
        public ProcessorDetailForm(List<List<string>> str, Program.Definitions definitions, bool isSystemDarkModeEnabled)
        {
            double individualCache;
            string individualCacheStr;
            InitializeComponent();
            KeyDown += ProcessorDetailForm_KeyDown;

            //Converts storage raw byte count into a more readable value and adds to the DataGridView
            foreach (List<string> s in str)
            {
                if (Convert.ToDouble(s[5].TrimEnd('K', 'M', 'G', 'T', 'B')) > 1024)
                {
                    individualCache = Convert.ToInt64(s[5]);
                    if (individualCache / 1024 / 1024 / 1024 >= 1024)
                        individualCacheStr = Math.Round(individualCache / 1024 / 1024 / 1024 / 1024, 0) + " " + Resources.TB;
                    else if (individualCache / 1024 / 1024 / 1024 < 1024 && individualCache / 1024 / 1024 / 1024 >= 1)
                        individualCacheStr = Math.Round(individualCache / 1024 / 1024 / 1024, 0) + " " + Resources.GB;
                    else
                        individualCacheStr = Math.Round(individualCache / 1024 / 1024, 0) + " " + Resources.MB;
                    s[5] = individualCacheStr;
                }
                _ = dataGridView1.Rows.Add(s.ToArray());
            }

            //Sorts by ID column
            dataGridView1.Sort(dataGridView1.Columns["processorId"], ListSortDirection.Ascending);

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
        private void ProcessorDetailForm_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Close();
            }
        }
    }
}