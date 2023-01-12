﻿using CommandLine;
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

namespace HardwareInformation
{
	public class Program
    {
        private static LogGenerator log;
        //Command line switch options specification
        public class Options
        {
            [Option("servidor", Required = false, HelpText = StringsAndConstants.cliHelpTextServer, Default = "192.168.76.103")]
            public string Servidor { get; set; }
            [Option("porta", Required = false, HelpText = StringsAndConstants.cliHelpTextPort, Default = "8081")]
            public string Porta { get; set; }
            [Option("modo", Required = false, HelpText = StringsAndConstants.cliHelpTextMode, Default = "m")]
            public string TipoDeServico { get; set; }
            [Option("patrimonio", Required = true, HelpText = StringsAndConstants.cliHelpTextPatrimony)]
            public string Patrimonio { get; set; }
            [Option("lacre", Required = false, HelpText = StringsAndConstants.cliHelpTextSeal, Default = "mesmo")]
            public string Lacre { get; set; }
            [Option("sala", Required = true, HelpText = StringsAndConstants.cliHelpTextRoom)]
            public string Sala { get; set; }
            [Option("predio", Required = true, HelpText = StringsAndConstants.cliHelpTextBuilding)]
            public string Predio { get; set; }
            [Option("ad", Required = false, HelpText = StringsAndConstants.cliHelpTextActiveDirectory, Default = "Sim")]
            public string AD { get; set; }
            [Option("padrao", Required = false, HelpText = StringsAndConstants.cliHelpTextStandard, Default = "Aluno")]
            public string Padrao { get; set; }
            [Option("data", Required = false, HelpText = StringsAndConstants.cliHelpTextDate, Default = "hoje")]
            public string Data { get; set; }
            [Option("pilha", Required = true, HelpText = StringsAndConstants.cliHelpTextBattery)]
            public string Pilha { get; set; }
            [Option("ticket", Required = true, HelpText = StringsAndConstants.cliHelpTextTicket)]
            public string Ticket { get; set; }
            [Option("uso", Required = false, HelpText = StringsAndConstants.cliHelpTextInUse, Default = "Sim")]
            public string Uso { get; set; }
            [Option("etiqueta", Required = true, HelpText = StringsAndConstants.cliHelpTextTag)]
            public string Etiqueta { get; set; }
            [Option("tipo", Required = false, HelpText = StringsAndConstants.cliHelpTextType, Default = "Desktop")]
            public string TipoHardware { get; set; }
            [Option("usuario", Required = true, HelpText = StringsAndConstants.cliHelpTextUser)]
            public string Usuario { get; set; }
            [Option("senha", Required = true, HelpText = StringsAndConstants.cliHelpTextPassword)]
            public string Senha { get; set; }
        }

        //Passes args to auth method and then to register class, otherwise informs auth error and closes the program
        public static void RunOptions(Options opts)
        {
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_INIT_LOGIN, opts.Usuario, StringsAndConstants.consoleOutCLI);
            string[] str = LoginFileReader.fetchInfoST(opts.Usuario, opts.Senha, opts.Servidor, opts.Porta);
            try
            {
                if (str[0] == "true")
                {
                    log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_LOGIN_SUCCESS, string.Empty, StringsAndConstants.consoleOutCLI);
                    Application.Run(new CLIRegister(opts.Servidor, opts.Porta, opts.TipoDeServico, opts.Patrimonio, opts.Lacre, opts.Sala, opts.Predio, opts.AD, opts.Padrao, opts.Data, opts.Pilha, opts.Ticket, opts.Uso, opts.Etiqueta, opts.TipoHardware, opts.Usuario, log));
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
                def = parser.ReadFile(StringsAndConstants.defFile);
                //Reads the INI file section
                var logLocationStr = def[StringsAndConstants.INI_SECTION_1][StringsAndConstants.INI_SECTION_1_9];

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
                    Application.Run(new LoginForm(log)); //If given no args, runs LoginForm
                }
                else
                {
                    args.CopyTo(argsLog, 0);
                    int index = Array.IndexOf(argsLog, "--senha");
                    argsLog[index + 1] = StringsAndConstants.LOG_PASSWORD_PLACEHOLDER;
                    log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_CLI_MODE, string.Join(" ", argsLog), StringsAndConstants.consoleOutCLI);
                    //If given args, parses them
                    Parser.Default.ParseArguments<Options>(args)
                       .WithParsed(RunOptions);
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
