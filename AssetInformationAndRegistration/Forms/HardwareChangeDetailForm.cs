using AssetInformationAndRegistration.Interfaces;
using ConstantsDLL.Properties;
using LogGeneratorDLL;
using RestApiDLL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AssetInformationAndRegistration.Forms
{
    public partial class HardwareChangeDetailForm : Form, ITheming
    {
        private readonly LogGenerator log;

        public HardwareChangeDetailForm(LogGenerator log)
        {
            InitializeComponent();
            FormClosing += HardwareChangeForm_Closing;
            this.log = log;
        }

        public void Recursion<T>(TreeNode root, T hw)
        {
            foreach (PropertyInfo pi in hw.GetType().GetProperties())
            {
                var childNode = root.Nodes[root.Nodes.Add(new TreeNode(pi.Name))];
                childNode.Tag = pi;
                Recursion(childNode, pi.GetValue(hw));
            }
        }

        public void FillData(hardware existingHardware, hardware newHardware)
        {
            treeViewExistingHardware.Nodes.Clear();
            treeViewNewHardware.Nodes.Clear();

            treeViewExistingHardware.Nodes.Add("testOld");
            treeViewExistingHardware.Nodes.Add("testNew");

            //Recursion(treeViewExistingHardware.Nodes[0], existingHardware);
            //Recursion(treeViewNewHardware.Nodes[0], newHardware);

            treeViewExistingHardware.ExpandAll();
            treeViewNewHardware.ExpandAll();
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
        private void HardwareChangeForm_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
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
        private void HardwareChangeForm_Closing(object sender, FormClosingEventArgs e)
        {
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_CLOSING_HARDWARE_CHANGE_FORM, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_GUI));
        }
    }
}
