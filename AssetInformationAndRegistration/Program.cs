using AssetInformationAndRegistration.Forms;
using AssetInformationAndRegistration.Properties;
using AssetInformationAndRegistration.Updater;
using CommandLine;
using ConstantsDLL;
using ConstantsDLL.Properties;
using Dark.Net;
using HardwareInfoDLL;
using LogGeneratorDLL;
using Newtonsoft.Json;
using Octokit;
using RestApiDLL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Application = System.Windows.Forms.Application;

namespace AssetInformationAndRegistration
{
    /// <summary> 
    /// Starting class for AIR
    /// </summary>
    public partial class Program
    {
        private static bool showCLIOutput;
        private static string jsonFile;

        private static Agent agent;
        private static ConfigurationOptions configOptions;
        private static Definitions definitions;
        private static Enforcement enforcement;
        private static GitHubClient ghc;
        private static HttpClient client;
        private static LogGenerator log;
        private static OrgData orgData;
        private static StreamReader fileC;

        /// <summary> 
        /// Command line switch options specification
        /// </summary>
        private class Options
        {
            [Option(StringsAndConstants.CLI_SERVER_IP_SWITCH, Required = false, HelpText = StringsAndConstants.CLI_HELP_TEXT_SERVER_IP)]
            public string ServerIP { get; set; }

            [Option(StringsAndConstants.CLI_SERVER_PORT_SWITCH, Required = false, HelpText = StringsAndConstants.CLI_HELP_TEXT_SERVER_PORT)]
            public string ServerPort { get; set; }

            [Option(StringsAndConstants.CLI_ASSET_NUMBER_SWITCH, Required = false, HelpText = StringsAndConstants.CLI_HELP_TEXT_ASSET_NUMBER, Default = "")]
            public string AssetNumber { get; set; }

            [Option(StringsAndConstants.CLI_BUILDING_SWITCH, Required = false, HelpText = StringsAndConstants.CLI_HELP_TEXT_BUILDING, Default = StringsAndConstants.CLI_DEFAULT_UNCHANGED)]
            public string Building { get; set; }

            [Option(StringsAndConstants.CLI_ROOM_NUMBER_SWITCH, Required = false, HelpText = StringsAndConstants.CLI_HELP_TEXT_ROOM_NUMBER, Default = StringsAndConstants.CLI_DEFAULT_UNCHANGED)]
            public string RoomNumber { get; set; }

            [Option(StringsAndConstants.CLI_SERVICE_DATE_SWITCH, Required = false, HelpText = StringsAndConstants.CLI_HELP_TEXT_SERVICE_DATE, Default = StringsAndConstants.CLI_DEFAULT_SERVICE_DATE)]
            public string ServiceDate { get; set; }

            [Option(StringsAndConstants.CLI_SERVICE_TYPE_SWITCH, Required = false, HelpText = StringsAndConstants.CLI_HELP_TEXT_SERVICE_TYPE, Default = StringsAndConstants.CLI_DEFAULT_SERVICE_TYPE)]
            public string ServiceType { get; set; }

            [Option(StringsAndConstants.CLI_BATTERY_CHANGE_SWITCH, Required = true, HelpText = StringsAndConstants.CLI_HELP_TEXT_BATTERY_CHANGE)]
            public string BatteryChange { get; set; }

            [Option(StringsAndConstants.CLI_TICKET_NUMBER_SWITCH, Required = true, HelpText = StringsAndConstants.CLI_HELP_TEXT_TICKET_NUMBER)]
            public string TicketNumber { get; set; }

            [Option(StringsAndConstants.CLI_STANDARD_SWITCH, Required = false, HelpText = StringsAndConstants.CLI_HELP_TEXT_STANDARD, Default = StringsAndConstants.CLI_DEFAULT_UNCHANGED)]
            public string Standard { get; set; }

            [Option(StringsAndConstants.CLI_IN_USE_SWITCH, Required = false, HelpText = StringsAndConstants.CLI_HELP_TEXT_IN_USE, Default = StringsAndConstants.CLI_DEFAULT_UNCHANGED)]
            public string InUse { get; set; }

            [Option(StringsAndConstants.CLI_SEAL_NUMBER_SWITCH, Required = false, HelpText = StringsAndConstants.CLI_HELP_TEXT_SEAL_NUMBER, Default = StringsAndConstants.CLI_DEFAULT_UNCHANGED)]
            public string SealNumber { get; set; }

