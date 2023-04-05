using CommandLine;
using ConstantsDLL;
using Dark.Net;
using HardwareInfoDLL;
using HardwareInformation.Properties;
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

namespace HardwareInformation
{
    public class Program
    {
        private static string logLocationStr, serverIPStr, serverPortStr, themeStr;
        private static string[] logLocationSection, serverListSection, portListSection, themeSection, firmwareTypes, tpmTypes, mediaOperationTypes, secureBootStates, virtualizationTechnologyStates;
        private static string orgFullNameStr, orgAcronymStr, depFullNameStr, depAcronymStr, subDepFullNameStr, subDepAcronymStr;
        private static string orgFullNameSection, orgAcronymSection, depFullNameSection, depAcronymSection, subDepFullNameSection, subDepAcronymSection;

        private static List<string[]> definitionListSection;
        private static List<string> orgDataListSection;
        private static LogGenerator log;

        //Command line switch options specification
        public class Options
        {
            [Option("server", Required = false, HelpText = StringsAndConstants.cliHelpTextServer)]
            public string Servidor { get; set; }

            [Option("port", Required = false, HelpText = StringsAndConstants.cliHelpTextPort)]
            public string Porta { get; set; }

            [Option("mode", Required = false, HelpText = StringsAndConstants.cliHelpTextMode, Default = "m")]
            public string TipoDeServico { get; set; }

            [Option("assetNumber", Required = false, HelpText = StringsAndConstants.cliHelpTextPatrimony, Default = "")]
            public string Patrimonio { get; set; }

            [Option("sealNumber", Required = false, HelpText = StringsAndConstants.cliHelpTextSeal, Default = "same")]
            public string Lacre { get; set; }

            [Option("roomNumber", Required = false, HelpText = StringsAndConstants.cliHelpTextRoom, Default = "same")]
            public string Sala { get; set; }

            [Option("building", Required = false, HelpText = StringsAndConstants.cliHelpTextBuilding, Default = "same")]
            public string Predio { get; set; }

            [Option("ad", Required = false, HelpText = StringsAndConstants.cliHelpTextActiveDirectory, Default = "same")]
            public string AD { get; set; }

            [Option("standard", Required = false, HelpText = StringsAndConstants.cliHelpTextStandard, Default = "same")]
            public string Padrao { get; set; }

            [Option("date", Required = false, HelpText = StringsAndConstants.cliHelpTextDate, Default = "today")]
            public string Data { get; set; }

            [Option("battery", Required = true, HelpText = StringsAndConstants.cliHelpTextBattery)]
            public string Pilha { get; set; }

            [Option("ticket", Required = true, HelpText = StringsAndConstants.cliHelpTextTicket)]
            public string Ticket { get; set; }

            [Option("inUse", Required = false, HelpText = StringsAndConstants.cliHelpTextInUse, Default = "same")]
            public string Uso { get; set; }

            [Option("tag", Required = false, HelpText = StringsAndConstants.cliHelpTextTag, Default = "same")]
            public string Etiqueta { get; set; }

            [Option("type", Required = false, HelpText = StringsAndConstants.cliHelpTextType, Default = "same")]
            public string TipoHardware { get; set; }

            [Option("username", Required = true, HelpText = StringsAndConstants.cliHelpTextUser)]
            public string Usuario { get; set; }

            [Option("password", Required = true, HelpText = StringsAndConstants.cliHelpTextPassword)]
            public string Senha { get; set; }
        }

