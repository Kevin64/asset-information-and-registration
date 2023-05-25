using ConstantsDLL;
using System.Drawing;
using System.Windows.Forms;

namespace AssetInformationAndRegistration
{
    internal interface ITheming
    {
        ///<summary>Sets a light theme for the UI</summary>
        void LightTheme();

        ///<summary>Sets a dark theme for the UI</summary>
        void DarkTheme();
    }
}
