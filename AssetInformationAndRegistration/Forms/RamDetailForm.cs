using AssetInformationAndRegistration.Interfaces;
using AssetInformationAndRegistration.Misc;
using ConstantsDLL;
using ConstantsDLL.Properties;
using HardwareInfoDLL;
using LogGeneratorDLL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace AssetInformationAndRegistration.Forms
{
    /// <summary>
    /// Class for Ram Form
    /// </summary>
    internal partial class RamDetailForm : Form, ITheming
    {
        private static List<List<string>> auxList;
        private readonly LogGenerator log;

        /// <summary>
        /// Ram form constructor
        /// </summary>
        public RamDetailForm(LogGenerator log)
        {
            InitializeComponent();
            FormClosing += RamDetailForm_Closing;
            this.log = log;
        }

        /// <summary>
        /// Treats collected Ram data
        /// </summary>
        /// <param name="str">Ram detail matrix</param>
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
                if (!s.Contains(GenericResources.FREE_CODE))
                {
                    individualRamStr = MiscMethods.FriendlySizeBinary(Convert.ToInt64(s[1]), false);
                    individualRam = Convert.ToInt64(individualRamStr.Substring(0, individualRamStr.Length - 3));
                    s[1] = individualRamStr;

                    if (!s[3].Contains(GenericResources.NOT_AVAILABLE_CODE))
                        s[3] = s[3] + " " + GenericResources.MHZ;
                }

                if (s[2] != GenericResources.FREE_CODE)
                    s[2] = Enum.GetName(typeof(HardwareInfo.RamTypes), Convert.ToInt32(s[2]));

                for (int i = 0; i < s.Count; i++)
                {
                    if (s[i] == GenericResources.FREE_CODE)
                        s[i] = UIStrings.FREE;
                    if (s[i] == GenericResources.NOT_AVAILABLE_CODE)
                        s[i] = GenericResources.NOT_AVAILABLE_NAME;
                }

                _ = dataGridView1.Rows.Add(s.ToArray());
            }

            //Sorts by slot column
            dataGridView1.Sort(dataGridView1.Columns["ramSlot"], ListSortDirection.Ascending);

            //Paints cell in gray if slot is free
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (cell.Value != null && cell.Value.Equals(UIStrings.FREE))
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

        /// <summary> 
        /// Handles the closing of the current form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RamDetailForm_Closing(object sender, FormClosingEventArgs e)
        {
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_CLOSING_RAM_FORM, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
        }
    }
}