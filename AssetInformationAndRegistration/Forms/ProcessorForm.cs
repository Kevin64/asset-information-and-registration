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
    /// Class for Processor Form
    /// </summary>
    internal partial class ProcessorDetailForm : Form, ITheming
    {
        private readonly LogGenerator log;

        /// <summary>
        /// Processor form constructor
        /// </summary>
        public ProcessorDetailForm(LogGenerator log)
        {
            InitializeComponent();
            FormClosing += ProcessorDetailForm_Closing;
            this.log = log;
        }

        /// <summary>
        /// Treats collected Processor data
        /// </summary>
        /// <param name="str">Processor detail matrix</param>
        public void TreatData(List<List<string>> str)
        {
            double individualCache;
            string individualCacheStr;
            dataGridView1.Rows.Clear();
            KeyDown += ProcessorDetailForm_KeyDown;

            //Converts storage raw byte count into a more readable value and adds to the DataGridView
            foreach (List<string> s in str)
            {
                individualCacheStr = MiscMethods.FriendlySizeBinary(Convert.ToInt64(s[5]), false);
                if(!individualCacheStr.Contains("0 B"))
                    individualCache = Convert.ToInt64(individualCacheStr.Substring(0, individualCacheStr.Length - 3));
                s[5] = individualCacheStr;

                if (!s[2].Contains(UIStrings.UNKNOWN))
                    s[2] = s[2] + " " + GenericResources.MHZ;

                _ = dataGridView1.Rows.Add(s.ToArray());
            }

            //Sorts by ID column
            dataGridView1.Sort(dataGridView1.Columns["processorId"], ListSortDirection.Ascending);
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

        /// <summary> 
        /// Handles the closing of the current form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProcessorDetailForm_Closing(object sender, FormClosingEventArgs e)
        {
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_CLOSING_PROCESSOR_FORM, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
        }
    }
}