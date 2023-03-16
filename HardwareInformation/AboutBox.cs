using ConstantsDLL;
using Dark.Net;
using HardwareInfoDLL;
using HardwareInformation.Properties;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;

namespace HardwareInformation
{
    partial class AboutBox : Form
    {
        public AboutBox(List<string[]> definitionList, bool themeBool)
        {
            InitializeComponent();
            this.Text = String.Format("Sobre {0}", AssemblyTitle);
            this.labelProductName.Text = AssemblyProduct;
#if DEBUG
            this.labelVersion.Text = String.Format("Versão {0}-{1}", AssemblyVersion, Resources.dev_status);
#else
            this.labelVersion.Text = String.Format("Versão {0}", AssemblyVersion);
#endif
            this.labelCopyright.Text = AssemblyCopyright;
            this.labelCompanyName.Text = AssemblyCompany;
            this.textBoxDescription.Text = AssemblyDescription;
            this.textBoxDescription.LinkClicked += TextBoxDescription_LinkClicked;

            if (StringsAndConstants.listThemeGUI.Contains(definitionList[5][0].ToString()) && definitionList[5][0].ToString().Equals(StringsAndConstants.listThemeGUI[0]))
            {
                if (themeBool)
                {
                    if (HardwareInfo.GetOSInfoAux().Equals(StringsAndConstants.windows10))
                        DarkNet.Instance.SetCurrentProcessTheme(Theme.Dark);
                    DarkTheme();
                }
                else
                {
                    if (HardwareInfo.GetOSInfoAux().Equals(StringsAndConstants.windows10))
                        DarkNet.Instance.SetCurrentProcessTheme(Theme.Light);
                    LightTheme();
                }
            }
            else if (definitionList[5][0].ToString().Equals(StringsAndConstants.listThemeGUI[1]))
                LightTheme();
            else if (definitionList[5][0].ToString().Equals(StringsAndConstants.listThemeGUI[2]))
                DarkTheme();
        }

        public void LightTheme()
        {
            this.BackColor = StringsAndConstants.LIGHT_BACKGROUND;

            this.labelProductName.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.labelVersion.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.labelCopyright.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            this.labelCompanyName.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;

            this.okButton.FlatStyle = System.Windows.Forms.FlatStyle.System;

            this.textBoxDescription.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
            this.textBoxDescription.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
        }

        public void DarkTheme()
        {
            this.BackColor = StringsAndConstants.DARK_BACKGROUND;

            this.labelProductName.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.labelVersion.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.labelCopyright.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.labelCompanyName.ForeColor = StringsAndConstants.DARK_FORECOLOR;

            this.okButton.BackColor = StringsAndConstants.DARK_BACKCOLOR;
            this.okButton.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            this.okButton.FlatAppearance.BorderColor = StringsAndConstants.DARK_BACKGROUND;

            this.textBoxDescription.BackColor = StringsAndConstants.DARK_BACKCOLOR;
            this.textBoxDescription.ForeColor = StringsAndConstants.DARK_FORECOLOR;
        }

        private void TextBoxDescription_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.LinkText);
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
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
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
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        public string AssemblyProduct
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        public string AssemblyCopyright
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        public string AssemblyCompany
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }
        #endregion
    }
}
