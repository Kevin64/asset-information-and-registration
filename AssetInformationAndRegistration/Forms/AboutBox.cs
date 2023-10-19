using AssetInformationAndRegistration.Interfaces;
using AssetInformationAndRegistration.Misc;
using AssetInformationAndRegistration.Properties;
using AssetInformationAndRegistration.Updater;
using ConstantsDLL;
using Dark.Net;
using HardwareInfoDLL;
using LogGeneratorDLL;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace AssetInformationAndRegistration.Forms
{
    /// <summary> 
    /// Class for About box
    /// </summary>
    internal partial class AboutBox : Form, ITheming
    {
        private readonly List<string[]> parametersList;
        private readonly LogGenerator log;
        private readonly Octokit.GitHubClient ghc;
        private bool isSystemDarkModeEnabled;
        private UserPreferenceChangedEventHandler UserPreferenceChanged;

        /// <summary> 
        /// About form constructor
        /// </summary>
        /// <param name="parametersList">List containing data from [Parameters]</param>
        /// <param name="isSystemDarkModeEnabled">Theme mode</param>
        internal AboutBox(Octokit.GitHubClient ghc, LogGenerator log, List<string[]> parametersList, bool isSystemDarkModeEnabled)
        {
            InitializeComponent();
            this.KeyDown += AboutBox_KeyDown;

            (int themeFileSet, bool _) = MiscMethods.GetFileThemeMode(parametersList, isSystemDarkModeEnabled);
            switch (themeFileSet)
            {
                case 0:
                    LightTheme();
                    break;
                case 1:
                    DarkTheme();
                    break;
            }

            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), ConstantsDLL.Properties.Strings.LOG_OPENING_ABOUTBOX, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));

            this.ghc = ghc;
            this.parametersList = parametersList;
            this.log = log;
            this.isSystemDarkModeEnabled = isSystemDarkModeEnabled;

            Text = string.Format("{0} {1}", labelFormTitle.Text, AssemblyTitle);
            labelProductName.Text = AssemblyProduct;
#if DEBUG
            labelVersion.Text = string.Format(labelVersion.Text + " {0}-{1}", AssemblyVersion, Resources.DEV_STATUS);
#else
            labelVersion.Text = string.Format(labelVersion.Text + " {0}", AssemblyVersion);
