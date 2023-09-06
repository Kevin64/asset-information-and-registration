using AssetInformationAndRegistration.Interfaces;
using AssetInformationAndRegistration.Misc;
using AssetInformationAndRegistration.Properties;
using ConstantsDLL;
using Dark.Net;
using HardwareInfoDLL;
using LogGeneratorDLL;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Taskbar;
using MRG.Controls.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace AssetInformationAndRegistration.Forms
{
    /// <summary> 
    /// Class for handling Login tasks and UI
    /// </summary>
    internal partial class LoginForm : Form, ITheming
    {
        private bool isSystemDarkModeEnabled;
        private string[] agentsJsonStr = { };
        private readonly LogGenerator log;
        private readonly List<string[]> parametersList;
        private readonly List<string> enforcementList, orgDataList;
        private TaskbarManager tbProgLogin;
        private MainForm mForm;
        private readonly Octokit.GitHubClient ghc;
        private UserPreferenceChangedEventHandler UserPreferenceChanged;

        /// <summary> 
        /// Login form constructor
        /// </summary>
        /// <param name="log">Log file object</param>
        /// <param name="parametersList">List containing data from [Parameters]</param>
        /// <param name="enforcementList">List containing data from [Enforcement]</param>
        /// <param name="orgDataList">List containing data from [OrgData]</param>
        internal LoginForm(Octokit.GitHubClient ghc, LogGenerator log, List<string[]> parametersList, List<string> enforcementList, List<string> orgDataList, bool isSystemDarkModeEnabled)
        {
            //Inits WinForms components
            InitializeComponent();

            //Define theming according to ini file provided info
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

            this.ghc = ghc;
            this.parametersList = parametersList;
            this.enforcementList = enforcementList;
            this.orgDataList = orgDataList;
            this.log = log;
            this.isSystemDarkModeEnabled = isSystemDarkModeEnabled;

            //Sets status bar text according to info provided in the ini file
            string[] oList = new string[6];
            for (int i = 0; i < orgDataList.Count; i++)
            {
                if (!orgDataList[i].Equals(string.Empty))
                    oList[i] = orgDataList[i].ToString() + " - ";
            }

            toolStripStatusBarText.Text = oList[3] + oList[1].Substring(0, oList[1].Length - 2);

            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_THEME, isSystemDarkModeEnabled.ToString(), Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));

            comboBoxServerIP.Items.AddRange(parametersList[0]);
            comboBoxServerPort.Items.AddRange(parametersList[1]);

            //Program version
#if DEBUG
            toolStripVersionText.Text = MiscMethods.Version(Resources.DEV_STATUS);
            comboBoxServerIP.SelectedIndex = 1;
            comboBoxServerPort.SelectedIndex = 0;
#else
            toolStripVersionText.Text = MiscMethods.Version();
            comboBoxServerIP.SelectedIndex = 0;
            comboBoxServerPort.SelectedIndex = 0;
#endif
        }

        public void LightTheme()
        {
            if (HardwareInfo.GetWinVersion().Equals(ConstantsDLL.Properties.Resources.WINDOWS_10))
                DarkNet.Instance.SetCurrentProcessTheme(Theme.Light);

            BackColor = StringsAndConstants.LIGHT_BACKGROUND;

            foreach (Button b in Controls.OfType<Button>())
            {
                b.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
                b.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
                b.FlatAppearance.BorderColor = StringsAndConstants.LIGHT_BACKGROUND;
                b.FlatStyle = System.Windows.Forms.FlatStyle.System;
            }
            foreach (CheckBox cb in Controls.OfType<CheckBox>())
            {
                cb.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
                cb.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            }
            foreach (CustomFlatComboBox cfcb in Controls.OfType<CustomFlatComboBox>())
            {
                cfcb.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
                cfcb.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
                cfcb.BorderColor = StringsAndConstants.LIGHT_FORECOLOR;
                cfcb.ButtonColor = StringsAndConstants.LIGHT_BACKCOLOR;
            }
            foreach (DataGridView dgv in Controls.OfType<DataGridView>())
            {
                dgv.BackgroundColor = StringsAndConstants.LIGHT_BACKGROUND;
                dgv.ColumnHeadersDefaultCellStyle.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
                dgv.ColumnHeadersDefaultCellStyle.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
                dgv.DefaultCellStyle.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
                dgv.DefaultCellStyle.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            }
            foreach (Label l in Controls.OfType<Label>())
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
            foreach (RadioButton rb in Controls.OfType<RadioButton>())
            {
                rb.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
                rb.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            }
            foreach (RichTextBox rtb in Controls.OfType<RichTextBox>())
            {
                rtb.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
                rtb.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            }
            foreach (TextBox tb in Controls.OfType<TextBox>())
            {
                tb.BackColor = StringsAndConstants.LIGHT_BACKCOLOR;
                tb.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
                if (tb.Name.Contains("Inactive"))
                    tb.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
            }

            BackColor = StringsAndConstants.LIGHT_BACKGROUND;

            aboutLabelButton.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            aboutLabelButton.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
            toolStripStatusBarText.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            toolStripVersionText.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            toolStripStatusBarText.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
            toolStripVersionText.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
            statusStrip1.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
            statusStrip1.Renderer = new ModifiedToolStripProfessionalLightTheme();

            aboutLabelButton.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_ABOUT_LIGHT_PATH));
            imgTopBanner.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.LOGIN_BANNER_LIGHT_PATH));
            iconImgUsername.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_AGENT_LIGHT_PATH));
            iconImgPassword.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_PASSWORD_LIGHT_PATH));
            iconImgServerIP.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_SERVER_LIGHT_PATH));
            iconImgServerPort.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_PORT_LIGHT_PATH));
        }

        public void DarkTheme()
        {
            if (HardwareInfo.GetWinVersion().Equals(ConstantsDLL.Properties.Resources.WINDOWS_10))
                DarkNet.Instance.SetCurrentProcessTheme(Theme.Dark);

            BackColor = StringsAndConstants.DARK_BACKGROUND;

            foreach (Button b in Controls.OfType<Button>())
            {
                b.BackColor = StringsAndConstants.DARK_BACKCOLOR;
                b.ForeColor = StringsAndConstants.DARK_FORECOLOR;
                b.FlatAppearance.BorderColor = StringsAndConstants.DARK_BACKGROUND;
                b.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            }
            foreach (CheckBox cb in Controls.OfType<CheckBox>())
            {
                cb.BackColor = StringsAndConstants.DARK_BACKGROUND;
                cb.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            }
            foreach (CustomFlatComboBox cfcb in Controls.OfType<CustomFlatComboBox>())
            {
                cfcb.BackColor = StringsAndConstants.DARK_BACKCOLOR;
                cfcb.ForeColor = StringsAndConstants.DARK_FORECOLOR;
                cfcb.BorderColor = StringsAndConstants.DARK_FORECOLOR;
                cfcb.ButtonColor = StringsAndConstants.DARK_BACKCOLOR;
            }
            foreach (DataGridView dgv in Controls.OfType<DataGridView>())
            {
                dgv.BackgroundColor = StringsAndConstants.DARK_BACKGROUND;
                dgv.ColumnHeadersDefaultCellStyle.BackColor = StringsAndConstants.DARK_BACKGROUND;
                dgv.ColumnHeadersDefaultCellStyle.ForeColor = StringsAndConstants.DARK_FORECOLOR;
                dgv.DefaultCellStyle.BackColor = StringsAndConstants.DARK_BACKCOLOR;
                dgv.DefaultCellStyle.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            }
            foreach (Label l in Controls.OfType<Label>())
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
            foreach (RadioButton rb in Controls.OfType<RadioButton>())
            {
                rb.ForeColor = StringsAndConstants.DARK_FORECOLOR;
                rb.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            }
            foreach (RichTextBox rtb in Controls.OfType<RichTextBox>())
            {
                rtb.BackColor = StringsAndConstants.DARK_BACKCOLOR;
                rtb.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            }
            foreach (TextBox tb in Controls.OfType<TextBox>())
            {
                tb.BackColor = StringsAndConstants.DARK_BACKCOLOR;
                tb.ForeColor = StringsAndConstants.DARK_FORECOLOR;
                if (tb.Name.Contains("Inactive"))
                    tb.BackColor = StringsAndConstants.DARK_BACKGROUND;
            }

            aboutLabelButton.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            aboutLabelButton.BackColor = StringsAndConstants.DARK_BACKGROUND;
            toolStripStatusBarText.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            toolStripVersionText.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            toolStripStatusBarText.BackColor = StringsAndConstants.DARK_BACKGROUND;
            toolStripVersionText.BackColor = StringsAndConstants.DARK_BACKGROUND;
            statusStrip1.BackColor = StringsAndConstants.DARK_BACKGROUND;
            statusStrip1.Renderer = new ModifiedToolStripProfessionalDarkTheme();

            aboutLabelButton.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_ABOUT_DARK_PATH));
            imgTopBanner.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.LOGIN_BANNER_DARK_PATH));
            iconImgUsername.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_AGENT_DARK_PATH));
            iconImgPassword.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_PASSWORD_DARK_PATH));
            iconImgServerIP.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_SERVER_DARK_PATH));
            iconImgServerPort.Image = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConstantsDLL.Properties.Resources.ICON_PORT_DARK_PATH));
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
        /// Loads the form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoginForm_Load(object sender, EventArgs e)
        {
            #region Define loading circle parameters

            switch (MiscMethods.GetWindowsScaling())
            {
                case 100:
                    //Init loading circles parameters for 100% scaling
                    foreach (GroupBox gb in Controls.OfType<GroupBox>())
                    {
                        foreach (LoadingCircle lc in gb.Controls.OfType<LoadingCircle>())
                        {
                            lc.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_100);
                            lc.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_100);
                            lc.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_100);
                            lc.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_100);
                        }
                    }
                    foreach (LoadingCircle lc in Controls.OfType<LoadingCircle>())
                    {
                        lc.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_100);
                        lc.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_100);
                        lc.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_100);
                        lc.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_100);
                    }

                    break;
                case 125:
                    //Init loading circles parameters for 125% scaling
                    foreach (GroupBox gb in Controls.OfType<GroupBox>())
                    {
                        foreach (LoadingCircle lc in gb.Controls.OfType<LoadingCircle>())
                        {
                            lc.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_125);
                            lc.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_125);
                            lc.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_125);
                            lc.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_125);
                        }
                    }
                    foreach (LoadingCircle lc in Controls.OfType<LoadingCircle>())
                    {
                        lc.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_125);
                        lc.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_125);
                        lc.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_125);
                        lc.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_125);
                    }
                    break;
                case 150:
                    //Init loading circles parameters for 150% scaling
                    foreach (GroupBox gb in Controls.OfType<GroupBox>())
                    {
                        foreach (LoadingCircle lc in gb.Controls.OfType<LoadingCircle>())
                        {
                            lc.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_150);
                            lc.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_150);
                            lc.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_150);
                            lc.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_150);
                        }
                    }
                    foreach (LoadingCircle lc in Controls.OfType<LoadingCircle>())
                    {
                        lc.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_150);
                        lc.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_150);
                        lc.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_150);
                        lc.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_150);
                    }
                    break;
                case 175:
                    //Init loading circles parameters for 175% scaling
                    foreach (GroupBox gb in Controls.OfType<GroupBox>())
                    {
                        foreach (LoadingCircle lc in gb.Controls.OfType<LoadingCircle>())
                        {
                            lc.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_175);
                            lc.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_175);
                            lc.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_175);
                            lc.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_175);
                        }
                    }
                    foreach (LoadingCircle lc in Controls.OfType<LoadingCircle>())
                    {
                        lc.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_175);
                        lc.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_175);
                        lc.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_175);
                        lc.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_175);
                    }
                    break;
                case 200:
                    //Init loading circles parameters for 200% scaling
                    foreach (GroupBox gb in Controls.OfType<GroupBox>())
                    {
                        foreach (LoadingCircle lc in gb.Controls.OfType<LoadingCircle>())
                        {
                            lc.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_200);
                            lc.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_200);
                            lc.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_200);
                            lc.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_200);
                        }
                    }
                    foreach (LoadingCircle lc in Controls.OfType<LoadingCircle>())
                    {
                        lc.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_200);
                        lc.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_200);
                        lc.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_200);
                        lc.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_200);
                    }
                    break;
                case 225:
                    //Init loading circles parameters for 225% scaling
                    foreach (GroupBox gb in Controls.OfType<GroupBox>())
                    {
                        foreach (LoadingCircle lc in gb.Controls.OfType<LoadingCircle>())
                        {
                            lc.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_225);
                            lc.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_225);
                            lc.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_225);
                            lc.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_225);
                        }
                    }
                    foreach (LoadingCircle lc in Controls.OfType<LoadingCircle>())
                    {
                        lc.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_225);
                        lc.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_225);
                        lc.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_225);
                        lc.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_225);
                    }
                    break;
                case 250:
                    //Init loading circles parameters for 250% scaling
                    foreach (GroupBox gb in Controls.OfType<GroupBox>())
                    {
                        foreach (LoadingCircle lc in gb.Controls.OfType<LoadingCircle>())
                        {
                            lc.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_250);
                            lc.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_250);
                            lc.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_250);
                            lc.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_250);
                        }
                    }
                    foreach (LoadingCircle lc in Controls.OfType<LoadingCircle>())
                    {
                        lc.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_250);
                        lc.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_250);
                        lc.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_250);
                        lc.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_250);
                    }
                    break;
                case 300:
                    //Init loading circles parameters for 300% scaling
                    foreach (GroupBox gb in Controls.OfType<GroupBox>())
                    {
                        foreach (LoadingCircle lc in gb.Controls.OfType<LoadingCircle>())
                        {
                            lc.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_300);
                            lc.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_300);
                            lc.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_300);
                            lc.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_300);
                        }
                    }
                    foreach (LoadingCircle lc in Controls.OfType<LoadingCircle>())
                    {
                        lc.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_300);
                        lc.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_300);
                        lc.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_300);
                        lc.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_300);
                    }
                    break;
                case 350:
                    //Init loading circles parameters for 350% scaling
                    foreach (GroupBox gb in Controls.OfType<GroupBox>())
                    {
                        foreach (LoadingCircle lc in gb.Controls.OfType<LoadingCircle>())
                        {
                            lc.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_350);
                            lc.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_350);
                            lc.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_350);
                            lc.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_350);
                        }
                    }
                    foreach (LoadingCircle lc in Controls.OfType<LoadingCircle>())
                    {
                        lc.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_350);
                        lc.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_350);
                        lc.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_350);
                        lc.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_350);
                    }
                    break;
            }
            foreach (GroupBox gb in Controls.OfType<GroupBox>())
            {
                foreach (LoadingCircle lc in gb.Controls.OfType<LoadingCircle>())
                {
                    lc.RotationSpeed = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_ROTATION_SPEED);
                    lc.Color = StringsAndConstants.ROTATING_CIRCLE_COLOR;
                }
            }
            foreach (LoadingCircle lc in Controls.OfType<LoadingCircle>())
            {
                lc.RotationSpeed = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_ROTATION_SPEED);
                lc.Color = StringsAndConstants.ROTATING_CIRCLE_COLOR;
            }
            #endregion

            FormClosing += LoginForm_Closing;
            UserPreferenceChanged = new UserPreferenceChangedEventHandler(SystemEvents_UserPreferenceChanged);
            SystemEvents.UserPreferenceChanged += UserPreferenceChanged;
            Disposed += new EventHandler(LoginForm_Disposed);
            tbProgLogin = TaskbarManager.Instance;
        }

        /// <summary> 
        /// Handles the closing of the current form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoginForm_Closing(object sender, FormClosingEventArgs e)
        {
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_CLOSING_LOGIN_FORM, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_MISC), ConstantsDLL.Properties.Resources.LOG_SEPARATOR_SMALL, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));

            //Deletes downloaded json files
            File.Delete(StringsAndConstants.MODEL_FILE_PATH);
            File.Delete(StringsAndConstants.CREDENTIALS_FILE_PATH);
            File.Delete(StringsAndConstants.ASSET_FILE_PATH);
            File.Delete(StringsAndConstants.CONFIG_FILE_PATH);
            if (e.CloseReason == CloseReason.UserClosing)
                Application.Exit();
        }

        /// <summary>
        /// Free resources
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoginForm_Disposed(object sender, EventArgs e)
        {
            SystemEvents.UserPreferenceChanged -= UserPreferenceChanged;
        }

        /// <summary> 
        /// Checks the username/password and shows the main form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void AuthButton_Click(object sender, EventArgs e)
        {
            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), ConstantsDLL.Properties.Strings.LOG_INIT_LOGIN, textBoxUsername.Text, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
            loadingCircleAuthButton.Visible = true;
            loadingCircleAuthButton.Active = true;
            aboutLabelButton.Enabled = false;
            if (checkBoxOfflineMode.Checked)
            {
                string[] offStr = { Strings.OFFLINE_MODE_ACTIVATED };
                tbProgLogin.SetProgressState(TaskbarProgressBarState.NoProgress, Handle);
                mForm = new MainForm(ghc, true, offStr, null, null, log, parametersList, enforcementList, orgDataList, isSystemDarkModeEnabled);
                if (HardwareInfo.GetWinVersion().Equals(ConstantsDLL.Properties.Resources.WINDOWS_10))
                    DarkNet.Instance.SetWindowThemeForms(mForm, Theme.Auto);

                Hide();
                textBoxUsername.Text = null;
                textBoxPassword.Text = null;
                textBoxUsername.Select();
                _ = mForm.ShowDialog();
                mForm.Close();
                mForm.Dispose();
                Show();
            }
            else
            {
                textBoxUsername.Enabled = false;
                textBoxPassword.Enabled = false;
                comboBoxServerIP.Enabled = false;
                comboBoxServerPort.Enabled = false;
                checkBoxOfflineMode.Enabled = false;
                tbProgLogin.SetProgressState(TaskbarProgressBarState.Indeterminate, Handle);
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOG_SERVER_DETAIL, comboBoxServerIP.Text + ":" + comboBoxServerPort.Text, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));

                //Feches login data from server
                agentsJsonStr = await JsonFileReaderDLL.CredentialsFileReader.FetchInfoMT(textBoxUsername.Text, textBoxPassword.Text, comboBoxServerIP.Text, comboBoxServerPort.Text);

                //If all the mandatory fields are filled
                if (!string.IsNullOrWhiteSpace(textBoxUsername.Text) && !string.IsNullOrWhiteSpace(textBoxPassword.Text))
                {
                    //If Login Json file does not exist, there is no internet connection
                    if (agentsJsonStr == null)
                    {
                        log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), ConstantsDLL.Properties.Strings.INTRANET_REQUIRED, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
                        _ = MessageBox.Show(ConstantsDLL.Properties.Strings.INTRANET_REQUIRED, ConstantsDLL.Properties.Strings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        tbProgLogin.SetProgressState(TaskbarProgressBarState.NoProgress, Handle);
                    }
                    else if (agentsJsonStr[0] == ConstantsDLL.Properties.Resources.FALSE) //If Login Json file does exist, but the agent do not exist
                    {
                        log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), ConstantsDLL.Properties.Strings.LOG_LOGIN_FAILED, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
                        _ = MessageBox.Show(Strings.AUTH_INVALID, ConstantsDLL.Properties.Strings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        tbProgLogin.SetProgressState(TaskbarProgressBarState.NoProgress, Handle);
                    }
                    else //If Login Json file does exist and agent logs in
                    {
                        log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), ConstantsDLL.Properties.Strings.LOG_LOGIN_SUCCESS, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
                        MainForm mForm = new MainForm(ghc, false, agentsJsonStr, comboBoxServerIP.Text, comboBoxServerPort.Text, log, parametersList, enforcementList, orgDataList, isSystemDarkModeEnabled);
                        if (HardwareInfo.GetWinVersion().Equals(ConstantsDLL.Properties.Resources.WINDOWS_10))
                            DarkNet.Instance.SetWindowThemeForms(mForm, Theme.Auto);

                        Hide();
                        textBoxUsername.Text = null;
                        textBoxPassword.Text = null;
                        textBoxUsername.Select();
                        _ = mForm.ShowDialog();
                        mForm.Close();
                        mForm.Dispose();
                        Show();
                    }
                }
                else //If all the mandatory fields are not filled
                {
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), ConstantsDLL.Properties.Strings.NO_AUTH, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
                    _ = MessageBox.Show(ConstantsDLL.Properties.Strings.NO_AUTH, ConstantsDLL.Properties.Strings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    tbProgLogin.SetProgressState(TaskbarProgressBarState.NoProgress, Handle);
                }
                aboutLabelButton.Enabled = true;
            }

            //Enables controls if login is not successful
            loadingCircleAuthButton.Visible = false;
            loadingCircleAuthButton.Active = false;
            textBoxUsername.Enabled = true;
            _ = textBoxUsername.Focus();
            textBoxPassword.Enabled = true;
            comboBoxServerIP.Enabled = true;
            comboBoxServerPort.Enabled = true;
            checkBoxOfflineMode.Enabled = true;
            checkBoxOfflineMode.Checked = false;
        }

        /// <summary> 
        /// Handles the offline mode toggle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxOfflineMode.Checked)
            {
                textBoxUsername.Enabled = false;
                textBoxPassword.Enabled = false;
                comboBoxServerIP.Enabled = false;
                comboBoxServerPort.Enabled = false;
            }
            else
            {
                textBoxUsername.Enabled = true;
                textBoxPassword.Enabled = true;
                comboBoxServerIP.Enabled = true;
                comboBoxServerPort.Enabled = true;
            }
        }

        /// <summary> 
        /// Opens the About box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AboutLabelButton_Click(object sender, EventArgs e)
        {
            AboutBox aForm = new AboutBox(ghc, log, parametersList, isSystemDarkModeEnabled);
            if (HardwareInfo.GetWinVersion().Equals(ConstantsDLL.Properties.Resources.WINDOWS_10))
                DarkNet.Instance.SetWindowThemeForms(aForm, Theme.Auto);
            _ = aForm.ShowDialog();
        }
    }
}
