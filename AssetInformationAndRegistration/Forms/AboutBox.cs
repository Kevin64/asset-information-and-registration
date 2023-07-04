using AssetInformationAndRegistration.Interfaces;
using AssetInformationAndRegistration.Properties;
using AssetInformationAndRegistration.Updater;
using ConstantsDLL;
using Dark.Net;
using HardwareInfoDLL;
using LogGeneratorDLL;
using System;
using System.Collections.Generic;
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
        private readonly bool themeBool;
        private readonly LogGenerator log;
        private readonly Octokit.GitHubClient ghc;

        /// <summary> 
        /// About form constructor
        /// </summary>
        /// <param name="parametersList">List containing data from [Parameters]</param>
        /// <param name="themeBool">Theme mode</param>
        internal AboutBox(Octokit.GitHubClient ghc, LogGenerator log, List<string[]> parametersList, bool themeBool)
        {
            InitializeComponent();

            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), ConstantsDLL.Properties.Strings.LOG_OPENING_ABOUTBOX, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));

            this.ghc = ghc;
            this.parametersList = parametersList;
            this.themeBool = themeBool;
            this.log = log;

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

            if (StringsAndConstants.LIST_THEME_GUI.Contains(parametersList[3][0].ToString()) && parametersList[3][0].ToString().Equals(StringsAndConstants.LIST_THEME_GUI[0]))
            {
                if (themeBool)
                {
                    if (HardwareInfo.GetWinVersion().Equals(ConstantsDLL.Properties.Resources.WINDOWS_10))
                    {
                        DarkNet.Instance.SetCurrentProcessTheme(Theme.Dark);
                    }
                    DarkTheme();
                }
                else
                {
                    if (HardwareInfo.GetWinVersion().Equals(ConstantsDLL.Properties.Resources.WINDOWS_10))
                    {
                        DarkNet.Instance.SetCurrentProcessTheme(Theme.Light);
                    }
                    LightTheme();
                }
            }
            else if (parametersList[3][0].ToString().Equals(StringsAndConstants.LIST_THEME_GUI[1]))
            {
                LightTheme();
            }
            else if (parametersList[3][0].ToString().Equals(StringsAndConstants.LIST_THEME_GUI[2]))
            {
                DarkTheme();
            }
        }

        public void LightTheme()
        {
            BackColor = StringsAndConstants.LIGHT_BACKGROUND;

            labelProductName.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            labelVersion.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            labelCopyright.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            labelCompanyName.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;

            checkUpdateButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            okButton.FlatStyle = System.Windows.Forms.FlatStyle.System;

            textBoxDescription.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
            textBoxDescription.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
        }

        public void DarkTheme()
        {
            BackColor = StringsAndConstants.DARK_BACKGROUND;

            labelProductName.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            labelVersion.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            labelCopyright.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            labelCompanyName.ForeColor = StringsAndConstants.DARK_FORECOLOR;

            checkUpdateButton.BackColor = StringsAndConstants.DARK_BACKCOLOR;
            checkUpdateButton.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            checkUpdateButton.FlatAppearance.BorderColor = StringsAndConstants.DARK_BACKGROUND;
            checkUpdateButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            okButton.BackColor = StringsAndConstants.DARK_BACKCOLOR;
            okButton.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            okButton.FlatAppearance.BorderColor = StringsAndConstants.DARK_BACKGROUND;
            okButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;

            textBoxDescription.BackColor = StringsAndConstants.DARK_BACKCOLOR;
            textBoxDescription.ForeColor = StringsAndConstants.DARK_FORECOLOR;
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
                    {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        public string AssemblyVersion => Assembly.GetExecutingAssembly().GetName().Version.ToString();

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

        /// <summary> 
        /// Triggers an update check
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckUpdateButton_Click(object sender, System.EventArgs e)
        {
            UpdateChecker.Check(ghc, log, parametersList, themeBool, false);
        }
    }
}
