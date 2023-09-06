using AssetInformationAndRegistration.Interfaces;
using AssetInformationAndRegistration.Misc;
using ConstantsDLL;
using Dark.Net;
using HardwareInfoDLL;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace AssetInformationAndRegistration.Forms
{
    /// <summary> 
    /// Class for the Busy form
    /// </summary>
    internal partial class BusyForm : Form, ITheming
    {
        private bool isSystemDarkModeEnabled;
        private readonly List<string[]> parametersList;
        private readonly UserPreferenceChangedEventHandler UserPreferenceChanged;

        /// <summary> 
        /// Busy form constructor
        /// </summary>
        /// <param name="isSystemDarkModeEnabled">Theme mode</param>
        internal BusyForm(List<string[]> parametersList, bool isSystemDarkModeEnabled)
        {
            InitializeComponent();

            this.isSystemDarkModeEnabled = isSystemDarkModeEnabled;
            this.parametersList = parametersList;

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

            loadingCircleLoading.Enabled = true;
            loadingCircleLoading.Active = true;

            switch (MiscMethods.GetWindowsScaling())
            {
                case 100:
                    //Init loading circles parameters for 100% scaling
                    loadingCircleLoading.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_100);
                    loadingCircleLoading.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_100);
                    loadingCircleLoading.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_100);
                    loadingCircleLoading.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_100);
                    break;
                case 125:
                    //Init loading circles parameters for 125% scaling
                    loadingCircleLoading.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_125);
                    loadingCircleLoading.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_125);
                    loadingCircleLoading.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_125);
                    loadingCircleLoading.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_125);
                    break;
                case 150:
                    //Init loading circles parameters for 150% scaling
                    loadingCircleLoading.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_150);
                    loadingCircleLoading.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_150);
                    loadingCircleLoading.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_150);
                    loadingCircleLoading.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_150);
                    break;
                case 175:
                    //Init loading circles parameters for 175% scaling
                    loadingCircleLoading.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_175);
                    loadingCircleLoading.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_175);
                    loadingCircleLoading.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_175);
                    loadingCircleLoading.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_175);
                    break;
                case 200:
                    //Init loading circles parameters for 200% scaling
                    loadingCircleLoading.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_200);
                    loadingCircleLoading.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_200);
                    loadingCircleLoading.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_200);
                    loadingCircleLoading.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_200);
                    break;
                case 225:
                    //Init loading circles parameters for 225% scaling
                    loadingCircleLoading.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_225);
                    loadingCircleLoading.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_225);
                    loadingCircleLoading.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_225);
                    loadingCircleLoading.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_225);
                    break;
                case 250:
                    //Init loading circles parameters for 250% scaling
                    loadingCircleLoading.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_250);
                    loadingCircleLoading.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_250);
                    loadingCircleLoading.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_250);
                    loadingCircleLoading.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_250);
                    break;
                case 300:
                    //Init loading circles parameters for 300% scaling
                    loadingCircleLoading.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_300);
                    loadingCircleLoading.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_300);
                    loadingCircleLoading.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_300);
                    loadingCircleLoading.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_300);
                    break;
                case 350:
                    //Init loading circles parameters for 350% scaling
                    loadingCircleLoading.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_NUMBER_SPOKE_350);
                    loadingCircleLoading.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_SPOKE_THICKNESS_350);
                    loadingCircleLoading.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_INNER_RADIUS_350);
                    loadingCircleLoading.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_OUTER_RADIUS_350);
                    break;
            }

            loadingCircleLoading.RotationSpeed = Convert.ToInt32(ConstantsDLL.Properties.Resources.ROTATING_CIRCLE_ROTATION_SPEED);
            loadingCircleLoading.Color = StringsAndConstants.ROTATING_CIRCLE_COLOR;

            UserPreferenceChanged = new UserPreferenceChangedEventHandler(SystemEvents_UserPreferenceChanged);
            SystemEvents.UserPreferenceChanged += UserPreferenceChanged;
            Disposed += new EventHandler(LoginForm_Disposed);
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
        private void LoginForm_Disposed(object sender, EventArgs e)
        {
            SystemEvents.UserPreferenceChanged -= UserPreferenceChanged;
        }
    }
}