            [Option(StringsAndConstants.CLI_TAG_SWITCH, Required = false, HelpText = StringsAndConstants.CLI_HELP_TEXT_TAG, Default = StringsAndConstants.CLI_DEFAULT_UNCHANGED)]
            public string Tag { get; set; }

            [Option(StringsAndConstants.CLI_HW_TYPE_SWITCH, Required = false, HelpText = StringsAndConstants.CLI_HELP_TEXT_HW_TYPE, Default = StringsAndConstants.CLI_DEFAULT_UNCHANGED)]
            public string HwType { get; set; }

            [Option(StringsAndConstants.CLI_USERNAME_SWITCH, Required = true, HelpText = StringsAndConstants.CLI_HELP_TEXT_USERNAME)]
            public string Username { get; set; }

            [Option(StringsAndConstants.CLI_PASSWORD_SWITCH, Required = true, HelpText = StringsAndConstants.CLI_HELP_TEXT_PASSWORD)]
            public string Password { get; set; }
        }

        public class ConfigurationOptions
        {
            public Definitions Definitions { get; set; }
            public Enforcement Enforcement { get; set; }
            public OrgData OrgData { get; set; }
        }

        public class Definitions
        {
            public string LogLocation { get; set; }
            public List<string> ServerIP { get; set; }
            public List<string> ServerPort { get; set; }
            public string ThemeUI { get; set; }
        }

        public class Enforcement
        {
            public bool RamLimit { get; set; }
            public bool SmartStatus { get; set; }
            public bool MediaOperationMode { get; set; }
            public bool Hostname { get; set; }
            public bool FirmwareType { get; set; }
            public bool FirmwareVersion { get; set; }
            public bool SecureBoot { get; set; }
            public bool VirtualizationTechnology { get; set; }
            public bool Tpm { get; set; }
            public bool CheckForUpdates { get; set; }
        }

        public class OrgData
        {
            public string OrganizationFullName { get; set; }
            public string OrganizationAcronym { get; set; }
            public string DepartamentFullName { get; set; }
            public string DepartamentAcronym { get; set; }
            public string SubDepartamentFullName { get; set; }
            public string SubDepartamentAcronym { get; set; }
        }

        public enum SpecBinaryStates
        {
            DISABLED,
            ENABLED
        }

        /// <summary> 
        /// Passes args to auth method and then to register class, otherwise informs auth error and closes the program
        /// </summary>
        /// <param name="opts">Argument list</param>
        private static void RunOptions(Options opts)
        {
            if (opts.ServerIP == null)
                opts.ServerIP = configOptions.Definitions.ServerIP[0];
            if (opts.ServerPort == null)
                opts.ServerPort = configOptions.Definitions.ServerPort[0];

            client = new HttpClient
            {
                BaseAddress = new Uri(Resources.HTTP + opts.ServerIP + ":" + opts.ServerPort)
            };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Resources.HTTP_CONTENT_TYPE_JSON));

