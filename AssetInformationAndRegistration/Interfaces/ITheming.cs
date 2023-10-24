namespace AssetInformationAndRegistration.Interfaces
{
    internal interface ITheming
    {
        /// <summary> 
        /// Sets a light theme for specific controls in the UI
        /// </summary>
        void LightThemeSpecificControls();

        /// <summary> 
        /// Sets a dark theme for specific controls in the UI
        /// </summary>
        void DarkThemeSpecificControls();
    }
}
