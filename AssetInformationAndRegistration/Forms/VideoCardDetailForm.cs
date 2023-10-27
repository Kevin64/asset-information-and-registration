using AssetInformationAndRegistration.Interfaces;
using ConstantsDLL.Properties;
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
        /// <summary>
        /// Video card form constructor
        /// </summary>
        public VideoCardDetailForm()
        {
            InitializeComponent();
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
                individualRam = Convert.ToInt64(s[2]);
                if (individualRam / 1024 / 1024 / 1024 >= 1024)
                    individualRamStr = Math.Round(individualRam / 1024 / 1024 / 1024 / 1024, 0) + " " + Resources.TB;
                else if (individualRam / 1024 / 1024 / 1024 < 1024 && individualRam / 1024 / 1024 / 1024 >= 1)
                    individualRamStr = Math.Round(individualRam / 1024 / 1024 / 1024, 0) + " " + Resources.GB;
                else
                    individualRamStr = Math.Round(individualRam / 1024 / 1024, 0) + " " + Resources.MB;
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
    }
}