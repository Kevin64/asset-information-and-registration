using AssetInformationAndRegistration.Properties;
using ConstantsDLL;
using Dark.Net;
using HardwareInfoDLL;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;

namespace AssetInformationAndRegistration
{
    ///<summary>Class for About box</summary>
    internal partial class AboutBox : Form
    {
        ///<summary>About form constructor</summary>
        ///<param name="parametersList">List containing data from [Parameters]</param>
        ///<param name="themeBool">Theme mode</param>
        public AboutBox(List<string[]> parametersList, bool themeBool)
        {
            InitializeComponent();
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

        ///<summary>Sets a light theme for the about form</summary>
        public void LightTheme()
        {
            BackColor = StringsAndConstants.LIGHT_BACKGROUND;

            labelProductName.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            labelVersion.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            labelCopyright.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            labelCompanyName.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;

            okButton.FlatStyle = System.Windows.Forms.FlatStyle.System;

            textBoxDescription.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
            textBoxDescription.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
        }

        ///<summary>Sets a dark theme for the about form</summary>
        public void DarkTheme()
        {
            BackColor = StringsAndConstants.DARK_BACKGROUND;

            labelProductName.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            labelVersion.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            labelCopyright.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            labelCompanyName.ForeColor = StringsAndConstants.DARK_FORECOLOR;

            okButton.BackColor = StringsAndConstants.DARK_BACKCOLOR;
            okButton.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            okButton.FlatAppearance.BorderColor = StringsAndConstants.DARK_BACKGROUND;

            textBoxDescription.BackColor = StringsAndConstants.DARK_BACKCOLOR;
            textBoxDescription.ForeColor = StringsAndConstants.DARK_FORECOLOR;
        }

        ///<summary>Handles link clicks inside the Description box</summary>
        ///<param name="sender"></param>
        ///<param name="e"></param>
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
    }
}
