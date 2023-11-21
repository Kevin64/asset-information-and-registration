using AssetInformationAndRegistration.Forms;
using ConstantsDLL.Properties;
using Dark.Net;
using HardwareInfoDLL;
using LogGeneratorDLL;
using Octokit;
using System;
using System.Net;
using System.Net.Http;
using System.Windows.Forms;

namespace AssetInformationAndRegistration.Updater
{
    /// <summary> 
    /// Template class for 'Updater'
    /// </summary>
    public class UpdateInfo
    {
        public string ETag { get; set; }
        public string TagName { get; set; }
        public string Body { get; set; }
        public string HtmlUrl { get; set; }
    }
    /// <summary> 
    /// Class for handling update checking tasks and UI
    /// </summary>
    public static class UpdateChecker
    {
        private static HttpClient httpHeader;
        private static HttpRequestMessage request;
        private static HttpResponseMessage response;
        private static Release releases;
        private static UpdateInfo ui;

        /// <summary> 
        /// Checks for updates on GitHub repo
        /// </summary>
        /// <param name="client">Octokit GitHub object</param>
        /// <param name="log">Log file object</param>
        /// <param name="parametersList">List containing data from [Parameters]</param>
        /// <param name="isSystemDarkModeEnabled">Theme mode</param>
        /// <param name="autoCheck">Toggle for update autocheck</param>
        public static async void Check(GitHubClient client, LogGenerator log, Program.Definitions definitions, bool autoCheck, bool manualCheck, bool cliMode, bool isSystemDarkModeEnabled)
        {
            try
            {
                if (autoCheck || manualCheck)
                {
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_MISC), LogStrings.LOG_CONNECTING_GITHUB, string.Empty, cliMode);

                    httpHeader = new HttpClient();
                    if (HardwareInfo.GetWinVersion() == GenericResources.WIN_7_NAMENUM)
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    request = new HttpRequestMessage(HttpMethod.Head, GenericResources.GITHUB_AIR_API_URL);
                    request.Headers.Add("User-Agent", "Other");
                    ui = Misc.MiscMethods.RegCheckUpdateData();
                    if (ui != null)
                        request.Headers.Add("If-None-Match", "\"" + ui.ETag + "\"");
                    response = await httpHeader.SendAsync(request);
                    if (!Convert.ToInt32(response.StatusCode).Equals(304))
                    {
                        releases = await client.Repository.Release.GetLatest(GenericResources.GITHUB_OWNER_AIR, GenericResources.GITHUB_REPO_AIR);
                        ui = new UpdateInfo
                        {
                            ETag = response.Headers.ETag.ToString().Substring(3, response.Headers.ETag.ToString().Length - 4),
                            TagName = releases.TagName,
                            Body = releases.Body,
                            HtmlUrl = releases.HtmlUrl
                        };
                        Misc.MiscMethods.RegCreateUpdateData(ui);
                    }
                    else
                    {
                        ui = new UpdateInfo
                        {
                            ETag = Misc.MiscMethods.RegCheckUpdateData().ETag,
                            TagName = Misc.MiscMethods.RegCheckUpdateData().TagName,
                            Body = Misc.MiscMethods.RegCheckUpdateData().Body,
                            HtmlUrl = Misc.MiscMethods.RegCheckUpdateData().HtmlUrl
                        };
                    }

                    if (!cliMode)
                    {
                        UpdaterForm uForm = new UpdaterForm(log, definitions, ui, isSystemDarkModeEnabled);
                        bool isNotUpdated = uForm.IsThereANewVersion();
                        if (HardwareInfo.GetWinVersion().Equals(GenericResources.WIN_10_NAMENUM))
                            DarkNet.Instance.SetWindowThemeForms(uForm, Theme.Auto);
                        if ((autoCheck && isNotUpdated) || manualCheck)
                            _ = uForm.ShowDialog();
                    }
                    else
                    {
                        string newVersion, changelog, url, currentVersion;
                        newVersion = ui.TagName;
                        changelog = ui.Body;
                        url = ui.HtmlUrl;
                        currentVersion = Misc.MiscMethods.Version();

                        switch (newVersion.CompareTo(currentVersion))
                        {
                            case 1:
                                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_MISC), UIStrings.NEW_VERSION_AVAILABLE, newVersion, Convert.ToBoolean(GenericResources.CONSOLE_OUT_CLI));
                                break;
                            default:
                                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_MISC), UIStrings.NO_NEW_VERSION_AVAILABLE, string.Empty, Convert.ToBoolean(GenericResources.CONSOLE_OUT_CLI));
                                break;
                        }
                    }
                }
            }
            catch (Exception e) when (e is ApiException || e is HttpRequestException)
            {
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_MISC), LogStrings.LOG_GITHUB_UNREACHABLE, e.Message, cliMode);
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_MISC), LogStrings.LOG_UPDATE_CHECK_IMPOSSIBLE, string.Empty, cliMode);
                _ = MessageBox.Show(LogStrings.LOG_UPDATE_CHECK_IMPOSSIBLE, UIStrings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
