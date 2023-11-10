using AssetInformationAndRegistration.Interfaces;
using ConstantsDLL.Properties;
using JsonDiffPatchDotNet;
using LogGeneratorDLL;
using Newtonsoft.Json;
using RestApiDLL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
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

            treeViewExistingHardware.AddLinkedTreeView(treeViewNewHardware);
            treeViewNewHardware.AddLinkedTreeView(treeViewExistingHardware);
        }

        public void PopulateTreeView<T>(TreeNode root, T item)
        {
            TreeNode childNode, childNodeAux;
            try
            {
                foreach (PropertyInfo pi in item.GetType().GetProperties())
                {
                    int index = 0;
                    object propValue = pi.GetValue(item, null);

                    if (propValue is IList elems)
                    {
                        childNode = root.Nodes[root.Nodes.Add(new TreeNode(pi.Name))];
                        foreach (object i in elems)
                        {
                            childNodeAux = childNode.Nodes[childNode.Nodes.Add(new TreeNode(index.ToString()))];
                            PopulateTreeView(childNodeAux, i);
                            index++;
                        }
                    }
                    else
                    {
                        childNode = root.Nodes[root.Nodes.Add(new TreeNode(pi.Name + " - " + pi.GetValue(item)))];
                        if (pi.PropertyType.Assembly == item.GetType().Assembly)
                            PopulateTreeView(childNode, propValue);
                    }
                }
            }
            catch
            {

            }
        }

        public string CheckDifferences(string treeExisting, string treeNew)
        {
            var jdp = new JsonDiffPatch();
            return jdp.Diff(treeExisting, treeNew);
        }

        public void FillData(Asset existingAsset, Asset newAsset)
        {
            treeViewExistingHardware.Nodes.Clear();
            treeViewNewHardware.Nodes.Clear();

            _ = treeViewExistingHardware.Nodes.Add(existingAsset.maintenances[0].serviceDate);
            _ = treeViewNewHardware.Nodes.Add(DateTime.Today.ToString(GenericResources.DATE_FORMAT));

            PopulateTreeView(treeViewExistingHardware.Nodes[0], existingAsset.hardware);
            PopulateTreeView(treeViewNewHardware.Nodes[0], newAsset.hardware);

            treeViewExistingHardware.ExpandAll();
            treeViewNewHardware.ExpandAll();

            textBoxExistingHardwareHashId.Text = existingAsset.hwUid;
            textBoxNewHardwareHashId.Text = newAsset.hwUid;

            string oldCpu = JsonConvert.SerializeObject(existingAsset.hardware.processor);
            string newCpu = JsonConvert.SerializeObject(newAsset.hardware.processor);
            string oldRam = JsonConvert.SerializeObject(existingAsset.hardware.ram);
            string newRam = JsonConvert.SerializeObject(newAsset.hardware.ram);
            string oldStorage = JsonConvert.SerializeObject(existingAsset.hardware.storage);
            string newStorage = JsonConvert.SerializeObject(newAsset.hardware.storage);
            string oldGpu = JsonConvert.SerializeObject(existingAsset.hardware.videoCard);
            string newGpu = JsonConvert.SerializeObject(newAsset.hardware.videoCard);

            string diffCpu = CheckDifferences(oldCpu, newCpu);
            string diffRam = CheckDifferences(oldRam, newRam);
            string diffStorage = CheckDifferences(oldStorage, newStorage);
            string diffGpu = CheckDifferences(oldGpu, newGpu);

            List<string> allDiff = new List<string>() { diffCpu, diffRam, diffStorage, diffGpu };

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
