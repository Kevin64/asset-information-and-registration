using CommandLine;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ConstantsDLL;
using JsonFileReaderDLL;
using LogGeneratorDLL;
using HardwareInformation.Properties;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Reflection;
using IniParser.Model;
using IniParser;
using IniParser.Exceptions;
using System.Collections.Generic;
using System.Text;

namespace HardwareInformation
{
	public class Program
    {
        private static List<string[]> definitionListSection;
        private static List<string> orgDataListSection;
        private static LogGenerator log;
        private static string logLocationStr, serverIPStr, serverPortStr, roomListStr, hwTypeListStr, themeStr;
        private static string[] logLocationSection, serverListSection, portListSection, roomListSection, hwTypeListSection, themeSection;

        private static string orgFullNameStr, orgAcronymStr, depFullNameStr, depAcronymStr, subDepFullNameStr, subDepAcronymStr;
        private static string orgFullNameSection, orgAcronymSection, depFullNameSection, depAcronymSection, subDepFullNameSection, subDepAcronymSection;
        
        //Command line switch options specification
        public class Options
        {
            [Option("servidor", Required = false, HelpText = StringsAndConstants.cliHelpTextServer)]
            public string Servidor { get; set; }

            [Option("porta", Required = false, HelpText = StringsAndConstants.cliHelpTextPort)]
            public string Porta { get; set; }
            
            [Option("modo", Required = false, HelpText = StringsAndConstants.cliHelpTextMode, Default = "m")]
            public string TipoDeServico { get; set; }
            
            [Option("patrimonio", Required = false, HelpText = StringsAndConstants.cliHelpTextPatrimony, Default = "")]
            public string Patrimonio { get; set; }
            
            [Option("lacre", Required = false, HelpText = StringsAndConstants.cliHelpTextSeal, Default = "mesmo")]
            public string Lacre { get; set; }
            
            [Option("sala", Required = false, HelpText = StringsAndConstants.cliHelpTextRoom, Default = "mesmo")]
            public string Sala { get; set; }
            
            [Option("predio", Required = false, HelpText = StringsAndConstants.cliHelpTextBuilding, Default = "mesmo")]
            public string Predio { get; set; }
            
            [Option("ad", Required = false, HelpText = StringsAndConstants.cliHelpTextActiveDirectory, Default = "mesmo")]
            public string AD { get; set; }
            
            [Option("padrao", Required = false, HelpText = StringsAndConstants.cliHelpTextStandard, Default = "mesmo")]
            public string Padrao { get; set; }
            
            [Option("data", Required = false, HelpText = StringsAndConstants.cliHelpTextDate, Default = "hoje")]
            public string Data { get; set; }
            
            [Option("pilha", Required = true, HelpText = StringsAndConstants.cliHelpTextBattery)]
            public string Pilha { get; set; }
            
            [Option("ticket", Required = true, HelpText = StringsAndConstants.cliHelpTextTicket)]
            public string Ticket { get; set; }
            
            [Option("uso", Required = false, HelpText = StringsAndConstants.cliHelpTextInUse, Default = "mesmo")]
            public string Uso { get; set; }
            
            [Option("etiqueta", Required = false, HelpText = StringsAndConstants.cliHelpTextTag, Default = "mesmo")]
            public string Etiqueta { get; set; }
            
            [Option("tipo", Required = false, HelpText = StringsAndConstants.cliHelpTextType, Default = "mesmo")]
            public string TipoHardware { get; set; }
            
            [Option("usuario", Required = true, HelpText = StringsAndConstants.cliHelpTextUser)]
            public string Usuario { get; set; }
            
            [Option("senha", Required = true, HelpText = StringsAndConstants.cliHelpTextPassword)]
            public string Senha { get; set; }
        }