        //Passes args to auth method and then to register class, otherwise informs auth error and closes the program
        public static void RunOptions(Options opts)
        {
            if (opts.Servidor == null)
            {
                opts.Servidor = serverListSection[0];
            }

            if (opts.Porta == null)
            {
                opts.Porta = portListSection[0];
            }

            log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), ConstantsDLL.Properties.Strings.LOG_INIT_LOGIN, opts.Usuario, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
            string[] str = CredentialsFileReader.FetchInfoST(opts.Usuario, opts.Senha, opts.Servidor, opts.Porta);
            try
            {
                if (str[0] == "true")
                {
                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), ConstantsDLL.Properties.Strings.LOG_LOGIN_SUCCESS, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
                    Application.Run(new CLIRegister(opts.Servidor, opts.Porta, opts.TipoDeServico, opts.Patrimonio, opts.Lacre, opts.Sala, opts.Predio, opts.AD, opts.Padrao, opts.Data, opts.Pilha, opts.Ticket, opts.Uso, opts.Etiqueta, opts.TipoHardware, opts.Usuario, log, definitionListSection));
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
            if (HardwareInfo.GetOSInfoAux().Equals(ConstantsDLL.Properties.Resources.windows10))
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

                //Reads the INI file Definition section
                logLocationStr = def[ConstantsDLL.Properties.Resources.INI_SECTION_1][ConstantsDLL.Properties.Resources.INI_SECTION_1_9];
                serverIPStr = def[ConstantsDLL.Properties.Resources.INI_SECTION_1][ConstantsDLL.Properties.Resources.INI_SECTION_1_11];
                serverPortStr = def[ConstantsDLL.Properties.Resources.INI_SECTION_1][ConstantsDLL.Properties.Resources.INI_SECTION_1_12];
                themeStr = def[ConstantsDLL.Properties.Resources.INI_SECTION_1][ConstantsDLL.Properties.Resources.INI_SECTION_1_15];

                orgFullNameStr = def[ConstantsDLL.Properties.Resources.INI_SECTION_2][ConstantsDLL.Properties.Resources.INI_SECTION_2_1];
                orgAcronymStr = def[ConstantsDLL.Properties.Resources.INI_SECTION_2][ConstantsDLL.Properties.Resources.INI_SECTION_2_2];
                depFullNameStr = def[ConstantsDLL.Properties.Resources.INI_SECTION_2][ConstantsDLL.Properties.Resources.INI_SECTION_2_3];
                depAcronymStr = def[ConstantsDLL.Properties.Resources.INI_SECTION_2][ConstantsDLL.Properties.Resources.INI_SECTION_2_4];
                subDepFullNameStr = def[ConstantsDLL.Properties.Resources.INI_SECTION_2][ConstantsDLL.Properties.Resources.INI_SECTION_2_5];
                subDepAcronymStr = def[ConstantsDLL.Properties.Resources.INI_SECTION_2][ConstantsDLL.Properties.Resources.INI_SECTION_2_6];

                logLocationSection = logLocationStr.Split().ToArray();
                serverListSection = serverIPStr.Split(',').ToArray();
                portListSection = serverPortStr.Split(',').ToArray();
                themeSection = themeStr.Split().ToArray();
                firmwareTypes = tpmTypes = mediaOperationTypes = secureBootStates = virtualizationTechnologyStates = null;

                if (!StringsAndConstants.listThemeGUI.Contains(themeSection[0]))
                {
                    throw new FormatException();
                }

                orgFullNameSection = orgFullNameStr;
                orgAcronymSection = orgAcronymStr;
                depFullNameSection = depFullNameStr;
                depAcronymSection = depAcronymStr;
                subDepFullNameSection = subDepFullNameStr;
                subDepAcronymSection = subDepAcronymStr;

                definitionListSection = new List<string[]>
                {
                    serverListSection,
                    portListSection,
                    logLocationSection,
                    themeSection,
                    firmwareTypes,
                    tpmTypes,
                    mediaOperationTypes,
                    secureBootStates,
                    virtualizationTechnologyStates
                };

                orgDataListSection = new List<string>
                {
                    orgFullNameSection,
                    orgAcronymSection,
                    depFullNameSection,
                    depAcronymSection,
                    subDepFullNameSection,
                    subDepAcronymSection
                };

                bool fileExists = bool.Parse(MiscMethods.CheckIfLogExists(logLocationStr));
#if DEBUG
                //Create a new log file (or append to a existing one)
                log = new LogGenerator(Application.ProductName + " - v" + Application.ProductVersion + "-" + Resources.dev_status, logLocationStr, ConstantsDLL.Properties.Resources.LOG_FILENAME_CP + "-v" + Application.ProductVersion + "-" + Resources.dev_status + ConstantsDLL.Properties.Resources.LOG_FILE_EXT, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
                log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), ConstantsDLL.Properties.Strings.LOG_DEBUG_MODE, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
#else
                //Create a new log file (or append to a existing one)
                log = new LogGenerator(Application.ProductName + " - v" + Application.ProductVersion, logLocationStr, ConstantsDLL.Properties.Resources.LOG_FILENAME_CP + "-v" + Application.ProductVersion + ConstantsDLL.Properties.Resources.LOG_FILE_EXT, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
                log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), ConstantsDLL.Properties.Strings.LOG_RELEASE_MODE, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
#endif
                if (!fileExists)
                {
                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), ConstantsDLL.Properties.Strings.LOGFILE_NOTEXISTS, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
                }
                else
                {
                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), ConstantsDLL.Properties.Strings.LOGFILE_EXISTS, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
                }

