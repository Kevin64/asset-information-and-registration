using CommandLine;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ConstantsDLL;
using JsonFileReaderDLL;
using LogGeneratorDLL;
using System.Linq;
using HardwareInformation.Properties;

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
            [Option("lacre", Required = false, HelpText = StringsAndConstants.cliHelpTextSeal, Default = "")]
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
                    Environment.Exit(2);
                }
            }
            catch
            {
                log.LogWrite(StringsAndConstants.LOG_ERROR, StringsAndConstants.INTRANET_REQUIRED, string.Empty, StringsAndConstants.consoleOutCLI);
                Environment.Exit(2);
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
            string[] argsLog = new string[args.Length];

            Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

#if DEBUG
            log = new LogGenerator(Application.ProductName + " - v" + Application.ProductVersion + "-" + Resources.dev_status, StringsAndConstants.LOG_FILENAME_CP + "-v" + Application.ProductVersion + "-" + Resources.dev_status + StringsAndConstants.LOG_FILE_EXT);
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_DEBUG_MODE, string.Empty, StringsAndConstants.consoleOutGUI);
#else
            log = new LogGenerator(Application.ProductName + " - v" + Application.ProductVersion, StringsAndConstants.LOG_FILENAME_CP + "-v" + Application.ProductVersion + StringsAndConstants.LOG_FILE_EXT);
            log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_RELEASE_MODE, string.Empty, StringsAndConstants.consoleOutGUI);
#endif

            if (args.Length == 0)
            {
                log.LogWrite(StringsAndConstants.LOG_INFO, StringsAndConstants.LOG_GUI_MODE, string.Empty, StringsAndConstants.consoleOutGUI);
                FreeConsole();
                Application.Run(new Form2(log)); //If given no args, runs Form2 (login)
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
    }
}
