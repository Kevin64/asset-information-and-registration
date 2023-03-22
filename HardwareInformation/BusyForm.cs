using ConstantsDLL;
using System;
using System.Windows.Forms;

namespace HardwareInformation
{
    public partial class BusyForm : Form
    {
        public BusyForm()
        {
            InitializeComponent();
            if (MiscMethods.ThemeInit())
            {
                label1.ForeColor = StringsAndConstants.DARK_FORECOLOR;
                BackColor = StringsAndConstants.DARK_BACKGROUND;
            }
            else
            {
                label1.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
                BackColor = StringsAndConstants.LIGHT_BACKGROUND;
            }

            loadingCircle1.Enabled = true;
            loadingCircle1.Active = true;

            switch (MiscMethods.GetWindowsScaling())
            {
                case 100:
                    //Init loading circles parameters for 100% scaling
                    loadingCircle1.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke100);
                    loadingCircle1.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness100);
                    loadingCircle1.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius100);
                    loadingCircle1.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius100);
                    break;
                case 125:
                    //Init loading circles parameters for 125% scaling
                    loadingCircle1.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke125);
                    loadingCircle1.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness125);
                    loadingCircle1.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius125);
                    loadingCircle1.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius125);
                    break;
                case 150:
                    //Init loading circles parameters for 150% scaling
                    loadingCircle1.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke150);
                    loadingCircle1.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness150);
                    loadingCircle1.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius150);
                    loadingCircle1.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius150);
                    break;
                case 175:
                    //Init loading circles parameters for 175% scaling
                    loadingCircle1.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke175);
                    loadingCircle1.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness175);
                    loadingCircle1.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius175);
                    loadingCircle1.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius175);
                    break;
                case 200:
                    //Init loading circles parameters for 200% scaling
                    loadingCircle1.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke200);
                    loadingCircle1.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness200);
                    loadingCircle1.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius200);
                    loadingCircle1.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius200);
                    break;
                case 225:
                    //Init loading circles parameters for 225% scaling
                    loadingCircle1.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke225);
                    loadingCircle1.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness225);
                    loadingCircle1.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius225);
                    loadingCircle1.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius225);
                    break;
                case 250:
                    //Init loading circles parameters for 250% scaling
                    loadingCircle1.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke250);
                    loadingCircle1.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness250);
                    loadingCircle1.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius250);
                    loadingCircle1.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius250);
                    break;
                case 300:
                    //Init loading circles parameters for 300% scaling
                    loadingCircle1.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke300);
                    loadingCircle1.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness300);
                    loadingCircle1.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius300);
                    loadingCircle1.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius300);
                    break;
                case 350:
                    //Init loading circles parameters for 350% scaling
                    loadingCircle1.NumberSpoke = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleNumberSpoke350);
                    loadingCircle1.SpokeThickness = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleSpokeThickness350);
                    loadingCircle1.InnerCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleInnerCircleRadius350);
                    loadingCircle1.OuterCircleRadius = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleOuterCircleRadius350);
                    break;
            }

            loadingCircle1.RotationSpeed = Convert.ToInt32(ConstantsDLL.Properties.Resources.rotatingCircleRotationSpeed);
            loadingCircle1.Color = StringsAndConstants.rotatingCircleColor;
        }
    }
}
