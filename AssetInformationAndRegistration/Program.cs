using AssetInformationAndRegistration.Properties;
using CommandLine;
using ConstantsDLL;
using Dark.Net;
using HardwareInfoDLL;
using IniParser;
using IniParser.Exceptions;
using IniParser.Model;
using JsonFileReaderDLL;
using LogGeneratorDLL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace AssetInformationAndRegistration
{
    ///<summary>Starting class for AIR</summary>
    public class Program
    {
        private static string logLocationStr, serverIPStr, serverPortStr, themeStr, secureBootEnforcementStr, vtEnforcementStr, tpmEnforcementStr, firmwareVersionEnforcementStr, firmwareTypeEnforcementStr, hostnameEnforcementStr, mediaOperationModeEnforcementStr, smartStatusEnforcementStr, ramLimitEnforcementStr, orgFullNameStr, orgAcronymStr, depFullNameStr, depAcronymStr, subDepFullNameStr, subDepAcronymStr;
        private static string[] logLocationSection, serverIPListSection, serverPortListSection, themeSection, buildings, hardwareTypes, firmwareTypes, tpmTypes, mediaOperationTypes, secureBootStates, virtualizationTechnologyStates;

        private static bool showCLIOutput;
        private static List<string[]> parametersListSection;
        private static List<string> orgDataListSection, enforcementListSection;
        private static LogGenerator log;

        ///<summary>Command line switch options specification</summary>
        public class Options
        {
            [Option(StringsAndConstants.CLI_SERVER_IP_SWITCH, Required = false, HelpText = StringsAndConstants.CLI_HELP_TEXT_SERVER_IP)]
            public string ServerIP { get; set; }

            [Option(StringsAndConstants.CLI_SERVER_PORT_SWITCH, Required = false, HelpText = StringsAndConstants.CLI_HELP_TEXT_SERVER_PORT)]
            public string ServerPort { get; set; }

            [Option(StringsAndConstants.CLI_SERVICE_TYPE_SWITCH, Required = false, HelpText = StringsAndConstants.CLI_HELP_TEXT_SERVICE_TYPE, Default = StringsAndConstants.CLI_DEFAULT_SERVICE_TYPE)]
            public string ServiceType { get; set; }

            [Option(StringsAndConstants.CLI_ASSET_NUMBER_SWITCH, Required = false, HelpText = StringsAndConstants.CLI_HELP_TEXT_ASSET_NUMBER, Default = "")]
            public string AssetNumber { get; set; }

            [Option(StringsAndConstants.CLI_SEAL_NUMBER_SWITCH, Required = false, HelpText = StringsAndConstants.CLI_HELP_TEXT_SEAL_NUMBER, Default = StringsAndConstants.CLI_DEFAULT_UNCHANGED)]
            public string SealNumber { get; set; }

            [Option(StringsAndConstants.CLI_ROOM_NUMBER_SWITCH, Required = false, HelpText = StringsAndConstants.CLI_HELP_TEXT_ROOM_NUMBER, Default = StringsAndConstants.CLI_DEFAULT_UNCHANGED)]
            public string RoomNumber { get; set; }

            [Option(StringsAndConstants.CLI_BUILDING_SWITCH, Required = false, HelpText = StringsAndConstants.CLI_HELP_TEXT_BUILDING, Default = StringsAndConstants.CLI_DEFAULT_UNCHANGED)]
            public string Building { get; set; }

            [Option(StringsAndConstants.CLI_AD_REGISTERED_SWITCH, Required = false, HelpText = StringsAndConstants.CLI_HELP_TEXT_AD_REGISTERED, Default = StringsAndConstants.CLI_DEFAULT_UNCHANGED)]
            public string AdRegistered { get; set; }

            [Option(StringsAndConstants.CLI_STANDARD_SWITCH, Required = false, HelpText = StringsAndConstants.CLI_HELP_TEXT_STANDARD, Default = StringsAndConstants.CLI_DEFAULT_UNCHANGED)]
            public string Standard { get; set; }

            [Option(StringsAndConstants.CLI_SERVICE_DATE_SWITCH, Required = false, HelpText = StringsAndConstants.CLI_HELP_TEXT_SERVICE_DATE, Default = StringsAndConstants.CLI_DEFAULT_SERVICE_DATE)]
            public string ServiceDate { get; set; }

            [Option(StringsAndConstants.CLI_BATTERY_CHANGE_SWITCH, Required = true, HelpText = StringsAndConstants.CLI_HELP_TEXT_BATTERY_CHANGE)]
            public string BatteryChange { get; set; }

            [Option(StringsAndConstants.CLI_TICKET_NUMBER_SWITCH, Required = true, HelpText = StringsAndConstants.CLI_HELP_TEXT_TICKET_NUMBER)]
            public string TicketNumber { get; set; }

            [Option(StringsAndConstants.CLI_IN_USE_SWITCH, Required = false, HelpText = StringsAndConstants.CLI_HELP_TEXT_IN_USE, Default = StringsAndConstants.CLI_DEFAULT_UNCHANGED)]
            public string InUse { get; set; }

            [Option(StringsAndConstants.CLI_TAG_SWITCH, Required = false, HelpText = StringsAndConstants.CLI_HELP_TEXT_TAG, Default = StringsAndConstants.CLI_DEFAULT_UNCHANGED)]
            public string Tag { get; set; }

            [Option(StringsAndConstants.CLI_HW_TYPE_SWITCH, Required = false, HelpText = StringsAndConstants.CLI_HELP_TEXT_HW_TYPE, Default = StringsAndConstants.CLI_DEFAULT_UNCHANGED)]
            public string HwType { get; set; }

            [Option(StringsAndConstants.CLI_USERNAME_SWITCH, Required = true, HelpText = StringsAndConstants.CLI_HELP_TEXT_USERNAME)]
            public string Username { get; set; }

            [Option(StringsAndConstants.CLI_PASSWORD_SWITCH, Required = true, HelpText = StringsAndConstants.CLI_HELP_TEXT_PASSWORD)]
            public string Password { get; set; }
        }

        ///<summary>Passes args to auth method and then to register class, otherwise informs auth error and closes the program</summary>
        ///<param name="opts">Argument list</param>
        public static void RunOptions(Options opts)
        {
            if (opts.ServerIP == null)
            {
                opts.ServerIP = serverIPListSection[0];
            }

            if (opts.ServerPort == null)
            {
                opts.ServerPort = serverPortListSection[0];
            }

            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), ConstantsDLL.Properties.Strings.LOG_INIT_LOGIN, opts.Username, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_CLI));
            string[] agentsJsonStr = CredentialsFileReader.FetchInfoST(opts.Username, opts.Password, opts.ServerIP, opts.ServerPort);
            try
            {
                if (agentsJsonStr[0] != ConstantsDLL.Properties.Resources.FALSE)
                {
                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), ConstantsDLL.Properties.Strings.LOG_LOGIN_SUCCESS, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_CLI));
                    Application.Run(new CLIRegister(opts.ServerIP, opts.ServerPort, opts.ServiceType, opts.AssetNumber, opts.SealNumber, opts.RoomNumber, opts.Building, opts.AdRegistered, opts.Standard, opts.ServiceDate, opts.BatteryChange, opts.TicketNumber, opts.InUse, opts.Tag, opts.HwType, agentsJsonStr, log, parametersListSection, enforcementListSection));
                }
                else
                {
                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_ERROR), Strings.AUTH_ERROR, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_CLI));
                    Environment.Exit(Convert.ToInt32(ConstantsDLL.Properties.Resources.RETURN_ERROR));
                }
            }
            catch
            {
                log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_ERROR), ConstantsDLL.Properties.Strings.INTRANET_REQUIRED, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.CONSOLE_OUT_CLI));
                Environment.Exit(Convert.ToInt32(ConstantsDLL.Properties.Resources.RETURN_ERROR));
            }
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern int FreeConsole();

        ///<summary>The main entry point for the application</summary>
        ///<param name="args">Argument list</param>
        [STAThread]
        private static void Main(string[] args)
        {
            //Code for testing string localization for other languages
            //var culture = System.Globalization.CultureInfo.GetCultureInfo("en");
            //System.Globalization.CultureInfo.DefaultThreadCurrentCulture = culture;
            //System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = culture;

            if (HardwareInfo.GetWinVersion().Equals(ConstantsDLL.Properties.Resources.WINDOWS_10))
            {
                DarkNet.Instance.SetCurrentProcessTheme(Theme.Auto);
            }
            //Check if application is running
            if (Process.GetProcessesByName(Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location)).Count() > 1)
            {
                MessageBox.Show(ConstantsDLL.Properties.Strings.ALREADY_RUNNING, ConstantsDLL.Properties.Strings.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Process.GetCurrentProcess().Kill();
            }

            string[] argsLog = new string[args.Length];

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                IniData def = null;
                FileIniDataParser parser = new FileIniDataParser();
                //Parses the INI file
                def = parser.ReadFile(ConstantsDLL.Properties.Resources.DEF_FILE, Encoding.UTF8);

                //Reads the INI file Parameters section
                logLocationStr = def[ConstantsDLL.Properties.Resources.INI_SECTION_1][ConstantsDLL.Properties.Resources.INI_SECTION_1_9];
                serverIPStr = def[ConstantsDLL.Properties.Resources.INI_SECTION_1][ConstantsDLL.Properties.Resources.INI_SECTION_1_11];
                serverPortStr = def[ConstantsDLL.Properties.Resources.INI_SECTION_1][ConstantsDLL.Properties.Resources.INI_SECTION_1_12];
                themeStr = def[ConstantsDLL.Properties.Resources.INI_SECTION_1][ConstantsDLL.Properties.Resources.INI_SECTION_1_15];

                logLocationSection = logLocationStr.Split().ToArray();
                serverIPListSection = serverIPStr.Split(',').ToArray();
                serverPortListSection = serverPortStr.Split(',').ToArray();
                themeSection = themeStr.Split().ToArray();

                //Reads the INI file Enforcement section
                ramLimitEnforcementStr = def[ConstantsDLL.Properties.Resources.INI_SECTION_2][ConstantsDLL.Properties.Resources.INI_SECTION_2_1];
                smartStatusEnforcementStr = def[ConstantsDLL.Properties.Resources.INI_SECTION_2][ConstantsDLL.Properties.Resources.INI_SECTION_2_2];
                mediaOperationModeEnforcementStr = def[ConstantsDLL.Properties.Resources.INI_SECTION_2][ConstantsDLL.Properties.Resources.INI_SECTION_2_3];
                hostnameEnforcementStr = def[ConstantsDLL.Properties.Resources.INI_SECTION_2][ConstantsDLL.Properties.Resources.INI_SECTION_2_4];
                firmwareTypeEnforcementStr = def[ConstantsDLL.Properties.Resources.INI_SECTION_2][ConstantsDLL.Properties.Resources.INI_SECTION_2_5];
                firmwareVersionEnforcementStr = def[ConstantsDLL.Properties.Resources.INI_SECTION_2][ConstantsDLL.Properties.Resources.INI_SECTION_2_6];
                secureBootEnforcementStr = def[ConstantsDLL.Properties.Resources.INI_SECTION_2][ConstantsDLL.Properties.Resources.INI_SECTION_2_7];
                vtEnforcementStr = def[ConstantsDLL.Properties.Resources.INI_SECTION_2][ConstantsDLL.Properties.Resources.INI_SECTION_2_8];
                tpmEnforcementStr = def[ConstantsDLL.Properties.Resources.INI_SECTION_2][ConstantsDLL.Properties.Resources.INI_SECTION_2_9];

                //Reads the INI file OrgData section
                orgFullNameStr = def[ConstantsDLL.Properties.Resources.INI_SECTION_3][ConstantsDLL.Properties.Resources.INI_SECTION_3_1];
                orgAcronymStr = def[ConstantsDLL.Properties.Resources.INI_SECTION_3][ConstantsDLL.Properties.Resources.INI_SECTION_3_2];
                depFullNameStr = def[ConstantsDLL.Properties.Resources.INI_SECTION_3][ConstantsDLL.Properties.Resources.INI_SECTION_3_3];
                depAcronymStr = def[ConstantsDLL.Properties.Resources.INI_SECTION_3][ConstantsDLL.Properties.Resources.INI_SECTION_3_4];
                subDepFullNameStr = def[ConstantsDLL.Properties.Resources.INI_SECTION_3][ConstantsDLL.Properties.Resources.INI_SECTION_3_5];
                subDepAcronymStr = def[ConstantsDLL.Properties.Resources.INI_SECTION_3][ConstantsDLL.Properties.Resources.INI_SECTION_3_6];

                //Assigns null to allow to pass as argument and be filled later
                buildings = hardwareTypes = firmwareTypes = tpmTypes = mediaOperationTypes = secureBootStates = virtualizationTechnologyStates = null;

                if (!StringsAndConstants.LIST_THEME_GUI.Contains(themeSection[0]))
                {
                    throw new FormatException();
                }

                //[Parameters] ini section
                parametersListSection = new List<string[]>
                {
                    serverIPListSection,
                    serverPortListSection,
                    logLocationSection,
                    themeSection,
                    buildings,
                    hardwareTypes,
                    firmwareTypes,
                    tpmTypes,
                    mediaOperationTypes,
                    secureBootStates,
                    virtualizationTechnologyStates
                };

                //[Enforcement] ini section
                enforcementListSection = new List<string>
                {
                    ramLimitEnforcementStr,
                    smartStatusEnforcementStr,
                    mediaOperationModeEnforcementStr,
                    hostnameEnforcementStr,
                    firmwareTypeEnforcementStr,
                    firmwareVersionEnforcementStr,
                    secureBootEnforcementStr,
                    vtEnforcementStr,
                    tpmEnforcementStr
                };

                //[OrgData] ini section
                orgDataListSection = new List<string>
                {
                    orgFullNameStr,
                    orgAcronymStr,
                    depFullNameStr,
                    depAcronymStr,
                    subDepFullNameStr,
                    subDepAcronymStr
                };

                //Checks if [Enforcement] bool values are valid
                foreach (string s in enforcementListSection)
                {
                    try
                    {
                        bool.Parse(s);
                    }
                    catch
                    {
                        throw new FormatException();
                    }
                }

                bool fileExists = bool.Parse(MiscMethods.CheckIfLogExists(logLocationStr));

                //If args has a --help switch, do not show log output
                if (!args.Contains(ConstantsDLL.Properties.Resources.DOUBLE_DASH + StringsAndConstants.CLI_HELP_SWITCH))
                {
                    showCLIOutput = true;
                }
#if DEBUG
                //Create a new log file (or append to a existing one)
                log = new LogGenerator(Application.ProductName + " - v" + Application.ProductVersion + "-" + Resources.DEV_STATUS, logLocationStr, ConstantsDLL.Properties.Resources.LOG_FILENAME_CP + "-v" + Application.ProductVersion + "-" + Resources.DEV_STATUS + ConstantsDLL.Properties.Resources.LOG_FILE_EXT, showCLIOutput);
                log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), ConstantsDLL.Properties.Strings.LOG_DEBUG_MODE, string.Empty, showCLIOutput);
