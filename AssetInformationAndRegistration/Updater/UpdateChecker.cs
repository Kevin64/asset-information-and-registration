using AssetInformationAndRegistration.Forms;
using Dark.Net;
using HardwareInfoDLL;
using LogGeneratorDLL;
using Octokit;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace AssetInformationAndRegistration.Updater
{
    ///<summary>Class for handling update checking tasks and UI</summary>
    internal static class UpdateChecker
    {
        ///<summary>Checks for updates on GitHub repo</summary>
        ///<param name="parametersList">List containing data from [Parameters]</param>
        ///<param name="themeBool">Theme mode</param>
        internal static async void Check(LogGenerator log, List<string[]> parametersList, bool themeBool, bool autoCheck)
        {
            try
            {
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), ConstantsDLL.Properties.Strings.LOG_CONNECTING_GITHUB, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
                GitHubClient client = new GitHubClient(new ProductHeaderValue(ConstantsDLL.Properties.Resources.GITHUB_REPO_AIR));
                Release releases = await client.Repository.Release.GetLatest(ConstantsDLL.Properties.Resources.GITHUB_OWNER_AIR, ConstantsDLL.Properties.Resources.GITHUB_REPO_AIR);

                UpdateCheckerForm uForm = new UpdateCheckerForm(log, parametersList, themeBool, releases);
                bool isUpdated = uForm.IsThereANewVersion();
                if (HardwareInfo.GetWinVersion().Equals(ConstantsDLL.Properties.Resources.WINDOWS_10))
                {
                    DarkNet.Instance.SetWindowThemeForms(uForm, Theme.Auto);
                }
                if((!isUpdated && !autoCheck) || isUpdated)
                {
                    _ = uForm.ShowDialog();
                }
            }
            catch (ApiException e)
            {
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), ConstantsDLL.Properties.Strings.LOG_GITHUB_UNREACHABLE, e.Message, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
                _ = MessageBox.Show(ConstantsDLL.Properties.Strings.LOG_UPDATE_CHECK_IMPOSSIBLE, ConstantsDLL.Properties.Strings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception e)
            {
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_WARNING), ConstantsDLL.Properties.Strings.LOG_NO_INTERNET_AVAILABLE, e.Message, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
                _ = MessageBox.Show(ConstantsDLL.Properties.Strings.LOG_NO_INTERNET_AVAILABLE, ConstantsDLL.Properties.Strings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
