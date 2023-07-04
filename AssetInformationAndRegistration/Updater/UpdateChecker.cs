using AssetInformationAndRegistration.Forms;
using Dark.Net;
using HardwareInfoDLL;
using LogGeneratorDLL;
using Octokit;
using System;
using System.Collections.Generic;
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
    internal static class UpdateChecker
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
        /// <param name="themeBool">Theme mode</param>
        /// <param name="autoCheck">Toggle for update autocheck</param>
        internal static async void Check(GitHubClient client, LogGenerator log, List<string[]> parametersList, bool themeBool, bool autoCheck)
        {
            try
            {
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), ConstantsDLL.Properties.Strings.LOG_CONNECTING_GITHUB, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));

                httpHeader = new HttpClient();
                if (HardwareInfo.GetWinVersion() == ConstantsDLL.Properties.Resources.WINDOWS_7)
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                }
                request = new HttpRequestMessage(HttpMethod.Head, ConstantsDLL.Properties.Resources.AIR_API_URL);
                request.Headers.Add("User-Agent", "Other");
                ui = Misc.MiscMethods.RegCheckUpdateData();
                if (ui != null)
                {
                    request.Headers.Add("If-None-Match", "\"" + ui.ETag + "\"");
                }
                response = await httpHeader.SendAsync(request);
                if (!((int)response.StatusCode).Equals(304))
                {
                    releases = await client.Repository.Release.GetLatest(ConstantsDLL.Properties.Resources.GITHUB_OWNER_AIR, ConstantsDLL.Properties.Resources.GITHUB_REPO_AIR);
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

                UpdateCheckerForm uForm = new UpdateCheckerForm(log, parametersList, themeBool, ui);
                bool isUpdated = uForm.IsThereANewVersion();
                if (HardwareInfo.GetWinVersion().Equals(ConstantsDLL.Properties.Resources.WINDOWS_10))
                {
                    DarkNet.Instance.SetWindowThemeForms(uForm, Theme.Auto);
                }
                if ((!isUpdated && !autoCheck) || isUpdated)
                {
                    _ = uForm.ShowDialog();
                }
            }
            catch (Exception e) when (e is ApiException || e is HttpRequestException)
            {
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), ConstantsDLL.Properties.Strings.LOG_GITHUB_UNREACHABLE, e.Message, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_GUI));
                _ = MessageBox.Show(ConstantsDLL.Properties.Strings.LOG_UPDATE_CHECK_IMPOSSIBLE, ConstantsDLL.Properties.Strings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