#endif
            labelCopyright.Text = AssemblyCopyright;
            labelCompanyName.Text = AssemblyCompany;
            textBoxDescription.Text = Strings.DESCRIPTION;
            textBoxDescription.LinkClicked += TextBoxDescription_LinkClicked;
        }

        private void AboutBox_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        public void LightTheme()
        {
            if (HardwareInfo.GetWinVersion().Equals(ConstantsDLL.Properties.Resources.WINDOWS_10))
                DarkNet.Instance.SetCurrentProcessTheme(Theme.Light);

            BackColor = StringsAndConstants.LIGHT_BACKGROUND;

            foreach (TableLayoutPanel tlp in Controls.OfType<TableLayoutPanel>())
            {
                foreach (Button b in tlp.Controls.OfType<Button>())
                {
                    b.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
                    b.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
                    b.FlatAppearance.BorderColor = StringsAndConstants.LIGHT_BACKGROUND;
                    b.FlatStyle = System.Windows.Forms.FlatStyle.System;
                }
                foreach (CheckBox cb in tlp.Controls.OfType<CheckBox>())
                {
                    cb.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
                    cb.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
                }
                foreach (CustomFlatComboBox cfcb in tlp.Controls.OfType<CustomFlatComboBox>())
                {
                    cfcb.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
                    cfcb.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
                    cfcb.BorderColor = StringsAndConstants.LIGHT_FORECOLOR;
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
                foreach (Label l in tlp.Controls.OfType<Label>())
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
                foreach (RadioButton rb in tlp.Controls.OfType<RadioButton>())
                {
                    rb.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
                    rb.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
                }
                foreach (RichTextBox rtb in tlp.Controls.OfType<RichTextBox>())
                {
                    rtb.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
                    rtb.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
                }
                foreach (TextBox tb in tlp.Controls.OfType<TextBox>())
                {
                    tb.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
                    tb.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
                    if (tb.Name.Contains("Inactive"))
                        tb.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
                }
            }
        }

        public void DarkTheme()
        {
            if (HardwareInfo.GetWinVersion().Equals(ConstantsDLL.Properties.Resources.WINDOWS_10))
                DarkNet.Instance.SetCurrentProcessTheme(Theme.Dark);

            BackColor = StringsAndConstants.DARK_BACKGROUND;

            foreach (TableLayoutPanel tlp in Controls.OfType<TableLayoutPanel>())
            {
                foreach (Button b in tlp.Controls.OfType<Button>())
                {
                    b.BackColor = StringsAndConstants.DARK_BACKCOLOR;
                    b.ForeColor = StringsAndConstants.DARK_FORECOLOR;
                    b.FlatAppearance.BorderColor = StringsAndConstants.DARK_BACKGROUND;
                    b.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                }
                foreach (CheckBox cb in tlp.Controls.OfType<CheckBox>())
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
                foreach (Label l in tlp.Controls.OfType<Label>())
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
                foreach (RadioButton rb in tlp.Controls.OfType<RadioButton>())
                {
                    rb.ForeColor = StringsAndConstants.DARK_FORECOLOR;
                    rb.ForeColor = StringsAndConstants.DARK_FORECOLOR;
                }
                foreach (RichTextBox rtb in tlp.Controls.OfType<RichTextBox>())
                {
                    rtb.BackColor = StringsAndConstants.DARK_BACKCOLOR;
                    rtb.ForeColor = StringsAndConstants.DARK_FORECOLOR;
                }
                foreach (TextBox tb in tlp.Controls.OfType<TextBox>())
                {
                    tb.BackColor = StringsAndConstants.DARK_BACKCOLOR;
                    tb.ForeColor = StringsAndConstants.DARK_FORECOLOR;
                    if (tb.Name.Contains("Inactive"))
                        tb.BackColor = StringsAndConstants.DARK_BACKGROUND;
                }
            }
        }

        /// <summary> 
        /// Method for auto selecting the app theme
        /// </summary>
        private void ToggleTheme()
        {
            (int themeFileSet, bool _) = MiscMethods.GetFileThemeMode(parametersList, MiscMethods.GetSystemThemeMode());
            switch (themeFileSet)
            {
                case 0:
                    LightTheme();
                    isSystemDarkModeEnabled = false;
                    break;
                case 1:
                    DarkTheme();
                    isSystemDarkModeEnabled = true;
                    break;
            }
        }

        /// <summary>
        /// Allows the theme to change automatically according to the system one
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SystemEvents_UserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
        {
            if (e.Category == UserPreferenceCategory.General)
                ToggleTheme();
        }

        /// <summary>
        /// Free resources
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AboutBox_Disposed(object sender, EventArgs e)
        {
            SystemEvents.UserPreferenceChanged -= UserPreferenceChanged;
        }

        /// <summary> 
        /// Handles link clicks inside the Description box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBoxDescription_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            _ = System.Diagnostics.Process.Start(e.LinkText);
        }

        /// <summary> 
        /// Triggers an update check
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckUpdateButton_Click(object sender, System.EventArgs e)
        {
            UpdateChecker.Check(ghc, log, parametersList, true, true, false, isSystemDarkModeEnabled);
        }

        /// <summary>
        /// Closes the form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OkButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary> 
        /// Handles the closing of the current form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AboutBox_Closing(object sender, FormClosingEventArgs e)
        {
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), ConstantsDLL.Properties.Strings.LOG_CLOSING_ABOUTBOX, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
        }

        /// <summary> 
        /// Loads the form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AboutBox_Load(object sender, EventArgs e)
        {
            UserPreferenceChanged = new UserPreferenceChangedEventHandler(SystemEvents_UserPreferenceChanged);
            SystemEvents.UserPreferenceChanged += UserPreferenceChanged;
            Disposed += new EventHandler(AboutBox_Disposed);
            FormClosing += AboutBox_Closing;
        }

        #region Acessório de Atributos do Assembly

        public string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != string.Empty)
                        return titleAttribute.Title;
                }
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        public string AssemblyVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public string AssemblyDescription
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                return attributes.Length == 0 ? string.Empty : ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        public string AssemblyProduct
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                return attributes.Length == 0 ? string.Empty : ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        public string AssemblyCopyright
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                return attributes.Length == 0 ? string.Empty : ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        public string AssemblyCompany
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                return attributes.Length == 0 ? string.Empty : ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }
        #endregion
    }
}
