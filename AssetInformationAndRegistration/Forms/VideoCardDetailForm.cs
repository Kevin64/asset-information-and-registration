using AssetInformationAndRegistration.Interfaces;
using AssetInformationAndRegistration.Misc;
using ConstantsDLL.Properties;
using LogGeneratorDLL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace AssetInformationAndRegistration.Forms
{
    /// <summary>
    /// Class for Video card Form
    /// </summary>
    internal partial class VideoCardDetailForm : Form, ITheming
    {
        private readonly LogGenerator log;
        /// <summary>
        /// Video card form constructor
        /// </summary>
        public VideoCardDetailForm(LogGenerator log)
        {
            InitializeComponent();
            FormClosing += VideoCardDetailForm_Closing;
            this.log = log;
        }

        /// <summary>
        /// Treats collected Video Card data
        /// </summary>
        /// <param name="str">Video card detail matrix</param>
        public void TreatData(List<List<string>> str)
        {
            double individualRam;
            string individualRamStr;
            dataGridView1.Rows.Clear();
            KeyDown += VideoCardDetailForm_KeyDown;

            //Converts storage raw byte count into a more readable value and adds to the DataGridView
            foreach (List<string> s in str)
            {
                individualRamStr = MiscMethods.FriendlySizeBinary(Convert.ToInt64(s[2]), false);
                individualRam = Convert.ToInt64(individualRamStr.Substring(0, individualRamStr.Length - 3));
                s[2] = individualRamStr;

                _ = dataGridView1.Rows.Add(s.ToArray());
            }

            //Sorts by ID columnn
            dataGridView1.Sort(dataGridView1.Columns["videoCardId"], ListSortDirection.Ascending);
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
        private void VideoCardDetailForm_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
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
        private void VideoCardDetailForm_Closing(object sender, FormClosingEventArgs e)
        {
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_CLOSING_VIDEO_CARD_FORM, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
        }
    }
}