        //Passes args to auth method and then to register class, otherwise informs auth error and closes the program
        public static void RunOptions(Options opts)
        {
            if(opts.Servidor == null)
                opts.Servidor = serverListSection[0];
            if (opts.Porta == null)
                opts.Porta = portListSection[0];
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_INIT_LOGIN, opts.Usuario, StringsAndConstants.consoleOutCLI);
            string[] str = LoginFileReader.fetchInfoST(opts.Usuario, opts.Senha, opts.Servidor, opts.Porta);
            try
            {
                if (str[0] == "true")
                {
                    log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_LOGIN_SUCCESS, string.Empty, StringsAndConstants.consoleOutCLI);
                    Application.Run(new CLIRegister(opts.Servidor, opts.Porta, opts.TipoDeServico, opts.Patrimonio, opts.Lacre, opts.Sala, opts.Predio, opts.AD, opts.Padrao, opts.Data, opts.Pilha, opts.Ticket, opts.Uso, opts.Etiqueta, opts.TipoHardware, opts.Usuario, log, definitionListSection));
                }
                else
                {
                    log.LogWrite(StringsAndConstants.LOG_ERROR, StringsAndConstants.AUTH_ERROR, string.Empty, StringsAndConstants.consoleOutCLI);
                    Environment.Exit(StringsAndConstants.RETURN_ERROR);
                }
            }
            catch
            {
                log.LogWrite(StringsAndConstants.LOG_ERROR, StringsAndConstants.INTRANET_REQUIRED, string.Empty, StringsAndConstants.consoleOutCLI);
                Environment.Exit(StringsAndConstants.RETURN_ERROR);
            }
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern int FreeConsole();

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
		static void Main(string[] args)
		{
            //Check if application is running
            if (Process.GetProcessesByName(Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location)).Count() > 1)
            {
                MessageBox.Show(StringsAndConstants.ALREADY_RUNNING, StringsAndConstants.ERROR_WINDOWTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Process.GetCurrentProcess().Kill();
            }

            string[] argsLog = new string[args.Length];

            Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                IniData def = null;
                var parser = new FileIniDataParser();
                //Parses the INI file
                def = parser.ReadFile(StringsAndConstants.defFile, Encoding.UTF8);
                
                //Reads the INI file Definition section
                logLocationStr = def[StringsAndConstants.INI_SECTION_1][StringsAndConstants.INI_SECTION_1_9];
                serverIPStr = def[StringsAndConstants.INI_SECTION_1][StringsAndConstants.INI_SECTION_1_11];
                serverPortStr = def[StringsAndConstants.INI_SECTION_1][StringsAndConstants.INI_SECTION_1_12];
                roomListStr = def[StringsAndConstants.INI_SECTION_1][StringsAndConstants.INI_SECTION_1_13];
                hwTypeListStr = def[StringsAndConstants.INI_SECTION_1][StringsAndConstants.INI_SECTION_1_14];
                themeStr = def[StringsAndConstants.INI_SECTION_1][StringsAndConstants.INI_SECTION_1_15];

                orgFullNameStr = def[StringsAndConstants.INI_SECTION_2][StringsAndConstants.INI_SECTION_2_1];
                orgAcronymStr = def[StringsAndConstants.INI_SECTION_2][StringsAndConstants.INI_SECTION_2_2];
                depFullNameStr = def[StringsAndConstants.INI_SECTION_2][StringsAndConstants.INI_SECTION_2_3];
                depAcronymStr = def[StringsAndConstants.INI_SECTION_2][StringsAndConstants.INI_SECTION_2_4];
                subDepFullNameStr = def[StringsAndConstants.INI_SECTION_2][StringsAndConstants.INI_SECTION_2_5];
                subDepAcronymStr = def[StringsAndConstants.INI_SECTION_2][StringsAndConstants.INI_SECTION_2_6];

                logLocationSection = logLocationStr.Split().ToArray();
                serverListSection = serverIPStr.Split(',').ToArray();
                portListSection = serverPortStr.Split(',').ToArray();
                roomListSection = roomListStr.Split(',').ToArray();
                hwTypeListSection = hwTypeListStr.Split(',').ToArray();
                themeSection = themeStr.Split().ToArray();
                    
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
                    roomListSection,
                    hwTypeListSection,
                    logLocationSection,
                    themeSection
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

                bool fileExists = bool.Parse(MiscMethods.checkIfLogExists(logLocationStr));
#if DEBUG
                //Create a new log file (or append to a existing one)
                log = new LogGenerator(Application.ProductName + " - v" + Application.ProductVersion + "-" + Resources.dev_status, logLocationStr, StringsAndConstants.LOG_FILENAME_CP + "-v" + Application.ProductVersion + "-" + Resources.dev_status + StringsAndConstants.LOG_FILE_EXT, StringsAndConstants.consoleOutCLI);
                log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_DEBUG_MODE, string.Empty, StringsAndConstants.consoleOutCLI);
#else
                //Create a new log file (or append to a existing one)
                log = new LogGenerator(Application.ProductName + " - v" + Application.ProductVersion, logLocationStr, StringsAndConstants.LOG_FILENAME_CP + "-v" + Application.ProductVersion + StringsAndConstants.LOG_FILE_EXT, StringsAndConstants.consoleOutCLI);
                log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_RELEASE_MODE, string.Empty, StringsAndConstants.consoleOutCLI);
