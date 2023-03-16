using ConstantsDLL;
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
                this.label1.ForeColor = StringsAndConstants.DARK_FORECOLOR;
                this.BackColor = StringsAndConstants.DARK_BACKGROUND;
            }
            else
            {
                this.label1.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
                this.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
            }

            loadingCircle1.Enabled = true;
            loadingCircle1.Active = true;

            switch (MiscMethods.GetWindowsScaling())
            {
                case 100:
                    //Init loading circles parameters for 100% scaling
                    loadingCircle1.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke100;
                    loadingCircle1.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness100;
                    loadingCircle1.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius100;
                    loadingCircle1.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius100;
                    break;
                case 125:
                    //Init loading circles parameters for 125% scaling
                    loadingCircle1.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke125;
                    loadingCircle1.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness125;
                    loadingCircle1.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius125;
                    loadingCircle1.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius125;
                    break;
                case 150:
                    //Init loading circles parameters for 150% scaling
                    loadingCircle1.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke150;
                    loadingCircle1.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness150;
                    loadingCircle1.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius150;
                    loadingCircle1.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius150;
                    break;
                case 175:
                    //Init loading circles parameters for 175% scaling
                    loadingCircle1.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke175;
                    loadingCircle1.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness175;
                    loadingCircle1.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius175;
                    loadingCircle1.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius175;
                    break;
                case 200:
                    //Init loading circles parameters for 200% scaling
                    loadingCircle1.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke200;
                    loadingCircle1.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness200;
                    loadingCircle1.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius200;
                    loadingCircle1.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius200;
                    break;
                case 225:
                    //Init loading circles parameters for 225% scaling
                    loadingCircle1.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke225;
                    loadingCircle1.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness225;
                    loadingCircle1.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius225;
                    loadingCircle1.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius225;
                    break;
                case 250:
                    //Init loading circles parameters for 250% scaling
                    loadingCircle1.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke250;
                    loadingCircle1.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness250;
                    loadingCircle1.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius250;
                    loadingCircle1.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius250;
                    break;
                case 300:
                    //Init loading circles parameters for 300% scaling
                    loadingCircle1.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke300;
                    loadingCircle1.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness300;
                    loadingCircle1.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius300;
                    loadingCircle1.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius300;
                    break;
                case 350:
                    //Init loading circles parameters for 350% scaling
                    loadingCircle1.NumberSpoke = StringsAndConstants.rotatingCircleNumberSpoke350;
                    loadingCircle1.SpokeThickness = StringsAndConstants.rotatingCircleSpokeThickness350;
                    loadingCircle1.InnerCircleRadius = StringsAndConstants.rotatingCircleInnerCircleRadius350;
                    loadingCircle1.OuterCircleRadius = StringsAndConstants.rotatingCircleOuterCircleRadius350;
                    break;
            }

            loadingCircle1.RotationSpeed = StringsAndConstants.rotatingCircleRotationSpeed;
            loadingCircle1.Color = StringsAndConstants.rotatingCircleColor;
        }
    }
}
