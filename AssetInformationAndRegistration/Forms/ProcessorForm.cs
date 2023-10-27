using AssetInformationAndRegistration.Interfaces;
using ConstantsDLL.Properties;
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
        /// <summary>
        /// Processor form constructor
        /// </summary>
        public ProcessorDetailForm()
        {
            InitializeComponent();
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
                individualCache = Convert.ToInt64(s[5]);
                if (individualCache / 1024 / 1024 / 1024 >= 1024)
                    individualCacheStr = Math.Round(individualCache / 1024 / 1024 / 1024 / 1024, 0) + " " + Resources.TB;
                else if (individualCache / 1024 / 1024 / 1024 < 1024 && individualCache / 1024 / 1024 / 1024 >= 1)
                    individualCacheStr = Math.Round(individualCache / 1024 / 1024 / 1024, 0) + " " + Resources.GB;
                else
                    individualCacheStr = Math.Round(individualCache / 1024 / 1024, 0) + " " + Resources.MB;
                s[5] = individualCacheStr;

                if (!s[2].Contains(Strings.UNKNOWN))
                    s[2] = s[2] + " " + Resources.MHZ;

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
    }
}