            log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_INIT_LOGIN, opts.Username, Convert.ToBoolean(Resources.CONSOLE_OUT_CLI));

            System.Threading.Tasks.Task<Agent> v = AuthenticationHandler.GetAgentAsync(client, Resources.HTTP + opts.ServerIP + ":" + opts.ServerPort + Resources.API_AGENT_URL + opts.Username);
            v.Wait();
            agent = v.Result;

            try
            {
                if (agent != null)
                {
                    string[] argsArray = { opts.ServerIP, opts.ServerPort, opts.AssetNumber, opts.Building, opts.RoomNumber, opts.ServiceDate, opts.ServiceType, opts.BatteryChange, opts.TicketNumber, opts.Standard, opts.InUse, opts.SealNumber, opts.Tag, opts.HwType };
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_LOGIN_SUCCESS, string.Empty, Convert.ToBoolean(Resources.CONSOLE_OUT_CLI));
                    CLIRegister cr = new CLIRegister(argsArray, agent, log, configOptions);
                    UpdateChecker.Check(ghc, log, configOptions.Definitions, configOptions.Enforcement.CheckForUpdates, false, true, true);
                }
                else
                {
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), AirStrings.AUTH_ERROR, string.Empty, Convert.ToBoolean(Resources.CONSOLE_OUT_CLI));
                    Environment.Exit(Convert.ToInt32(ExitCodes.ERROR));
                }
            }
            catch
            {
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), Strings.INTRANET_REQUIRED, string.Empty, Convert.ToBoolean(Resources.CONSOLE_OUT_CLI));
                Environment.Exit(Convert.ToInt32(ExitCodes.ERROR));
            }
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern int FreeConsole();

        /// <summary> 
        /// The main entry point for the application
        /// </summary>
        /// <param name="args">Argument list</param>
        [STAThread]
        private static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //Code for testing string localization for other languages
            //var culture = System.Globalization.CultureInfo.GetCultureInfo("en");
            //System.Globalization.CultureInfo.DefaultThreadCurrentCulture = culture;
            //System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = culture;

            ghc = new GitHubClient(new Octokit.ProductHeaderValue(Resources.GITHUB_REPO_AIR));

            //Check if application is running
            if (Process.GetProcessesByName(Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location)).Count() > 1)
            {
                MessageBox.Show(Strings.ALREADY_RUNNING, Strings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Process.GetCurrentProcess().Kill();
            }

            string[] argsLog = new string[args.Length];

            try
            {
                fileC = new StreamReader(Resources.CONFIG_FILE);
                jsonFile = fileC.ReadToEnd();
                ConfigurationOptions jsonParse = JsonConvert.DeserializeObject<ConfigurationOptions>(@jsonFile);
                fileC.Close();

                //Creates 'Definitions' JSON section object
                definitions = new Definitions()
                {
                    LogLocation = jsonParse.Definitions.LogLocation,
                    ServerIP = jsonParse.Definitions.ServerIP,
                    ServerPort = jsonParse.Definitions.ServerPort,
                    ThemeUI = jsonParse.Definitions.ThemeUI,
                };
                //Creates 'Enforcement' JSON section object
                enforcement = new Enforcement()
                {
                    RamLimit = jsonParse.Enforcement.RamLimit,
                    SmartStatus = jsonParse.Enforcement.SmartStatus,
                    MediaOperationMode = jsonParse.Enforcement.MediaOperationMode,
                    Hostname = jsonParse.Enforcement.FirmwareType,
                    FirmwareType = jsonParse.Enforcement.FirmwareType,
                    FirmwareVersion = jsonParse.Enforcement.FirmwareVersion,
                    SecureBoot = jsonParse.Enforcement.VirtualizationTechnology,
                    VirtualizationTechnology = jsonParse.Enforcement.VirtualizationTechnology,
                    Tpm = jsonParse.Enforcement.Tpm,
                    CheckForUpdates = jsonParse.Enforcement.CheckForUpdates
                };
                //Creates 'OrgData' JSON section object
                orgData = new OrgData()
                {
                    OrganizationFullName = jsonParse.OrgData.OrganizationFullName,
                    OrganizationAcronym = jsonParse.OrgData.OrganizationAcronym,
                    DepartamentFullName = jsonParse.OrgData.DepartamentFullName,
                    DepartamentAcronym = jsonParse.OrgData.DepartamentAcronym,
                    SubDepartamentFullName = jsonParse.OrgData.SubDepartamentFullName,
                    SubDepartamentAcronym = jsonParse.OrgData.SubDepartamentAcronym
                };
                //Creates general JSON structure object
                configOptions = new ConfigurationOptions()
                {
                    Definitions = definitions,
                    Enforcement = enforcement,
                    OrgData = orgData
                };

                //Check if theme set in parameters file is valid
                if (!StringsAndConstants.LIST_THEME_GUI.Contains(configOptions.Definitions.ThemeUI))
                    throw new FormatException();

                //Check if log file exists
                bool fileExists = bool.Parse(Misc.MiscMethods.CheckIfLogExists(configOptions.Definitions.LogLocation));

                //If args has a --help switch, do not show log output
                if (!args.Contains(Resources.DOUBLE_DASH + StringsAndConstants.CLI_HELP_SWITCH))
                    showCLIOutput = true;
#if DEBUG
                //Create a new log file (or append to a existing one)
                log = new LogGenerator(Application.ProductName + " - v" + Application.ProductVersion + "-" + AirResources.DEV_STATUS, configOptions.Definitions.LogLocation, Resources.LOG_FILENAME_AIR + "-v" + Application.ProductVersion + "-" + AirResources.DEV_STATUS + Resources.LOG_FILE_EXT, showCLIOutput);
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_DEBUG_MODE, string.Empty, showCLIOutput);
#else
                //Create a new log file (or append to a existing one)
                log = new LogGenerator(Application.ProductName + " - v" + Application.ProductVersion, configOptions.Definitions.LogLocation, ConstantsDLL.Properties.Resources.LOG_FILENAME_AIR + "-v" + Application.ProductVersion + ConstantsDLL.Properties.Resources.LOG_FILE_EXT, showCLIOutput);
                log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_RELEASE_MODE, string.Empty, showCLIOutput);
