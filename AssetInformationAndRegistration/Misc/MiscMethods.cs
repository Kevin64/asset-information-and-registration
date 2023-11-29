using AssetInformationAndRegistration.Properties;
using AssetInformationAndRegistration.Updater;
using ConstantsDLL;
using ConstantsDLL.Properties;
using Dark.Net;
using HardwareInfoDLL;
using LogGeneratorDLL;
using Microsoft.Win32;
using MRG.Controls.UI;
using Newtonsoft.Json;
using RestApiDLL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using static AssetInformationAndRegistration.Program;

namespace AssetInformationAndRegistration.Misc
{
    /// <summary> 
    /// Class that allows changing the progressbar color
    /// </summary>
    public static class ModifyProgressBarColor
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr w, IntPtr l);
        public static void SetState(this System.Windows.Forms.ProgressBar pBar, int state)
        {
            _ = SendMessage(pBar.Handle, 1040, (IntPtr)state, IntPtr.Zero);
        }
    }

    /// <summary> 
    /// Class for miscelaneous methods
    /// </summary>
    public static class MiscMethods
    {
        /// <summary> 
        /// Creates registry keys when a successful update check is made
        /// </summary>
        /// <param name="ui">An UpdateInfo object to write into the registry</param>
        internal static void RegCreateUpdateData(UpdateInfo ui)
        {
            RegistryKey rk;
            if (Environment.Is64BitOperatingSystem)
                rk = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            else
                rk = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
            rk = rk.CreateSubKey(GenericResources.REGISTRY_AIR_PATH, true);
            rk.SetValue(GenericResources.GITHUB_ETAG, ui.ETag, RegistryValueKind.String);
            rk.SetValue(GenericResources.GITHUB_TAG_NAME, ui.TagName, RegistryValueKind.String);
            rk.SetValue(GenericResources.GITHUB_BODY, ui.Body, RegistryValueKind.String);
            rk.SetValue(GenericResources.GITHUB_HTML_URL, ui.HtmlUrl, RegistryValueKind.String);
        }

        /// <summary>
        /// Deletes AIR registry keys
        /// </summary>
        public static void RegDeleteUpdateData()
        {
            try
            {
                RegistryKey rk;
                if (Environment.Is64BitOperatingSystem)
                    rk = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
                else
                    rk = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
                rk.DeleteSubKeyTree(GenericResources.REGISTRY_AIR_PATH);
            }
            catch
            {

            }
        }

        /// <summary>
        /// Checks the registry for existing update metadata
        /// </summary>
        /// <returns>An UpdateInfo object containing the ETag, TagName, Body and HtmlURL</returns>
        public static UpdateInfo RegCheckUpdateData()
        {
            UpdateInfo ui = new UpdateInfo();
            try
            {
                RegistryKey rk;
                if (Environment.Is64BitOperatingSystem)
                    rk = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
                else
                    rk = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
                rk = rk.CreateSubKey(GenericResources.REGISTRY_AIR_PATH, true);
                ui.ETag = rk.GetValue(GenericResources.GITHUB_ETAG).ToString();
                ui.TagName = rk.GetValue(GenericResources.GITHUB_TAG_NAME).ToString();
                ui.Body = rk.GetValue(GenericResources.GITHUB_BODY).ToString();
                ui.HtmlUrl = rk.GetValue(GenericResources.GITHUB_HTML_URL).ToString();
                return ui;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Hides Loading Circles
        /// </summary>
        /// <param name="form">Target form where controls will be changed</param>
        internal static void HideLoadingCircles(Form form)
        {
            foreach (System.Windows.Forms.GroupBox gb in form.Controls.OfType<System.Windows.Forms.GroupBox>())
            {
                foreach (LoadingCircle lc in gb.Controls.OfType<LoadingCircle>())
                {
                    if (lc.Name.Contains("Scan"))
                    {
                        lc.Visible = false;
                        lc.Active = false;
                    }
                }
            }
            foreach (LoadingCircle lc in form.Controls.OfType<LoadingCircle>())
            {
                if (lc.Name.Contains("Scan"))
                {
                    lc.Visible = false;
                    lc.Active = false;
                }
            }
        }

        /// <summary>
        /// Sets Loading Circles parameters
        /// </summary>
        /// <param name="form">Target form where controls will be changed</param>
        public static void SetLoadingCircles(Form form)
        {
            switch (GetWindowsScaling())
            {
                case 100:
                    //Init loading circles parameters for 100% scaling
                    foreach (System.Windows.Forms.GroupBox gb in form.Controls.OfType<System.Windows.Forms.GroupBox>())
                    {
                        foreach (LoadingCircle lc in gb.Controls.OfType<LoadingCircle>())
                        {
                            lc.NumberSpoke = Convert.ToInt32(GenericResources.LOADING_CIRCLE_NUMBER_SPOKE_100);
                            lc.SpokeThickness = Convert.ToInt32(GenericResources.LOADING_CIRCLE_SPOKE_THICKNESS_100);
                            lc.InnerCircleRadius = Convert.ToInt32(GenericResources.LOADING_CIRCLE_INNER_RADIUS_100);
                            lc.OuterCircleRadius = Convert.ToInt32(GenericResources.LOADING_CIRCLE_OUTER_RADIUS_100);
                        }
                    }
                    foreach (LoadingCircle lc in form.Controls.OfType<LoadingCircle>())
                    {
                        lc.NumberSpoke = Convert.ToInt32(GenericResources.LOADING_CIRCLE_NUMBER_SPOKE_100);
                        lc.SpokeThickness = Convert.ToInt32(GenericResources.LOADING_CIRCLE_SPOKE_THICKNESS_100);
                        lc.InnerCircleRadius = Convert.ToInt32(GenericResources.LOADING_CIRCLE_INNER_RADIUS_100);
                        lc.OuterCircleRadius = Convert.ToInt32(GenericResources.LOADING_CIRCLE_OUTER_RADIUS_100);
                    }

                    break;
                case 125:
                    //Init loading circles parameters for 125% scaling
                    foreach (System.Windows.Forms.GroupBox gb in form.Controls.OfType<System.Windows.Forms.GroupBox>())
                    {
                        foreach (LoadingCircle lc in gb.Controls.OfType<LoadingCircle>())
                        {
                            lc.NumberSpoke = Convert.ToInt32(GenericResources.LOADING_CIRCLE_NUMBER_SPOKE_125);
                            lc.SpokeThickness = Convert.ToInt32(GenericResources.LOADING_CIRCLE_SPOKE_THICKNESS_125);
                            lc.InnerCircleRadius = Convert.ToInt32(GenericResources.LOADING_CIRCLE_INNER_RADIUS_125);
                            lc.OuterCircleRadius = Convert.ToInt32(GenericResources.LOADING_CIRCLE_OUTER_RADIUS_125);
                        }
                    }
                    foreach (LoadingCircle lc in form.Controls.OfType<LoadingCircle>())
                    {
                        lc.NumberSpoke = Convert.ToInt32(GenericResources.LOADING_CIRCLE_NUMBER_SPOKE_125);
                        lc.SpokeThickness = Convert.ToInt32(GenericResources.LOADING_CIRCLE_SPOKE_THICKNESS_125);
                        lc.InnerCircleRadius = Convert.ToInt32(GenericResources.LOADING_CIRCLE_INNER_RADIUS_125);
                        lc.OuterCircleRadius = Convert.ToInt32(GenericResources.LOADING_CIRCLE_OUTER_RADIUS_125);
                    }
                    break;
                case 150:
                    //Init loading circles parameters for 150% scaling
                    foreach (System.Windows.Forms.GroupBox gb in form.Controls.OfType<System.Windows.Forms.GroupBox>())
                    {
                        foreach (LoadingCircle lc in gb.Controls.OfType<LoadingCircle>())
                        {
                            lc.NumberSpoke = Convert.ToInt32(GenericResources.LOADING_CIRCLE_NUMBER_SPOKE_150);
                            lc.SpokeThickness = Convert.ToInt32(GenericResources.LOADING_CIRCLE_SPOKE_THICKNESS_150);
                            lc.InnerCircleRadius = Convert.ToInt32(GenericResources.LOADING_CIRCLE_INNER_RADIUS_150);
                            lc.OuterCircleRadius = Convert.ToInt32(GenericResources.LOADING_CIRCLE_OUTER_RADIUS_150);
                        }
                    }
                    foreach (LoadingCircle lc in form.Controls.OfType<LoadingCircle>())
                    {
                        lc.NumberSpoke = Convert.ToInt32(GenericResources.LOADING_CIRCLE_NUMBER_SPOKE_150);
                        lc.SpokeThickness = Convert.ToInt32(GenericResources.LOADING_CIRCLE_SPOKE_THICKNESS_150);
                        lc.InnerCircleRadius = Convert.ToInt32(GenericResources.LOADING_CIRCLE_INNER_RADIUS_150);
                        lc.OuterCircleRadius = Convert.ToInt32(GenericResources.LOADING_CIRCLE_OUTER_RADIUS_150);
                    }
                    break;
                case 175:
                    //Init loading circles parameters for 175% scaling
                    foreach (System.Windows.Forms.GroupBox gb in form.Controls.OfType<System.Windows.Forms.GroupBox>())
                    {
                        foreach (LoadingCircle lc in gb.Controls.OfType<LoadingCircle>())
                        {
                            lc.NumberSpoke = Convert.ToInt32(GenericResources.LOADING_CIRCLE_NUMBER_SPOKE_175);
                            lc.SpokeThickness = Convert.ToInt32(GenericResources.LOADING_CIRCLE_SPOKE_THICKNESS_175);
                            lc.InnerCircleRadius = Convert.ToInt32(GenericResources.LOADING_CIRCLE_INNER_RADIUS_175);
                            lc.OuterCircleRadius = Convert.ToInt32(GenericResources.LOADING_CIRCLE_OUTER_RADIUS_175);
                        }
                    }
                    foreach (LoadingCircle lc in form.Controls.OfType<LoadingCircle>())
                    {
                        lc.NumberSpoke = Convert.ToInt32(GenericResources.LOADING_CIRCLE_NUMBER_SPOKE_175);
                        lc.SpokeThickness = Convert.ToInt32(GenericResources.LOADING_CIRCLE_SPOKE_THICKNESS_175);
                        lc.InnerCircleRadius = Convert.ToInt32(GenericResources.LOADING_CIRCLE_INNER_RADIUS_175);
                        lc.OuterCircleRadius = Convert.ToInt32(GenericResources.LOADING_CIRCLE_OUTER_RADIUS_175);
                    }
                    break;
                case 200:
                    //Init loading circles parameters for 200% scaling
                    foreach (System.Windows.Forms.GroupBox gb in form.Controls.OfType<System.Windows.Forms.GroupBox>())
                    {
                        foreach (LoadingCircle lc in gb.Controls.OfType<LoadingCircle>())
                        {
                            lc.NumberSpoke = Convert.ToInt32(GenericResources.LOADING_CIRCLE_NUMBER_SPOKE_200);
                            lc.SpokeThickness = Convert.ToInt32(GenericResources.LOADING_CIRCLE_SPOKE_THICKNESS_200);
                            lc.InnerCircleRadius = Convert.ToInt32(GenericResources.LOADING_CIRCLE_INNER_RADIUS_200);
                            lc.OuterCircleRadius = Convert.ToInt32(GenericResources.LOADING_CIRCLE_OUTER_RADIUS_200);
                        }
                    }
                    foreach (LoadingCircle lc in form.Controls.OfType<LoadingCircle>())
                    {
                        lc.NumberSpoke = Convert.ToInt32(GenericResources.LOADING_CIRCLE_NUMBER_SPOKE_200);
                        lc.SpokeThickness = Convert.ToInt32(GenericResources.LOADING_CIRCLE_SPOKE_THICKNESS_200);
                        lc.InnerCircleRadius = Convert.ToInt32(GenericResources.LOADING_CIRCLE_INNER_RADIUS_200);
                        lc.OuterCircleRadius = Convert.ToInt32(GenericResources.LOADING_CIRCLE_OUTER_RADIUS_200);
                    }
                    break;
                case 225:
                    //Init loading circles parameters for 225% scaling
                    foreach (System.Windows.Forms.GroupBox gb in form.Controls.OfType<System.Windows.Forms.GroupBox>())
                    {
                        foreach (LoadingCircle lc in gb.Controls.OfType<LoadingCircle>())
                        {
                            lc.NumberSpoke = Convert.ToInt32(GenericResources.LOADING_CIRCLE_NUMBER_SPOKE_225);
                            lc.SpokeThickness = Convert.ToInt32(GenericResources.LOADING_CIRCLE_SPOKE_THICKNESS_225);
                            lc.InnerCircleRadius = Convert.ToInt32(GenericResources.LOADING_CIRCLE_INNER_RADIUS_225);
                            lc.OuterCircleRadius = Convert.ToInt32(GenericResources.LOADING_CIRCLE_OUTER_RADIUS_225);
                        }
                    }
                    foreach (LoadingCircle lc in form.Controls.OfType<LoadingCircle>())
                    {
                        lc.NumberSpoke = Convert.ToInt32(GenericResources.LOADING_CIRCLE_NUMBER_SPOKE_225);
                        lc.SpokeThickness = Convert.ToInt32(GenericResources.LOADING_CIRCLE_SPOKE_THICKNESS_225);
                        lc.InnerCircleRadius = Convert.ToInt32(GenericResources.LOADING_CIRCLE_INNER_RADIUS_225);
                        lc.OuterCircleRadius = Convert.ToInt32(GenericResources.LOADING_CIRCLE_OUTER_RADIUS_225);
                    }
                    break;
                case 250:
                    //Init loading circles parameters for 250% scaling
                    foreach (System.Windows.Forms.GroupBox gb in form.Controls.OfType<System.Windows.Forms.GroupBox>())
                    {
                        foreach (LoadingCircle lc in gb.Controls.OfType<LoadingCircle>())
                        {
                            lc.NumberSpoke = Convert.ToInt32(GenericResources.LOADING_CIRCLE_NUMBER_SPOKE_250);
                            lc.SpokeThickness = Convert.ToInt32(GenericResources.LOADING_CIRCLE_SPOKE_THICKNESS_250);
                            lc.InnerCircleRadius = Convert.ToInt32(GenericResources.LOADING_CIRCLE_INNER_RADIUS_250);
                            lc.OuterCircleRadius = Convert.ToInt32(GenericResources.LOADING_CIRCLE_OUTER_RADIUS_250);
                        }
                    }
                    foreach (LoadingCircle lc in form.Controls.OfType<LoadingCircle>())
                    {
                        lc.NumberSpoke = Convert.ToInt32(GenericResources.LOADING_CIRCLE_NUMBER_SPOKE_250);
                        lc.SpokeThickness = Convert.ToInt32(GenericResources.LOADING_CIRCLE_SPOKE_THICKNESS_250);
                        lc.InnerCircleRadius = Convert.ToInt32(GenericResources.LOADING_CIRCLE_INNER_RADIUS_250);
                        lc.OuterCircleRadius = Convert.ToInt32(GenericResources.LOADING_CIRCLE_OUTER_RADIUS_250);
                    }
                    break;
                case 300:
                    //Init loading circles parameters for 300% scaling
                    foreach (System.Windows.Forms.GroupBox gb in form.Controls.OfType<System.Windows.Forms.GroupBox>())
                    {
                        foreach (LoadingCircle lc in gb.Controls.OfType<LoadingCircle>())
                        {
                            lc.NumberSpoke = Convert.ToInt32(GenericResources.LOADING_CIRCLE_NUMBER_SPOKE_300);
                            lc.SpokeThickness = Convert.ToInt32(GenericResources.LOADING_CIRCLE_SPOKE_THICKNESS_300);
                            lc.InnerCircleRadius = Convert.ToInt32(GenericResources.LOADING_CIRCLE_INNER_RADIUS_300);
                            lc.OuterCircleRadius = Convert.ToInt32(GenericResources.LOADING_CIRCLE_OUTER_RADIUS_300);
                        }
                    }
                    foreach (LoadingCircle lc in form.Controls.OfType<LoadingCircle>())
                    {
                        lc.NumberSpoke = Convert.ToInt32(GenericResources.LOADING_CIRCLE_NUMBER_SPOKE_300);
                        lc.SpokeThickness = Convert.ToInt32(GenericResources.LOADING_CIRCLE_SPOKE_THICKNESS_300);
                        lc.InnerCircleRadius = Convert.ToInt32(GenericResources.LOADING_CIRCLE_INNER_RADIUS_300);
                        lc.OuterCircleRadius = Convert.ToInt32(GenericResources.LOADING_CIRCLE_OUTER_RADIUS_300);
                    }
                    break;
                case 350:
                    //Init loading circles parameters for 350% scaling
                    foreach (System.Windows.Forms.GroupBox gb in form.Controls.OfType<System.Windows.Forms.GroupBox>())
                    {
                        foreach (LoadingCircle lc in gb.Controls.OfType<LoadingCircle>())
                        {
                            lc.NumberSpoke = Convert.ToInt32(GenericResources.LOADING_CIRCLE_NUMBER_SPOKE_350);
                            lc.SpokeThickness = Convert.ToInt32(GenericResources.LOADING_CIRCLE_SPOKE_THICKNESS_350);
                            lc.InnerCircleRadius = Convert.ToInt32(GenericResources.LOADING_CIRCLE_INNER_RADIUS_350);
                            lc.OuterCircleRadius = Convert.ToInt32(GenericResources.LOADING_CIRCLE_OUTER_RADIUS_350);
                        }
                    }
                    foreach (LoadingCircle lc in form.Controls.OfType<LoadingCircle>())
                    {
                        lc.NumberSpoke = Convert.ToInt32(GenericResources.LOADING_CIRCLE_NUMBER_SPOKE_350);
                        lc.SpokeThickness = Convert.ToInt32(GenericResources.LOADING_CIRCLE_SPOKE_THICKNESS_350);
                        lc.InnerCircleRadius = Convert.ToInt32(GenericResources.LOADING_CIRCLE_INNER_RADIUS_350);
                        lc.OuterCircleRadius = Convert.ToInt32(GenericResources.LOADING_CIRCLE_OUTER_RADIUS_350);
                    }
                    break;
            }

            foreach (System.Windows.Forms.GroupBox gb in form.Controls.OfType<System.Windows.Forms.GroupBox>())
            {
                foreach (LoadingCircle lc in gb.Controls.OfType<LoadingCircle>())
                {
                    lc.RotationSpeed = Convert.ToInt32(GenericResources.LOADING_CIRCLE_ROTATION_SPEED);
                    lc.Color = StringsAndConstants.LOADING_CIRCLE_COLOR;
                }
            }
            foreach (LoadingCircle lc in form.Controls.OfType<LoadingCircle>())
            {
                lc.RotationSpeed = Convert.ToInt32(GenericResources.LOADING_CIRCLE_ROTATION_SPEED);
                lc.Color = StringsAndConstants.LOADING_CIRCLE_COLOR;
            }
        }

        /// <summary>
        /// Sets all controls from form in light theme, recursively
        /// </summary>
        /// <param name="form">Target form where controls will be changed</param>
        internal static void LightThemeAllControls(Form form)
        {
            if (HardwareInfo.GetWinVersion().Equals(GenericResources.WIN_10_NAMENUM))
                DarkNet.Instance.SetCurrentProcessTheme(Theme.Light);

            form.BackColor = StringsAndConstants.LIGHT_BACKGROUND;

            foreach (System.Windows.Forms.Button b in form.Controls.OfType<System.Windows.Forms.Button>())
            {
                b.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
                b.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
                b.FlatAppearance.BorderColor = StringsAndConstants.LIGHT_BACKGROUND;
                b.FlatStyle = FlatStyle.System;
            }
            foreach (System.Windows.Forms.CheckBox cb in form.Controls.OfType<System.Windows.Forms.CheckBox>())
            {
                cb.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
                cb.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            }
            foreach (CustomFlatComboBox cfcb in form.Controls.OfType<CustomFlatComboBox>())
            {
                cfcb.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
                cfcb.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
                cfcb.BorderColor = StringsAndConstants.LIGHT_DROPDOWN_BORDER;
                cfcb.ButtonColor = StringsAndConstants.LIGHT_BACKCOLOR;
            }
            foreach (DataGridView dgv in form.Controls.OfType<DataGridView>())
            {
                dgv.BackgroundColor = StringsAndConstants.LIGHT_BACKGROUND;
                dgv.ColumnHeadersDefaultCellStyle.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
                dgv.ColumnHeadersDefaultCellStyle.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
                dgv.DefaultCellStyle.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
                dgv.DefaultCellStyle.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            }
            foreach (System.Windows.Forms.Label l in form.Controls.OfType<System.Windows.Forms.Label>())
            {
                if (l.Name.Contains("Separator"))
                    l.BackColor = StringsAndConstants.LIGHT_SUBTLE_DARKDARKCOLOR;
                else if (l.Name.Contains("Mandatory"))
                    l.ForeColor = StringsAndConstants.LIGHT_ASTERISKCOLOR;
                else if (l.Name.Contains("Notice"))
                    l.ForeColor = StringsAndConstants.LIGHT_NOTICECOLOR;
                else if (l.Name.Contains("Fixed"))
                    l.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
                else if (!l.Name.Contains("Color"))
                    l.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
            }
            foreach (System.Windows.Forms.RadioButton rb in form.Controls.OfType<System.Windows.Forms.RadioButton>())
            {
                rb.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
                rb.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            }
            foreach (System.Windows.Forms.RichTextBox rtb in form.Controls.OfType<System.Windows.Forms.RichTextBox>())
            {
                rtb.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
                rtb.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            }
            foreach (System.Windows.Forms.TextBox tb in form.Controls.OfType<System.Windows.Forms.TextBox>())
            {
                tb.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
                tb.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
                if (tb.Name.Contains("Inactive"))
                    tb.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
            }
            foreach (System.Windows.Forms.TreeView tb in form.Controls.OfType<System.Windows.Forms.TreeView>())
            {
                tb.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
                tb.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            }

            foreach (System.Windows.Forms.GroupBox gb in form.Controls.OfType<System.Windows.Forms.GroupBox>())
            {
                gb.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
                gb.Paint += CustomColors.GroupBox_PaintLightTheme;

                foreach (System.Windows.Forms.Button b in gb.Controls.OfType<System.Windows.Forms.Button>())
                {
                    b.BackColor = SystemColors.Control;
                    b.ForeColor = SystemColors.ControlText;
                    b.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
                }
                foreach (System.Windows.Forms.CheckBox cb in gb.Controls.OfType<System.Windows.Forms.CheckBox>())
                {
                    cb.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
                    cb.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
                }
                foreach (CustomFlatComboBox cfcb in gb.Controls.OfType<CustomFlatComboBox>())
                {
                    cfcb.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
                    cfcb.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
                    cfcb.BorderColor = StringsAndConstants.LIGHT_DROPDOWN_BORDER;
                    cfcb.ButtonColor = StringsAndConstants.LIGHT_BACKCOLOR;
                }
                foreach (DataGridView dgv in gb.Controls.OfType<DataGridView>())
                {
                    dgv.BackgroundColor = StringsAndConstants.LIGHT_BACKGROUND;
                    dgv.ColumnHeadersDefaultCellStyle.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
                    dgv.ColumnHeadersDefaultCellStyle.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
                    dgv.DefaultCellStyle.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
                    dgv.DefaultCellStyle.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
                }
                foreach (System.Windows.Forms.Label l in gb.Controls.OfType<System.Windows.Forms.Label>())
                {
                    if (l.Name.Contains("Separator"))
                        l.BackColor = StringsAndConstants.LIGHT_SUBTLE_DARKDARKCOLOR;
                    else if (l.Name.Contains("Mandatory"))
                        l.ForeColor = StringsAndConstants.LIGHT_ASTERISKCOLOR;
                    else if (l.Name.Contains("Notice"))
                        l.ForeColor = StringsAndConstants.LIGHT_NOTICECOLOR;
                    else if (l.Name.Contains("Fixed"))
                        l.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
                    else if (!l.Name.Contains("Color"))
                        l.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
                }
                foreach (System.Windows.Forms.RadioButton rb in gb.Controls.OfType<System.Windows.Forms.RadioButton>())
                {
                    rb.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
                    rb.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
                }
                foreach (System.Windows.Forms.RichTextBox rtb in gb.Controls.OfType<System.Windows.Forms.RichTextBox>())
                {
                    rtb.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
                    rtb.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
                }
                foreach (System.Windows.Forms.TextBox tb in gb.Controls.OfType<System.Windows.Forms.TextBox>())
                {
                    tb.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
                    tb.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
                    if (tb.Name.Contains("Inactive"))
                        tb.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
                }
                foreach (System.Windows.Forms.TreeView tb in gb.Controls.OfType<System.Windows.Forms.TreeView>())
                {
                    tb.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
                    tb.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
                }
            }

            foreach (TableLayoutPanel tlp in form.Controls.OfType<TableLayoutPanel>())
            {
                foreach (System.Windows.Forms.Button b in tlp.Controls.OfType<System.Windows.Forms.Button>())
                {
                    b.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
                    b.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
                    b.FlatAppearance.BorderColor = StringsAndConstants.LIGHT_BACKGROUND;
                    b.FlatStyle = System.Windows.Forms.FlatStyle.System;
                }
                foreach (System.Windows.Forms.CheckBox cb in tlp.Controls.OfType<System.Windows.Forms.CheckBox>())
                {
                    cb.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
                    cb.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
                }
                foreach (CustomFlatComboBox cfcb in tlp.Controls.OfType<CustomFlatComboBox>())
                {
                    cfcb.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
                    cfcb.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
                    cfcb.BorderColor = StringsAndConstants.LIGHT_DROPDOWN_BORDER;
                    cfcb.ButtonColor = StringsAndConstants.LIGHT_BACKCOLOR;
                }
                foreach (DataGridView dgv in tlp.Controls.OfType<DataGridView>())
                {
                    dgv.BackgroundColor = StringsAndConstants.LIGHT_BACKGROUND;
                    dgv.ColumnHeadersDefaultCellStyle.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
                    dgv.ColumnHeadersDefaultCellStyle.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
                    dgv.DefaultCellStyle.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
                    dgv.DefaultCellStyle.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
                }
                foreach (System.Windows.Forms.Label l in tlp.Controls.OfType<System.Windows.Forms.Label>())
                {
                    if (l.Name.Contains("Separator"))
                        l.BackColor = StringsAndConstants.LIGHT_SUBTLE_DARKDARKCOLOR;
                    else if (l.Name.Contains("Mandatory"))
                        l.ForeColor = StringsAndConstants.LIGHT_ASTERISKCOLOR;
                    else if (l.Name.Contains("Fixed"))
                        l.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
                    else if (!l.Name.Contains("Color"))
                        l.ForeColor = StringsAndConstants.LIGHT_SUBTLE_DARKCOLOR;
                }
                foreach (System.Windows.Forms.RadioButton rb in tlp.Controls.OfType<System.Windows.Forms.RadioButton>())
                {
                    rb.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
                    rb.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
                }
                foreach (System.Windows.Forms.RichTextBox rtb in tlp.Controls.OfType<System.Windows.Forms.RichTextBox>())
                {
                    rtb.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
                    rtb.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
                }
                foreach (System.Windows.Forms.TextBox tb in tlp.Controls.OfType<System.Windows.Forms.TextBox>())
                {
                    tb.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
                    tb.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
                    if (tb.Name.Contains("Inactive"))
                        tb.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
                }
                foreach (System.Windows.Forms.TreeView tb in tlp.Controls.OfType<System.Windows.Forms.TreeView>())
                {
                    tb.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
                    tb.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
                }
            }

            form.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
        }

        /// <summary>
        /// Sets all controls from form in dark theme, recursively
        /// </summary>
        /// <param name="form">Target form where controls will be changed</param>
        internal static void DarkThemeAllControls(Form form)
        {
            if (HardwareInfo.GetWinVersion().Equals(GenericResources.WIN_10_NAMENUM))
                DarkNet.Instance.SetCurrentProcessTheme(Theme.Dark);

            form.BackColor = StringsAndConstants.DARK_BACKGROUND;

            foreach (System.Windows.Forms.Button b in form.Controls.OfType<System.Windows.Forms.Button>())
            {
                b.BackColor = StringsAndConstants.DARK_BACKCOLOR;
                b.ForeColor = StringsAndConstants.DARK_FORECOLOR;
                b.FlatAppearance.BorderColor = StringsAndConstants.DARK_BACKGROUND;
                b.FlatStyle = FlatStyle.Flat;
            }
            foreach (System.Windows.Forms.CheckBox cb in form.Controls.OfType<System.Windows.Forms.CheckBox>())
            {
                cb.BackColor = StringsAndConstants.DARK_BACKGROUND;
                cb.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            }
            foreach (CustomFlatComboBox cfcb in form.Controls.OfType<CustomFlatComboBox>())
            {
                cfcb.BackColor = StringsAndConstants.DARK_BACKCOLOR;
                cfcb.ForeColor = StringsAndConstants.DARK_FORECOLOR;
                cfcb.BorderColor = StringsAndConstants.DARK_FORECOLOR;
                cfcb.ButtonColor = StringsAndConstants.DARK_BACKCOLOR;
            }
            foreach (DataGridView dgv in form.Controls.OfType<DataGridView>())
            {
                dgv.BackgroundColor = StringsAndConstants.DARK_BACKGROUND;
                dgv.ColumnHeadersDefaultCellStyle.BackColor = StringsAndConstants.DARK_BACKGROUND;
                dgv.ColumnHeadersDefaultCellStyle.ForeColor = StringsAndConstants.DARK_FORECOLOR;
                dgv.DefaultCellStyle.BackColor = StringsAndConstants.DARK_BACKCOLOR;
                dgv.DefaultCellStyle.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            }
            foreach (System.Windows.Forms.Label l in form.Controls.OfType<System.Windows.Forms.Label>())
            {
                if (l.Name.Contains("Separator"))
                    l.BackColor = StringsAndConstants.DARK_SUBTLE_LIGHTLIGHTCOLOR;
                else if (l.Name.Contains("Mandatory"))
                    l.ForeColor = StringsAndConstants.DARK_ASTERISKCOLOR;
                else if (l.Name.Contains("Notice"))
                    l.ForeColor = StringsAndConstants.DARK_NOTICECOLOR;
                else if (l.Name.Contains("Fixed"))
                    l.ForeColor = StringsAndConstants.DARK_FORECOLOR;
                else if (!l.Name.Contains("Color"))
                    l.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
            }
            foreach (System.Windows.Forms.RadioButton rb in form.Controls.OfType<System.Windows.Forms.RadioButton>())
            {
                rb.ForeColor = StringsAndConstants.DARK_FORECOLOR;
                rb.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            }
            foreach (System.Windows.Forms.RichTextBox rtb in form.Controls.OfType<System.Windows.Forms.RichTextBox>())
            {
                rtb.BackColor = StringsAndConstants.DARK_BACKCOLOR;
                rtb.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            }
            foreach (System.Windows.Forms.TextBox tb in form.Controls.OfType<System.Windows.Forms.TextBox>())
            {
                tb.BackColor = StringsAndConstants.DARK_BACKCOLOR;
                tb.ForeColor = StringsAndConstants.DARK_FORECOLOR;
                if (tb.Name.Contains("Inactive"))
                    tb.BackColor = StringsAndConstants.DARK_BACKGROUND;
            }
            foreach (System.Windows.Forms.TreeView tv in form.Controls.OfType<System.Windows.Forms.TreeView>())
            {
                tv.BackColor = StringsAndConstants.DARK_BACKCOLOR;
                tv.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            }

            foreach (System.Windows.Forms.GroupBox gb in form.Controls.OfType<System.Windows.Forms.GroupBox>())
            {
                gb.ForeColor = StringsAndConstants.DARK_FORECOLOR;
                gb.Paint += CustomColors.GroupBox_PaintDarkTheme;

                foreach (System.Windows.Forms.Button b in gb.Controls.OfType<System.Windows.Forms.Button>())
                {
                    b.BackColor = StringsAndConstants.DARK_BACKCOLOR;
                    b.ForeColor = StringsAndConstants.DARK_FORECOLOR;
                    b.FlatAppearance.BorderColor = StringsAndConstants.DARK_BACKGROUND;
                    b.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                }
                foreach (System.Windows.Forms.CheckBox cb in gb.Controls.OfType<System.Windows.Forms.CheckBox>())
                {
                    cb.BackColor = StringsAndConstants.DARK_BACKGROUND;
                    cb.ForeColor = StringsAndConstants.DARK_FORECOLOR;
                }
                foreach (CustomFlatComboBox cfcb in gb.Controls.OfType<CustomFlatComboBox>())
                {
                    cfcb.BackColor = StringsAndConstants.DARK_BACKCOLOR;
                    cfcb.ForeColor = StringsAndConstants.DARK_FORECOLOR;
                    cfcb.BorderColor = StringsAndConstants.DARK_FORECOLOR;
                    cfcb.ButtonColor = StringsAndConstants.DARK_BACKCOLOR;
                }
                foreach (DataGridView dgv in gb.Controls.OfType<DataGridView>())
                {
                    dgv.BackgroundColor = StringsAndConstants.DARK_BACKGROUND;
                    dgv.ColumnHeadersDefaultCellStyle.BackColor = StringsAndConstants.DARK_BACKGROUND;
                    dgv.ColumnHeadersDefaultCellStyle.ForeColor = StringsAndConstants.DARK_FORECOLOR;
                    dgv.DefaultCellStyle.BackColor = StringsAndConstants.DARK_BACKCOLOR;
                    dgv.DefaultCellStyle.ForeColor = StringsAndConstants.DARK_FORECOLOR;
                }
                foreach (System.Windows.Forms.Label l in gb.Controls.OfType<System.Windows.Forms.Label>())
                {
                    if (l.Name.Contains("Separator"))
                        l.BackColor = StringsAndConstants.DARK_SUBTLE_LIGHTLIGHTCOLOR;
                    else if (l.Name.Contains("Mandatory"))
                        l.ForeColor = StringsAndConstants.DARK_ASTERISKCOLOR;
                    else if (l.Name.Contains("Notice"))
                        l.ForeColor = StringsAndConstants.DARK_NOTICECOLOR;
                    else if (l.Name.Contains("Fixed"))
                        l.ForeColor = StringsAndConstants.DARK_FORECOLOR;
                    else if (!l.Name.Contains("Color"))
                        l.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
                }
                foreach (System.Windows.Forms.RadioButton rb in gb.Controls.OfType<System.Windows.Forms.RadioButton>())
                {
                    rb.ForeColor = StringsAndConstants.DARK_FORECOLOR;
                    rb.ForeColor = StringsAndConstants.DARK_FORECOLOR;
                }
                foreach (System.Windows.Forms.RichTextBox rtb in gb.Controls.OfType<System.Windows.Forms.RichTextBox>())
                {
                    rtb.BackColor = StringsAndConstants.DARK_BACKCOLOR;
                    rtb.ForeColor = StringsAndConstants.DARK_FORECOLOR;
                }
                foreach (System.Windows.Forms.TextBox tb in gb.Controls.OfType<System.Windows.Forms.TextBox>())
                {
                    tb.BackColor = StringsAndConstants.DARK_BACKCOLOR;
                    tb.ForeColor = StringsAndConstants.DARK_FORECOLOR;
                    if (tb.Name.Contains("Inactive"))
                        tb.BackColor = StringsAndConstants.DARK_BACKGROUND;
                }
                foreach (System.Windows.Forms.TreeView tv in gb.Controls.OfType<System.Windows.Forms.TreeView>())
                {
                    tv.BackColor = StringsAndConstants.DARK_BACKCOLOR;
                    tv.ForeColor = StringsAndConstants.DARK_FORECOLOR;
                }
            }

            foreach (TableLayoutPanel tlp in form.Controls.OfType<TableLayoutPanel>())
            {
                foreach (System.Windows.Forms.Button b in tlp.Controls.OfType<System.Windows.Forms.Button>())
                {
                    b.BackColor = StringsAndConstants.DARK_BACKCOLOR;
                    b.ForeColor = StringsAndConstants.DARK_FORECOLOR;
                    b.FlatAppearance.BorderColor = StringsAndConstants.DARK_BACKGROUND;
                    b.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                }
                foreach (System.Windows.Forms.CheckBox cb in tlp.Controls.OfType<System.Windows.Forms.CheckBox>())
                {
                    cb.BackColor = StringsAndConstants.DARK_BACKGROUND;
                    cb.ForeColor = StringsAndConstants.DARK_FORECOLOR;
                }
                foreach (CustomFlatComboBox cfcb in tlp.Controls.OfType<CustomFlatComboBox>())
                {
                    cfcb.BackColor = StringsAndConstants.DARK_BACKCOLOR;
                    cfcb.ForeColor = StringsAndConstants.DARK_FORECOLOR;
                    cfcb.BorderColor = StringsAndConstants.DARK_FORECOLOR;
                    cfcb.ButtonColor = StringsAndConstants.DARK_BACKCOLOR;
                }
                foreach (DataGridView dgv in tlp.Controls.OfType<DataGridView>())
                {
                    dgv.BackgroundColor = StringsAndConstants.DARK_BACKGROUND;
                    dgv.ColumnHeadersDefaultCellStyle.BackColor = StringsAndConstants.DARK_BACKGROUND;
                    dgv.ColumnHeadersDefaultCellStyle.ForeColor = StringsAndConstants.DARK_FORECOLOR;
                    dgv.DefaultCellStyle.BackColor = StringsAndConstants.DARK_BACKCOLOR;
                    dgv.DefaultCellStyle.ForeColor = StringsAndConstants.DARK_FORECOLOR;
                }
                foreach (System.Windows.Forms.Label l in tlp.Controls.OfType<System.Windows.Forms.Label>())
                {
                    if (l.Name.Contains("Separator"))
                        l.BackColor = StringsAndConstants.DARK_SUBTLE_LIGHTLIGHTCOLOR;
                    else if (l.Name.Contains("Mandatory"))
                        l.ForeColor = StringsAndConstants.DARK_ASTERISKCOLOR;
                    else if (l.Name.Contains("Fixed"))
                        l.ForeColor = StringsAndConstants.DARK_FORECOLOR;
                    else if (!l.Name.Contains("Color"))
                        l.ForeColor = StringsAndConstants.DARK_SUBTLE_LIGHTCOLOR;
                }
                foreach (System.Windows.Forms.RadioButton rb in tlp.Controls.OfType<System.Windows.Forms.RadioButton>())
                {
                    rb.ForeColor = StringsAndConstants.DARK_FORECOLOR;
                    rb.ForeColor = StringsAndConstants.DARK_FORECOLOR;
                }
                foreach (System.Windows.Forms.RichTextBox rtb in tlp.Controls.OfType<System.Windows.Forms.RichTextBox>())
                {
                    rtb.BackColor = StringsAndConstants.DARK_BACKCOLOR;
                    rtb.ForeColor = StringsAndConstants.DARK_FORECOLOR;
                }
                foreach (System.Windows.Forms.TextBox tb in tlp.Controls.OfType<System.Windows.Forms.TextBox>())
                {
                    tb.BackColor = StringsAndConstants.DARK_BACKCOLOR;
                    tb.ForeColor = StringsAndConstants.DARK_FORECOLOR;
                    if (tb.Name.Contains("Inactive"))
                        tb.BackColor = StringsAndConstants.DARK_BACKGROUND;
                }
                foreach (System.Windows.Forms.TreeView tv in tlp.Controls.OfType<System.Windows.Forms.TreeView>())
                {
                    tv.BackColor = StringsAndConstants.DARK_BACKCOLOR;
                    tv.ForeColor = StringsAndConstants.DARK_FORECOLOR;
                }
            }
        }

        internal static string FriendlySizeBinary(long bytes, bool hasDecimal)
        {
            if (bytes < 0) throw new ArgumentException("bytes");

            double friendly = 0;
            string sufix = "B";

            if (bytes >= 1152921504606846976L)
            {
                friendly = bytes >> 50;
                sufix = GenericResources.SIZE_EB;
            }
            else if (bytes >= 1125899906842624L)
            {
                friendly = bytes >> 40;
                sufix = GenericResources.SIZE_PB;
            }
            else if (bytes >= 1099511627776L)
            {
                friendly = bytes >> 30;
                sufix = GenericResources.SIZE_TB;
            }
            else if (bytes >= 1073741824)
            {
                friendly = bytes >> 20;
                sufix = GenericResources.SIZE_GB;
            }
            else if (bytes >= 1048576)
            {
                friendly = bytes >> 10;
                sufix = GenericResources.SIZE_MB;
            }
            else if (bytes >= 1024)
            {
                friendly = bytes;
                sufix = GenericResources.SIZE_KB;
            }

            friendly /= 1024;
            if (hasDecimal)
                return friendly.ToString("0.##") + " " + sufix;
            else
                return Convert.ToInt64(friendly).ToString() + " " + sufix;
        }

        internal static string FriendlySizeDecimal(long bytes, bool hasDecimal)
        {
            if (bytes < 0) throw new ArgumentException("bytes");

            double friendly = 0;
            string sufix = "B";

            if (bytes >= 1000000000000000000L)
            {
                friendly = bytes / 1000 / 1000 / 1000 / 1000 / 1000;
                sufix = GenericResources.SIZE_EB;
            }
            else if (bytes >= 1000000000000000L)
            {
                friendly = bytes / 1000 / 1000 / 1000 / 1000;
                sufix = GenericResources.SIZE_PB;
            }
            else if (bytes >= 1000000000000L)
            {
                friendly = bytes / 1000 / 1000 / 1000;
                sufix = GenericResources.SIZE_TB;
            }
            else if (bytes >= 1000000000)
            {
                friendly = bytes / 1000 / 1000;
                sufix = GenericResources.SIZE_GB;
            }
            else if (bytes >= 1000000)
            {
                friendly = bytes / 1000;
                sufix = GenericResources.SIZE_MB;
            }
            else if (bytes >= 1000)
            {
                friendly = bytes;
                sufix = GenericResources.SIZE_KB;
            }

            friendly /= 1000;
            if (hasDecimal)
                return friendly.ToString("0.##") + " " + sufix;
            else
                return Convert.ToInt64(friendly).ToString() + " " + sufix;
        }

        /// <summary> 
        /// Checks if a log file exists and creates a directory if necessary
        /// </summary>
        /// <param name="path">File path</param>
        /// <returns>'true' if log exists, 'false' if not</returns>
        internal static string CheckIfLogExists(string path)
        {
            try
            {
                bool b;
#if DEBUG
                //Checks if log directory exists
                b = File.Exists(path + GenericResources.LOG_FILENAME_AIR + "-v" + Application.ProductVersion + "-" + GenericResources.DEV_STATUS_BETA + GenericResources.LOG_FILE_EXT);
#else
                //Checks if log directory exists
                b = File.Exists(path + GenericResources.LOG_FILENAME_AIR + "-v" + Application.ProductVersion + GenericResources.LOG_FILE_EXT);
#endif
                //If not, creates a new directory
                if (!b)
                {
                    Directory.CreateDirectory(path);
                    return GenericResources.FALSE;
                }
                return GenericResources.TRUE;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        /// <summary> 
        /// Initializes the theme, according to the host theme
        /// </summary>
        /// <returns>'true' if system is using Dark theme, 'false' if otherwise</returns>
        internal static bool GetSystemThemeMode()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(GenericResources.REGISTRY_THEME_PATH))
                {
                    if (key != null)
                    {
                        object o = key.GetValue(GenericResources.REGISTRY_THEME_KEY);
                        return o != null && o.Equals(0);
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Gets theme setting from config file
        /// </summary>
        /// <param name="parametersList">List containing data from [Parameters]</param>
        /// <param name="isSystemDarkModeEnabled">Theme mode</param>
        /// <returns>0 for light mode, 1 for dark mode, true for allow theme edit, false otherwise</returns>
        internal static (int themeFileSet, bool themeEditable) GetFileThemeMode(Definitions definitions, bool isSystemDarkModeEnabled)
        {
            if (StringsAndConstants.LIST_THEME_GUI.Contains(definitions.ThemeUI) && definitions.ThemeUI.Equals(StringsAndConstants.LIST_THEME_GUI[0]))
            {
                if (isSystemDarkModeEnabled)
                    return (1, true);
                else
                    return (0, true);
            }
            else if (definitions.ThemeUI.Equals(StringsAndConstants.LIST_THEME_GUI[1]))
            {
                return (0, false);
            }
            else if (definitions.ThemeUI.Equals(StringsAndConstants.LIST_THEME_GUI[2]))
            {
                return (1, false);
            }
            else
            {
                return (0, false);
            }
        }

        /// <summary> 
        /// Updates the 'last installed' or 'last maintenance' labels
        /// </summary>
        /// <param name="date">Last service date</param>
        /// <returns>Text which will be shown inside the program, with number of days since the last service</returns>
        internal static string SinceLabelUpdate(string date)
        {
            if (date != string.Empty)
            {
                DateTime d = Convert.ToDateTime(date);
                return (DateTime.Today - d).TotalDays + AirUIStrings.DAYS_PASSED_TEXT + AirUIStrings.SERVICE_TEXT;
            }
            else
            {
                return AirUIStrings.SINCE_UNKNOWN;
            }
        }

        /// <summary> 
        /// Fetches the screen scale
        /// </summary>
        /// <returns>The current window scaling</returns>
        internal static int GetWindowsScaling()
        {
            return Convert.ToInt32(100 * Screen.PrimaryScreen.Bounds.Width / System.Windows.SystemParameters.PrimaryScreenWidth);
        }

        /// <summary>
        /// Transpose an List object that containing Lists, inverting its rows and columns
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="lists">List of Lists</param>
        /// <returns>The list of lists transposed</returns>
        internal static List<List<T>> Transpose<T>(List<List<T>> lists)
        {
            int longest = lists.Any() ? lists.Max(l => l.Count) : 0;
            List<List<T>> outer = new List<List<T>>(longest);
            for (int i = 0; i < longest; i++)
                outer.Add(new List<T>(lists.Count));
            for (int j = 0; j < lists.Count; j++)
            {
                for (int i = 0; i < longest; i++)
                    outer[i].Add(lists[j].Count > i ? lists[j][i] : default);
            }
            return outer;
        }

        /// <summary>
        /// Closes the program, outputting info
        /// </summary>
        /// <param name="log">Log file object</param>
        /// <param name="exitCode">Program exit code</param>
        internal static void CloseProgram(LogGenerator log, int exitCode, bool outputCLI)
        {
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_CLOSING_FORM, string.Empty, outputCLI);
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_MISC), GenericResources.LOG_SEPARATOR_SMALL, string.Empty, outputCLI);

            Environment.Exit(exitCode);
        }

        /// <summary>
        /// Generates a unique SHA256 hash according to the asset brand, model, serial number and MAC Address
        /// </summary>
        /// <param name="a">Asset object</param>
        /// <returns>Hash string</returns>
        internal static string HardwareSha256UniqueId(Asset a)
        {
            hardware hwTemp = new hardware();
            network nwTemp = new network();
            Asset assetTemp = new Asset()
            {
                hardware = hwTemp,
                network = nwTemp,
            };
            assetTemp.hardware.hwBrand = a.hardware.hwBrand;
            assetTemp.hardware.hwModel = a.hardware.hwModel;
            assetTemp.hardware.hwSerialNumber = a.hardware.hwSerialNumber;
            assetTemp.network.netMacAddress = a.network.netMacAddress;

            using (SHA256 hash = SHA256.Create())
            {
                byte[] bytes = hash.ComputeHash(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(assetTemp)));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                    _ = builder.Append(bytes[i].ToString("x2"));
                return builder.ToString();
            }
        }

        /// <summary>
        /// Generates a unique SHA256 hash according to the asset removable hardware
        /// </summary>
        /// <param name="a">Asset object</param>
        /// <returns>Hash string</returns>
        internal static string HardwareSha256Hash(Asset a)
        {
            Asset assetTemp = a.ShallowCopy();
            hardware hardwareTemp = a.hardware.ShallowCopy();
            assetTemp.hardware = hardwareTemp;
            assetTemp.assetNumber = null;
            assetTemp.discarded = null;
            assetTemp.inUse = null;
            assetTemp.note = null;
            assetTemp.sealNumber = null;
            assetTemp.standard = null;
            assetTemp.tag = null;
            assetTemp.adRegistered = null;
            hardwareTemp.hwType = null;
            assetTemp.hwHash = null;
            assetTemp.assetHash = null;
            assetTemp.location = null;
            assetTemp.network = null;
            assetTemp.firmware = null;
            assetTemp.maintenances = null;
            assetTemp.operatingSystem = null;

            using (SHA256 hash = SHA256.Create())
            {
                byte[] bytes = hash.ComputeHash(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(assetTemp)));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                    _ = builder.Append(bytes[i].ToString("x2"));
                return builder.ToString();
            }
        }

        /// <summary> 
        /// Fetches the program's binary version
        /// </summary>
        /// <returns>The current application version in the format 'v0.0.0.0'</returns>
        internal static string Version()
        {
            return "v" + Application.ProductVersion;
        }

        /// <summary> 
        /// Fetches the program's binary version (for unstable releases)
        /// </summary>
        /// <param name="testBranch">Test branch (alpha, beta, rc, etc)</param>
        /// <returns>The current application version in the format 'v0.0.0.0-testBranch'</returns>
        internal static string Version(string testBranch)
        {
            return "v" + Application.ProductVersion + "-" + testBranch;
        }
    }
}
