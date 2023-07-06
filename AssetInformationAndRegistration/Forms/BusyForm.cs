using AssetInformationAndRegistration.Interfaces;
using AssetInformationAndRegistration.Misc;
using ConstantsDLL;
using Dark.Net;
using HardwareInfoDLL;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace AssetInformationAndRegistration.Forms
{
    /// <summary> 
    /// Class for the Busy form
    /// </summary>
    internal partial class BusyForm : Form, ITheming
    {
        private UserPreferenceChangedEventHandler UserPreferenceChanged;

        /// <summary> 
        /// Busy form constructor
        /// </summary>
        /// <param name="themeBool">Theme mode</param>
        internal BusyForm(List<string[]> parametersList, bool themeBool)
        {
            InitializeComponent();

            int themeFileSet = MiscMethods.GetFileThemeMode(parametersList, themeBool);
            switch (themeFileSet)
            {
                case 0:
                    DarkTheme();
                    break;
                case 1:
                    LightTheme();
                    break;
                case 2:
                    LightTheme();
                    break;
                case 3:
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
            this.Disposed += new EventHandler(LoginForm_Disposed);
        }

        public void LightTheme()
        {
            lblFixedLoading.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
            BackColor = StringsAndConstants.LIGHT_BACKGROUND;

            if (HardwareInfo.GetWinVersion().Equals(ConstantsDLL.Properties.Resources.WINDOWS_10))
            {
                DarkNet.Instance.SetCurrentProcessTheme(Theme.Light);
            }
        }

        public void DarkTheme()
        {
            lblFixedLoading.ForeColor = StringsAndConstants.DARK_FORECOLOR;
            BackColor = StringsAndConstants.DARK_BACKGROUND;

            if (HardwareInfo.GetWinVersion().Equals(ConstantsDLL.Properties.Resources.WINDOWS_10))
            {
                DarkNet.Instance.SetCurrentProcessTheme(Theme.Dark);
            }
        }

        /// <summary> 
        /// Method for auto selecting the app theme
        /// </summary>
        private void ToggleTheme()
        {
            if (MiscMethods.GetSystemThemeMode())
            {
                DarkTheme();
            }
            else
            {
                LightTheme();
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
            {
                ToggleTheme();
            }
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
