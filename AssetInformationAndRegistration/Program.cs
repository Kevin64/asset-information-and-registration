﻿using CommandLine;
using ConstantsDLL;
using Dark.Net;
using HardwareInfoDLL;
using AssetInformationAndRegistration.Properties;
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
    public class Program
    {
        private static string logLocationStr, serverIPStr, serverPortStr, themeStr, secureBootEnforcementStr, vtEnforcementStr, tpmEnforcementStr, firmwareVersionEnforcementStr, firmwareTypeEnforcementStr, hostnameEnforcementStr, mediaOperationModeEnforcementStr, smartStatusEnforcementStr, ramLimitEnforcementStr, orgFullNameStr, orgAcronymStr, depFullNameStr, depAcronymStr, subDepFullNameStr, subDepAcronymStr;
        private static string[] logLocationSection, serverIPListSection, serverPortListSection, themeSection, buildings, hardwareTypes, firmwareTypes, tpmTypes, mediaOperationTypes, secureBootStates, virtualizationTechnologyStates;

        private static bool showCLIOutput;
        private static List<string[]> parametersListSection;
        private static List<string> orgDataListSection, enforcementListSection;
        private static LogGenerator log;

        //Command line switch options specification
        public class Options
        {
            [Option(StringsAndConstants.cliServerIPSwitch, Required = false, HelpText = StringsAndConstants.cliHelpTextServerIP)]
            public string ServerIP { get; set; }

            [Option(StringsAndConstants.cliServerPortSwitch, Required = false, HelpText = StringsAndConstants.cliHelpTextServerPort)]
            public string ServerPort { get; set; }

            [Option(StringsAndConstants.cliServiceTypeSwitch, Required = false, HelpText = StringsAndConstants.cliHelpTextServiceType, Default = StringsAndConstants.cliDefaultServiceType)]
            public string ServiceType { get; set; }

            [Option(StringsAndConstants.cliAssetNumberSwitch, Required = false, HelpText = StringsAndConstants.cliHelpTextAssetNumber, Default = "")]
            public string AssetNumber { get; set; }

            [Option(StringsAndConstants.cliSealNumberSwitch, Required = false, HelpText = StringsAndConstants.cliHelpTextSealNumber, Default = StringsAndConstants.cliDefaultUnchanged)]
            public string SealNumber { get; set; }

            [Option(StringsAndConstants.cliRoomNumberSwitch, Required = false, HelpText = StringsAndConstants.cliHelpTextRoomNumber, Default = StringsAndConstants.cliDefaultUnchanged)]
            public string RoomNumber { get; set; }

            [Option(StringsAndConstants.cliBuildingSwitch, Required = false, HelpText = StringsAndConstants.cliHelpTextBuilding, Default = StringsAndConstants.cliDefaultUnchanged)]
            public string Building { get; set; }

            [Option(StringsAndConstants.cliAdRegisteredSwitch, Required = false, HelpText = StringsAndConstants.cliHelpTextAdRegistered, Default = StringsAndConstants.cliDefaultUnchanged)]
            public string AdRegistered { get; set; }

            [Option(StringsAndConstants.cliStandardSwitch, Required = false, HelpText = StringsAndConstants.cliHelpTextStandard, Default = StringsAndConstants.cliDefaultUnchanged)]
            public string Standard { get; set; }

            [Option(StringsAndConstants.cliServiceDateSwitch, Required = false, HelpText = StringsAndConstants.cliHelpTextServiceDate, Default = StringsAndConstants.cliDefaultServiceDate)]
            public string ServiceDate { get; set; }

            [Option(StringsAndConstants.cliBatteryChangeSwitch, Required = true, HelpText = StringsAndConstants.cliHelpTextBatteryChange)]
            public string BatteryChange { get; set; }

            [Option(StringsAndConstants.cliTicketNumberSwitch, Required = true, HelpText = StringsAndConstants.cliHelpTextTicketNumber)]
            public string TicketNumber { get; set; }

            [Option(StringsAndConstants.cliInUseSwitch, Required = false, HelpText = StringsAndConstants.cliHelpTextInUse, Default = StringsAndConstants.cliDefaultUnchanged)]
            public string InUse { get; set; }

            [Option(StringsAndConstants.cliTagSwitch, Required = false, HelpText = StringsAndConstants.cliHelpTextTag, Default = StringsAndConstants.cliDefaultUnchanged)]
            public string Tag { get; set; }

            [Option(StringsAndConstants.cliHwTypeSwitch, Required = false, HelpText = StringsAndConstants.cliHelpTextHwType, Default = StringsAndConstants.cliDefaultUnchanged)]
            public string HwType { get; set; }

            [Option(StringsAndConstants.cliUsernameSwitch, Required = true, HelpText = StringsAndConstants.cliHelpTextUsername)]
            public string Username { get; set; }

            [Option(StringsAndConstants.cliPasswordSwitch, Required = true, HelpText = StringsAndConstants.cliHelpTextPassword)]
            public string Password { get; set; }
        }

        //Passes args to auth method and then to register class, otherwise informs auth error and closes the program
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

            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), ConstantsDLL.Properties.Strings.LOG_INIT_LOGIN, opts.Username, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
            string[] agentsJsonStr = CredentialsFileReader.FetchInfoST(opts.Username, opts.Password, opts.ServerIP, opts.ServerPort);
            try
            {
                if (agentsJsonStr[0] != "false")
                {
                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), ConstantsDLL.Properties.Strings.LOG_LOGIN_SUCCESS, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
                    Application.Run(new CLIRegister(opts.ServerIP, opts.ServerPort, opts.ServiceType, opts.AssetNumber, opts.SealNumber, opts.RoomNumber, opts.Building, opts.AdRegistered, opts.Standard, opts.ServiceDate, opts.BatteryChange, opts.TicketNumber, opts.InUse, opts.Tag, opts.HwType, agentsJsonStr, log, parametersListSection, enforcementListSection));
                }
                else
                {
                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_ERROR), Strings.AUTH_ERROR, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
                    Environment.Exit(Convert.ToInt32(ConstantsDLL.Properties.Resources.RETURN_ERROR));
                }
            }
            catch
            {
                log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_ERROR), ConstantsDLL.Properties.Strings.INTRANET_REQUIRED, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
                Environment.Exit(Convert.ToInt32(ConstantsDLL.Properties.Resources.RETURN_ERROR));
            }
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern int FreeConsole();

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            //Code for testing string localization for other languages
            //var culture = System.Globalization.CultureInfo.GetCultureInfo("en");
            //System.Globalization.CultureInfo.DefaultThreadCurrentCulture = culture;
            //System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = culture;

            if (HardwareInfo.GetWinVersion().Equals(ConstantsDLL.Properties.Resources.windows10))
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
                def = parser.ReadFile(ConstantsDLL.Properties.Resources.defFile, Encoding.UTF8);

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

                if (!StringsAndConstants.listThemeGUI.Contains(themeSection[0]))
                {
                    throw new FormatException();
                }

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

                orgDataListSection = new List<string>
                {
                    orgFullNameStr,
                    orgAcronymStr,
                    depFullNameStr,
                    depAcronymStr,
                    subDepFullNameStr,
                    subDepAcronymStr
                };

                bool fileExists = bool.Parse(MiscMethods.CheckIfLogExists(logLocationStr));

                //If args has a --help switch, do not show log output
                if (!args.Contains(ConstantsDLL.Properties.Resources.DOUBLE_DASH + StringsAndConstants.cliHelpSwitch))
                {
                    showCLIOutput = true;
                }