                //Installs WebView2 Runtime if not found
                log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_CHECKING_WEBVIEW2, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
                if ((!Directory.Exists(ConstantsDLL.Properties.Resources.WEBVIEW2_SYSTEM_PATH_X64 + MiscMethods.GetWebView2Version())) && (!Directory.Exists(ConstantsDLL.Properties.Resources.WEBVIEW2_SYSTEM_PATH_X86 + MiscMethods.GetWebView2Version())))
                {
                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_WARNING), Strings.LOG_WEBVIEW2_NOT_FOUND, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_INSTALLING_WEBVIEW2, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
                    string returnCode = WebView2Installer.Install();
                    if (!int.TryParse(returnCode, out int returnCodeInt))
                    {
                        log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_ERROR), Strings.LOG_WEBVIEW2_INSTALL_FAILED, returnCode, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
                        Environment.Exit(Convert.ToInt32(ConstantsDLL.Properties.Resources.RETURN_ERROR));
                    }
                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_WEBVIEW2_INSTALLED, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
                }
                else
                {
                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_WEBVIEW2_ALREADY_INSTALLED, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
                }

                //If given no args, runs LoginForm
                if (args.Length == 0)
                {
                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_GUI_MODE, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutGUI));
                    FreeConsole();
                    Form lForm = new LoginForm(log, definitionListSection, orgDataListSection);
                    if (HardwareInfo.GetOSInfoAux().Equals(ConstantsDLL.Properties.Resources.windows10))
                    {
                        DarkNet.Instance.SetWindowThemeForms(lForm, Theme.Auto);
                    }

                    Application.Run(lForm);
                }
                else //If given args, hides password from Console and Log file and runs CLIRegister
                {
                    args.CopyTo(argsLog, 0);
                    int index = Array.IndexOf(argsLog, "--senha");
                    if (index == -1)
                    {
                        index = Array.FindIndex(argsLog, x => x.StartsWith("--senha"));
                        if (index != -1)
                        {
                            argsLog[index] = "--senha=" + ConstantsDLL.Properties.Resources.LOG_PASSWORD_PLACEHOLDER;
                        }
                    }
                    else
                    {
                        argsLog[index + 1] = ConstantsDLL.Properties.Resources.LOG_PASSWORD_PLACEHOLDER;
                    }

                    log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_CLI_MODE, string.Join(" ", argsLog), Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));

                    //Parses the args
                    Parser.Default.ParseArguments<Options>(args)
                       .WithParsed(RunOptions);
                    if (args.Length == 1 && args.Contains("--help"))
                    {
                        log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_INFO), Strings.LOG_SHOWING_HELP, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
                        Environment.Exit(Convert.ToInt32(ConstantsDLL.Properties.Resources.RETURN_SUCCESS));
                    }
                    else
                    {
                        log.LogWrite(Convert.ToInt32(ConstantsDLL.Properties.Resources.LOG_ERROR), Strings.ARGS_ERROR, string.Empty, Convert.ToBoolean(ConstantsDLL.Properties.Resources.consoleOutCLI));
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
