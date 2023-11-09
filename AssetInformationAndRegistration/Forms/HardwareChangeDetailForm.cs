using AssetInformationAndRegistration.Interfaces;
using ConstantsDLL.Properties;
using LogGeneratorDLL;
using RestApiDLL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Management.Instrumentation;
using System.Reflection;
using System.Windows.Forms;

namespace AssetInformationAndRegistration.Forms
{
    public partial class HardwareChangeDetailForm : Form, ITheming
    {
        private readonly LogGenerator log;
        private readonly List<string> attributes = new List<string>() { "processor", "ram", "storage", "videoCard", "cpu_id", "name", "frequency", "numberOfCores", "numberOfThreads", "cache", "amount", "manufacturer", "type", "serialNumber", "type", "connection", "model", "serialNumber", "size", "smartStatus", "storageId", "gpuId", "vRam" };

        public HardwareChangeDetailForm(LogGenerator log)
        {
            InitializeComponent();
            FormClosing += HardwareChangeForm_Closing;
            this.log = log;

        }

        public void PopulateTreeView<T>(TreeNode root, T item)
        {
            TreeNode childNode;
            try
            {
                foreach (PropertyInfo pi in item.GetType().GetProperties())
                {
                    object propValue = pi.GetValue(item, null);
                    var elems = propValue as IList;
                    childNode = root.Nodes[root.Nodes.Add(new TreeNode(pi.Name + " - " + pi.GetValue(item)))];
                    if (elems != null)
                    {
                        foreach (var i in elems)
                        {
                            
                            PopulateTreeView(childNode, i);
                        }
                    }
                    else
                    {
                        // This will not cut-off System.Collections because of the first check
                        if (pi.PropertyType.Assembly == item.GetType().Assembly)
                        {
                            PopulateTreeView(childNode, propValue);
                        }
                    }
                }
            }
            catch
            {

            }
            //foreach (PropertyInfo pi in item.GetType().GetProperties())
            //{
            //    var childNode = root.Nodes[root.Nodes.Add(new TreeNode(pi.Name))];
            //    childNode.Tag = pi;
            //    PopulateTreeView(childNode, pi.GetValue(item));
            //}
        }

        public void FillData(hardware existingHardware, hardware newHardware)
        {
            treeViewExistingHardware.Nodes.Clear();
            treeViewNewHardware.Nodes.Clear();

            _ = treeViewExistingHardware.Nodes.Add("PC");
            _ = treeViewNewHardware.Nodes.Add("PC");

            PopulateTreeView(treeViewExistingHardware.Nodes[0], existingHardware);
            PopulateTreeView(treeViewNewHardware.Nodes[0], newHardware);

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