#if DEBUG
                //Create a new log file (or append to a existing one)
                log = new LogGenerator(Application.ProductName + " - v" + Application.ProductVersion + "-" + Resources.dev_status, logLocationStr, ConstantsDLL.Properties.Resources.LOG_FILENAME_CP + "-v" + Application.ProductVersion + "-" + Resources.dev_status + ConstantsDLL.Properties.Resources.LOG_FILE_EXT, showCLIOutput);
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
                    if (HardwareInfo.GetWinVersion().Equals(ConstantsDLL.Properties.Resources.windows10))
                    {
                        DarkNet.Instance.SetWindowThemeForms(lForm, Theme.Auto);
                    }

                    Application.Run(lForm);
                }
                else //If given args, hides password from Console and Log file and runs CLIRegister
                {
                    args.CopyTo(argsLog, 0);
                    int index = Array.IndexOf(argsLog, ConstantsDLL.Properties.Resources.DOUBLE_DASH + StringsAndConstants.cliPasswordSwitch);
                    if (index == -1)
                    {
                        index = Array.FindIndex(argsLog, x => x.StartsWith(ConstantsDLL.Properties.Resources.DOUBLE_DASH + StringsAndConstants.cliPasswordSwitch));
                        if (index != -1)
                        {
                            argsLog[index] = ConstantsDLL.Properties.Resources.DOUBLE_DASH + StringsAndConstants.cliPasswordSwitch + "=" + ConstantsDLL.Properties.Resources.LOG_PASSWORD_PLACEHOLDER;
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
                    if (args.Length == 1 && args.Contains(ConstantsDLL.Properties.Resources.DOUBLE_DASH + StringsAndConstants.cliHelpSwitch))
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