#else
                //Create a new log file (or append to a existing one)
                log = new LogGenerator(Application.ProductName + " - v" + Application.ProductVersion, logLocationStr, ConstantsDLL.Properties.Resources.LOG_FILENAME_CP + "-v" + Application.ProductVersion + ConstantsDLL.Properties.Resources.LOG_FILE_EXT, showCLIOutput);
                log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), ConstantsDLL.Properties.Strings.LOG_RELEASE_MODE, string.Empty, showCLIOutput);
#endif
                if (!fileExists)
                {
                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), ConstantsDLL.Properties.Strings.LOGFILE_NOTEXISTS, string.Empty, showCLIOutput);
                }
                else
                {
                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), ConstantsDLL.Properties.Strings.LOGFILE_EXISTS, string.Empty, showCLIOutput);
                }

                //Installs WebView2 Runtime if not found
                log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_CHECKING_WEBVIEW2, string.Empty, showCLIOutput);
                if ((!Directory.Exists(ConstantsDLL.Properties.Resources.WEBVIEW2_SYSTEM_PATH_X64 + MiscMethods.GetWebView2Version())) && (!Directory.Exists(ConstantsDLL.Properties.Resources.WEBVIEW2_SYSTEM_PATH_X86 + MiscMethods.GetWebView2Version())))
                {
                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_WARNING), Strings.LOG_WEBVIEW2_NOT_FOUND, string.Empty, showCLIOutput);
                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_INSTALLING_WEBVIEW2, string.Empty, showCLIOutput);
                    string returnCode = WebView2Installer.Install();
                    if (!int.TryParse(returnCode, out int returnCodeInt))
                    {
                        log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_ERROR), Strings.LOG_WEBVIEW2_INSTALL_FAILED, returnCode, showCLIOutput);
                        Environment.Exit(Convert.ToInt32(ConstantsDLL.Properties.Resources.RETURN_ERROR));
                    }
                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_WEBVIEW2_INSTALLED, string.Empty, showCLIOutput);
                }
                else
                {
                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_WEBVIEW2_ALREADY_INSTALLED, string.Empty, showCLIOutput);
                }

                //If given no args, runs LoginForm
                if (args.Length == 0)
                {
                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_GUI_MODE, string.Empty, showCLIOutput);
                    FreeConsole();
                    Form lForm = new LoginForm(log, parametersListSection, enforcementListSection, orgDataListSection);
                    if (HardwareInfo.GetWinVersion().Equals(ConstantsDLL.Properties.Resources.WINDOWS_10))
                    {
                        DarkNet.Instance.SetWindowThemeForms(lForm, Theme.Auto);
                    }

                    Application.Run(lForm);
                }
                else //If given args, hides password from Console and Log file and runs CLIRegister
                {
                    args.CopyTo(argsLog, 0);
                    int index = Array.IndexOf(argsLog, ConstantsDLL.Properties.Resources.DOUBLE_DASH + StringsAndConstants.CLI_PASSWORD_SWITCH);
                    if (index == -1)
                    {
                        index = Array.FindIndex(argsLog, x => x.StartsWith(ConstantsDLL.Properties.Resources.DOUBLE_DASH + StringsAndConstants.CLI_PASSWORD_SWITCH));
                        if (index != -1)
                        {
                            argsLog[index] = ConstantsDLL.Properties.Resources.DOUBLE_DASH + StringsAndConstants.CLI_PASSWORD_SWITCH + "=" + ConstantsDLL.Properties.Resources.LOG_PASSWORD_PLACEHOLDER;
                        }
                    }
                    else
                    {
                        argsLog[index + 1] = ConstantsDLL.Properties.Resources.LOG_PASSWORD_PLACEHOLDER;
                    }

                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_CLI_MODE, string.Join(" ", argsLog), showCLIOutput);

                    //Parses the args
                    Parser.Default.ParseArguments<Options>(args)
                       .WithParsed(RunOptions);
                    if (args.Length == 1 && args.Contains(ConstantsDLL.Properties.Resources.DOUBLE_DASH + StringsAndConstants.CLI_HELP_SWITCH))
                    {
                        log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_SHOWING_HELP, string.Empty, showCLIOutput);
                        Environment.Exit(Convert.ToInt32(ConstantsDLL.Properties.Resources.RETURN_SUCCESS));
                    }
                    else
                    {
                        log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_ERROR), Strings.ARGS_ERROR, string.Empty, showCLIOutput);
                        Environment.Exit(Convert.ToInt32(ConstantsDLL.Properties.Resources.RETURN_ERROR));
                    }
                }
            }
            catch (ParsingException e) //If definition file was not found
            {
                Console.WriteLine(ConstantsDLL.Properties.Strings.LOG_DEFFILE_NOT_FOUND + ": " + e.Message);
                Console.WriteLine(ConstantsDLL.Properties.Strings.KEY_FINISH);
                Console.ReadLine();
                Environment.Exit(Convert.ToInt32(ConstantsDLL.Properties.Resources.RETURN_ERROR));
            }
            catch (FormatException e) //If definition file was malformed, but the logfile is not created (log path is undefined)
            {
                Console.WriteLine(ConstantsDLL.Properties.Strings.PARAMETER_ERROR + ": " + e.Message);
                Console.WriteLine(ConstantsDLL.Properties.Strings.KEY_FINISH);
                Console.ReadLine();
                Environment.Exit(Convert.ToInt32(ConstantsDLL.Properties.Resources.RETURN_ERROR));
            }
        }
    }
}