#endif
                if (!fileExists)
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOGFILE_NOTEXISTS, string.Empty, showCLIOutput);
                else
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), Strings.LOGFILE_EXISTS, string.Empty, showCLIOutput);

                //If given no args, runs LoginForm
                if (args.Length == 0)
                {
                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_GUI_MODE, string.Empty, showCLIOutput);

                    FreeConsole();

                    bool isSystemDarkModeEnabled = Misc.MiscMethods.GetSystemThemeMode();
                    (int themeFileSet, bool _) = Misc.MiscMethods.GetFileThemeMode(configOptions.Definitions, isSystemDarkModeEnabled);
                    bool initialTheme = false;

                    Form lForm = new LoginForm(ghc, log, configOptions, isSystemDarkModeEnabled);

                    if (HardwareInfo.GetWinVersion().Equals(Resources.WINDOWS_10))
                    {
                        switch (themeFileSet)
                        {
                            case 0:
                                DarkNet.Instance.SetWindowThemeForms(lForm, Theme.Light);
                                initialTheme = false;
                                break;
                            case 1:
                                DarkNet.Instance.SetWindowThemeForms(lForm, Theme.Dark);
                                initialTheme = true;
                                break;
                        }
                    }
                    UpdateChecker.Check(ghc, log, configOptions.Definitions, configOptions.Enforcement.CheckForUpdates, false, false, initialTheme);
                    Application.Run(lForm);
                }
                else //If given args, hides password from Console and Log file and runs CLIRegister
                {
                    args.CopyTo(argsLog, 0);
                    int index = Array.IndexOf(argsLog, Resources.DOUBLE_DASH + StringsAndConstants.CLI_PASSWORD_SWITCH);
                    if (index == -1)
                    {
                        index = Array.FindIndex(argsLog, x => x.StartsWith(Resources.DOUBLE_DASH + StringsAndConstants.CLI_PASSWORD_SWITCH));
                        if (index != -1)
                            argsLog[index] = Resources.DOUBLE_DASH + StringsAndConstants.CLI_PASSWORD_SWITCH + "=" + Resources.LOG_PASSWORD_PLACEHOLDER;
                    }
                    else
                    {
                        argsLog[index + 1] = Resources.LOG_PASSWORD_PLACEHOLDER;
                    }

                    log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_CLI_MODE, string.Join(" ", argsLog), showCLIOutput);

                    //Parses the args
                    Parser.Default.ParseArguments<Options>(args).WithParsed(RunOptions);

                    if (args.Length == 1 && args.Contains(Resources.DOUBLE_DASH + StringsAndConstants.CLI_HELP_SWITCH))
                    {
                        log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_INFO), LogStrings.LOG_SHOWING_HELP, string.Empty, showCLIOutput);
                        Environment.Exit(Convert.ToInt32(ExitCodes.SUCCESS));
                    }
                    else
                    {
                        log.LogWrite(Convert.ToInt32(LogGenerator.LOG_SEVERITY.LOG_ERROR), AirStrings.ARGS_ERROR, string.Empty, showCLIOutput);
                        Environment.Exit(Convert.ToInt32(ExitCodes.ERROR));
                    }
                }
            }
            //If config file was not found or is malformed
            catch (Exception e) when (e is JsonReaderException || e is JsonSerializationException || e is FormatException)
            {
                Console.WriteLine(Strings.PARAMETER_ERROR + ": " + e.Message);
                Console.WriteLine(Strings.KEY_FINISH);
                Console.ReadLine();
                Environment.Exit(Convert.ToInt32(ExitCodes.ERROR));
            }
            //If config file is not found
            catch(FileNotFoundException e)
            {
                Console.WriteLine(LogStrings.LOG_PARAMETER_FILE_NOT_FOUND + ": " + e.Message);
                Console.WriteLine(Strings.KEY_FINISH);
                Console.ReadLine();
                Environment.Exit(Convert.ToInt32(ExitCodes.ERROR));
            }
        }
    }
}
