using System.Drawing;
using System.Windows.Forms;

namespace HardwareInformation
{
    public partial class BusyWindow : Form
    {
		private Color LIGHT_FORECOLOR = SystemColors.ControlText;
		private Color DARK_FORECOLOR = SystemColors.ControlLightLight;
		private Color LIGHT_BACKGROUND = Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
		private Color DARK_BACKGROUND = Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));

		public BusyWindow()
        {
            InitializeComponent();
			bool themeDark = MiscMethods.ThemeInit();
			if(themeDark)
            {
				this.label1.ForeColor = DARK_FORECOLOR;
				this.BackColor = DARK_BACKGROUND;
			}
            else
            {
				this.label1.ForeColor = LIGHT_FORECOLOR;
				this.BackColor = LIGHT_BACKGROUND;
			}
        }
	}
}
