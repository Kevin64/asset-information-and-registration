using System.Windows.Forms;
using ConstantsDLL;

namespace HardwareInformation
{
    public partial class BusyWindow : Form
    {
		public BusyWindow()
        {
            InitializeComponent();
			if(MiscMethods.ThemeInit())
            {
				this.label1.ForeColor = StringsAndConstants.DARK_FORECOLOR;
				this.BackColor = StringsAndConstants.DARK_BACKGROUND;
			}
            else
            {
				this.label1.ForeColor = StringsAndConstants.LIGHT_FORECOLOR;
				this.BackColor = StringsAndConstants.LIGHT_BACKGROUND;
			}
        }
	}
}