#endif
                if (!fileExists)
                    log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOGFILE_NOTEXISTS, string.Empty, StringsAndConstants.consoleOutCLI);
                else
                    log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOGFILE_EXISTS, string.Empty, StringsAndConstants.consoleOutCLI);

                //Installs WebView2 Runtime if not found
                log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_CHECKING_WEBVIEW2, string.Empty, StringsAndConstants.consoleOutCLI);
                if ((!Directory.Exists(StringsAndConstants.WEBVIEW2_SYSTEM_PATH_X64 + MiscMethods.getWebView2Version())) && (!Directory.Exists(StringsAndConstants.WEBVIEW2_SYSTEM_PATH_X86 + MiscMethods.getWebView2Version())))
                {
                    log.LogWrite(StringsAndConstants.LOG_WARNING, StringsAndConstants.LOG_WEBVIEW2_NOT_FOUND, string.Empty, StringsAndConstants.consoleOutCLI);
                    log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_INSTALLING_WEBVIEW2, string.Empty, StringsAndConstants.consoleOutCLI);
                    var returnCode = WebView2Installer.install();
                    int returnCodeInt;
                    if (!int.TryParse(returnCode, out returnCodeInt))
                    {
                        log.LogWrite(StringsAndConstants.LOG_ERROR, StringsAndConstants.LOG_WEBVIEW2_INSTALL_FAILED, returnCode, StringsAndConstants.consoleOutCLI);
                        Environment.Exit(StringsAndConstants.RETURN_ERROR);
                    }
                    log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_WEBVIEW2_INSTALLED, string.Empty, StringsAndConstants.consoleOutCLI);
                }
                else
                    log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_WEBVIEW2_ALREADY_INSTALLED, string.Empty, StringsAndConstants.consoleOutCLI);

                if (args.Length == 0)
                {
                    log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_GUI_MODE, string.Empty, StringsAndConstants.consoleOutGUI);
                    FreeConsole();
                    Application.Run(new LoginForm(log, definitionListSection, orgDataListSection)); //If given no args, runs LoginForm
                }
                else
                {
                    args.CopyTo(argsLog, 0);
                    int index = Array.IndexOf(argsLog, "--senha");
                    if (index == -1)
                    {
                        index = Array.FindIndex(argsLog, x => x.StartsWith("--senha"));
                        if(index != -1)
                            argsLog[index] = "--senha=" + StringsAndConstants.LOG_PASSWORD_PLACEHOLDER;
                    }
                    else
                        argsLog[index + 1] = StringsAndConstants.LOG_PASSWORD_PLACEHOLDER;
                    
                    log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_CLI_MODE, string.Join(" ", argsLog), StringsAndConstants.consoleOutCLI);
                    //If given args, parses them
                    Parser.Default.ParseArguments<Options>(args)
                       .WithParsed(RunOptions);
                    if (args.Length == 1 && args.Contains("--help"))
                    {
                        log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_SHOWING_HELP, string.Empty, StringsAndConstants.consoleOutCLI);
                        Environment.Exit(StringsAndConstants.RETURN_SUCCESS);
                    }
                    else
                    {
                        log.LogWrite(StringsAndConstants.LOG_ERROR, StringsAndConstants.LOG_ARGS_ERROR, string.Empty, StringsAndConstants.consoleOutCLI);
                        Environment.Exit(StringsAndConstants.RETURN_ERROR);
                    }
                }
            }
            catch (ParsingException e) //If definition file was not found
            {
                Console.WriteLine(StringsAndConstants.LOG_DEFFILE_NOT_FOUND + ": " + e.Message);
                Console.WriteLine(StringsAndConstants.KEY_FINISH);
                Console.ReadLine();
                Environment.Exit(StringsAndConstants.RETURN_ERROR);
            }
            catch (FormatException e) //If definition file was malformed, but the logfile is not created (log path is undefined)
            {
                Console.WriteLine(StringsAndConstants.PARAMETER_ERROR + ": " + e.Message);
                Console.WriteLine(StringsAndConstants.KEY_FINISH);
                Console.ReadLine();
                Environment.Exit(StringsAndConstants.RETURN_ERROR);
            }
        }
    }
}
