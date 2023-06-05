using AssetInformationAndRegistration.Forms;
using Dark.Net;
using HardwareInfoDLL;
using Octokit;
using System.Collections.Generic;

namespace AssetInformationAndRegistration.Updater
{
    ///<summary>Class for handling update checking tasks and UI</summary>
    internal static class UpdateChecker
    {
        ///<summary>Checks for updates on GitHub repo</summary>
        ///<param name="parametersList">List containing data from [Parameters]</param>
        ///<param name="themeBool">Theme mode</param>
        internal static async void Check(List<string[]> parametersList, bool themeBool)
        {
            GitHubClient client = new GitHubClient(new ProductHeaderValue(ConstantsDLL.Properties.Resources.GITHUB_REPO_AIR));
            Release releases = await client.Repository.Release.GetLatest(ConstantsDLL.Properties.Resources.GITHUB_OWNER_AIR, ConstantsDLL.Properties.Resources.GITHUB_REPO_AIR);

            UpdateCheckerForm uForm = new UpdateCheckerForm(parametersList, themeBool, releases);
            if (HardwareInfo.GetWinVersion().Equals(ConstantsDLL.Properties.Resources.WINDOWS_10))
            {
                DarkNet.Instance.SetWindowThemeForms(uForm, Theme.Auto);
            }
            _ = uForm.ShowDialog();
        }
    }
}
